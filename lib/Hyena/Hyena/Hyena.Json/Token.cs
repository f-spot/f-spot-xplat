//
// Token.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Hyena.Json
{
	public class Token
    {
        public Token (TokenType type) : this (type, null)
        {
        }

        public Token (TokenType type, object value)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; }

        public object Value { get; set; }

        public int SourceLine { get; set; }

        public int SourceColumn { get; set; }

        public static Token ObjectStart {
            get { return new Token (TokenType.ObjectStart); }
        }

        public static Token ObjectFinish {
            get { return new Token (TokenType.ObjectFinish); }
        }

        public static Token ArrayStart {
            get { return new Token (TokenType.ArrayStart); }
        }

        public static Token ArrayFinish {
            get { return new Token (TokenType.ArrayFinish); }
        }

        public static Token Null {
            get { return new Token (TokenType.Null); }
        }

        public static Token Comma {
            get { return new Token (TokenType.Comma); }
        }

        public static Token Colon {
            get { return new Token (TokenType.Colon); }
        }

        public static Token Number (double value)
        {
            return new Token (TokenType.Number, value);
        }

        public static Token Integer (int value)
        {
            return new Token (TokenType.Integer, value);
        }

        public static Token String (string value)
        {
            return new Token (TokenType.String, value);
        }

        public static Token Bool (bool value)
        {
            return new Token (TokenType.Boolean, value);
        }
    }
}
