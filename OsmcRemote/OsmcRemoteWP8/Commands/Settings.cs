using System;
using System.Windows.Input;
using OsmcRemoteWP8.Views;
using OsmcRemoteWP8.Helpers;
using Windows.UI.Xaml;

namespace OsmcRemoteWP8.Commands
{
    public class Settings : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await DialogsHelper.ShowAuthenticationDialog(((App)Application.Current).LoginCommand);
        }
    }
}
