using System;
using System.Windows.Input;
using OsmcRemoteApp.Helpers;
using Windows.UI.Xaml;

namespace OsmcRemoteApp.Commands
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
            var login = ((App)Application.Current).LoginCommand;
            await DialogsAndCommandsHelper.ShowAuthenticationDialog(login);
        }
    }
}
