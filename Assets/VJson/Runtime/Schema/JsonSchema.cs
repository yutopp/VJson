//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Runtime;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Globalization;

namespace VJson.Schema
{
    [Json(ImplicitConstructable=true)]
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Field,
                    Inherited = false)]
    public class JsonSchema : Attribute
    {
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

        // dependencies
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

        private bool shouldRef = false;

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

        public static JsonSchema CreateFromClass<T>()
        {
            return CreateFromType(typeof(T));
        }

        public static JsonSchema CreateFromType(Type ty)
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

            var schema = (JsonSchema)Attribute.GetCustomAttribute(ty, typeof(JsonSchema));
            if (schema == null) {
                schema = new JsonSchema();
            }
            schema.Type = "object";
            schema.Properties = new Dictionary<string, JsonSchema>();
            schema.shouldRef = true;

            var fields = ty.GetFields(BindingFlags.Public|BindingFlags.Instance);
            foreach(var field in fields) {
                var attr = (JsonField)Attribute.GetCustomAttribute(field, typeof(JsonField), false);

                // TODO: duplication check
                var elemName = JsonField.FieldName(attr, field);

                var fieldSchema =
                    (JsonSchema)Attribute.GetCustomAttributes(field, typeof(JsonSchema), false)
                    .Where(f => f.GetType() == typeof(JsonSchema))
                    .FirstOrDefault();
                if (fieldSchema == null) {
                    fieldSchema = new JsonSchema();
                }

                var fieldItemsSchema =
                    (ItemsJsonSchema)Attribute.GetCustomAttribute(field, typeof(ItemsJsonSchema), false);
                if (fieldItemsSchema != null) {
                    fieldSchema.Items = fieldItemsSchema;
                }

                var fieldTypeSchema = CreateFromType(field.FieldType);
                if (fieldTypeSchema.shouldRef) {
                    // TODO:
                    throw new NotImplementedException();

                } else {
                    // Update
                    if (fieldSchema.Type == null) {
                        fieldSchema.Type = fieldTypeSchema.Type;
                    }
                }

                schema.Properties.Add(elemName, fieldSchema);
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
        public static bool Validate(this JsonSchema j, object o)
        {
            return (new JsonSchemaValidator(j)).Validate(o);
        }
    }

    public class JsonSchemaValidator
    {
        JsonSchema _schema;

        public JsonSchemaValidator(JsonSchema j) {
            _schema = j;
        }

        public bool Validate(object o) {
            var kind = Node.KindOfValue(o);

            if (_schema.Type != null) {
                if (_schema.Type.GetType().IsArray) {
                    var ts = (string[])_schema.Type;
                    var found = false;
                    foreach(var t in ts) {
                        if (ValidateKind(kind, t)) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        return false;
                    }

                } else {
                    var t = (string)_schema.Type;
                    if (!ValidateKind(kind, t)) {
                        return false;
                    }
                }
            }

            if (_schema.Enum != null) {
                var found = false;
                foreach(var e in _schema.Enum) {
                    if (TypeHelper.DeepEquals(o, e)) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    return false;
                }
            }

            if (_schema.Not != null) {
                if (_schema.Not.Validate(o)) {
                    return false;
                }
            }

            switch(kind) {
                case NodeKind.Boolean:
                    break;

                case NodeKind.Float:
                case NodeKind.Integer:
                    if (!ValidateNumber(Convert.ToDouble(o)))
                    {
                        return false;
                    }
                    break;

                case NodeKind.String:
                    if (!ValidateString((string)o))
                    {
                        return false;
                    }
                    break;

                case NodeKind.Array:
                    if (!ValidateArray(TypeHelper.ToIEnumerable(o)))
                    {
                        return false;
                    }
                    break;

                case NodeKind.Object:
                    if (!ValidateObject(o))
                    {
                        return false;
                    }
                    break;

                case NodeKind.Null:
                    break;

                default:
                    throw new NotImplementedException(kind.ToString());
            }

            return true;
        }

        bool ValidateNumber(double v) {
            if (_schema.MultipleOf != double.MinValue) {
                throw new NotImplementedException();
            }

            if (_schema.Maximum != double.MinValue) {
                if (!(v <= _schema.Maximum))
                {
                    return false;
                }
            }

            if (_schema.ExclusiveMaximum != double.MinValue) {
                if (!(v < _schema.ExclusiveMaximum))
                {
                    return false;
                }
            }

            if (_schema.Minimum != double.MaxValue) {
                if (!(v >= _schema.Minimum))
                {
                    return false;
                }
            }

            if (_schema.ExclusiveMinimum != double.MaxValue) {
                if (!(v > _schema.ExclusiveMinimum))
                {
                    return false;
                }
            }

            return true;
        }

        bool ValidateString(string v) {
            StringInfo si = null;

            if (_schema.MaxLength != int.MinValue) {
                si = si ?? new StringInfo(v);
                if (!(si.LengthInTextElements <= _schema.MaxLength)) {
                    return false;
                }
            }

            if (_schema.MinLength != int.MaxValue) {
                si = si ?? new StringInfo(v);
                if (!(si.LengthInTextElements >= _schema.MinLength)) {
                    return false;
                }
            }

            if (_schema.Pattern != null) {
                if (!Regex.IsMatch(v, _schema.Pattern)) {
                    return false;
                }
            }

            return true;
        }

        bool ValidateArray(IEnumerable<object> v) {
            var length = v.Count();

            if (_schema.MaxItems != int.MinValue) {
                if (!(length <= _schema.MaxItems)) {
                    return false;
                }
            }

            if (_schema.MinItems != int.MaxValue) {
                if (!(length >= _schema.MinItems)) {
                    return false;
                }
            }

            List<object> extraItems = null;

            if (_schema.Items != null) {
                if (_schema.Items.GetType().IsArray) {
                    var itemSchemas = (JsonSchema[])_schema.Items;

                    var i = 0;
                    foreach(var elem in v) {
                        var itemSchema = itemSchemas.ElementAtOrDefault(i);
                        if (itemSchema == null) {
                            if (extraItems == null) {
                                extraItems = new List<object>();
                            }
                            extraItems.Add(elem);
                            continue;
                        }

                        if (!itemSchema.Validate(elem)) {
                            return false;
                        }

                        ++i;
                    }

                } else {
                    var itemSchema = (JsonSchema)_schema.Items;
                    foreach(var elem in v) {
                        if (!itemSchema.Validate(elem)) {
                            return false;
                        }
                    }
                }
            }

            if (_schema.AdditionalItems != null) {
                if (extraItems != null) {
                    foreach(var elem in extraItems) {
                        if (!_schema.AdditionalItems.Validate(elem)) {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        bool ValidateObject(object v) {
            var validated = new List<string>();

            foreach(var kv in TypeHelper.ToKeyValues(v)) {
                if (!ValidateObjectField(kv.Key, kv.Value))
                {
                    return false;
                }

                validated.Add((string)kv.Key);
            }

            if (_schema.Required != null) {
                var req = new HashSet<string>(_schema.Required);
                req.IntersectWith(validated);

                if (req.Count != _schema.Required.Count()) {
                    return false;
                }
            }

            if (_schema.MaxProperties != int.MinValue) {
                if (!(validated.Count <= _schema.MaxProperties)) {
                    return false;
                }
            }

            if (_schema.MinProperties != int.MaxValue) {
                if (!(validated.Count >= _schema.MinProperties)) {
                    return false;
                }
            }

            return true;
        }

        bool ValidateObjectField(string key, object value)
        {
            var matched = false;

            if (_schema.Properties != null) {
                JsonSchema itemSchema = null;
                if (_schema.Properties.TryGetValue(key, out itemSchema)) {
                    matched = true;

                    if (!itemSchema.Validate(value)) {
                        return false;
                    }
                }
            }

            if (_schema.PatternProperties != null) {
                foreach(var pprop in _schema.PatternProperties) {
                    if (Regex.IsMatch(key, pprop.Key)) {
                        matched = true;

                        if (!pprop.Value.Validate(value)) {
                            return false;
                        }
                    }
                }
            }

            if (_schema.AdditionalProperties != null && !matched) {
                if (!_schema.AdditionalProperties.Validate(value)) {
                    return false;
                }
            }

            return true;
        }

        static bool ValidateKind(NodeKind kind, string typeName)
        {
            switch (typeName)
            {
                case "null":
                    return kind == NodeKind.Null;

                case "boolean":
                    return kind == NodeKind.Boolean;

                case "object":
                    return kind == NodeKind.Object;

                case "array":
                    return kind == NodeKind.Array;

                case "number":
                    return kind == NodeKind.Integer || kind == NodeKind.Float;

                case "string":
                    return kind == NodeKind.String;

                case "integer":
                    return kind == NodeKind.Integer;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
