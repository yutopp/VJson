using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace VJson.Schema
{
    [Json(ImplicitConstructable=true)]
    public class JsonSchema : System.Attribute
    {
        #region Metadata
        public string Title;
        public string Description;
        #endregion

        #region 6.1: Any instances
        [JsonField(Name="type", TypeHints = new Type[] {typeof(string), typeof(string[])})]
        public object Type;

        [JsonField(Name="enum")]
        public object[] Enum;

        [JsonField(Name="const")]
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
        public double MultipleOf = double.MinValue;

        [JsonField(Name="maximum")]
        public double Maximum = double.MinValue;

        [JsonField(Name="exclusiveMaximum")]
        public double ExclusiveMaximum = double.MinValue;

        [JsonField(Name="minimum")]
        public double Minimum = double.MaxValue;

        [JsonField(Name="exclusiveMinimum")]
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

        #region 6.4: Arrays
        [JsonField(Name="items", TypeHints=new Type[] {typeof(JsonSchema), typeof(JsonSchema[])})]
        public object Items;

        // additionalItems
        // maxItems
        // minItems
        // uniqueItems
        // contains

        bool EqualsOnlyArray(JsonSchema rhs) {
            return EqualsSingletonOrArray<JsonSchema>(Items, rhs.Items);
        }
        #endregion

        #region 6.5: Objects
        // maxProperties
        // minProperties
        // required
        [JsonField(Name="properties")]
        public Dictionary<string, JsonSchema> Properties;
        // patternProperties
        // additionalProperties
        // dependencies
        // propertyNames
        #endregion

        #region 6.7: Subschemas With Boolean Logic
        // allOf
        // anyOf
        // oneOf
        [JsonField(Name="not")]
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
                && EqualsOnlyArray(rhs)
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

                case NodeKind.Object:
                    break;

                default:
                    throw new NotImplementedException();
            }

            var schema = ty.GetCustomAttribute<Schema>();
            schema.Type = "object";
            schema.Properties = new Dictionary<string, JsonSchema>();
            schema.shouldRef = true;

            var fields = ty.GetFields(BindingFlags.Public|BindingFlags.Instance);
            foreach(var field in fields) {
                var attr = field.GetCustomAttribute<JsonField>();

                var elemName = field.Name;
                if (attr != null && attr.Name != null) {
                    // TODO: duplication check
                    elemName = attr.Name;
                }

                var fieldSchema = (Schema)Attribute.GetCustomAttribute(field, typeof(Schema));
                //var fieldItemsSchema = Attribute.GetCustomAttributes(field, typeof(ItemsSchema));

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
                    break;

                case NodeKind.Array:
                    if (!ValidateArray((IEnumerable<object>)o))
                    {
                        return false;
                    }
                    break;

                case NodeKind.Object:
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

        bool ValidateArray(IEnumerable<object> v) {
            if (_schema.Items != null) {
                if (_schema.Items.GetType().IsArray) {
                    var itemSchemas = (JsonSchema[])_schema.Items;

                    var i = 0;
                    foreach(var elem in v) {
                        var itemSchema = itemSchemas.ElementAtOrDefault(i);
                        if (itemSchema == null) {
                            // TODO: check if additional items are supported
                            break;
                        }

                        if (!itemSchema.Validate(elem)) {
                            return false;
                        }

                        i++;
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
