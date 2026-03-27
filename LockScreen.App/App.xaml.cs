using System.IO;
using System.Windows;
using Microsoft.Win32;
using FormsScreen = System.Windows.Forms.Screen;
using LockScreen.App.ViewModels;
using Serilog;

namespace LockScreen.App;

public partial class App : System.Windows.Application
{
    private ILogger? _logger;
    private readonly List<MainWindow> _windows = [];
    private MainWindowViewModel? _sharedViewModel;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Directory.CreateDirectory("logs");

        _logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/lockscreen-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _sharedViewModel = new MainWindowViewModel(_logger);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
        RebuildLockWindows();
        _logger.Information("Application started.");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        _logger?.Information("Application stopped.");
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            _logger?.Information("Display settings changed. Rebuilding lock windows.");
            RebuildLockWindows();
        });
    }

    private void RebuildLockWindows()
    {
        foreach (var window in _windows)
        {
            window.Close();
        }

        _windows.Clear();

        var screens = FormsScreen.AllScreens;
        foreach (var screen in screens)
        {
            var window = new MainWindow(_sharedViewModel!, screen.Bounds);
            _windows.Add(window);
            window.Show();
        }

        MainWindow = _windows.FirstOrDefault();
        _logger?.Information("Lock windows active on {ScreenCount} screen(s).", screens.Length);
    }
}
