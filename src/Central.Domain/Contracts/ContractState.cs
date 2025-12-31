namespace Central.Domain.Contracts;

/// <summary>
/// Represents the lifecycle state of a contract.
/// </summary>
public enum ContractState
{
    /// <summary>
    /// Contract is in draft state and not yet finalized.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Contract is active and in effect.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Contract has expired and is no longer in effect.
    /// </summary>
    Expired = 2,

    /// <summary>
    /// Contract was terminated before its natural end.
    /// </summary>
    Terminated = 3
}