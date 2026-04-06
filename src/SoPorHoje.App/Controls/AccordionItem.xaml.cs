namespace SoPorHoje.App.Controls;

/// <summary>
/// Accordion item reutilizável para listas de Passos, Tradições e similares.
/// </summary>
public partial class AccordionItem : ContentView
{
    public AccordionItem()
    {
        InitializeComponent();
    }

    // ── BindableProperties ────────────────────────────────────────────────────

    public static readonly BindableProperty NumberProperty =
        BindableProperty.Create(nameof(Number), typeof(int), typeof(AccordionItem), 0);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(AccordionItem), string.Empty);

    public static readonly BindableProperty BodyProperty =
        BindableProperty.Create(nameof(Body), typeof(string), typeof(AccordionItem), string.Empty);

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(AccordionItem), false,
            propertyChanged: OnIsExpandedChanged);

    // ── Properties ────────────────────────────────────────────────────────────

    public int Number
    {
        get => (int)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Body
    {
        get => (string)GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>Descrição de acessibilidade gerada a partir do título.</summary>
    public string AccessibilityDescription =>
        IsExpanded
            ? $"{Title}. Toque para recolher."
            : $"{Title}. Toque para expandir e ler o texto completo.";

    // ── Events ────────────────────────────────────────────────────────────────

    private static void OnIsExpandedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AccordionItem item)
            item.OnPropertyChanged(nameof(AccessibilityDescription));
    }

    private void OnTapped(object? sender, EventArgs e)
    {
        IsExpanded = !IsExpanded;
    }
}
