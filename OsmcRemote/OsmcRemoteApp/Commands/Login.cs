using OsmcRemoteApp.Helpers;
using System;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace OsmcRemoteApp.Commands
{
    public class Login : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return ((App)Application.Current).Settings.CredentialsLoaded;
        }

        public void Execute(object parameter)
        {
            DialogsAndCommandsHelper.RunLogin();
        }
    }
}
