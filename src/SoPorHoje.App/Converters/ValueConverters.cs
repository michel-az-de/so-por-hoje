using System.Globalization;
using Microsoft.Maui.Graphics;

namespace SoPorHoje.App.Converters;

/// <summary>Retorna a cor de fundo do card de reunião (verde claro se ao vivo).</summary>
public class LiveToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isLive && isLive)
            return Application.Current?.Resources["SuccessLight"] as Color
                   ?? Color.FromArgb("#D4EDDA");
        return Application.Current?.Resources["BgSurface"] as Color
               ?? Colors.White;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retorna a cor de borda do card de reunião (verde se ao vivo).</summary>
public class LiveToBorderConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isLive && isLive)
            return Application.Current?.Resources["Success"] as Color
                   ?? Color.FromArgb("#28A745");
        return Application.Current?.Resources["BorderLight"] as Color
               ?? Color.FromArgb("#E0D8C8");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retorna o texto do botão de reunião ("AO VIVO" ou "Entrar").</summary>
public class LiveToButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool isLive && isLive ? "AO VIVO" : "Entrar";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retorna a cor do botão de reunião (verde se ao vivo, azul caso contrário).</summary>
public class LiveToButtonColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isLive && isLive)
            return Application.Current?.Resources["Success"] as Color
                   ?? Color.FromArgb("#28A745");
        return Application.Current?.Resources["Accent"] as Color
               ?? Color.FromArgb("#2E5BB8");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retorna descrição de acessibilidade para o botão de reunião.</summary>
public class LiveToAccessibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool isLive && isLive
            ? "Reunião acontecendo agora. Toque para entrar."
            : "Entrar na reunião online.";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// Converte bool IsExpanded em graus de rotação para o chevron do accordion.
/// Expandido = 180°, recolhido = 0°.
/// </summary>
public class BoolToRotationConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool expanded && expanded ? 180.0 : 0.0;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retorna cor do número circular do accordion (accent se expandido).</summary>
public class BoolToAccentConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool expanded && expanded)
            return Application.Current?.Resources["Accent"] as Color
                   ?? Color.FromArgb("#2E5BB8");
        return Application.Current?.Resources["BgCard"] as Color
               ?? Color.FromArgb("#FAFAF7");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retorna a cor do texto do número no accordion (branco se expandido).</summary>
public class BoolToNumberTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool expanded && expanded)
            return Colors.White;
        return Application.Current?.Resources["TextPrimary"] as Color
               ?? Color.FromArgb("#1A1A1A");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Inverte um bool.</summary>
public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}
