using System;
using System.Globalization;
using System.Text;

namespace LiteJSON
{
    sealed class JsonParser
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
            NULL,
            WORD
        };
            
        private string _json;
        private int _position;
        private TypesInfo _typesInfo;

        public JsonParser(TypesInfo typesInfo)
        {
            _typesInfo = typesInfo;
        }

        public JsonObject Parse(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }
            _json = jsonString;
            _position = 0;
            Token nextToken = NextToken();
            if (nextToken != Token.CURLY_OPEN)
                throw new Exception("Bad json");
            return ParseObject(false);
        }

        private object ParseValue()
        {
            return ParseByToken(NextToken());
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
                    return ParseObject(false);
                case Token.OPEN:
                    return ParseObject(true);
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

        private JsonObject ParseObject(bool withType)
        {
            JsonObject jsonObject;
            if (withType)
            {
                string typeName = ParseTypeName();
                Type t;
                if (!string.IsNullOrEmpty(typeName) && _typesInfo.RegisteredTypes.TryGetValue(typeName, out t))
                {
                    jsonObject = new JsonObject(t);
                }
                else
                {
                    throw new Exception("Unregistered type: " + typeName);
                }

                EatWhitespace();
            }
            else
            {
                jsonObject = new JsonObject();
            }
                
            // ditch opening brace
            SkipChar();


            // {
            while (true)
            {
                switch (NextToken())
                {
                    case Token.NONE:
                        throw new Exception("None token");
                    case Token.COMMA:
                        continue;
                    case Token.CURLY_CLOSE:
                        return jsonObject;
                    default:
                        // name
                        string name = ParseVariableName();
                        if (name == null)
                        {
                            throw new Exception("Empty variable name");
                        }

                        // :
                        if (NextToken() != Token.COLON)
                        {
                            throw new Exception("Missing colon after variable name");
                        }
                        // ditch the colon
                        SkipChar();

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
            SkipChar();

            // [
            var parsing = true;
            while (parsing)
            {
                Token nextToken = NextToken();

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
            SkipChar();

            bool parsing = true;
            while (parsing)
            {
                if (IsEof())
                {
                    break;
                }

                c = NextChar();
                switch (c)
                {
                    case ')':
                        parsing = false;
                        break;
                    case '\\':
                        if (IsEof())
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

        private string ParseVariableName()
        {
            char c = PeekChar();
            if (c != '"')
            {
                StringBuilder s = new StringBuilder();
                bool parsing = true;
                while (parsing)
                {
                    if (IsEof())
                    {
                        break;
                    }

                    s.Append(NextChar());
                    c = PeekChar();
                    if (c == ':' || c == ' ')
                    {
                        parsing = false;
                    }
                }
                return s.ToString();
            }
            else
            {
                return ParseString();
            }
        }

        private string ParseString()
        {
            StringBuilder s = new StringBuilder();
            char c;

            // ditch opening quote
            SkipChar();

            bool parsing = true;
            while (parsing)
            {
                if (IsEof())
                {
                    break;
                }

                c = NextChar();
                switch (c)
                {
                    case '"':
                        parsing = false;
                        break;
                    case '\\':
                        if (IsEof())
                        {
                            parsing = false;
                            break;
                        }

                        c = NextChar();
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
                                    hex[i] = NextChar();
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
            string number = NextWord();

            if (number.IndexOf('.') == -1)
            {
                long parsedInt;
                if (Int64.TryParse(number, out parsedInt))
                {
                    return parsedInt;
                }
                else
                {
                    throw new Exception("Incorrect number");
                }
            }

            double parsedDouble;
            if (Double.TryParse(number, NumberStyles.AllowDecimalPoint,
                CultureInfo.CreateSpecificCulture("en-US").NumberFormat, out parsedDouble))
            {
                return parsedDouble;
            }
            else
            {
                throw new Exception("Incorrect number");
            }
        }

        private void EatWhitespace()
        {
            while (Char.IsWhiteSpace(PeekChar()))
            {
                SkipChar();

                if (IsEof())
                {
                    break;
                }
            }
        }

        private bool IsEof()
        {
            return _position >= _json.Length;
        }

        private char PeekChar()
        {
            return _json[_position];
        }

        private char NextChar()
        {
            char c = _json[_position];
            _position++;
            return c;
        }

        private void SkipChar()
        {
            _position++;
        }

        private string NextWord()
        {
            StringBuilder word = new StringBuilder();

            while (!IsWordBreak(PeekChar()))
            {
                word.Append(NextChar());

                if (IsEof())
                {
                    break;
                }
            }

            return word.ToString();
        }

        private Token NextToken()
        {
            EatWhitespace();

            if (IsEof())
            {
                return Token.NONE;
            }

            switch (PeekChar())
            {
                case '(':
                    return Token.OPEN;
                case ')':
                    return Token.CLOSE;
                case '{':
                    return Token.CURLY_OPEN;
                case '}':
                    SkipChar();
                    return Token.CURLY_CLOSE;
                case '[':
                    return Token.SQUARED_OPEN;
                case ']':
                    SkipChar();
                    return Token.SQUARED_CLOSE;
                case ',':
                    SkipChar();
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

            string word = NextWord();
            switch (word)
            {
                case "false":
                    return Token.FALSE;
                case "true":
                    return Token.TRUE;
                case "null":
                    return Token.NULL;
                default:
                    _position -= word.Length;
                    return Token.WORD;
            }
        }
    }
}

