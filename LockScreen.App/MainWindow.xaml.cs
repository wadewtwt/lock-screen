using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using LockScreen.App.Native;
using LockScreen.App.ViewModels;

namespace LockScreen.App;

public partial class MainWindow : Window
{
    private readonly ScreenBounds _screenBounds;
    private readonly DispatcherTimer _settingsHintCycleTimer;
    private readonly DispatcherTimer _settingsHintHideTimer;

    public MainWindow(MainWindowViewModel viewModel, ScreenBounds screenBounds)
    {
        InitializeComponent();
        DataContext = viewModel;
        _screenBounds = screenBounds;
        WindowStartupLocation = WindowStartupLocation.Manual;
        Left = screenBounds.Left;
        Top = screenBounds.Top;
        Width = screenBounds.Width;
        Height = screenBounds.Height;
        SourceInitialized += OnSourceInitialized;
        _settingsHintCycleTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _settingsHintCycleTimer.Tick += (_, _) => ShowSettingsHintIfNeeded();

        _settingsHintHideTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1.8)
        };
        _settingsHintHideTimer.Tick += (_, _) =>
        {
            _settingsHintHideTimer.Stop();
            SettingsHintBubble.Visibility = Visibility.Collapsed;
        };

        Loaded += (_, _) =>
        {
            Activate();
            Focus();
            Keyboard.Focus(this);
            ShowSettingsHintIfNeeded();
            _settingsHintCycleTimer.Start();
        };
        Unloaded += (_, _) =>
        {
            _settingsHintCycleTimer.Stop();
            _settingsHintHideTimer.Stop();
        };
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        WindowPlacement.MoveToScreenBounds(handle, _screenBounds);
    }

    private void Window_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.HandleKeyPressCommand.Execute(e.Key);
        }
    }

    private void Window_OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel || !viewModel.IsSettingsPanelOpen)
        {
            return;
        }

        if (e.Key is not (Key.Enter or Key.Escape))
        {
            return;
        }

        viewModel.HandleKeyPressCommand.Execute(e.Key);
        e.Handled = true;
    }

    private void SettingsOverlay_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.CloseSettingsPanelCommand.Execute(null);
        }
    }

    private void ShowSettingsHintIfNeeded()
    {
        if (DataContext is MainWindowViewModel { IsSettingsPanelOpen: true })
        {
            SettingsHintBubble.Visibility = Visibility.Collapsed;
            return;
        }

        SettingsHintBubble.Visibility = Visibility.Visible;
        _settingsHintHideTimer.Stop();
        _settingsHintHideTimer.Start();
    }
}
