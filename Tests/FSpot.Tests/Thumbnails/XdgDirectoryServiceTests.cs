//
// XdgDirectoryServiceTests.cs
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

using System.IO;
using FSpot.FileSystem;
using FSpot.Thumbnail;
using Mocks;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace FSpot.Tests.Thumbnails
{
	[TestFixture]
	public class XdgDirectoryServiceTests
	{
		static readonly string pathToCache = Path.Combine ("path", "to", ".cache");
		static readonly string homeDir = Path.Combine ("home", "user");
		static readonly string homeDirCache = Path.Combine (homeDir, ".cache");

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetUserCacheDir_ReturnsXdgCacheHome_WhenXdgCacheHomeAndHomeAreSet ()
		{
			var environment = new EnvironmentMock ("XDG_CACHE_HOME", pathToCache);
			environment.SetVariable ("HOME", homeDir);
			var xdg = new XdgDirectoryService (null, environment.Object);

			var result = xdg.GetUserCacheDir ();

			Assert.AreEqual (pathToCache, result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetUserCacheDir_ReturnsXdgCacheHome_WhenXdgCacheHomeSet ()
		{
			var environment = new EnvironmentMock ("XDG_CACHE_HOME", pathToCache);
			var xdg = new XdgDirectoryService (null, environment.Object);

			var result = xdg.GetUserCacheDir ();

			Assert.AreEqual (pathToCache, result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetUserCacheDir_ReturnsHome_WhenXdgCacheHomeNotSet ()
		{
			var environment = new EnvironmentMock ("HOME", homeDir);
			var xdg = new XdgDirectoryService (null, environment.Object);

			var result = xdg.GetUserCacheDir ();

			Assert.AreEqual (homeDirCache, result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetUserCacheDir_ReturnsTempPath_WhenXdgCacheHomeAndHomeNotSet ()
		{
			var environment = EnvironmentMock.Create ("user");
			var fileSystem = new Mock<IFileSystem> { DefaultValue = DefaultValue.Mock };
			var path = fileSystem.Object.Path;
			var pathMock = Mock.Get (path);
			pathMock.Setup (p => p.GetTempPath ()).Returns (Path.Combine ("temp", "path"));

			var result = new XdgDirectoryService (fileSystem.Object, environment).GetUserCacheDir ();

			Assert.AreEqual (Path.Combine ("temp", "path", "user", ".cache"), result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GetUserCacheDir_ReturnsCachedValue_WhenXdgCacheHomeIsChanged ()
		{
			var environment = new EnvironmentMock ("XDG_CACHE_HOME", Path.Combine("first", "path", ".cache"));
			var xdg = new XdgDirectoryService (null, environment.Object);
			xdg.GetUserCacheDir ();
			environment.SetVariable ("XDG_CACHE_HOME", Path.Combine("second", "path", ".cache"));

			var result = xdg.GetUserCacheDir ();

			Assert.AreEqual (Path.Combine("first", "path", ".cache"), result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GethThumbnailsDir_ReturnsFolderForLargeThumbnails ()
		{
			var environment = new EnvironmentMock ("XDG_CACHE_HOME", pathToCache);
			var xdg = new XdgDirectoryService (null, environment.Object);

			var result = xdg.GetThumbnailsDir (ThumbnailSize.Large);

			Assert.AreEqual (Path.Combine("path", "to", ".cache", "thumbnails", "large"), result);
		}

		[Test]
		// Analysis disable once InconsistentNaming
		public void GethThumbnailsDir_ReturnsFolderForNormalThumbnails ()
		{
			var environment = new EnvironmentMock ("XDG_CACHE_HOME", pathToCache);
			var xdg = new XdgDirectoryService (null, environment.Object);

			var result = xdg.GetThumbnailsDir (ThumbnailSize.Normal);

			Assert.AreEqual (Path.Combine ("path", "to", ".cache", "thumbnails", "normal"), result);
		}
	}
}
