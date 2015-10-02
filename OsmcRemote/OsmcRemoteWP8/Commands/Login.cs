using System;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace OsmcRemoteWP8.Commands
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
            var client = ((App)Application.Current).Client;
            var settings = ((App)Application.Current).Settings;
            var task = client.Login(settings.ServerAddress, settings.UserName, settings.Password);
            task.Wait();
            var successful = task.Result;
            if (!successful)
            {
                var md = new MessageDialog("Error connecting to server");
            }
        }
    }
}
