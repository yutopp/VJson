using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace VJson.Schema
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Field)]
    public class Schema : JsonSchema
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ItemsSchema : Schema
    {
    }
}
