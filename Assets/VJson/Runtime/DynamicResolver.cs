//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Generic;

namespace VJson
{
    // TODO: Support multi-threaded
    // Should be divided as an instant class for testing...?
    public static class DynamicResolver
    {
        private static Dictionary<Type, DynamicResolverPerTypes> typeResolver =
            new Dictionary<Type, DynamicResolverPerTypes>();

        public static void Register<T>(string keyName, Type elemType)
        {
            DynamicResolverPerTypes resolver;
            if (!typeResolver.TryGetValue(typeof(T), out resolver))
            {
                resolver = new DynamicResolverPerTypes();
                typeResolver.Add(typeof(T), resolver);
            }

            resolver.Register(keyName, elemType);
        }

        public static void DeRegister<T>(string keyName)
        {
            DynamicResolverPerTypes resolver;
            if (!typeResolver.TryGetValue(typeof(T), out resolver))
            {
                return;
            }

            resolver.DeRegister(keyName);
        }

        public static void DeRegister<T>()
        {
            typeResolver.Remove(typeof(T));
        }

        public static bool Find<T>(string keyName, out Type result)
        {
            return Find(typeof(T), keyName, out result);
        }

        public static bool Find(Type tagType, string keyName, out Type result)
        {
            DynamicResolverPerTypes resolver;
            if (!typeResolver.TryGetValue(tagType, out resolver))
            {
                result = null;
                return false;
            }

            return resolver.Find(keyName, out result);
        }
    }

    // TODO: Support multi-threaded
    class DynamicResolverPerTypes
    {
        private Dictionary<string, Type> keyResolver = new Dictionary<string, Type>();

        public void Register(string keyName, Type elemType)
        {
            keyResolver.Add(keyName, elemType);
        }

        public void DeRegister(string keyName)
        {
            keyResolver.Remove(keyName);
        }

        public bool Find(string keyName, out Type result)
        {
            return keyResolver.TryGetValue(keyName, out result);
        }
    }
}
