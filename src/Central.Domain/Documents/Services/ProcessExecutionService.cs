using System.Text.Json;

using Azure;
using Azure.AI.DocumentIntelligence;
using Azure.AI.OpenAI;

using Central.Domain.Contracts.Ports;
using Central.Domain.Correspondents.Ports;
using Central.Domain.Documents.Ports;
using Central.Domain.DocumentTypes.Ports;
using Central.Domain.Tags.Ports;

using Microsoft.Extensions.Logging;

using OpenAI.Chat;

namespace Central.Domain.Documents.Services;

/// <summary>
/// Service implementation for executing document processing workflows.
/// </summary>
public sealed class ProcessExecutionService : IProcessExecutionService
{
    private readonly IProcessDefinitionRepository _processDefinitionRepository;
    private readonly IProcessExecutionRepository _processExecutionRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly ICorrespondentRepository _correspondentRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<ProcessExecutionService> _logger;

    public ProcessExecutionService(
        IProcessDefinitionRepository processDefinitionRepository,
        IProcessExecutionRepository processExecutionRepository,
        IDocumentRepository documentRepository,
        IContractRepository contractRepository,
        IDocumentTypeRepository documentTypeRepository,
        ICorrespondentRepository correspondentRepository,
        ITagRepository tagRepository,
        ILogger<ProcessExecutionService> logger)
    {
        _processDefinitionRepository = processDefinitionRepository;
        _processExecutionRepository = processExecutionRepository;
        _documentRepository = documentRepository;
        _contractRepository = contractRepository;
        _documentTypeRepository = documentTypeRepository;
        _correspondentRepository = correspondentRepository;
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<ProcessExecution> ExecuteProcessAsync(
        long processDefinitionId,
        long documentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Starting process execution. ProcessDefinitionId={ProcessDefinitionId}, DocumentId={DocumentId}",
            processDefinitionId,
            documentId);

        // Load process definition
        var processDefinition = await _processDefinitionRepository.GetByIdAsync(processDefinitionId, cancellationToken);
        if (processDefinition == null)
        {
            throw new InvalidOperationException($"Process definition {processDefinitionId} not found.");
        }

        // Load document
        var document = await _documentRepository.GetByIdAsync(documentId, cancellationToken);
        if (document == null)
        {
            throw new InvalidOperationException($"Document {documentId} not found.");
        }

        // Create execution record
        var execution = new ProcessExecution
        {
            Id = 0,
            ProcessDefinitionId = processDefinitionId,
            DocumentId = documentId,
            Status = ExecutionStatus.Pending,
            StartedAt = null,
            CompletedAt = null,
            ErrorMessage = null,
            Steps = Array.Empty<ProcessExecutionStep>()
        };

        execution = await _processExecutionRepository.CreateAsync(execution, cancellationToken);

        // Update document state to Processing
        document = document with { State = DocumentState.Processing };
        await _documentRepository.UpdateAsync(document, cancellationToken);

        try
        {
            // Start execution
            execution = execution with { Status = ExecutionStatus.Running, StartedAt = DateTimeOffset.UtcNow };
            execution = await _processExecutionRepository.UpdateAsync(execution, cancellationToken);

            _logger.LogInformation("Executing {StepCount} steps for execution {ExecutionId}",
                processDefinition.Steps.Count, execution.Id);

            // Execute steps in order
            var executionSteps = new List<ProcessExecutionStep>();
            foreach (var step in processDefinition.Steps.OrderBy(s => s.Order))
            {
                var stepResult = await ExecuteStepAsync(step, document, cancellationToken);
                executionSteps.Add(stepResult);

                if (stepResult.Status == ExecutionStatus.Failed)
                {
                    _logger.LogError("Step {StepName} failed: {ErrorMessage}",
                        step.Name, stepResult.ErrorMessage);

                    // Update execution as failed
                    execution = execution with
                    {
                        Status = ExecutionStatus.Failed,
                        CompletedAt = DateTimeOffset.UtcNow,
                        ErrorMessage = $"Step '{step.Name}' failed: {stepResult.ErrorMessage}",
                        Steps = executionSteps
                    };
                    execution = await _processExecutionRepository.UpdateAsync(execution, cancellationToken);

                    // Update document state to Failed
                    document = document with { State = DocumentState.Failed };
                    await _documentRepository.UpdateAsync(document, cancellationToken);

                    return execution;
                }

                // Reload document to get any changes made by the step
                document = await _documentRepository.GetByIdAsync(documentId, cancellationToken)
                           ?? throw new InvalidOperationException(
                               $"Document {documentId} not found after step execution.");
            }

            // All steps completed successfully
            execution = execution with
            {
                Status = ExecutionStatus.Completed, CompletedAt = DateTimeOffset.UtcNow, Steps = executionSteps
            };
            execution = await _processExecutionRepository.UpdateAsync(execution, cancellationToken);

            // Update document state to Processed
            document = document with { State = DocumentState.Processed };
            await _documentRepository.UpdateAsync(document, cancellationToken);

            _logger.LogInformation("Process execution {ExecutionId} completed successfully", execution.Id);

            return execution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Process execution {ExecutionId} failed with exception", execution.Id);

            // Update execution as failed
            execution = execution with
            {
                Status = ExecutionStatus.Failed,
                CompletedAt = DateTimeOffset.UtcNow,
                ErrorMessage = $"Execution failed: {ex.Message}"
            };
            execution = await _processExecutionRepository.UpdateAsync(execution, cancellationToken);

            // Update document state to Failed
            document = document with { State = DocumentState.Failed };
            await _documentRepository.UpdateAsync(document, cancellationToken);

            throw;
        }
    }

    public async Task<int> ProcessPendingDocumentsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking for pending documents to process");

        // Get all enabled process definitions
        var allProcesses = await _processDefinitionRepository.GetAllAsync(cancellationToken);
        var enabledProcesses = allProcesses.Where(p => p.Enabled).ToList();

        if (!enabledProcesses.Any())
        {
            _logger.LogDebug("No enabled processes found");
            return 0;
        }

        var executionCount = 0;

        // For each unique trigger state, find documents and create executions
        var triggerStates = enabledProcesses.Select(p => p.TriggerState).Distinct();

        foreach (var triggerState in triggerStates)
        {
            var processesForState = enabledProcesses.Where(p => p.TriggerState == triggerState).ToList();

            // Find documents in this state
            var allDocuments = await _documentRepository.GetAllAsync(cancellationToken);
            var documentsInState = allDocuments.Where(d => d.State == triggerState).ToList();

            _logger.LogInformation("Found {DocumentCount} documents in state {State}",
                documentsInState.Count, triggerState);

            foreach (var document in documentsInState)
            {
                foreach (var process in processesForState)
                {
                    try
                    {
                        // Check if there's already a pending or running execution for this document and process
                        var existingExecutions =
                            await _processExecutionRepository.GetByDocumentIdAsync(document.Id, cancellationToken);
                        var hasActiveExecution = existingExecutions.Any(e =>
                            e.ProcessDefinitionId == process.Id &&
                            (e.Status == ExecutionStatus.Pending || e.Status == ExecutionStatus.Running));

                        if (!hasActiveExecution)
                        {
                            _logger.LogInformation(
                                "Creating execution for document {DocumentId} with process {ProcessName}",
                                document.Id, process.Name);

                            await ExecuteProcessAsync(process.Id, document.Id, cancellationToken);
                            executionCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to execute process {ProcessId} for document {DocumentId}",
                            process.Id, document.Id);
                    }
                }
            }
        }

        _logger.LogInformation("Created {ExecutionCount} process executions", executionCount);
        return executionCount;
    }

    private async Task<ProcessExecutionStep> ExecuteStepAsync(
        ProcessingStep step,
        Document document,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing step {StepName} (Type: {StepType})", step.Name, step.StepType);

        var executionStep = new ProcessExecutionStep
        {
            Id = 0,
            ProcessExecutionId = 0, // Will be set when saved
            StepName = step.Name,
            StepType = step.StepType,
            Order = step.Order,
            Status = ExecutionStatus.Running,
            StartedAt = DateTimeOffset.UtcNow,
            CompletedAt = null,
            ErrorMessage = null,
            Output = null
        };

        try
        {
            string? output = null;

            switch (step.StepType)
            {
                case StepType.AzureOpenAI:
                    output = await ExecuteAzureOpenAIStepAsync(step, document, cancellationToken);
                    break;

                case StepType.AzureDocumentIntelligence:
                    output = await ExecuteAzureDocumentIntelligenceStepAsync(step, document, cancellationToken);
                    break;

                default:
                    throw new NotSupportedException($"Step type {step.StepType} is not supported.");
            }

            executionStep = executionStep with
            {
                Status = ExecutionStatus.Completed, CompletedAt = DateTimeOffset.UtcNow, Output = output
            };

            _logger.LogInformation("Step {StepName} completed successfully", step.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Step {StepName} failed", step.Name);

            executionStep = executionStep with
            {
                Status = ExecutionStatus.Failed, CompletedAt = DateTimeOffset.UtcNow, ErrorMessage = ex.Message
            };
        }

        return executionStep;
    }

    private async Task<string> ExecuteAzureOpenAIStepAsync(
        ProcessingStep step,
        Document document,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(step.AzureEndpoint))
        {
            throw new InvalidOperationException("Azure endpoint is required for Azure OpenAI steps.");
        }

        if (string.IsNullOrEmpty(step.AzureApiKey))
        {
            throw new InvalidOperationException("Azure API key is required for Azure OpenAI steps.");
        }

        if (string.IsNullOrEmpty(step.AzureModelOrDeployment))
        {
            throw new InvalidOperationException("Azure model or deployment name is required for Azure OpenAI steps.");
        }

        _logger.LogDebug(
            "Calling Azure OpenAI API. Endpoint={Endpoint}, Deployment={Deployment}",
            step.AzureEndpoint, step.AzureModelOrDeployment);

        var client = new AzureOpenAIClient(
            new Uri(step.AzureEndpoint),
            new AzureKeyCredential(step.AzureApiKey));

        var chatClient = client.GetChatClient(step.AzureModelOrDeployment);

        // Build the prompt with document context
        var systemPrompt = step.Prompt ?? "You are a helpful AI assistant that processes documents.";

        var userPrompt = "Execute your task for the document with the identifier " + document.Id + ".\n\n";

        var messages = new List<ChatMessage> { new SystemChatMessage(systemPrompt), new UserChatMessage(userPrompt) };

        // Parse enabled tools from configuration
        var enabledTools = ParseEnabledTools(step.Configuration);
        var tools = BuildChatTools(enabledTools);

        var chatOptions = new ChatCompletionOptions();
        foreach (var tool in tools)
        {
            chatOptions.Tools.Add(tool);
        }

        // Execute chat with potential tool calls
        var conversationMessages = new List<ChatMessage>(messages);
        var maxIterations = 5; // Prevent infinite loops
        var iteration = 0;
        var finalResult = new System.Text.StringBuilder();

        while (iteration < maxIterations)
        {
            iteration++;
            var response = await chatClient.CompleteChatAsync(conversationMessages, chatOptions, cancellationToken);
            var responseMessage = response.Value;

            // Check for tool calls
            if (responseMessage.ToolCalls.Count > 0)
            {
                conversationMessages.Add(new AssistantChatMessage(responseMessage));

                foreach (var toolCall in responseMessage.ToolCalls)
                {
                    if (toolCall is ChatToolCall functionToolCall)
                    {
                        var toolResult = await ExecuteToolAsync(
                            functionToolCall.FunctionName,
                            functionToolCall.FunctionArguments.ToString(),
                            document,
                            cancellationToken);

                        conversationMessages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                        finalResult.AppendLine($"Tool '{functionToolCall.FunctionName}' executed: {toolResult}");
                    }
                }
            }
            else
            {
                // No more tool calls, we have the final response
                var result = responseMessage.Content[0].Text;
                finalResult.AppendLine(result);
                break;
            }
        }

        _logger.LogDebug("Azure OpenAI API call completed. Iterations: {Iterations}", iteration);

        return finalResult.ToString();
    }

    private List<string> ParseEnabledTools(string? configuration)
    {
        if (string.IsNullOrEmpty(configuration))
        {
            return new List<string>();
        }

        try
        {
            var tools = System.Text.Json.JsonSerializer.Deserialize<List<string>>(configuration);
            return tools ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private List<ChatTool> BuildChatTools(List<string> enabledTools)
    {
        var tools = new List<ChatTool>();

        if (enabledTools.Contains("SetDocumentTitle"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_document_title",
                functionDescription:
                "Sets the title of the current document. Use this when you have analyzed the document and determined an appropriate title based on its content.",
                functionParameters: BinaryData.FromBytes("""
                                                         {
                                                             "type": "object",
                                                             "properties": {
                                                                 "title": {
                                                                     "type": "string",
                                                                     "description": "The new title for the document"
                                                                 }
                                                             },
                                                             "required": ["title"]
                                                         }
                                                         """u8.ToArray())
            ));
        }

        if (enabledTools.Contains("SetContract"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_contract",
                functionDescription: "Sets the contract for the current document by contract ID.",
                functionParameters: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "contractId": {
                                "type": "number",
                                "description": "The ID of the contract to assign"
                            }
                        },
                        "required": ["contractId"]
                    }
                    """u8.ToArray())
            ));
        }

        if (enabledTools.Contains("SetCorrespondent"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_correspondent",
                functionDescription: "Sets the correspondent for the current document by correspondent ID.",
                functionParameters: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "correspondentId": {
                                "type": "number",
                                "description": "The ID of the correspondent to assign"
                            }
                        },
                        "required": ["correspondentId"]
                    }
                    """u8.ToArray())
            ));
        }

        if (enabledTools.Contains("SetDocumentType"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_document_type",
                functionDescription: "Sets the document type for the current document by document type ID.",
                functionParameters: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "documentTypeId": {
                                "type": "number",
                                "description": "The ID of the document type to assign"
                            }
                        },
                        "required": ["documentTypeId"]
                    }
                    """u8.ToArray())
            ));
        }

        if (enabledTools.Contains("SetTags"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_tags",
                functionDescription: "Sets the tags for the current document by tag IDs. Replaces all existing tags.",
                functionParameters: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "tagIds": {
                                "type": "array",
                                "items": {
                                    "type": "number"
                                },
                                "description": "Array of tag IDs to assign to the document"
                            }
                        },
                        "required": ["tagIds"]
                    }
                    """u8.ToArray())
            ));
        }

        if (enabledTools.Contains("SetContent"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_content",
                functionDescription: "Sets or updates the textual content of the current document.",
                functionParameters: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "content": {
                                "type": "string",
                                "description": "The text content to set"
                            }
                        },
                        "required": ["content"]
                    }
                    """u8.ToArray())
            ));
        }

        if (enabledTools.Contains("GetDocumentContent"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_document_content",
                functionDescription:
                "Get the content of the current document to analyze its text and structure."
            ));
        }

        if (enabledTools.Contains("GetDocument"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_document",
                functionDescription: "Get detailed information about a specific document by its ID.",
                functionParameters: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "documentId": {
                                "type": "number",
                                "description": "The ID of the document to retrieve"
                            }
                        },
                        "required": ["documentId"]
                    }
                    """u8.ToArray())
            ));
        }

        if (enabledTools.Contains("GetSimilarDocuments"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_similar_documents",
                functionDescription:
                "Retrieves a list of existing documents with their titles to help create consistent naming. You can filter by document type or correspondent.",
                functionParameters: BinaryData.FromBytes("""
                                                         {
                                                             "type": "object",
                                                             "properties": {
                                                                 "documentTypeId": {
                                                                     "type": "number",
                                                                     "description": "Optional: Filter by document type ID"
                                                                 },
                                                                 "correspondentId": {
                                                                     "type": "number",
                                                                     "description": "Optional: Filter by correspondent ID"
                                                                 },
                                                                 "limit": {
                                                                     "type": "number",
                                                                     "description": "Maximum number of documents to return (default: 10, max: 50)"
                                                                 }
                                                             }
                                                         }
                                                         """u8.ToArray())
            ));
        }

        if (enabledTools.Contains("GetContracts"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_contracts",
                functionDescription: "Retrieves all available contracts with their IDs and names."
            ));
        }

        if (enabledTools.Contains("GetDocumentTypes"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_document_types",
                functionDescription: "Retrieves all available document types with their IDs and names."
            ));
        }

        if (enabledTools.Contains("GetCorrespondents"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_correspondents",
                functionDescription: "Retrieves all available correspondents with their IDs and names."
            ));
        }

        if (enabledTools.Contains("GetTags"))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_tags",
                functionDescription: "Retrieves all available tags with their IDs and names."
            ));
        }

        return tools;
    }

    private async Task<string> ExecuteToolAsync(
        string toolName,
        string toolArguments,
        Document document,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing tool: {ToolName} with arguments: {Arguments}", toolName, toolArguments);

        try
        {
            switch (toolName)
            {
                case "set_document_title":
                    return await ExecuteSetDocumentTitleAsync(toolArguments, document, cancellationToken);

                case "set_contract":
                    return await ExecuteSetContractAsync(toolArguments, document, cancellationToken);

                case "set_correspondent":
                    return await ExecuteSetCorrespondentAsync(toolArguments, document, cancellationToken);

                case "set_document_type":
                    return await ExecuteSetDocumentTypeAsync(toolArguments, document, cancellationToken);

                case "set_tags":
                    return await ExecuteSetTagsAsync(toolArguments, document, cancellationToken);

                case "set_content":
                    return await ExecuteSetContentAsync(toolArguments, document, cancellationToken);

                case "get_document_content":
                    return document.Content ?? "Document has no content.";

                case "get_document":
                    return await ExecuteGetDocumentAsync(toolArguments, cancellationToken);

                case "get_similar_documents":
                    return await ExecuteGetSimilarDocumentsAsync(toolArguments, cancellationToken);

                case "get_contracts":
                    return await ExecuteGetContractsAsync(cancellationToken);

                case "get_document_types":
                    return await ExecuteGetDocumentTypesAsync(cancellationToken);

                case "get_correspondents":
                    return await ExecuteGetCorrespondentsAsync(cancellationToken);

                case "get_tags":
                    return await ExecuteGetTagsAsync(cancellationToken);

                default:
                    return $"Unknown tool: {toolName}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool {ToolName}", toolName);
            return $"Error executing tool: {ex.Message}";
        }
    }

    private async Task<string> ExecuteSetDocumentTitleAsync(
        string arguments,
        Document document,
        CancellationToken cancellationToken)
    {
        var args = System.Text.Json.JsonSerializer.Deserialize<SetDocumentTitleArgs>(arguments, JsonSerializerOptions.Web);
        if (args == null || string.IsNullOrWhiteSpace(args.Title))
        {
            return "Error: Title cannot be empty";
        }

        // Update document title
        var updatedDocument = document with { Title = args.Title };
        await _documentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} title updated to: {Title}", document.Id, args.Title);
        return $"Document title successfully updated to: {args.Title}";
    }

    private async Task<string> ExecuteGetSimilarDocumentsAsync(
        string arguments,
        CancellationToken cancellationToken)
    {
        var args = JsonSerializer.Deserialize<GetSimilarDocumentsArgs>(arguments, JsonSerializerOptions.Web);
        var limit = Math.Min(args?.Limit ?? 10, 50);

        // Get all documents and filter
        var allDocuments = await _documentRepository.GetAllAsync(cancellationToken);
        var filteredDocuments = allDocuments.AsEnumerable();

        if (args?.DocumentTypeId > 0)
        {
            filteredDocuments = filteredDocuments.Where(d => d.DocumentTypeId == args.DocumentTypeId);
        }

        if (args?.CorrespondentId > 0)
        {
            filteredDocuments = filteredDocuments.Where(d => d.CorrespondentId == args.CorrespondentId);
        }

        var documents = filteredDocuments
            .OrderByDescending(d => d.Added)
            .Take(limit)
            .Select(d => new
            {
                d.Id,
                d.Title,
                d.DocumentDate,
                d.DocumentTypeId,
                d.CorrespondentId
            })
            .ToList();

        var result = System.Text.Json.JsonSerializer.Serialize(documents,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        return $"Found {documents.Count} similar documents:\n{result}";
    }

    private async Task<string> ExecuteSetContractAsync(
        string arguments,
        Document document,
        CancellationToken cancellationToken)
    {
        var args = JsonSerializer.Deserialize<SetContractArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.ContractId == null || args.ContractId <= 0)
        {
            return "Error: Valid contract ID is required";
        }

        var contract = await _contractRepository.GetByIdAsync(args.ContractId, cancellationToken);
        if (contract == null)
        {
            return $"Error: Contract with ID {args.ContractId} not found";
        }

        var updatedDocument = document with { ContractId = args.ContractId };
        await _documentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} contract updated to: {ContractName} (ID: {ContractId})",
            document.Id, contract.Name, args.ContractId);
        return $"Document contract successfully set to: {contract.Name}";
    }

    private async Task<string> ExecuteSetCorrespondentAsync(
        string arguments,
        Document document,
        CancellationToken cancellationToken)
    {
        var args = JsonSerializer.Deserialize<SetCorrespondentArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.CorrespondentId == null || args.CorrespondentId <= 0)
        {
            return "Error: Valid correspondent ID is required";
        }

        var correspondent = await _correspondentRepository.GetByIdAsync(args.CorrespondentId, cancellationToken);
        if (correspondent == null)
        {
            return $"Error: Correspondent with ID {args.CorrespondentId} not found";
        }

        var updatedDocument = document with { CorrespondentId = args.CorrespondentId };
        await _documentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} correspondent updated to: {CorrespondentName} (ID: {CorrespondentId})",
            document.Id, correspondent.Name, args.CorrespondentId);
        return $"Document correspondent successfully set to: {correspondent.Name}";
    }

    private async Task<string> ExecuteSetDocumentTypeAsync(
        string arguments,
        Document document,
        CancellationToken cancellationToken)
    {
        var args = JsonSerializer.Deserialize<SetDocumentTypeArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.DocumentTypeId == null || args.DocumentTypeId <= 0)
        {
            return "Error: Valid document type ID is required";
        }

        var documentType = await _documentTypeRepository.GetByIdAsync(args.DocumentTypeId, cancellationToken);
        if (documentType == null)
        {
            return $"Error: Document type with ID {args.DocumentTypeId} not found";
        }

        var updatedDocument = document with { DocumentTypeId = args.DocumentTypeId };
        await _documentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} type updated to: {DocumentTypeName} (ID: {DocumentTypeId})",
            document.Id, documentType.Name, args.DocumentTypeId);
        return $"Document type successfully set to: {documentType.Name}";
    }

    private async Task<string> ExecuteSetTagsAsync(
        string arguments,
        Document document,
        CancellationToken cancellationToken)
    {
        var args = JsonSerializer.Deserialize<SetTagsArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.TagIds == null)
        {
            return "Error: Tag IDs array is required";
        }

        // Validate all tag IDs exist
        var allTags = await _tagRepository.GetAllAsync(cancellationToken);
        var validTagIds = allTags.Select(t => t.Id).ToHashSet();
        var invalidIds = args.TagIds.Where(id => !validTagIds.Contains(id)).ToList();

        if (invalidIds.Count > 0)
        {
            return $"Error: Invalid tag IDs: {string.Join(", ", invalidIds)}";
        }

        var updatedDocument = document with { TagIds = args.TagIds.ToArray() };
        await _documentRepository.UpdateAsync(updatedDocument, cancellationToken);

        var tagNames = allTags.Where(t => args.TagIds.Contains(t.Id)).Select(t => t.Name).ToList();
        _logger.LogInformation("Document {DocumentId} tags updated to: {TagNames}",
            document.Id, string.Join(", ", tagNames));
        return $"Document tags successfully set to: {string.Join(", ", tagNames)}";
    }

    private async Task<string> ExecuteSetContentAsync(
        string arguments,
        Document document,
        CancellationToken cancellationToken)
    {
        var args = JsonSerializer.Deserialize<SetContentArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.Content == null)
        {
            return "Error: Content cannot be null";
        }

        var updatedDocument = document with { Content = args.Content };
        await _documentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} content updated ({Length} characters)",
            document.Id, args.Content.Length);
        return $"Document content successfully updated ({args.Content.Length} characters)";
    }

    private async Task<string> ExecuteGetDocumentAsync(
        string arguments,
        CancellationToken cancellationToken)
    {
        var args = JsonSerializer.Deserialize<GetDocumentArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.DocumentId == null || args.DocumentId <= 0)
        {
            return "Error: Valid document ID is required";
        }

        var doc = await _documentRepository.GetByIdAsync(args.DocumentId, cancellationToken);
        if (doc == null)
        {
            return $"Error: Document with ID {args.DocumentId} not found";
        }

        var result = JsonSerializer.Serialize(new
        {
            doc.Id,
            doc.Title,
            doc.DocumentDate,
            doc.Content,
            doc.DocumentTypeId,
            doc.CorrespondentId,
            doc.ContractId,
            doc.TagIds,
            doc.State,
            doc.Added,
            doc.Updated
        }, new JsonSerializerOptions { WriteIndented = true });

        return $"Document details:\n{result}";
    }

    private async Task<string> ExecuteGetContractsAsync(CancellationToken cancellationToken)
    {
        var contracts = await _contractRepository.GetAllAsync(cancellationToken);
        var contractList = contracts.Select(c => new { c.Id, c.Name }).ToList();

        var result = JsonSerializer.Serialize(contractList, new JsonSerializerOptions { WriteIndented = true });
        return $"Available contracts ({contractList.Count}):\n{result}";
    }

    private async Task<string> ExecuteGetDocumentTypesAsync(CancellationToken cancellationToken)
    {
        var documentTypes = await _documentTypeRepository.GetAllAsync(cancellationToken);
        var typeList = documentTypes.Select(dt => new { dt.Id, dt.Name }).ToList();

        var result = JsonSerializer.Serialize(typeList, new JsonSerializerOptions { WriteIndented = true });
        return $"Available document types ({typeList.Count}):\n{result}";
    }

    private async Task<string> ExecuteGetCorrespondentsAsync(CancellationToken cancellationToken)
    {
        var correspondents = await _correspondentRepository.GetAllAsync(cancellationToken);
        var correspondentList = correspondents.Select(c => new { c.Id, c.Name }).ToList();

        var result = JsonSerializer.Serialize(correspondentList, new JsonSerializerOptions { WriteIndented = true });
        return $"Available correspondents ({correspondentList.Count}):\n{result}";
    }

    private async Task<string> ExecuteGetTagsAsync(CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAllAsync(cancellationToken);
        var tagList = tags.Select(t => new { t.Id, t.Name }).ToList();

        var result = JsonSerializer.Serialize(tagList, new JsonSerializerOptions { WriteIndented = true });
        return $"Available tags ({tagList.Count}):\n{result}";
    }

    private sealed class SetDocumentTitleArgs
    {
        public string Title { get; set; } = string.Empty;
    }

    private sealed class GetSimilarDocumentsArgs
    {
        public long? DocumentTypeId { get; set; }
        public long? CorrespondentId { get; set; }
        public int Limit { get; set; } = 10;
    }

    private sealed class SetContractArgs
    {
        public long ContractId { get; set; }
    }

    private sealed class SetCorrespondentArgs
    {
        public long CorrespondentId { get; set; }
    }

    private sealed class SetDocumentTypeArgs
    {
        public long DocumentTypeId { get; set; }
    }

    private sealed class SetTagsArgs
    {
        public List<long> TagIds { get; set; } = new();
    }

    private sealed class SetContentArgs
    {
        public string Content { get; set; } = string.Empty;
    }

    private sealed class GetDocumentArgs
    {
        public long DocumentId { get; set; }
    }

    private async Task<string> ExecuteAzureDocumentIntelligenceStepAsync(
        ProcessingStep step,
        Document document,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(step.AzureEndpoint))
        {
            throw new InvalidOperationException("Azure endpoint is required for Azure Document Intelligence steps.");
        }

        if (string.IsNullOrEmpty(step.AzureApiKey))
        {
            throw new InvalidOperationException("Azure API key is required for Azure Document Intelligence steps.");
        }

        if (document.OriginalFile == null || string.IsNullOrEmpty(document.OriginalFile.FilePath))
        {
            throw new InvalidOperationException($"Document {document.Id} does not have an original file.");
        }

        _logger.LogDebug(
            "Calling Azure Document Intelligence API. Endpoint={Endpoint}, Model={Model}",
            step.AzureEndpoint, step.AzureModelOrDeployment ?? "prebuilt-layout");

        var client = new DocumentIntelligenceClient(
            new Uri(step.AzureEndpoint),
            new AzureKeyCredential(step.AzureApiKey));

        // For simplicity, using prebuilt-layout model which extracts text, tables, and structure
        var modelId = step.AzureModelOrDeployment ?? "prebuilt-layout";

        // In a real implementation, you would load the document from storage
        // For now, we'll assume document.OriginalFile.FilePath is a local file path or URL
        // You should implement proper file loading logic based on your storage solution

        Operation<AnalyzeResult>? operation = null;
        
        var filePath = document.OriginalFile.FilePath;

        if (Uri.TryCreate(filePath, UriKind.Absolute, out var documentUri) &&
            (documentUri.Scheme == Uri.UriSchemeHttp || documentUri.Scheme == Uri.UriSchemeHttps))
        {
            var options = new AnalyzeDocumentOptions(modelId, documentUri) { OutputContentFormat = DocumentContentFormat.Markdown };
            // Document is accessible via URL
            operation = await client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                options,
                cancellationToken: cancellationToken);
        }
        else
        {
            // Document is a local file
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Document file not found: {filePath}");
            }

            var options = new AnalyzeDocumentOptions(modelId, new BinaryData(await File.ReadAllBytesAsync(filePath, cancellationToken))) { OutputContentFormat = DocumentContentFormat.Markdown };
            operation = await client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                options,
                cancellationToken: cancellationToken);
        }

        var result = operation.Value;

        // Extract text content
        var extractedText = result.Content;

        // Build a structured output with extracted information
        var output = new System.Text.StringBuilder();
        output.AppendLine($"Extracted Text ({extractedText.Length} characters):");
        output.AppendLine(extractedText);
        output.AppendLine();

        if (result.Pages.Count > 0)
        {
            output.AppendLine($"Document has {result.Pages.Count} page(s)");
        }

        if (result.Tables.Count > 0)
        {
            output.AppendLine($"Found {result.Tables.Count} table(s)");
            foreach (var table in result.Tables)
            {
                output.AppendLine($"  Table with {table.RowCount} rows and {table.ColumnCount} columns");
            }
        }

        if (result.KeyValuePairs.Count > 0)
        {
            output.AppendLine($"Found {result.KeyValuePairs.Count} key-value pair(s)");
        }

        _logger.LogDebug(
            "Azure Document Intelligence API call completed. Extracted {CharCount} characters from {PageCount} pages",
            extractedText.Length, result.Pages.Count);

        // Update document content with extracted text
        document = document with { Content = extractedText };
        await _documentRepository.UpdateAsync(document, cancellationToken);

        _logger.LogInformation("Updated document {DocumentId} with extracted content ({CharCount} characters)",
            document.Id, extractedText.Length);

        return output.ToString();
    }
}