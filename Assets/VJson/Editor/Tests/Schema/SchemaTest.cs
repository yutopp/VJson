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
    [Schema(Title="Person")]
    class Person
    {
        [Schema(Description="The person's first name.")]
        public string firstName;

        [Schema(Description="The person's last name.")]
        public string lastName;

        [Schema(Description="Age in years which must be equal to or greater than zero.",
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
            [TestCase("type.json")]
            //[TestCase("multipleOf.json")]
            [TestCase("minimum.json")]
            [TestCase("exclusiveMinimum.json")]
            [TestCase("maximum.json")]
            [TestCase("exclusiveMaximum.json")]
            [TestCase("items.json")]
            public void ValidationTest(string casePath)
            {
                var path = Path.Combine("JSON-Schema-Test-Suite", "tests", "draft7", casePath);
                using(var sr = new StreamReader(path)) {
                    var d = new JsonSerializer(typeof(TestCase[]));
                    var cases = (TestCase[])d.Deserialize(sr);

                    foreach(var c in cases) {
                        Console.WriteLine("c: " + c.description);
                        Console.WriteLine("s: " + c.schema);

                        foreach(var t in c.tests) {
                            Console.WriteLine("  t: " + t.description);
                            Console.WriteLine("  d: " + t.data);
                            Console.WriteLine("  v: " + t.valid);

                            var result = c.schema.Validate(t.data);
                            Console.WriteLine("  r: " + result);

                            Assert.That(result, Is.EqualTo(t.valid), t.description);
                        }
                    }
                }
            }
        }
    }
}
