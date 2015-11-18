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

        public bool GetBool(int index)
        {
            return (bool)_list[index];
        }

        public string GetString(int index)
        {
            return (string)_list[index];
        }

        public int GetInt(int index)
        {
            object obj = _list[index];
            if (obj is double)
                return (int)(double)obj;
            return (int)(long)obj;
        }

        public long GetLong(int index)
        {
            object obj = _list[index];
            if (obj is double)
                return (long)(double)obj;
            return (long)obj;
        }

        public float GetFloat(int index)
        {
            object obj = _list[index];
            if (obj is long)
                return (long)obj;
            return (float)(double)obj;
        }

        public double GetDouble(int index)
        {
            object obj = _list[index];
            if (obj is long)
                return (long)obj;
            return (double)obj;
        }

        public object Opt(int index)
        {
            if (index >= 0 && index < _list.Count)
                return _list[index];
            return null;
        }

        public int OptInt(int index, int defaultValue = 0)
        {
            if (index >= 0 && index < _list.Count)
                return (int)(long)_list[index];
            return defaultValue;
        }

        public long OptLong(int index, long defaultValue = 0)
        {
            if (index >= 0 && index < _list.Count)
                return (long)_list[index];
            return defaultValue;
        }

        public bool OptBool(int index, bool defaultValue = false)
        {
            if (index >= 0 && index < _list.Count)
                return (bool)_list[index];
            return defaultValue;
        }

        public string OptString(int index, string defaultValue = "")
        {
            if (index >= 0 && index < _list.Count)
                return (string)_list[index];
            return defaultValue;
        }

        public float OptFloat(int index, float defaultValue = 0f)
        {
            if (index >= 0 && index < _list.Count)
            {
                object value = _list[index];
                if (value is long)
                    return (long)value;
                
                return (float)(double)value;
            }
            return defaultValue;
        }

        public double OptDouble(int index, double defaultValue = 0.0)
        {
            if (index >= 0 && index < _list.Count)
            {
                object value = _list[index];
                if (value is long)
                    return (long)value;

                return (double)value;
            }
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

        public T Deserialize<T>(int index) where T : IJsonDeserializable
        {
            return Json.Deserialize<T>((JsonObject) _list[index]);
        }

        public List<object> ToList()
        {
            return new List<object>(_list);
        }

        public List<int> ToListInt(bool castFloats = false)
        {
            int count = _list.Count;
            List<int> result = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                object current = _list[i];
                int value;

                if (current is double)
                {
                    if (castFloats)
                        value = (int)(double)current;
                    else
                        throw new InvalidCastException();
                }
                else
                    value = (int)(long)current;

                result.Add(value);
            }
            return result;
        }

        public List<long> ToListLong(bool castFloats = false)
        {
            int count = _list.Count;
            List<long> result = new List<long>(count);
            for (int i = 0; i < count; i++)
            {
                object current = _list[i];
                long value;

                if (current is double)
                {
                    if (castFloats)
                        value = (long)(double)current;
                    else
                        throw new InvalidCastException();
                }
                else
                    value = (long)current;
                
                result.Add(value);
            }
            return result;
        }

        public List<string> ToListString()
        {
            int count = _list.Count;
            List<string> result = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add((string)_list[i]);
            }
            return result;
        }

        public List<bool> ToListBool()
        {
            int count = _list.Count;
            List<bool> result = new List<bool>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add((bool)_list[i]);
            }
            return result;
        }

        public List<float> ToListFloat()
        {
            int count = _list.Count;
            List<float> result = new List<float>(count);
            for (int i = 0; i < count; i++)
            {
                object current = _list[i];
                float value;

                if (current is long)
                    value = (long)current;
                else
                    value = (float)(double)current;

                result.Add(value);
            }
            return result;
        }

        public List<double> ToListDouble()
        {
            int count = _list.Count;
            List<double> result = new List<double>(count);
            for (int i = 0; i < count; i++)
            {
                object current = _list[i];
                double value;

                if (current is long)
                    value = (long)current;
                else
                    value = (double) current;

                result.Add(value);
            }
            return result;
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

        public object[] ToArray()
        {
            return _list.ToArray();
        }

        public int[] ToArrayInt(bool castFloats = false)
        {
            int count = _list.Count;
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
            {
                if (_list[i] is double)
                {
                    if(castFloats)
                        result[i] = (int) (double) _list[i];
                    else
                        throw new InvalidCastException();
                }
                else
                    result[i] = (int) (long) _list[i];
            }
            return result;
        }

        public long[] ToArrayLong(bool castFloats = false)
        {
            int count = _list.Count;
            long[] result = new long[count];
            for (int i = 0; i < count; i++)
            {
                if (_list[i] is double)
                {
                    if (castFloats)
                        result[i] = (long)(double)_list[i];
                    else
                        throw new InvalidCastException();
                }
                else
                    result[i] = (long)_list[i];
            }
            return result;
        }

        public string[] ToArrayString()
        {
            int count = _list.Count;
            string[] result = new string[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = (string)_list[i];
            }
            return result;
        }

        public bool[] ToArrayBool()
        {
            int count = _list.Count;
            bool[] result = new bool[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = (bool)_list[i];
            }
            return result;
        }

        public float[] ToArrayFloat()
        {
            int count = _list.Count;
            float[] result = new float[count];
            for (int i = 0; i < count; i++)
            {
                if (_list[i] is long)
                    result[i] = (long) _list[i];
                else
                    result[i] = (float)(double) _list[i];
            }
            return result;
        }

        public double[] ToArrayDouble()
        {
            int count = _list.Count;
            double[] result = new double[count];
            for (int i = 0; i < count; i++)
            {
                if (_list[i] is long)
                    result[i] = (long) _list[i];
                else
                    result[i] = (double) _list[i];
            }
            return result;
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

