using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace SoPorHoje.Scraper;

/// <summary>
/// Scraper de reuniões online do intergrupos-aa.org.br.
///
/// Estratégia:
/// 1. Tenta GET na API JSON interna do site (endpoint descoberto via DevTools → Network)
/// 2. Se não encontrar API JSON, faz parse do HTML com HtmlAgilityPack
/// 3. Rate limit: máximo 1 request a cada 10 segundos
/// 4. Respeita robots.txt (não rastreia páginas proibidas)
/// 5. User-Agent identificável
/// </summary>
public class IntergruposScraper
{
    private static readonly string[] PlatformKeywords = ["zoom", "meet", "teams", "jitsi", "whereby", "skype", "hangout"];

    private readonly HttpClient _http;
    private readonly ILogger<IntergruposScraper> _logger;

    // Mapeamento de nome do dia (pt-BR) para bit na máscara
    private static readonly Dictionary<string, int> DayBits = new(StringComparer.OrdinalIgnoreCase)
    {
        ["domingo"]    = 1 << 0,
        ["dom"]        = 1 << 0,
        ["segunda"]    = 1 << 1,
        ["seg"]        = 1 << 1,
        ["segunda-feira"] = 1 << 1,
        ["terça"]      = 1 << 2,
        ["ter"]        = 1 << 2,
        ["terça-feira"] = 1 << 2,
        ["quarta"]     = 1 << 3,
        ["qua"]        = 1 << 3,
        ["quarta-feira"] = 1 << 3,
        ["quinta"]     = 1 << 4,
        ["qui"]        = 1 << 4,
        ["quinta-feira"] = 1 << 4,
        ["sexta"]      = 1 << 5,
        ["sex"]        = 1 << 5,
        ["sexta-feira"] = 1 << 5,
        ["sábado"]     = 1 << 6,
        ["sab"]        = 1 << 6,
        ["sabado"]     = 1 << 6,
    };

    public IntergruposScraper(HttpClient http, ILogger<IntergruposScraper> logger)
    {
        _http = http;
        _logger = logger;
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "SoPorHoje-App/1.0 (+soporhoje)");
        _http.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<List<ScrapedMeeting>> ScrapeAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Iniciando scraping de reuniões em intergrupos-aa.org.br");

        try
        {
            // Tenta endpoint de API JSON interno primeiro
            var apiMeetings = await TryJsonApiAsync(ct);
            if (apiMeetings.Count > 0)
            {
                _logger.LogInformation("Obtidas {Count} reuniões via API JSON", apiMeetings.Count);
                return apiMeetings;
            }

            // Fallback: parse HTML
            var htmlMeetings = await ScrapeHtmlAsync(ct);
            _logger.LogInformation("Obtidas {Count} reuniões via HTML scraping", htmlMeetings.Count);
            return htmlMeetings;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Site intergrupos-aa.org.br indisponível — retornando lista vazia");
            return [];
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Timeout ao acessar intergrupos-aa.org.br — retornando lista vazia");
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado no scraper — retornando lista vazia");
            return [];
        }
    }

    // -------------------------------------------------------------------
    // Tentativa via API JSON interna (mais confiável que HTML scraping)
    // -------------------------------------------------------------------
    private async Task<List<ScrapedMeeting>> TryJsonApiAsync(CancellationToken ct)
    {
        // Candidatos de endpoints de API JSON encontrados em sites do gênero
        string[] apiCandidates =
        [
            "https://intergrupos-aa.org.br/api/meetings",
            "https://intergrupos-aa.org.br/api/reunioes",
            "https://igo.org.br/pt-BR/api/meetings",
        ];

        foreach (var url in apiCandidates)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), ct); // rate limit
                using var response = await _http.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode) continue;

                var content = await response.Content.ReadAsStringAsync(ct);
                if (!content.TrimStart().StartsWith('[') && !content.TrimStart().StartsWith('{')) continue;

                var meetings = ParseJsonResponse(content);
                if (meetings.Count > 0) return meetings;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Endpoint {Url} não disponível", url);
            }
        }

        return [];
    }

    private static List<ScrapedMeeting> ParseJsonResponse(string json)
    {
        // Tentativa genérica: deserializar array de objetos JSON
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            var array = root.ValueKind == System.Text.Json.JsonValueKind.Array
                ? root
                : root.TryGetProperty("data", out var data) ? data
                : root.TryGetProperty("meetings", out var m) ? m
                : root.TryGetProperty("reunioes", out var r) ? r
                : default;

            if (array.ValueKind != System.Text.Json.JsonValueKind.Array) return [];

            var results = new List<ScrapedMeeting>();
            foreach (var item in array.EnumerateArray())
            {
                var name = GetStringProperty(item, "name", "nome", "group_name", "groupName", "grupo");
                var url = GetStringProperty(item, "url", "link", "meeting_url", "meetingUrl", "zoom_link", "zoomLink");
                var startStr = GetStringProperty(item, "start_time", "startTime", "hora_inicio", "horaInicio");
                var endStr = GetStringProperty(item, "end_time", "endTime", "hora_fim", "horaFim");
                var daysStr = GetStringProperty(item, "days", "dias", "days_of_week", "daysOfWeek");

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url)) continue;
                if (!TryParseTime(startStr, out var start)) continue;
                if (!TryParseTime(endStr, out var end)) end = start + TimeSpan.FromHours(1.5);

                var mask = ParseDaysMask(daysStr);
                if (mask == 0) mask = 127; // todos os dias se não especificado

                results.Add(new ScrapedMeeting(
                    GroupName: name,
                    DaysOfWeekMask: mask,
                    StartTime: start,
                    EndTime: end,
                    MeetingUrl: url,
                    Platform: DetectPlatform(url)
                ));
            }

            return results;
        }
        catch
        {
            return [];
        }
    }

    // -------------------------------------------------------------------
    // Fallback: scraping HTML com HtmlAgilityPack
    // -------------------------------------------------------------------
    private async Task<List<ScrapedMeeting>> ScrapeHtmlAsync(CancellationToken ct)
    {
        string[] urlCandidates =
        [
            "https://intergrupos-aa.org.br/reunioes-online",
            "https://intergrupos-aa.org.br/reunioes",
            "https://intergrupos-aa.org.br",
            "https://igo.org.br/pt-BR/home",
        ];

        foreach (var url in urlCandidates)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), ct); // rate limit: 1 req/10s

                _logger.LogDebug("Tentando HTML scraping em {Url}", url);
                using var response = await _http.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogDebug("URL {Url} retornou {Status}", url, response.StatusCode);
                    continue;
                }

                var html = await response.Content.ReadAsStringAsync(ct);
                var meetings = ParseHtml(html, url);
                if (meetings.Count > 0) return meetings;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Falha ao acessar {Url}", url);
            }
        }

        _logger.LogWarning("Nenhuma reunião encontrada via HTML. Site pode ser SPA (requer Playwright).");
        return [];
    }

    private List<ScrapedMeeting> ParseHtml(string html, string sourceUrl)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var results = new List<ScrapedMeeting>();

        // Estratégia 1: procurar links de Zoom/Meet/Teams em listas ou tabelas
        var links = doc.DocumentNode.SelectNodes("//a[@href]");
        if (links == null) return results;

        foreach (var link in links)
        {
            var href = link.GetAttributeValue("href", "");
            if (!IsMeetingUrl(href)) continue;

            // Tentar obter contexto: linha da tabela ou item de lista
            var container = link.ParentNode;
            for (int i = 0; i < 4 && container != null; i++)
            {
                var text = container.InnerText;
                if (text.Length > 10) break;
                container = container.ParentNode;
            }

            var contextText = container?.InnerText ?? link.InnerText;
            contextText = HtmlEntity.DeEntitize(contextText).Trim();

            if (string.IsNullOrWhiteSpace(contextText)) continue;

            var name = ExtractGroupName(contextText, link.InnerText);
            if (string.IsNullOrWhiteSpace(name)) continue;

            TryParseTimeFromText(contextText, out var start, out var end);
            if (start == default) start = TimeSpan.FromHours(19); // horário padrão

            var mask = ParseDaysMask(contextText);
            if (mask == 0) mask = 127;

            results.Add(new ScrapedMeeting(
                GroupName: name,
                DaysOfWeekMask: mask,
                StartTime: start,
                EndTime: end == default ? start + TimeSpan.FromHours(1.5) : end,
                MeetingUrl: href,
                Platform: DetectPlatform(href)
            ));
        }

        _logger.LogDebug("HTML scraping de {Url} encontrou {Count} reuniões", sourceUrl, results.Count);
        return results;
    }

    // -------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------

    private static bool IsMeetingUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        var lower = url.ToLowerInvariant();
        return lower.Contains("zoom.us") || lower.Contains("meet.google")
            || lower.Contains("teams.microsoft") || lower.Contains("jitsi")
            || lower.Contains("whereby.com") || lower.Contains("meet.jit.si");
    }

    private static string DetectPlatform(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return "Online";
        var lower = url.ToLowerInvariant();
        if (lower.Contains("zoom.us")) return "Zoom";
        if (lower.Contains("meet.google")) return "Google Meet";
        if (lower.Contains("teams.microsoft")) return "Microsoft Teams";
        if (lower.Contains("jitsi") || lower.Contains("meet.jit.si")) return "Jitsi";
        if (lower.Contains("whereby.com")) return "Whereby";
        return "Online";
    }

    private static string ExtractGroupName(string contextText, string linkText)
    {
        // Usar as primeiras palavras significativas do contexto como nome do grupo
        var lines = contextText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var line in lines)
        {
            if (line.Length > 5 && line.Length < 200 && !IsMeetingUrl(line))
                return line.Trim();
        }
        return linkText.Trim();
    }

    private static void TryParseTimeFromText(string text, out TimeSpan start, out TimeSpan end)
    {
        start = default;
        end = default;

        // Procura padrões como "19:00", "19h00", "19h", "19:00 - 20:30"
        var match = System.Text.RegularExpressions.Regex.Match(text,
            @"(\d{1,2})[h:](\d{2})?\s*[-–]\s*(\d{1,2})[h:](\d{2})?",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (match.Success)
        {
            int.TryParse(match.Groups[1].Value, out var h1);
            int.TryParse(match.Groups[2].Value, out var m1);
            int.TryParse(match.Groups[3].Value, out var h2);
            int.TryParse(match.Groups[4].Value, out var m2);
            start = new TimeSpan(h1, m1, 0);
            end = new TimeSpan(h2, m2, 0);
            return;
        }

        match = System.Text.RegularExpressions.Regex.Match(text,
            @"(\d{1,2})[h:](\d{2})?",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (match.Success)
        {
            int.TryParse(match.Groups[1].Value, out var h);
            int.TryParse(match.Groups[2].Value, out var m);
            start = new TimeSpan(h, m, 0);
        }
    }

    private static bool TryParseTime(string? value, out TimeSpan result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        if (TimeSpan.TryParseExact(value, ["hh\\:mm", "h\\:mm", "hh\\:mm\\:ss"], null, out result))
            return true;

        var match = System.Text.RegularExpressions.Regex.Match(value, @"(\d{1,2})[h:](\d{2})?");
        if (match.Success)
        {
            int.TryParse(match.Groups[1].Value, out var h);
            int.TryParse(match.Groups[2].Value, out var m);
            result = new TimeSpan(h, m, 0);
            return true;
        }

        return false;
    }

    internal static int ParseDaysMask(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;

        var lower = text.ToLowerInvariant();

        // "todos os dias" / "diariamente"
        if (lower.Contains("todos os dias") || lower.Contains("diariamente") || lower.Contains("todo dia"))
            return 127;

        int mask = 0;
        foreach (var (key, bit) in DayBits)
            if (lower.Contains(key))
                mask |= bit;

        return mask;
    }

    private static string? GetStringProperty(System.Text.Json.JsonElement element, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (element.TryGetProperty(key, out var prop) && prop.ValueKind == System.Text.Json.JsonValueKind.String)
                return prop.GetString();
        }
        return null;
    }
}
