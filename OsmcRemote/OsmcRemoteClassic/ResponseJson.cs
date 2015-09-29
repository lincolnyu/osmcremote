namespace OsmcRemoteClassic
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

        public string Result { get; set; }

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
                        Result = val.Trim('"');
                        break;
                }
            }
        }

        #endregion
    }
}
