namespace SqlServerMcp.Models;

public record BusinessRulesConfig
{
    public string Description { get; init; } = string.Empty;
    public List<BusinessRule> Rules { get; init; } = [];
}

public record BusinessRule
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> Conditions { get; init; } = [];
    public string ExampleSql { get; init; } = string.Empty;
}
