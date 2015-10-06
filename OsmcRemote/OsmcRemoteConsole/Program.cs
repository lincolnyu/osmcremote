using OsmcRemote;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OsmcRemoteConsole
{
    class Program
    {
        static AutoResetEvent ConnectionStatusChangedEvent = new AutoResetEvent(false);

        static async Task DoIt()
        {
            using (var client = new Client("192.168.1.5", "family", "family"))
            {
                client.Start();
                client.CheckedConnection += ClientCheckedConnection;

                ConnectionStatusChangedEvent.WaitOne();
                if (!client.IsConnected)
                {
                    Console.WriteLine("Failed to connect");
                    return;
                }

                while (true)
                {
                    Console.Write(">");
                    ResponseJson resp;
                    var s = Console.ReadLine();
                    if (s == "q") break;
                    if (s == "volup")
                    {
                        resp = await client.InputVolumeUp();
                    }
                    else if (s == "voldown")
                    {
                        resp = await client.InputVolumeDown();
                    }
                    else
                    {
                        resp = await client.Post(s);
                    }
                    Console.WriteLine("id={0}, jsonrpc={1}, result={2}", resp.Id, resp.JsonRpc, resp.ResultJson.ToString());
                }
            }
        }

        private static void ClientCheckedConnection(object sender, Client.CheckedConnectionEventArgs args)
        {
            ConnectionStatusChangedEvent.Set();
        }

        static void Main(string[] args)
        {
            DoIt().Wait();
        }
    }
}
