using System.IO;
using System.Text;
using NUnit.Framework;
using VJson;

namespace VJson.UnitTests
{
    class SomeObject
    {
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
            new object[] {
                new SomeObject() {
                },
                "",
            },
        };
    }
}
