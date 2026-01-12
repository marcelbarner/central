using System.Text;
using System.Text.Json;

using Azure;

using Azure.AI.OpenAI;

using Central.Domain.Contracts.Ports;
using Central.Domain.Correspondents.Ports;
using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Domain.DocumentTypes.Ports;
using Central.Domain.Ports;
using Central.Domain.Tags.Ports;

using Microsoft.Extensions.Logging;

using OpenAI.Chat;

namespace Central.Infrastructure.Services;

/// <summary>
/// Task executer for Azure OpenAI tasks.
/// </summary>
public sealed class OpenAITaskExecuter : ITaskExecuter
{
    private readonly ILogger<OpenAITaskExecuter> _logger;
    private readonly IDocumentRepository _documentRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly ICorrespondentRepository _correspondentRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IChatToolProvider _chatToolProvider;
    private readonly IDocumentToolFactory _documentToolFactory;

    public OpenAITaskExecuter(
        ILogger<OpenAITaskExecuter> logger,
        IDocumentRepository documentRepository,
        IContractRepository contractRepository,
        IDocumentTypeRepository documentTypeRepository,
        ICorrespondentRepository correspondentRepository,
        ITagRepository tagRepository,
        IChatToolProvider chatToolProvider,
        IDocumentToolFactory documentToolFactory)
    {
        _logger = logger;
        _documentRepository = documentRepository;
        _contractRepository = contractRepository;
        _documentTypeRepository = documentTypeRepository;
        _correspondentRepository = correspondentRepository;
        _tagRepository = tagRepository;
        _chatToolProvider = chatToolProvider;
        _documentToolFactory = documentToolFactory;
    }

    /// <inheritdoc />
    public async Task<string> ExecuteAsync(TaskExecutionContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(context.Task.Configuration.AzureEndpoint))
        {
            throw new InvalidOperationException("Azure endpoint is required for Azure OpenAI tasks.");
        }

        if (string.IsNullOrEmpty(context.Task.Configuration.AzureApiKey))
        {
            throw new InvalidOperationException("Azure API key is required for Azure OpenAI tasks.");
        }

        if (string.IsNullOrEmpty(context.Task.Configuration.AzureModelOrDeployment))
        {
            throw new InvalidOperationException("Azure model or deployment name is required for Azure OpenAI tasks.");
        }

        _logger.LogDebug(
            "Calling Azure OpenAI API. Endpoint={Endpoint}, Deployment={Deployment}",
            context.Task.Configuration.AzureEndpoint, context.Task.Configuration.AzureModelOrDeployment);

        var client = new AzureOpenAIClient(
            new Uri(context.Task.Configuration.AzureEndpoint),
            new AzureKeyCredential(context.Task.Configuration.AzureApiKey));

        var chatClient = client.GetChatClient(context.Task.Configuration.AzureModelOrDeployment);

        // Build the prompt with document context
        var systemPrompt = context.Task.Configuration.Prompt ?? "You are a helpful AI assistant that processes documents.";
        var userPrompt = "Execute your task for the document with the identifier " + context.Document.Id + ".\n\n";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };

        // Build enabled tools from configuration
        var enabledTools = context.Task.Configuration.Capabilities ?? [];
        var tools = _chatToolProvider.BuildChatTools(enabledTools);

        var chatOptions = new ChatCompletionOptions();
        foreach (var tool in tools)
        {
            chatOptions.Tools.Add(tool);
        }

        // Execute chat with potential tool calls
        var conversationMessages = new List<ChatMessage>(messages);
        var maxIterations = 5; // Prevent infinite loops
        var iteration = 0;
        var finalResult = new StringBuilder();

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
                            context.Document,
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

    private async Task<string> ExecuteToolAsync(
        string toolName,
        string toolArguments,
        Document document,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing tool: {ToolName} with arguments: {Arguments}", toolName, toolArguments);

        try
        {
            var tool = _documentToolFactory.GetTool(toolName);
            if (tool == null)
            {
                return $"Unknown tool: {toolName}";
            }

            var toolContext = new DocumentToolContext
            {
                Document = document,
                DocumentRepository = _documentRepository,
                ContractRepository = _contractRepository,
                DocumentTypeRepository = _documentTypeRepository,
                CorrespondentRepository = _correspondentRepository,
                TagRepository = _tagRepository
            };

            return await tool.ExecuteAsync(toolArguments, toolContext, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool {ToolName}", toolName);
            return $"Error executing tool: {ex.Message}";
        }
    }
}
