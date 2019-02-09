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
    [Entity(Title="Person")]
    class Person
    {
        [Field(Description="The person's first name.")]
        public string firstName;

        [Field(Description="The person's last name.")]
        public string lastName;

        [Field(Description="Age in years which must be equal to or greater than zero.")]
        [NumericValidation(Minimum = 0)]
        public int age;
    }

    public class JsonSchemaFromClassTests
    {
        [Test]
        public void CreateFromPersonTest()
        {
            var schema = JsonSchema.CreateFromClass<Person>();
            Assert.IsNotNull(schema);

            Assert.AreEqual("Person", schema.Entity.Title);
            Assert.AreEqual(null, schema.Entity.Description);

            Assert.AreEqual(NodeKind.Object, schema.Kind);

            var validator = schema.Validator as ObjectValidator;
            Assert.IsNotNull(validator);

            Assert.That( new Dictionary<string, JsonSchema>{
                    {"firstName", new JsonSchema
                        {
                            Entity = new Entity
                            {
                                Description = "The person's first name.",
                            },
                            Kind = NodeKind.String,
                        }
                    },
                    {"lastName", new JsonSchema
                        {
                            Entity = new Entity
                            {
                                Description = "The person's last name.",
                            },
                            Kind = NodeKind.String,
                        }
                    },
                    {"age", new JsonSchema
                        {
                            Entity = new Entity
                            {
                                Description = "Age in years which must be equal to or greater than zero.",
                            },
                            Kind = NodeKind.Integer,
                            Validator = new NumericValidator(new NumericValidation {
                                    Minimum = 0,
                                }),
                        }
                    },
                }, Is.EquivalentTo(validator.Props));
        }

        class TestCase
        {
            public string description;
            public object schema;
            public Test[] tests;
        }

        class Test
        {
            public string description;
            public object data;
            public bool valid;
        }

        public class JsonSchemaFromTestCasesTests
        {
            [Test]
            public void ValidationTest()
            {
                var path = Path.Combine("JSON-Schema-Test-Suite", "tests", "draft7", "minimum.json");
                using(var sr = new StreamReader(path)) {
                    var d = new JsonSerializer(typeof(TestCase[]));
                    var cases = (TestCase[])d.Deserialize(sr);

                    foreach(var c in cases) {
                        Console.WriteLine("c: " + c.description);
                        foreach(var t in c.tests) {
                            Console.WriteLine("  t: " + t.description);
                            Console.WriteLine("  d: " + t.data);
                            Console.WriteLine("  v: " + t.valid);
                        }
                    }

                    Assert.AreEqual(0, 1);
                }
            }
        }
    }
}
