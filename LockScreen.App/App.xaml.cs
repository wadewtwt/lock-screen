using System.IO;
using System.Windows;
using LockScreen.App.ViewModels;
using Serilog;

namespace LockScreen.App;

public partial class App : Application
{
    private ILogger? _logger;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Directory.CreateDirectory("logs");

        _logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/lockscreen-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var viewModel = new MainWindowViewModel(_logger);
        var window = new MainWindow(viewModel);

        MainWindow = window;
        window.Show();

        _logger.Information("Application started.");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _logger?.Information("Application stopped.");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
