using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using LockScreen.App.Native;
using LockScreen.App.ViewModels;

namespace LockScreen.App;

public partial class MainWindow : Window
{
    private readonly System.Drawing.Rectangle _screenBounds;

    public MainWindow(MainWindowViewModel viewModel, System.Drawing.Rectangle screenBounds)
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
        Loaded += (_, _) =>
        {
            Activate();
            Focus();
            Keyboard.Focus(this);
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
}
