using OsmcRemote;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel;
using System;

namespace OsmcRemoteAppCommon.Data
{
    public class RemoteControlClient : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private bool _isConnected;
        private bool _playersActive;
        private ConnectionIndicatorStatuses _connectionIndicatorStatus;

        #endregion

        #region Properties

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    RaisePropertyChangedEvent("IsConnected");
                }
            }
        }

        public ConnectionIndicatorStatuses ConnectionIndicatorStatus
        {
            get { return _connectionIndicatorStatus; }
            set
            {
                if (_connectionIndicatorStatus != value)
                {
                    _connectionIndicatorStatus = value;
                    RaisePropertyChangedEvent("ConnectionIndicatorStatus");
                }
            }
        }

        public bool PlayersActive
        {
            get
            {
                return _playersActive;
            }
            private set
            {
                if (_playersActive != value)
                {
                    _playersActive = value;
                    RaisePropertyChangedEvent("PlayersActive");
                }
            }
        }

        public Client Client { get; private set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        public void Dispose()
        {
            if (Client != null)
            {
                Client.PropertyChanged -= ClientOnPropertyChanged;
                Client.CheckingConnection -= ClientOnCheckingConnection;
                Client.CheckedConnection -= ClientOnCheckedConnection;
                Client.Dispose();
                Client = null;
            }
        }

        public void Login(string serverAddress, string userName, string password)
        {
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
                IsConnected = false;
            }
            Client = new Client(serverAddress, userName, password);
            PlayersActive = Client.PlayersActive;
            Client.PropertyChanged += ClientOnPropertyChanged;
            Client.CheckingConnection += ClientOnCheckingConnection;
            Client.CheckedConnection += ClientOnCheckedConnection;
            Client.Start();
        }

        private void ClientOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "PlayersActive")
            {
                PlayersActive = Client.PlayersActive;
            }
            else if (args.PropertyName == "IsConnected")
            {
                IsConnected = Client.IsConnected;
                UpdateIndicatorStatusOnConnectionStatusChange();
            }
        }

        private void ClientOnCheckedConnection(object sender, Client.CheckedConnectionEventArgs args)
        {
            UpdateIndicatorStatusOnConnectionStatusChange();
        }

        private void UpdateIndicatorStatusOnConnectionStatusChange()
        {
            ConnectionIndicatorStatus = Client.IsConnected ? ConnectionIndicatorStatuses.Connected
                : ConnectionIndicatorStatuses.Disconnected;
        }

        private void ClientOnCheckingConnection(object sender, EventArgs e)
        {
            ConnectionIndicatorStatus = ConnectionIndicatorStatuses.Checking;
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
