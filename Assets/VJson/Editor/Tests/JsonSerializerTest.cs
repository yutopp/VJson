using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using VJson;

namespace VJson.UnitTests
{
    class SomeObject
    {
        private float _p = 3.14f; // A private field will not be exported.
        public int X;
        public string Y;

        public override bool Equals(object rhsObj) {
            var rhs = rhsObj as SomeObject;
            if (rhs == null) {
                return false;
            }

            return _p == rhs._p && X == rhs.X && Y == rhs.Y;
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }
    }

    class DerivedSomeObject : SomeObject
    {
        public bool D;

        public override bool Equals(object rhsObj) {
            var rhs = rhsObj as DerivedSomeObject;
            if (rhs == null) {
                return false;
            }

            return D == rhs.D && base.Equals(rhsObj);
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }
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
        public void SerializeTest()
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));

            using(var textWriter = new StringWriter()) {
                serializer.Serialize(textWriter, obj);
                var actual = textWriter.ToString();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void DeserializeTest()
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            using(var textReader = new StringReader(expected)) {
                var actual = serializer.Deserialize(textReader);

                Assert.AreEqual(obj, actual);
            }
        }

        //
        static object [] FixtureArgs = {
            // Boolean
            new object[] {
                true,
                @"true",
            },

            new object[] {
                false,
                @"false",
            },

            // Null
            new object[] {
                (object)null,
                @"null",
            },

            // Numbers
            new object[] {
                1,
                @"1",
            },

            // Strings
            new object[] {
                "🍣",
                @"""🍣"""
            },

            // Arrays
            new object[] {
                new object[] {1, "hoge", null},
                @"[1,""hoge"",null]",
            },
            new object[] {
                new int[] {1, 2, 3},
                @"[1,2,3]",
            },

            // Objects
            new object[] {
                new SomeObject {
                    X = 10,
                    Y = "abab",
                },
                @"{""X"":10,""Y"":""abab""}",
            },
            new object[] {
                new DerivedSomeObject {
                    X = 10,
                    Y = "abab",
                    D = true,
                },
                @"{""D"":true,""X"":10,""Y"":""abab""}",
            },
            new object[] {
                (SomeObject)(new DerivedSomeObject {
                        X = 20,
                        Y = "cdcd",
                        D = false,
                    }),
                @"{""D"":false,""X"":20,""Y"":""cdcd""}",
            },
        };
    }
}
