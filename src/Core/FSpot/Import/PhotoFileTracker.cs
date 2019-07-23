//
// PhotoFileTracker.cs
//
// Author:
//   Daniel Köb <daniel.koeb@peony.at>
//
// Copyright (C) 2016 Daniel Köb
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

using FSpot.Core;
using FSpot.FileSystem;
using FSpot.Utils;

namespace FSpot.Import
{
	public class PhotoFileTracker
	{
		readonly IFileSystem file_system;

		readonly List<Uri> original_files = new List<Uri> ();
		readonly List<Uri> copied_files = new List<Uri> ();

		public PhotoFileTracker (IFileSystem fileSystem)
		{
			file_system = fileSystem;
		}

		public IEnumerable<Uri> OriginalFiles {
			get {
				return original_files;
			}
		}

		public IEnumerable<Uri> CopiedFiles {
			get {
				return copied_files;
			}
		}

		public void CopyIfNeeded (IPhoto item, Uri destinationBase)
		{
			// remember source uri for copying xmp file
			var defaultVersionUri = item.DefaultVersion.Uri;

			foreach (var version in item.Versions) {
				// Copy into photo folder and update IPhotoVersion uri
				var source = version.Uri;
				var destination = destinationBase.Append (source.GetFilename ());
				if (!source.Equals (destination)) {
					destination = GetUniqueFilename (destination);
					file_system.File.Copy (source, destination, false);
					copied_files.Add (destination);
					original_files.Add (source);
					version.Uri = destination;
				}
			}

			// Copy XMP sidecar
			var xmp_original = defaultVersionUri.ReplaceExtension(".xmp");
			if (file_system.File.Exists (xmp_original)) {
				var xmp_destination = item.DefaultVersion.Uri.ReplaceExtension (".xmp");
				file_system.File.Copy (xmp_original, xmp_destination, true);
				copied_files.Add (xmp_destination);
				original_files.Add (xmp_original);
			}
		}

		Uri GetUniqueFilename (Uri dest)
		{
			// Find an unused name
			int i = 1;
			var base_uri = dest.GetBaseUri ();
			var filename = dest.GetFilenameWithoutExtension ();
			var extension = dest.GetExtension ();
			while (file_system.File.Exists (dest)) {
				dest = base_uri.Append ($"{filename}-{i++}{extension}");
			}

			return dest;
		}
	}
}
