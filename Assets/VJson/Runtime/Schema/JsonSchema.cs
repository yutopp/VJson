//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace VJson.Schema
{
    [Json(ImplicitConstructable = true)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false)]
    public class JsonSchemaAttribute : Attribute
    {
        #region Core
        [JsonField(Name = "$schema")]
        [JsonFieldIgnorable]
        public string Schema;

        [JsonField(Name = "$id")]
        [JsonFieldIgnorable]
        public string Id;

        [JsonField(Name = "$ref")]
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
        [JsonField(Name = "type", TypeHints = new Type[] { typeof(string), typeof(string[]) })]
        [JsonFieldIgnorable]
        public object Type;

        [JsonField(Name = "enum")]
        [JsonFieldIgnorable]
        public object[] Enum;

        [JsonField(Name = "const")]
        [JsonFieldIgnorable]
        public object Const;

        bool EqualsOnlyAny(JsonSchemaAttribute rhs)
        {
            return EqualsSingletonOrArray<string>(Type, rhs.Type)
                && EqualsEnumerable(Enum, rhs.Enum)
                && Object.Equals(Const, rhs.Const)
                ;
        }
        #endregion

        #region 6.2: Numeric instances
        [JsonField(Name = "multipleOf")]
        [JsonFieldIgnorable(WhenValueIs = double.MinValue)]
        public double MultipleOf = double.MinValue;

        [JsonField(Name = "maximum")]
        [JsonFieldIgnorable(WhenValueIs = double.MinValue)]
        public double Maximum = double.MinValue;

        [JsonField(Name = "exclusiveMaximum")]
        [JsonFieldIgnorable(WhenValueIs = double.MinValue)]
        public double ExclusiveMaximum = double.MinValue;

        [JsonField(Name = "minimum")]
        [JsonFieldIgnorable(WhenValueIs = double.MaxValue)]
        public double Minimum = double.MaxValue;

        [JsonField(Name = "exclusiveMinimum")]
        [JsonFieldIgnorable(WhenValueIs = double.MaxValue)]
        public double ExclusiveMinimum = double.MaxValue;

        bool EqualsOnlyNum(JsonSchemaAttribute rhs)
        {
            return MultipleOf == rhs.MultipleOf
                && Maximum == rhs.Maximum
                && ExclusiveMaximum == rhs.ExclusiveMaximum
                && Minimum == rhs.Minimum
                && ExclusiveMinimum == rhs.ExclusiveMinimum
                ;
        }
        #endregion

        #region 6.3. Strings
        [JsonField(Name = "maxLength")]
        [JsonFieldIgnorable(WhenValueIs = int.MinValue)]
        public int MaxLength = int.MinValue;

        [JsonField(Name = "minLength")]
        [JsonFieldIgnorable(WhenValueIs = int.MaxValue)]
        public int MinLength = int.MaxValue;

        [JsonField(Name = "pattern")]
        [JsonFieldIgnorable]
        public string Pattern;

        bool EqualsOnlyString(JsonSchemaAttribute rhs)
        {
            return MaxLength == rhs.MaxLength
                && MinLength == rhs.MinLength
                && Object.Equals(Pattern, rhs.Pattern)
                ;
        }
        #endregion

        #region 6.4: Arrays
        [JsonField(Name = "items",
                   TypeHints = new Type[] { typeof(JsonSchemaAttribute), typeof(JsonSchemaAttribute[]) })]
        [JsonFieldIgnorable]
        public object Items;

        [JsonField(Name = "additionalItems")]
        [JsonFieldIgnorable]
        public JsonSchemaAttribute AdditionalItems;

        [JsonField(Name = "maxItems")]
        [JsonFieldIgnorable(WhenValueIs = int.MinValue)]
        public int MaxItems = int.MinValue;

        [JsonField(Name = "minItems")]
        [JsonFieldIgnorable(WhenValueIs = int.MaxValue)]
        public int MinItems = int.MaxValue;

        [JsonField(Name = "uniqueItems")]
        [JsonFieldIgnorable(WhenValueIs = false)]
        public bool UniqueItems = false;

        // contains

        bool EqualsOnlyArray(JsonSchemaAttribute rhs)
        {
            return EqualsSingletonOrArray<JsonSchemaAttribute>(Items, rhs.Items)
                && Object.Equals(AdditionalItems, rhs.AdditionalItems)
                && MaxItems == rhs.MaxItems
                && MinItems == rhs.MinItems
                && UniqueItems == rhs.UniqueItems
                ;
        }
        #endregion

        #region 6.5: Objects
        [JsonField(Name = "maxProperties")]
        [JsonFieldIgnorable(WhenValueIs = int.MinValue)]
        public int MaxProperties = int.MinValue;

        [JsonField(Name = "minProperties")]
        [JsonFieldIgnorable(WhenValueIs = int.MaxValue)]
        public int MinProperties = int.MaxValue;

        [JsonField(Name = "required")]
        [JsonFieldIgnorable]
        public string[] Required; // Use [JsonSchemaRequired] instead when specify it by attributes.

        [JsonField(Name = "properties")]
        [JsonFieldIgnorable]
        public Dictionary<string, JsonSchemaAttribute> Properties;

        [JsonField(Name = "patternProperties")]
        [JsonFieldIgnorable]
        public Dictionary<string, JsonSchemaAttribute> PatternProperties;

        [JsonField(Name = "additionalProperties")]
        [JsonFieldIgnorable]
        public JsonSchemaAttribute AdditionalProperties;

        [JsonField(Name = "dependencies",
                   /* TODO:
                      A type of this field should be Map<string, string[] | JsonSchemaAttribute>.
                      But there are no ways to represent this type currently...
                    */
                   TypeHints = new Type[] {
                       typeof(Dictionary<string, string[]>),
                       typeof(Dictionary<string, JsonSchemaAttribute>)
                   })]
        [JsonFieldIgnorable]
        public object Dependencies; // Use [JsonSchemaDependencies] instead when specify it by attributes.

        // propertyNames

        bool EqualsOnlyObject(JsonSchemaAttribute rhs)
        {
            // TODO
            return true;
        }
        #endregion

        #region 6.7: Subschemas With Boolean Logic
        [JsonField(Name = "allOf")]
        [JsonFieldIgnorable]
        public List<JsonSchemaAttribute> AllOf;

        [JsonField(Name = "anyOf")]
        [JsonFieldIgnorable]
        public List<JsonSchemaAttribute> AnyOf;

        [JsonField(Name = "oneOf")]
        [JsonFieldIgnorable]
        public List<JsonSchemaAttribute> OneOf;

        [JsonField(Name = "not")]
        [JsonFieldIgnorable]
        public JsonSchemaAttribute Not;

        bool EqualsOnlySubBool(JsonSchemaAttribute rhs)
        {
            return EqualsEnumerable(AllOf, rhs.AllOf)
                && EqualsEnumerable(AnyOf, rhs.AnyOf)
                && EqualsEnumerable(OneOf, rhs.OneOf)
                && Object.Equals(Not, rhs.Not)
                ;
        }
        #endregion

        internal Type _dynamicResolverTag;

        public JsonSchemaAttribute()
        {
        }

        public JsonSchemaAttribute(bool b)
        {
            if (!b)
            {
                // Equivalent to {"not": {}}
                Not = new JsonSchemaAttribute();
            }

            // Equivalent to {}
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as JsonSchemaAttribute;
            if (rhs == null)
            {
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
            var serializer = new JsonSerializer(typeof(JsonSchemaAttribute));
            return serializer.Serialize(this);
        }

        public static JsonSchemaAttribute CreateFromClass<T>(JsonSchemaRegistory reg = null, bool asRef = false)
        {
            return CreateFromType(typeof(T), reg, asRef);
        }

        public static JsonSchemaAttribute CreateFromType(Type ty, JsonSchemaRegistory reg = null, bool asRef = false)
        {
            var kind = Node.KindOfType(ty);
            switch (kind)
            {
                case NodeKind.Boolean:
                    return new JsonSchemaAttribute
                    {
                        Type = "boolean",
                    };

                case NodeKind.Integer:
                    object[] enumsForInteger = null;
                    if (TypeHelper.TypeWrap(ty).IsEnum)
                    {
                        enumsForInteger = System.Enum.GetValues(ty).Cast<object>().ToArray();
                    }
                    return new JsonSchemaAttribute
                    {
                        Type = "integer",
                        Enum = enumsForInteger,
                    };

                case NodeKind.Float:
                    return new JsonSchemaAttribute
                    {
                        Type = "number",
                    };

                case NodeKind.String:
                    object[] enumsForString = null;
                    if (TypeHelper.TypeWrap(ty).IsEnum)
                    {
                        enumsForString = TypeHelper.GetStringEnumNames(ty);
                    }
                    return new JsonSchemaAttribute
                    {
                        Type = "string",
                        Enum = enumsForString,
                    };

                case NodeKind.Array:
                    var elemTy = TypeHelper.ElemTypeOfIEnumerable(ty);
                    return new JsonSchemaAttribute
                    {
                        Type = "array",
                        Items = elemTy != null ? CreateFromType(elemTy, reg, true) : null,
                    };

                case NodeKind.Object:
                    if (ty == typeof(object))
                    {
                        return new JsonSchemaAttribute();
                    }

                    if (TypeHelper.TypeWrap(ty).IsGenericType && ty.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        return new JsonSchemaAttribute
                        {
                            Type = "object",
                        };
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }

            if (reg == null)
            {
                reg = JsonSchemaRegistory.GetDefault();
            }

            var schema = TypeHelper.GetCustomAttribute<JsonSchemaAttribute>(ty);
            if (schema == null)
            {
                schema = new JsonSchemaAttribute();
            }
            schema.Type = "object";

            var schemaId = schema.Id;
            if (schemaId == null)
            {
                schemaId = ty.ToString();
            }
            var refSchema = reg.Resolve(schemaId);
            if (refSchema != null)
            {
                schema = refSchema;
                goto skip;
            }
            else
            {
                reg.Register(schemaId, schema);
            }

            var baseType = TypeHelper.TypeWrap(ty).BaseType;
            HashSet<string> baseFieldNames = null;
            if (baseType != null)
            {
                var baseSchema = CreateFromType(baseType, reg, true);
                if (baseSchema != null && baseSchema.Ref != null)
                {
                    if (schema.AllOf == null)
                    {
                        schema.AllOf = new List<JsonSchemaAttribute>();
                    }
                    schema.AllOf.Add(baseSchema);

                    var baseFields = TypeHelper.TypeWrap(baseType).GetFields(BindingFlags.Public | BindingFlags.Instance);
                    baseFieldNames = new HashSet<string>(baseFields.Select(f => f.Name));
                }
            }

            var properties = new Dictionary<string, JsonSchemaAttribute>();
            var required = new List<string>();
            var dependencies = new Dictionary<string, string[]>();

            var fields = TypeHelper.TypeWrap(ty).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;

                JsonSchemaAttribute fieldSchema = null;
                var attr = TypeHelper.GetCustomAttribute<JsonFieldAttribute>(field);
                var elemName = JsonFieldAttribute.FieldName(attr, field); // TODO: duplication check

                // If elements are also included in Base classes, skip collecting a schema for the elements.
                if (baseFieldNames != null && baseFieldNames.Contains(field.Name))
                {
                    fieldSchema = new JsonSchemaAttribute();
                    goto skipField;
                }

                fieldSchema = TypeHelper.GetCustomAttribute<JsonSchemaAttribute>(field);
                if (fieldSchema == null)
                {
                    fieldSchema = new JsonSchemaAttribute();
                }

                var fieldItemsSchema = TypeHelper.GetCustomAttribute<ItemsJsonSchemaAttribute>(field);
                if (fieldItemsSchema != null)
                {
                    fieldSchema.Items = fieldItemsSchema;
                }

                if (attr != null && attr.DynamicResolverTag != null)
                {
                    if (!TypeHelper.TypeWrap(fieldType).IsGenericType || fieldType.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                    {
                        var baseMsg = "A type of the field which has DynamicResolver must be a Dictionary<,>";
                        var msg = string.Format("{0}: Type = {1} at \"{2}\" of {3}", baseMsg, fieldType, elemName, ty);
                        throw new InvalidOperationException(msg);
                    }

                    var keyType = TypeHelper.TypeWrap(fieldType).GetGenericArguments()[0];
                    if (keyType != typeof(string))
                    {
                        var baseMsg = "A key of the dictionary which has DynamicResolver must be a string type";
                        var msg = string.Format("{0}: KeyType = {1} at \"{2}\" of {3}", baseMsg, keyType, elemName, ty);
                        throw new InvalidOperationException(msg);
                    }

                    fieldSchema._dynamicResolverTag = attr.DynamicResolverTag;
                }

                var fieldItemRequired = TypeHelper.GetCustomAttribute<JsonSchemaRequiredAttribute>(field);
                if (fieldItemRequired != null)
                {
                    required.Add(elemName);
                }

                var fieldItemDependencies = TypeHelper.GetCustomAttribute<JsonSchemaDependenciesAttribute>(field);
                if (fieldItemDependencies != null)
                {
                    dependencies.Add(elemName, fieldItemDependencies.Dependencies);
                }

                var fieldTypeSchema = CreateFromType(fieldType, reg, true);
                if (fieldTypeSchema.Ref != null)
                {
                    fieldSchema = fieldTypeSchema;
                }
                else
                {
                    // Update
                    if (fieldSchema.Type == null)
                    {
                        fieldSchema.Type = fieldTypeSchema.Type;
                    }

                    if (fieldTypeSchema.Items != null)
                    {
                        var fieldTypeSchemaItems = fieldTypeSchema.Items as JsonSchemaAttribute;
                        if (fieldTypeSchemaItems.Ref != null)
                        {
                            fieldSchema.Items = fieldTypeSchemaItems;
                        }
                        else
                        {
                            if (fieldTypeSchemaItems.Type != null)
                            {
                                var fieldSchemaItems = fieldSchema.Items as JsonSchemaAttribute;
                                if (fieldSchemaItems != null)
                                {
                                    fieldSchemaItems.Type = fieldTypeSchemaItems.Type;
                                }
                                else
                                {
                                    fieldSchema.Items = new JsonSchemaAttribute
                                    {
                                        Type = fieldTypeSchemaItems.Type,
                                    };
                                }
                            }
                        }
                    }
                }

            skipField:
                properties.Add(elemName, fieldSchema);
            }

            schema.Properties = properties;
            if (required.Count != 0)
            {
                schema.Required = required.ToArray();
            }
            if (dependencies.Count != 0)
            {
                schema.Dependencies = dependencies;
            }

        skip:
            if (asRef)
            {
                return new JsonSchemaAttribute
                {
                    Ref = schemaId,
                };
            }

            return schema;
        }

        static bool EqualsSingletonOrArray<T>(object lhs, object rhs) where T : class
        {
            if (lhs == null && rhs == null)
            {
                return true;
            }

            if (lhs == null || rhs == null)
            {
                return false;
            }

            var lhsArr = lhs as T[];
            var rhsArr = rhs as T[];
            if (lhsArr != null && rhsArr != null)
            {
                return EqualsEnumerable<T>(lhsArr, rhsArr);
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

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class ItemsJsonSchemaAttribute : JsonSchemaAttribute
    {
    }

    public static class JsonSchemaExtensions
    {
        public static ConstraintsViolationException Validate(this JsonSchemaAttribute j,
                                                             object o,
                                                             JsonSchemaRegistory reg = null)
        {
            return (new JsonSchemaValidator(j)).Validate(o, reg);
        }

        internal static ConstraintsViolationException Validate(this JsonSchemaAttribute j,
                                                               object o,
                                                               Internal.State state,
                                                               JsonSchemaRegistory reg)
        {
            return (new JsonSchemaValidator(j)).Validate(o, state, reg);
        }
    }
}
