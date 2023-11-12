namespace Domain.Configuration;

public class AccessOptions
{
    public const string SectionName = "Access";

    public List<UsernamePassword>? Users { get; init; }
}
