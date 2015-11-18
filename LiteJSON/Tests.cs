using NUnit.Framework;

namespace LiteJSON
{
    class Tests
    {
        [TestFixture]
        public class NUnitTests
        {
            [Test]
            public void ArrayTest()
            {
                string jtext = "{ a: [0.5, 1] }";
                JsonObject jo = new JsonObject(jtext);
                var a1 = jo.GetJsonArray("a").ToArrayDouble();
                var a2 = jo.GetJsonArray("a").ToArrayFloat();
                var a3 = jo.GetJsonArray("a").ToArrayInt(true);
                var a4 = jo.GetJsonArray("a").ToArrayLong(true);

                Assert.AreEqual(a1[0], 0.5);
                Assert.AreEqual(a1[1], 1.0);

                Assert.AreEqual(a2[0], 0.5f);
                Assert.AreEqual(a2[1], 1.0f);

                Assert.AreEqual(a3[0], 0);
                Assert.AreEqual(a3[1], 1);

                Assert.AreEqual(a4[0], 0);
                Assert.AreEqual(a4[1], 1);
            }

            [Test]
            public void ListTest()
            {
                string jtext = "{ a: [0.5, 1] }";
                JsonObject jo = new JsonObject(jtext);
                var a1 = jo.GetJsonArray("a").ToListDouble();
                var a2 = jo.GetJsonArray("a").ToListFloat();
                var a3 = jo.GetJsonArray("a").ToListInt(true);
                var a4 = jo.GetJsonArray("a").ToListLong(true);

                Assert.AreEqual(a1[0], 0.5);
                Assert.AreEqual(a1[1], 1.0);

                Assert.AreEqual(a2[0], 0.5f);
                Assert.AreEqual(a2[1], 1.0f);

                Assert.AreEqual(a3[0], 0);
                Assert.AreEqual(a3[1], 1);

                Assert.AreEqual(a4[0], 0);
                Assert.AreEqual(a4[1], 1);
            }

            [Test]
            public void StringsTest()
            {
                string jtext = "{ kalabanga: 5, b  : \"Тестовая Строка\", c: \"TestString\" }";
                JsonObject jo = new JsonObject(jtext);

                Assert.AreEqual(jo.GetString("b"), "Тестовая Строка");
                Assert.AreEqual(jo.GetString("c"), "TestString");
            }

            interface ITestClass : IJsonDeserializable
            {
                int GetNum();
            }

            class TestClassA : ITestClass
            {
                private int a;

                public virtual void FromJson(JsonObject jsonObject)
                {
                    a = jsonObject.GetInt("a");
                }

                public virtual int GetNum()
                {
                    return a;
                }
            }

            class TestClassB : TestClassA
            {
                private int b;

                public override void FromJson(JsonObject jsonObject)
                {
                    base.FromJson(jsonObject);
                    b = jsonObject.GetInt("b");
                }

                public override int GetNum()
                {
                    return base.GetNum() + b;
                }
            }

            [Test]
            public void TypeParseTest()
            {
                TypesInfo ti = new TypesInfo();
                ti.RegisterType<TestClassA>();
                ti.RegisterType<TestClassB>();

                JsonObject jo = new JsonObject("{a:[(TestClassA){ a: 5 }, (TestClassB){ a: 5, b: 6 }]}", ti);
                ITestClass[] tClasses = jo.GetJsonArray("a").DeserializeToArray<ITestClass>();
                
                Assert.AreEqual(tClasses[0].GetNum(), 5);
                Assert.AreEqual(tClasses[1].GetNum(), 11);
            }
        }
    }
}
