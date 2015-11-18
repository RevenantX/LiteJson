using System;
using System.Collections.Generic;

namespace LiteJSON
{
    public class JsonObject
    {
        private Dictionary<string, object> _dict;
        private string _typeName;
        private Type _type;

        public JsonObject()
        {
            _dict = new Dictionary<string, object>();
        }

        public JsonObject(Type type)
        {
            _dict = new Dictionary<string, object>();
            _type = type;
            _typeName = type.Name;
        }

        public JsonObject(Type type, bool fullTypeName)
        {
            _dict = new Dictionary<string, object>();
            _type = type;
            _typeName = fullTypeName ? type.FullName : type.Name;
        }

        public JsonObject(string source)
        {
            JsonParser parser = new JsonParser(new TypesInfo());
            _dict = parser.Parse(source)._dict;
        }

        public JsonObject(string source, TypesInfo typesInfo)
        {
            JsonParser parser = new JsonParser(typesInfo);
            _dict = parser.Parse(source)._dict;
        }

        public Dictionary<string, object>.KeyCollection Keys
        {
            get { return _dict.Keys; }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new SerializerConfig());
        }

        public string ToString(bool indent)
        {
            SerializerConfig config = new SerializerConfig();
            config.Indent = indent;
            return JsonSerializer.Serialize(this, config);
        }

        public string ToString(SerializerConfig config)
        {
            return JsonSerializer.Serialize(this, config);
        }

        public string TypeName
        {
            get { return _typeName; }
        }

        public Type TypeInfo
        {
            get { return _type; }
        }

        public void Put<T>(string key, T[] value)
        {
            if (value == null)
                _dict.Add(key, new JsonArray());
            else
                _dict.Add(key, JsonArray.FromArray(value));
        }

        public void Put<T>(string key, List<T> value)
        {
            if (value == null)
                _dict.Add(key, new JsonArray());
            else
                _dict.Add(key, JsonArray.FromList(value));
        }

        public void Put(string key, IJsonSerializable value)
        {
            _dict.Add(key, value.ToJson());
        }

        public void Put(string key, object value)
        {
            _dict.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _dict.Remove(key);
        }

        public bool Has(string key)
        {
            return _dict.ContainsKey(key);
        }

        public bool IsNull(string key)
        {
            return !_dict.ContainsKey(key) || _dict[key] == null;
        }

        public object Get(string key)
        {
            return _dict[key];
        }

        public T Deserialize<T>(string key) where T : IJsonDeserializable
        {
            return Json.Deserialize<T>((JsonObject) _dict[key]);
        }

        public JsonObject GetJsonObject(string key)
        {
            return (JsonObject)_dict[key];
        }

        public JsonArray GetJsonArray(string key)
        {
            return (JsonArray)_dict[key];
        }

        public int GetInt(string key)
        {
            object obj = _dict[key];
            if (obj is double)
                return (int)(double)obj;
            return (int)(long)obj;
        }

        public long GetLong(string key)
        {
            object obj = _dict[key];
            if (obj is double)
                return (long)(double)obj;
            return (long)obj;
        }

        public bool GetBool(string key)
        {
            return (bool)_dict[key];
        }

        public string GetString(string key)
        {
            return (string)_dict[key];
        }

        public float GetFloat(string key)
        {
            object obj = _dict[key];
            if (obj is long)
                return (long)obj;
            return (float)(double)obj;
        }

        public double GetDouble(string key)
        {
            object obj = _dict[key];
            if (obj is long)
                return (long)obj;
            return (double)obj;
        }


        public T GetEnum<T>(string key) where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), (string) _dict[key]);
        }

        public object GetEnum(string key, Type enumType)
        {
            return Enum.Parse(enumType, (string)_dict[key]);
        }

        public object Opt(string key)
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                return result;
            }
            return null;            
        }

        public int OptInt(string key, int defaultValue = 0)
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                return (int)(long)result;
            }
            return defaultValue;
        }

        public long OptLong(string key, long defaultValue = 0)
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                return (long)result;
            }
            return defaultValue;
        }

        public bool OptBool(string key, bool defaultValue = false)
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                return (bool)result;
            }
            return defaultValue;
        }

        public string OptString(string key, string defaultValue = "")
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                return (string)result;
            }
            return defaultValue;
        }

        public float OptFloat(string key, float defaultValue = 0f)
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                if (result is long)
                    return (long)result;
                return (float)(double)result;
            }
            return defaultValue;
        }

        public double OptDouble(string key, double defaultValue = 0.0)
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                if (result is long)
                    return (long)result;
                return (double)result;
            }
            return defaultValue;
        }

        public JsonObject OptJsonObject(string key)
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                return (JsonObject)result;
            }
            return null;
        }

        public JsonArray OptJsonArray(string key)
        {
            object result;
            if (_dict.TryGetValue(key, out result))
            {
                return (JsonArray)result;
            }
            return null;
        }
    }
}

