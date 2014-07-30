namespace LiteJSON
{
    public interface IJsonSerializable
    {
        JsonObject ToJson();
        void FromJson(JsonObject jsonObject);
    }
}
