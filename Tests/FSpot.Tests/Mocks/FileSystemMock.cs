//
// FileSystemMock.cs
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
using FSpot.FileSystem;
using Moq;

namespace FSpot.Tests.Mocks
{
	public class FileSystemMock : IFileSystem
	{
		readonly Mock<IDirectory> directory_mock;


        public Mock<IFile> FileMock { get; }

        public Mock<IDirectory> DirectoryMock {
			get {
				return directory_mock;
			}
		}


		public FileSystemMock (params Uri[] existingFiles)
		{
			FileMock = new Mock<IFile> ();
			FileMock.Setup (m => m.Exists (It.IsIn (existingFiles))).Returns (true);

			directory_mock = new Mock<IDirectory> ();
		}

		public void SetFile (Uri file)
		{
			SetFile (file, DateTime.MinValue);
		}

		public void SetFile (Uri file, DateTime lastWriteTime)
		{
			SetFile (file, lastWriteTime, new byte[] { });
		}

		public void SetFile (Uri file, DateTime lastWriteTime, byte[] contents)
		{
			FileMock.Setup (m => m.Exists (file)).Returns (true);
			FileMock.Setup (m => m.GetLastWriteTime (file)).Returns (lastWriteTime);
			FileMock.Setup (m => m.Read (file)).Returns (new MemoryStream (contents));
		}

		public void SetDirectory (Uri directory)
		{
			directory_mock.Setup (m => m.Exists (directory)).Returns (true);
		}

		#region IFileSystem implementation

		public IFile File {
			get {
				return FileMock.Object;
			}
		}

		public IDirectory Directory {
			get {
				return new Mock<IDirectory> ().Object;
			}
		}

		public IPath Path {
			get {
				return new Mock<IPath> ().Object;
			}
		}

		#endregion
	}
}
