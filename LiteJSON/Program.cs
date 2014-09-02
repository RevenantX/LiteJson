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
                JsonObject obj = new JsonObject(typeof(A));
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
            public string z;
            public ISomeWhat someShto = new A { s = 0.79f };
            public JsonObject ToJson()
            {
                JsonObject obj = new JsonObject(typeof(B), true);
                obj.Put("z", z);
                obj.Put("someShto", someShto);
                return obj;
            }

            public void FromJson(JsonObject jsonObject)
            {
                z = jsonObject.GetString("z");
                someShto = jsonObject.Deserialize<ISomeWhat>("someShto");
            }

            public void Write()
            {
                Console.WriteLine("I am B: " + z);
                someShto.Write();
            }
        }

        class Test : IJsonSerializable, IJsonDeserializable
        {
            public double a;
            public string[] b;
            public List<ISomeWhat> c;
            public long d;

            public JsonObject ToJson()
            {
                JsonObject obj = new JsonObject();
                obj.Put("a", a);
                obj.Put("b", b);
                obj.Put("c", c);
                obj.Put("d", d);
                return obj;
            }

            public void FromJson(JsonObject jsonObject)
            {
                a = jsonObject.GetDouble("a");
                b = jsonObject.GetJsonArray("b").ToArrayString();
                c = jsonObject.GetJsonArray("c").DeserializeToList<ISomeWhat>();
                d = jsonObject.GetLong("d");
            }
        }

        public static void Main(string[] args)
        {
            //TEST1
            Test t1 = new Test();
            t1.a = 0.7f;
            t1.b = new[] {"тест", "test2"};
            t1.c = new List<ISomeWhat>();
            t1.c.Add(new A{s = 0.333f});
            t1.c.Add(new B{z = "MYTEXT"});
            t1.d = 5;
            Console.WriteLine("Converting to json...");

            SerializerConfig config = new SerializerConfig();
            config.Indent = true;

            string text = Json.Serialize(t1, config);
            Console.WriteLine(text);

            //TEST2
            TypesInfo ti = new TypesInfo();
            ti.RegisterType<A>();
            ti.RegisterType<B>(true);

            Console.WriteLine("Deserializing");
            Test t2 = Json.Deserialize<Test>(text, ti);
            t2.c[0].Write();
            t2.c[1].Write();

            //TEST3
            string jtext = "{ kalabanga: 5, b  : \"Тестовая Строка\", c: \"TestString\" }";
            JsonObject jo = new JsonObject(jtext);
            Console.WriteLine(jo.GetString("b"));
            Console.WriteLine(jo.GetString("c"));

            Console.WriteLine("Must be: Тестовая строка");

            Console.ReadKey();
        }
    }
}
