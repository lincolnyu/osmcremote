using JsonParser.Helpers;
using JsonParser.JsonStructures;

namespace OsmcRemote.Results
{
    public class Item
    {
        public string Episode { get; set; }

        public string Label { get; set; }

        public string Plot { get; set; }

        public int RunTime { get; set; }

        public int Season { get; set; }

        public string ShowTitle { get; set; }

        public string Thumbnail { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public void LoadFromJson(JsonPairs jsPairs)
        {
            Episode = jsPairs.GetValueOrDefault<string>("episode");
            Label = jsPairs.GetValueOrDefault<string>("label");
            Plot = jsPairs.GetValueOrDefault<string>("plot");
            RunTime = jsPairs.GetValueOrDefault<int>("runtime");
            Season = jsPairs.GetValueOrDefault<int>("season");
            ShowTitle = jsPairs.GetValueOrDefault<string>("showtitle");
            Thumbnail = jsPairs.GetValueOrDefault<string>("thumbnail");
            Title = jsPairs.GetValueOrDefault<string>("title");
            Type = jsPairs.GetValueOrDefault<string>("type");
        }
    }
}
