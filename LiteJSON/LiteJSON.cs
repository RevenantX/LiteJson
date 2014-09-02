using System.Reflection;
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

        public static string Serialize(JsonObject obj)
        {
            return JsonSerializer.Serialize(obj, new SerializerConfig());
        }

        public static string Serialize(JsonObject obj, SerializerConfig config)
        {
            return JsonSerializer.Serialize(obj, config);
        }

        public static T Deserialize<T>(string jsonString, TypesInfo typesInfo) where T : IJsonDeserializable
        {
            JsonDeserializer parser = new JsonDeserializer(typesInfo);
            T result = Activator.CreateInstance<T>();
            result.FromJson(parser.Parse(jsonString));
            return result;
        }

        public static T Deserialize<T>(string jsonString) where T : IJsonDeserializable
        {
            JsonDeserializer parser = new JsonDeserializer(new TypesInfo());
            T result = Activator.CreateInstance<T>();
            result.FromJson(parser.Parse(jsonString));
            return result;
        }

        public static T Deserialize<T>(JsonObject jsonObject) where T : IJsonDeserializable
        {
            T jsonSerializable = Activator.CreateInstance<T>();
            jsonSerializable.FromJson(jsonObject);
            return jsonSerializable;
        }

        public static JsonObject Deserialize(string jsonString, TypesInfo typesInfo)
        {
            JsonDeserializer parser = new JsonDeserializer(typesInfo);
            return parser.Parse(jsonString);
        }

        public static JsonObject Deserialize(string jsonString)
        {
            JsonDeserializer parser = new JsonDeserializer(new TypesInfo());
            return parser.Parse(jsonString);
        }
    }
}

