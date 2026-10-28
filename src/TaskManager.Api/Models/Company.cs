namespace TaskManager.Api.Models;

public sealed class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Freemium;
    public SubscriptionType SubscriptionType { get; set; } = SubscriptionType.None;
    public List<Project> Projects { get; set; } = new();
}
