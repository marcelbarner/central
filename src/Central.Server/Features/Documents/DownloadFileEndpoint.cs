using Central.Domain.Documents.Services;
using FastEndpoints;

namespace Central.Server.Features.Documents;

public sealed record DownloadFileRequest
{
    public required long Id { get; init; }
    public required string FileType { get; init; } // "original", "archive", "thumbnail"
}

public sealed class DownloadFileEndpoint(IDocumentService documentService)
    : Endpoint<DownloadFileRequest>
{
    public override void Configure()
    {
        Get("/api/documents/{Id}/files/{FileType}");
    }

    public override async Task HandleAsync(DownloadFileRequest req, CancellationToken ct)
    {
        try
        {
            var document = await documentService.GetByIdAsync(req.Id, ct);
            if (document == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            var file = req.FileType.ToLowerInvariant() switch
            {
                "original" => document.OriginalFile,
                "archive" => document.ArchiveFile,
                "thumbnail" => document.Thumbnail,
                _ => null
            };

            if (file == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            var fileStream = await documentService.GetFileAsync(req.Id, req.FileType, ct);
            var contentType = GetContentType(file.FileName);

            await Send.StreamAsync(
                stream: fileStream,
                fileName: file.FileName,
                fileLengthBytes: fileStream.Length,
                contentType: contentType,
                cancellation: ct);
        }
        catch (InvalidOperationException)
        {
            await Send.NotFoundAsync(ct);
        }
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }
}
