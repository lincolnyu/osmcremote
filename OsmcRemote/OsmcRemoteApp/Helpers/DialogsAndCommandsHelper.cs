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

        public static async Task RunLogin()
        {
            var client = ((App)Application.Current).Client;
            var settings = ((App)Application.Current).Settings;
            string errorMessage = null;
            try
            {
                var task = await client.Login(settings.ServerAddress, settings.UserName, settings.Password);
                if (!client.IsConnected)
                {
                    errorMessage = "Error connecting to server";
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Exception raises when connecting to server: {0}", e.Message);
            }
            if (errorMessage != null)
            {
                var md = new MessageDialog(errorMessage);
                await md.ShowAsync();
            }
        }
    }
}
