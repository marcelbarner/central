namespace Central.Domain.Documents;

/// <summary>
/// Represents a document file attachment.
/// </summary>
/// <param name="FileName">The name of the file.</param>
/// <param name="FilePath">The complete file path including directory and filename.</param>
public sealed record DocumentFile(string FileName, string FilePath);