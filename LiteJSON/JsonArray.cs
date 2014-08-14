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

        public int Count
        {
            get { return _list.Count; }
        }

        public void Add<T>(T value)
        {
            _list.Add(value);
        }

        public T Get<T>(int index)
        {
            return (T)_list[index];
        }

        public List<T> ToList<T>()
        {
            bool deserialize = typeof(IJsonDeserializable).IsAssignableFrom(typeof(T));
            int count = _list.Count;
            List<T> result = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                if(deserialize)
                {
                    JsonObject jsonObject = (JsonObject)_list[i];
                    result.Add((T)jsonObject.Deserialize());
                }
                else
                {
                    result.Add((T)_list[i]);
                }
            }
            return result;
        }

        public T[] ToArray<T>()
        {
            bool deserialize = typeof(IJsonDeserializable).IsAssignableFrom(typeof(T));
            int count = _list.Count;
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                if(deserialize)
                {
                    JsonObject jsonObject = (JsonObject)_list[i];
                    result[i] = (T)jsonObject.Deserialize();
                }
                else
                {
                    result[i] = (T)_list[i];
                }
            }
            return result; 
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}

