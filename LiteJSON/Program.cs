using System;
using System.Collections.Generic;

namespace LiteJSON
{
    class MainClass
    {
        interface ISomeWhat : IJsonSerializable
        {
            void Write();
        }

        class A : ISomeWhat
        {
            public float s = 1.3f;
            public JsonObject ToJson()
            {
                JsonObject obj = new JsonObject("A");
                obj.Put("s", s);
                return obj;
            }

            public void FromJson(JsonObject jsonObject)
            {
                s = jsonObject.GetFloat("s");
            }

            public void Write()
            {
                Console.WriteLine("I am A: " + s);
            }
        }

        class B : ISomeWhat
        {
            public string z = "sobachka";
            public A someShto = new A {s = 0.79f};
            public JsonObject ToJson()
            {
                JsonObject obj = new JsonObject("B");
                obj.Put("z", z);
                obj.Put("someShto", someShto);
                return obj;
            }

            public void FromJson(JsonObject jsonObject)
            {
                z = jsonObject.GetString("z");
                someShto = jsonObject.Deserialize<A>("someShto");
            }

            public void Write()
            {
                Console.WriteLine("I am B: " + z);
            }
        }

        class Test : IJsonSerializable
        {
            public float a;
            public string[] b;
            public List<ISomeWhat> c;

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
                b = jsonObject.GetJsonArray("b").ToArray<string>();
                c = jsonObject.GetJsonArray("c").ToList<ISomeWhat>();
            }
        }

        public static void Main(string[] args)
        {
            Test t1 = new Test();
            t1.b = new[] {"avtobus", "pipiska"};
            t1.c = new List<ISomeWhat>();
            t1.c.Add(new A());
            t1.c.Add(new B());
            Console.WriteLine("Converting to json...");
            string text = Json.Serialize(t1);
            Console.WriteLine(text);

            JsonParser p = new JsonParser();
            p.RegisterType<A>("A");
            p.RegisterType<B>("B");

            Console.WriteLine("Deserializing");
            Test t2 = p.Deserialize<Test>(text);
            t2.c[0].Write();
            t2.c[1].Write();
            Console.ReadKey();
        }
    }
}
