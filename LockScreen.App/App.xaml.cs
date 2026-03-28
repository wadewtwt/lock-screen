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

        // 捕获未处理异常，写入日志便于排查
        AppDomain.CurrentDomain.UnhandledException += (_, ex) =>
            _logger?.Fatal(ex.ExceptionObject as Exception, "Unhandled domain exception");

        DispatcherUnhandledException += (_, ex) =>
        {
            _logger?.Fatal(ex.Exception, "Unhandled dispatcher exception");
            ex.Handled = true;          // 不让程序崩掉，继续运行
        };

        try
        {
            _sharedViewModel = new MainWindowViewModel(_logger);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            RebuildLockWindows();
            _logger.Information("Application started.");
        }
        catch (Exception ex)
        {
            _logger?.Fatal(ex, "Fatal error during startup");
            System.Windows.MessageBox.Show($"启动失败：{ex.Message}\n\n详情见 logs/ 目录", "LockScreen Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            Shutdown(1);
        }
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
