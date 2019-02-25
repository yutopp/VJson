//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace VJson.UnitTests
{
    class DynamicResolverTests
    {
        class Tag { }

        [Test]
        public void PassTest()
        {
            Type ty;
            Assert.False(DynamicResolver.Find<Tag>("a", out ty));

            DynamicResolver.Register<Tag>("a", typeof(int));
            DynamicResolver.Register<Tag>("b", typeof(string));

            Assert.True(DynamicResolver.Find<Tag>("a", out ty));
            Assert.True(DynamicResolver.Find<Tag>("b", out ty));
            Assert.False(DynamicResolver.Find<Tag>("c", out ty));

            DynamicResolver.DeRegister<Tag>("b");

            Assert.True(DynamicResolver.Find<Tag>("a", out ty));
            Assert.False(DynamicResolver.Find<Tag>("b", out ty)); // "b" is unregisterd

            DynamicResolver.DeRegister<Tag>();

            Assert.False(DynamicResolver.Find<Tag>("a", out ty)); // All keys for Tag are unregisterd
        }
    }
}
