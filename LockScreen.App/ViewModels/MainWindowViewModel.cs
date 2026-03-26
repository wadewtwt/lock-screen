using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace LockScreen.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly DispatcherTimer _clockTimer;

    [ObservableProperty]
    private string currentTime = DateTime.Now.ToString("HH:mm");

    [ObservableProperty]
    private string currentDate = DateTime.Now.ToString("yyyy-MM-dd dddd");

    [ObservableProperty]
    private string statusMessage = "This prototype exits immediately when you press Enter.";

    public MainWindowViewModel(ILogger logger)
    {
        _logger = logger;

        _clockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _clockTimer.Tick += (_, _) => UpdateClock();
        _clockTimer.Start();
    }

    [RelayCommand]
    private void HandleKeyPress(Key key)
    {
        if (key != Key.Enter)
        {
            return;
        }

        _logger.Information("Lock screen dismissed with Enter key.");
        Application.Current.Shutdown();
    }

    private void UpdateClock()
    {
        CurrentTime = DateTime.Now.ToString("HH:mm");
        CurrentDate = DateTime.Now.ToString("yyyy-MM-dd dddd");
    }
}
