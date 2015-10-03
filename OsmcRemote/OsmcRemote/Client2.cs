using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace OsmcRemote
{
    public class Client2 : IDisposable
    {
        #region Fields

        public Windows.Web.Http.HttpClient _httpClient;

        #endregion

        #region Constructors

        public Client2(string address, string username, string password)
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


        #endregion

        #region Methods

        #region IDisposable members

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
        }

        #endregion

        public async Task<HttpResponseMessage> Connect()
        {
            var httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            _httpClient = new HttpClient(httpFilter);

            var credentialString = string.Format("{0}:{1}", UserName, Password);
            var byteArray = Encoding.UTF8.GetBytes(credentialString);

            var uri = new Uri(MainUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Clear();
            request.Headers.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //request.Headers.Remove("If-Modified-Since");
            //request.Headers.IfModifiedSince = DateTimeOffset.Now;
            var response = await _httpClient.SendRequestAsync(request);
            return response;
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
            return await Post("Input.Select");
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

        public async Task<ResponseJson> ShutDown()
        {
            return await Post("System.Shutdown");
        }

        public async Task<ResponseJson> Post(string method, params KeyValuePair<string, object>[] kvps)
        {
            var url = GetRpcUrl(method);

            var requestJson = new RequestJson { Method = method, Id = Id };
            requestJson.Params = kvps;

            var content = new HttpStringContent(requestJson.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8); //"application/json"
            var uri = new Uri(url);
            var response = await _httpClient.PostAsync(uri, content);
            var resp = await response.Content.ReadAsStringAsync();
            var responseJson = new ResponseJson(resp);
            return responseJson;
        }

        private string GetRpcUrl(string command)
        {
            return string.Format("{0}{1}", RpcUrl, command);
        }

        #endregion
    }
}
