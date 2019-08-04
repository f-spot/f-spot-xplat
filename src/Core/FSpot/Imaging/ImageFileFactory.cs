//
// ImageFileFactory.cs
//
// Author:
//   Daniel Köb <daniel.koeb@peony.at>
//   Stephane Delcroix <stephane@delcroix.org>
//   Ruben Vermeersch <ruben@savanne.be>
//   Stephen Shaw <sshaw@decriptor.com>
//
// Copyright (C) 2016 Daniel Köb
// Copyright (C) 2007-2010 Novell, Inc.
// Copyright (C) 2007-2009 Stephane Delcroix
// Copyright (C) 2010 Ruben Vermeersch
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
using System.Linq;

using FSpot.FileSystem;
using FSpot.Utils;

using Hyena;

namespace FSpot.Imaging
{
	public class ImageFileFactory : IImageFileFactory
	{
		readonly TinyIoCContainer container;
		readonly List<string> imageTypes;
		readonly List<string> jpegExtensions;
		readonly List<string> rawExtensions;

		readonly IFileSystem fileSystem;

		public ImageFileFactory (IFileSystem fileSystem)
		{
			container = new TinyIoCContainer ();
			imageTypes = new List<string> ();
			jpegExtensions = new List<string> ();
			rawExtensions = new List<string> ();

			this.fileSystem = fileSystem;

			RegisterTypes ();
		}

		enum ImageType
		{
			Other,
			Jpeg,
			Raw
		}

		void RegisterTypes ()
		{
			// Plain image file extenions
			RegisterExtensions<BaseImageFile> (ImageType.Other,
				".gif",
				".pcx",
				".pnm",
				".pbm",
				".pgm",
				".ppm",
				".bmp",
				".png",
				".tif", ".tiff",
				".svg", ".svgz");

			// Jpegs
			RegisterExtensions<BaseImageFile> (ImageType.Jpeg,
				".jfi", ".jfif", ".jif", ".jpe", ".jpeg", ".jpg");

			// Plain image mime types
			RegisterMimeTypes<BaseImageFile> (
				"image/gif",
				"image/x-pcx",
				"image/x-portable-anymap",
				"image/x-portable-bitmap",
				"image/x-portable-graymap",
				"image/x-portable-pixmap",
				"image/x-bmp", "image/x-MS-bmp",
				"image/jpeg",
				"image/png",
				"image/tiff",
				"image/svg+xml");

			// RAW files
			RegisterExtensions<NefImageFile> (ImageType.Raw,
				".arw",
				".nef",
				".pef",
				".raw",
				".orf",
				".kdc",
				".srf");
			RegisterMimeTypes<NefImageFile> (
				"image/arw", "image/x-sony-arw",
				"image/nef", "image/x-nikon-nef",
				"image/pef", "image/x-pentax-pef",
				"image/raw", "image/x-panasonic-raw",
				"image/x-orf");

			RegisterExtensions<Cr2ImageFile> (ImageType.Raw,
				".cr2");
			RegisterMimeTypes<Cr2ImageFile> (
				"image/cr2", "image/x-canon-cr2");

			RegisterExtensions<DngImageFile> (ImageType.Raw,
				".dng");
			RegisterMimeTypes<DngImageFile> (
				"image/dng", "image/x-adobe-dng");

			RegisterExtensions<DCRawImageFile> (ImageType.Raw,
				".rw2",
				".mrw",
				".x3f",
				".srw");
			RegisterMimeTypes<DCRawImageFile> (
				"image/rw2", "image/x-raw",
				"image/x-mrw",
				"image/x-x3f");

			RegisterExtensions<CiffImageFile> (ImageType.Raw,
				".crw");
			RegisterMimeTypes<CiffImageFile> (
				"image/x-ciff");

			RegisterExtensions<RafImageFile> (ImageType.Raw,
				".raf");
			RegisterMimeTypes<RafImageFile> (
				"image/x-raf");

			// as xcf pixbufloader is not part of gdk-pixbuf, check if it's there,
			// and enable it if needed.
			// FIXME, ImageSharp, Image Format, XCF????
			//foreach (PixbufFormat format in Pixbuf.Formats) {
			//	if (format.Name == "xcf") {
			//		if (format.IsDisabled)
			//			format.SetDisabled (false);
			//		RegisterExtensions<BaseImageFile> (ImageType.Other, ".xcf");
			//	}
			//}
		}

		void RegisterMimeTypes<T> (params string[] mimeTypes)
			where T : class, IImageFile
		{
			foreach (var mimeType in mimeTypes) {
				container.Register<IImageFile, T> (mimeType).AsMultiInstance ();
			}
			imageTypes.AddRange (mimeTypes);
		}

		void RegisterExtensions<T> (ImageType type, params string[] extensions)
			where T : class, IImageFile
		{
			foreach (var extension in extensions) {
				container.Register<IImageFile, T> (extension).AsMultiInstance ();
				switch (type) {
				case ImageType.Jpeg:
					jpegExtensions.Add (extension);
					break;
				case ImageType.Raw:
					rawExtensions.Add (extension);
					break;
				}
			}
			imageTypes.AddRange (extensions);
		}

		public List<string> UnitTestImageFileTypes () => imageTypes;

		public bool HasLoader (Uri uri) => GetLoaderType (uri) != null;

		string GetLoaderType (Uri uri)
		{
			// check if we can find the file, which is not the case
			// with filenames with invalid encoding
			if (!fileSystem.File.Exists (uri))
				return null;

			string extension = uri.GetExtension ().ToLower ();

			// Ignore video thumbnails
			if (extension == ".thm")
				return null;

			// Ignore empty files
			if (fileSystem.File.GetSize (uri) == 0)
				return null;

			var param = UriAsParameter (uri);

			// Get loader by mime-type
			string mime = fileSystem.File.GetMimeType (uri);
			if (container.CanResolve<IImageFile> (mime, param))
				return mime;

			// Get loader by extension
			return container.CanResolve<IImageFile> (extension, param) ? extension : null;
		}

		static NamedParameterOverloads UriAsParameter (Uri uri)
		{
			return new NamedParameterOverloads (new Dictionary<string, object> {
				{ "uri", uri }
			});
		}

		public IImageFile Create (Uri uri)
		{
			var name = GetLoaderType (uri);
			if (name == null)
				throw new Exception ($"Unsupported image: {uri}");

			try {
				return container.Resolve<IImageFile> (name, UriAsParameter (uri));
			} catch (Exception e) {
				Log.DebugException (e);
				throw e;
			}
		}

		public bool IsRaw (string extension)
			=> rawExtensions.Any (x => x == extension);

		public bool IsJpeg (string extension)
			=> jpegExtensions.Any (x => x == extension);

		public bool IsJpegRawPair (Uri file1, Uri file2)
		{
			return file1.GetBaseUri ().ToString () == file2.GetBaseUri ().ToString () &&
				file1.GetFilenameWithoutExtension () == file2.GetFilenameWithoutExtension () &&
				((IsJpeg (file1.GetExtension ()) && IsRaw (file2.GetExtension ())) || (IsRaw (file1.GetExtension ()) && IsJpeg (file2.GetExtension ())));
		}
	}
}
