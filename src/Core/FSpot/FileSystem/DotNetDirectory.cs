//
// DotNetDirectory.cs
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
using System.Collections.Generic;
using System.IO;

using FSpot.Utils;

namespace FSpot.FileSystem
{
	class DotNetDirectory : IDirectory
	{
		/// <summary>
		/// Determines whether the given path refers to an existing directory on disk.
		/// </summary>
		/// <param name="uri">The path to test</param>
		/// <returns>true if path refers to an existing directory; false if the directory
		/// does not exist or an error occurs when trying to determine if the specified
		/// directory exists.</returns>
		public bool Exists (Uri uri)
			=> Directory.Exists (uri.AbsolutePath);

		/// <summary>
		/// Creates all directories and subdirectories in the specified path unless they already exist.
		/// </summary>
		/// <param name="uri">The directory to create.</param>
		public void CreateDirectory (Uri uri)
			=> Directory.CreateDirectory (uri.AbsolutePath);

		/// <summary>
		/// Deletes a specified directory, and optionally any subdirectories.
		/// </summary>
		/// <param name="uri">The name of the empty directory to remove. This directory must be writable and empty.</param>
		public void Delete (Uri uri)
		{
			Directory.Delete (uri.AbsolutePath);
			// TODO: Do we want this to be recursive instead?
			//Directory.Delete (uri.AbsolutePath, true);
		}


		/// <summary>
		/// Enumerates all of this files in the given uri directory
		/// </summary>
		/// <param name="uri">Directory uri</param>
		/// <returns>File uri</returns>
		public IEnumerable<Uri> Enumerate (Uri uri)
		{
			var directory = uri.AbsolutePath;
			if (!Directory.Exists (directory))
				yield break;

			foreach (var file in Directory.EnumerateFiles (directory))
				yield return uri.Append (file);
		}
	}
}
