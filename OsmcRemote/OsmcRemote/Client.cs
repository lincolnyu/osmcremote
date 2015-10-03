﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OsmcRemote
{
    public class Client : IDisposable
    {
        #region Fields

        public HttpClient _httpClient;

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

        public int PlayerId { get; private set; } = 1;

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
            //var handler = new HttpClientHandler();
            //handler.UseDefaultCredentials = false;
            //_httpClient = new HttpClient(handler);

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
            
            var response = await _httpClient.GetAsync(MainUrl, HttpCompletionOption.ResponseHeadersRead);
            return response;
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

        public async Task<ResponseJson> PlayPause()
        {
            return await Post("Player.PlayPause", new KeyValuePair<string, object>("playerid", PlayerId));
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

            var content = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
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
