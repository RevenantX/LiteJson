using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace LiteJSON
{
    sealed class Parser : IDisposable
    {
        const string WordBreak = "{}[],:\"";

        public static bool IsWordBreak(char c)
        {
            return Char.IsWhiteSpace(c) || WordBreak.IndexOf(c) != -1;
        }

        enum Token
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
            JsonObject jsonObject;
            using (var instance = new Parser(jsonString))
            {
                Token nextToken = instance.NextToken;
                if (nextToken != Token.CURLY_OPEN)
                    throw new Exception("Bad json");

                jsonObject = instance.ParseObject();
            }
            return jsonObject;
        }

        public void Dispose()
        {
            _json.Dispose();
            _json = null;
        }

        private object ParseValue()
        {
            Token nextToken = NextToken;
            return ParseByToken(nextToken);
        }

        private object ParseByToken(Token token)
        {
            switch (token)
            {
                case Token.STRING:
                    return ParseString();
                case Token.NUMBER:
                    return ParseNumber();
                case Token.CURLY_OPEN:
                    return ParseObject();
                case Token.SQUARED_OPEN:
                    return ParseArray();
                case Token.TRUE:
                    return true;
                case Token.FALSE:
                    return false;
                case Token.NULL:
                    return null;
                default:
                    return null;
            }
        }

        private JsonObject ParseObject()
        {
            JsonObject jsonObject = new JsonObject();

            // ditch opening brace
            _json.Read();

            // {
            while (true)
            {
                switch (NextToken)
                {
                    case Token.NONE:
                        return null;
                    case Token.COMMA:
                        continue;
                    case Token.CURLY_CLOSE:
                        return jsonObject;
                    default:
                        string typeName = null;
                        if (NextToken == Token.OPEN)
                        {
                            typeName = ParseTypeName();
                        }
                        // name
                        string name = ParseString();
                        if (name == null)
                        {
                            return null;
                        }

                        // :
                        if (NextToken != Token.COLON)
                        {
                            return null;
                        }
                        // ditch the colon
                        _json.Read();

                        // value
                        jsonObject.Put(name, ParseValue());
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
                Token nextToken = NextToken;

                switch (nextToken)
                {
                    case Token.NONE:
                        return null;
                    case Token.COMMA:
                        continue;
                    case Token.SQUARED_CLOSE:
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
                        }
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

        private Token NextToken
        {
            get
            {
                EatWhitespace();

                if (_json.Peek() == -1)
                {
                    return Token.NONE;
                }

                switch (PeekChar)
                {
                    case '(':
                        return Token.OPEN;
                    case ')':
                        return Token.CLOSE;
                    case '{':
                        return Token.CURLY_OPEN;
                    case '}':
                        _json.Read();
                        return Token.CURLY_CLOSE;
                    case '[':
                        return Token.SQUARED_OPEN;
                    case ']':
                        _json.Read();
                        return Token.SQUARED_CLOSE;
                    case ',':
                        _json.Read();
                        return Token.COMMA;
                    case '"':
                        return Token.STRING;
                    case ':':
                        return Token.COLON;
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
                        return Token.NUMBER;
                }

                switch (NextWord)
                {
                    case "false":
                        return Token.FALSE;
                    case "true":
                        return Token.TRUE;
                    case "null":
                        return Token.NULL;
                }

                return Token.NONE;
            }
        }
    }
}

