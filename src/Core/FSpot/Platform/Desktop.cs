//
// Desktop.cs
//
// Author:
//   Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (C) 2008-2009 Novell, Inc.
// Copyright (C) 2008-2009 Stephane Delcroix
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

namespace FSpot.Platform
{
	public static class Desktop
	{
		public static void SetBackgroundImage (string path)
		{
			//GConf.Client client = new GConf.Client (); 
			//client.Set ("/desktop/gnome/background/color_shading_type", "solid");
			//client.Set ("/desktop/gnome/background/primary_color", "#000000");
			//client.Set ("/desktop/gnome/background/picture_options", "zoom");
			//client.Set ("/desktop/gnome/background/picture_opacity", 100);
			//client.Set ("/desktop/gnome/background/picture_filename", path);
			//client.Set ("/desktop/gnome/background/draw_background", true);
		}
	}
}
