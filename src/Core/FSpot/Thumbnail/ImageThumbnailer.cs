//
// ImageThumbnailer.cs
//
// Author:
//   Daniel Köb <daniel.koeb@peony.at>
//   Ruben Vermeersch <ruben@savanne.be>
//
// Copyright (C) 2016 Daniel Köb
// Copyright (C) 2010 Novell, Inc.
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
using System.IO;
using FSpot.FileSystem;
using FSpot.Imaging;

using Hyena;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FSpot.Thumbnail
{
	class ImageThumbnailer : IThumbnailer
	{
		readonly Uri fileUri;
		readonly IImageFileFactory factory;
		readonly IFileSystem fileSystem;

		public ImageThumbnailer (Uri fileUri, IImageFileFactory factory, IFileSystem fileSystem)
		{
			this.fileUri = fileUri;
			this.factory = factory;
			this.fileSystem = fileSystem;
		}

		public bool TryCreateThumbnail (Uri thumbnailUri, ThumbnailSize size)
		{
			try {
				var imageFile = factory.Create (fileUri);
				return CreateThumbnail (thumbnailUri, size, imageFile);
			}
			catch {
				return false;
			}
		}
		bool CreateThumbnail (Uri thumbnailUri, ThumbnailSize size, IImageFile imageFile)
		{
			var pixels = size == ThumbnailSize.Normal ? 128 : 256;
			IImage image;
			try {
				image = imageFile.Load ();
			} catch (Exception e) {
				Log.DebugFormat ("Failed loading image for thumbnailing: {0}", imageFile.Uri);
				Log.DebugException (e);
				return false;
			}

			double scale_x = (double)image.Width / pixels;
			double scale_y = (double)image.Height / pixels;
			double scale = Math.Max (1.0, Math.Max (scale_x, scale_y));
			// Ensures that the minimum value is 1 so that pixbuf.ScaleSimple doesn't return null
			// Seems to only happen in rare(?) cases
			int target_x = Math.Max ((int)(image.Width / scale), 1);
			int target_y = Math.Max ((int)(image.Height / scale), 1);

			var thumbImage = image as Image<Rgba32>;
			thumbImage.Mutate (x => x.Resize (target_x, target_y));// pixbuf.ScaleSimple (target_x, target_y, InterpType.Bilinear);
			var mtime = fileSystem.File.GetLastWriteTime (imageFile.Uri).ToString ();
			using (var stream = new FileStream (thumbnailUri.LocalPath, FileMode.Create)) {
				thumbImage.SaveAsPng (stream, new PngEncoder ());
			}
			// FIXME
				//thumbImage.SaveAsPng (. .Savev (thumbnailUri.LocalPath, "png",
				//	new string[] { ThumbnailService.ThumbUriOpt, ThumbnailService.ThumbMTimeOpt, null },
				//	new string[] { imageFile.Uri.AbsoluteUri, mtime });

			image.Dispose ();
			thumbImage.Dispose ();

			return true;
		}
	}
}
