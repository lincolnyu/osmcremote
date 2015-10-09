using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using OsmcRemoteAppCommon.Data;
using OsmcRemoteWP8.Commands;
using OsmcRemoteWP8.Helpers;

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

        private bool _blinkingStoryboardRunning;
        private Storyboard _blinkingStoryboard;

        #endregion

        #region Constructors

        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            SetupCommands();

            SetupEventHandling();

            SetupStoryboard();

            DataContext = this;
        }

        private void SetupStoryboard()
        {
            var anim = new DoubleAnimation();
            var duration = new Duration(TimeSpan.FromMilliseconds(500));
            anim.From = 1.0;
            anim.To = 0.0;
            anim.Duration = duration;
            anim.AutoReverse = true;
            anim.RepeatBehavior = RepeatBehavior.Forever;
            _blinkingStoryboard = new Storyboard();
            _blinkingStoryboard.Children.Add(anim);
            Storyboard.SetTarget(anim, PowerBorder);
            Storyboard.SetTargetProperty(anim, "Opacity");

            if (Client.ConnectionIndicatorStatus == ConnectionIndicatorStatuses.Checking)
            {
                _blinkingStoryboard.Begin();
                _blinkingStoryboardRunning = true;
            }
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

        public bool IsConnected
        {
            get
            {
                return Client.IsConnected;
            }
        }

        public bool PlayersActive
        {
            get
            {
                return Client.PlayersActive;
            }
        }

        public ConnectionIndicatorStatuses ConnectionIndicatorStatus
        {
            get
            {
                return Client.ConnectionIndicatorStatus;
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

            base.OnNavigatedTo(args);

            DialogsAndCommandsHelper.Connect();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DialogsAndCommandsHelper.Disconnect();

            base.OnNavigatedFrom(e);
        }

        private async void ShowAuthenticationDialog()
        {
            if (!Settings.CredentialsLoaded)
            {
                await DialogsAndCommandsHelper.ShowAuthenticationDialog();
            }
        }

        private void OkButtonSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateSmallButtonSize(args.NewSize);
        }

        private void UpdateSmallButtonSize(Size largeButtonSize)
        {
            var h = largeButtonSize.Height / 2;
            var w = h * 2;
            if (w > largeButtonSize.Width)
            {
                w = largeButtonSize.Width;
            }
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

        private async void ClientOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "IsConnected":
                case "PlayersActive":
                case "ConnectionIndicatorStatus":
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    { RaisePropertyChangedEvent(args.PropertyName); });
                    break;
            }
            if (args.PropertyName == "ConnectionIndicatorStatus")
            {
                if (Client.ConnectionIndicatorStatus == ConnectionIndicatorStatuses.Checking)
                {
                    if (!_blinkingStoryboardRunning)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () => _blinkingStoryboard.Begin());
                        _blinkingStoryboardRunning = true;
                    }
                }
                else
                {
                    if (_blinkingStoryboardRunning)
                    {   
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () => _blinkingStoryboard.Stop());
                        _blinkingStoryboardRunning = false;
                    }
                }
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

        private async void SetSpeedIncrementOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.SetSpeedIncrement();
        }

        private async void SetSpeedDecrementOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.SetSpeedDecrement();
        }
        private async void GoToPreviousOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.GoToPrevious();
        }

        private async void GoToNextOnClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.GoToNext();
        }

        private async void MuteButtonClick(object sender, RoutedEventArgs e)
        {
            await Client.Client.SetMute();
        }

        #endregion

        #endregion
    }
}
