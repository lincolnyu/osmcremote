using System.Collections.Generic;
using System.Text;

namespace OsmcRemoteClassic
{
    public class RequestJson
    {
        #region Properties

        public int Id { get; set; } = 1;

        public string JsonRpc { get; set; } = "2.0";

        public string Method { get; set; }

        public IList<KeyValuePair<string, object>> Params { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{{\"jsonrpc\":\"{0}\",\"method\":\"{1}\",\"id\":{2}", JsonRpc, Method, Id);
            if (Params != null && Params.Count > 0)
            {
                sb.Append(",\"params\":{");
                for (var i = 0; i < Params.Count; i++)
                {
                    var p = Params[i];
                    var key = p.Key;
                    var val = p.Value;
                    if (val is string)
                    {
                        sb.AppendFormat("\"{0}\":\"{1}\"", key, val);
                    }
                    else
                    {
                        sb.AppendFormat("\"{0}\":{1}", key, val);
                    }

                    if (i < Params.Count-1)
                    {
                        sb.Append(',');
                    }
                }
                sb.Append("}");
            }
            sb.Append("}");
            return sb.ToString();
        }

        #endregion
    }
}
