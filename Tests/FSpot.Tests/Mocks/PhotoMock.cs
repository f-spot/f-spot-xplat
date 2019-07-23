//
// PhotoMock.cs
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
using System.IO;
using System.Linq;

using FSpot.Core;

using Moq;

namespace Mocks
{
	public static class PhotoMock
	{
		public static IPhoto Create (Uri uri, params Uri[] versionUris)
		{
			// don't care about time
			return Create (uri, DateTime.MinValue, versionUris);
		}

		public static IPhoto Create (Uri uri, DateTime time, params Uri[] versionUris)
		{
			var defaultVersionMock = new Mock<IPhotoVersion> ();
			defaultVersionMock.SetupProperty (v => v.Uri, uri);
			var defaultVersion = defaultVersionMock.Object;

			var versions = versionUris.Select (u => {
				var mock = new Mock<IPhotoVersion> ();
				mock.SetupProperty (m => m.Uri, u);
				return mock.Object;
			}).ToList ();

			var allVersions = new[]{ defaultVersion }.Concat (versions);

			var photo = new Mock<IPhoto> ();
			photo.Setup (p => p.DefaultVersion).Returns (defaultVersion);
			photo.Setup (p => p.Time).Returns (time);
			photo.Setup (p => p.Versions).Returns (allVersions);
			return photo.Object;
		}

		static Mock<IPhoto> CreateMock (Uri uri, string name)
		{
			var versionMock = new Mock<IPhotoVersion> ();
			versionMock.Setup (v => v.BaseUri).Returns (uri);
			versionMock.Setup (v => v.Filename).Returns (Path.GetFileName (uri.AbsoluteUri));
			versionMock.Setup (v => v.Name).Returns (name);

			var photoMock = new Mock<IPhoto> ();
			photoMock.Setup (p => p.DefaultVersion).Returns (versionMock.Object);

			return photoMock;
		}

		public static IPhoto Create (Uri uri, string name)
		{
			return CreateMock (uri, name).Object;
		}

		public static IPhoto CreateWithVersion (Uri originalUri, string originalName, Uri versionUri, string versionName)
		{
			var photoMock = CreateMock (originalUri, originalName);

			var versionMock = new Mock<IPhotoVersion> ();
			versionMock.Setup (v => v.Name).Returns (versionName);
			versionMock.Setup (v => v.BaseUri).Returns (versionUri);
			versionMock.Setup (v => v.Filename).Returns (Path.GetFileName (versionUri.AbsoluteUri));

			var versions = new[] { photoMock.Object.DefaultVersion, versionMock.Object };

			photoMock.Setup (p => p.Versions).Returns (versions);
			return photoMock.Object;
		}
	}
}
