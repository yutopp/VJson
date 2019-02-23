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
            var schema = JsonSchema.CreateFromClass<Person>();
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

    public class JsonSchemaFormatTests
    {
        [Test]
        [TestCaseSource("SchemaStringArgs")]
        public void SchemaFormatTest(Type ty, string expected)
        {
            var schema = JsonSchema.CreateFromType(ty);
            Assert.AreEqual(expected, schema.ToString());
        }

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
                "{\"items\":{},\"type\":\"array\"}",
            },
            new object[] {
                typeof(int[]),
                "{\"items\":{\"type\":\"integer\"},\"type\":\"array\"}",
            },
            new object[] {
                typeof(float[]),
                "{\"items\":{\"type\":\"number\"},\"type\":\"array\"}",
            },
            new object[] {
                typeof(List<object>),
                "{\"items\":{},\"type\":\"array\"}",
            },
            new object[] {
                typeof(List<int>),
                "{\"items\":{\"type\":\"integer\"},\"type\":\"array\"}",
            },
            new object[] {
                typeof(List<float>),
                "{\"items\":{\"type\":\"number\"},\"type\":\"array\"}",
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
                "{\"properties\":{},\"type\":\"object\"}",
            },
            new object[] {
                typeof(JsonSchemaFormatTests[]),
                "{\"items\":{\"$ref\":\"VJson.Schema.UnitTests.JsonSchemaFormatTests\"},\"type\":\"array\"}",
            },
            new object[] {
                typeof(List<JsonSchemaFormatTests>),
                "{\"items\":{\"$ref\":\"VJson.Schema.UnitTests.JsonSchemaFormatTests\"},\"type\":\"array\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.NotRequiredObject),
                "{\"$id\":\"not_required_object.json\",\"properties\":{\"X\":{\"minimum\":1,\"type\":\"integer\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.NotRequiredObjectWithIgnorable),
                "{\"properties\":{\"X\":{\"minimum\":1,\"type\":\"integer\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasDictionary),
                "{\"properties\":{\"FP\":{\"type\":\"object\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasEnumerable),
                "{\"properties\":{\"Fs\":{\"items\":{\"maximum\":1,\"minimum\":0,\"type\":\"number\"},\"type\":\"array\"},\"FsList\":{\"items\":{\"type\":\"number\"},\"type\":\"array\"},\"Os\":{\"type\":\"array\"},\"OsList\":{\"type\":\"array\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasRequiredItems),
                "{\"properties\":{\"Xs\":{\"items\":{\"minimum\":0,\"type\":\"integer\"},\"minItems\":1,\"type\":\"array\"}},\"required\":[\"Xs\"],\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasRequiredString),
                "{\"properties\":{\"S\":{\"type\":\"string\"}},\"required\":[\"S\"],\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasRequiredButIgnorableString),
                "{\"properties\":{\"S\":{\"type\":\"string\"}},\"required\":[\"S\"],\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasDeps),
                "{\"dependencies\":{\"Y\":[\"X\"]},\"properties\":{\"X\":{\"minimum\":0,\"type\":\"integer\"},\"Y\":{\"type\":\"integer\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.HasNested),
                "{\"properties\":{\"C\":{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+HasNestedChild\"}},\"required\":[\"C\"],\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.DerivingBase),
                "{\"properties\":{\"X\":{\"minLength\":1,\"type\":\"string\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.Deriving),
                "{\"allOf\":[{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+DerivingBase\"}],\"properties\":{\"X\":{},\"Y\":{\"minLength\":2,\"type\":\"string\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(ValidatorWithSerializerTests.MoreDeriving),
                "{\"allOf\":[{\"$ref\":\"VJson.Schema.UnitTests.ValidatorWithSerializerTests+Deriving\"}],\"properties\":{\"X\":{},\"Y\":{},\"Z\":{\"minLength\":3,\"type\":\"string\"}},\"type\":\"object\"}",
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsInt),
                "{\"enum\":[0,1,100],\"type\":\"integer\"}",
            },
            new object[] {
                typeof(VJson.UnitTests.EnumAsString),
                "{\"enum\":[\"NameA\",\"NameB\",\"OtherName\"],\"type\":\"string\"}",
            },
        };
    }

    public class JsonSchemaFromTestCasesTests
    {
        class TestCase
        {
            public string description = default(string);
            public JsonSchema schema = default(JsonSchema);
            public UnitTest[] tests = default(UnitTest[]);
        }

        class UnitTest
        {
            public string description = default(string);
            public object data = default(object);
            public bool valid = default(bool);
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
}
