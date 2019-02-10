using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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

    class CustomObject
    {
        [JsonField(TypeHints=new Type[] {typeof(bool), typeof(SomeObject)})]
        public object Obj;

        public override bool Equals(object rhsObj) {
            var rhs = rhsObj as CustomObject;
            if (rhs == null) {
                return false;
            }

            return object.Equals(Obj, rhs.Obj);
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }
    }

    class CustomObjectHasArray
    {
        [JsonField(TypeHints=new Type[] {typeof(SomeObject), typeof(SomeObject[])})]
        public object Obj;

        public override bool Equals(object rhsObj) {
            var rhs = rhsObj as CustomObjectHasArray;
            if (rhs == null) {
                return false;
            }

            if (Obj == null && rhs.Obj == null) {
                return true;
            }

            if (Obj == null || rhs.Obj == null) {
                return false;
            }

            var lhsArr = Obj as SomeObject[];
            var rhsArr = rhs.Obj as SomeObject[];
            if (lhsArr != null && rhsArr != null) {
                return lhsArr.SequenceEqual(rhsArr);
            }

            var lhsSgt = Obj as SomeObject;
            var rhsAgt = rhs.Obj as SomeObject;
            return Object.Equals(lhsSgt, rhsAgt);
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }
    }

    class RenamedObject
    {
        [JsonField(Name="renamed")]
        public int Actual;

        public override bool Equals(object rhsObj) {
            var rhs = rhsObj as RenamedObject;
            if (rhs == null) {
                return false;
            }

            return Actual == rhs.Actual;
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }
    }

    class IgnorableObject
    {
        [JsonFieldIgnore(WhenValueIs=0)]
        public int Ignore0;

        [JsonFieldIgnore(WhenLengthIs=0)]
        public List<int> Ignore1;

        public override bool Equals(object rhsObj) {
            var rhs = rhsObj as IgnorableObject;
            if (rhs == null) {
                return false;
            }

            return Ignore0 == rhs.Ignore0
                && ((Ignore1 == null && rhs.Ignore1 == null)
                    || Ignore1 != null && rhs.Ignore1 != null && Ignore1.SequenceEqual(rhs.Ignore1));
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }
    }

    [Json(ImplicitConstructable=true)]
    class Hoge
    {
        public bool B {get; private set;}
        public Hoge(bool b)
        {
            B = b;
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
        public void SerializeFromStringTest()
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));

            var actual = serializer.Serialize(obj);

            Assert.AreEqual(expected, actual);
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

        [Test]
        public void DeserializeFromStringTest()
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            var actual = serializer.Deserialize(expected);

            Assert.AreEqual(obj, actual);
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
            new object[] {
                (new RenamedObject {
                        Actual = 42,
                    }),
                @"{""renamed"":42}",
            },
        };
    }

    class JsonSerializerForContainerTests
    {
        [TestCaseSource("ListArgs")]
        public void SerializeTest<E>(IEnumerable<E> obj, string expected)
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            using(var textWriter = new StringWriter()) {
                serializer.Serialize(textWriter, obj);
                var actual = textWriter.ToString();

                Assert.That(actual, Is.EquivalentTo(expected));
            }
        }

        [TestCaseSource("DictArgs")]
        public void SerializeTest<K, V>(IDictionary<K, V> obj, string expected)
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            using(var textWriter = new StringWriter()) {
                serializer.Serialize(textWriter, obj);
                var actual = textWriter.ToString();

                Assert.That(actual, Is.EquivalentTo(expected));
            }
        }

        [TestCaseSource("ListArgs")]
        public void DeserializeTest<E>(IEnumerable<E> expected, string json)
        {
            var serializer = new VJson.JsonSerializer(expected != null ? expected.GetType() : typeof(object));
            using(var textReader = new StringReader(json)) {
                var actual = serializer.Deserialize(textReader);

                Assert.That(actual, Is.EquivalentTo(expected));
            }
        }

        [TestCaseSource("DictArgs")]
        public void DeserializeTest<K, V>(IDictionary<K, V> expected, string json)
        {
            var serializer = new VJson.JsonSerializer(expected != null ? expected.GetType() : typeof(object));
            using(var textReader = new StringReader(json)) {
                var actual = serializer.Deserialize(textReader);

                Assert.That(actual, Is.EquivalentTo(expected));
            }
        }

        [TestCaseSource("ListArgs")]
        public void DeserializeUntypedTest<E>(IEnumerable<E> expected, string json)
        {
            var serializer = new VJson.JsonSerializer(typeof(object));
            using(var textReader = new StringReader(json)) {
                var actual = serializer.Deserialize(textReader);

                Assert.That(actual, Is.EquivalentTo(expected));
            }
        }

        [TestCaseSource("DictArgs")]
        public void DeserializeUntypedTest<K, V>(IDictionary<K, V> expected, string json)
        {
            var serializer = new VJson.JsonSerializer(typeof(object));
            using(var textReader = new StringReader(json)) {
                var actual = serializer.Deserialize(textReader);

                Assert.That(actual, Is.EquivalentTo(expected));
            }
        }

        [Test]
        public void DeserializeHasHintFieldsBoolTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(CustomObject));
            using(var textReader = new StringReader(@"{""Obj"": true}")) {
                var actual = (CustomObject)serializer.Deserialize(textReader);

                Assert.AreEqual(typeof(bool), actual.Obj.GetType());
                Assert.AreEqual(true, actual.Obj);
            }
        }

        [Test]
        public void DeserializeHasHintFieldsClassTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(CustomObject));
            using(var textReader = new StringReader(@"{""Obj"": {""X"":10}}")) {
                var actual = (CustomObject)serializer.Deserialize(textReader);

                Assert.AreEqual(typeof(SomeObject), actual.Obj.GetType());
                Assert.AreEqual(new SomeObject{
                        X = 10,
                    }, actual.Obj);
            }
        }

        [Test]
        public void DeserializeHasHintFieldsArrayOrSingletonSgtTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(CustomObjectHasArray));
            using(var textReader = new StringReader(@"{""Obj"": {""X"":10}}")) {
                var actual = (CustomObjectHasArray)serializer.Deserialize(textReader);

                Assert.AreEqual(typeof(SomeObject), actual.Obj.GetType());
                Assert.That(actual.Obj,
                            Is.EqualTo(new SomeObject {
                                    X = 10,
                                })
                            );
            }
        }

        [Test]
        public void DeserializeImplicitConstructableTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(Hoge));
            using(var textReader = new StringReader(@"true")) {
                var actual = (Hoge)serializer.Deserialize(textReader);

                Assert.That(actual.B, Is.EqualTo(true));
            }
        }

        public static object[] ListArgs = {
            new object[] {
                new int[] {},
                @"[]",
            },
            new object[] {
                new object[] {1, "hoge", null},
                @"[1,""hoge"",null]",
            },
            new object[] {
                new int[] {1, 2, 3},
                @"[1,2,3]",
            },
            new object[] {
                new List<int> {1, 2, 3},
                @"[1,2,3]",
            },
            new object[] {
                new List<Dictionary<string, int>> {
                    new Dictionary<string, int> {
                        {"a", 1},
                    },
                    new Dictionary<string, int> {
                        {"b", 2},
                    },
                    new Dictionary<string, int> {
                        {"c", 3},
                    },
                },
                @"[{""a"":1},{""b"":2},{""c"":3}]",
            },
        };

        public static object[] DictArgs = {
            new object[] {
                new Dictionary<string, object>(),
                @"{}",
            },
            new object[] {
                new Dictionary<string, object> {
                    {"X", 10},
                    {"Y", "abab"},
                },
                @"{""X"":10,""Y"":""abab""}",
            },
        };
    }
}
