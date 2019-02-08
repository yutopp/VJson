using System.IO;
using System.Collections.Generic;
using NUnit.Framework;

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

    public class JsonSchemaFromTypeTests
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
    }
}
