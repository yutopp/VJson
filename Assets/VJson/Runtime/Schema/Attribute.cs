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
    public sealed class JsonSchemaRequiredAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class JsonSchemaDependenciesAttribute : Attribute
    {
        public string[] Dependencies { get; private set; }

        public JsonSchemaDependenciesAttribute(params string[] deps)
        {
            Dependencies = deps;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class JsonSchemaRefAttribute : Attribute
    {
        public Type TagType { get; private set; }

        public JsonSchemaRefAttribute(Type tagType)
        {
            Type schemaBaseType;
            if (!RefChecker.IsRefTagDerived(tagType, out schemaBaseType))
            {
                throw new ArgumentException("IRefTag<T> must be derived by tagType");
            }

            TagType = tagType;
        }
    }
}
