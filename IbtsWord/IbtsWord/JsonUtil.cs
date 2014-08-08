using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace IbtsWord
{
    class JsonUtil
    {
        public static Object getObject(string json)
        {
            JObject jobj = JObject.Parse(json);
            Object a = getChildObject(jobj);
            return a;
        }

        public static Object getChildObject(JToken jtoken)
        {
            if (jtoken.Type == JTokenType.Object)
            {
                Hashtable hs = new Hashtable();
                foreach (KeyValuePair<string, JToken> token in (JObject)jtoken)
                {
                    JToken token1 = token.Value;
                    Object a = getChildObject(token1);
                    hs.Add(token.Key, a);
                }
                return hs;
            }
            else if (jtoken.Type == JTokenType.Boolean)
            {
                return (Boolean)jtoken;
            }
            else if (jtoken.Type == JTokenType.String)
            {
                return jtoken.ToString();
            }
            else if (jtoken.Type == JTokenType.Array)
            {
                ArrayList list = new ArrayList();
                foreach (JToken token1 in (JArray)jtoken)
                {
                    Object a = getChildObject(token1);
                    list.Add(a);
                }
                return list;
            }
            return null;
        }
    }
}
