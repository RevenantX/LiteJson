using System;

namespace LiteJSON
{
    class MainClass
    {
        class Test
        {
            [JsonTypeInfoAttribute("float")]
            public float a;
            public int b;
            public Test c;
            public string d { get; set; }
        }

        public static void Main(string[] args)
        {
            string str = LiteJSON.Json.Serialize(new Test() {d="shit", c = new Test() {d="ass"} });
            Console.WriteLine(str);
            //JsonObject res = LiteJSON.Json.Deserialize(str);
            //Console.WriteLine(res.Get<int>("ass"));
        }
    }
}
