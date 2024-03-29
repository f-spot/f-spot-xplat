//
// Cr2ImageFile.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Larry Ewing <lewing@src.gnome.org>
//
// Copyright (C) 2005-2010 Novell, Inc.
// Copyright (C) 2010 Ruben Vermeersch
// Copyright (C) 2005-2007 Larry Ewing
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

using Hyena;

using TagLib;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;

namespace FSpot.Imaging
{
	class Cr2ImageFile : BaseImageFile
	{
		uint offset;

		public Cr2ImageFile (Uri uri) : base (uri)
		{
		}

		protected override void ExtractMetadata (TagLib.Image.File metadata)
		{
			base.ExtractMetadata (metadata);

			if (metadata == null)
				return;

			try {
				var tag = metadata.GetTag (TagTypes.TiffIFD) as IFDTag;
				var structure = tag.Structure;
				var entry = structure.GetEntry (0, (ushort)IFDEntryTag.StripOffsets);
				offset = (entry as StripOffsetsIFDEntry).Values [0];
			} catch (Exception e) {
				Log.DebugException (e);
			}
		}

		public override Stream ImageFileStream ()
		{
			var file = base.ImageFileStream ();
			file.Position = offset;
			return file;
		}
	}
}
