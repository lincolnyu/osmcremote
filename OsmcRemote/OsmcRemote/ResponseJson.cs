using JsonParser;
using JsonParser.JsonStructures;
using OsmcRemote.Results;

namespace OsmcRemote
{
    public class ResponseJson
    {
        #region Constructors

        public ResponseJson(string body)
        {
            FromString(body);
        }

        #endregion

        #region Properties

        public int Id { get; set; } = 1;

        public string JsonRpc { get; set; } = "2.0";

        public string ResultJsonString { get; private set; }

        public JsonNode ResultJson { get; private set; }

        public Result Result { get; private set; }

        #endregion

        #region Methods

        public void FromString(string str)
        {
            str = str.TrimStart('{').TrimEnd('}');
            var kvps = str.Split(',');
            foreach (var kvp in kvps)
            {
                var kvs = kvp.Split(':');
                var key = kvs[0];
                var val = kvs[1];
                key = key.Trim('"').ToLower();
                switch (key)
                {
                    case "id":
                        {
                            int ival;
                            if (int.TryParse(val, out ival))
                            {
                                Id = ival;
                            }
                            break;
                        }
                    case "jsonrpc":
                        JsonRpc = val.Trim('"');
                        break;
                    case "result":
                        {
                            ResultJsonString = val;
                            ResultJson = ResultJsonString.ParseJson();
                            Result = GenerateResultObject();
                        }
                        break;
                }
            }
        }

        private Result GenerateResultObject()
        {
            var rjp = ResultJson as JsonPairs;
            if (rjp != null)
            {
                if (rjp.KeyValues.ContainsKey("items"))
                {
                    var res = new ItemsResult();
                    res.LoadFromJson(rjp);
                    return res;
                }
                else if (rjp.KeyValues.ContainsKey("playlistid"))
                {
                    var res = new PropertiesResult();
                    res.LoadFromJson(rjp);
                    return res;
                }
            }
         

            var rja = ResultJson as JsonArray;
            if (rja != null)
            {
                // should be players, which is the only type that presents as an array
                var res = new PlayersResult();
                res.LoadFromJson(rja);
                return res;
            }

            return null;
        }

        #endregion
    }
}
