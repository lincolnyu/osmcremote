using OsmcRemoteWP8.Commands;
using OsmcRemoteWP8.Data;
using OsmcRemoteWP8.Helpers;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace OsmcRemoteWP8
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
                    RaisePropertyChanged("SmallButtonWidth");
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
                    RaisePropertyChanged("SmallButtonHeight");
                }
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
                await DialogsHelper.ShowAuthenticationDialog(((App)Application.Current).LoginCommand);
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

        private void RaisePropertyChanged(string propertyName)
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

        #region Button click event handlers

        private void OkButtonOnClick(object sender, RoutedEventArgs e)
        {
            Client.Client.InputOk().Wait();
        }

        #endregion

        #endregion

    }
}
