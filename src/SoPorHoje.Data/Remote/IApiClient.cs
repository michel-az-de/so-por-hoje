using Refit;
using SoPorHoje.Data.Remote.DTOs;

namespace SoPorHoje.Data.Remote;

[Headers("Content-Type: application/json")]
public interface IApiClient
{
    [Post("/api/sync/push")]
    Task<ApiResponse<SyncPushResponse>> PushAsync([Body] SyncPushRequest request);

    [Get("/api/sync/pull")]
    Task<ApiResponse<SyncPullResponse>> PullAsync([Query] DateTime? since);

    [Get("/api/meetings")]
    Task<ApiResponse<List<MeetingDto>>> GetMeetingsAsync();

    [Get("/api/reflections")]
    Task<ApiResponse<List<ReflectionDto>>> GetReflectionsAsync();

    [Get("/api/health")]
    Task<ApiResponse<string>> HealthCheckAsync();
}
