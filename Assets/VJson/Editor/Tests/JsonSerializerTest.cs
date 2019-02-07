using System.IO;
using System.Text;
using NUnit.Framework;
using VJson;

namespace VJson.UnitTests
{
    class SomeObject
    {
        public int X;
        public string Y;
    }

    [TestFixtureSource("FixtureArgs")]
    class JsonSerializerTests
    {
        object obj;
        string expected;

        public JsonSerializerTests(object obj, string expected) {
            this.obj = obj;
            this.expected = expected;
        }

        [Test]
        public void SampleTest()
        {
            var serializer = new VJson.JsonSerializer(obj.GetType());

            using(var textWriter = new StringWriter()) {
                serializer.Serialize(textWriter, obj);
                var actual = textWriter.ToString();

                Assert.AreEqual(expected, actual);
            }
        }

        //
        static object [] FixtureArgs = {
            // Numbers
            new object[] {
                1,
                @"1",
            },
            // Arrays
            new object[] {
                new object[]{1, "hoge", null},
                @"[1,""hoge"",null]",
            },
            // Objects
            new object[] {
                new SomeObject {
                    X = 10,
                    Y = "abab",
                },
                @"{""X"":10,""Y"":""abab""}",
            },
        };
    }
}
