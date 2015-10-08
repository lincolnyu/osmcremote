using OsmcRemoteWP8.Commands;
using OsmcRemoteWP8.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OsmcRemoteWP8.Helpers
{
    class DialogsAndCommandsHelper
    {
        public static async Task ShowAuthenticationDialog(Login login)
        {
            var dialog = new AuthenticationDialog();
            await dialog.ShowAsync();

            if (login.CanExecute(null))
            {
                login.Execute(null);
            }
        }

        public static void RunLogin()
        {
            var client = ((App)Application.Current).Client;
            var settings = ((App)Application.Current).Settings;
            client.Login(settings.ServerAddress, settings.UserName, settings.Password);
        }
    }
}
