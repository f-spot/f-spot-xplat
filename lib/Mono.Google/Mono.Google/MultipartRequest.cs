//
// Mono.Google.MultipartRequest
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//	Stephane Delcroix (stephane@delcroix.org)
//
// (C) Copyright 2006 Novell, Inc. (http://www.novell.com)
// (C) Copyright 2007 S. Delcroix
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
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Mono.Google {
	class MultipartRequest {
		static readonly byte [] crlf = new byte [] { 13, 10 };
		Stream output_stream;
		const string separator_string = "PART_SEPARATOR";
		const string separator = "--" + separator_string + "\r\n";
		const string separator_end = "--" + separator_string + "--\r\n";
		readonly byte [] separator_bytes = Encoding.ASCII.GetBytes (separator);
		readonly byte [] separator_end_bytes = Encoding.ASCII.GetBytes (separator_end);
		bool output_set;

		public MultipartRequest (GoogleConnection conn, string url)
		{
			Request = conn.AuthenticatedRequest (url);
			Request.Method = "POST";
			Request.ContentType = "multipart/related; boundary=\"" + separator_string + "\"";
			Request.Headers.Add ("MIME-version", "1.0");
		}

        public HttpWebRequest Request { get; }

        public Stream OutputStream {
			get { return output_stream; }
			set {
				output_set = true;
				output_stream = value;
			}
		}

		public void BeginPart ()
		{
			BeginPart (false);
		}

		public void BeginPart (bool first)
		{
			if (!first)
				return;
			if (output_stream == null)
				output_stream = Request.GetRequestStream ();

			string multipart = "Media multipart posting\r\n";
			byte [] multipart_bytes = Encoding.ASCII.GetBytes (multipart);
			output_stream.Write (multipart_bytes, 0, multipart_bytes.Length);
			output_stream.Write (separator_bytes, 0, separator_bytes.Length);
		}

		public void AddHeader (string name, string val)
		{
			AddHeader (name, val, false);
		}

		public void AddHeader (string name, string val, bool last)
		{
			AddHeader ($"{name}: {last}");
		}

		public void AddHeader (string header)
		{
			AddHeader (header, false);
		}

		public void AddHeader (string header, bool last)
		{
			bool need_crlf = !header.EndsWith ("\r\n");
			byte [] bytes = Encoding.UTF8.GetBytes (header);
			output_stream.Write (bytes, 0, bytes.Length);
			if (need_crlf)
				output_stream.Write (crlf, 0, 2);
			if (last)
				output_stream.Write (crlf, 0, 2);
		}

		public void WriteContent (string content)
		{
			WriteContent (Encoding.UTF8.GetBytes (content));
		}

		public void WriteContent (byte [] content)
		{
			output_stream.Write (content, 0, content.Length);
			output_stream.Write (crlf, 0, crlf.Length);
		}

		public void WritePartialContent (byte [] content, int offset, int nbytes)
		{
			output_stream.Write (content, offset, nbytes);
		}

		public void EndPartialContent ()
		{
			output_stream.Write (crlf, 0, crlf.Length);
		}

		public void EndPart (bool last)
		{
			if (last) {
				output_stream.Write (separator_end_bytes, 0, separator_end_bytes.Length);
				if (!output_set)
					output_stream.Close ();
			} else {
				output_stream.Write (separator_bytes, 0, separator_bytes.Length);
			}
		}

		public string GetResponseAsString ()
		{
			var response = (HttpWebResponse)Request.GetResponse ();
			string received = "";
			// FIXME: use CharacterSet?
			using (var stream = response.GetResponseStream ()) {
				var sr = new StreamReader (stream, Encoding.UTF8);
				received = sr.ReadToEnd ();
			}
			response.Close ();
			return received;
		}
	}
}
