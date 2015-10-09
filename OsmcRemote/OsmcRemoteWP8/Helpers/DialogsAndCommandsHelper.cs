using OsmcRemoteWP8.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OsmcRemoteWP8.Helpers
{
    class DialogsAndCommandsHelper
    {
        public static async Task ShowAuthenticationDialog()
        {
            var dialog = new AuthenticationDialog();
            var res = await dialog.ShowAsync();
            if (res == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                Connect();
            }
        }

        public static void Connect()
        {
            var client = ((App)Application.Current).Client;
            var settings = ((App)Application.Current).Settings;
            client.Connect(settings.ServerAddress, settings.UserName, settings.Password);
        }

        public static void Disconnect()
        {
            var client = ((App)Application.Current).Client;
            client.Disconnect();
        }

    }
}
