//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VJson.Schema
{
    public sealed class JsonSchemaRegistry
    {
        readonly Dictionary<string, JsonSchemaAttribute> _registory = new Dictionary<string, JsonSchemaAttribute>();

        public JsonSchemaAttribute Resolve(string id)
        {
            JsonSchemaAttribute j = null;
            if (_registory.TryGetValue(id, out j))
            {
                return j;
            }

            return null;
        }

        public void Register(string id, JsonSchemaAttribute j)
        {
            _registory.Add(id, j);
        }

        public IEnumerable<string> GetRegisteredIDs()
        {
            return _registory.Keys;
        }
    }
}
