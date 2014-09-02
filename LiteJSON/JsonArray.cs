using System.Collections.Generic;
using System.Collections;
using System;

namespace LiteJSON
{
    public class JsonArray : IEnumerable
    {
        private List<object> _list;

        public JsonArray()
        {
            _list = new List<object>();
        }

        private JsonArray(int capacity)
        {
            _list = new List<object>(capacity);
        }

        public static JsonArray FromList<T>(List<T> list)
        {
            int count = list.Count;
            JsonArray result = new JsonArray(count);
            bool serialize = typeof(IJsonSerializable).IsAssignableFrom(typeof(T));
            for (int i = 0; i < count; i++)
            {
                if (serialize)
                    result._list.Add(((IJsonSerializable)list[i]).ToJson());
                else
                    result._list.Add(list[i]);
            }
            return result; 
        }

        public static JsonArray FromArray<T>(T[] array)
        {
            int count = array.Length;
            JsonArray result = new JsonArray(count);
            bool serialize = typeof(IJsonSerializable).IsAssignableFrom(typeof(T));
            for (int i = 0; i < count; i++)
            {
                if (serialize)
                {
                    IJsonSerializable srz = (IJsonSerializable)array[i];
                    result._list.Add(srz.ToJson());
                }
                else
                    result._list.Add(array[i]);
            }
            return result;
        }

        public bool IsNull(int index)
        {
            if (index >= 0 && index < _list.Count && _list[index] != null)
            {
                return false;
            }
            return true;
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public void Add(object value)
        {
            _list.Add(value);
        }

        public object Get(int index)
        {
            return _list[index];
        }

        public void Remove(int index)
        {
            _list.RemoveAt(index);
        }

        public JsonObject GetJsonObject(int index)
        {
            return (JsonObject)_list[index];
        }

        public JsonArray GetJsonArray(int index)
        {
            return (JsonArray)_list[index];
        }

        public int GetInt(int index)
        {
            return (int)_list[index];
        }

        public long GetLong(int index)
        {
            return (long)_list[index];
        }

        public bool GetBool(int index)
        {
            return (bool)_list[index];
        }

        public string GetString(int index)
        {
            return (string)_list[index];
        }

        public float GetFloat(int index)
        {
            object obj = _list[index];
            if (obj is int)
                return (int)obj;
            else
                return (float)obj;
        }

        public double GetDouble(int index)
        {
            return (double)_list[index];
        }

        public object Opt(int index)
        {
            if (index >= 0 && index < _list.Count)
                return _list[index];
            return null;
        }

        public int OptInt(int index, int defaultValue)
        {
            if (index >= 0 && index < _list.Count)
                return (int)_list[index];
            return defaultValue;
        }

        public long OptLong(int index, long defaultValue)
        {
            if (index >= 0 && index < _list.Count)
                return (long)_list[index];
            return defaultValue;
        }

        public bool OptBool(int index, bool defaultValue)
        {
            if (index >= 0 && index < _list.Count)
                return (bool)_list[index];
            return defaultValue;
        }

        public string OptString(int index, string defaultValue)
        {
            if (index >= 0 && index < _list.Count)
                return (string)_list[index];
            return defaultValue;
        }

        public float OptFloat(int index, float defaultValue)
        {
            if (index >= 0 && index < _list.Count)
                return (float)_list[index];
            return defaultValue;
        }

        public double OptDouble(int index, double defaultValue)
        {
            if (index >= 0 && index < _list.Count)
                return (double)_list[index];
            return defaultValue;
        }

        public JsonObject OptJsonObject(int index)
        {
            if (index >= 0 && index < _list.Count)
                return (JsonObject)_list[index];
            return null;
        }

        public JsonArray OptJsonArray(int index)
        {
            if (index >= 0 && index < _list.Count)
                return (JsonArray)_list[index];
            return null;
        }

        public int OptInt(int index)
        {
            if (index >= 0 && index < _list.Count)
                return (int)_list[index];
            return 0;
        }

        public long OptLong(int index)
        {
            object result;
            if (index >= 0 && index < _list.Count)
                return (long)_list[index];
            return 0;
        }

        public bool OptBool(int index)
        {
            if (index >= 0 && index < _list.Count)
                return (bool)_list[index];
            return false;
        }

        public string OptString(int index)
        {
            if (index >= 0 && index < _list.Count)
                return (string)_list[index];
            return string.Empty;
        }

        public float OptFloat(int index)
        {
            if (index >= 0 && index < _list.Count)
                return (float)_list[index];
            return float.NaN;
        }

        public double OptDouble(int index)
        {
            if (index >= 0 && index < _list.Count)
                return (double)_list[index];
            return double.NaN;
        }

        public T Deserialize<T>(int index) where T : IJsonDeserializable
        {
            return Json.Deserialize<T>((JsonObject) _list[index]);
        }

        private List<T> ToListGeneric<T>()
        {
            int count = _list.Count;
            List<T> result = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add((T)_list[i]);
            }
            return result;
        }

        public List<object> ToList()
        {
            return new List<object>(_list);
        }

        public List<int> ToListInt()
        {
            return ToListGeneric<int>();
        }

        public List<long> ToListLong()
        {
            return ToListGeneric<long>();
        }

        public List<string> ToListString()
        {
            return ToListGeneric<string>();
        }

        public List<bool> ToListBool()
        {
            return ToListGeneric<bool>();
        }

        public List<float> ToListFloat()
        {
            return ToListGeneric<float>();
        }

        public List<double> ToListDouble()
        {
            return ToListGeneric<double>();
        }

        public List<T> DeserializeToList<T>() where T : IJsonDeserializable
        {
            int count = _list.Count;
            List<T> result = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(Json.Deserialize<T>((JsonObject)_list[i]));
            }
            return result;
        }

        private T[] ToArrayGeneric<T>()
        {
            int count = _list.Count;
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = (T) _list[i];
            }
            return result;
        }

        public object[] ToArray()
        {
            return _list.ToArray();
        }

        public int[] ToArrayInt()
        {
            return ToArrayGeneric<int>();
        }

        public long[] ToArrayLong()
        {
            return ToArrayGeneric<long>();
        }

        public string[] ToArrayString()
        {
            return ToArrayGeneric<string>();
        }

        public bool[] ToArrayBool()
        {
            return ToArrayGeneric<bool>();
        }

        public float[] ToArrayFloat()
        {
            return ToArrayGeneric<float>();
        }

        public double[] ToArrayDouble()
        {
            return ToArrayGeneric<double>();
        }

        public T[] DeserializeToArray<T>() where T : IJsonDeserializable
        {
            int count = _list.Count;
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = Json.Deserialize<T>((JsonObject)_list[i]);
            }
            return result; 
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}

