// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class DriveAdapter : GLib.GInterfaceAdapter, Drive
	{

		public DriveAdapter (IntPtr handle)
		{
			this.handle = handle;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_drive_get_type();

		static GLib.GType _gtype = new GLib.GType (g_drive_get_type ());

		public override GLib.GType GType {
			get {
				return _gtype;
			}
		}

		readonly IntPtr handle;
		public override IntPtr Handle {
			get {
				return handle;
			}
		}

		public static Drive GetObject (IntPtr handle, bool owned)
		{
			GLib.Object obj = GLib.Object.GetObject (handle, owned);
			return GetObject (obj);
		}

		public static Drive GetObject (GLib.Object obj)
		{
			if (obj == null)
				return null;
			else if (obj as Drive == null)
				return new DriveAdapter (obj.Handle);
			else
				return obj as Drive;
		}

		[GLib.Signal("changed")]
		public event EventHandler Changed {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "changed");
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "changed");
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("eject-button")]
		public event EventHandler EjectButton {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "eject-button");
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "eject-button");
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("disconnected")]
		public event EventHandler Disconnected {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "disconnected");
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "disconnected");
				sig.RemoveDelegate (value);
			}
		}

		[GLib.Signal("stop-button")]
		public event EventHandler StopButton {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "stop-button");
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (GLib.Object.GetObject (Handle), "stop-button");
				sig.RemoveDelegate (value);
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_drive_stop(IntPtr raw, int flags, IntPtr mount_operation, IntPtr cancellable, GLibSharp.AsyncReadyCallbackNative cb, IntPtr user_data);

		public void Stop(MountUnmountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb) {
			var cb_wrapper = new GLibSharp.AsyncReadyCallbackWrapper (cb);
			cb_wrapper.PersistUntilCalled ();
			g_drive_stop(Handle, (int) flags, mount_operation == null ? IntPtr.Zero : mount_operation.Handle, cancellable == null ? IntPtr.Zero : cancellable.Handle, cb_wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_drive_poll_for_media(IntPtr raw, IntPtr cancellable, GLibSharp.AsyncReadyCallbackNative cb, IntPtr user_data);

		public void PollForMedia(Cancellable cancellable, AsyncReadyCallback cb) {
			var cb_wrapper = new GLibSharp.AsyncReadyCallbackWrapper (cb);
			cb_wrapper.PersistUntilCalled ();
			g_drive_poll_for_media(Handle, cancellable == null ? IntPtr.Zero : cancellable.Handle, cb_wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_start_finish(IntPtr raw, IntPtr result, out IntPtr error);

		public bool StartFinish(AsyncResult result) {
			var error = IntPtr.Zero;
			bool raw_ret = g_drive_start_finish(Handle, result == null ? IntPtr.Zero : result.Handle, out error);
			bool ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_drive_get_icon(IntPtr raw);

		public Icon Icon { 
			get {
				var raw_ret = g_drive_get_icon(Handle);
				var ret = GLib.IconAdapter.GetObject (raw_ret, false);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_has_media(IntPtr raw);

		public bool HasMedia { 
			get {
				bool raw_ret = g_drive_has_media(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_can_eject(IntPtr raw);

		public bool CanEject() {
			bool raw_ret = g_drive_can_eject(Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_drive_start(IntPtr raw, int flags, IntPtr mount_operation, IntPtr cancellable, GLibSharp.AsyncReadyCallbackNative cb, IntPtr user_data);

		public void Start(DriveStartFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb) {
			var cb_wrapper = new GLibSharp.AsyncReadyCallbackWrapper (cb);
			cb_wrapper.PersistUntilCalled ();
			g_drive_start(Handle, (int) flags, mount_operation == null ? IntPtr.Zero : mount_operation.Handle, cancellable == null ? IntPtr.Zero : cancellable.Handle, cb_wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_drive_enumerate_identifiers(IntPtr raw);

		public string EnumerateIdentifiers() {
			var raw_ret = g_drive_enumerate_identifiers(Handle);
			string ret = GLib.Marshaller.PtrToStringGFree(raw_ret);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_drive_get_volumes(IntPtr raw);

		public GLib.List Volumes { 
			get {
				var raw_ret = g_drive_get_volumes(Handle);
				var ret = new GLib.List(raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_is_media_check_automatic(IntPtr raw);

		public bool IsMediaCheckAutomatic { 
			get {
				bool raw_ret = g_drive_is_media_check_automatic(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_drive_eject_with_operation(IntPtr raw, int flags, IntPtr mount_operation, IntPtr cancellable, GLibSharp.AsyncReadyCallbackNative cb, IntPtr user_data);

		public void EjectWithOperation(MountUnmountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb) {
			var cb_wrapper = new GLibSharp.AsyncReadyCallbackWrapper (cb);
			cb_wrapper.PersistUntilCalled ();
			g_drive_eject_with_operation(Handle, (int) flags, mount_operation == null ? IntPtr.Zero : mount_operation.Handle, cancellable == null ? IntPtr.Zero : cancellable.Handle, cb_wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_can_start_degraded(IntPtr raw);

		public bool CanStartDegraded() {
			bool raw_ret = g_drive_can_start_degraded(Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern int g_drive_get_start_stop_type(IntPtr raw);

		public DriveStartStopType StartStopType { 
			get {
				int raw_ret = g_drive_get_start_stop_type(Handle);
				var ret = (DriveStartStopType) raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_poll_for_media_finish(IntPtr raw, IntPtr result, out IntPtr error);

		public bool PollForMediaFinish(AsyncResult result) {
			var error = IntPtr.Zero;
			bool raw_ret = g_drive_poll_for_media_finish(Handle, result == null ? IntPtr.Zero : result.Handle, out error);
			bool ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_stop_finish(IntPtr raw, IntPtr result, out IntPtr error);

		public bool StopFinish(AsyncResult result) {
			var error = IntPtr.Zero;
			bool raw_ret = g_drive_stop_finish(Handle, result == null ? IntPtr.Zero : result.Handle, out error);
			bool ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_drive_eject(IntPtr raw, int flags, IntPtr cancellable, GLibSharp.AsyncReadyCallbackNative cb, IntPtr user_data);

		[Obsolete]
		public void Eject(MountUnmountFlags flags, Cancellable cancellable, AsyncReadyCallback cb) {
			var cb_wrapper = new GLibSharp.AsyncReadyCallbackWrapper (cb);
			cb_wrapper.PersistUntilCalled ();
			g_drive_eject(Handle, (int) flags, cancellable == null ? IntPtr.Zero : cancellable.Handle, cb_wrapper.NativeDelegate, IntPtr.Zero);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_drive_get_name(IntPtr raw);

		public string Name { 
			get {
				var raw_ret = g_drive_get_name(Handle);
				string ret = GLib.Marshaller.PtrToStringGFree(raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_drive_get_identifier(IntPtr raw, IntPtr kind);

		public string GetIdentifier(string kind) {
			IntPtr native_kind = GLib.Marshaller.StringToPtrGStrdup (kind);
			var raw_ret = g_drive_get_identifier(Handle, native_kind);
			string ret = GLib.Marshaller.PtrToStringGFree(raw_ret);
			GLib.Marshaller.Free (native_kind);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_is_media_removable(IntPtr raw);

		public bool IsMediaRemovable { 
			get {
				bool raw_ret = g_drive_is_media_removable(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_has_volumes(IntPtr raw);

		public bool HasVolumes { 
			get {
				bool raw_ret = g_drive_has_volumes(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_eject_with_operation_finish(IntPtr raw, IntPtr result, out IntPtr error);

		public bool EjectWithOperationFinish(AsyncResult result) {
			var error = IntPtr.Zero;
			bool raw_ret = g_drive_eject_with_operation_finish(Handle, result == null ? IntPtr.Zero : result.Handle, out error);
			bool ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_can_stop(IntPtr raw);

		public bool CanStop() {
			bool raw_ret = g_drive_can_stop(Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_can_start(IntPtr raw);

		public bool CanStart() {
			bool raw_ret = g_drive_can_start(Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_can_poll_for_media(IntPtr raw);

		public bool CanPollForMedia() {
			bool raw_ret = g_drive_can_poll_for_media(Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_drive_eject_finish(IntPtr raw, IntPtr result, out IntPtr error);

		[Obsolete]
		public bool EjectFinish(AsyncResult result) {
			var error = IntPtr.Zero;
			bool raw_ret = g_drive_eject_finish(Handle, result == null ? IntPtr.Zero : result.Handle, out error);
			bool ret = raw_ret;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

#endregion
	}
}
