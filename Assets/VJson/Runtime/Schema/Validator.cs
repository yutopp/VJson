//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

namespace VJson.Schema
{
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
            var validated = new Dictionary<string, object>();

            foreach(var kv in TypeHelper.ToKeyValues(v)) {
                if (!ValidateObjectField(kv.Key, kv.Value))
                {
                    return false;
                }

                validated.Add(kv.Key, kv.Value);
            }

            if (_schema.Required != null) {
                var req = new HashSet<string>(_schema.Required);
                req.IntersectWith(validated.Keys);

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

            if (_schema.Dependencies != null) {
                if (_schema.Dependencies is Dictionary<string, string[]> strDep) {
                    foreach(var va in validated) {
                        string[] deps = null;
                        if (strDep.TryGetValue(va.Key, out deps)) {
                            var intersected = ((string[])deps.Clone()).Intersect(validated.Keys);
                            if (intersected.Count() != deps.Count()) {
                                return false;
                            }
                        }
                    }

                } else if (_schema.Dependencies is Dictionary<string, JsonSchema> schemaDep) {
                    foreach(var va in validated) {
                        JsonSchema ext = null;
                        if (schemaDep.TryGetValue(va.Key, out ext)) {
                            if (!ext.Validate(v)) {
                                return false;
                            }
                        }
                    }
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
