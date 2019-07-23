// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class FileMonitor : GLib.Object {

		[Obsolete]
		protected FileMonitor(GLib.GType gtype) : base(gtype) {}
		public FileMonitor(IntPtr raw) : base(raw) {}

		protected FileMonitor() : base(IntPtr.Zero)
		{
			CreateNativeObject (new string [0], new GLib.Value [0]);
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_file_monitor_set_rate_limit(IntPtr raw, int limit_msecs);

		[GLib.Property ("rate-limit")]
		public int RateLimit {
			get {
				GLib.Value val = GetProperty ("rate-limit");
				int ret = (int) val;
				val.Dispose ();
				return ret;
			}
			set  {
				g_file_monitor_set_rate_limit(Handle, value);
			}
		}

		[GLib.Property ("cancelled")]
		public bool Cancelled {
			get {
				GLib.Value val = GetProperty ("cancelled");
				bool ret = (bool) val;
				val.Dispose ();
				return ret;
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void ChangedVMDelegate (IntPtr monitor, IntPtr file, IntPtr other_file, int event_type);

		static ChangedVMDelegate ChangedVMCallback;

		static void changed_cb (IntPtr monitor, IntPtr file, IntPtr other_file, int event_type)
		{
			try {
				var monitor_managed = GLib.Object.GetObject (monitor, false) as FileMonitor;
				monitor_managed.OnChanged (GLib.FileAdapter.GetObject (file, false), GLib.FileAdapter.GetObject (other_file, false), (FileMonitorEvent) event_type);
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		static void OverrideChanged (GLib.GType gtype)
		{
			if (ChangedVMCallback == null)
				ChangedVMCallback = new ChangedVMDelegate (changed_cb);
			OverrideVirtualMethod (gtype, "changed", ChangedVMCallback);
		}

		[GLib.DefaultSignalHandler(Type=typeof(FileMonitor), ConnectionMethod="OverrideChanged")]
		protected virtual void OnChanged (File file, File other_file, FileMonitorEvent event_type)
		{
			GLib.Value ret = GLib.Value.Empty;
			var inst_and_params = new GLib.ValueArray (4);
			var vals = new GLib.Value [4];
			vals [0] = new GLib.Value (this);
			inst_and_params.Append (vals [0]);
			vals [1] = new GLib.Value (file);
			inst_and_params.Append (vals [1]);
			vals [2] = new GLib.Value (other_file);
			inst_and_params.Append (vals [2]);
			vals [3] = new GLib.Value (event_type);
			inst_and_params.Append (vals [3]);
			g_signal_chain_from_overridden (inst_and_params.ArrayPtr, ref ret);
			foreach (GLib.Value v in vals)
				v.Dispose ();
		}

		[GLib.Signal("changed")]
		public event ChangedHandler Changed {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (this, "changed", typeof (ChangedArgs));
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (this, "changed", typeof (ChangedArgs));
				sig.RemoveDelegate (value);
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern IntPtr g_file_monitor_get_type();

		public static new GLib.GType GType { 
			get {
				var raw_ret = g_file_monitor_get_type();
				var ret = new GLib.GType(raw_ret);
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern unsafe IntPtr g_file_monitor_file(IntPtr file, int flags, IntPtr cancellable, out IntPtr error);

		public static unsafe FileMonitor File (File file, FileMonitorFlags flags, Cancellable cancellable) {
			var error = IntPtr.Zero;
			var raw_ret = g_file_monitor_file(file == null ? IntPtr.Zero : file.Handle, (int) flags, cancellable == null ? IntPtr.Zero : cancellable.Handle, out error);
			var ret = GLib.Object.GetObject (raw_ret) as FileMonitor;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_file_monitor_is_cancelled(IntPtr raw);

		public bool IsCancelled { 
			get {
				bool raw_ret = g_file_monitor_is_cancelled(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern unsafe IntPtr g_file_monitor_directory(IntPtr file, int flags, IntPtr cancellable, out IntPtr error);

		public static unsafe FileMonitor Directory (File file, FileMonitorFlags flags, Cancellable cancellable) {
			var error = IntPtr.Zero;
			var raw_ret = g_file_monitor_directory(file == null ? IntPtr.Zero : file.Handle, (int) flags, cancellable == null ? IntPtr.Zero : cancellable.Handle, out error);
			var ret = GLib.Object.GetObject (raw_ret) as FileMonitor;
			if (error != IntPtr.Zero) throw new GLib.GException (error);
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern bool g_file_monitor_cancel(IntPtr raw);

		public bool Cancel() {
			bool raw_ret = g_file_monitor_cancel(Handle);
			bool ret = raw_ret;
			return ret;
		}

		[DllImport("libgio-2.0-0.dll")]
		static extern void g_file_monitor_emit_event(IntPtr raw, IntPtr child, IntPtr other_file, int event_type);

		public void EmitEvent(File child, File other_file, FileMonitorEvent event_type) {
			g_file_monitor_emit_event(Handle, child == null ? IntPtr.Zero : child.Handle, other_file == null ? IntPtr.Zero : other_file.Handle, (int) event_type);
		}

#endregion
	}
}