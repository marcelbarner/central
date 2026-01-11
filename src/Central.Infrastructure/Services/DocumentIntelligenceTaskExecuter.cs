using System.Text;

using Azure;
using Azure.AI.DocumentIntelligence;

using Central.Domain.Documents.Ports;
using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services;

/// <summary>
/// Task executer for Azure Document Intelligence tasks.
/// </summary>
public sealed class DocumentIntelligenceTaskExecuter : ITaskExecuter
{
    private readonly ILogger<DocumentIntelligenceTaskExecuter> _logger;
    private readonly IDocumentRepository _documentRepository;

    public DocumentIntelligenceTaskExecuter(
        ILogger<DocumentIntelligenceTaskExecuter> logger,
        IDocumentRepository documentRepository)
    {
        _logger = logger;
        _documentRepository = documentRepository;
    }

    /// <inheritdoc />
    public async Task<string> ExecuteAsync(TaskExecutionContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(context.Task.Configuration.AzureEndpoint))
        {
            throw new InvalidOperationException("Azure endpoint is required for Azure Document Intelligence tasks.");
        }

        if (string.IsNullOrEmpty(context.Task.Configuration.AzureApiKey))
        {
            throw new InvalidOperationException("Azure API key is required for Azure Document Intelligence tasks.");
        }

        if (context.Document.OriginalFile == null || string.IsNullOrEmpty(context.Document.OriginalFile.FilePath))
        {
            throw new InvalidOperationException($"Document {context.Document.Id} does not have an original file.");
        }

        _logger.LogDebug(
            "Calling Azure Document Intelligence API. Endpoint={Endpoint}, Model={Model}",
            context.Task.Configuration.AzureEndpoint,
            context.Task.Configuration.AzureModelOrDeployment ?? "prebuilt-layout");

        var client = new DocumentIntelligenceClient(
            new Uri(context.Task.Configuration.AzureEndpoint),
            new AzureKeyCredential(context.Task.Configuration.AzureApiKey));

        // For simplicity, using prebuilt-layout model which extracts text, tables, and structure
        var modelId = context.Task.Configuration.AzureModelOrDeployment ?? "prebuilt-layout";

        Operation<AnalyzeResult>? operation = null;
        var filePath = context.Document.OriginalFile.FilePath;

        if (Uri.TryCreate(filePath, UriKind.Absolute, out var documentUri) &&
            (documentUri.Scheme == Uri.UriSchemeHttp || documentUri.Scheme == Uri.UriSchemeHttps))
        {
            // Document is accessible via URL
            var options = new AnalyzeDocumentOptions(modelId, documentUri)
            {
                OutputContentFormat = DocumentContentFormat.Markdown
            };

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

            var options = new AnalyzeDocumentOptions(
                modelId,
                new BinaryData(await File.ReadAllBytesAsync(filePath, cancellationToken)))
            {
                OutputContentFormat = DocumentContentFormat.Markdown
            };

            operation = await client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                options,
                cancellationToken: cancellationToken);
        }

        var result = operation.Value;

        // Extract text content
        var extractedText = result.Content;

        // Build a structured output with extracted information
        var output = new StringBuilder();
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
        var updatedDocument = context.Document with { Content = extractedText };
        await _documentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Updated document {DocumentId} with extracted content ({CharCount} characters)",
            context.Document.Id, extractedText.Length);

        return output.ToString();
    }
}
