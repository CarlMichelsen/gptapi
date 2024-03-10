namespace Domain.LargeLanguageModel.Shared;

public enum LargeLanguageModelMessageRole
{
    /// <summary>
    /// System message.
    /// </summary>
    System,

    /// <summary>
    /// Message sent by assistant.
    /// </summary>
    Assistant,

    /// <summary>
    /// Message sent by user.
    /// </summary>
    User,
}
