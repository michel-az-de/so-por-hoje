using Refit;
using SoPorHoje.Data.Remote.DTOs;

namespace SoPorHoje.Data.Remote;

/// <summary>
/// Factory helper that creates a typed Refit client for the given base URL.
/// In production, use dependency injection (e.g. AddRefitClient) instead.
/// </summary>
public static class ApiClientFactory
{
    public static IApiClient Create(string baseUrl)
    {
        return RestService.For<IApiClient>(baseUrl);
    }
}
