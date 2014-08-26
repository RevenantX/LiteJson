using System;
using System.Globalization;
using System.Text;

namespace LiteJSON
{
    sealed class JsonSerializer
    {
        StringBuilder _builder;
        private SerializerConfig _config;
        private int _depth;
        private const int TabSize = 2;

        private JsonSerializer(SerializerConfig config)
        {
            _builder = new StringBuilder();
            _config = config;
        }

        public static string Serialize(JsonObject obj, SerializerConfig config)
        {
            var instance = new JsonSerializer(config);
            instance.SerializeObject(obj);
            return instance._builder.ToString();
        }

        private void SerializeValue(object value)
        {
            JsonArray jsonArray;
            JsonObject jsonObject;
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
            else if ((jsonArray = value as JsonArray) != null)
            {
                SerializeArray(jsonArray);
            }
            else if ((jsonObject = value as JsonObject) != null)
            {
                SerializeObject(jsonObject);
            }
            else if (value is char)
            {
                SerializeString(new string((char)value, 1));
            }
            else if (value is float)
            {
                _builder.Append(((float)value).ToString("R", CultureInfo.CreateSpecificCulture("en-US").NumberFormat));
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
                _builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.CreateSpecificCulture("en-US").NumberFormat));
            }
            else
            {
                SerializeString(value.ToString());
            }
        }

        private void SerializeObject(JsonObject obj)
        {
            bool first = true;

            if (!string.IsNullOrEmpty(obj.TypeName))
            {
                _builder.Append('(');
                _builder.Append(obj.TypeName);
                _builder.Append(')');
            }

            _builder.Append('{');
            _depth += 1;
            if (_config.Indent) _builder.Append('\n');
            foreach (string e in obj.Keys)
            {
                if (!first)
                {
                    _builder.Append(',');
                    if (_config.Indent) _builder.Append('\n');
                }
                if (_config.Indent) _builder.Append(' ', _depth * TabSize);

                SerializeString(e);
                _builder.Append(':');

                SerializeValue(obj.Get(e));


                first = false;
            }
            _depth -= 1;
            if (_config.Indent)
            {
                _builder.Append('\n');
                _builder.Append(' ', _depth * TabSize);
            }
            _builder.Append('}');
        }

        private void SerializeArray(JsonArray anArray)
        {
            _depth += 1;
            _builder.Append('[');
            bool first = true;

            foreach (object obj in anArray)
            {
                if (!first)
                {
                    _builder.Append(',');
                }
                if (_config.Indent)
                {
                    _builder.Append('\n');
                    _builder.Append(' ', _depth * TabSize);
                }

                SerializeValue(obj);

                first = false;
            }
            _depth -= 1;
            if (_config.Indent)
            {
                _builder.Append('\n');
                _builder.Append(' ', _depth * TabSize);
            }
            _builder.Append(']');
        }

        private void SerializeString(string str)
        {
            _builder.Append('\"');

            foreach (var c in str)
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
                        _builder.Append(c);
                        break;
                }
            }

            _builder.Append('\"');
        }
    }
}

