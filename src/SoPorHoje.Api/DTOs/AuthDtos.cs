namespace SoPorHoje.Api.DTOs;

/// <summary>
/// Request body for anonymous authentication.
/// / Corpo da requisição para autenticação anônima.
/// </summary>
/// <param name="DeviceId">
/// Unique device identifier generated on the client (UUID string).
/// / Identificador único do dispositivo gerado no cliente (string UUID).
/// </param>
public record AnonymousAuthRequest(string DeviceId);

/// <summary>
/// Response returned after anonymous authentication.
/// / Resposta retornada após autenticação anônima.
/// </summary>
/// <param name="UserId">Server-assigned user UUID. / UUID do usuário atribuído pelo servidor.</param>
/// <param name="IsNew">
/// <c>true</c> if a new account was just created; <c>false</c> if the device was already registered.
/// / <c>true</c> se a conta foi criada agora; <c>false</c> se o dispositivo já estava cadastrado.
/// </param>
public record AnonymousAuthResponse(Guid UserId, bool IsNew);
