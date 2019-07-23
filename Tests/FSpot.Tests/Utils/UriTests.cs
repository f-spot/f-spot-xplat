//
// UriTests.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Stephen Shaw <sshaw@decriptor.com>
//
// Copyright (C) 2019 Stephen Shaw
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FSpot.Utils;
using NUnit.Framework;
using Shouldly;

namespace FSpot.Tests.Utils
{
    [TestFixture]
    public class UriTests
    {
        [TestCaseSource(typeof(UriTestData), nameof(UriTestData.TestCases))]
        public void TestFileUris (UriTest test)
        {
            var uri = new Uri (test.Uri);
			Uri.UnescapeDataString (uri.AbsoluteUri).ShouldBe (test.AbsoluteUri, $"AbsoluteUri for {test.Uri}");
            uri.GetExtension ().ShouldBe (test.Extension, $"Extension for {test.Uri}");
            uri.GetBaseUri ().ToString ().ShouldBe (test.BaseUri, $"BaseUri for {test.Uri}");
            Uri.UnescapeDataString (uri.GetFilename ()).ShouldBe (test.Filename, $"Filename for {test.Uri}");
            Uri.UnescapeDataString (uri.GetFilenameWithoutExtension ()).ShouldBe (test.FilenameWithoutExtension, $"FilenameWithoutExtension for {test.Uri}");
        }

        [Test]
        public void TestReplaceExtension ()
        {
            var uri = new Uri ("file:///test/image.jpg");
            var goal = new Uri ("file:///test/image.xmp");

            Assert.AreEqual (goal, uri.ReplaceExtension (".xmp"));
        }
    }

    public class UriTest
    {
        public string Uri { get; set; }
        public bool IsURI { get; set; }
        public string AbsoluteUri { get; set; }
        public string Extension { get; set; }
        public string BaseUri { get; set; }
        public string Filename { get; set; }
        public string FilenameWithoutExtension { get; set; }
    }

	class UriTestData
	{
		public static IEnumerable<UriTest> TestCases {
			get {
				yield return new UriTest {
					Uri = "https://bugzilla.gnome.org/process_bug.cgi",
					IsURI = true,
					AbsoluteUri = "https://bugzilla.gnome.org/process_bug.cgi",
					Extension = ".cgi",
					BaseUri = "https://bugzilla.gnome.org/",
					Filename = "process_bug.cgi",
					FilenameWithoutExtension = "process_bug"
				};
				yield return new UriTest {
					Uri = "file:///home/ruben/Desktop/F-Spot Vision.pdf",
					IsURI = true,
					AbsoluteUri = "file:///home/ruben/Desktop/F-Spot Vision.pdf",
					Extension = ".pdf",
					BaseUri = "file:///home/ruben/Desktop",
					Filename = "F-Spot Vision.pdf",
					FilenameWithoutExtension = "F-Spot Vision"
				};
				yield return new UriTest {
					Uri = "file:///home/ruben/Projects/f-spot/",
					IsURI = false,
					AbsoluteUri = "file:///home/ruben/Projects/f-spot/",
					Extension = "",
					BaseUri = "file:///home/ruben/Projects/f-spot",
					Filename = "",
					FilenameWithoutExtension = ""
				};
				yield return new UriTest {
					Uri = "file:///home/ruben/Projects/f-spot/README",
					IsURI = false,
					AbsoluteUri = "file:///home/ruben/Projects/f-spot/README",
					Extension = "",
					BaseUri = "file:///home/ruben/Projects/f-spot",
					Filename = "README",
					FilenameWithoutExtension = "README"
				};
				if (RuntimeInformation.IsOSPlatform (OSPlatform.Linux))
					yield return new UriTest {
						Uri = "gphoto2://[usb:002,014]/",
						AbsoluteUri = "gphoto2://[usb:002,014]/",
						Extension = "",
						BaseUri = "gphoto2://[usb:002,014]",
						Filename = "",
						FilenameWithoutExtension = ""
					};
			}
		}
	}
}