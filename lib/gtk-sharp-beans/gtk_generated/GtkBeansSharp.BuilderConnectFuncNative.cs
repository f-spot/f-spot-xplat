// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GtkBeansSharp {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
delegate void BuilderConnectFuncNative(IntPtr builder, IntPtr objekt, IntPtr signal_name, IntPtr handler_name, IntPtr connect_object, int flags, IntPtr user_data);

	class BuilderConnectFuncInvoker {
		readonly BuilderConnectFuncNative native_cb;
		readonly IntPtr __data;
		GLib.DestroyNotify __notify;

		~BuilderConnectFuncInvoker ()
		{
			if (__notify == null)
				return;
			__notify (__data);
		}

		internal BuilderConnectFuncInvoker (BuilderConnectFuncNative native_cb) : this (native_cb, IntPtr.Zero, null) {}

		internal BuilderConnectFuncInvoker (BuilderConnectFuncNative native_cb, IntPtr data) : this (native_cb, data, null) {}

		internal BuilderConnectFuncInvoker (BuilderConnectFuncNative native_cb, IntPtr data, GLib.DestroyNotify notify)
		{
			this.native_cb = native_cb;
			__data = data;
			__notify = notify;
		}

		internal GtkBeans.BuilderConnectFunc Handler {
			get {
				return new GtkBeans.BuilderConnectFunc(InvokeNative);
			}
		}

		void InvokeNative (GtkBeans.Builder builder, GLib.Object objekt, string signal_name, string handler_name, GLib.Object connect_object, GLib.ConnectFlags flags)
		{
			IntPtr native_signal_name = GLib.Marshaller.StringToPtrGStrdup (signal_name);
			IntPtr native_handler_name = GLib.Marshaller.StringToPtrGStrdup (handler_name);
			native_cb (builder == null ? IntPtr.Zero : builder.Handle, objekt == null ? IntPtr.Zero : objekt.Handle, native_signal_name, native_handler_name, connect_object == null ? IntPtr.Zero : connect_object.Handle, (int) flags, __data);
			GLib.Marshaller.Free (native_signal_name);
			GLib.Marshaller.Free (native_handler_name);
		}
	}

	class BuilderConnectFuncWrapper {

		public void NativeCallback (IntPtr builder, IntPtr objekt, IntPtr signal_name, IntPtr handler_name, IntPtr connect_object, int flags, IntPtr user_data)
		{
			try {
				managed (GLib.Object.GetObject(builder) as GtkBeans.Builder, GLib.Object.GetObject (objekt), GLib.Marshaller.Utf8PtrToString (signal_name), GLib.Marshaller.Utf8PtrToString (handler_name), GLib.Object.GetObject (connect_object), (GLib.ConnectFlags) flags);
				if (release_on_call)
					gch.Free ();
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		bool release_on_call = false;
		GCHandle gch;

		public void PersistUntilCalled ()
		{
			release_on_call = true;
			gch = GCHandle.Alloc (this);
		}

		internal BuilderConnectFuncNative NativeDelegate;
		readonly GtkBeans.BuilderConnectFunc managed;

		public BuilderConnectFuncWrapper (GtkBeans.BuilderConnectFunc managed)
		{
			this.managed = managed;
			if (managed != null)
				NativeDelegate = new BuilderConnectFuncNative (NativeCallback);
		}

		public static GtkBeans.BuilderConnectFunc GetManagedDelegate (BuilderConnectFuncNative native)
		{
			if (native == null)
				return null;
			var wrapper = (BuilderConnectFuncWrapper) native.Target;
			if (wrapper == null)
				return null;
			return wrapper.managed;
		}
	}
#endregion
}
