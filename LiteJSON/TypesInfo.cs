using System;
using System.Collections.Generic;

namespace LiteJSON
{
    public class TypesInfo
    {
        private Dictionary<string, Type> _types = new Dictionary<string, Type>();
        public void RegisterType<T>(string name) where T : IJsonDeserializable
        {
            _types.Add(name, typeof(T));
        }
        public Dictionary<string, Type> RegisteredTypes
        {
            get { return _types; }
        }
    }
}

