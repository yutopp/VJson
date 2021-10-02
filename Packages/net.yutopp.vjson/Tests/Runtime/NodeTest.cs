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
        [Test]
        [TestCaseSource(nameof(ValuesArgs))]
        public void KindOfValueTest(object value, NodeKind expected)
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
        [TestCaseSource(nameof(FixtureArgs))]
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
                UndefinedNode.Undef,
                true
            },
            new object[] {
                new BooleanNode(true)["a"],
                UndefinedNode.Undef,
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
                UndefinedNode.Undef,
                true
            },
            new object[] {
                new NullNode()["a"],
                UndefinedNode.Undef,
                true
            },

            // Undefined
            new object[] {
                new UndefinedNode(),
                new UndefinedNode(),
                true
            },
            new object[] {
                new UndefinedNode(),
                new BooleanNode(true),
                false
            },
            new object[] {
                new UndefinedNode(),
                null,
                false
            },
            new object[] {
                new UndefinedNode()[0],
                UndefinedNode.Undef,
                true
            },
            new object[] {
                new UndefinedNode()["a"],
                UndefinedNode.Undef,
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
                UndefinedNode.Undef,
                true
            },
            new object[] {
                new IntegerNode(42)["a"],
                UndefinedNode.Undef,
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
                UndefinedNode.Undef,
                true
            },
            new object[] {
                new FloatNode(42)["a"],
                UndefinedNode.Undef,
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
                UndefinedNode.Undef,
                true
            },
            new object[] {
                new StringNode("42")["a"],
                UndefinedNode.Undef,
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
                UndefinedNode.Undef,
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
                UndefinedNode.Undef,
                true
            },
            new object[] {
                new ObjectNode()["c"],
                UndefinedNode.Undef,
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
                UndefinedNode.Undef,
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
                UndefinedNode.Undef,
                true
            },
            new object[] {
                new ArrayNode()[0],
                UndefinedNode.Undef,
                true
            },

            // Nested
            new object[] {
                new ArrayNode()[0]["a"][1][20000],
                UndefinedNode.Undef,
                true
            },
        };
    }

    public class ArrayNodeTests
    {
        [Test]
        public void ElementsModificationTest()
        {
            var n = new ArrayNode();
            Assert.AreEqual(null, n.Elems);

            // Addition
            n.AddElement(new IntegerNode(42));
            n.AddElement(new StringNode("test"));

            Assert.AreEqual(2, n.Elems.Count);

            Assert.AreEqual(new IntegerNode(42), n.Elems[0]);
            Assert.AreEqual(new StringNode("test"), n.Elems[1]);

            // Deletion
            n.RemoveElementAt(0);
            n.RemoveElementAt(0);
            n.RemoveElementAt(0); // Do not fail if removes non-existing keys

            Assert.AreEqual(0, n.Elems.Count);
        }

        [Test]
        public void DeletionForEmptyElemsTest()
        {
            var n = new ArrayNode();

            n.RemoveElementAt(0);

            Assert.AreEqual(null, n.Elems);
        }

        [Test]
        public void ForeachForEmptyElemsTest()
        {
            var n = new ArrayNode();

            foreach (var e in n) { }
        }

        [Test]
        public void ForeachElemsTest()
        {
            var n = new ArrayNode();

            n.AddElement(new IntegerNode(42));
            n.AddElement(new StringNode("test"));

            Assert.That(n, Is.EquivalentTo(new INode[] {
                        new IntegerNode(42),
                        new StringNode("test"),
                    }));
        }
    }

    public class ObjectNodeTests
    {
        [Test]
        public void ElementsModificationTest()
        {
            var n = new ObjectNode();
            Assert.AreEqual(null, n.Elems);

            // Addition
            n.AddElement("a", new IntegerNode(42));
            n.AddElement("b", new StringNode("test"));

            Assert.AreEqual(2, n.Elems.Count);

            Assert.AreEqual(new IntegerNode(42), n.Elems["a"]);
            Assert.AreEqual(new StringNode("test"), n.Elems["b"]);

            // Deletion
            n.RemoveElement("a");
            n.RemoveElement("b");
            n.RemoveElement("c"); // Do not fail if removes non-existing keys

            Assert.AreEqual(0, n.Elems.Count);
        }

        [Test]
        public void DeletionForEmptyElemsTest()
        {
            var n = new ObjectNode();

            n.RemoveElement("a");

            Assert.AreEqual(null, n.Elems);
        }

        [Test]
        public void ForeachForEmptyElemsTest()
        {
            var n = new ObjectNode();

            foreach (var e in n) { }
        }

        [Test]
        public void ForeachElemsTest()
        {
            var n = new ObjectNode();

            n.AddElement("a", new IntegerNode(42));
            n.AddElement("b", new StringNode("test"));

            Assert.That(n, Is.EquivalentTo(new KeyValuePair<string, INode>[] {
                        new KeyValuePair<string, INode>("a", new IntegerNode(42)),
                        new KeyValuePair<string, INode>("b", new StringNode("test")),
                    }));
        }
    }
}
