namespace TaskManager.Api.Domain;

public static class SubscriptionPlanLimits
{
    /// <summary>null = unlimited.</summary>
    public static int? GetMaxProjects(this SubscriptionPlan plan) => plan switch
    {
        SubscriptionPlan.Freemium => 1,
        SubscriptionPlan.Basic => 10,
        SubscriptionPlan.Advanced => null,
        _ => throw new ArgumentOutOfRangeException(nameof(plan), plan, "Unknown plan.")
    };

    public static int? GetMaxTasksPerProject(this SubscriptionPlan plan) => plan switch
    {
        SubscriptionPlan.Freemium => 20,
        SubscriptionPlan.Basic or SubscriptionPlan.Advanced => null,
        _ => throw new ArgumentOutOfRangeException(nameof(plan), plan, "Unknown plan.")
    };
}