using SoPorHoje.Core.Models;

namespace SoPorHoje.Core.Constants;

public static class ChipDefinitions
{
    public static readonly List<SobrietyChip> BrazilianChips = new()
    {
        new() { RequiredDays = 1,    Name = "Amarela",         Label = "Ingresso",  ColorHex = "#D4A017", Emoji = "🌅", SortOrder = 1 },
        new() { RequiredDays = 90,   Name = "Azul",            Label = "3 Meses",   ColorHex = "#2E86C1", Emoji = "🌊", SortOrder = 2 },
        new() { RequiredDays = 180,  Name = "Rosa",            Label = "6 Meses",   ColorHex = "#C7588E", Emoji = "🌸", SortOrder = 3 },
        new() { RequiredDays = 270,  Name = "Vermelha",        Label = "9 Meses",   ColorHex = "#C0392B", Emoji = "🔥", SortOrder = 4 },
        new() { RequiredDays = 365,  Name = "Verde",           Label = "1 Ano",     ColorHex = "#27AE60", Emoji = "🌿", SortOrder = 5 },
        new() { RequiredDays = 730,  Name = "Verde Gravata",   Label = "2 Anos",    ColorHex = "#1E8449", Emoji = "🌳", SortOrder = 6 },
        new() { RequiredDays = 1825, Name = "Branca Gravata",  Label = "5 Anos",    ColorHex = "#D5D8DC", Emoji = "⭐", SortOrder = 7 },
        new() { RequiredDays = 3650, Name = "Amarela Gravata", Label = "10 Anos",   ColorHex = "#D4A017", Emoji = "🏆", SortOrder = 8 },
        new() { RequiredDays = 5475, Name = "Azul Gravata",    Label = "15 Anos",   ColorHex = "#2E86C1", Emoji = "💎", SortOrder = 9 },
        new() { RequiredDays = 7300, Name = "Rosa Gravata",    Label = "20 Anos",   ColorHex = "#C7588E", Emoji = "👑", SortOrder = 10 },
    };
}
