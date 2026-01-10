namespace Central.Domain.Documents;

/// <summary>
/// Represents the type of pipeline step.
/// </summary>
public enum PipelineStepType
{
    /// <summary>
    /// Step that executes a task on the document.
    /// </summary>
    TaskStep,

    /// <summary>
    /// Step that waits for a specified duration before continuing.
    /// </summary>
    WaitStep
}
