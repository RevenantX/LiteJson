using System.Reflection;
using System;

namespace LiteJSON
{
    public static class Json
    {
        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An JsonObject</returns>
        public static JsonObject Deserialize(string json)
        {
            // save the string for debug information
            if (json == null)
            {
                return null;
            }

            return Parser.Parse(json);
        }

        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="json">A JsonObject</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(JsonObject obj)
        {
            return Serializer.Serialize(obj);
        }

        public static string Serialize<T>(T value)
        {
            return Serializer.Serialize(value);
        }
    }
}

