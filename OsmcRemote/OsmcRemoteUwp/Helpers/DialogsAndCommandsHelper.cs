using OsmcRemoteUwp.Commands;
using OsmcRemoteUwp.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OsmcRemoteUwp.Helpers
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

        public static async void Connect()
        {
            var client = ((App)Application.Current).Client;
            var settings = ((App)Application.Current).Settings;
            await client.Connect(settings.ServerAddress, settings.UserName, settings.Password);
        }

        public static async void Disconnect()
        {
            var client = ((App)Application.Current).Client;
            await client.Disconnect();
        }
    }
}
