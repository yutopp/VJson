//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;

namespace VJson.Internal
{
    internal struct State
    {
        string _elemName;
        string ElemName
        {
            get
            {
                return _elemName != null ? _elemName : "(root)";
            }
        }

        internal State NestAsElem(int elem)
        {
            return new State()
            {
                _elemName = String.Format("{0}[{1}]", ElemName, elem),
            };
        }

        internal State NestAsElem(string elem)
        {
            return new State()
            {
                _elemName = String.Format("{0}[\"{1}\"]", ElemName, elem),
            };
        }

        internal string CreateMessage(string format, params object[] args)
        {
            return String.Format("{0}: {1}.", ElemName, String.Format(format, args));
        }
    }
}
