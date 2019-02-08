using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace VJson.Schema
{
    public class NumericValidator : IValidator
    {
        public NumericConstraints Constraints { get; private set; }

        public NumericValidator(NumericConstraints c) {
            Constraints = c;
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as NumericValidator;
            if (rhs == null) {
                return false;
            }

            return Object.Equals(Constraints, rhs.Constraints);
        }

        public override int GetHashCode()
        {
            throw new NotFiniteNumberException();
        }
    }

    public class ObjectValidator : IValidator
    {
        public Dictionary<string, JsonSchema> Props { get; private set; }

        public ObjectValidator(Dictionary<string, JsonSchema> props) {
            Props = props;
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as ObjectValidator;
            if (rhs == null) {
                return false;
            }

            return Props.OrderBy(p => p.Key).SequenceEqual(rhs.Props.OrderBy(p => p.Key));
        }

        public override int GetHashCode()
        {
            throw new NotFiniteNumberException();
        }
    }

    public class JsonSchema
    {
        public Entity Entity;
        public NodeKind Kind;
        public IValidator Validator;

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as JsonSchema;
            if (rhs == null) {
                return false;
            }

            return Object.Equals(Entity, rhs.Entity) &&
                Kind == rhs.Kind &&
                Object.Equals(Validator, rhs.Validator);
        }

        public override int GetHashCode()
        {
            throw new NotFiniteNumberException();
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
                        Kind = NodeKind.Integer,
                    };

                case NodeKind.String:
                    return new JsonSchema {
                        Kind = NodeKind.String,
                    };

                case NodeKind.Object:
                    break;

                default:
                    throw new NotFiniteNumberException();
            }

            var properties = new Dictionary<string, JsonSchema>();

            var entityAttr = ty.GetCustomAttribute<Entity>();
            var fields = ty.GetFields(BindingFlags.Public|BindingFlags.Instance);
            foreach(var field in fields) {
                var fieldEntity = (Entity)Attribute.GetCustomAttribute(field, typeof(Entity), true);

                var validations = Attribute.GetCustomAttributes(field, typeof(IValidation), true);
                var normalVaridations = validations.Where(a => a is INormalMarker);
                var itemsVaridations = validations.Where(a => a is IItemMarker);

                Console.WriteLine("name: " + field.Name);
                Console.WriteLine("validations: " + String.Join(", ", validations.Select(a => a.ToString())));

                var varidators = validations.Select(v => ((IValidation)v).CreateValidator(/*itemsVaridations*/));

                var schema = CreateFromType(field.FieldType);
                if (fieldEntity != null) {
                    if (schema.Entity != null) {
                        throw new NotImplementedException();
                    }
                    schema.Entity = fieldEntity;
                }
                switch (varidators.Count())
                {
                    case 0:
                        break;

                    case 1:
                        if (schema.Validator != null) {
                            throw new NotImplementedException();
                        }
                        // TODO: check Kind relations
                        schema.Validator = varidators.First();
                        break;

                    default:
                        throw new NotImplementedException();
                }

                properties.Add(field.Name, schema);
            }

            return new JsonSchema {
                Entity = entityAttr != null ? entityAttr : new Entity {
                },
                Kind = NodeKind.Object,
                Validator = new ObjectValidator(properties),
            };
        }
    }

    public interface ICondition {}

    [AttributeUsage(AttributeTargets.Class)]
    public class Entity : System.Attribute
    {
        #region Metadata
        public string Title;
        public string Description;
        #endregion

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as Entity;
            if (rhs == null) {
                return false;
            }

            return Title == rhs.Title && Description == rhs.Description;
        }

        public override int GetHashCode()
        {
            throw new NotFiniteNumberException();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Field : Entity
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Items : Field
    {
    }

    interface INormalMarker
    {
    }

    interface IItemMarker
    {
    }

    public abstract class IValidation : System.Attribute
    {
        public abstract NodeKind Kind { get; }
        public abstract IValidator CreateValidator();
    }

    public class NumericConstraints : IValidation
    {
        #region 6.2: Numeric instances
        // multipleOf
        // maximum
        // exclusiveMaximum
        // minimum
        public double Minimum = double.MaxValue; // -1 is ignored

        // exclusiveMinimum
        #endregion

        public override NodeKind Kind { get => NodeKind.Float; }

        public override IValidator CreateValidator() {
            return new NumericValidator(this);
        }

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as NumericConstraints;
            if (rhs == null) {
                return false;
            }

            return Minimum == rhs.Minimum;
        }

        public override int GetHashCode()
        {
            throw new NotFiniteNumberException();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NumericValidation : NumericConstraints, INormalMarker
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ItemNumericValidation : NumericConstraints, IItemMarker
    {
    }

    // TODO: Move to elsewhere



    #region 6.3: Strings
    // maxLength
    // minLength
    // pattern
    #endregion

    // 6.4

    #region 6.4: Objects
    // maxProperties
    // minProperties
    // required
    // properties
    // patternProperties
    // additionalProperties
    // dependencies
    // propertyNames
    #endregion

    // if
    // then
    // else

    // allOf
    // anyOf
    // oneOf
    // not
}
