//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
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
            new object[] { EnumAsString.NameA, NodeKind.String },
        };
    }

    public class NodeComparisonTests
    {
        [Test]
        [TestCaseSource("FixtureArgs")]
        public void EqualityTest(INode lhs, INode rhs, bool expected)
        {
            var actual = Object.Equals(lhs, rhs);
            Assert.AreEqual(expected, actual);
        }

        //
        static object[] FixtureArgs = {
            // Boolean
            new object[] {
                new BooleanNode(true),
                new BooleanNode(true),
                true
            },
            new object[] {
                new BooleanNode(true),
                new NullNode(),
                false
            },
            new object[] {
                new BooleanNode(true),
                new BooleanNode(false),
                false
            },
            new object[] {
                new BooleanNode(true),
                null,
                false
            },
            new object[] {
                new BooleanNode(true)[0],
                NullNode.Null,
                true
            },
            new object[] {
                new BooleanNode(true)["a"],
                NullNode.Null,
                true
            },

            // Null
            new object[] {
                new NullNode(),
                new NullNode(),
                true
            },
            new object[] {
                new NullNode(),
                new BooleanNode(true),
                false
            },
            new object[] {
                new NullNode(),
                null,
                false
            },
            new object[] {
                new NullNode()[0],
                NullNode.Null,
                true
            },
            new object[] {
                new NullNode()["a"],
                NullNode.Null,
                true
            },

            // Integer
            new object[] {
                new IntegerNode(42),
                new IntegerNode(42),
                true
            },
            new object[] {
                new IntegerNode(42),
                new NullNode(),
                false
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
            },
            new object[] {
                new IntegerNode(42)[0],
                NullNode.Null,
                true
            },
            new object[] {
                new IntegerNode(42)["a"],
                NullNode.Null,
                true
            },

            // Float
            new object[] {
                new FloatNode(42),
                new FloatNode(42),
                true
            },
            new object[] {
                new FloatNode(42),
                new NullNode(),
                false
            },
            new object[] {
                new FloatNode(42),
                new FloatNode(72),
                false
            },
            new object[] {
                new FloatNode(42),
                null,
                false
            },
            new object[] {
                new FloatNode(42)[0],
                NullNode.Null,
                true
            },
            new object[] {
                new FloatNode(42)["a"],
                NullNode.Null,
                true
            },

            // String
            new object[] {
                new StringNode("42"),
                new StringNode("42"),
                true
            },
            new object[] {
                new StringNode("42"),
                new NullNode(),
                false
            },
            new object[] {
                new StringNode("42"),
                new StringNode("72"),
                false
            },
            new object[] {
                new StringNode("42"),
                null,
                false
            },
            new object[] {
                new StringNode("42")[0],
                NullNode.Null,
                true
            },
            new object[] {
                new StringNode("42")["a"],
                NullNode.Null,
                true
            },

            // Object
            new object[] {
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                },
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                },
                true
            },
            new object[] {
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                },
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"b", new StringNode("42")},
                        {"a", new IntegerNode(42)},
                    },
                },
                true
            },
            new object[] {
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                },
                new NullNode(),
                false
            },
            new object[] {
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                },
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(72)},
                        {"b", new StringNode("72")},
                    },
                },
                false
            },
            new object[] {
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                },
                null,
                false
            },
            new object[] {
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                }[0],
                NullNode.Null,
                true
            },
            new object[] {
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                }["a"],
                new IntegerNode(42),
                true
            },
            new object[] {
                new ObjectNode {
                    Elems = new Dictionary<string, INode> {
                        {"a", new IntegerNode(42)},
                        {"b", new StringNode("42")},
                    },
                }["c"],
                NullNode.Null,
                true
            },
            new object[] {
                new ObjectNode()["c"],
                NullNode.Null,
                true
            },

            // Array
            new object[] {
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                },
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                },
                true
            },
            new object[] {
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                },
                new ArrayNode {
                    Elems = new List<INode> {
                        new StringNode("42"),
                        new IntegerNode(42),
                    },
                },
                false
            },
            new object[] {
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                },
                new NullNode(),
                false
            },
            new object[] {
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                },
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(72),
                        new StringNode("72"),
                    },
                },
                false
            },
            new object[] {
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                },
                null,
                false
            },
            new object[] {
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                }["a"],
                NullNode.Null,
                true
            },
            new object[] {
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                }[0],
                new IntegerNode(42),
                true
            },
            new object[] {
                new ArrayNode {
                    Elems = new List<INode> {
                        new IntegerNode(42),
                        new StringNode("42"),
                    },
                }[2],
                NullNode.Null,
                true
            },
            new object[] {
                new ArrayNode()[0],
                NullNode.Null,
                true
            },

            // Nested
            new object[] {
                new ArrayNode()[0]["a"][1][20000],
                NullNode.Null,
                true
            },
        };
    }
}
