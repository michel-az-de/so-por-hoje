using System.Text.Json;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace SoPorHoje.Tests.Integration;

/// <summary>
/// Valida o conteúdo das reflexões (data/reflections) contra o schema do projeto
/// (data/reflections/SCHEMA.md) e garante que o JSON empacotado no app está
/// sincronizado com a fonte. Roda no CI a cada PR — barra contribuições inválidas.
/// </summary>
public class ReflectionsContentTests
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private sealed class Entry
    {
        public string? Date { get; set; }
        public string? Title { get; set; }
        public string? Quote { get; set; }
        public string? Text { get; set; }
        public string? Theme { get; set; }
        public string? License { get; set; }
        public string? Author { get; set; }
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "SoPorHoje.sln")))
            dir = dir.Parent;
        dir.Should().NotBeNull("os testes devem rodar dentro do repositório (SoPorHoje.sln não encontrado)");
        return dir!.FullName;
    }

    private static string ReflectionsDir() => Path.Combine(RepoRoot(), "data", "reflections");

    private static IEnumerable<string> SourceFiles() =>
        Directory.Exists(ReflectionsDir())
            ? Directory.EnumerateFiles(ReflectionsDir(), "*.json", SearchOption.AllDirectories)
            : Enumerable.Empty<string>();

    private static List<Entry> Load(string path) =>
        JsonSerializer.Deserialize<List<Entry>>(File.ReadAllText(path), JsonOpts) ?? new();

    [Fact]
    public void SourceFiles_Exist()
    {
        SourceFiles().Should().NotBeEmpty("deve haver ao menos um arquivo de reflexões em data/reflections");
    }

    [Fact]
    public void EveryEntry_FollowsSchema()
    {
        var errors = new List<string>();

        foreach (var file in SourceFiles())
        {
            var name = Path.GetFileName(file);
            List<Entry> entries;
            try { entries = Load(file); }
            catch (Exception ex) { errors.Add($"{name}: JSON inválido — {ex.Message}"); continue; }

            foreach (var e in entries)
            {
                var id = $"{name}[{e.Date}]";
                if (string.IsNullOrWhiteSpace(e.Date) || !Regex.IsMatch(e.Date, @"^\d{2}-\d{2}$"))
                    errors.Add($"{id}: 'date' deve estar no formato MM-DD");
                if (string.IsNullOrWhiteSpace(e.Title)) errors.Add($"{id}: 'title' vazio");
                if (string.IsNullOrWhiteSpace(e.Quote)) errors.Add($"{id}: 'quote' vazio");
                if (string.IsNullOrWhiteSpace(e.Text)) errors.Add($"{id}: 'text' vazio");
                if (string.IsNullOrWhiteSpace(e.Theme)) errors.Add($"{id}: 'theme' vazio");
                if (string.IsNullOrWhiteSpace(e.Author)) errors.Add($"{id}: 'author' vazio");
                if (e.License != "CC-BY-SA-4.0") errors.Add($"{id}: 'license' deve ser exatamente CC-BY-SA-4.0");
            }
        }

        errors.Should().BeEmpty("as reflexões devem seguir data/reflections/SCHEMA.md:\n" + string.Join("\n", errors));
    }

    [Fact]
    public void Dates_AreUnique_PerLanguage()
    {
        if (!Directory.Exists(ReflectionsDir())) return;

        foreach (var langDir in Directory.EnumerateDirectories(ReflectionsDir()))
        {
            var dates = Directory.EnumerateFiles(langDir, "*.json", SearchOption.AllDirectories)
                .SelectMany(Load)
                .Select(e => e.Date)
                .ToList();

            dates.Should().OnlyHaveUniqueItems($"as datas devem ser únicas em {Path.GetFileName(langDir)}");
        }
    }

    [Fact]
    public void PackagedPtBr_IsInSyncWithSource()
    {
        var packaged = Path.Combine(RepoRoot(), "src", "SoPorHoje.App", "Resources", "Raw", "reflections-pt-BR.json");
        File.Exists(packaged).Should().BeTrue("o JSON empacotado deve existir em Resources/Raw");

        var ptBrDir = Path.Combine(ReflectionsDir(), "pt-BR");
        if (!Directory.Exists(ptBrDir)) return;

        var source = Directory.EnumerateFiles(ptBrDir, "*.json", SearchOption.AllDirectories)
            .SelectMany(Load)
            .Select(e => (e.Date, e.Title, e.Text))
            .OrderBy(x => x.Date)
            .ToList();

        var bundled = Load(packaged)
            .Select(e => (e.Date, e.Title, e.Text))
            .OrderBy(x => x.Date)
            .ToList();

        bundled.Should().BeEquivalentTo(source,
            "o JSON empacotado (Resources/Raw/reflections-pt-BR.json) deve refletir data/reflections/pt-BR — atualize-o ao editar a fonte");
    }
}
