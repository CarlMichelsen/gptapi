namespace Domain.Entity;

public enum Role
{
    /// <summary>
    /// Give behind the scenes messages to the assistant.
    /// </summary>
    System,

    /// <summary>
    /// Chat with the assistant.
    /// </summary>
    User,

    /// <summary>
    /// Assistant response.
    /// </summary>
    Assistant,
}
