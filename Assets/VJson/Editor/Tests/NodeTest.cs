//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System.Collections.Generic;
using NUnit.Framework;

namespace VJson.UnitTests
{
    public class NodeKindTests
    {
        [TestCaseSource("ValuesArgs")]
        public void KindOfValueTest<T>(T value, NodeKind expected)
        {
            var actual = Node.KindOfValue(value);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void KindOfNullValueTest()
        {
            var actual = Node.KindOfValue<object>(null);
            Assert.AreEqual(NodeKind.Null, actual);
        }

        public static object[] ValuesArgs = {
            new object[] { true, NodeKind.Boolean },
            new object[] { false, NodeKind.Boolean },
            new object[] { 1, NodeKind.Integer },
            new object[] { 1.0, NodeKind.Float },
            new object[] { "", NodeKind.String },
            new object[] { new object[] {}, NodeKind.Array },
            new object[] { new List<int>(), NodeKind.Array },
            new object[] { new Dictionary<string, object>(), NodeKind.Object },
            new object[] { EnumAsInt.A, NodeKind.Integer },
            /*new object[] { EnumAsString.A, NodeKind.String },*/
        };
    }

    [TestFixtureSource("FixtureArgs")]
    public class NodeComparisonTests
    {
        IntegerNode _lhs;
        IntegerNode _rhs;
        bool _expected;

        public NodeComparisonTests(IntegerNode lhs, IntegerNode rhs, bool expected)
        {
            _lhs = lhs;
            _rhs = rhs;
            _expected = expected;
        }

        public void EqualityTest()
        {
            var actual = _lhs.Equals(_rhs);
            Assert.AreEqual(_expected, actual);
        }

        //
        static object[] FixtureArgs = {
            new object[] {
                new IntegerNode(42),
                 new IntegerNode(42),
                 true
            },
            new object[] {
                new IntegerNode(42),
                new IntegerNode(72),
                false
            },
            new object[] {
                new IntegerNode(42),
                null,
                false
            }
        };
    }
}
