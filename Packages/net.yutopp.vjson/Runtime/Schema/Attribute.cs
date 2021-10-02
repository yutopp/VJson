//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;

namespace VJson.Schema
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class JsonSchemaRequiredAttribute : PreserveAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class JsonSchemaDependenciesAttribute : PreserveAttribute
    {
        public string[] Dependencies { get; private set; }

        public JsonSchemaDependenciesAttribute(params string[] deps)
        {
            Dependencies = deps;
        }
    }

    public enum InfluenceRange
    {
        Entiry,
        AdditionalProperties,
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class JsonSchemaRefAttribute : PreserveAttribute
    {
        public Type TagType { get; private set; }
        public InfluenceRange Influence { get; private set; }

        public JsonSchemaRefAttribute(Type tagType, InfluenceRange influence = InfluenceRange.Entiry)
        {
            Type schemaBaseType;
            if (!RefChecker.IsRefTagDerived(tagType, out schemaBaseType))
            {
                throw new ArgumentException("IRefTag<T> must be derived by tagType");
            }

            TagType = tagType;
            Influence = influence;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class ItemsJsonSchemaRefAttribute : JsonSchemaRefAttribute
    {
        public ItemsJsonSchemaRefAttribute(Type tagType, InfluenceRange influence = InfluenceRange.Entiry)
        : base(tagType, influence)
        {
        }
    }
}
