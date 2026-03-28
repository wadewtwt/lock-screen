using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using Brush = System.Windows.Media.Brush;
using WpfColor = System.Windows.Media.Color;

namespace LockScreen.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly DispatcherTimer _clockTimer;
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private DateTimeOffset _lastTickSecond = DateTimeOffset.MinValue;
    private bool _isSanitizingText;

    [ObservableProperty]
    private string currentTime = FormatCurrentTime();

    [ObservableProperty]
    private string currentDate = FormatCurrentDate(CultureInfo.CurrentCulture);

    [ObservableProperty]
    private string titleText = "DOT LOCK";

    [ObservableProperty]
    private string statusMessage = "DOT MATRIX SCREEN ACTIVE";

    [ObservableProperty]
    private string dismissText = "ENTER TO DISMISS";

    [ObservableProperty]
    private bool isSettingsPanelOpen;

    [ObservableProperty]
    private bool isColorPaletteOpen;

    [ObservableProperty]
    private string selectedTheme = "Blue";

    [ObservableProperty]
    private Brush backgroundBaseBrush = new SolidColorBrush(WpfColor.FromRgb(6, 9, 15));

    [ObservableProperty]
    private Brush glowTopLeftBrush = System.Windows.Media.Brushes.Transparent;

    [ObservableProperty]
    private Brush glowTopRightBrush = System.Windows.Media.Brushes.Transparent;

    [ObservableProperty]
    private Brush glowBottomBrush = System.Windows.Media.Brushes.Transparent;

    private record ThemeColors(WpfColor Base, WpfColor TopLeft, WpfColor TopRight, WpfColor Bottom);

    private static readonly Dictionary<string, ThemeColors> Themes = new()
    {
        ["Blue"] = new(
            WpfColor.FromRgb(6, 9, 15),
            WpfColor.FromArgb(42, 54, 88, 255),
            WpfColor.FromArgb(31, 0, 220, 170),
            WpfColor.FromArgb(22, 255, 198, 64)),
        ["Purple"] = new(
            WpfColor.FromRgb(10, 6, 16),
            WpfColor.FromArgb(44, 130, 0, 255),
            WpfColor.FromArgb(35, 220, 0, 180),
            WpfColor.FromArgb(22, 190, 60, 255)),
        ["Green"] = new(
            WpfColor.FromRgb(5, 14, 7),
            WpfColor.FromArgb(38, 0, 200, 80),
            WpfColor.FromArgb(28, 0, 230, 140),
            WpfColor.FromArgb(20, 80, 255, 30)),
        ["Ember"] = new(
            WpfColor.FromRgb(14, 5, 3),
            WpfColor.FromArgb(44, 255, 40, 0),
            WpfColor.FromArgb(32, 220, 80, 0),
            WpfColor.FromArgb(22, 255, 200, 0)),
        ["Mono"] = new(
            WpfColor.FromRgb(8, 8, 8),
            WpfColor.FromArgb(30, 180, 180, 180),
            WpfColor.FromArgb(20, 160, 160, 160),
            WpfColor.FromArgb(15, 210, 210, 210))
    };

    public MainWindowViewModel(ILogger logger)
    {
        _logger = logger;

        _clockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _clockTimer.Tick += (_, _) => UpdateClockIfNeeded();
        _clockTimer.Start();

        UpdateClock();
        ApplyTheme("Blue");
    }

    [RelayCommand]
    private void HandleKeyPress(Key key)
    {
        if (IsSettingsPanelOpen)
        {
            if (key is Key.Escape or Key.Enter)
            {
                IsSettingsPanelOpen = false;
                IsColorPaletteOpen = false;
            }
            return;
        }

        if (key != Key.Enter)
        {
            return;
        }

        _logger.Information("Lock screen dismissed with Enter key.");
        System.Windows.Application.Current.Shutdown();
    }

    [RelayCommand]
    private void ToggleSettingsPanel()
    {
        IsSettingsPanelOpen = !IsSettingsPanelOpen;
        if (!IsSettingsPanelOpen)
        {
            IsColorPaletteOpen = false;
        }
    }

    [RelayCommand]
    private void CloseSettingsPanel()
    {
        IsSettingsPanelOpen = false;
        IsColorPaletteOpen = false;
    }

    [RelayCommand]
    private void ToggleColorPalette()
    {
        IsColorPaletteOpen = !IsColorPaletteOpen;
    }

    [RelayCommand]
    private void ApplyTheme(string themeName)
    {
        if (!Themes.TryGetValue(themeName, out var theme))
        {
            return;
        }

        SelectedTheme = themeName;
        IsColorPaletteOpen = false;
        BackgroundBaseBrush = new SolidColorBrush(theme.Base);
        GlowTopLeftBrush = RadialGlow(0.18, 0.18, 0.55, 0.45, theme.TopLeft);
        GlowTopRightBrush = RadialGlow(0.80, 0.20, 0.50, 0.40, theme.TopRight);
        GlowBottomBrush = RadialGlow(0.55, 0.95, 0.60, 0.45, theme.Bottom);
        _logger.Information("Theme applied: {Theme}", themeName);
    }

    partial void OnTitleTextChanged(string value) =>
        NormalizeEditableText(value, normalized => TitleText = normalized);

    partial void OnStatusMessageChanged(string value) =>
        NormalizeEditableText(value, normalized => StatusMessage = normalized);

    partial void OnDismissTextChanged(string value) =>
        NormalizeEditableText(value, normalized => DismissText = normalized);

    private void UpdateClockIfNeeded()
    {
        var now = DateTimeOffset.Now;
        var second = new DateTimeOffset(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            now.Second,
            now.Offset);

        if (second == _lastTickSecond)
        {
            return;
        }

        _lastTickSecond = second;
        UpdateClock();
    }

    private void UpdateClock()
    {
        CurrentTime = FormatCurrentTime();
        CurrentDate = FormatCurrentDate(_culture);
    }

    private void NormalizeEditableText(string? value, Action<string> assign)
    {
        if (_isSanitizingText)
        {
            return;
        }

        var normalized = SanitizeDotMatrixText(value);
        if (normalized == value)
        {
            return;
        }

        _isSanitizingText = true;
        assign(normalized);
        _isSanitizingText = false;
    }

    private static string SanitizeDotMatrixText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var buffer = new List<char>(value.Length);
        foreach (var character in value.ToUpperInvariant())
        {
            if ((character >= 'A' && character <= 'Z') ||
                (character >= '0' && character <= '9') ||
                character is ' ' or '.' or ':' or '-')
            {
                buffer.Add(character);
            }
        }

        return new string(buffer.ToArray()).Trim();
    }

    private static string FormatCurrentTime() =>
        DateTimeOffset.Now.ToString("HH:mm:ss");

    private static string FormatCurrentDate(CultureInfo culture) =>
        DateTimeOffset.Now.ToString("yyyy.MM.dd ddd", culture).ToUpperInvariant();

    private static RadialGradientBrush RadialGlow(
        double centerX,
        double centerY,
        double radiusX,
        double radiusY,
        WpfColor color)
    {
        var brush = new RadialGradientBrush
        {
            Center = new System.Windows.Point(centerX, centerY),
            GradientOrigin = new System.Windows.Point(centerX, centerY),
            RadiusX = radiusX,
            RadiusY = radiusY,
            MappingMode = BrushMappingMode.RelativeToBoundingBox
        };
        brush.GradientStops.Add(new GradientStop(color, 0));
        brush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
        brush.Freeze();
        return brush;
    }
}
