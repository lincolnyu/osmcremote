using JsonParser.Helpers;
using JsonParser.JsonStructures;
using System.Collections.Generic;
using System.Linq;

namespace OsmcRemoteClassic.Results
{
    public class PlayersResult : Result
    {
        public List<Player> Players { get; private set; } = new List<Player>();

        public void LoadFromJson(JsonArray jsonArray)
        {
            Players.Clear();
            foreach (var jsItem in jsonArray.Items.OfType<JsonPairs>())
            {
                var player = new Player();
                player.PlayerId = jsItem.GetValueOrDefault<int>("playerid");
                player.Type = jsItem.GetValueOrDefault<string>("type");
                Players.Add(player);
            }
        }
    }
}
