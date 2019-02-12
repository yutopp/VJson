//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace VJson.Schema
{
    [Json(ImplicitConstructable=true)]
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Field,
                    Inherited = false)]
    public class JsonSchema : Attribute
    {
        #region Core
        [JsonField(Name="$schema")]
        [JsonFieldIgnorable]
        public string Schema;

        [JsonField(Name="$id")]
        [JsonFieldIgnorable]
        public string Id;

        [JsonField(Name="$ref")]
        [JsonFieldIgnorable]
        public string Ref;
        #endregion

        #region Metadata
        [JsonFieldIgnorable]
        public string Title;

        [JsonFieldIgnorable]
        public string Description;
        #endregion

        #region 6.1: Any instances
        [JsonField(Name="type", TypeHints = new Type[] {typeof(string), typeof(string[])})]
        [JsonFieldIgnorable]
        public object Type;

        [JsonField(Name="enum")]
        [JsonFieldIgnorable]
        public object[] Enum;

        [JsonField(Name="const")]
        [JsonFieldIgnorable]
        public object Const;

        bool EqualsOnlyAny(JsonSchema rhs) {
            return EqualsSingletonOrArray<string>(Type, rhs.Type)
                && EqualsEnumerable(Enum, rhs.Enum)
                && Object.Equals(Const, rhs.Const)
                ;
        }
        #endregion

        #region 6.2: Numeric instances
        [JsonField(Name="multipleOf")]
        [JsonFieldIgnorable(WhenValueIs = double.MinValue)]
        public double MultipleOf = double.MinValue;

        [JsonField(Name="maximum")]
        [JsonFieldIgnorable(WhenValueIs = double.MinValue)]
        public double Maximum = double.MinValue;

        [JsonField(Name="exclusiveMaximum")]
        [JsonFieldIgnorable(WhenValueIs = double.MinValue)]
        public double ExclusiveMaximum = double.MinValue;

        [JsonField(Name="minimum")]
        [JsonFieldIgnorable(WhenValueIs = double.MaxValue)]
        public double Minimum = double.MaxValue;

        [JsonField(Name="exclusiveMinimum")]
        [JsonFieldIgnorable(WhenValueIs = double.MaxValue)]
        public double ExclusiveMinimum = double.MaxValue;

        bool EqualsOnlyNum(JsonSchema rhs) {
            return MultipleOf == rhs.MultipleOf
                && Maximum == rhs.Maximum
                && ExclusiveMaximum == rhs.ExclusiveMaximum
                && Minimum == rhs.Minimum
                && ExclusiveMinimum == rhs.ExclusiveMinimum
                ;
        }
        #endregion

        #region 6.3. Strings
        [JsonField(Name="maxLength")]
        [JsonFieldIgnorable(WhenValueIs = int.MinValue)]
        public int MaxLength = int.MinValue;

        [JsonField(Name="minLength")]
        [JsonFieldIgnorable(WhenValueIs = int.MaxValue)]
        public int MinLength = int.MaxValue;

        [JsonField(Name="pattern")]
        [JsonFieldIgnorable]
        public string Pattern;

        bool EqualsOnlyString(JsonSchema rhs) {
            return MaxLength == rhs.MaxLength
                && MinLength == rhs.MinLength
                && Object.Equals(Pattern, rhs.Pattern)
                ;
        }
        #endregion

        #region 6.4: Arrays
        [JsonField(Name="items", TypeHints=new Type[] {typeof(JsonSchema), typeof(JsonSchema[])})]
        [JsonFieldIgnorable]
        public object Items;

        [JsonField(Name="additionalItems")]
        [JsonFieldIgnorable]
        public JsonSchema AdditionalItems;

        [JsonField(Name="maxItems")]
        [JsonFieldIgnorable(WhenValueIs = int.MinValue)]
        public int MaxItems = int.MinValue;

        [JsonField(Name="minItems")]
        [JsonFieldIgnorable(WhenValueIs = int.MaxValue)]
        public int MinItems = int.MaxValue;

        // uniqueItems
        // contains

        bool EqualsOnlyArray(JsonSchema rhs) {
            return EqualsSingletonOrArray<JsonSchema>(Items, rhs.Items)
                && Object.Equals(AdditionalItems, rhs.AdditionalItems)
                && MaxItems == rhs.MaxItems
                && MinItems == rhs.MinItems
                ;
        }
        #endregion

        #region 6.5: Objects
        [JsonField(Name="maxProperties")]
        [JsonFieldIgnorable(WhenValueIs = int.MinValue)]
        public int MaxProperties = int.MinValue;

        [JsonField(Name="minProperties")]
        [JsonFieldIgnorable(WhenValueIs = int.MaxValue)]
        public int MinProperties = int.MaxValue;

        [JsonField(Name="required")]
        [JsonFieldIgnorable]
        public string[] Required;

        [JsonField(Name="properties")]
        [JsonFieldIgnorable]
        public Dictionary<string, JsonSchema> Properties;

        [JsonField(Name="patternProperties")]
        [JsonFieldIgnorable]
        public Dictionary<string, JsonSchema> PatternProperties;

        [JsonField(Name="additionalProperties")]
        [JsonFieldIgnorable]
        public JsonSchema AdditionalProperties;

        [JsonField(Name="dependencies",
                   /* TODO:
                      A type of this field should be Map<string, string[] | JsonSchema>.
                      But there are no ways to represent this type currently...
                    */
                   TypeHints=new Type[] {
                       typeof(Dictionary<string, string[]>),
                       typeof(Dictionary<string, JsonSchema>)
                   })]
        [JsonFieldIgnorable]
        public object Dependencies;

        // propertyNames

        bool EqualsOnlyObject(JsonSchema rhs) {
            // TODO
            return true;
        }
        #endregion

        #region 6.7: Subschemas With Boolean Logic
        // allOf
        // anyOf
        // oneOf
        [JsonField(Name="not")]
        [JsonFieldIgnorable]
        public JsonSchema Not;

        bool EqualsOnlySubBool(JsonSchema rhs) {
            return Object.Equals(Not, rhs.Not)
                ;
        }
        #endregion

        public JsonSchema()
        {
        }

        public JsonSchema(bool b) {
            if (!b) {
                // Equivalent to {"not": {}}
                Not = new JsonSchema();
            }

            // Equivalent to {}
        }

        public override bool Equals(object rhsObj) {
            var rhs = rhsObj as JsonSchema;
            if (rhs == null) {
                return false;
            }

            return Title == rhs.Title
                && Description == rhs.Description
                && EqualsOnlyAny(rhs)
                && EqualsOnlyNum(rhs)
                && EqualsOnlyString(rhs)
                && EqualsOnlyArray(rhs)
                && EqualsOnlyObject(rhs)
                && EqualsOnlySubBool(rhs)
                ;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var serializer = new JsonSerializer(typeof(JsonSchema));
            using(var textWriter = new StringWriter()) {
                serializer.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        public static JsonSchema CreateFromClass<T>(JsonSchemaRegistory reg = null, bool asRef = false)
        {
            return CreateFromType(typeof(T), reg, asRef);
        }

        public static JsonSchema CreateFromType(Type ty, JsonSchemaRegistory reg = null, bool asRef = false)
        {
            var kind = Node.KindOfType(ty);
            switch (kind) {
                case NodeKind.Boolean:
                    return new JsonSchema {
                        Type = "boolean",
                    };

                case NodeKind.Integer:
                    return new JsonSchema {
                        Type = "integer",
                    };

                case NodeKind.Float:
                    return new JsonSchema {
                        Type = "number",
                    };

                case NodeKind.String:
                    return new JsonSchema {
                        Type = "string",
                    };

                case NodeKind.Array:
                    return new JsonSchema {
                        Type = "array",
                    };

                case NodeKind.Object:
                    if (ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                        return new JsonSchema {
                            Type = "object",
                        };
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }

            if (reg == null) {
                reg = JsonSchemaRegistory.GetDefault();
            }

            var schema = (JsonSchema)Attribute.GetCustomAttribute(ty, typeof(JsonSchema));
            if (schema == null) {
                schema = new JsonSchema();
            }
            schema.Type = "object";

            var schemaId = schema.Id;
            if (schemaId == null) {
                schemaId = ty.ToString();
            }
            var refSchema = reg.Resolve(schemaId);
            if (refSchema != null) {
                schema = refSchema;
                goto skip;
            } else {
                reg.Register(schemaId, schema);
            }

            var properties = new Dictionary<string, JsonSchema>();
            var required = new List<string>();
            var dependencies = new Dictionary<string, string[]>();

            var fields = ty.GetFields(BindingFlags.Public|BindingFlags.Instance);
            foreach(var field in fields) {
                var attr = (JsonField)Attribute.GetCustomAttribute(field, typeof(JsonField), false);

                // TODO: duplication check
                var elemName = JsonField.FieldName(attr, field);

                var fieldSchema =
                    (JsonSchema)Attribute.GetCustomAttributes(field, typeof(JsonSchema))
                    .Where(f => f.GetType() == typeof(JsonSchema))
                    .FirstOrDefault();
                if (fieldSchema == null) {
                    fieldSchema = new JsonSchema();
                }

                var fieldItemsSchema =
                    (ItemsJsonSchema)Attribute.GetCustomAttribute(field, typeof(ItemsJsonSchema));
                if (fieldItemsSchema != null) {
                    fieldSchema.Items = fieldItemsSchema;
                }

                var fieldItemRequired =
                    (JsonSchemaRequired)Attribute.GetCustomAttribute(field, typeof(JsonSchemaRequired));
                if (fieldItemRequired != null) {
                    required.Add(elemName);
                }

                var fieldItemDependencies =
                    (JsonSchemaDependencies)Attribute.GetCustomAttribute(field, typeof(JsonSchemaDependencies));
                if (fieldItemDependencies != null) {
                    dependencies.Add(elemName, fieldItemDependencies.Dependencies);
                }

                var fieldTypeSchema = CreateFromType(field.FieldType, reg, true);
                if (fieldTypeSchema.Ref != null) {
                    fieldSchema = fieldTypeSchema;

                } else {
                    // Update
                    if (fieldSchema.Type == null) {
                        fieldSchema.Type = fieldTypeSchema.Type;
                    }
                }

                properties.Add(elemName, fieldSchema);
            }

            schema.Properties = properties;
            if (required.Count != 0) {
                schema.Required = required.ToArray();
            }
            if (dependencies.Count != 0) {
                schema.Dependencies = dependencies;
            }

        skip:
            if (asRef) {
                return new JsonSchema {
                    Ref = schemaId,
                };
            }
            return schema;
        }

        static bool EqualsSingletonOrArray<T>(object lhs, object rhs) where T : class {
            if (lhs == null && rhs == null) {
                return true;
            }

            if (lhs == null || rhs == null) {
                return false;
            }

            var lhsArr = lhs as T[];
            var rhsArr = rhs as T[];
            if (lhsArr != null && rhsArr != null) {
                return EqualsSingletonOrArray<T>(lhsArr, rhsArr);
            }

            var lhsSgt = lhs as T;
            var rhsAgt = rhs as T;
            return Object.Equals(lhsSgt, rhsAgt);
        }

        static bool EqualsEnumerable<E>(IEnumerable<E> lhs, IEnumerable<E> rhs)
        {
            return (lhs == null && rhs == null)
                || (lhs != null && lhs != null && lhs.SequenceEqual(rhs))
                ;
        }
    }

    [AttributeUsage(AttributeTargets.Field,
                    Inherited = false)]
    public class ItemsJsonSchema : JsonSchema
    {
    }

    public static class JsonSchemaExtensions
    {
        public static ConstraintsViolationException Validate(this JsonSchema j,
                                                             object o,
                                                             JsonSchemaRegistory reg = null)
        {
            return (new JsonSchemaValidator(j)).Validate(o, reg);
        }

        internal static ConstraintsViolationException Validate(this JsonSchema j,
                                                               object o,
                                                               JsonSchemaValidator.State state,
                                                               JsonSchemaRegistory reg)
        {
            return (new JsonSchemaValidator(j)).Validate(o, state, reg);
        }
    }
}
