using Central.Domain.Documents;
using Central.Server.Features.ProcessExecutions;

using Riok.Mapperly.Abstractions;

namespace Central.Server.Mappers;

/// <summary>
/// Static mapper for converting between ProcessExecution domain models and DTOs.
/// </summary>
[Mapper]
public static partial class ProcessExecutionDtoMapper
{
    /// <summary>
    /// Maps a domain ProcessExecution to ProcessExecutionDto.
    /// </summary>
    public static partial ProcessExecutionDto ToDto(this ProcessExecution execution);

    /// <summary>
    /// Maps a collection of ProcessExecutions to DTOs.
    /// </summary>
    public static partial IReadOnlyCollection<ProcessExecutionDto> ToDto(this IEnumerable<ProcessExecution> executions);

    /// <summary>
    /// Maps ProcessExecutionStep to ProcessExecutionStepDto.
    /// </summary>
    public static partial ProcessExecutionStepDto ToDto(this ProcessExecutionStep step);
}