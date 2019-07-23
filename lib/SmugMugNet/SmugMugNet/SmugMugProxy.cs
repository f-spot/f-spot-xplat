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
	public class SmugMugProxy
	{
		// FIXME: this getting should be done over https
		const string GET_URL = "https://api.SmugMug.com/hack/rest/";
		const string POST_URL = "https://upload.SmugMug.com/hack/rest/";
		// key from massis
		const string APIKEY = "umtr0zB2wzwTZDhF2BySidg0hY0le3K6";
		const string VERSION = "1.1.1";

		// rest methods
		const string LOGIN_WITHPASS_METHOD = "smugmug.login.withPassword";
		const string LOGIN_WITHHASH_METHOD = "smugmug.login.withHash";
		const string LOGOUT_METHOD = "smugmug.logout";
		const string ALBUMS_CREATE_METHOD = "smugmug.albums.create";
		const string ALBUMS_GET_URLS_METHOD = "smugmug.images.getURLs";
		const string ALBUMS_GET_METHOD = "smugmug.albums.get";
		const string CATEGORIES_GET_METHOD = "smugmug.categories.get";

		// parameter constants
		const string EMAIL = "EmailAddress";
		const string PASSWORD = "Password";
		const string USER_ID = "UserID";
		const string PASSWORD_HASH = "PasswordHash";
		const string SESSION_ID = "SessionID";
		const string CATEGORY_ID = "CategoryID";
		const string IMAGE_ID = "ImageID";
		const string TITLE = "Title";
		const string ID = "id";

		public static Credentials LoginWithPassword (string username, string password)
		{
			string url = FormatGetUrl (LOGIN_WITHPASS_METHOD, new SmugMugParam (EMAIL, username), new SmugMugParam (PASSWORD, password));
			var doc = GetResponseXml (url);

			string sessionId = doc.SelectSingleNode ("/rsp/Login/SessionID").InnerText;
			int userId = int.Parse (doc.SelectSingleNode ("/rsp/Login/UserID").InnerText);
			string passwordHash = doc.SelectSingleNode ("/rsp/Login/PasswordHash").InnerText;

			return new Credentials (sessionId, userId, passwordHash);
		}

		public static string LoginWithHash (int user_id, string password_hash)
		{
			string url = FormatGetUrl (LOGIN_WITHHASH_METHOD, new SmugMugParam (USER_ID, user_id), new SmugMugParam (PASSWORD_HASH, password_hash));
			var doc = GetResponseXml(url);

			return doc.SelectSingleNode ("/rsp/Login/SessionID").InnerText;
		}

		public static void Logout (string session_id)
		{
			string url = FormatGetUrl (LOGOUT_METHOD, new SmugMugParam (SESSION_ID, session_id));
			GetResponseXml (url);
		}

		public static Album[] GetAlbums (string session_id)
		{
			string url = FormatGetUrl (ALBUMS_GET_METHOD, new SmugMugParam(SESSION_ID, session_id));
			var doc = GetResponseXml (url);
			var albumNodes = doc.SelectNodes ("/rsp/Albums/Album");

			var albums = new Album[albumNodes.Count];

			for (int i = 0; i < albumNodes.Count; i++)
			{
				var current = albumNodes[i];
				albums[i] = new Album (current.SelectSingleNode (TITLE).InnerText, int.Parse (current.Attributes[ID].Value));
			}
			return albums;
		}

		public static Uri GetAlbumUrl (int image_id, string session_id)
		{
			string url = FormatGetUrl(ALBUMS_GET_URLS_METHOD, new SmugMugParam(IMAGE_ID, image_id), new SmugMugParam(SESSION_ID, session_id));
			var doc = GetResponseXml(url);

			string album_url = doc.SelectSingleNode("/rsp/ImageURLs/Image/AlbumURL").InnerText;

			return new Uri(album_url);
		}

		public static Category[] GetCategories (string session_id)
		{
			string url = FormatGetUrl(CATEGORIES_GET_METHOD, new SmugMugParam (SESSION_ID, session_id));
			var doc = GetResponseXml (url);

			var categoryNodes = doc.SelectNodes ("/rsp/Categories/Category");
			var categories = new Category[categoryNodes.Count];

			for (int i = 0; i < categoryNodes.Count; i++)
			{
				var current = categoryNodes[i];
				categories[i] = new Category (current.SelectSingleNode (TITLE).InnerText, int.Parse (current.Attributes[ID].Value));
			}
			return categories;
		}

		public static Album CreateAlbum (string title, int category_id, string session_id)
		{
			return CreateAlbum (title, category_id, session_id, true);
		}

		public static Album CreateAlbum (string title, int category_id, string session_id, bool is_public)
		{
			int public_int = is_public ? 1 : 0;
			string url = FormatGetUrl (ALBUMS_CREATE_METHOD, new SmugMugParam (TITLE, title), new SmugMugParam (CATEGORY_ID, category_id), new SmugMugParam (SESSION_ID, session_id), new SmugMugParam ("Public", public_int));
			var doc = GetResponseXml (url);

			int id = int.Parse(doc.SelectSingleNode("/rsp/Create/Album").Attributes[ID].Value);

			return new Album(title, id);
		}

		public static int Upload (string path, int album_id, string session_id)
		{
			var file = new FileInfo(path);

			if (!file.Exists)
				throw new ArgumentException("Image does not exist: " + file.FullName);

			try
			{
				var client = new WebClient {
					BaseAddress = "http://upload.smugmug.com"
				};
				client.Headers.Add ("Cookie:SMSESS=" + session_id);

				var queryStringCollection = new NameValueCollection {
					{ "AlbumID", album_id.ToString () },
					// Temporarily disabled because rest doesn't seem to return the ImageID anymore
					// queryStringCollection.Add ("ResponseType", "REST");
					// luckily JSON still holds it
					{ "ResponseType", "JSON" }
				};
				client.QueryString = queryStringCollection;

				byte[] responseArray = client.UploadFile ("http://upload.smugmug.com/photos/xmladd.mg", "POST", file.FullName);
				string response = Encoding.ASCII.GetString (responseArray);

				// JSon approach
				var id_regex = new Regex ("\\\"id\\\":( )?(?<image_id>\\d+),");
				var m  = id_regex.Match (response);

				int id = -1;

				if (m.Success)
					id = int.Parse (m.Groups["image_id"].Value);

				return id;

				// REST approach, disabled for now
				//XmlDocument doc = new XmlDocument ();
				//doc.LoadXml (response);
				// return int.Parse (doc.SelectSingleNode ("/rsp/ImageID").InnerText);

			}
			catch (Exception ex)
			{
				throw new SmugMugUploadException ("Error uploading image: " + file.FullName, ex.InnerException);
			}
		}

		static string FormatGetUrl(string method_name, params SmugMugParam[] parameters)
		{
			var builder = new StringBuilder (string.Format ("{0}{1}/?method={2}", GET_URL, VERSION, method_name));

			foreach (var param in parameters)
				builder.Append (param.ToString ());

			builder.Append (new SmugMugParam ("APIKey", APIKEY));
			return builder.ToString();
		}

		static XmlDocument GetResponseXml (string url)
		{
			var request = HttpWebRequest.Create (url) as HttpWebRequest;
			request.Credentials = CredentialCache.DefaultCredentials;
			var response = request.GetResponse ();

			var doc = new XmlDocument ();
			doc.LoadXml (new StreamReader (response.GetResponseStream ()).ReadToEnd ());
			CheckResponseXml (doc);

			response.Close ();
			return doc;
		}

		static void CheckResponseXml (XmlDocument doc)
		{
			if (doc.SelectSingleNode("/rsp").Attributes["stat"].Value == "ok")
				return;

			string message = doc.SelectSingleNode ("/rsp/err").Attributes["msg"].Value;
			throw new SmugMugException (message);
		}

		class SmugMugParam
		{
			readonly object value;

			public SmugMugParam (string name, object value)
			{
				Name = name;
				this.value = (value is string ? System.Web.HttpUtility.UrlEncode ((string)value) : value);
			}

            public string Name { get; }

            public object Value
			{
				get {return value;}
			}

			public override string ToString()
			{
				return string.Format("&{0}={1}", Name, Value);
			}
		}
	}
}
