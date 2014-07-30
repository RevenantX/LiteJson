namespace LiteJSON
{
    public class JsonValue
    {
        public string Info;
        public object Value;

        public JsonValue(object value)
        {
            Value = value;
        }

        public JsonValue(object value, string info)
        {
            Value = value;
            Info = info;
        }
    }
}
