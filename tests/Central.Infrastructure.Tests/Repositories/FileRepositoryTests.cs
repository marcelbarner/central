using AwesomeAssertions;

using Central.Infrastructure.Configuration;
using Central.Infrastructure.Repositories;

using Microsoft.Extensions.Options;

using Xunit;

namespace Central.Infrastructure.Tests.Repositories;

public sealed class FileRepositoryTests : IAsyncLifetime
{
    private readonly string _testDirectory;
    private OriginalFileRepository _repository = null!;

    public FileRepositoryTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"FileRepoTests_{Guid.NewGuid()}");
    }

    public ValueTask InitializeAsync()
    {
        var config = Options.Create(new FileSystemConfiguration
        {
            Media = _testDirectory
        });

        _repository = new OriginalFileRepository(config);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task SaveAsync_ShouldCreateFileAndReturnPath()
    {
        // Arrange
        var content = "Test file content"u8.ToArray();
        using var stream = new MemoryStream(content);
        var fileName = "test-file.txt";

        // Act
        var filePath = await _repository.SaveAsync(stream, fileName);

        // Assert
        filePath.Should().NotBeNullOrEmpty();
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_ShouldGenerateUniqueFileName()
    {
        // Arrange
        var content = "Test content"u8.ToArray();
        var fileName = "duplicate.txt";

        // Act
        using var stream1 = new MemoryStream(content);
        var filePath1 = await _repository.SaveAsync(stream1, fileName);

        using var stream2 = new MemoryStream(content);
        var filePath2 = await _repository.SaveAsync(stream2, fileName);

        // Assert
        filePath1.Should().NotBe(filePath2);
        File.Exists(filePath1).Should().BeTrue();
        File.Exists(filePath2).Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WhenFileExists_ShouldReturnStream()
    {
        // Arrange
        var content = "Test file content"u8.ToArray();
        using var saveStream = new MemoryStream(content);
        var filePath = await _repository.SaveAsync(saveStream, "test.txt");

        // Act
        using var retrievedStream = await _repository.GetAsync(filePath);

        // Assert
        retrievedStream.Should().NotBeNull();
        retrievedStream.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAsync_WhenFileDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testDirectory, "nonexistent.txt");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => _repository.GetAsync(nonExistentPath));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveFile()
    {
        // Arrange
        var content = "Test file content"u8.ToArray();
        using var stream = new MemoryStream(content);
        var filePath = await _repository.SaveAsync(stream, "to-delete.txt");

        // Act
        await _repository.DeleteAsync(filePath);

        // Assert
        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public async Task Exists_WhenFileExists_ShouldReturnTrue()
    {
        // Arrange
        var content = "Test file content"u8.ToArray();
        using var stream = new MemoryStream(content);
        var filePath = await _repository.SaveAsync(stream, "exists.txt");

        // Act
        var exists = _repository.Exists(filePath);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public void Exists_WhenFileDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testDirectory, "nonexistent.txt");

        // Act
        var exists = _repository.Exists(nonExistentPath);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task SaveAsync_WithSpecialCharactersInFileName_ShouldSanitize()
    {
        // Arrange
        var content = "Test content"u8.ToArray();
        using var stream = new MemoryStream(content);
        var fileName = "test<>:file?.txt";

        // Act
        var filePath = await _repository.SaveAsync(stream, fileName);

        // Assert
        filePath.Should().NotBeNullOrEmpty();
        File.Exists(filePath).Should().BeTrue();
        Path.GetFileName(filePath).Should().NotContain("<");
        Path.GetFileName(filePath).Should().NotContain(">");
    }
}