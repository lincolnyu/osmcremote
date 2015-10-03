using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OsmcRemoteApp.Views
{
    public sealed partial class AuthenticationDialog : ContentDialog
    {
        public AuthenticationDialog()
        {
            InitializeComponent();
        }


        private void OkButtonOnClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(ServerAddressTextBox.Text) && string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                return;
            }
            var settings = ((App)Application.Current).Settings;
            settings.ServerAddress = ServerAddressTextBox.Text;
            settings.UserName = UserNameTextBox.Text;
            settings.Password = PasswordBox.Password;
        }

        private void CancelButtonOnClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void DialogLoaded(object sender, RoutedEventArgs e)
        {
            var settings = ((App)Application.Current).Settings;
            ServerAddressTextBox.Text = settings.ServerAddress;
            UserNameTextBox.Text = settings.UserName;
            PasswordBox.Password = settings.Password;
        }
    }
}
