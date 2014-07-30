using System;

namespace LiteJSON
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class JsonTypeInfoAttribute : Attribute
    {
        private string _typeInfo;
        public JsonTypeInfoAttribute(string typeInfo)
        {
            _typeInfo = typeInfo;
        }

        public string GetTypeInfo()
        {
            return _typeInfo;
        }
    }
}

