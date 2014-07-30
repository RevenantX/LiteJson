using System;
using System.Text;
using System.Collections;
using System.Reflection;

namespace LiteJSON
{
    sealed class Serializer
    {
        StringBuilder builder;

        private Serializer()
        {
            builder = new StringBuilder();
        }

        public static string Serialize(JsonObject obj)
        {
            var instance = new Serializer();
            instance.SerializeObject(obj);
            return instance.builder.ToString();
        }

        public static string Serialize<T>(T obj)
        {
            var instance = new Serializer();
            instance.SerializeClass(obj, typeof(T));
            return instance.builder.ToString();
        }

        private void SerializeValue(object value)
        {
            JsonArray asList;
            JsonObject asDict;
            string asStr;

            if (value == null)
            {
                builder.Append("null");
            }
            else if ((asStr = value as string) != null)
            {
                SerializeString(asStr);
            }
            else if (value is bool)
            {
                builder.Append((bool)value ? "true" : "false");
            }
            else if ((asList = value as JsonArray) != null)
            {
                SerializeArray(asList);
            }
            else if ((asDict = value as JsonObject) != null)
            {
                SerializeObject(asDict);
            }
            else if (value is char)
            {
                SerializeString(new string((char)value, 1));
            }
            else if (value is float)
            {
                builder.Append(((float)value).ToString("R"));
            }
            else if (value is int
                || value is uint
                || value is long
                || value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is ulong)
            {
                builder.Append(value);
            }
            else if (value is double
                || value is decimal)
            {
                builder.Append(Convert.ToDouble(value).ToString("R"));
            }
            else
            {
                SerializeString(value.ToString());
            }
        }

        private void SerializeClass(object obj, Type t)
        {
            bool first = true;
            builder.Append('{');

            foreach (FieldInfo field in t.GetFields())
            {
                if (!first)
                {
                    builder.Append(',');
                }

                foreach (Attribute attr in field.GetCustomAttributes(true))
                {
                    JsonTypeInfoAttribute ti = attr as JsonTypeInfoAttribute;
                    if (ti != null)
                    {
                        builder.Append("(" + ti.GetTypeInfo() + ")");
                    }
                }

                SerializeString(field.Name);
                builder.Append(':');

                object fieldValue = field.GetValue(obj);
                if (fieldValue != null && field.FieldType == t)
                    SerializeClass(fieldValue, t);
                else
                    SerializeValue(fieldValue);
                first = false;
            }

            foreach (PropertyInfo property in t.GetProperties())
            {
                if (!first)
                {
                    builder.Append(',');
                }

                foreach (Attribute attr in property.GetCustomAttributes(true))
                {
                    JsonTypeInfoAttribute ti = attr as JsonTypeInfoAttribute;
                    if (ti != null)
                    {
                        builder.Append("(" + ti.GetTypeInfo() + ")");
                    }
                }

                SerializeString(property.Name);
                builder.Append(':');

                object fieldValue = property.GetValue(obj,null);
                if (fieldValue != null && property.PropertyType == t)
                    SerializeClass(fieldValue, t);
                else
                    SerializeValue(fieldValue);
                first = false;
            }

            builder.Append('}');
        }

        private void SerializeObject(JsonObject obj)
        {
            bool first = true;

            builder.Append('{');

            foreach (string e in obj.Keys)
            {
                if (!first)
                {
                    builder.Append(',');
                }

                SerializeString(e.ToString());
                builder.Append(':');

                SerializeValue(obj.Get(e));

                first = false;
            }

            builder.Append('}');
        }

        private void SerializeArray(JsonArray anArray)
        {
            builder.Append('[');

            bool first = true;

            foreach (object obj in anArray)
            {
                if (!first)
                {
                    builder.Append(',');
                }

                SerializeValue(obj);

                first = false;
            }

            builder.Append(']');
        }

        private void SerializeString(string str)
        {
            builder.Append('\"');

            char[] charArray = str.ToCharArray();
            foreach (var c in charArray)
            {
                switch (c)
                {
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        int codepoint = Convert.ToInt32(c);
                        if ((codepoint >= 32) && (codepoint <= 126))
                        {
                            builder.Append(c);
                        }
                        else
                        {
                            builder.Append("\\u");
                            builder.Append(codepoint.ToString("x4"));
                        }
                        break;
                }
            }

            builder.Append('\"');
        }
    }
}

