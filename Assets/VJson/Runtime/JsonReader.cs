//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.IO;
using System.Text;

namespace VJson
{
	public class JsonReader
	{
		private TextReader _reader;

		private StringBuilder _strCache = new StringBuilder();

		public JsonReader(TextReader reader)
		{
			_reader = reader;
		}

		public INode Read()
		{
			var node = ReadElement();

            var next = _reader.Peek();
            if (next != -1) {
                throw new Exception("Not EOS: " + next);
            }

			return node;
		}

		INode ReadElement()
		{
			SkipWS();
			var node = ReadValue();
			SkipWS();

			return node;
		}

		INode ReadValue()
		{
			INode node = null;

			if ((node = ReadObject()) != null)
			{
				return node;
			}

			if ((node = ReadArray()) != null)
			{
				return node;
			}

            if ((node = ReadString()) != null)
			{
				return node;
			}

			if ((node = ReadNumber()) != null)
			{
				return node;
			}

			if ((node = ReadLiteral()) != null)
			{
				return node;
			}

			return null;
		}

		INode ReadObject()
		{
			var next = _reader.Peek();
			if (next != '{')
			{
				return null;
			}
			_reader.Read(); // Discard

			var node = new ObjectNode();

			for (int i=0;;++i)
			{
				SkipWS();

				next = _reader.Peek();
				if (next == '}')
				{
					_reader.Read(); // Discard
                    break;
				}

				if (i > 0)
				{
					if (next != ',')
					{
						throw new Exception("");
					}
					_reader.Read(); // Discard
				}

				SkipWS();
				INode keyNode = ReadString();
				if (keyNode == null)
				{
					throw new Exception("");
				}
				SkipWS();

				next = _reader.Peek();
				if (next != ':')
				{
					throw new Exception("");
				}
				_reader.Read(); // Discard

				INode elemNode = ReadElement();
				if (elemNode == null)
				{
					throw new Exception("");
				}

				node.AddElement(((StringNode)keyNode).Value, elemNode);
			}

            return node;
		}

		INode ReadArray()
		{
			var next = _reader.Peek();
			if (next != '[')
			{
				return null;
			}
			_reader.Read(); // Discard

			var node = new ArrayNode();

			for (int i=0;;++i)
			{
				SkipWS();

				next = _reader.Peek();
				if (next == ']')
				{
					_reader.Read(); // Discard
					break;
				}

				if (i > 0)
				{
					if (next != ',')
					{
						throw new Exception("");
					}
					_reader.Read(); // Discard
				}

				INode elemNode = ReadElement();
				if (elemNode == null)
				{
					throw new Exception("");
				}

				node.AddElement(elemNode);
			}

            return node;
		}

		INode ReadString()
		{
			var next = _reader.Peek();
			if (next != '"')
			{
				return null;
			}
			_reader.Read(); // Discard

			for (;;)
			{
				next = _reader.Peek();
				switch (next)
				{
					case '"':
						_reader.Read(); // Discard

						var span = CommitBuffer();
						return new StringNode(span);

					case '\\':
                        SaveToBuffer(_reader.Read());
						if (!ReadEscape())
						{
							throw new Exception("");
						};
						break;

					default:
                        var c = _reader.Read(); // Consume
                        var codePoint = c;
                        var isPair = char.IsHighSurrogate((char)c);
                        if (isPair) {
                            next = _reader.Read();  // Consume
                            if (!char.IsLowSurrogate((char)next)) {
                                throw new Exception("");
                            }
                            codePoint = char.ConvertToUtf32((char)c, (char)next);
                        }

						if (codePoint < 0x20 || codePoint > 0x10ffff)
						{
							throw new Exception("");
						}

						SaveToBuffer(c);
                        if (isPair) {
                            SaveToBuffer(next);
                        }

						break;
				}
			}
		}

        bool ReadEscape()
        {
            var next = _reader.Peek();
            switch(next)
            {
                case '\"':
                    SaveToBuffer(_reader.Read());
                    return true;

                case '\\':
                    SaveToBuffer(_reader.Read());
                    return true;

                case 'b':
                    SaveToBuffer(_reader.Read());
                    return true;

                case 'n':
                    SaveToBuffer(_reader.Read());
                    return true;

                case 'r':
                    SaveToBuffer(_reader.Read());
                    return true;

                case 't':
                    SaveToBuffer(_reader.Read());
                    return true;

                case 'u':
                    SaveToBuffer(_reader.Read());
                    for(int i=0; i<4; ++i) {
                        if (!ReadHex()) {
                            throw new Exception("");
                        }
                    }
                    return true;

                default:
                    return false;
            }
        }

        bool ReadHex()
        {
            if (ReadDigit()) {
                return true;
            }

			var next = _reader.Peek();
			if (next >= 'A' && next <= 'F')
			{
                SaveToBuffer(_reader.Read());
				return true;
			}

            if (next >= 'a' && next <= 'f')
			{
                SaveToBuffer(_reader.Read());
				return true;
			}

            return false;
        }

		INode ReadNumber()
		{
			if (!ReadInt())
			{
				return null;
			}

            var isFloat = false;
			isFloat |= ReadFrac();
            isFloat |= ReadExp();

			var span = CommitBuffer();
            if (isFloat) {
                return new FloatNode(span);
            }
			return new IntegerNode(span);
		}

		bool ReadInt()
		{
			if (ReadOneNine())
			{
				ReadDigits();
				return true;
			}

			if (ReadDigit())
			{
				return true;
			}

			var next = _reader.Peek();
			if (next != '-')
			{
				return false;
			}

			SaveToBuffer(_reader.Read());

			if (ReadOneNine())
			{
				ReadDigits();
				return true;
			}

			if (ReadDigit())
			{
				return true;
			}

			throw new Exception("");
		}

		bool ReadDigits()
		{
			if (!ReadDigit())
			{
				return false;
			}

			while (ReadDigit()) {}
			return true;
		}

		bool ReadDigit()
		{
			var next = _reader.Peek();
			if (next != '0')
			{
				return ReadOneNine();
			}

			SaveToBuffer(_reader.Read());

			return true;
		}

		bool ReadOneNine()
		{
			var next = _reader.Peek();
			if (next < '1' || next > '9')
			{
				return false;
			}

			SaveToBuffer(_reader.Read());

			return true;
		}

		bool ReadFrac()
		{
			var next = _reader.Peek();
			if (next != '.')
			{
				return false;
			}

            SaveToBuffer(_reader.Read());

            if (!ReadDigit()) {
                throw new Exception("");
            }

            return true;
		}

        bool ReadExp()
        {
            var next = _reader.Peek();
			if (next != 'E' && next != 'e')
			{
				return false;
			}

            SaveToBuffer(_reader.Read());

            ReadSign();

            if (!ReadDigit()) {
                throw new Exception("");
            }

            return true;
        }

        bool ReadSign() {
            var next = _reader.Peek();
			if (next != '+' && next != '-')
			{
				return false;
			}

            SaveToBuffer(_reader.Read());

            return true;
        }

        INode ReadLiteral() {
            var s = String.Empty;

            var next = _reader.Peek();
            switch(next) {
                case 't':
                    // Maybe true
                    s = ConsumeChars(4);
                    if (s.ToLower() != "true") {
                        throw new Exception("T: " + s);
                    }
                    return new BooleanNode(s);

                case 'f':
                    // Maybe false
                    s = ConsumeChars(5);
                    if (s.ToLower() != "false") {
                        throw new Exception("F: " + s);
                    }
                    return new BooleanNode(s);

                case 'n':
                    // Maybe null
                    s = ConsumeChars(4);
                    if (s.ToLower() != "null") {
                        throw new Exception("N: " + s);
                    }
                    return new NullNode();

                default:
                    return null;
            }
        }

        void SkipWS()
		{
			for (;;)
			{
				var next = _reader.Peek();
				switch (next)
				{
					case 0x0009:
					case 0x000a:
					case 0x000d:
					case 0x0020:
						_reader.Read(); // Discard
						break;

					default:
						return;
				}
			}
		}

		void SaveToBuffer(int c)
		{
			_strCache.Append((char)c);
		}

		string CommitBuffer()
		{
			var span = _strCache.ToString();
			_strCache.Length = 0;

			return span;
		}

        string ConsumeChars(int length)
        {
            for(int i=0; i<length; ++i) {
                var c = _reader.Read();
                SaveToBuffer(c);
            }
            return CommitBuffer();
        }
	}
}
