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
        public void RegisterType<T>() where T : IJsonDeserializable
        {
            Type t = typeof(T);
            _types.Add(t.Name, t);
        }
        public void RegisterType<T>(bool fullTypeName) where T : IJsonDeserializable
        {
            Type t = typeof(T);
            if(fullTypeName)
                _types.Add(t.FullName, t);
            else
                _types.Add(t.Name, t);
        }
        public Dictionary<string, Type> RegisteredTypes
        {
            get { return _types; }
        }
    }
}

