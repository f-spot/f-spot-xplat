//
// UriCollection.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (C) 2007-2010 Novell, Inc.
// Copyright (C) 2010 Ruben Vermeersch
// Copyright (C) 2007-2009 Stephane Delcroix
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
using System.Xml;

using Hyena;

using GLib;

using FSpot.Core;
using FSpot.Imaging;

namespace FSpot
{
	public class UriCollection : PhotoList
	{
		public UriCollection () : base (new IPhoto[0])
		{
		}

		public UriCollection (System.IO.FileInfo[] files) : this ()
		{
			LoadItems (files);
		}

		public UriCollection (Uri[] uri) : this ()
		{
			LoadItems (uri);
		}

		public void Add (Uri uri)
		{
			if (App.Instance.Container.Resolve<IImageFileFactory> ().HasLoader (uri)) {
				//Console.WriteLine ("using image loader {0}", uri.ToString ());
				Add (new FilePhoto (uri));
			} else {
				var info = FileFactory.NewForUri (uri).QueryInfo ("standard::type,standard::content-type", FileQueryInfoFlags.None, null);

				if (info.FileType == FileType.Directory)
					new DirectoryLoader (this, uri);
				else {
					// FIXME ugh...
					if (info.ContentType == "text/xml"
					 || info.ContentType == "application/xml"
					 || info.ContentType == "application/rss+xml"
					 || info.ContentType == "text/plain") {
						new RssLoader (this, uri);
					}
				}
			}
		}

		public void LoadItems (Uri[] uris)
		{
			foreach (var uri in uris) {
				Add (uri);
			}
		}

		class RssLoader
		{
			public RssLoader (UriCollection collection, Uri uri)
			{
				var doc = new XmlDocument ();
				doc.Load (uri.ToString ());
				var ns = new XmlNamespaceManager (doc.NameTable);
				ns.AddNamespace ("media", "http://search.yahoo.com/mrss/");
				ns.AddNamespace ("pheed", "http://www.pheed.com/pheed/");
				ns.AddNamespace ("apple", "http://www.apple.com/ilife/wallpapers");

				var items = new List<FilePhoto> ();
				var list = doc.SelectNodes ("/rss/channel/item/media:content", ns);
				foreach (XmlNode item in list) {
					var image_uri = new Uri (item.Attributes["url"].Value);
					Log.Debug ($"flickr uri = {image_uri}");
					items.Add (new FilePhoto (image_uri));
				}

				if (list.Count < 1) {
					list = doc.SelectNodes ("/rss/channel/item/pheed:imgsrc", ns);
					foreach (XmlNode item in list) {
						var image_uri = new Uri (item.InnerText.Trim ());
						Log.Debug ($"pheed uri = {uri}");
						items.Add (new FilePhoto (image_uri));
					}
				}

				if (list.Count < 1) {
					list = doc.SelectNodes ("/rss/channel/item/apple:image", ns);
					foreach (XmlNode item in list) {
						var image_uri = new Uri (item.InnerText.Trim ());
						Log.Debug ($"apple uri = {uri}");
						items.Add (new FilePhoto (image_uri));
					}
				}
				collection.Add (items.ToArray ());
			}
		}

		class DirectoryLoader
		{
			readonly UriCollection collection;
			readonly File file;

			public DirectoryLoader (UriCollection collection, Uri uri)
			{
				this.collection = collection;
				file = FileFactory.NewForUri (uri);
				file.EnumerateChildrenAsync ("standard::*", FileQueryInfoFlags.None, 500, null, InfoLoaded);

			}

			void InfoLoaded (GLib.Object o, AsyncResult res)
			{
				var items = new List<FilePhoto> ();
				foreach (FileInfo info in file.EnumerateChildrenFinish (res)) {
					var i = new Uri (file.GetChild (info.Name).Uri);
					Log.Debug ($"testing uri = {i}");
					if (App.Instance.Container.Resolve<IImageFileFactory> ().HasLoader (i))
						items.Add (new FilePhoto (i));
				}
				ThreadAssist.ProxyToMain (() => collection.Add (items.ToArray ()));
			}
		}

		protected void LoadItems (System.IO.FileInfo[] files)
		{
			var items = new List<IPhoto> ();
			foreach (var f in files) {
				if (App.Instance.Container.Resolve<IImageFileFactory> ().HasLoader (new Uri (f.FullName))) {
					Log.Debug (f.FullName);
					items.Add (new FilePhoto (new Uri (f.FullName)));
				}
			}

			list = items;
			Reload ();
		}
	}
}
