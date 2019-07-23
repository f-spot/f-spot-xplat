//
// ImageFileTests.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Stephen Shaw <sshaw@decriptor.com>
//   Daniel Köb <daniel.koeb@peony.at>
//
// Copyright (C) 2016 Daniel Köb
// Copyright (c) 2012 Stephen Shaw
// Copyright (C) 2010 Novell, Inc.
// Copyright (C) 2010 Ruben Vermeersch
//
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
using FSpot.Imaging;
using Hyena;
using NUnit.Framework;
using Shouldly;

namespace FSpot.Tests.Imaging
{
	[TestFixture]
	public class ImageFileTests
	{
		List<string> _imageTypes;
		[SetUp]
		public void Initialize ()
		{
			var factory = new ImageFileFactory (null);
			_imageTypes = factory.UnitTestImageFileTypes ();
		}

		[TestCase ("file.arw")]
		[TestCase ("file.crw")]
		[TestCase ("file.cr2")]
		[TestCase ("file.dng")]
		[TestCase ("file.mrw")]
		[TestCase ("file.nef")]
		[TestCase ("file.orf")]
		[TestCase ("file.pef")]
		[TestCase ("file.raw")]
		[TestCase ("file.raf")]
		[TestCase ("file.rw2")]
		public void TestIfFileIsRaw (string file)
		{
			var factory = new ImageFileFactory (null);
			var result = factory.IsRaw (Path.GetExtension (file));
			result.ShouldBeTrue ();
		}

		[TestCase ("file.jpg")]
		[TestCase ("file.jpeg")]
		[TestCase ("file.jpe")]
		[TestCase ("file.jfi")]
		[TestCase ("file.jfif")]
		[TestCase ("file.jif")]
		public void TestIfUriJpeg (string file)
		{
			var factory = new ImageFileFactory (null);
			var result = factory.IsJpeg (Path.GetExtension (file));
			Assert.That (result);
		}

		[Test]
		public void CheckLoadableTypes ()
		{
			bool missing = false;

			// Test that we have loaders defined for all Taglib# parseable types.
			foreach (var key in TagLib.FileTypes.AvailableTypes.Keys) {
				var type = TagLib.FileTypes.AvailableTypes[key];
				if (!type.IsSubclassOf (typeof (TagLib.Image.File))) {
					continue;
				}

				var test_key = key;
				if (key.StartsWith ("taglib/")) {
					test_key = "." + key.Substring (7);
				}

				if (!_imageTypes.Contains (test_key)) {
					Log.Information ($"Missing key for {test_key}");
					missing = true;
				}
			}

			Assert.IsFalse (missing, "No missing loaders for Taglib# parseable files.");
		}

		[Test]
		public void CheckTaglibSupport ()
		{
			bool missing = false;
			var missingTypes = new List<string> ();

			foreach (var key in _imageTypes) {
				string type = key;
				if (type.StartsWith ("."))
					type = $"taglib/{type.Substring (1)}";

				if (!TagLib.FileTypes.AvailableTypes.ContainsKey (type)) {
					//Log.Information ($"Missing type support in Taglib# for {type}");
					missingTypes.Add (type);
					missing = true;
				}
			}

			Assert.That (missingTypes.Count == 6, string.Join (",", missingTypes));

			Assert.IsTrue (missing, $"There are {missingTypes.Count} missing type support in Taglib#.");
		}

		[Test]
		public void TestIsJpegRawPair ()
		{
			var jpeg = new Uri ("file:///a/photo.jpeg");
			var jpg = new Uri ("file:///a/photo.jpg");

			var nef = new Uri ("file:///a/photo.nef");
			var nef2 = new Uri ("file:///b/photo.nef");
			var crw = new Uri ("file:///a/photo.crw");
			var crw2 = new Uri ("file:///a/photo2.jpeg");

			var factory = new ImageFileFactory (null);

			// both jpegs
			Assert.IsFalse (factory.IsJpegRawPair (jpeg, jpg));
			// both raw
			Assert.IsFalse (factory.IsJpegRawPair (nef, crw));
			// different filename
			Assert.IsFalse (factory.IsJpegRawPair (jpeg, crw2));
			// different basedir
			Assert.IsFalse (factory.IsJpegRawPair (jpeg, nef2));

			Assert.IsTrue (factory.IsJpegRawPair (jpeg, nef));
			Assert.IsTrue (factory.IsJpegRawPair (jpeg, crw));
			Assert.IsTrue (factory.IsJpegRawPair (jpg, nef));
		}
	}
}
