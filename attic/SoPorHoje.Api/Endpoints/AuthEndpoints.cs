using SoPorHoje.Api.DTOs;
using SoPorHoje.Api.Services;

namespace SoPorHoje.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/anonymous", async (AnonymousAuthRequest request, SyncService sync) =>
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId))
                return Results.BadRequest(new { error = "deviceId é obrigatório" });

            var (userId, isNew) = await sync.GetOrCreateUserAsync(request.DeviceId);
            return Results.Ok(new AnonymousAuthResponse(userId, isNew));
        })
        .WithName("AuthAnonymous")
        .WithSummary("Cria ou retorna usuário anônimo pelo deviceId")
        .WithTags("Auth");
    }
}
