using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace VJson.Schema
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Field)]
    public class Meta : System.Attribute
    {
        #region Metadata
        public string Title;
        public string Description;
        #endregion

        public override bool Equals(object rhsObj)
        {
            var rhs = rhsObj as Meta;
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

    interface INormalMarker
    {
    }

    interface IItemMarker
    {
    }

    public abstract class IValidation : System.Attribute
    {
        public abstract void UpdateSchemaOf(JsonSchema j);
    }

    public class NumericConstraints : IValidation
    {
        #region 6.2: Numeric instances
        //public double MultipleOf = double.MinValue;
        //public double Maximum = double.MinValue;
        //public double ExclusiveMaximum = double.MinValue;
        public double Minimum = double.MaxValue;
        //public double ExclusiveMinimum = double.MaxValue;
        #endregion

        public override void UpdateSchemaOf(JsonSchema j) {
            j.UpdateBy(this);
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
            throw new NotImplementedException();
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


}
