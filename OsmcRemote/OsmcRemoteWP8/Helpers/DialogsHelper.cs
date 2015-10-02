using OsmcRemoteWP8.Commands;
using OsmcRemoteWP8.Views;
using System;
using System.Threading.Tasks;

namespace OsmcRemoteWP8.Helpers
{
    class DialogsHelper
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
    }
}
