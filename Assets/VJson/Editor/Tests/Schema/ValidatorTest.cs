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
        [TestCaseSource("NotRequiredObjectArgs")]
        [TestCaseSource("NotRequiredObjectWithIgnorableArgs")]
        [TestCaseSource("HasDictionaryArgs")]
        [TestCaseSource("HasEnumerableArgs")]
        [TestCaseSource("HasRequiredItemsArgs")]
        [TestCaseSource("HasRequiredStringArgs")]
        [TestCaseSource("HasRequiredButIgnorableStringArgs")]
        [TestCaseSource("HasDepsArgs")]
        [TestCaseSource("HasNestedArgs")]
        [TestCaseSource("DerivingArgs")]
        public void ValidationTest<T>(T o, string expectedMsg, string _expectedContent)
        {
            var schema = JsonSchema.CreateFromClass<T>();

            var ex = schema.Validate(o);

            var message =
                String.Format("{0} : {1}", new JsonSerializer(typeof(T)).Serialize(o), schema.ToString());
            Assert.AreEqual(expectedMsg, ex != null ? ex.Message : null, message);
        }

        // TODO: Move to elsewhere
        [Test]
        [TestCaseSource("NotRequiredObjectArgs")]
        [TestCaseSource("NotRequiredObjectWithIgnorableArgs")]
        [TestCaseSource("HasDictionaryArgs")]
        [TestCaseSource("HasEnumerableArgs")]
        [TestCaseSource("HasRequiredItemsArgs")]
        [TestCaseSource("HasRequiredStringArgs")]
        [TestCaseSource("HasRequiredButIgnorableStringArgs")]
        [TestCaseSource("HasDepsArgs")]
        [TestCaseSource("HasNestedArgs")]
        [TestCaseSource("DerivingArgs")]
        public void SerializationTest<T>(T o, string _expectedMsg, string expectedContent)
        {
            if (_expectedMsg != null)
            {
                return;
            }

            var content = new JsonSerializer(typeof(T)).Serialize(o);
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

        public class HasEnumerable
        {
            [ItemsJsonSchema(Minimum = 0.0, Maximum = 1.0)]
            public float[] Fs;
            public object[] Os = new object[] { };

            public List<float> FsList = new List<float>();
            public List<object> OsList = new List<object>();
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

        public static object[] DerivingArgs = new object[] {
            new object[] {
                new Deriving(),
                "(root): AllOf: Failed at 0..Object.Property.(root)[\"X\"]: Type is not matched(Actual: Null; Expected: string).",
                null,
            },
            new object[] {
                new Deriving() {
                    Y = "abc",
                },
                "(root): AllOf: Failed at 0..Object.Property.(root)[\"X\"]: Type is not matched(Actual: Null; Expected: string).",
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
                "(root): AllOf: Failed at 0..Object.Property.String.(root)[\"X\"]: MinLength assertion !(0 >= 1).",
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
        };
    }
}
