using OsmcRemote;
using System;
using System.Threading.Tasks;

namespace OsmcRemoteConsole
{
    class Program
    {
        static async Task DoIt()
        {
            using (var client = new Client("192.168.1.5", "family", "family"))
            {
                await client.Connect();

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

        static void Main(string[] args)
        {
            DoIt().Wait();
        }
    }
}
