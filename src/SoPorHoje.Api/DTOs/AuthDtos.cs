namespace SoPorHoje.Api.DTOs;

public record AnonymousAuthRequest(string DeviceId);

public record AnonymousAuthResponse(Guid UserId, bool IsNew);
