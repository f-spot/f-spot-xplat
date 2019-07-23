
//
// FilterRequest.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Larry Ewing <lewing@novell.com>
//   Stephane Delcroix <sdelcroix@src.gnome.org>
//
// Copyright (C) 2006-2010 Novell, Inc.
// Copyright (C) 2010 Ruben Vermeersch
// Copyright (C) 2006 Larry Ewing
// Copyright (C) 2006, 2008 Stephane Delcroix
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
using System.Collections.Generic;
using System.IO;

using Hyena;

namespace FSpot.Filters
{
	public class FilterRequest : IDisposable
	{
		Uri current;
		readonly List<Uri> tempUris;

		public FilterRequest (Uri source)
		{
			Source = source;
			current = source;
			tempUris = new List<Uri> ();
		}

		// FIXME: We probably don't want this?
		~FilterRequest ()
		{
			Close ();
		}

		public Uri Source { get; private set; }

		public Uri Current {
			get { return current; }
			set {
				if (!value.Equals (Source) && !tempUris.Contains (value))
					tempUris.Add (value);
				current = value;
			}
		}

		public virtual void Close ()
		{
			foreach (var uri in tempUris) {
				try {
					File.Delete (uri.LocalPath);
				} catch (IOException e) {
					Log.Exception (e);
				}
			}
			tempUris.Clear ();
		}

		public void Dispose ()
		{
			Close ();
			GC.SuppressFinalize (this);
		}

		public Uri TempUri ()
		{
			return TempUri (null);
		}

		public Uri TempUri (string extension)
		{
			string imgtemp;
			if (extension != null) {
				string temp = Path.GetTempFileName ();
				imgtemp = temp + "." + extension;
				File.Move (temp, imgtemp);
			} else
				imgtemp = Path.GetTempFileName ();

			var uri = new Uri (imgtemp);
			if (!tempUris.Contains (uri))
				tempUris.Add (uri);
			return uri;
		}

		public void Preserve (Uri uri)
		{
			tempUris.Remove (uri);
		}
	}
}
