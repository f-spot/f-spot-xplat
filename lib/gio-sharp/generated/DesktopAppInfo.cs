// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class DesktopAppInfo : GLib.Object, AppInfo
	{

		[Obsolete]
		protected DesktopAppInfo(GLib.GType gtype) : base(gtype) {}
		public DesktopAppInfo(IntPtr raw) : base(raw) {}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_desktop_app_info_new(IntPtr desktop_id);

		public DesktopAppInfo (string desktop_id) : base (IntPtr.Zero)
		{
			if (GetType () != typeof (DesktopAppInfo)) {
				throw new InvalidOperationException ("Can't override this constructor.");
			}
			IntPtr native_desktop_id = GLib.Marshaller.StringToPtrGStrdup (desktop_id);
			Raw = g_desktop_app_info_new(native_desktop_id);
			GLib.Marshaller.Free (native_desktop_id);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_desktop_app_info_new_from_filename(IntPtr filename);

		public static DesktopAppInfo NewFromFilename(string filename)
		{
			IntPtr native_filename = GLib.Marshaller.StringToPtrGStrdup (filename);
			var result = new DesktopAppInfo (g_desktop_app_info_new_from_filename(native_filename));
			GLib.Marshaller.Free (native_filename);
			return result;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_desktop_app_info_set_desktop_env(IntPtr desktop_env);

		public static string DesktopEnv { 
			set {
				IntPtr native_value = GLib.Marshaller.StringToPtrGStrdup (value);
				g_desktop_app_info_set_desktop_env(native_value);
				GLib.Marshaller.Free (native_value);
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_desktop_app_info_get_is_hidden(IntPtr raw);

		public bool IsHidden { 
			get {
				bool raw_ret = g_desktop_app_info_get_is_hidden(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_desktop_app_info_get_type();

		public static new GLib.GType GType { 
			get {
				var raw_ret = g_desktop_app_info_get_type();
				var ret = new GLib.GType(raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_app_info_get_executable(IntPtr raw);

		public string Executable { 
			get {
				var raw_ret = g_app_info_get_executable(Handle);
				string ret = GLib.Marshaller.Utf8PtrToString (raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_app_info_get_name(IntPtr raw);

		public string Name { 
			get {
				var raw_ret = g_app_info_get_name(Handle);
				string ret = GLib.Marshaller.Utf8PtrToString (raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_app_info_dup(IntPtr raw);

		public AppInfo Dup () {
			var raw_ret = g_app_info_dup(Handle);
			var ret = GLib.AppInfoAdapter.GetObject (raw_ret, false);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_equal(IntPtr raw, IntPtr appinfo2);

		public bool Equal(AppInfo appinfo2) {
			bool raw_ret = g_app_info_equal(Handle, appinfo2 == null ? IntPtr.Zero : appinfo2.Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_set_as_default_for_type(IntPtr raw, IntPtr content_type, out IntPtr error);

		public bool SetAsDefaultForType(string content_type) {
			IntPtr native_content_type = GLib.Marshaller.StringToPtrGStrdup (content_type);
			var error = IntPtr.Zero;
			bool raw_ret = g_app_info_set_as_default_for_type(Handle, native_content_type, out error);
			bool ret = raw_ret;
			GLib.Marshaller.Free (native_content_type);
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_app_info_get_description(IntPtr raw);

		public string Description { 
			get {
				var raw_ret = g_app_info_get_description(Handle);
				string ret = GLib.Marshaller.Utf8PtrToString (raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_set_as_default_for_extension(IntPtr raw, IntPtr extension, out IntPtr error);

		public bool SetAsDefaultForExtension(string extension) {
			IntPtr native_extension = GLib.Marshaller.StringToPtrGStrdup (extension);
			var error = IntPtr.Zero;
			bool raw_ret = g_app_info_set_as_default_for_extension(Handle, native_extension, out error);
			bool ret = raw_ret;
			GLib.Marshaller.Free (native_extension);
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_remove_supports_type(IntPtr raw, IntPtr content_type, out IntPtr error);

		public bool RemoveSupportsType(string content_type) {
			IntPtr native_content_type = GLib.Marshaller.StringToPtrGStrdup (content_type);
			var error = IntPtr.Zero;
			bool raw_ret = g_app_info_remove_supports_type(Handle, native_content_type, out error);
			bool ret = raw_ret;
			GLib.Marshaller.Free (native_content_type);
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_launch_uris(IntPtr raw, IntPtr uris, IntPtr launch_context, out IntPtr error);

		public bool LaunchUris(GLib.List uris, AppLaunchContext launch_context) {
			var error = IntPtr.Zero;
			bool raw_ret = g_app_info_launch_uris(Handle, uris == null ? IntPtr.Zero : uris.Handle, launch_context == null ? IntPtr.Zero : launch_context.Handle, out error);
			bool ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_launch(IntPtr raw, IntPtr files, IntPtr launch_context, out IntPtr error);

		public bool Launch(GLib.List files, AppLaunchContext launch_context) {
			var error = IntPtr.Zero;
			bool raw_ret = g_app_info_launch(Handle, files == null ? IntPtr.Zero : files.Handle, launch_context == null ? IntPtr.Zero : launch_context.Handle, out error);
			bool ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_app_info_get_commandline(IntPtr raw);

		public string Commandline { 
			get {
				var raw_ret = g_app_info_get_commandline(Handle);
				string ret = GLib.Marshaller.Utf8PtrToString (raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_delete(IntPtr raw);

		public bool Delete() {
			bool raw_ret = g_app_info_delete(Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_app_info_get_id(IntPtr raw);

		public string Id { 
			get {
				var raw_ret = g_app_info_get_id(Handle);
				string ret = GLib.Marshaller.Utf8PtrToString (raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_app_info_get_icon(IntPtr raw);

		public Icon Icon { 
			get {
				var raw_ret = g_app_info_get_icon(Handle);
				var ret = GLib.IconAdapter.GetObject (raw_ret, false);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_can_delete(IntPtr raw);

		public bool CanDelete() {
			bool raw_ret = g_app_info_can_delete(Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_should_show(IntPtr raw);

		public bool ShouldShow { 
			get {
				bool raw_ret = g_app_info_should_show(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_can_remove_supports_type(IntPtr raw);

		public bool CanRemoveSupportsType { 
			get {
				bool raw_ret = g_app_info_can_remove_supports_type(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_add_supports_type(IntPtr raw, IntPtr content_type, out IntPtr error);

		public bool AddSupportsType(string content_type) {
			IntPtr native_content_type = GLib.Marshaller.StringToPtrGStrdup (content_type);
			var error = IntPtr.Zero;
			bool raw_ret = g_app_info_add_supports_type(Handle, native_content_type, out error);
			bool ret = raw_ret;
			GLib.Marshaller.Free (native_content_type);
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_supports_uris(IntPtr raw);

		public bool SupportsUris { 
			get {
				bool raw_ret = g_app_info_supports_uris(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_app_info_supports_files(IntPtr raw);

		public bool SupportsFiles { 
			get {
				bool raw_ret = g_app_info_supports_files(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

#endregion
	}
}
