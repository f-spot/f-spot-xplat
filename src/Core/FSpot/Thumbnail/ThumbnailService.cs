//
// ThumbnailService.cs
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
using System.Linq;

using FSpot.FileSystem;

using Hyena;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace FSpot.Thumbnail
{
	public class ThumbnailService : IThumbnailService
	{
		readonly IXdgDirectoryService xdgDirectoryService;
		readonly IThumbnailerFactory thumbnailerFactory;
		readonly IFileSystem fileSystem;

		public ThumbnailService (IXdgDirectoryService xdgDirectoryService, IThumbnailerFactory thumbnailerFactory, IFileSystem fileSystem)
		{
			this.xdgDirectoryService = xdgDirectoryService;
			this.thumbnailerFactory = thumbnailerFactory;
			this.fileSystem = fileSystem;

			var large = new Uri (Path.Combine (xdgDirectoryService.GetThumbnailsDir (ThumbnailSize.Large)));
			if (!fileSystem.Directory.Exists (large))
				fileSystem.Directory.CreateDirectory (large);

			var normal = new Uri (Path.Combine (xdgDirectoryService.GetThumbnailsDir (ThumbnailSize.Normal)));
			if (!fileSystem.Directory.Exists (normal))
				fileSystem.Directory.CreateDirectory (normal);
		}

		public IImage GetThumbnail (Uri fileUri, ThumbnailSize size)
		{
			var thumbnailUri = GetThumbnailPath (fileUri, size);
			var thumbnail = LoadThumbnail (thumbnailUri);
			if (IsValid (fileUri, thumbnail))
				return thumbnail;
			var thumbnailer = thumbnailerFactory.GetThumbnailerForUri (fileUri);
			if (thumbnailer == null)
				return null;
			return !thumbnailer.TryCreateThumbnail (thumbnailUri, size)
				? null
				: LoadThumbnail (thumbnailUri);
		}

		public IImage TryLoadThumbnail (Uri fileUri, ThumbnailSize size)
		{
			var thumbnailUri = GetThumbnailPath (fileUri, size);
			return LoadThumbnail (thumbnailUri);
		}

		public void DeleteThumbnails (Uri fileUri)
		{
			Enum.GetValues (typeof(ThumbnailSize))
				.OfType<ThumbnailSize> ()
				.Select (size => GetThumbnailPath (fileUri, size))
				.ToList ()
				.ForEach (thumbnailUri => {
					if (fileSystem.File.Exists (thumbnailUri)) {
						try {
							fileSystem.File.Delete (thumbnailUri);
						}
						// Analysis disable once EmptyGeneralCatchClause
						catch {
							// catch and ignore any errors on deleting thumbnails
							// e.g., unauthorized access, read-only filesystem
						}
					}
				});
		}

		// TODO internal for unit testing with Moq
		public Uri GetThumbnailPath (Uri fileUri, ThumbnailSize size)
		{
			var fileHash = CryptoUtil.Md5Encode (fileUri.AbsoluteUri);
			return new Uri (Path.Combine (xdgDirectoryService.GetThumbnailsDir (size), fileHash + ".png"));
		}

		// TODO internal for unit testing with Moq
		public IImage LoadThumbnail (Uri thumbnailUri)
		{
			if (!fileSystem.File.Exists (thumbnailUri))
				return null;
			try {
				return LoadPng (thumbnailUri);
			} catch (Exception e) {
				try {
					fileSystem.File.Delete (thumbnailUri);
				}
				// Analysis disable once EmptyGeneralCatchClause
				catch {
					// catch and ignore any errors on deleting thumbnails
					// e.g., unauthorized access, read-only filesystem
				}
				Log.Debug ($"Failed to load thumbnail: {thumbnailUri}");
				Log.DebugException (e);
				return null;
			}
		}

		internal const string ThumbMTimeOpt = "tEXt::Thumb::MTime";
		internal const string ThumbUriOpt = "tEXt::Thumb::URI";

		// internal for unit testing with Moq
		// FIXME
		public bool IsValid (Uri uri, IImage image)
		{
			if (image == null) {
				return false;
			}

			//if (image.GetOption (ThumbUriOpt) != uri.ToString ()) {
			//	return false;
			//}

			if (!fileSystem.File.Exists (uri))
				return false;

			var mTime = fileSystem.File.GetLastWriteTime (uri);
			return true;// image.MetaData. .GetOption (ThumbMTimeOpt) == mTime.ToString ();
		}

		IImage LoadPng (Uri uri)
		{
			using (var stream = fileSystem.File.Read (uri))
				return Image.Load (stream, new PngDecoder ());
		}
	}
}
