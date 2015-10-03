using JsonParser;
using JsonParser.Helpers;
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

        public JsonNode ResultJson { get; private set; }

        public Result Result { get; private set; }

        #endregion

        #region Methods

        public void FromString(string str)
        {
            var jsPairs = str.ParseJson() as JsonPairs;
            if (jsPairs == null)
            {
                // TODO should report the error
                return;
            }

            Id = jsPairs.GetValueOrDefault<int>("id");
            JsonRpc = jsPairs.GetValueOrDefault<string>("jsonrpc");
            ResultJson = jsPairs.GetNodeOrNull("result");
            Result = GenerateResultObject();            
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
