//
// PhotoFileTrackerTests.cs
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
using System.Linq;

using FSpot.Import;
using FSpot.Tests.Mocks;
using Mocks;

using Moq;

using NUnit.Framework;

namespace FSpot.Tests.Import
{
	[TestFixture]
	public class PhotoFileTrackerTests
	{
		static readonly Uri sourceUri = new Uri ("file:///path/to/photo.jpg");
		static readonly Uri rawUri = new Uri ("file:///path/to/photo.nef");
		static readonly Uri xmpSourceUri = new Uri ("file:///path/to/photo.xmp");

		static readonly Uri targetBase = new Uri ("file:///photo/store/2016/02/06");
		static readonly Uri targetUri = new Uri ("file:///photo/store/2016/02/06/photo.jpg");
		static readonly Uri targetUri1 = new Uri ("file:///photo/store/2016/02/06/photo-1.jpg");
		static readonly Uri targetUri2 = new Uri ("file:///photo/store/2016/02/06/photo-2.jpg");
		static readonly Uri targetRawUri = new Uri ("file:///photo/store/2016/02/06/photo.nef");
		static readonly Uri targetRawUri1 = new Uri ("file:///photo/store/2016/02/06/photo-1.nef");
		static readonly Uri targetRawUri2 = new Uri ("file:///photo/store/2016/02/06/photo-2.nef");
		static readonly Uri targetRawUri3 = new Uri ("file:///photo/store/2016/02/06/photo-3.nef");
		static readonly Uri targetRawUri4 = new Uri ("file:///photo/store/2016/02/06/photo-4.nef");
		static readonly Uri xmpTargetUri = new Uri ("file:///photo/store/2016/02/06/photo.xmp");
		static readonly Uri xmpTargetUri1 = new Uri ("file:///photo/store/2016/02/06/photo-1.xmp");
		static readonly Uri xmpTargetUri2 = new Uri ("file:///photo/store/2016/02/06/photo-2.xmp");

		[Test]
		public void InitialListsAreEmpty ()
		{
			var tracker = new PhotoFileTracker (null);

			Assert.AreEqual (0, tracker.OriginalFiles.Count ());
			Assert.AreEqual (0, tracker.CopiedFiles.Count ());
		}

		[Test]
		public void DontCopyIfSourceIsTargetFile ()
		{
			var photo = PhotoMock.Create (targetUri);
			var fileSystem = new FileSystemMock ();
			var tracker = new PhotoFileTracker (fileSystem);

			tracker.CopyIfNeeded (photo, targetBase);

			Assert.AreEqual (0, tracker.OriginalFiles.Count ());
			Assert.AreEqual (0, tracker.CopiedFiles.Count ());
			Assert.AreEqual (targetUri, photo.DefaultVersion.Uri);
			fileSystem.FileMock.Verify (f => f.Copy (It. IsAny<Uri> (), It.IsAny<Uri> (), It.IsAny<bool> ()), Times.Never);
		}

		[Test]
		public void CopyNewFile ()
		{
			var photo = PhotoMock.Create (sourceUri);
			var fileSystem = new FileSystemMock ();
			var tracker = new PhotoFileTracker (fileSystem);

			tracker.CopyIfNeeded (photo, targetBase);

			CollectionAssert.AreEquivalent (new[]{ sourceUri }, tracker.OriginalFiles);
			CollectionAssert.AreEquivalent (new[]{ targetUri }, tracker.CopiedFiles);
			Assert.AreEqual (targetUri, photo.DefaultVersion.Uri);
			fileSystem.FileMock.Verify (f => f.Copy (sourceUri, targetUri, false), Times.Once);
		}

		[Test]
		public void XmpIsCopiedIfItExists ()
		{
			var photo = PhotoMock.Create (sourceUri);
			var fileSystem = new FileSystemMock (xmpSourceUri);
			var tracker = new PhotoFileTracker (fileSystem);

			tracker.CopyIfNeeded (photo, targetBase);

			CollectionAssert.AreEquivalent (new[]{ sourceUri, xmpSourceUri }, tracker.OriginalFiles);
			CollectionAssert.AreEquivalent (new[]{ targetUri, xmpTargetUri }, tracker.CopiedFiles);
			Assert.AreEqual (targetUri, photo.DefaultVersion.Uri);
			fileSystem.FileMock.Verify (f => f.Copy (sourceUri, targetUri, false), Times.Once);
			fileSystem.FileMock.Verify (f => f.Copy (xmpSourceUri, xmpTargetUri, true), Times.Once);
		}

		[Test]
		public void CopyWithNewNameIfTargetAlreadyExists ()
		{
			var photo = PhotoMock.Create (sourceUri);
			var fileSystem = new FileSystemMock (targetUri);
			var tracker = new PhotoFileTracker (fileSystem);

			tracker.CopyIfNeeded (photo, targetBase);

			CollectionAssert.AreEquivalent (new[]{ sourceUri }, tracker.OriginalFiles);
			CollectionAssert.AreEquivalent (new[]{ targetUri1 }, tracker.CopiedFiles);
			Assert.AreEqual (targetUri1, photo.DefaultVersion.Uri);
			fileSystem.FileMock.Verify (f => f.Copy (sourceUri, targetUri1, false), Times.Once);
		}

		[Test]
		public void XmpIsCopiedWithSameNameAsDefaultVersion ()
		{
			var photo = PhotoMock.Create (sourceUri);
			var fileSystem = new FileSystemMock (targetUri, xmpSourceUri);
			var tracker = new PhotoFileTracker (fileSystem);

			tracker.CopyIfNeeded (photo, targetBase);

			CollectionAssert.AreEquivalent (new[]{ sourceUri, xmpSourceUri }, tracker.OriginalFiles);
			CollectionAssert.AreEquivalent (new[]{ targetUri1, xmpTargetUri1 }, tracker.CopiedFiles);
			Assert.AreEqual (targetUri1, photo.DefaultVersion.Uri);
			fileSystem.FileMock.Verify (f => f.Copy (sourceUri, targetUri1, false), Times.Once);
			fileSystem.FileMock.Verify (f => f.Copy (xmpSourceUri, xmpTargetUri1, true), Times.Once);
		}

		[Test]
		public void PhotoWithVersionsAndXmpIsCopied ()
		{
			var photo = PhotoMock.Create (sourceUri, rawUri);
			var fileSystem = new FileSystemMock (xmpSourceUri);
			var tracker = new PhotoFileTracker (fileSystem);

			tracker.CopyIfNeeded (photo, targetBase);

			CollectionAssert.AreEquivalent (new[]{ sourceUri, rawUri, xmpSourceUri }, tracker.OriginalFiles);
			CollectionAssert.AreEquivalent (new[]{ targetUri, targetRawUri, xmpTargetUri }, tracker.CopiedFiles);
			Assert.AreEqual (targetUri, photo.DefaultVersion.Uri);
			Assert.AreEqual (targetRawUri, photo.Versions.ElementAt(1).Uri);
			fileSystem.FileMock.Verify (f => f.Copy (sourceUri, targetUri, false), Times.Once);
			fileSystem.FileMock.Verify (f => f.Copy (rawUri, targetRawUri, false), Times.Once);
			fileSystem.FileMock.Verify (f => f.Copy (xmpSourceUri, xmpTargetUri, true), Times.Once);
		}

		[Test]
		public void FilesAreRenamedToNextFreeIndex ()
		{
			var photo = PhotoMock.Create (sourceUri, rawUri);
			var fileSystem = new FileSystemMock (
				xmpSourceUri,
				targetUri, targetUri1,
				targetRawUri, targetRawUri1, targetRawUri2, targetRawUri4);
			var tracker = new PhotoFileTracker (fileSystem);

			tracker.CopyIfNeeded (photo, targetBase);

			CollectionAssert.AreEquivalent (new[]{ sourceUri, rawUri, xmpSourceUri }, tracker.OriginalFiles);
			CollectionAssert.AreEquivalent (new[]{ targetUri2, targetRawUri3, xmpTargetUri2 }, tracker.CopiedFiles);
			Assert.AreEqual (targetUri2, photo.DefaultVersion.Uri);
			Assert.AreEqual (targetRawUri3, photo.Versions.ElementAt(1).Uri);
			fileSystem.FileMock.Verify (f => f.Copy (sourceUri, targetUri2, false), Times.Once);
			fileSystem.FileMock.Verify (f => f.Copy (rawUri, targetRawUri3, false), Times.Once);
			fileSystem.FileMock.Verify (f => f.Copy (xmpSourceUri, xmpTargetUri2, true), Times.Once);
		}
	}
}
