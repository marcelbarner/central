using AwesomeAssertions;

using Central.Domain.Documents;
using Central.Infrastructure.Entities;
using Central.Infrastructure.Mappers;
using Central.Infrastructure.Persistence;
using Central.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Central.Infrastructure.Tests.Repositories;

public sealed class DocumentRepositoryTests : IAsyncLifetime
{
    private ApplicationDbContext _context = null!;
    private DocumentRepository _repository = null!;

    public async ValueTask InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        await _context.Database.EnsureCreatedAsync();

        _repository = new DocumentRepository(_context);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldAddDocumentAndReturnWithGeneratedId()
    {
        // Arrange
        var document = new Document
        {
            Id = 0,
            Title = "Test Document",
            DocumentDate = DateTimeOffset.UtcNow,
            Content = "Test content",
            OriginalFile = null,
            ArchiveFile = null,
            Thumbnail = null,
            Added = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            AddedBy = 1,
            UpdatedBy = 1
        };

        // Act
        var result = await _repository.AddAsync(document);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Test Document");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDocumentExists_ShouldReturnDocument()
    {
        // Arrange
        var document = new Document
        {
            Id = 0,
            Title = "Test Document",
            DocumentDate = null,
            Content = null,
            OriginalFile = null,
            ArchiveFile = null,
            Thumbnail = null,
            Added = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            AddedBy = 1,
            UpdatedBy = 1
        };
        var added = await _repository.AddAsync(document);

        // Act
        var result = await _repository.GetByIdAsync(added.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(added.Id);
        result.Title.Should().Be("Test Document");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDocumentDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDocuments()
    {
        // Arrange
        var doc1 = new Document
        {
            Id = 0,
            Title = "Document 1",
            DocumentDate = null,
            Content = null,
            OriginalFile = null,
            ArchiveFile = null,
            Thumbnail = null,
            Added = DateTimeOffset.UtcNow.AddDays(-2),
            Updated = DateTimeOffset.UtcNow.AddDays(-2),
            AddedBy = 1,
            UpdatedBy = 1
        };

        var doc2 = new Document
        {
            Id = 0,
            Title = "Document 2",
            DocumentDate = null,
            Content = null,
            OriginalFile = null,
            ArchiveFile = null,
            Thumbnail = null,
            Added = DateTimeOffset.UtcNow.AddDays(-1),
            Updated = DateTimeOffset.UtcNow.AddDays(-1),
            AddedBy = 2,
            UpdatedBy = 2
        };

        await _repository.AddAsync(doc1);
        await _repository.AddAsync(doc2);

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        results.Should().NotBeNull();
        results.Count.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingDocument()
    {
        // Arrange
        var document = new Document
        {
            Id = 0,
            Title = "Original Title",
            DocumentDate = null,
            Content = null,
            OriginalFile = null,
            ArchiveFile = null,
            Thumbnail = null,
            Added = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            AddedBy = 1,
            UpdatedBy = 1
        };
        var added = await _repository.AddAsync(document);

        var updated = added with
        {
            Title = "Updated Title",
            Content = "New content",
            Updated = DateTimeOffset.UtcNow
        };

        // Act
        var result = await _repository.UpdateAsync(updated);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        result.Content.Should().Be("New content");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveDocument()
    {
        // Arrange
        var document = new Document
        {
            Id = 0,
            Title = "To Delete",
            DocumentDate = null,
            Content = null,
            OriginalFile = null,
            ArchiveFile = null,
            Thumbnail = null,
            Added = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            AddedBy = 1,
            UpdatedBy = 1
        };
        var added = await _repository.AddAsync(document);

        // Act
        await _repository.DeleteAsync(added.Id);

        // Assert
        var deleted = await _repository.GetByIdAsync(added.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WithAllFileTypes_ShouldPersistCorrectly()
    {
        // Arrange
        var document = new Document
        {
            Id = 0,
            Title = "Complete Document",
            DocumentDate = DateTimeOffset.UtcNow,
            Content = "Full content",
            OriginalFile = new DocumentFile("original.pdf", "/path/to/original.pdf"),
            ArchiveFile = new DocumentFile("archive.pdf", "/path/to/archive.pdf"),
            Thumbnail = new DocumentFile("thumb.jpg", "/path/to/thumb.jpg"),
            Added = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            AddedBy = 1,
            UpdatedBy = 1
        };

        // Act
        var result = await _repository.AddAsync(document);

        // Assert
        result.OriginalFile.Should().NotBeNull();
        result.OriginalFile!.FileName.Should().Be("original.pdf");
        result.ArchiveFile.Should().NotBeNull();
        result.ArchiveFile!.FileName.Should().Be("archive.pdf");
        result.Thumbnail.Should().NotBeNull();
        result.Thumbnail!.FileName.Should().Be("thumb.jpg");
    }
}