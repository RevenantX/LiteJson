using System;

namespace LiteJSON
{
    public static class Json
    {
        public static string Serialize(IJsonSerializable obj)
        {
            return JsonSerializer.Serialize(obj.ToJson(), new SerializerConfig());
        }

        public static string Serialize(IJsonSerializable obj, SerializerConfig config)
        {
            return JsonSerializer.Serialize(obj.ToJson(), config);
        }

        public static T Deserialize<T>(string jsonString, TypesInfo typesInfo) where T : IJsonDeserializable
        {
            JsonParser parser = new JsonParser(typesInfo);
            T result = Activator.CreateInstance<T>();
            result.FromJson(parser.Parse(jsonString));
            return result;
        }

        public static T Deserialize<T>(string jsonString) where T : IJsonDeserializable
        {
            JsonParser parser = new JsonParser(new TypesInfo());
            T result = Activator.CreateInstance<T>();
            result.FromJson(parser.Parse(jsonString));
            return result;
        }

        public static T Deserialize<T>(JsonObject jsonObject) where T : IJsonDeserializable
        {
            if (jsonObject.TypeInfo != null)
            {
                T item = (T)Activator.CreateInstance(jsonObject.TypeInfo);
                item.FromJson(jsonObject);
                return item;
            }
            else
            {
                T item = Activator.CreateInstance<T>();
                item.FromJson(jsonObject);
                return item;
            }
        }
    }
}

