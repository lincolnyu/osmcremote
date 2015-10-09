using OsmcRemote;
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
            Disconnect();
        }

        public void Connect(string serverAddress, string userName, string password)
        {
            Disconnect();

            Client = new Client(serverAddress, userName, password);
            PlayersActive = Client.PlayersActive;
            Client.PropertyChanged += ClientOnPropertyChanged;
            Client.CheckingConnection += ClientOnCheckingConnection;
            Client.CheckedConnection += ClientOnCheckedConnection;
            Client.Start();
        }

        public void Disconnect()
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
