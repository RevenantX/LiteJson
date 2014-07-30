using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace LiteJSON
{
    sealed class Parser : IDisposable
    {
        const string WORD_BREAK = "{}[],:\"";

        public static bool IsWordBreak(char c)
        {
            return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
        }

        enum TOKEN
        {
            NONE,
            OPEN,
            CLOSE,
            CURLY_OPEN,
            CURLY_CLOSE,
            SQUARED_OPEN,
            SQUARED_CLOSE,
            COLON,
            COMMA,
            STRING,
            NUMBER,
            TRUE,
            FALSE,
            NULL
        };

        private StringReader _json;

        private Parser(string jsonString)
        {
            _json = new StringReader(jsonString);
        }

        public static JsonObject Parse(string jsonString)
        {
            JsonObject jsonObject = null;
            using (var instance = new Parser(jsonString))
            {
                TOKEN nextToken = instance.NextToken;
                if (nextToken != TOKEN.CURLY_OPEN)
                    throw new Exception("Bad json");

                jsonObject = instance.ParseObject();
            }
            return jsonObject;
        }

        public static T Parse<T>(string jsonString)
        {
            T result = default(T);

            using (var instance = new Parser(jsonString))
            {
                TOKEN nextToken = instance.NextToken;
                if (nextToken != TOKEN.CURLY_OPEN)
                    throw new Exception("Bad json");

                result = instance.ParseClass<T>();
            }

            return result;
        }

        public void Dispose()
        {
            _json.Dispose();
            _json = null;
        }

        private object ParseValue()
        {
            TOKEN nextToken = NextToken;
            return ParseByToken(nextToken);
        }

        private object ParseByToken(TOKEN token)
        {
            switch (token)
            {
                case TOKEN.STRING:
                    return ParseString();
                case TOKEN.NUMBER:
                    return ParseNumber();
                case TOKEN.CURLY_OPEN:
                    return ParseObject();
                case TOKEN.SQUARED_OPEN:
                    return ParseArray();
                case TOKEN.TRUE:
                    return true;
                case TOKEN.FALSE:
                    return false;
                case TOKEN.NULL:
                    return null;
                default:
                    return null;
            }
        }

        private T ParseClass<T>()
        {
            T result = Activator.CreateInstance<T>();
            Type t = typeof(T);

            _json.Read();
            while (true)
            {
                switch (NextToken)
                {
                    case TOKEN.NONE:
                        return default(T);
                    case TOKEN.COMMA:
                        continue;
                    case TOKEN.CURLY_CLOSE:
                        return result;
                    default:
                        string typeName = null;
                        if (NextToken == TOKEN.OPEN)
                        {
                            typeName = ParseTypeName();
                        }
                        // name
                        string name = ParseString();
                        if (name == null)
                        {
                            return default(T);
                        }

                        // :
                        if (NextToken != TOKEN.COLON)
                        {
                            return default(T);
                        }
                        // ditch the colon
                        _json.Read();

                        if (typeName != null)
                        {
                            object obj = Activator.CreateInstance(;
                            t.GetField(name).SetValue(result, obj);
                        }
                        else
                        {
                            t.GetField(name).SetValue(result, obj);
                        }
                        // value
                        //table.Put(name, ParseValue());
                        break;
                }
            }
        }

        private JsonObject ParseObject()
        {
            JsonObject table = new JsonObject();

            // ditch opening brace
            _json.Read();

            // {
            while (true)
            {
                switch (NextToken)
                {
                    case TOKEN.NONE:
                        return null;
                    case TOKEN.COMMA:
                        continue;
                    case TOKEN.CURLY_CLOSE:
                        return table;
                    default:
                        // name
                        string name = ParseString();
                        if (name == null)
                        {
                            return null;
                        }

                        // :
                        if (NextToken != TOKEN.COLON)
                        {
                            return null;
                        }
                        // ditch the colon
                        _json.Read();

                        // value
                        table.Put(name, ParseValue());
                        break;
                }
            }
        }

        private JsonArray ParseArray()
        {
            JsonArray array = new JsonArray();

            // ditch opening bracket
            _json.Read();

            // [
            var parsing = true;
            while (parsing)
            {
                TOKEN nextToken = NextToken;

                switch (nextToken)
                {
                    case TOKEN.NONE:
                        return null;
                    case TOKEN.COMMA:
                        continue;
                    case TOKEN.SQUARED_CLOSE:
                        parsing = false;
                        break;
                    default:
                        object value = ParseByToken(nextToken);

                        array.Add(value);
                        break;
                }
            }

            return array;
        }

        private string ParseTypeName()
        {
            StringBuilder s = new StringBuilder();
            char c;

            // ditch opening '('
            _json.Read();

            bool parsing = true;
            while (parsing)
            {

                if (_json.Peek() == -1)
                {
                    parsing = false;
                    break;
                }

                c = NextChar;
                switch (c)
                {
                    case ')':
                        parsing = false;
                        break;
                    case '\\':
                        if (_json.Peek() == -1)
                        {
                            parsing = false;
                            break;
                        }
                        c = NextChar;
                        break;
                    default:
                        s.Append(c);
                        break;
                }
            }

            return s.ToString();
        }

        private string ParseString()
        {
            StringBuilder s = new StringBuilder();
            char c;

            // ditch opening quote
            _json.Read();

            bool parsing = true;
            while (parsing)
            {

                if (_json.Peek() == -1)
                {
                    parsing = false;
                    break;
                }

                c = NextChar;
                switch (c)
                {
                    case '"':
                        parsing = false;
                        break;
                    case '\\':
                        if (_json.Peek() == -1)
                        {
                            parsing = false;
                            break;
                        }

                        c = NextChar;
                        switch (c)
                        {
                            case '"':
                            case '\\':
                            case '/':
                                s.Append(c);
                                break;
                            case 'b':
                                s.Append('\b');
                                break;
                            case 'f':
                                s.Append('\f');
                                break;
                            case 'n':
                                s.Append('\n');
                                break;
                            case 'r':
                                s.Append('\r');
                                break;
                            case 't':
                                s.Append('\t');
                                break;
                            case 'u':
                                var hex = new char[4];

                                for (int i = 0; i < 4; i++)
                                {
                                    hex[i] = NextChar;
                                }

                                s.Append((char)Convert.ToInt32(new string(hex), 16));
                                break;
                        }
                        break;
                    default:
                        s.Append(c);
                        break;
                }
            }

            return s.ToString();
        }

        private object ParseNumber()
        {
            string number = NextWord;

            if (number.IndexOf('.') == -1)
            {
                long parsedInt;
                Int64.TryParse(number, out parsedInt);
                if (parsedInt <= int.MaxValue || parsedInt >= int.MinValue)
                    return (int)parsedInt;
                else
                    return parsedInt;
            }

            double parsedDouble;
            Double.TryParse(number, out parsedDouble);
            if (parsedDouble <= float.MaxValue || parsedDouble >= float.MinValue)
                return (float)parsedDouble;
            else
                return parsedDouble;
        }

        private void EatWhitespace()
        {
            while (Char.IsWhiteSpace(PeekChar))
            {
                _json.Read();

                if (_json.Peek() == -1)
                {
                    break;
                }
            }
        }

        private char PeekChar
        {
            get
            {
                return Convert.ToChar(_json.Peek());
            }
        }

        private char NextChar
        {
            get
            {
                return Convert.ToChar(_json.Read());
            }
        }

        private string NextWord
        {
            get
            {
                StringBuilder word = new StringBuilder();

                while (!IsWordBreak(PeekChar))
                {
                    word.Append(NextChar);

                    if (_json.Peek() == -1)
                    {
                        break;
                    }
                }

                return word.ToString();
            }
        }

        private TOKEN NextToken
        {
            get
            {
                EatWhitespace();

                if (_json.Peek() == -1)
                {
                    return TOKEN.NONE;
                }

                switch (PeekChar)
                {
                    case '(':
                        return TOKEN.OPEN;
                    case ')':
                        return TOKEN.CLOSE;
                    case '{':
                        return TOKEN.CURLY_OPEN;
                    case '}':
                        _json.Read();
                        return TOKEN.CURLY_CLOSE;
                    case '[':
                        return TOKEN.SQUARED_OPEN;
                    case ']':
                        _json.Read();
                        return TOKEN.SQUARED_CLOSE;
                    case ',':
                        _json.Read();
                        return TOKEN.COMMA;
                    case '"':
                        return TOKEN.STRING;
                    case ':':
                        return TOKEN.COLON;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                        return TOKEN.NUMBER;
                }

                switch (NextWord)
                {
                    case "false":
                        return TOKEN.FALSE;
                    case "true":
                        return TOKEN.TRUE;
                    case "null":
                        return TOKEN.NULL;
                }

                return TOKEN.NONE;
            }
        }
    }
}

