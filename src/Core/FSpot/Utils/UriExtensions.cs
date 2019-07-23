//
// UriExtensions.cs
//
// Author:
//   Mike Gemünde <mike@gemuende.de>
//
// Copyright (C) 2009 Novell, Inc.
// Copyright (C) 2009 Mike Gemünde
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
using System.IO;

namespace FSpot.Utils
{
	public static class UriExtensions
	{
		public static Uri GetDirectoryUri (this Uri uri)
		{
			var builder = new UriBuilder (uri) {
				Path = $"{Path.GetDirectoryName (uri.AbsolutePath)}/"
			};

			return builder.Uri;
		}

		public static string GetFilename (this Uri uri)
		{
			return Path.GetFileName (uri.AbsolutePath);
		}

		public static Uri Append (this Uri base_uri, string filename)
		{
			return new Uri (base_uri.AbsoluteUri + (base_uri.AbsoluteUri.EndsWith ("/") ? "" : "/") + filename);
		}

		public static Uri GetBaseUri (this Uri uri)
		{
			var abs_uri = uri.AbsoluteUri;
			return new Uri (abs_uri.Substring (0, abs_uri.LastIndexOf ('/')));
		}

		public static string GetExtension (this Uri uri)
		{
			var abs_uri = uri.AbsoluteUri;
			var index = abs_uri.LastIndexOf ('.');
			return index == -1 ? string.Empty : abs_uri.Substring (index);
		}

		public static string GetFilenameWithoutExtension (this Uri uri)
		{
			var name = uri.GetFilename ();
			var index = name.LastIndexOf ('.');
			return index > -1 ? name.Substring (0, index) : name;
		}

		public static Uri ReplaceExtension (this Uri uri, string extension)
		{
			return uri.GetBaseUri ().Append (uri.GetFilenameWithoutExtension () + extension);
		}
	}
}
