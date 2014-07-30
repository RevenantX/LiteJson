using System;
using System.Text;

namespace LiteJSON
{
    sealed class Serializer
    {
        StringBuilder _builder;

        private Serializer()
        {
            _builder = new StringBuilder();
        }

        public static string Serialize(JsonObject obj)
        {
            var instance = new Serializer();
            instance.SerializeObject(obj);
            return instance._builder.ToString();
        }

        private void SerializeValue(object value)
        {
            JsonArray asList;
            JsonObject asDict;
            string asStr;

            if (value == null)
            {
                _builder.Append("null");
            }
            else if ((asStr = value as string) != null)
            {
                SerializeString(asStr);
            }
            else if (value is bool)
            {
                _builder.Append((bool)value ? "true" : "false");
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
                _builder.Append(((float)value).ToString("R"));
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
                _builder.Append(value);
            }
            else if (value is double
                || value is decimal)
            {
                _builder.Append(Convert.ToDouble(value).ToString("R"));
            }
            else
            {
                SerializeString(value.ToString());
            }
        }

        private void SerializeObject(JsonObject obj)
        {
            bool first = true;

            _builder.Append('{');

            foreach (string e in obj.Keys)
            {
                if (!first)
                {
                    _builder.Append(',');
                }

                SerializeString(e);
                _builder.Append(':');

                SerializeValue(obj.Get(e));

                first = false;
            }

            _builder.Append('}');
        }

        private void SerializeArray(JsonArray anArray)
        {
            _builder.Append('[');

            bool first = true;

            foreach (object obj in anArray)
            {
                if (!first)
                {
                    _builder.Append(',');
                }

                SerializeValue(obj);

                first = false;
            }

            _builder.Append(']');
        }

        private void SerializeString(string str)
        {
            _builder.Append('\"');

            char[] charArray = str.ToCharArray();
            foreach (var c in charArray)
            {
                switch (c)
                {
                    case '"':
                        _builder.Append("\\\"");
                        break;
                    case '\\':
                        _builder.Append("\\\\");
                        break;
                    case '\b':
                        _builder.Append("\\b");
                        break;
                    case '\f':
                        _builder.Append("\\f");
                        break;
                    case '\n':
                        _builder.Append("\\n");
                        break;
                    case '\r':
                        _builder.Append("\\r");
                        break;
                    case '\t':
                        _builder.Append("\\t");
                        break;
                    default:
                        int codepoint = Convert.ToInt32(c);
                        if ((codepoint >= 32) && (codepoint <= 126))
                        {
                            _builder.Append(c);
                        }
                        else
                        {
                            _builder.Append("\\u");
                            _builder.Append(codepoint.ToString("x4"));
                        }
                        break;
                }
            }

            _builder.Append('\"');
        }
    }
}

