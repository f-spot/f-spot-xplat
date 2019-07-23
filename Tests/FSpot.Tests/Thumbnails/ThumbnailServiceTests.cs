//
// ThumbnailServiceTests.cs
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

using Mocks;

using Moq;

using NUnit.Framework;

namespace FSpot.Thumbnail.UnitTest
{
#if FALSE
	[TestFixture]
	public class ThumbnailServiceTests
	{
		static readonly Uri fileUri = new Uri ("file:///path/to/file");
		const string fileHash = "d005f819e9ddaad2d09a23915aa68afe";
		static readonly DateTime fileLastWrite = DateTime.MinValue;

		const string largeThumbnailPath = "/path/to/.cache/thumbnails/large";
		const string normalThumbnailPath = "/path/to/.cache/thumbnails/normal";
		static readonly Uri largeThumbnailUri = new Uri ($"{largeThumbnailPath}/{fileHash}.png");
		static readonly Uri normalThumbnailUri = new Uri ($"{normalThumbnailPath}/{fileHash}.png");

		IXdgDirectoryService xdgDirectoryService;
		IThumbnailerFactory thumbnailerFactory;
		readonly Mock<IThumbnailer> thumbnailerMock = new Mock<IThumbnailer> ();
		readonly byte[] thumbnail = PixbufMock.CreateThumbnail (fileUri, fileLastWrite);

		[OneTimeSetUp]
		public void TestFixtureSetup ()
		{
			var xdgDirectoryServiceMock = new Mock<IXdgDirectoryService> ();
			xdgDirectoryServiceMock.Setup (xdg => xdg.GetThumbnailsDir (ThumbnailSize.Large)).Returns (largeThumbnailPath);
			xdgDirectoryServiceMock.Setup (xdg => xdg.GetThumbnailsDir (ThumbnailSize.Normal)).Returns (normalThumbnailPath);
			xdgDirectoryService = xdgDirectoryServiceMock.Object;

			var thumbnailerFactoryMock = new Mock<IThumbnailerFactory> ();
			thumbnailerFactoryMock.Setup (factory => factory.GetThumbnailerForUri (fileUri)).Returns (thumbnailerMock.Object);
			thumbnailerFactory = thumbnailerFactoryMock.Object;
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void Constructor_CreatesThumbnailFolders_IfTheyDontExist ()
		{
			var fileSystem = new FileSystemMock ();

			// Analysis disable once ObjectCreationAsStatement
			new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			fileSystem.DirectoryMock.Verify (m => m.CreateDirectory (new Uri (largeThumbnailPath)), Times.Once ());
			fileSystem.DirectoryMock.Verify (m => m.CreateDirectory (new Uri (normalThumbnailPath)), Times.Once ());
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void Constructor_DoesNotCreateThumbnailFolders_IfTheyExist ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetDirectory (new Uri (largeThumbnailPath));
			fileSystem.SetDirectory (new Uri (normalThumbnailPath));

			// Analysis disable once ObjectCreationAsStatement
			new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			fileSystem.DirectoryMock.Verify (m => m.CreateDirectory (It.IsAny<Uri> ()), Times.Never ());
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetThumbnail_ReturnsPixbuf_IfValidPngExists ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (fileUri, fileLastWrite);
			fileSystem.SetFile (largeThumbnailUri, fileLastWrite, thumbnail);
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.GetThumbnail (fileUri, ThumbnailSize.Large);

			Assert.IsNotNull (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetThumbnail_CreatesPngAndReturnsPixbuf_IfPngIsMissing ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (fileUri, fileLastWrite);
			thumbnailerMock.Setup (thumb => thumb.TryCreateThumbnail (largeThumbnailUri, ThumbnailSize.Large))
				.Returns (true)
				.Callback (() => fileSystem.SetFile (largeThumbnailUri, fileLastWrite, thumbnail));
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.GetThumbnail (fileUri, ThumbnailSize.Large);

			Assert.IsNotNull (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetThumbnail_ReturnsNull_IfNoThumbnailerFound ()
		{
			var fileSystem = new FileSystemMock ();
			var thumbnailerFactoryMock = new Mock<IThumbnailerFactory> ();
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactoryMock.Object, fileSystem);

			var result = thumbnailService.GetThumbnail (fileUri, ThumbnailSize.Large);

			Assert.IsNull (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetThumbnail_ReturnsNull_IfThumbnailCreationFails ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (fileUri, fileLastWrite);
			thumbnailerMock.Setup (thumb => thumb.TryCreateThumbnail (largeThumbnailUri, ThumbnailSize.Large)).Returns (false);
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.GetThumbnail (fileUri, ThumbnailSize.Large);

			Assert.IsNull (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void DeleteThumbnail_DeletesLargeAndNormalThumbnails_IfTheyExist ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (largeThumbnailUri);
			fileSystem.SetFile (normalThumbnailUri);
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			thumbnailService.DeleteThumbnails (fileUri);

			fileSystem.FileMock.Verify (m => m.Delete (largeThumbnailUri), Times.Once ());
			fileSystem.FileMock.Verify (m => m.Delete (normalThumbnailUri), Times.Once ());
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void DeleteThumbnail_DoesNotDeleteThumbnails_IfNotExist ()
		{
			var fileSystem = new FileSystemMock ();
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			thumbnailService.DeleteThumbnails (fileUri);

			fileSystem.FileMock.Verify (m => m.Delete (largeThumbnailUri), Times.Never ());
			fileSystem.FileMock.Verify (m => m.Delete (normalThumbnailUri), Times.Never ());
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void DeleteThumbnail_IgnoresExceptionsOnDeletingThumbnails ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (largeThumbnailUri);
			fileSystem.SetFile (normalThumbnailUri);
			fileSystem.FileMock.Setup (m => m.Delete (largeThumbnailUri)).Throws<Exception> ();
			fileSystem.FileMock.Setup (m => m.Delete (normalThumbnailUri)).Throws<Exception> ();
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			thumbnailService.DeleteThumbnails (fileUri);

			fileSystem.FileMock.Verify (m => m.Delete (largeThumbnailUri), Times.Once ());
			fileSystem.FileMock.Verify (m => m.Delete (normalThumbnailUri), Times.Once ());
			// test ends without the exceptions thrown by File.Delete beeing unhandled
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void LoadThumbnail_ReturnsNull_IfThumbnailDoesNotExist ()
		{
			var fileSystem = new FileSystemMock ();
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.LoadThumbnail (largeThumbnailUri);

			Assert.IsNull (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void LoadThumbnail_ReturnsPixbuf_IfThumbnailExists ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (largeThumbnailUri, fileLastWrite, thumbnail);
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.LoadThumbnail (largeThumbnailUri);

			Assert.IsNotNull (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void LoadThumbnail_DeletesThumbnail_IfLoadFails ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (largeThumbnailUri, fileLastWrite, thumbnail);
			fileSystem.FileMock.Setup (m => m.Read (largeThumbnailUri)).Throws<Exception> ();
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.LoadThumbnail (largeThumbnailUri);

			Assert.IsNull (result);
			fileSystem.FileMock.Verify (file => file.Delete (largeThumbnailUri), Times.Once ());
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void LoadThumbnail_IgnoresExceptionsOnDeletingThumbnails ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (largeThumbnailUri, fileLastWrite, thumbnail);
			fileSystem.FileMock.Setup (m => m.Read (largeThumbnailUri)).Throws<Exception> ();
			fileSystem.FileMock.Setup (m => m.Delete (largeThumbnailUri)).Throws<Exception> ();
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.LoadThumbnail (largeThumbnailUri);

			Assert.IsNull (result);
			fileSystem.FileMock.Verify (file => file.Delete (largeThumbnailUri), Times.Once ());
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetThumbnailPath_ReturnsPathForLargeThumbnail ()
		{
			var fileSystem = new FileSystemMock ();
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.GetThumbnailPath (fileUri, ThumbnailSize.Large);

			Assert.AreEqual (largeThumbnailUri, result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetThumbnailPath_ReturnsPathForNormalThumbnail ()
		{
			var fileSystem = new FileSystemMock ();
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.GetThumbnailPath (fileUri, ThumbnailSize.Normal);

			Assert.AreEqual (normalThumbnailUri, result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void IsValid_ReturnsFalse_IfPixbufIsNull ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (fileUri, fileLastWrite);
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.IsValid (fileUri, null);

			Assert.IsFalse (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void IsValid_ReturnsFalse_IfFileUriIsDifferent ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (fileUri, fileLastWrite);
			var pixbuf = PixbufMock.CreatePixbuf (new Uri ("file:///some-uri"), fileLastWrite);
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.IsValid (fileUri, pixbuf);

			Assert.IsFalse (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void IsValid_ReturnsFalse_IfMTimeIsDifferent ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (fileUri, fileLastWrite);
			var pixbuf = PixbufMock.CreatePixbuf (fileUri, fileLastWrite.AddMilliseconds (1.0));
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.IsValid (fileUri, pixbuf);

			Assert.IsFalse (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void IsValid_ReturnsFalse_IfFileDoesNotExist ()
		{
			var fileSystem = new FileSystemMock ();
			var pixbuf = PixbufMock.CreatePixbuf (fileUri, fileLastWrite.AddMilliseconds (1.0));
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.IsValid (fileUri, pixbuf);

			Assert.IsFalse (result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void IsValid_ReturnsTrue_IfPixbufIsValid ()
		{
			var fileSystem = new FileSystemMock ();
			fileSystem.SetFile (fileUri, fileLastWrite);
			var pixbuf = PixbufMock.CreatePixbuf (fileUri, fileLastWrite);
			var thumbnailService = new ThumbnailService (xdgDirectoryService, thumbnailerFactory, fileSystem);

			var result = thumbnailService.IsValid (fileUri, pixbuf);

			Assert.IsTrue (result);
		}
	}
#endif
}
