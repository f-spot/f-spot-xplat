/*
 * SmugMugApi.cs
 *
 * Authors:
 *   Thomas Van Machelen <thomas.vanmachelen@gmail.com>
 *
 * Copyright (C) 2006 Thomas Van Machelen
 * This is free software. See COPYING for details.
 *
 */

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
	public class SmugMugApi
	{
		readonly string username = string.Empty;
		readonly string password = string.Empty;
		Credentials credentials;
		const string VERSION = "1.1.1";
		Category[] categories;

        public bool Connected { get; private set; } = false;

		public SmugMugApi (string email_address, string password)
		{
			username = email_address;
			this.password = password;
		}

		public bool Login ()
		{
			if (username.Length == 0 | password.Length == 0)
			{
				throw new SmugMugException("There is no username or password.");
			}

			if (Connected == false && credentials.UserID == 0)
			{
				try
				{
					credentials = SmugMugProxy.LoginWithPassword (username, password);
					Connected = true;
				}
				catch
				{
					return false;
				}
			}
			else
			{
				LoginWithHash ();
			}

			return true;
		}

		void LoginWithHash ()
		{
			try {
				string session_id = SmugMugProxy.LoginWithHash (credentials.UserID, credentials.PasswordHash);

				if (session_id != null && session_id.Length > 0)
				{
					credentials.SessionID = session_id;
				}
				else
				{
					throw new SmugMugException ("SessionID was empty");
				}
			}
			catch (Exception ex) {
				throw new SmugMugException ("A login error occurred, SessionID may be invalid.", ex.InnerException);
			}
		}

		public void Logout ()
		{
			if (!Connected)
				return;

			if (credentials.SessionID == null && credentials.SessionID.Length == 0)
				return;

			SmugMugProxy.Logout (credentials.SessionID);
			Connected = false;
			credentials = new Credentials (null, 0, null);
		}

		public Category[] GetCategories ()
		{
			if (categories == null)
			{
				try {
					categories = SmugMugProxy.GetCategories (credentials.SessionID);
				}
				catch (Exception ex) {
					throw new SmugMugException ("Could not retrieve Categories", ex.InnerException);
				}
			}
			return categories;
		}

		public Album CreateAlbum (string title, int category_id, bool is_public)
		{
			try {
				return SmugMugProxy.CreateAlbum (title, category_id, credentials.SessionID, is_public);
			}
			catch (Exception ex) {
				throw new SmugMugException ("Could not create album", ex.InnerException);
			}
		}

		public Album[] GetAlbums ()
		{
			try {
				return SmugMugProxy.GetAlbums(credentials.SessionID);
			}
			catch (Exception ex) {
				throw new SmugMugException ("Could not get albums", ex.InnerException);
			}
		}

		public Uri GetAlbumUrl (int image_id)
		{
			try {
				return SmugMugProxy.GetAlbumUrl (image_id, credentials.SessionID);
			}
			catch (Exception ex) {
				throw new SmugMugException ("Could not get album url", ex.InnerException);
			}
		}

		public int Upload (string path, int album_id)
		{
			try {
				return SmugMugProxy.Upload (path, album_id, credentials.SessionID);
			}
			catch (Exception ex) {
				throw new SmugMugException ("Could not upload file", ex.InnerException);
			}
		}
	}
}
