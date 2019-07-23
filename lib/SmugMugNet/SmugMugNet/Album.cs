using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Specialized;
using Hyena;

namespace SmugMugNet
{
	public struct Album
	{
		public Album(string title, int id)
		{
			AlbumID = id;
			this.title = title;
		}

        public int AlbumID { get; set; }

        string title;
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
	}
}
