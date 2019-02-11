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
    //
    // http://json-schema.org/learn/miscellaneous-examples.html
    //
    [JsonSchema(Title="Person")]
    class Person
    {
        [JsonSchema(Description="The person's first name.")]
        public string firstName;

        [JsonSchema(Description="The person's last name.")]
        public string lastName;

        [JsonSchema(Description="Age in years which must be equal to or greater than zero.",
                    Minimum=0)]
        public int age;
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
                        Is.EqualTo( new JsonSchema
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

        class TestCase
        {
            public string description;
            public JsonSchema schema;
            public UnitTest[] tests;
        }

        class UnitTest
        {
            public string description;
            public object data;
            public bool valid;
        }

        public class JsonSchemaFromTestCasesTests
        {
            [Test]
            [TestCase("additionalItems.json")]
            [TestCase("additionalProperties.json")]
            //[TestCase("allOf.json")]
            //[TestCase("anyOf.json")]
            [TestCase("boolean_schema.json")]
            //[TestCase("const.json")]
            //[TestCase("contains.json")]
            //[TestCase("default.json")]
            //[TestCase("definitions.json")]
            //[TestCase("dependencies.json")]
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
            //[TestCase("multipleOf.json")]
            [TestCase("not.json")]
            //[TestCase("oneOf.json")]
            //[TestCase("optional")]
            [TestCase("pattern.json")]
            [TestCase("patternProperties.json")]
            [TestCase("properties.json")]
            //[TestCase("propertyNames.json")]
            //[TestCase("ref.json")]
            //[TestCase("refRemote.json")]
            [TestCase("required.json")]
            [TestCase("type.json")]
            //[TestCase("uniqueItems.json")]
            public void ValidationTest(string casePath)
            {
                var path = Path.Combine(Path.Combine(Path.Combine("JSON-Schema-Test-Suite", "tests"), "draft7"), casePath);
                using(var sr = new StreamReader(path)) {
                    var d = new JsonSerializer(typeof(TestCase[]));
                    var cases = (TestCase[])d.Deserialize(sr);

                    foreach(var c in cases) {
                        foreach(var t in c.tests) {
                            var result = c.schema.Validate(t.data);

                            Assert.That(result, Is.EqualTo(t.valid),
                                        String.Format("{0} ({1})", t.description, c.description)
                                        );
                        }
                    }
                }
            }
        }
    }
}
