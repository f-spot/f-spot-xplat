//
// Timer.cs
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

using System;

namespace Hyena
{
    public class Timer : IDisposable
    {
		readonly DateTime start;
		readonly string label;

        public Timer (string format, params object [] vals) : this (string.Format (format, vals))
        {
        }

        public Timer (string label)
        {
            this.label = label;
            start = DateTime.Now;
        }

        public TimeSpan ElapsedTime {
            get { return DateTime.Now - start; }
        }

        public void WriteElapsed (string message)
        {
            Console.Error.WriteLine ("{0} {1} {2}", label, message, ElapsedTime);
        }

        public void Dispose ()
        {
            WriteElapsed ("timer stopped:");
        }
    }
}
