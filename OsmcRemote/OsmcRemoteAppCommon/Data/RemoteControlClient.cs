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

        private bool _connected;
        private bool _playersActive;

        #endregion

        #region Properties

        public bool Connected
        {
            get
            {
                return _connected;
            }
            private set
            {
                if (_connected != value)
                {
                    _connected = value;
                    RaisePropertyChangedEvent("Connected");
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
                Connected = false;
            }
            Client = new Client(serverAddress, userName, password);
            PlayersActive = Client.PlayersActive;
            Client.PropertyChanged += ClientPropertyChanged;
            var resp = await Client.Connect();
            Connected = resp.IsSuccessStatusCode;
            return resp.StatusCode;
        }

        private void ClientPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "PlayersActive")
            {
                PlayersActive = Client.PlayersActive;
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
