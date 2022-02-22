using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using URemote.Desktop.XPlat.ViewModels;
using URemote.Desktop.XPlat.Views;

namespace URemote.Desktop.XPlat.Views
{
    public class HostNamePrompt : Window
    {
        public HostNamePrompt()
        {
            Owner = MainWindow.Current;
            InitializeComponent();
        }

        public HostNamePromptViewModel ViewModel => DataContext as HostNamePromptViewModel;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
