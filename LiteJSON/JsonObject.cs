using System;
using System.Collections.Generic;

namespace LiteJSON
{
    public class JsonObject
    {
        private Dictionary<string, object> _dict;

        public JsonObject()
        {
            _dict = new Dictionary<string, object>();
        }

        public void Put<T>(string key, T value)
        {
            _dict.Add(key, value);
        }

        public T Get<T>(string key)
        {
            return (T)_dict[key];
        }

        public object Get(string key)
        {
            return _dict[key];
        }

        public Dictionary<string, object>.KeyCollection Keys
        {
            get { return _dict.Keys; }
        }
    }
}

