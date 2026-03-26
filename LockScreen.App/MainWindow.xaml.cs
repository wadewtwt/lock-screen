using System.Windows;
using System.Windows.Input;
using LockScreen.App.ViewModels;

namespace LockScreen.App;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += (_, _) => Keyboard.Focus(this);
    }

    private void Window_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.HandleKeyPressCommand.Execute(e.Key);
        }
    }
}
