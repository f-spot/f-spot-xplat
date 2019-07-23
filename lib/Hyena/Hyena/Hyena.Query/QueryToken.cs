//
// QueryToken.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007 Novell, Inc.
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

namespace Hyena.Query
{
	public enum TokenID {
        Unknown,
        OpenParen,
        CloseParen,
        Not,
        Or,
        And,
        Range,
        Term
    }

    public class QueryToken
    {
		int line;
		string term;

        public QueryToken()
        {
        }

        public QueryToken(string term)
        {
            ID = TokenID.Term;
            this.term = term;
        }

        public QueryToken(TokenID id)
        {
            ID = id;
        }

        public QueryToken(TokenID id, int line, int column)
        {
            ID = id;
            this.line = line;
            Column = column;
        }

        public TokenID ID { get; set; }

        public int Line {
            get { return line; }
            set { line = value; }
        }

        public int Column { get; set; }

        public string Term {
            get { return term; }
            set { term = value; }
        }
    }
}
