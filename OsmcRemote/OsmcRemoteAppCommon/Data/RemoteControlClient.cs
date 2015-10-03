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
                Client.PropertyChanged -= ClientPropertyChanged;
                Client.Dispose();
                Client = null;
            }
        }

        public async Task<HttpStatusCode> Login(string serverAddress, string userName, string password)
        {
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
                IsConnected = false;
            }
            Client = new Client(serverAddress, userName, password);
            PlayersActive = Client.PlayersActive;
            Client.PropertyChanged += ClientPropertyChanged;
            var resp = await Client.Connect();
            return resp.StatusCode;
        }

        private void ClientPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "PlayersActive")
            {
                PlayersActive = Client.PlayersActive;
            }
            else if (args.PropertyName == "IsConnected")
            {
                IsConnected = Client.IsConnected;
            }
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
