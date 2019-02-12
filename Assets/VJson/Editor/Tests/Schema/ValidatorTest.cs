//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace VJson.Schema.UnitTests
{
    public class ValidatorWithSerializerTests
    {
        [Test]
        [TestCaseSource("NotRequiredObjectArgs")]
        [TestCaseSource("NotRequiredObjectWithIgnorableArgs")]
        [TestCaseSource("HasDictionaryArgs")]
        [TestCaseSource("HasEnumerableArgs")]
        [TestCaseSource("HasRequiredItemsArgs")]
        [TestCaseSource("HasRequiredStringArgs")]
        [TestCaseSource("HasRequiredButIgnorableStringArgs")]
        [TestCaseSource("HasDepsArgs")]
        public void ValidationTest<T>(T o, string expectedMsg)
        {
            var schema = JsonSchema.CreateFromClass<T>();

            var ex = schema.Validate(o);

            var message =
                String.Format("{0} : {1}", new JsonSerializer(typeof(T)).Serialize(o), schema.ToString());
            if (ex == null) {
                Assert.AreEqual(null, expectedMsg, message);
            } else {
                Assert.AreEqual(expectedMsg, ex.Diagnosis(), message);
            }
        }

        [Test]
        [TestCaseSource("NotRequiredObjectArgs")]
        [TestCaseSource("NotRequiredObjectWithIgnorableArgs")]
        [TestCaseSource("HasDictionaryArgs")]
        [TestCaseSource("HasEnumerableArgs")]
        [TestCaseSource("HasRequiredItemsArgs")]
        [TestCaseSource("HasRequiredStringArgs")]
        [TestCaseSource("HasRequiredButIgnorableStringArgs")]
        [TestCaseSource("HasDepsArgs")]
        public void SerializationTest<T>(T o, string expected)
        {
            /*
            var schema = JsonSchema.CreateFromClass<T>();

            var ex = schema.Validate(o);

            var message =
                String.Format("{0} : {1}", new JsonSerializer(typeof(T)).Serialize(o), schema.ToString());
            if (ex == null) {
                Assert.AreEqual(null, expected, message);
            } else {
                Assert.AreEqual(expected, ex.Diagnosis(), message);
            }
            */
        }

        [Test]
        [TestCaseSource("SchemaStringArgs")]
        public void SchemaFormatTest(Type ty, string expected)
        {
            var schema = JsonSchema.CreateFromType(ty);
            Assert.AreEqual(expected, schema.ToString());
        }

        public class NotRequiredObject
        {
            [JsonSchema(Minimum = 1)]
            public int X;
        }

        public static object[] NotRequiredObjectArgs = new object[] {
            new object[] {
                new NotRequiredObject {X = 0},
                "Object.Property.Number.[\"X\"]: Minimum assertion !(0 >= 1).",
            },
            new object[] {
                new NotRequiredObject {X = 1},
                null,
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
            },
            new object[] {
                new NotRequiredObjectWithIgnorable {X = 0},
                "Object.Property.Number.[\"X\"]: Minimum assertion !(0 >= 1).",
            },
            new object[] {
                new NotRequiredObjectWithIgnorable {X = 1},
                null,
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
            },
            new object[] {
                new HasDictionary() {FP = null},
                // Empty is not allowed
                "Object.Property.[\"FP\"]: Type is not matched(Actual: Null; Expected: object).",
            },
        };

        class HasEnumerable
        {
            [ItemsJsonSchema(Minimum = 0.0, Maximum = 1.0)]
            public float[] Fs;

            public object[] Os = new object[] {};

            public List<int> FsList = new List<int>();

            public List<object> OsList = new List<object>();
        }

        public static object[] HasEnumerableArgs = new object[] {
            new object[] {
                new HasEnumerable {Fs = new float[] {}},
                null,
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {-0.5f}},
                "Object.Property.Array.Items.Number.[\"Fs\"][0]: Minimum assertion !(-0.5 >= 0).",
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {0.5f}},
                null,
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {1.5f}},
                "Object.Property.Array.Items.Number.[\"Fs\"][0]: Maximum assertion !(1.5 <= 1).",
            },
            new object[] {
                new HasEnumerable {Fs = null},
                // Empty is not allowed
                "Object.Property.[\"Fs\"]: Type is not matched(Actual: Null; Expected: array).",
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
                "Object.Property.[\"Xs\"]: Type is not matched(Actual: Null; Expected: array).",
            },
            new object[] {
                new HasRequiredItems {Xs = new int[] {}},
                "Object.Property.Array.[\"Xs\"]: MinItems assertion !(0 >= 1).",
            },
            new object[] {
                new HasRequiredItems {Xs = new int[] {-1}},
                "Object.Property.Array.Items.Number.[\"Xs\"][0]: Minimum assertion !(-1 >= 0).",
            },
            new object[] {
                new HasRequiredItems {Xs =  new int[] {0}},
                null,
            },
        };

        class HasRequiredString
        {
            [JsonSchemaRequired]
            public string S;
        }

        public static object[] HasRequiredStringArgs = new object[] {
            new object[] {
                new HasRequiredString(),
                "Object.Property.[\"S\"]: Type is not matched(Actual: Null; Expected: string).",
            },
            new object[] {
                new HasRequiredString {S = ""},
                null,
            },
        };

        class HasRequiredButIgnorableString
        {
            [JsonSchemaRequired]
            [JsonFieldIgnorable]
            public string S;
        }

        public static object[] HasRequiredButIgnorableStringArgs = new object[] {
            new object[] {
                new HasRequiredButIgnorableString(),
                "Object.: Lack of required fields(Actual: []; Expected: [S]).",
            },
            new object[] {
                new HasRequiredButIgnorableString {S = ""},
                null,
            },
        };

        public class HasDeps
        {
            [JsonSchema(Minimum = 0)]
            [JsonFieldIgnorable(WhenValueIs = -1)]
            public int X;

            [JsonSchemaDependencies(new string[] {"X"})]
            public int Y;
        }

        public static object[] HasDepsArgs = new object[] {
            new object[] {
                new HasDeps(),
                null,
            },
            new object[] {
                new HasDeps {X = -1},
                "Object.: Dependencies assertion. Lack of depended fields for Y(Actual: []; Expected: [X]).",
            },
        };

        public static object[] SchemaStringArgs = new object[] {
            new object[] {
                typeof(NotRequiredObject),
                "{\"properties\":{\"X\":{\"minimum\":1,\"type\":\"integer\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(NotRequiredObjectWithIgnorable),
                "{\"properties\":{\"X\":{\"minimum\":1,\"type\":\"integer\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(HasDictionary),
                "{\"properties\":{\"FP\":{\"type\":\"object\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(HasEnumerable),
                "{\"properties\":{\"Fs\":{\"items\":{\"maximum\":1,\"minimum\":0},\"type\":\"array\"},\"FsList\":{\"type\":\"array\"},\"Os\":{\"type\":\"array\"},\"OsList\":{\"type\":\"array\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(HasRequiredItems),
                "{\"properties\":{\"Xs\":{\"items\":{\"minimum\":0},\"minItems\":1,\"type\":\"array\"}},\"required\":[\"Xs\"],\"type\":\"object\"}",
            },
            new object[] {
                typeof(HasRequiredString),
                "{\"properties\":{\"S\":{\"type\":\"string\"}},\"required\":[\"S\"],\"type\":\"object\"}",
            },
            new object[] {
                typeof(HasRequiredButIgnorableString),
                "{\"properties\":{\"S\":{\"type\":\"string\"}},\"required\":[\"S\"],\"type\":\"object\"}",
            },
            new object[] {
                typeof(HasDeps),
                "{\"dependencies\":{\"Y\":[\"X\"]},\"properties\":{\"X\":{\"minimum\":0,\"type\":\"integer\"},\"Y\":{\"type\":\"integer\"}},\"type\":\"object\"}",
            },
        };
    }
}
