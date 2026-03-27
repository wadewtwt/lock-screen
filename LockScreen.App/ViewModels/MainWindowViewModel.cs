using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace LockScreen.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly DispatcherTimer _clockTimer;
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private DateTimeOffset _lastTickSecond = DateTimeOffset.MinValue;

    [ObservableProperty]
    private string currentTime = FormatCurrentTime();

    [ObservableProperty]
    private string currentDate = FormatCurrentDate(CultureInfo.CurrentCulture);

    [ObservableProperty]
    private string statusMessage = "DOT MATRIX SCREEN ACTIVE";

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
    }

    [RelayCommand]
    private void HandleKeyPress(Key key)
    {
        if (key != Key.Enter)
        {
            return;
        }

        _logger.Information("Lock screen dismissed with Enter key.");
        System.Windows.Application.Current.Shutdown();
    }

    private void UpdateClockIfNeeded()
    {
        var now = DateTimeOffset.Now;
        var currentSecond = new DateTimeOffset(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            now.Second,
            now.Offset);

        if (currentSecond == _lastTickSecond)
        {
            return;
        }

        _lastTickSecond = currentSecond;
        UpdateClock();
    }

    private void UpdateClock()
    {
        CurrentTime = FormatCurrentTime();
        CurrentDate = FormatCurrentDate(_culture);
    }

    private static string FormatCurrentTime()
    {
        return DateTimeOffset.Now.ToString("HH:mm:ss");
    }

    private static string FormatCurrentDate(CultureInfo culture)
    {
        return DateTimeOffset.Now.ToString("yyyy.MM.dd ddd", culture).ToUpperInvariant();
    }
}
