//
// BackEase.cs
//
// Author:
//   Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (C) 2009 Novell, Inc.
// Copyright (C) 2009 Stephane Delcroix
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
// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace FSpot.Bling
{	
	public abstract class BackEase : EasingFunction
	{
		double amplitude;

		protected BackEase () : this (1.0)
		{
		}

		protected BackEase (double amplitude)
		{
			if (amplitude < 0)
				throw new ArgumentOutOfRangeException (nameof (amplitude));

			this.amplitude = amplitude;
		}

		protected BackEase (EasingMode easingMode) : this (easingMode, 1.0)
		{
		}

		protected BackEase (EasingMode easingMode, double amplitude) : base (easingMode)
		{
			if (amplitude < 0)
				throw new ArgumentOutOfRangeException (nameof (amplitude));

			this.amplitude = amplitude;
		}

		public double Amplitude {
			get { return amplitude; }
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException (nameof (amplitude));

				amplitude = value;
			}
		}

		protected override double EaseInCore (double normalizedTime)
		{
			return normalizedTime * normalizedTime * normalizedTime - normalizedTime * amplitude * Math.Sin (normalizedTime * Math.PI);
		}
	}
}
