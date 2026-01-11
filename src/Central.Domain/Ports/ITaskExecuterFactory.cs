using Central.Domain.Documents;

namespace Central.Domain.Ports;

/// <summary>
/// Factory for creating task executers based on task type.
/// </summary>
public interface ITaskExecuterFactory
{
    /// <summary>
    /// Gets the appropriate task executer for the given task type.
    /// </summary>
    /// <param name="taskType">The type of task to execute.</param>
    /// <returns>The task executer for the specified type.</returns>
    /// <exception cref="NotSupportedException">If the task type is not supported.</exception>
    ITaskExecuter GetExecuter(TaskType taskType);

    /// <summary>
    /// Gets the wait task executer for pipeline wait steps.
    /// </summary>
    /// <returns>The wait task executer.</returns>
    ITaskExecuter GetWaitExecuter();
}
