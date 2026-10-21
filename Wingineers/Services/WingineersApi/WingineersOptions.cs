using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

public sealed class WingineersOptions
{
    public const string SectionName = "WingineersApi";

    [Required, Url] public required string BaseUrl { get; init; }
    [Required] public required string ApiKey { get; init; }
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(5);
}