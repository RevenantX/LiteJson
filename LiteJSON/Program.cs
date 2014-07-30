using System;
using System.Collections.Generic;

namespace LiteJSON
{
    class MainClass
    {
        class Test : IJsonSerializable
        {
            public float a;
            public string[] b;
            public List<string> c;

            public JsonObject ToJson()
            {
                JsonObject obj = new JsonObject();
                obj.Put("a", a);
                obj.Put("b", b);
                obj.Put("c", c);
                return obj;
            }

            public void FromJson(JsonObject jsonObject)
            {
                a = jsonObject.GetFloat("a");
                b = jsonObject.GetArray<string>("b");
                c = jsonObject.GetList<string>("c");
            }
        }

        public static void Main(string[] args)
        {
            Test t1 = new Test();
            t1.b = new[] {"avtobus", "pipiska"};
            string text = Json.Serialize(t1);
            Console.WriteLine(text);
            Test t2 = Json.Deserialize<Test>(text);
            Console.WriteLine(t2.b[0]);
            Console.ReadKey();
        }
    }
}
