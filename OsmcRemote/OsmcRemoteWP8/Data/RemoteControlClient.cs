using System;
using OsmcRemoteClassic;
using System.Threading.Tasks;

namespace OsmcRemoteWP8.Data
{
    public class RemoteControlClient
    {

        public Client Client { get; private set; }

        internal async Task<bool> Login(string serverAddress, string userName, string password)
        {
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
            }
            Client = new Client(serverAddress, userName, password);
            var resp = await Client.Connect();
            return resp.IsSuccessStatusCode;
        }
    }
}
