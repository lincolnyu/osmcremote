using OsmcRemoteApp.Commands;
using OsmcRemoteAppCommon.Data;
using OsmcRemoteApp.Helpers;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OsmcRemoteApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region Fields

        private double _smallButtonWidth;

        private double _smallButtonHeight;

        #endregion

        #region Constructors

        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            SetupCommands();

            SetupEventHandling();

            DataContext = this;
        }

        #endregion

        #region Properties

        public double SmallButtonWidth
        {
            get
            {
                return _smallButtonWidth;
            }
            set
            {
                if (_smallButtonWidth != value)
                {
                    _smallButtonWidth = value;
                    RaisePropertyChangedEvent("SmallButtonWidth");
                }
            }
        }

        public double SmallButtonHeight
        {
            get
            {
                return _smallButtonHeight;
            }
            set
            {
                if (_smallButtonHeight != value)
                {
                    _smallButtonHeight = value;
                    RaisePropertyChangedEvent("SmallButtonHeight");
                }
            }
        }

        public bool AreButtonsEnabled
        {
            get
            {
                return Client.Connected;
            }
        }

        public bool PlayersActive
        {
            get
            {
                return Client.PlayersActive;
            }
        }

        public SettingsData Settings
        {
            get
            {
                return ((App)Application.Current).Settings;
            }
        }

        public RemoteControlClient Client
        {
            get
            {
                return ((App)Application.Current).Client;
            }
        }


        public Settings SettingsCommand
        {
            get; set;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="args">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void ShowAuthenticationDialog()
        {
            if (!Settings.CredentialsLoaded)
            {
                await DialogsAndCommandsHelper.ShowAuthenticationDialog(((App)Application.Current).LoginCommand);
            }
        }

        private void OkButtonSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateSmallButtonSize(args.NewSize);
        }

        private void UpdateSmallButtonSize(Size largeButtonSize)
        {
            var w = largeButtonSize.Width / 3;
            var h = w / 2;
            SmallButtonWidth = w;
            SmallButtonHeight = h;
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OkButtonLayoutUpdated(object sender, object e)
        {
            UpdateSmallButtonSize(new Size(OkButton.ActualWidth, OkButton.ActualHeight));
        }

        private void SetupCommands()
        {
            SettingsCommand = new Settings();
        }

        private void PageOnLoaded(object sender, RoutedEventArgs e)
        {
            ShowAuthenticationDialog();
        }

        private void SetupEventHandling()
        {
            Client.PropertyChanged += ClientOnPropertyChanged;
        }

        private void ClientOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Connected")
            {
                RaisePropertyChangedEvent("AreButtonsEnabled");
            }
            else if (args.PropertyName == "PlayersActive")
            {
                RaisePropertyChangedEvent("PlayersActive");
            }
        }

        #region Button click event handlers

        private async void OkButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputOk();
        }

        private async void LeftButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputLeft();
        }

        private async void RightButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputRight();
        }

        private async void UpButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputUp();
        }

        private async void DownButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputDown();
        }

        private async void BackButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputBack();
        }

        private async void HomeButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputHome();
        }

        private async void VolumeUpOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputVolumeUp();
        }

        private async void VolumeDownOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.InputVolumeDown();
        }

        private async void PlayPauseOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.PlayPause();
        }

        private async void PowerButtonOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.ShutDown();
        }


        private async void StopOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.Stop();
        }

        #endregion

        #endregion
    }
}
