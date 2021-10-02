//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VJson.UnitTests
{
    [Json]
    class SomeObject
    {
        private float _p = 3.14f; // Fields which are non-public will not be exported defaultly.
        public int X;
        public string Y;

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as SomeObject;
            if (rhs == null)
            {
                return false;
            }

            return _p == rhs._p && X == rhs.X && Y == rhs.Y;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{{_p = {0}, X = {1}, Y = {2}}}", _p, X, Y);
        }
    }

    [Json]
    sealed class DerivedSomeObject : SomeObject
    {
        public bool D;

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as DerivedSomeObject;
            if (rhs == null)
            {
                return false;
            }

            return D == rhs.D && base.Equals(rhsObj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{{D = {0}, base = {1}}}", D, base.ToString());
        }
    }

    // Fields which are non-public BUT having [JsonField] etc... will BE exported!
    [Json]
    sealed class HasNonPublicObject
    {
        [JsonField] internal long _p1 = 2;
        [JsonField] long _p2 = 4; // Fields which are non-public BUT having [JsonField] (+etc) attributes will BE exported!
        [JsonField] private long _p3 = 6;

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as HasNonPublicObject;
            if (rhs == null)
            {
                return false;
            }

            return _p1 == rhs._p1 && _p2 == rhs._p2 && _p3 == rhs._p3;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{{_p1 = {0}, _p2 = {1}, _p3 = {2}}}", _p1, _p2, _p3);
        }
    }

    [Json]
    sealed class CustomObject
    {
        [JsonField(TypeHints = new Type[] { typeof(bool), typeof(SomeObject) })]
        public object Obj = default(object);

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as CustomObject;
            if (rhs == null)
            {
                return false;
            }

            return object.Equals(Obj, rhs.Obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    [Json]
    sealed class CustomObjectHasArray
    {
        [JsonField(TypeHints = new Type[] { typeof(SomeObject), typeof(SomeObject[]) })]
        public object Obj = default(object);

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as CustomObjectHasArray;
            if (rhs == null)
            {
                return false;
            }

            if (Obj == null && rhs.Obj == null)
            {
                return true;
            }

            if (Obj == null || rhs.Obj == null)
            {
                return false;
            }

            var lhsArr = Obj as SomeObject[];
            var rhsArr = rhs.Obj as SomeObject[];
            if (lhsArr != null && rhsArr != null)
            {
                return lhsArr.SequenceEqual(rhsArr);
            }

            var lhsSgt = Obj as SomeObject;
            var rhsAgt = rhs.Obj as SomeObject;
            return Object.Equals(lhsSgt, rhsAgt);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    [Json]
    sealed class RenamedObject
    {
        [JsonField(Name = "renamed")]
        public int Actual;

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as RenamedObject;
            if (rhs == null)
            {
                return false;
            }

            return Actual == rhs.Actual;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    [Json]
    sealed class IgnorableObject
    {
        [JsonFieldIgnorable(WhenValueIs = 0)]
        public int Ignore0;

        [JsonFieldIgnorable(WhenLengthIs = 0)]
        public List<int> Ignore1;

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as IgnorableObject;
            if (rhs == null)
            {
                return false;
            }

            return Ignore0 == rhs.Ignore0
                && ((Ignore1 == null && rhs.Ignore1 == null)
                    || Ignore1 != null && rhs.Ignore1 != null && Ignore1.SequenceEqual(rhs.Ignore1));
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    [Json]
    sealed class PartialTypedObject
    {
        public int A;
        public Dictionary<string, INode> Ext;

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as PartialTypedObject;
            if (rhs == null)
            {
                return false;
            }

            return A == rhs.A &&
                Ext.OrderBy(p => p.Key).SequenceEqual(rhs.Ext.OrderBy(p => p.Key));
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    [Json(ImplicitConstructable = true)]
    class Hoge
    {
        public bool B { get; private set; }

        [Preserve]
        public Hoge(bool b)
        {
            B = b;
        }
    }

    public enum EnumAsInt
    {
        A,
        B,
        C = 100,
    }

    [Json(EnumConversion = EnumConversionType.AsString)]
    public enum EnumAsString
    {
        NameA,
        NameB,

        [JsonField(Name = "OtherName")]
        NameC = 100,
    }

    public class HasNullable
    {
        public Nullable<int> X;

        [JsonFieldIgnorable]
        public Nullable<int> Y;

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as HasNullable;
            if (rhs == null)
            {
                return false;
            }

            return Object.Equals(X, rhs.X) && Object.Equals(Y, rhs.Y);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public class HasStatic
    {
        public int I;
        public static int S = 301; // Will not be exported

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as HasStatic;
            if (rhs == null)
            {
                return false;
            }

            return Object.Equals(I, rhs.I);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    class JsonSerializerTests
    {
        [Test]
        [TestCaseSource(nameof(CommonArgs))]
        [TestCaseSource(nameof(OnlySerializeArgs))]
        public void SerializeFromStringTest(object obj, string expected)
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            var actual = serializer.Serialize(obj);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCaseSource(nameof(WithIndentArgs))]
        public void SerializeWithIndentFromStringTest(object obj, string expected)
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            var actual = serializer.Serialize(obj, 4/* indent */);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCaseSource(nameof(CommonArgs))]
        [TestCaseSource(nameof(WithIndentArgs))]
        [TestCaseSource(nameof(OnlyDeserializeArgs))]
        public void DeserializeFromStringTest(object obj, string expected)
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            var actual = serializer.Deserialize(expected);

            Assert.AreEqual(obj, actual);
        }

        //
        static object[] CommonArgs = {
            // Boolean
            new object[] {
                true,
                @"true",
            },
            new object[] {
                false,
                @"false",
            },
            new object[] {
                new BooleanNode(true),
                @"true",
            },

            // Null
            new object[] {
                (object)null,
                @"null",
            },
            new object[] {
                new NullNode(),
                @"null",
            },

            // Numbers
            new object[] {
                (byte)1,
                @"1",
            },
            new object[] {
                (sbyte)1,
                @"1",
            },
            new object[] {
                (char)1,
                @"1",
            },
            new object[] {
                (decimal)1,
                @"1",
            },
            new object[] {
                (double)1,
                @"1",
            },
            new object[] {
                (float)1,
                @"1",
            },
            new object[] {
                (int)1,
                @"1",
            },
            new object[] {
                (uint)1,
                @"1",
            },
            new object[] {
                (short)1,
                @"1",
            },
            new object[] {
                (ushort)1,
                @"1",
            },
            new object[] {
                (long)1,
                @"1",
            },
            new object[] {
                (ulong)1,
                @"1",
            },
            new object[] {
                new IntegerNode(1),
                @"1",
            },
            new object[] {
                (float)3.1400001,
                @"3.1400001",
            },
            new object[] {
                (double)3.1400000000000001,
                @"3.1400000000000001",
            },
            new object[] {
                (decimal)3.1400001,
                @"3.1400001",
            },
            new object[] {
                (float)1.25,
                @"1.25",
            },
            new object[] {
                (float)-1.25,
                @"-1.25",
            },
            new object[] {
                new FloatNode(-1.25f),
                @"-1.25",
            },

            // Strings
            new object[] {
                "🍣",
                @"""🍣"""
            },
            new object[] {
                @"a
b
c
""
".AsLF(),
                "\"a\\nb\\nc\\n\\\"\\n\"",
            },
            new object[] {
                "\" \\ / \b \n \r \t",
                "\"\\\" \\\\ / \\b \\n \\r \\t\"",
            },
            new object[] {
                "\"\\/\b\n\r\t",
                "\"\\\"\\\\/\\b\\n\\r\\t\"",
            },
            new object[] {
                new StringNode("\"\\/\b\n\r\t"),
                "\"\\\"\\\\/\\b\\n\\r\\t\"",
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
            new object[] {
                new List<object> {1, 2, 3},
                @"[1,2,3]",
            },
            new object[] {
                new List<int> {1, 2, 3},
                @"[1,2,3]",
            },
            new object[] {
                new ArrayNode(new List<INode> {
                        new IntegerNode(1),
                        new IntegerNode(2),
                        new IntegerNode(3)
                    }),
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
                new HasNonPublicObject {},
                @"{""_p1"":2,""_p2"":4,""_p3"":6}",
            },
            new object[] {
                new RenamedObject {
                    Actual = 42,
                },
                @"{""renamed"":42}",
            },
            new object[] {
                new IgnorableObject {
                    Ignore0 = 0,
                },
                @"{}",
            },
            new object[] {
                new IgnorableObject {
                    Ignore0 = 1,
                },
                @"{""Ignore0"":1}",
            },
            new object[] {
                new IgnorableObject {
                    Ignore1 = new List<int> {1},
                },
                @"{""Ignore1"":[1]}",
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(10)},
                        {"Y", new StringNode("abab")},
                    }),
                @"{""X"":10,""Y"":""abab""}",
            },
            new object[] {
                new PartialTypedObject {
                    A = 10,
                    Ext = new Dictionary<string, INode> {
                        {"X", new ArrayNode(new List<INode> {
                                    new IntegerNode(10)
                                })},
                        {"Y", new ObjectNode(new Dictionary<string, INode> {
                                    {"A", new IntegerNode(20)},
                                })},
                    }
                },
                @"{""A"":10,""Ext"":{""X"":[10],""Y"":{""A"":20}}}",
            },

            // Enums
            new object[] {
                EnumAsInt.A,
                @"0",
            },
            new object[] {
                EnumAsInt.B,
                @"1",
            },
            new object[] {
                EnumAsInt.C,
                @"100",
            },
            new object[] {
                EnumAsString.NameA,
                @"""NameA""",
            },
            new object[] {
                EnumAsString.NameB,
                @"""NameB""",
            },
            new object[] {
                EnumAsString.NameC,
                @"""OtherName""",
            },

            // Nullable
            new object[] {
                (Nullable<int>)(null),
                @"null",
            },
            new object[] {
                new Nullable<int>(1),
                @"1",
            },
            new object[] {
                new HasNullable(),
                "{\"X\":null}"
            },
            new object[] {
                new HasNullable {
                    X = 10,
                },
                "{\"X\":10}"
            },
            new object[] {
                new HasStatic {
                    I = 10,
                },
                "{\"I\":10}"
            },
        };

        static object[] OnlySerializeArgs = {
            new object[] {
                new IgnorableObject {
                    Ignore1 = new List<int>(),
                },
                @"{}",
            },
        };

        static object[] OnlyDeserializeArgs = {
            new object[] {
                "/",
                "\"\\/\"",
            },
            new object[] {
                "/",
                "\"/\"",
            },
        };

        static object[] WithIndentArgs = {
            new object[] {
                false,
                "false",
            },
            new object[] {
                1,
                "1",
            },
            new object[] {
                3.14f,
                "3.1400001",
            },
            new object[] {
                "🍣",
                "\"🍣\"",
            },
            new object[] {
                new object[] {},
                "[]",
            },
            new object[] {
                new object[] {1},
                @"[
    1
]".AsLF(),
            },
            new object[] {
                new object[] {1, "a"},
                @"[
    1,
    ""a""
]".AsLF(),
            },
            new object[] {
                new Dictionary<string, object> {},
                "{}",
            },
            new object[] {
                new Dictionary<string, object> {
                    {"a", 1}
                },
                @"{
    ""a"": 1
}".AsLF(),
            },
            new object[] {
                new Dictionary<string, object> {
                    {"a", 1},
                    {"b", 2}
                },
                @"{
    ""a"": 1,
    ""b"": 2
}".AsLF(),
            },
            new object[] {
                new Dictionary<string, object> {
                    {"a", new int[] { 1, 2, 3 }},
                    {"b",
                     new Dictionary<string, object> {
                            {"1a", 10},
                            {"1b", new string[] { "こんにちは", "Hello" }},
                            {"1c",
                             new Dictionary<string, object> {
                                    {"🍣", 1234},
                             }},
                     }}
                },
                @"{
    ""a"": [
        1,
        2,
        3
    ],
    ""b"": {
        ""1a"": 10,
        ""1b"": [
            ""こんにちは"",
            ""Hello""
        ],
        ""1c"": {
            ""🍣"": 1234
        }
    }
}".AsLF(),
            },
        };
    }

    class JsonSerializerForContainerTests
    {
        [Test]
        [TestCaseSource(nameof(ListArgs))]
        public void SerializeTest(IEnumerable obj, string expected)
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            var actual = serializer.Serialize(obj);
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(DictArgs))]
        public void SerializeTest(IDictionary obj, string expected)
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            var actual = serializer.Serialize(obj);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(ListArgs))]
        public void DeserializeTest(IEnumerable expected, string json)
        {
            var serializer = new VJson.JsonSerializer(expected != null ? expected.GetType() : typeof(object));
            var actual = serializer.Deserialize(json);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(DictArgs))]
        public void DeserializeTest(IDictionary expected, string json)
        {
            var serializer = new VJson.JsonSerializer(expected != null ? expected.GetType() : typeof(object));
            var actual = serializer.Deserialize(json);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(ListArgs))]
        public void DeserializeUntypedTest(IEnumerable expected, string json)
        {
            var serializer = new VJson.JsonSerializer(typeof(object));
            var actual = serializer.Deserialize(json);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(DictArgs))]
        public void DeserializeUntypedTest(IDictionary expected, string json)
        {
            var serializer = new VJson.JsonSerializer(typeof(object));
            var actual = serializer.Deserialize(json);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void DeserializeHasHintFieldsBoolTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(CustomObject));
            var actual = (CustomObject)serializer.Deserialize(@"{""Obj"": true}");

            Assert.AreEqual(typeof(bool), actual.Obj.GetType());
            Assert.AreEqual(true, actual.Obj);
        }

        [Test]
        public void DeserializeHasHintFieldsClassTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(CustomObject));
            var actual = (CustomObject)serializer.Deserialize(@"{""Obj"": {""X"":10}}");

            Assert.AreEqual(typeof(SomeObject), actual.Obj.GetType());
            Assert.AreEqual(new SomeObject
            {
                X = 10,
            }, actual.Obj);
        }

        [Test]
        public void DeserializeHasHintFieldsArrayOrSingletonSgtTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(CustomObjectHasArray));
            var actual = (CustomObjectHasArray)serializer.Deserialize(@"{""Obj"": {""X"":10}}");

            Assert.AreEqual(typeof(SomeObject), actual.Obj.GetType());
            Assert.That(actual.Obj,
                        Is.EqualTo(new SomeObject
                        {
                            X = 10,
                        })
                );
        }

        [Test]
        public void DeserializeImplicitConstructableTest()
        {
            var serializer = new VJson.JsonSerializer(typeof(Hoge));
            var actual = (Hoge)serializer.Deserialize(@"true");

            Assert.That(actual.B, Is.EqualTo(true));
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

    class JsonSerializerErrorCaseTests
    {
        [Test]
        [TestCaseSource(nameof(CommonArgs))]
        public void DeserializeFailureTest(Type ty, string json, string errorMsg)
        {
            var s = new JsonSerializer(ty);
            var ex = Assert.Throws<DeserializeFailureException>(() => s.Deserialize(json));

            Assert.AreEqual(errorMsg, ex.Message);
        }

        //
        static object[] CommonArgs = {
            new object[] {
                typeof(bool),
                "1",
                "(root): Integer node (1) cannot convert to System.Boolean.",
            },
            new object[] {
                typeof(bool),
                "null",
                "(root): Null node cannot convert to non-boxed value(System.Boolean).",
            },
            new object[] {
                typeof(int),
                "true",
                "(root): Boolean node (True) cannot convert to System.Int32.",
            },
            new object[] {
                typeof(int),
                "3.14",
                "(root): System.Double value (3.14) cannot convert to System.Int32 (Reason: Not implicit constructable).",
            },
            new object[] {
                typeof(uint),
                "-1",
                "(root): System.Int64 value (-1) cannot convert to System.UInt32.",
            },
            new object[] {
                typeof(uint),
                "4294967296",
                "(root): System.Int64 value (4294967296) cannot convert to System.UInt32.",
            },
            new object[] {
                typeof(byte),
                "256",
                "(root): System.Int64 value (256) cannot convert to System.Byte.",
            },
            new object[] {
                typeof(int),
                "null",
                "(root): Null node cannot convert to non-boxed value(System.Int32).",
            },
            new object[] {
                typeof(string),
                "true",
                "(root): Boolean node (True) cannot convert to System.String.",
            },
            new object[] {
                typeof(string[]),
                "true",
                "(root): Boolean node (True) cannot convert to System.String[].",
            },
            new object[] {
                typeof(List<string>),
                "true",
                "(root): Boolean node (True) cannot convert to System.Collections.Generic.List`1[System.String].",
            },
            new object[] {
                typeof(string[]),
                "[\"\", 1]",
                "(root)[1]: Integer node (1) cannot convert to System.String.",
            },
            new object[] {
                typeof(List<string>),
                "[\"\", 1]",
                "(root)[1]: Integer node (1) cannot convert to System.String.",
            },
            new object[] {
                typeof(Dictionary<string, int>),
                "true",
                "(root): System.Boolean value (True) cannot convert to System.Collections.Generic.Dictionary`2[System.String,System.Int32] (Reason: Not implicit constructable).",
            },
            new object[] {
                typeof(Dictionary<string, int>),
                "{\"a\": 1, \"b\": \"bo\"}",
                "(root)[\"b\"]: String node cannot convert to System.Int32.",
            },
            new object[] {
                typeof(SomeObject),
                "true",
                "(root): System.Boolean value (True) cannot convert to VJson.UnitTests.SomeObject (Reason: Not implicit constructable).",
            },
            new object[] {
                typeof(SomeObject),
                "{\"X\": \"bo\"}",
                "(root)[\"X\"]: String node cannot convert to System.Int32.",
            },
            new object[] {
                typeof(CustomObject),
                "{\"Obj\": \"bo\"}",
                "(root)[\"Obj\"]: String node cannot convert to one of [System.Boolean, VJson.UnitTests.SomeObject].",
            },
            new object[] {
                typeof(Hoge),
                "42",
                "(root): System.Int64 value (42) cannot convert to VJson.UnitTests.Hoge (Reason: Suitable constructers are not found).",
            },
            new object[] {
                typeof(EnumAsInt),
                "42",
                "(root): System.Int64 value (42) cannot convert to VJson.UnitTests.EnumAsInt (Reason: Enum value is not defined).",
            },
            new object[] {
                typeof(EnumAsInt),
                "\"A\"",
                "(root): String node cannot convert to VJson.UnitTests.EnumAsInt.",
            },
            new object[] {
                typeof(EnumAsString),
                "1",
                "(root): Integer node (1) cannot convert to VJson.UnitTests.EnumAsString.",
            },
            new object[] {
                typeof(EnumAsString),
                "\"HogeHoge\"",
                "(root): System.String value cannot convert to VJson.UnitTests.EnumAsString (Reason: Enum name is not defined).",
            },
            new object[] {
                typeof(EnumAsString),
                "\"NameC\"",
                "(root): System.String value cannot convert to VJson.UnitTests.EnumAsString (Reason: Enum name is not defined).",
            },
        };
    }

    class JsonBetweenNodesTests
    {
        [Test]
        [TestCaseSource("CommonArgs")]
        public void SerializeToNodeTest(INode expected, object obj)
        {
            var serializer = new VJson.JsonSerializer(obj != null ? obj.GetType() : typeof(object));
            var actual = serializer.SerializeToNode(obj);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource("CommonArgs")]
        public void DeserializeFromNodeTest(INode node, object expected)
        {
            var deserializer = new VJson.JsonDeserializer(expected != null ? expected.GetType() : typeof(object));
            var actual = deserializer.DeserializeFromNode(node);

            Assert.AreEqual(expected, actual);
        }

        //
        static object[] CommonArgs = {
            // Boolean
            new object[] {
                new BooleanNode(true),
                true,
            },

            // Null
            new object[] {
                new NullNode(),
                null,
            },

            // Numbers
            new object[] {
                new IntegerNode(1),
                1,
            },
            new object[] {
                new FloatNode(-1.25f),
                -1.25,
            },

            // Strings
            new object[] {
                new StringNode("abc"),
                "abc",
            },

            // Arrays
            new object[] {
                new ArrayNode(new List<INode> {
                        new IntegerNode(1),
                        new IntegerNode(2),
                        new IntegerNode(3)
                    }),
                new List<int> { 1, 2, 3 },
            },

            // Objects
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(10)},
                        {"Y", new StringNode("abab")},
                    }),
                new SomeObject {
                    X = 10,
                    Y = "abab",
                },
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(10)},
                        {"Y", new StringNode("abab")},
                        {"D", new BooleanNode(true)},
                    }),
                new DerivedSomeObject {
                    X = 10,
                    Y = "abab",
                    D = true,
                },
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(20)},
                        {"Y", new StringNode("cdcd")},
                        {"D", new BooleanNode(false)},
                    }),
                (SomeObject)(new DerivedSomeObject {
                        X = 20,
                        Y = "cdcd",
                        D = false,
                    }),
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"renamed", new IntegerNode(42)},
                    }),
                new RenamedObject {
                    Actual = 42,
                },
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                    }),
                new IgnorableObject {
                    Ignore0 = 0,
                },
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"Ignore0", new IntegerNode(1)},
                    }),
                new IgnorableObject {
                    Ignore0 = 1,
                },
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"Ignore1", new ArrayNode(new List<INode> {
                                    new IntegerNode(1),
                                })
                        },
                    }),
                new IgnorableObject {
                    Ignore1 = new List<int> {1},
                },
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"A", new IntegerNode(10)},
                        {"Ext", new ObjectNode(new Dictionary<string, INode> {
                                    {"X", new ArrayNode(new List<INode> {
                                                new IntegerNode(10)
                                            })},
                                    {"Y", new ObjectNode(new Dictionary<string, INode> {
                                                {"A", new IntegerNode(20)},
                                            })},
                                })},
                    }),
                new PartialTypedObject {
                    A = 10,
                    Ext = new Dictionary<string, INode> {
                        {"X", new ArrayNode(new List<INode> {
                                    new IntegerNode(10)
                                })},
                        {"Y", new ObjectNode(new Dictionary<string, INode> {
                                    {"A", new IntegerNode(20)},
                                })},
                    }
                },
            },

            // Enums
            new object[] {
                new IntegerNode(0),
                EnumAsInt.A,
            },
            new object[] {
                new IntegerNode(1),
                EnumAsInt.B,
            },
            new object[] {
                new IntegerNode(100),
                EnumAsInt.C,
            },
            new object[] {
                new StringNode("NameA"),
                EnumAsString.NameA,
            },
            new object[] {
                new StringNode("NameB"),
                EnumAsString.NameB,
            },
            new object[] {
                new StringNode("OtherName"),
                EnumAsString.NameC,
            },

            // Nullable
            new object[] {
                new NullNode(),
                (Nullable<int>)(null),
            },
            new object[] {
                new IntegerNode(1),
                new Nullable<int>(1),
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new NullNode()},
                    }),
                new HasNullable(),
            },
            new object[] {
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(10)},
                    }),
                new HasNullable {
                    X = 10,
                },
            },
        };
    }

    public static class StringExtensions
    {
        public static string AsLF(this string str)
        {
            return str.Replace("\r\n", "\n");
        }
    }
}
