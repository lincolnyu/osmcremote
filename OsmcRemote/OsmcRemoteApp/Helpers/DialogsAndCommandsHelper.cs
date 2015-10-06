using OsmcRemoteApp.Commands;
using OsmcRemoteApp.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace OsmcRemoteApp.Helpers
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
