using OsmcRemote.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OsmcRemote
{
    public class Client : IDisposable, INotifyPropertyChanged
    {
        #region Nested types

        public class CheckedConnectionEventArgs : EventArgs
        {
            public CheckedConnectionEventArgs(HttpResponseMessage response)
            {
                Response = response;
            }

            public HttpResponseMessage Response { get; private set; }
        }

        public delegate void CheckedConnectionEventHandler(object sender, CheckedConnectionEventArgs args);

        #endregion

        #region Fields

        public HttpClient _httpClient;

        private bool _playersActive;
        private bool _isConnected;

        /// <summary>
        ///  If the connection checker is running
        /// </summary>
        private bool _isCheckerRunning;

        private Task _checkingloopTask;

        #endregion

        #region Constructors

        public Client(string address, string username, string password)
        {
            Address = address;
            UserName = username;
            Password = password;
        }

        #endregion

        #region Properties

        public string Address { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public int Id { get; private set; } = 1;

        public string MainUrl
        {
            get { return string.Format("http://{0}/", Address); }
        }

        public string RpcUrl
        {
            get { return string.Format("http://{0}/jsonrpc?", Address); }
        }

        /// <summary>
        ///  In seconds
        /// </summary>
        public int PlayersCheckInterval { get; set; } = 5;

        public PlayersResult CurrentPlayers { get; private set; }

        public PropertiesResult CurrentProperties { get; private set; }

        public ItemsResult CurrentItems { get; private set; }

        public bool PlayersActive
        {
            get { return _playersActive;  }
            set
            {
                if (_playersActive != value)
                {
                    _playersActive = value;
                    RaisePropertyChangedEvent("PlayersActive");
                }
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    RaisePropertyChangedEvent("IsConnected");
                }
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler CheckingConnection;

        public event CheckedConnectionEventHandler CheckedConnection;

        #endregion

        #region Methods

        #region IDisposable members

        public void Dispose()
        {
            Close().Wait();
        }

        #endregion

        public void Start()
        {
            _isCheckerRunning = true;
            _checkingloopTask = TimerDrivenUpdateLoop();
        }

        public async Task Close()
        {
            _isCheckerRunning = false;
            if (_checkingloopTask != null)
            {
                await _checkingloopTask;
            }
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
                IsConnected = false;
            }
        }

        private async Task<HttpResponseMessage> Connect()
        {
            _httpClient = new HttpClient();
            var credentialString = string.Format("{0}:{1}", UserName, Password);

            var byteArray = GetAsciiBytes(credentialString).ToArray();
            var encodedCredentials = Convert.ToBase64String(byteArray);
            System.Diagnostics.Debug.WriteLine("Encoded credentials: {0}", encodedCredentials);
            // NOTE it's supposed to be basic authentication
            var auth = new AuthenticationHeaderValue("Basic", encodedCredentials);

            var defHeader = _httpClient.DefaultRequestHeaders;
            defHeader.Authorization = auth;
            // the code below guarantees a return of OK, but that's not essential
            //var cacheControl = new CacheControlHeaderValue();
            //cacheControl.NoCache = true;
            //defHeader.CacheControl = cacheControl;

            try
            {
                var response = await _httpClient.GetAsync(MainUrl, HttpCompletionOption.ResponseHeadersRead);
                IsConnected = response.IsSuccessStatusCode;
                return response;
            }
            catch (Exception)
            {
                IsConnected = false;
                return null;
            }
        }

        private static IEnumerable<byte> GetAsciiBytes(string str)
        {
            foreach (var c in str)
            {
                var b = (byte)c;
                yield return b;
            }
        }

        public async Task<ResponseJson> InputBack()
        {
            return await Post("Input.Back");
        }

        public async Task<ResponseJson> InputHome()
        {
            return await Post("Input.Home");
        }

        public async Task<ResponseJson> InputUp()
        {
            return await Post("Input.Up");
        }

        public async Task<ResponseJson> InputDown()
        {
            return await Post("Input.Down");
        }

        public async Task<ResponseJson> InputLeft()
        {
            return await Post("Input.Left");
        }

        public async Task<ResponseJson> InputRight()
        {
            return await Post("Input.Right");
        }

        public async Task<ResponseJson> InputOk()
        {
            var resp =  await Post("Input.Select");
            // this may change the status
            await UpdatePlaybackStatus();
            return resp;
        }

        public async Task<ResponseJson> InputVolumeUp()
        {
            return await Post("Application.SetVolume", new KeyValuePair<string, object>("volume", "increment"));
        }

        public async Task<ResponseJson> InputVolumeDown()
        {
            return await Post("Application.SetVolume", new KeyValuePair<string, object>("volume", "decrement"));
        }

        public async Task<ResponseJson> SetMute()
        {
            return await Post("Application.SetMute", new KeyValuePair<string, object>("mute", "toggle"));
        }

        public async Task<ResponseJson> SetSpeedIncrement()
        {
            return await SetSpeed(true);
        }

        public async Task<ResponseJson> SetSpeedDecrement()
        {
            return await SetSpeed(false);
        }

        private async Task<ResponseJson> SetSpeed(bool increment)
        {
            var playerId = GetPlayerId();
            if (playerId < 0)
            {
                return null;
            }
            return await Post("Player.SetSpeed", new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("playerid", playerId),
                new KeyValuePair<string, object>("speed", increment? "increment":"decrement")
            });
        }

        public async Task<ResponseJson> ShutDown()
        {
            var res = await Post("System.Shutdown");
            IsConnected = false;
            return res;
        }

        public async Task<ResponseJson> PlayPause()
        {
            var playerId = GetPlayerId();
            if (playerId < 0)
            {
                return null;
            }

            return await Post("Player.PlayPause", new KeyValuePair<string, object>("playerid", playerId));
        }

        public async Task<ResponseJson> Stop()
        {
            var playerId = GetPlayerId();
            if (playerId < 0)
            {
                return null;
            }

            return await Post("Player.Stop", new KeyValuePair<string, object>("playerid", playerId));
        }
        public async Task<ResponseJson> GoToPrevious()
        {
            return await GoTo(false);
        }

        public async Task<ResponseJson> GoToNext()
        {
            return await GoTo(true);
        }

        private async Task<ResponseJson> GoTo(bool next)
        {
            var playerId = GetPlayerId();
            if (playerId < 0)
            {
                return null;
            }
            return await Post("Player.GoTo", new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("playerid", playerId),
                new KeyValuePair<string, object>("to", next? "next":"previous")
            });
        }

        private int GetPlayerId()
        {
            if (!PlayersActive)
            {
                return -1;
            }

            var playerId = CurrentPlayers.Players[0].PlayerId;
            return playerId;
        }
      
        public async Task<ResponseJson> Post(string method, params KeyValuePair<string, object>[] kvps)
        {
            try
            {
                return await PostNoThrow(method, kvps);
            }
            catch (HttpRequestException)
            {
                IsConnected = false;
                return null;
            }
        }

        private async Task<ResponseJson> PostNoThrow(string method, params KeyValuePair<string, object>[] kvps)
        {
            var url = GetRpcUrl(method);

            var requestJson = new RequestJson { Method = method, Id = Id };
            requestJson.Params = kvps;

            var content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var resp = await response.Content.ReadAsStringAsync();
            var responseJson = new ResponseJson(resp);
            return responseJson;
        }

        public async Task UpdatePlaybackStatus()
        {
            try
            {
                var respPlayers = await PostNoThrow("Player.GetActivePlayers");
                CurrentPlayers = respPlayers.Result as PlayersResult;
                PlayersActive = CurrentPlayers != null && CurrentPlayers.Players.Count > 0;
                if (CurrentPlayers == null)
                {
                    return;
                }

                var respProperties = await PostNoThrow("Player.GetProperties");
                CurrentProperties = respProperties.Result as PropertiesResult;

                var respItems = await PostNoThrow("Player.GetItems");
                CurrentItems = respItems.Result as ItemsResult;
            }
            catch (HttpRequestException)
            {
                IsConnected = false;
            }
        }

        private string GetRpcUrl(string command)
        {
            return string.Format("{0}{1}", RpcUrl, command);
        }

        private async Task TimerDrivenUpdateLoop()
        {
            while (_isCheckerRunning)
            {
                await TimerDrivenUpdate(null);
                await Task.Delay(PlayersCheckInterval * 1000);
            }
        }
        
        private async Task TimerDrivenUpdate(object state)
        {
            if (IsConnected)
            {
                await UpdatePlaybackStatus();
            }
            else
            {
                CheckingConnection?.Invoke(this, new EventArgs());

                var resp = await Connect();

                CheckedConnection?.Invoke(this, new CheckedConnectionEventArgs(resp));
            }
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
