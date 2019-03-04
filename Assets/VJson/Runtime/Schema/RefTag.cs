//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;

namespace VJson.Schema
{
    public class RefTag<T> where T : struct
    {
    }

    internal static class RefChecker
    {
        public static bool IsRefTagDerived(Type ty, out Type elemType)
        {
            var baseType = TypeHelper.TypeWrap(ty).BaseType;
            if (baseType != null)
            {
                return IsRefTag(baseType, out elemType);
            }

            elemType = null;
            return false;
        }

        public static bool IsRefTag(Type ty, out Type elemType)
        {
            if (TypeHelper.TypeWrap(ty).IsGenericType && ty.GetGenericTypeDefinition() == typeof(RefTag<>))
            {
                elemType = TypeHelper.TypeWrap(ty).GetGenericArguments()[0];
                return true;
            }

            elemType = null;
            return false;
        }
    }
}
