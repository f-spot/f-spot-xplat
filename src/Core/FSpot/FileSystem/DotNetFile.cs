//
// DotNetFile.cs
//
// Author:
//   Stephen Shaw <sshaw@decriptor.com>
//
// Copyright (C) 2019 Stephen Shaw
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
using HeyRed.Mime;

namespace FSpot.FileSystem
{
	class DotNetFile : IFile
	{
		/// <summary>
		/// Determines whether the specified file exists.
		/// </summary>
		/// <param name="uri">The file to check.</param>
		/// <returns><see cref="true"/> if the caller has the required permissions and path contains the name of an existing
		/// file; otherwise, <see cref="false"/>. This method also returns false if path is null, an invalid path, or a
		/// zero-length string. If the caller does not have sufficient permissions to read the specified file,
		/// no exception is thrown and the method returns false regardless of the existence of path.</returns>
		public bool Exists (Uri uri) => File.Exists (uri.AbsolutePath);

		public bool IsSymlink (Uri uri)
		{
			// FIXME!!!, We are no longer checking if the file is a Symlink and assuming it isn't
			return false;
			//var file = FileFactory.NewForUri (uri);
			//using (var root_info = file.QueryInfo ("standard::is-symlink", FileQueryInfoFlags.None, null)) {
			//	return root_info.IsSymlink;
			//}
		}

		/// <summary>
		/// Copies an existing file to a new file.
		/// </summary>
		/// <param name="source">The file to copy.</param>
		/// <param name="destination">The name of the destination file. This cannot be a directory.</param>
		/// <param name="overwrite"><see cref="true"/> if the destination file can be overwritten; otherwise <see cref="false"/></param>
		public void Copy (Uri source, Uri destination, bool overwrite)
			=> File.Copy (source.AbsolutePath, destination.AbsolutePath, overwrite);

		/// <summary>
		/// Deletes the specified file.
		/// </summary>
		/// <param name="uri">The name of the file to be deleted. Wildcard characters are not supported.</param>
		public void Delete (Uri uri)
			=> File.Delete (uri.AbsolutePath);

		public string GetMimeType (Uri uri)
		{
			// FIXME, Find a good way to return MimeType
			var file = Path.GetFileName (uri.AbsolutePath);
			return MimeTypesMap.GetMimeType (file);

		}

		/// <summary>
		/// Returns the date and time the specified file or directory was last written to.
		/// </summary>
		/// <param name="uri">The file or directory for which to obtain write data and time information.</param>
		/// <returns></returns>
		public DateTime GetLastWriteTime (Uri uri)
			=> File.GetLastWriteTime (uri.AbsolutePath);

		/// <summary>
		/// Gets the size, in bytes, of the current file.
		/// </summary>
		/// <param name="uri">The file.</param>
		/// <returns>The size of the content file in bytes.</returns>
		public long GetSize (Uri uri)
			=> new FileInfo (uri.AbsolutePath).Length;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileStream"/> class with the specified path, creation mode, and read/write permission.
		/// </summary>
		/// <param name="uri">File to read.</param>
		/// <returns></returns>
		public Stream Read (Uri uri)
			=> new FileStream (uri.AbsolutePath, FileMode.Open, FileAccess.Read);
	}
}

