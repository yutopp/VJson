//
// Copyright (c) 2019- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Generic;
using System.IO;

namespace VJson
{
    // TODO: Add [Preserve] in Unity
    public class JsonWriter
    {
        enum State
        {
            ObjectKeyHead,
            ObjectKeyOther,
            ObjectValue,
            ArrayHead,
            ArrayOther,
            None,
        }

        private TextWriter _writer;
        private Stack<State> _states = new Stack<State>();

        public JsonWriter(TextWriter writer)
        {
            this._writer = writer;
            this._states.Push(State.None);
        }

        public void WriteObjectStart()
        {
            var state = _states.Peek();
            if (state == State.ObjectKeyHead || state == State.ObjectKeyOther)
            {
                throw new Exception("");
            }

            WriteDelimiter();
            _writer.Write("{");

            _states.Push(State.ObjectKeyHead);
        }

        public void WriteObjectKey(string key)
        {
            var state = _states.Peek();
            if (state != State.ObjectKeyHead && state != State.ObjectKeyOther)
            {
                throw new Exception("");
            }

            WriteValue(key);
            _writer.Write(":");

            _states.Pop();
            _states.Push(State.ObjectValue);
        }

        public void WriteObjectEnd()
        {
            var state = _states.Peek();
            if (state != State.ObjectKeyHead && state != State.ObjectKeyOther)
            {
                throw new Exception("");
            }

            _writer.Write("}");

            _states.Pop();
        }

        public void WriteArrayStart()
        {
            var state = _states.Peek();
            if (state == State.ObjectKeyHead || state == State.ObjectKeyOther)
            {
                throw new Exception("");
            }

            WriteDelimiter();
            _writer.Write("[");

            _states.Push(State.ArrayHead);
        }

        public void WriteArrayEnd()
        {
            var state = _states.Peek();
            if (state != State.ArrayHead && state != State.ArrayOther)
            {
                throw new Exception("");
            }

            _writer.Write("]");

            _states.Pop();
        }

        public void WriteValue(bool v)
        {
            WriteDelimiter();

            _writer.Write(v ? "true" : "false");
        }

        public void WriteValue(byte v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(sbyte v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(char v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(decimal v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(double v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(float v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(int v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(uint v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(long v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(ulong v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(short v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(ushort v)
        {
            WritePrimitive(v);
        }

        public void WriteValue(string v)
        {
            WriteDelimiter();

            _writer.Write(@"""");
            _writer.Write(v);
            _writer.Write(@"""");
        }

        public void WriteValueNull()
        {
            WriteDelimiter();

            _writer.Write("null");
        }

        void WritePrimitive<T>(T v)
        {
            WriteDelimiter();

            _writer.Write(v);
        }

        void WriteDelimiter()
        {
            var state = _states.Peek();
            if (state == State.ArrayHead)
            {
                _states.Pop();
                _states.Push(State.ArrayOther);
                return;
            }

            if (state == State.ArrayOther || state == State.ObjectKeyOther)
            {
                _writer.Write(",");
            }

            if (state == State.ObjectValue)
            {
                _states.Pop();
                _states.Push(State.ObjectKeyOther);
            }
        }
    }
}
