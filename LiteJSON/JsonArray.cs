using System;
using System.Collections.Generic;
using System.Collections;

namespace LiteJSON
{
    public class JsonArray : IEnumerable
    {

        private List<object> _list;

        public JsonArray()
        {
            _list = new List<object>();
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

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}

