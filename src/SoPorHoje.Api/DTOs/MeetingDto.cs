namespace SoPorHoje.Api.DTOs;

/// <summary>
/// Represents an AA online meeting.
/// / Representa uma reunião online de A.A.
/// </summary>
/// <param name="Id">Database identifier. / Identificador no banco de dados.</param>
/// <param name="GroupName">Name of the AA group. / Nome do grupo de A.A.</param>
/// <param name="DaysOfWeekMask">
/// Bitmask encoding which weekdays the meeting occurs:
/// Sunday=1, Monday=2, Tuesday=4, Wednesday=8, Thursday=16, Friday=32, Saturday=64.
/// Example: 127 = every day.
/// / Bitmask dos dias da semana em que a reunião ocorre:
/// Domingo=1, Segunda=2, Terça=4, Quarta=8, Quinta=16, Sexta=32, Sábado=64.
/// Exemplo: 127 = todos os dias.
/// </param>
/// <param name="StartTime">Meeting start time in HH:mm:ss format (Brasília timezone). / Horário de início no formato HH:mm:ss (fuso de Brasília).</param>
/// <param name="EndTime">Meeting end time in HH:mm:ss format (Brasília timezone). / Horário de término no formato HH:mm:ss (fuso de Brasília).</param>
/// <param name="MeetingUrl">URL of the online meeting (Zoom, Google Meet, etc.). / URL da reunião online (Zoom, Google Meet, etc.).</param>
/// <param name="Platform">Platform name (e.g. "Zoom", "Google Meet"). / Nome da plataforma (ex: "Zoom", "Google Meet").</param>
/// <param name="Source">Data source (typically "intergrupos-aa.org.br"). / Fonte dos dados (tipicamente "intergrupos-aa.org.br").</param>
/// <param name="IsLiveNow">
/// <c>true</c> if this meeting is currently happening based on Brasília time.
/// / <c>true</c> se esta reunião está acontecendo agora com base no horário de Brasília.
/// </param>
public record MeetingDto(
    int Id,
    string GroupName,
    int DaysOfWeekMask,
    string StartTime,
    string EndTime,
    string MeetingUrl,
    string? Platform,
    string Source,
    bool IsLiveNow
);
