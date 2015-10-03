using JsonParser.Helpers;
using JsonParser.JsonStructures;
using System.Collections.Generic;
using System.Linq;

namespace OsmcRemote.Results
{
    public class ItemsResult : Result
    {
        public List<Item> Items { get; private set; } = new List<Item>();

        public Limits Limits { get; private set; } = new Limits();

        public void LoadFromJson(JsonPairs jsPairs)
        {
            Items.Clear();
            var jsItems = jsPairs.GetNodeOrNull<JsonArray>("items");
            if (jsItems != null)
            {
                foreach (var jsItem in jsItems.Items.OfType<JsonPairs>())
                {
                    var item = new Item();
                    item.LoadFromJson(jsItem);
                    Items.Add(item);
                }
            }

            var jsLimits = jsPairs.GetNodeOrNull<JsonPairs>("limits");
            if (jsLimits != null)
            {
                Limits.End = jsLimits.GetValueOrDefault<int>("end");
                Limits.Start = jsLimits.GetValueOrDefault<int>("start");
                Limits.Total = jsLimits.GetValueOrDefault<int>("total");
            }
        }
    }
}
