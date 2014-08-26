using System;
using System.Collections.Generic;

namespace LiteJSON
{
    class MainClass
    {
        interface ISomeWhat : IJsonSerializable, IJsonDeserializable
        {
            void Write();
        }

        class A : ISomeWhat
        {
            public float s;

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
                someShto = jsonObject.GetJsonObject("someShto").Deserialize<A>();
            }

            public void Write()
            {
                Console.WriteLine("I am B: " + z);
            }
        }

        class Test : IJsonSerializable, IJsonDeserializable
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
            //TEST1
            Test t1 = new Test();
            t1.b = new[] {"тест", "test2"};
            t1.c = new List<ISomeWhat>();
            t1.c.Add(new A() {s = 0.333f});
            t1.c.Add(new B());
            Console.WriteLine("Converting to json...");

            SerializerConfig config = new SerializerConfig();
            config.Indent = true;

            string text = Json.Serialize(t1, config);
            Console.WriteLine(text);

            //TEST2
            TypesInfo ti = new TypesInfo();
            ti.RegisterType<A>("A");
            ti.RegisterType<B>("B");

            Console.WriteLine("Deserializing");
            Test t2 = Json.Deserialize<Test>(text, ti);
            t2.c[0].Write();
            t2.c[1].Write();

            //TEST3
            string jtext = "{ kalabanga: 5, b  : \"Тестовая Строка\", c: \"TestString\" }";
            JsonObject jo = Json.Deserialize(jtext);
            Console.WriteLine(jo.GetString("b"));
            Console.WriteLine(jo.GetString("c"));

            Console.WriteLine("Must be: Тестовая строка");

            Console.ReadKey();
        }
    }
}
