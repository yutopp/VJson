//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace VJson.Schema.UnitTests
{
    public class ValidatorWithSerializerTests
    {
        [Test]
        [TestCaseSource(nameof(NotRequiredObjectArgs))]
        [TestCaseSource(nameof(NotRequiredObjectWithIgnorableArgs))]
        [TestCaseSource(nameof(HasDictionaryArgs))]
        [TestCaseSource(nameof(HasEnumerableArgs))]
        [TestCaseSource(nameof(HasEnumerableNestedArgs))]
        [TestCaseSource(nameof(HasRequiredItemsArgs))]
        [TestCaseSource(nameof(HasRequiredStringArgs))]
        [TestCaseSource(nameof(HasRequiredButIgnorableStringArgs))]
        [TestCaseSource(nameof(HasDepsArgs))]
        [TestCaseSource(nameof(HasNestedArgs))]
        [TestCaseSource(nameof(DerivingArgs))]
        [TestCaseSource(nameof(HasNullableArgs))]
        [TestCaseSource(nameof(HasEnumArgs))]
        [TestCaseSource(nameof(HasCustomTagArgs))]
        public void ValidationTest<T>(T o, string expectedMsg, string _expectedContent)
        {
            var r = new JsonSchemaRegistry();
            var schema = JsonSchema.CreateFromType<T>(r);

            var ex = schema.Validate(o, r);

            var message =
                String.Format("{0} : {1}", new JsonSerializer(typeof(T)).Serialize(o), schema.ToString());
            Assert.AreEqual(expectedMsg, ex != null ? ex.Message : null, message);
        }

        [Test]
        [TestCaseSource(nameof(NotRequiredObjectINodeArgs))]
        [TestCaseSource(nameof(NotRequiredObjectWithIgnorableINodeArgs))]
        // [TestCaseSource(nameof(HasDictionaryArgs))]
        // [TestCaseSource(nameof(HasEnumerableArgs))]
        // [TestCaseSource(nameof(HasEnumerableNestedArgs))]
        // [TestCaseSource(nameof(HasRequiredItemsArgs))]
        // [TestCaseSource(nameof(HasRequiredStringArgs))]
        // [TestCaseSource(nameof(HasRequiredButIgnorableStringArgs))]
        // [TestCaseSource(nameof(HasDepsArgs))]
        // [TestCaseSource(nameof(HasNestedArgs))]
        // [TestCaseSource(nameof(DerivingArgs))]
        // [TestCaseSource(nameof(HasNullableArgs))]
        // [TestCaseSource(nameof(HasEnumArgs))]
        // [TestCaseSource(nameof(HasCustomTagArgs))]
        public void ValidationForINodeTest(Type ty, INode node, string expectedMsg, string _expectedContent)
        {
            var r = new JsonSchemaRegistry();
            var schema = JsonSchema.CreateFromType(ty, r);

            var ex = schema.Validate(node, r);

            var message =
                String.Format("{0} : {1}", new JsonSerializer(ty).Serialize(node), schema.ToString());
            Assert.AreEqual(expectedMsg, ex != null ? ex.Message : null, message);
        }

        // TODO: Move to elsewhere
        [Test]
        [TestCaseSource(nameof(NotRequiredObjectArgs))]
        [TestCaseSource(nameof(NotRequiredObjectWithIgnorableArgs))]
        [TestCaseSource(nameof(HasDictionaryArgs))]
        [TestCaseSource(nameof(HasEnumerableArgs))]
        [TestCaseSource(nameof(HasEnumerableNestedArgs))]
        [TestCaseSource(nameof(HasRequiredItemsArgs))]
        [TestCaseSource(nameof(HasRequiredStringArgs))]
        [TestCaseSource(nameof(HasRequiredButIgnorableStringArgs))]
        [TestCaseSource(nameof(HasDepsArgs))]
        [TestCaseSource(nameof(HasNestedArgs))]
        [TestCaseSource(nameof(DerivingArgs))]
        [TestCaseSource(nameof(HasNullableArgs))]
        [TestCaseSource(nameof(HasEnumArgs))]
        [TestCaseSource(nameof(HasCustomTagArgs))]
        public void SerializationTest<T>(T o, string _expectedMsg, string expectedContent)
        {
            if (_expectedMsg != null)
            {
                return;
            }

            var content = new JsonSerializer(typeof(T)).Serialize(o);
            Assert.AreEqual(expectedContent, content);
        }

        [TestCaseSource(nameof(EnumArgs))]
        public void FromTypeValidationTest(Type ty, object e, string expectedMsg, string _expectedContent)
        {
            var schema = JsonSchema.CreateFromType(ty);

            var ex = schema.Validate(e);

            var message =
                String.Format("{0} : {1}", new JsonSerializer(ty).Serialize(e), schema.ToString());
            Assert.AreEqual(expectedMsg, ex != null ? ex.Message : null, message);
        }

        [TestCaseSource(nameof(EnumArgs))]
        public void FromTypeSerializationTest(Type ty, object e, string _expectedMsg, string expectedContent)
        {
            if (_expectedMsg != null)
            {
                return;
            }

            var content = new JsonSerializer(ty).Serialize(e);
            Assert.AreEqual(expectedContent, content);
        }

        [JsonSchema(Id = "not_required_object.json")]
        public class NotRequiredObject
        {
            [JsonSchema(Minimum = 1)]
            public int X;
        }

        public static object[] NotRequiredObjectArgs = new object[] {
            new object[] {
                new NotRequiredObject {X = 0},
                "Object.Property.Number.(root)[\"X\"]: Minimum assertion !(0 >= 1).",
                null,
            },
            new object[] {
                new NotRequiredObject {X = 1},
                null,
                "{\"X\":1}",
            },
        };

        public static object[] NotRequiredObjectINodeArgs = new object[] {
            new object[] {
                typeof(NotRequiredObject),
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(0)},
                    }),
                "Object.Property.Number.(root)[\"X\"]: Minimum assertion !(0 >= 1).",
                null,
            },
            new object[] {
                typeof(NotRequiredObject),
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(1)},
                    }),
                null,
                "{\"X\":1}",
            },
        };

        public class NotRequiredObjectWithIgnorable
        {
            [JsonSchema(Minimum = 1)]
            [JsonFieldIgnorable(WhenValueIs = -1)]
            public int X;
        }

        public static object[] NotRequiredObjectWithIgnorableArgs = new object[] {
            new object[] {
                new NotRequiredObjectWithIgnorable {X = -1},
                null,
                "{}",
            },
            new object[] {
                new NotRequiredObjectWithIgnorable {X = 0},
                "Object.Property.Number.(root)[\"X\"]: Minimum assertion !(0 >= 1).",
                null,
            },
            new object[] {
                new NotRequiredObjectWithIgnorable {X = 1},
                null,
                "{\"X\":1}",
            },
        };

        public static object[] NotRequiredObjectWithIgnorableINodeArgs = new object[] {
            new object[] {
                typeof(NotRequiredObjectWithIgnorable),
                new ObjectNode(new Dictionary<string, INode> {
                        // Ignorable is not a schema attribute, thus constraints are not applied to the INode...
                        // {"X", new IntegerNode(-1)},
                    }),
                null,
                "{}",
            },
            new object[] {
                typeof(NotRequiredObjectWithIgnorable),
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(0)},
                    }),
                "Object.Property.Number.(root)[\"X\"]: Minimum assertion !(0 >= 1).",
                null,
            },
            new object[] {
                typeof(NotRequiredObjectWithIgnorable),
                new ObjectNode(new Dictionary<string, INode> {
                        {"X", new IntegerNode(1)},
                    }),
                null,
                "{\"X\":1}",
            },
        };

        public class HasDictionary
        {
            public Dictionary<string, float> FP = new Dictionary<string, float>();
        }

        public static object[] HasDictionaryArgs = new object[] {
            new object[] {
                new HasDictionary(),
                null,
                "{\"FP\":{}}",
            },
            new object[] {
                new HasDictionary() {FP = null},
                // Empty is not allowed
                "Object.Property.(root)[\"FP\"]: Type is not matched(Actual: Null; Expected: object).",
                null,
            },
        };

        [Json]
        public class HasEnumerable
        {
            [ItemsJsonSchema(Minimum = 0.0, Maximum = 1.0)]
            public float[] Fs;
            [JsonField] public object[] Os = new object[] { };

            [JsonField] public List<float> FsList = new List<float>();
            [JsonField] public List<object> OsList = new List<object>();
        }

        public static object[] HasEnumerableArgs = new object[] {
            new object[] {
                new HasEnumerable {Fs = new float[] {}},
                null,
                "{\"Fs\":[],\"FsList\":[],\"Os\":[],\"OsList\":[]}",
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {-0.5f}},
                "Object.Property.Array.Items.Number.(root)[\"Fs\"][0]: Minimum assertion !(-0.5 >= 0).",
                null,
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {0.5f}},
                null,
                "{\"Fs\":[0.5],\"FsList\":[],\"Os\":[],\"OsList\":[]}",
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {1.5f}},
                "Object.Property.Array.Items.Number.(root)[\"Fs\"][0]: Maximum assertion !(1.5 <= 1).",
                null,
            },
            new object[] {
                new HasEnumerable {Fs = null},
                // Empty is not allowed
                "Object.Property.(root)[\"Fs\"]: Type is not matched(Actual: Null; Expected: array).",
                null,
            },
            // nested
            new object[] {
                new HasEnumerable
                {
                    Fs = new float[] {},
                    OsList = new List<object>
                    {
                        new NotRequiredObjectWithIgnorable { X = 10 },
                    },
                },
                null,
                "{\"Fs\":[],\"FsList\":[],\"Os\":[],\"OsList\":[{\"X\":10}]}",
            },
            new object[] {
                new HasEnumerable
                {
                    Fs = new float[] {},
                    OsList = new List<object>
                    {
                        new NotRequiredObjectWithIgnorable { X = 10 },
                        new NotRequiredObjectWithIgnorable { X = -1 },
                        new NotRequiredObjectWithIgnorable { X = 20 },
                    },
                },
                null,
                "{\"Fs\":[],\"FsList\":[],\"Os\":[],\"OsList\":[{\"X\":10},{},{\"X\":20}]}",
            },
            new object[] {
                new HasEnumerable
                {
                    Fs = new float[] {},
                    OsList = new List<object>
                    {
                        new HasEnumerable
                        {
                            Fs = new float[] {},
                            OsList = new List<object>
                            {
                                new NotRequiredObjectWithIgnorable { X = 10 },
                            },
                        },
                        new HasEnumerable
                        {
                            Fs = new float[] {},
                            OsList = new List<object>
                            {
                                new NotRequiredObjectWithIgnorable { X = -1 },
                            },
                        },
                    },
                },
                null,
                "{\"Fs\":[],\"FsList\":[],\"Os\":[],\"OsList\":[{\"Fs\":[],\"FsList\":[],\"Os\":[],\"OsList\":[{\"X\":10}]},{\"Fs\":[],\"FsList\":[],\"Os\":[],\"OsList\":[{}]}]}",
            },
        };

        [Json]
        public class HasEnumerableNested
        {
            [ItemsJsonSchema]
            public HasRequiredItems[] Os;
        }

        public static object[] HasEnumerableNestedArgs = new object[] {
            new object[] {
                new HasEnumerableNested {Os = new HasRequiredItems[] { new HasRequiredItems { Xs = new int[] { 42 } }}},
                null,
                "{\"Os\":[{\"Xs\":[42]}]}",
            },
            new object[] {
                new HasEnumerableNested {Os = null},
                // Empty is not allowed
                "Object.Property.(root)[\"Os\"]: Type is not matched(Actual: Null; Expected: array).",
                null,
            },
            new object[] {
                new HasEnumerableNested
                {
                    Os = new HasRequiredItems[]
                    {
                        new HasRequiredItems { Xs = new int[] { 1 } },
                        new HasRequiredItems { Xs = new int[] { 2 } },
                        new HasRequiredItems { Xs = new int[] { 3 } },
                    },
                },
                null,
                "{\"Os\":[{\"Xs\":[1]},{\"Xs\":[2]},{\"Xs\":[3]}]}",
            },
            new object[] {
                new HasEnumerableNested
                {
                    Os = new HasRequiredItems[]
                    {
                        new HasRequiredItems { Xs = new int[] { 1 } },
                        new HasRequiredItems { Xs = new int[] { -1 } },
                        new HasRequiredItems { Xs = new int[] { 3 } },
                    },
                },
                "Object.Property.Array.Items.Object.Property.Array.Items.Number.(root)[\"Os\"][1][\"Xs\"][0]: Minimum assertion !(-1 >= 0).",
                null,
            },
        };

        public class HasRequiredItems
        {
            [JsonSchema(MinItems = 1)]
            [ItemsJsonSchema(Minimum = 0)]
            [JsonSchemaRequired]
            public int[] Xs;
        }

        public static object[] HasRequiredItemsArgs = new object[] {
            new object[] {
                new HasRequiredItems(),
                "Object.Property.(root)[\"Xs\"]: Type is not matched(Actual: Null; Expected: array).",
                null,
            },
            new object[] {
                new HasRequiredItems {Xs = new int[] {}},
                "Object.Property.Array.(root)[\"Xs\"]: MinItems assertion !(0 >= 1).",
                null,
            },
            new object[] {
                new HasRequiredItems {Xs = new int[] {-1}},
                "Object.Property.Array.Items.Number.(root)[\"Xs\"][0]: Minimum assertion !(-1 >= 0).",
                null,
            },
            new object[] {
                new HasRequiredItems {Xs =  new int[] {0}},
                null,
                "{\"Xs\":[0]}",
            },
        };

        public class HasRequiredString
        {
            [JsonSchemaRequired]
            public string S;
        }

        public static object[] HasRequiredStringArgs = new object[] {
            new object[] {
                new HasRequiredString(),
                "Object.Property.(root)[\"S\"]: Type is not matched(Actual: Null; Expected: string).",
                null,
            },
            new object[] {
                new HasRequiredString {S = ""},
                null,
                "{\"S\":\"\"}",
            },
        };

        public class HasRequiredButIgnorableString
        {
            [JsonSchemaRequired]
            [JsonFieldIgnorable]
            public string S;
        }

        public static object[] HasRequiredButIgnorableStringArgs = new object[] {
            new object[] {
                new HasRequiredButIgnorableString(),
                "Object.(root): Lack of required fields(Actual: []; Expected: [S]).",
                null,
            },
            new object[] {
                new HasRequiredButIgnorableString {S = ""},
                null,
                "{\"S\":\"\"}",
            },
        };

        public class HasDeps
        {
            [JsonSchema(Minimum = 0)]
            [JsonFieldIgnorable(WhenValueIs = -1)]
            public int X;

            [JsonSchemaDependencies(new string[] { "X" })]
            public int Y;
        }

        public static object[] HasDepsArgs = new object[] {
            new object[] {
                new HasDeps(),
                null,
                "{\"X\":0,\"Y\":0}",
            },
            new object[] {
                new HasDeps {X = -1},
                "Object.(root): Dependencies assertion. Lack of depended fields for Y(Actual: []; Expected: [X]).",
                null,
            },
        };

        public class HasNestedChild
        {
            [JsonSchema(MinLength = 1)]
            public string X;
        }

        public class HasNested
        {
            [JsonSchemaRequired]
            public HasNestedChild C;
        }

        public static object[] HasNestedArgs = new object[] {
            new object[] {
                new HasNested(),
                "Object.Property.(root)[\"C\"]: Type is not matched(Actual: Null; Expected: object).",
                null,
            },
            new object[] {
                new HasNested() {
                    C = new HasNestedChild(),
                },
                "Object.Property.Object.Property.(root)[\"C\"][\"X\"]: Type is not matched(Actual: Null; Expected: string).",
                null,
            },
            new object[] {
                new HasNested() {
                    C = new HasNestedChild {
                        X = "",
                    },
                },
                "Object.Property.Object.Property.String.(root)[\"C\"][\"X\"]: MinLength assertion !(0 >= 1).",
                null,
            },
        };

        public class DerivingBase
        {
            [JsonSchema(MinLength = 1)]
            public string X;
        }

        public class Deriving : DerivingBase
        {
            [JsonSchema(MinLength = 2)]
            public string Y;
        }

        public class MoreDeriving : Deriving
        {
            [JsonSchema(MinLength = 3)]
            public string Z;
        }

        public static object[] DerivingArgs = new object[] {
            new object[] {
                new Deriving(),
                "(root): AllOf[0] is failed..Object.Property.(root)[\"X\"]: Type is not matched(Actual: Null; Expected: string).",
                null,
            },
            new object[] {
                new Deriving() {
                    Y = "abc",
                },
                "(root): AllOf[0] is failed..Object.Property.(root)[\"X\"]: Type is not matched(Actual: Null; Expected: string).",
                null,
            },
            new object[] {
                new Deriving() {
                    X = "abc",
                },
                "Object.Property.(root)[\"Y\"]: Type is not matched(Actual: Null; Expected: string).",
                null,
            },
            new object[] {
                new Deriving() {
                    X = "",
                    Y = "",
                },
                "(root): AllOf[0] is failed..Object.Property.String.(root)[\"X\"]: MinLength assertion !(0 >= 1).",
                null,
            },
            new object[] {
                new Deriving() {
                    X = "a",
                    Y = "",
                },
                "Object.Property.String.(root)[\"Y\"]: MinLength assertion !(0 >= 2).",
                null,
            },
            new object[] {
                new MoreDeriving() {
                    X = "a",
                    Y = "",
                },
                "(root): AllOf[0] is failed..Object.Property.String.(root)[\"Y\"]: MinLength assertion !(0 >= 2).",
                null,
            },
            new object[] {
                new MoreDeriving() {
                    X = "a",
                    Y = "ab",
                    Z = "",
                },
                "Object.Property.String.(root)[\"Z\"]: MinLength assertion !(0 >= 3).",
                null,
            },
        };

        public class HasNullable
        {
            public int? X;
        }

        // Types are Nullable<T> BUT null will not be allowed.
        // The reason is that classes are nullable but null values are not allowed. As same as it.
        public static object[] HasNullableArgs = new object[] {
            new object[] {
                new HasNullable(),
                "Object.Property.(root)[\"X\"]: Type is not matched(Actual: Null; Expected: integer).",
                null,
            },
            new object[] {
                new HasNullable() {
                    X = 10,
                },
                null,
                "{\"X\":10}",
            },
        };

        public class HasStrEnum
        {
            public VJson.UnitTests.EnumAsString E;
        }

        public class HasNumEnum
        {
            public VJson.UnitTests.EnumAsInt E;
        }

        // Types are Nullable<T> BUT null will not be allowed.
        // The reason is that classes are nullable but null values are not allowed. As same as it.
        public static object[] HasEnumArgs = new object[] {
            new object[] {
                new HasStrEnum(), // A default value is used.
                null,
                "{\"E\":\"NameA\"}",
            },
            new object[] {
                new HasStrEnum() {
                    E = VJson.UnitTests.EnumAsString.NameC,
                },
                null/*"Object.Property.(root)[\"E\"]: Enum is not matched."*/,
                "{\"E\":\"OtherName\"}",
            },
            new object[] {
                new HasNumEnum(), // A default value is used.
                null,
                "{\"E\":0}",
            },
            new object[] {
                new HasNumEnum() {
                    E = VJson.UnitTests.EnumAsInt.C,
                },
                null,
                "{\"E\":100}",
            },
        };

        [JsonSchema(Minimum = 0)]
        public class CustomTag<T> : RefTag<T> where T : struct
        {
        }

        public class HasCustomTagInt
        {
            [JsonSchemaRef(typeof(CustomTag<int>))]
            public int X = 0; // Int but has constraints that "Minimum = 0" defined by CustomTag
        }

        public class HasCustomTagArray
        {
            [ItemsJsonSchemaRef(typeof(CustomTag<int>))]
            public int[] Xs = new int[] { };
        }

        public class HasCustomTagDict
        {
            [JsonSchemaRef(typeof(CustomTag<int>), InfluenceRange.AdditionalProperties)]
            public Dictionary<string, int> Xs = new Dictionary<string, int>();
        }

        public class HasCustomTagArrayDict
        {
            [ItemsJsonSchemaRef(typeof(CustomTag<int>), InfluenceRange.AdditionalProperties)]
            public List<Dictionary<string, int>> Xs = new List<Dictionary<string, int>>();
        }

        // TODO
        //public class HasCustomTagDictArray
        //{
        //    [JsonSchemaRef(typeof(CustomTag<int>), InfluenceRange.AdditionalProperties)]
        //    public Dictionary<string, List<int>> Xs = new Dictionary<string, List<int>>();
        //}

        public static object[] HasCustomTagArgs = new object[] {
            new object[] {
                new HasCustomTagInt(),
                null,
                "{\"X\":0}",
            },
            new object[] {
                new HasCustomTagInt {
                    X = -1,
                },
                "Object.Property.(root)[\"X\"]: AllOf[0] is failed..Number.(root)[\"X\"]: Minimum assertion !(-1 >= 0).",
                null,
            },
            new object[] {
                new HasCustomTagArray(),
                null,
                "{\"Xs\":[]}",
            },
            new object[] {
                new HasCustomTagArray {
                    Xs = new int[] { -1 },
                },
                "Object.Property.Array.Items.(root)[\"Xs\"][0]: AllOf[0] is failed..Number.(root)[\"Xs\"][0]: Minimum assertion !(-1 >= 0).",
                null,
            },
            new object[] {
                new HasCustomTagDict(),
                null,
                "{\"Xs\":{}}",
            },
            new object[] {
                new HasCustomTagDict {
                    Xs = new Dictionary<string, int>
                    {
                        {"a", -1 },
                    },
                },
                "Object.Property.Object.AdditionalProperties.(root)[\"Xs\"][\"a\"]: AllOf[0] is failed..Number.(root)[\"Xs\"][\"a\"]: Minimum assertion !(-1 >= 0).",
                null,
            },
            new object[] {
                new HasCustomTagArrayDict(),
                null,
                "{\"Xs\":[]}",
            },
            new object[] {
                new HasCustomTagArrayDict {
                    Xs = new List<Dictionary<string, int>>
                    {
                        new Dictionary<string, int>{
                            {"a", -1 },
                        },
                    },
                },
                "Object.Property.Array.Items.Object.AdditionalProperties.(root)[\"Xs\"][0][\"a\"]: AllOf[0] is failed..Number.(root)[\"Xs\"][0][\"a\"]: Minimum assertion !(-1 >= 0).",
                null,
            },
        };

        public static object[] EnumArgs = new object[] {
            new object[] {
                typeof(VJson.UnitTests.EnumAsInt),
                VJson.UnitTests.EnumAsInt.A,
                null,
                "0",
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsInt),
                8888,
                "(root): Enum is not matched.",
                null,
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsInt),
                "???",
                "(root): Type is not matched(Actual: String; Expected: integer).",
                null,
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsString),
                "NameA", /*VJson.UnitTests.EnumAsString.NameA,*/
                null,
                "\"NameA\"",
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsString),
                VJson.UnitTests.EnumAsString.NameA,
                null,
                "\"NameA\"",
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsString),
                "???",
                "(root): Enum is not matched.",
                null,
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsString),
                8888,
                "(root): Type is not matched(Actual: Integer; Expected: string).",
                null,
            },
        };
    }
}
