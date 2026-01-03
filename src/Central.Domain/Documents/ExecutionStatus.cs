namespace Central.Domain.Documents;

/// <summary>
/// Represents the execution status of a process or step.
/// </summary>
public enum ExecutionStatus
{
    /// <summary>
    /// Execution is queued and waiting to start.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Execution is currently in progress.
    /// </summary>
    Running = 1,

    /// <summary>
    /// Execution completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Execution failed with an error.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Execution was cancelled by the user or system.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Step was skipped due to a previous failure or condition.
    /// </summary>
    Skipped = 5
}