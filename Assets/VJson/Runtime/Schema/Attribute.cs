//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;

namespace VJson.Schema
{
    [AttributeUsage(AttributeTargets.Field,
                    Inherited = false)]
    public class JsonSchemaRequired : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field,
                    Inherited = false)]
    public class JsonSchemaDependencies : Attribute
    {
        public string[] Dependencies;

        public JsonSchemaDependencies(string[] deps)
        {
            Dependencies = deps;
        }
    }
}
