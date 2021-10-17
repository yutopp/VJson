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

namespace VJson.Schema.UnitTests
{
    //
    // http://json-schema.org/learn/miscellaneous-examples.html
    //
    [JsonSchema(Title = "Person")]
    class Person
    {
        [JsonSchema(Description = "The person's first name.")]
        public string firstName = default(string);

        [JsonSchema(Description = "The person's last name.")]
        public string lastName = default(string);

        [JsonSchema(Description = "Age in years which must be equal to or greater than zero.",
                    Minimum = 0)]
        public int age = default(int);
    }

    public class JsonSchemaFromClassTests
    {
        [Test]
        public void CreateFromPersonTest()
        {
            var schema = JsonSchema.CreateFromType<Person>();
            Assert.IsNotNull(schema);

            Assert.AreEqual("Person", schema.Title);
            Assert.AreEqual(null, schema.Description);

            //Assert.AreEqual(NodeKind.Object, schema.Kind);

            Assert.That(schema.Properties.Count, Is.EqualTo(3));

            Assert.That(schema.Properties["firstName"],
                        Is.EqualTo(new JsonSchema
                        {
                            Description = "The person's first name.",
                            Type = "string",
                        })
                        );
            Assert.That(schema.Properties["lastName"],
                        Is.EqualTo(new JsonSchema
                        {
                            Description = "The person's last name.",
                            Type = "string",
                        })
                        );
            Assert.That(schema.Properties["age"],
                        Is.EqualTo(new JsonSchema
                        {
                            Description = "Age in years which must be equal to or greater than zero.",
                            Type = "integer",
                            Minimum = 0,
                        })
                        );
        }
    }

    [JsonSchema(Title = "a", Description = "b")]
    public class JsonSchemaFormatTests
    {
        [Test]
        [TestCaseSource(nameof(SchemaStringArgs))]
        public void SchemaFormatTest(Type ty, string expected)
        {
            var schema = JsonSchema.CreateFromType(ty);
            Assert.AreEqual(expected, schema.ToString());
        }

        public abstract class Gb2 {}

        [JsonSchema(Id = "g2")]
        public sealed class G2 : Gb2 {}

        public static object[] SchemaStringArgs = new object[] {
            new object[] {
                typeof(int),
                "{\"type\":\"integer\"}",
            },
            new object[] {
                typeof(double),
                "{\"type\":\"number\"}",
            },
            new object[] {
                typeof(bool),
                "{\"type\":\"boolean\"}",
            },
            new object[] {
                typeof(string),
                "{\"type\":\"string\"}",
            },
            new object[] {
                typeof(object[]),
                "{\"type\":\"array\",\"items\":{}}",
            },
            new object[] {
                typeof(int[]),
                "{\"type\":\"array\",\"items\":{\"type\":\"integer\"}}",
            },
            new object[] {
                typeof(float[]),
                "{\"type\":\"array\",\"items\":{\"type\":\"number\"}}",
            },
            new object[] {
                typeof(List<object>),
                "{\"type\":\"array\",\"items\":{}}",
            },
            new object[] {
                typeof(List<int>),
                "{\"type\":\"array\",\"items\":{\"type\":\"integer\"}}",
            },
            new object[] {
                typeof(List<float>),
                "{\"type\":\"array\",\"items\":{\"type\":\"number\"}}",
            },
            new object[] {
                typeof(Dictionary<string, string>),
                "{\"type\":\"object\"}",
            },
            new object[] {
                typeof(object),
                "{}",
            },
            new object[] {
                typeof(JsonSchemaFormatTests),
                "{\"title\":\"a\",\"description\":\"b\",\"type\":\"object\",\"properties\":{}}",
            },
            new object[] {
                typeof(JsonSchemaFormatTests[]),
                "{\"type\":\"array\",\"items\":{\"$ref\":\"VJson.Schema.UnitTests.JsonSchemaFormatTests\"}}",
            },
            new object[] {
                typeof(List<JsonSchemaFormatTests>),
                "{\"type\":\"array\",\"items\":{\"$ref\":\"VJson.Schema.UnitTests.JsonSchemaFormatTests\"}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.NotRequiredObject),
                "{\"$id\":\"not_required_object.json\",\"type\":\"object\",\"properties\":{\"X\":{\"type\":\"integer\",\"minimum\":1}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.NotRequiredObjectWithIgnorable),
                "{\"type\":\"object\",\"properties\":{\"X\":{\"type\":\"integer\",\"minimum\":1}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasDictionary),
                "{\"type\":\"object\",\"properties\":{\"FP\":{\"type\":\"object\"}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasEnumerable),
                "{\"type\":\"object\",\"properties\":{\"Fs\":{\"type\":\"array\",\"items\":{\"type\":\"number\",\"maximum\":1,\"minimum\":0}},\"FsList\":{\"type\":\"array\",\"items\":{\"type\":\"number\"}},\"Os\":{\"type\":\"array\"},\"OsList\":{\"type\":\"array\"}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasRequiredItems),
                "{\"type\":\"object\",\"required\":[\"Xs\"],\"properties\":{\"Xs\":{\"type\":\"array\",\"items\":{\"type\":\"integer\",\"minimum\":0},\"minItems\":1}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasRequiredString),
                "{\"type\":\"object\",\"required\":[\"S\"],\"properties\":{\"S\":{\"type\":\"string\"}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasRequiredButIgnorableString),
                "{\"type\":\"object\",\"required\":[\"S\"],\"properties\":{\"S\":{\"type\":\"string\"}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasDeps),
                "{\"type\":\"object\",\"properties\":{\"X\":{\"type\":\"integer\",\"minimum\":0},\"Y\":{\"type\":\"integer\"}},\"dependencies\":{\"Y\":[\"X\"]}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasNested),
                "{\"type\":\"object\",\"required\":[\"C\"],\"properties\":{\"C\":{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+HasNestedChild\"}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.DerivingBase),
                "{\"type\":\"object\",\"properties\":{\"X\":{\"type\":\"string\",\"minLength\":1}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.Deriving),
                "{\"type\":\"object\",\"properties\":{\"X\":{},\"Y\":{\"type\":\"string\",\"minLength\":2}},\"allOf\":[{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+DerivingBase\"}]}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.MoreDeriving),
                "{\"type\":\"object\",\"properties\":{\"X\":{},\"Y\":{},\"Z\":{\"type\":\"string\",\"minLength\":3}},\"allOf\":[{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+Deriving\"}]}",
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsInt),
                "{\"type\":\"integer\",\"enum\":[0,1,100]}",
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsString),
                "{\"type\":\"string\",\"enum\":[\"NameA\",\"NameB\",\"OtherName\"]}",
            },
            new object[] {
                typeof(Nullable<int>),
                "{\"type\":\"integer\"}",
            },
            new object[] {
                typeof(int?),
                "{\"type\":\"integer\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasNullable),
                "{\"type\":\"object\",\"properties\":{\"X\":{\"type\":\"integer\"}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasStrEnum),
                "{\"type\":\"object\",\"properties\":{\"E\":{\"type\":\"string\",\"enum\":[\"NameA\",\"NameB\",\"OtherName\"]}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasNumEnum),
                "{\"type\":\"object\",\"properties\":{\"E\":{\"type\":\"integer\",\"enum\":[0,1,100]}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.CustomTag<int>),
                "{\"type\":\"integer\",\"minimum\":0}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasCustomTagInt),
                "{\"type\":\"object\",\"properties\":{\"X\":{\"type\":\"integer\",\"allOf\":[{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+CustomTag`1[System.Int32]\"}]}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasCustomTagArray),
                "{\"type\":\"object\",\"properties\":{\"Xs\":{\"type\":\"array\",\"items\":{\"type\":\"integer\",\"allOf\":[{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+CustomTag`1[System.Int32]\"}]}}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasCustomTagDict),
                "{\"type\":\"object\",\"properties\":{\"Xs\":{\"type\":\"object\",\"additionalProperties\":{\"allOf\":[{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+CustomTag`1[System.Int32]\"}]}}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasCustomTagArrayDict),
                "{\"type\":\"object\",\"properties\":{\"Xs\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"additionalProperties\":{\"allOf\":[{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+CustomTag`1[System.Int32]\"}]}}}}}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.CustomTag<float>),
                "{\"type\":\"number\",\"minimum\":0}",
            },
            // ↓ Run same test cases twice, it will be broken on IL2CPP. Thus added as test cases.
            // See: https://github.com/yutopp/VJson/blob/120020a1c90dd1f0275d0f9353eef4e089943b0a/Packages/net.yutopp.vjson/Runtime/Schema/JsonSchema.cs#L385
            new object[] {
                typeof(G2),
                @"{""$id"":""g2"",""type"":""object"",""properties"":{},""allOf"":[{""$ref"":""VJson.Schema.UnitTests.JsonSchemaFormatTests+Gb2""}]}",
            },
            new object[] {
                typeof(G2),
                @"{""$id"":""g2"",""type"":""object"",""properties"":{},""allOf"":[{""$ref"":""VJson.Schema.UnitTests.JsonSchemaFormatTests+Gb2""}]}",
            },
            // ↑
        };
    }

#if VJSON_FULL_TESTS
    public class JsonSchemaFromTestCasesTests
    {
        [Json]
        sealed class TestCase
        {
            [JsonField] public string description = default;
            [JsonField] public JsonSchema schema = default;
            [JsonField] public UnitTest[] tests = default;
        }

        [Json]
        sealed class UnitTest
        {
            [JsonField] public string description = default;
            [JsonField] public object data = default;
            [JsonField] public bool valid = default;
        }

        [Test]
        [TestCase("additionalItems.json")]
        [TestCase("additionalProperties.json")]
        [TestCase("allOf.json")]
        [TestCase("anyOf.json")]
        [TestCase("boolean_schema.json")]
        //[TestCase("const.json")]
        //[TestCase("contains.json")]
        //[TestCase("default.json")]
        //[TestCase("definitions.json")]
        [TestCase("dependencies.json")]
        [TestCase("enum.json")]
        [TestCase("exclusiveMaximum.json")]
        [TestCase("exclusiveMinimum.json")]
        //[TestCase("if-then-else.json")]
        [TestCase("items.json")]
        [TestCase("maximum.json")]
        [TestCase("maxItems.json")]
        [TestCase("maxLength.json")]
        [TestCase("maxProperties.json")]
        [TestCase("minimum.json")]
        [TestCase("minItems.json")]
        [TestCase("minLength.json")]
        [TestCase("minProperties.json")]
        [TestCase("multipleOf.json")]
        [TestCase("not.json")]
        [TestCase("oneOf.json")]
        //[TestCase("optional")]
        [TestCase("pattern.json")]
        [TestCase("patternProperties.json")]
        [TestCase("properties.json")]
        //[TestCase("propertyNames.json")]
        //[TestCase("ref.json")]
        //[TestCase("refRemote.json")]
        [TestCase("required.json")]
        [TestCase("type.json")]
        [TestCase("uniqueItems.json")]
        public void ValidationTest(string casePath)
        {
            var path = Path.Combine(Path.Combine(Path.Combine("JSON-Schema-Test-Suite", "tests"), "draft7"), casePath);
            using (var fs = File.OpenRead(path))
            {
                var d = new JsonSerializer(typeof(TestCase[]));
                var cases = (TestCase[])d.Deserialize(fs);

                foreach (var c in cases)
                {
                    foreach (var t in c.tests)
                    {
                        var ex = c.schema.Validate(t.data);

                        Assert.That(ex == null,
                                    Is.EqualTo(t.valid),
                                    String.Format("{0} / {1} ({2})", ex, t.description, c.description));
                    }
                }
            }
        }
    }
#endif
}
