//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;

namespace VJson.Schema.UnitTests
{
    public class JsonSchemaRegistoryTests
    {
        [Test]
        public void BasicOperationTest()
        {
            var r = new JsonSchemaRegistory();

            Assert.Null(r.Resolve("=TEST="));
            Assert.That(r.GetRegisteredIDs(), Is.EquivalentTo(new string[] { }));

            var s = new JsonSchemaAttribute();
            r.Register("a", s);

            Assert.That(r.Resolve("a"), Is.EqualTo(s));
            Assert.That(r.GetRegisteredIDs(), Is.EquivalentTo(new string[] { "a" }));
        }
    }
}
