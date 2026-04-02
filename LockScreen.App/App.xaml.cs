using System.IO;
using System.Windows;
using Microsoft.Win32;
using LockScreen.App.Native;
using LockScreen.App.ViewModels;
using Serilog;
using Serilog.Core;

namespace LockScreen.App;

public partial class App : System.Windows.Application
{
    private ILogger _logger = Logger.None;
    private readonly List<MainWindow> _windows = [];
    private MainWindowViewModel? _sharedViewModel;
    private string? _logDirectory;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            _logDirectory = GetWritableLogDirectory();
            Directory.CreateDirectory(_logDirectory);

            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(Path.Combine(_logDirectory, "lockscreen-.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
                _logger.Fatal(args.ExceptionObject as Exception, "Unhandled domain exception");

            DispatcherUnhandledException += (_, args) =>
            {
                _logger.Fatal(args.Exception, "Unhandled dispatcher exception");
                args.Handled = true;
            };

            _sharedViewModel = new MainWindowViewModel(_logger);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            RebuildLockWindows();
            _logger.Information("Application started. Log directory: {LogDirectory}", _logDirectory);
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "Fatal error during startup");
            var logHint = _logDirectory is null ? "Unable to determine log path." : $"Logs: {_logDirectory}";
            System.Windows.MessageBox.Show(
                $"Startup failed: {ex.Message}\n\n{logHint}",
                "LockScreen Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        _logger.Information("Application stopped.");
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            _logger.Information("Display settings changed. Rebuilding lock windows.");
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

        var screens = DisplayMonitor.GetAll();
        foreach (var screen in screens)
        {
            var window = new MainWindow(_sharedViewModel!, screen.Bounds);
            _windows.Add(window);
            window.Show();
        }

        MainWindow = _windows.FirstOrDefault();
        _logger.Information("Lock windows active on {ScreenCount} screen(s).", screens.Count);
    }

    private static string GetWritableLogDirectory()
    {
        var baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            baseDirectory = Path.GetTempPath();
        }

        return Path.Combine(baseDirectory, "DotLockPro", "Logs");
    }
}
