//
// Thickness.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright 2009-2010 Novell, Inc.
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

namespace Hyena.Gui.Canvas
{
    public struct Thickness
    {
		double top;
		double bottom;

        public static readonly Thickness Zero = new Thickness (0);

        public Thickness (double thickness)
            : this (thickness, thickness, thickness, thickness)
        {
        }

        public Thickness (double xthickness, double ythickness)
            : this (xthickness, ythickness, xthickness, ythickness)
        {
        }

        public Thickness (double left, double top, double right, double bottom)
        {
            Left = left;
            this.top = top;
            Right = right;
            this.bottom = bottom;
        }

        public override string ToString ()
        {
            return string.Format ("{0},{1},{2},{3}", double.IsNaN (Left) ? "Auto" : Left.ToString (),
				double.IsNaN (top) ? "Auto" : top.ToString (),
				double.IsNaN (Right) ? "Auto" : Right.ToString (),
				double.IsNaN (bottom) ? "Auto" : bottom.ToString ());
        }

        public override bool Equals (object o)
        {
            if (!(o is Thickness)) {
                return false;
            }

            return this == (Thickness)o;
        }

        public bool Equals (Thickness thickness)
        {
            return this == thickness;
        }

        public override int GetHashCode ()
        {
            return Left.GetHashCode () ^ top.GetHashCode () ^ Right.GetHashCode () ^ bottom.GetHashCode ();
        }

        public static bool operator == (Thickness t1, Thickness t2)
        {
            return t1.Left == t2.Left &&
                t1.Right == t2.Right &&
                t1.top == t2.top &&
                t1.bottom == t2.bottom;
        }

        public static bool operator != (Thickness t1, Thickness t2)
        {
            return !(t1 == t2);
        }

        public double Left { get; set; }

        public double Top {
            get { return top; }
            set { top = value; }
        }

        public double Right { get; set; }

        public double Bottom {
            get { return bottom; }
            set { bottom = value; }
        }

        public double X {
            get { return Left + Right; }
        }

        public double Y {
            get { return Top + Bottom; }
        }
    }
}
