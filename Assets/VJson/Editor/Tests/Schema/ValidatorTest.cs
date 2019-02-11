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
    public class ValidatorTests
    {
        [Test]
        [TestCaseSource("NotRequiredObjectArgs")]
        [TestCaseSource("NotRequiredObjectWithIgnorableArgs")]
        [TestCaseSource("HasDictionaryArgs")]
        [TestCaseSource("HasEnumerableArgs")]
        [TestCaseSource("HasRequiredItemsArgs")]
        [TestCaseSource("HasRequiredStringArgs")]
        public void ValidationTest<T>(T o, bool expected)
        {
            var schema = JsonSchema.CreateFromClass<T>();

            var r = schema.Validate(o);
            Assert.That(r, Is.EqualTo(expected),
                        String.Format("{0} : {1}", new JsonSerializer(typeof(T)).Serialize(o), schema.ToString())
                        );
        }

        public class NotRequiredObject
        {
            [JsonSchema(Minimum = 1)]
            public int X;
        }

        public static object[] NotRequiredObjectArgs = new object[] {
            new object[] {
                new NotRequiredObject {X = 0},
                false,
            },
            new object[] {
                new NotRequiredObject {X = 1},
                true,
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
                true,
            },
            new object[] {
                new NotRequiredObjectWithIgnorable {X = 0},
                false,
            },
            new object[] {
                new NotRequiredObjectWithIgnorable {X = 1},
                true,
            },
        };

        public class HasDictionary
        {
            public Dictionary<string, float> FP = new Dictionary<string, float>();
        }

        public static object[] HasDictionaryArgs = new object[] {
            new object[] {
                new HasDictionary(),
                true,
            },
            new object[] {
                new HasDictionary() {FP = null},
                false, // Empty is not allowed
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
                true,
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {-0.5f}},
                false,
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {0.5f}},
                true,
            },
            new object[] {
                new HasEnumerable {Fs = new float[] {1.5f}},
                false,
            },
            new object[] {
                new HasEnumerable {Fs = null},
                false, // Empty is not allowed
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
                false,
            },
            new object[] {
                new HasRequiredItems {Xs = new int[] {}},
                false,
            },
            new object[] {
                new HasRequiredItems {Xs = new int[] {-1}},
                false,
            },
            new object[] {
                new HasRequiredItems {Xs =  new int[] {0}},
                true,
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
                false,
            },
            new object[] {
                new HasRequiredString {S = ""},
                true,
            },
        };
    }
}
