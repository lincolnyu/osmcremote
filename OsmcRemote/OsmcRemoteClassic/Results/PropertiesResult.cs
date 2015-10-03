using JsonParser.Helpers;
using JsonParser.JsonStructures;
using System;

namespace OsmcRemoteClassic.Results
{
    public class PropertiesResult : Result
    {
        public int PlayListId { get; set; }

        public int Position { get; set; }

        public int Speed { get; set; }

        public TimeSpan Time { get; set; }

        public TimeSpan TotalTime { get; set; }

        public void LoadFromJson(JsonPairs jsonPairs)
        {
            PlayListId = jsonPairs.GetValueOrDefault<int>("playlistid");
            Position = jsonPairs.GetValueOrDefault<int>("position");
            Speed = jsonPairs.GetValueOrDefault<int>("speed");
            var timeNode = jsonPairs.GetNodeOrNull<JsonPairs>("time");
            if (timeNode != null)
            {
                Time = LoadTimeFromJson(timeNode);
            }
            var totalTimeNode = jsonPairs.GetNodeOrNull<JsonPairs>("totaltime");
            if (totalTimeNode != null)
            {
                TotalTime = LoadTimeFromJson(totalTimeNode);
            }
        }

        public static TimeSpan LoadTimeFromJson(JsonPairs jsonPairs)
        {
            var hours = jsonPairs.GetValueOrDefault<int>("hours");
            var minutes = jsonPairs.GetValueOrDefault<int>("minutes");
            var seconds = jsonPairs.GetValueOrDefault<int>("seconds");
            var mils = jsonPairs.GetValueOrDefault<int>("milliseconds");
            var days = hours / 24;
            hours = hours % 24;
            return new TimeSpan(days, hours, minutes, seconds, mils);
        }
    }
}
