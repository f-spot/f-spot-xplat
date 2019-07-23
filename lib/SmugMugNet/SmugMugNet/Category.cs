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
	public struct Category
	{
		public Category( string title, int id)
		{
			this.title = title;
			CategoryID = id;
		}

        public int CategoryID { get; set; }

        string title;
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
	}
}
