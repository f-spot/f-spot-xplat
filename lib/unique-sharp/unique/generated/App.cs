// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace Unique {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class App : GLib.Object {

		[Obsolete]
		protected App(GLib.GType gtype) : base(gtype) {}
		public App(IntPtr raw) : base(raw) {}

		[DllImport("libunique-1.0-0.dll")]
		static extern IntPtr unique_app_new(IntPtr name, IntPtr startup_id);

		public App (string name, string startup_id) : base (IntPtr.Zero)
		{
			if (GetType () != typeof (App)) {
				var vals = new ArrayList();
				var names = new ArrayList {
					"name"
				};
				vals.Add (new GLib.Value (name));
				names.Add ("startup_id");
				vals.Add (new GLib.Value (startup_id));
				CreateNativeObject ((string[])names.ToArray (typeof (string)), (GLib.Value[])vals.ToArray (typeof (GLib.Value)));
				return;
			}
			IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup (name);
			IntPtr native_startup_id = GLib.Marshaller.StringToPtrGStrdup (startup_id);
			Raw = unique_app_new(native_name, native_startup_id);
			GLib.Marshaller.Free (native_name);
			GLib.Marshaller.Free (native_startup_id);
		}

		[DllImport("libunique-1.0-0.dll")]
		static extern bool unique_app_is_running(IntPtr raw);

		[GLib.Property ("is-running")]
		public bool IsRunning {
			get  {
				bool raw_ret = unique_app_is_running(Handle);
				bool ret = raw_ret;
				return ret;
			}
		}

		[GLib.Property ("startup-id")]
		public string StartupId {
			get {
				GLib.Value val = GetProperty ("startup-id");
				string ret = (string) val;
				val.Dispose ();
				return ret;
			}
		}

		[GLib.Property ("name")]
		public string Name {
			get {
				GLib.Value val = GetProperty ("name");
				string ret = (string) val;
				val.Dispose ();
				return ret;
			}
		}

		[DllImport("libunique-1.0-0.dll")]
		static extern void unique_app_add_command(IntPtr raw, IntPtr command_name, int command_id);

		public void AddCommand(string command_name, int command_id) {
			IntPtr native_command_name = GLib.Marshaller.StringToPtrGStrdup (command_name);
			unique_app_add_command(Handle, native_command_name, command_id);
			GLib.Marshaller.Free (native_command_name);
		}

		[DllImport("libunique-1.0-0.dll")]
		static extern int unique_app_send_message(IntPtr raw, int command_id, IntPtr message_data);

		public Unique.Response SendMessage(Unique.Command command_id, Unique.MessageData message_data) {
			int raw_ret = unique_app_send_message(Handle, (int) command_id, message_data == null ? IntPtr.Zero : message_data.Handle);
			var ret = (Unique.Response) raw_ret;
			return ret;
		}

		[DllImport("libunique-1.0-0.dll")]
		static extern IntPtr unique_app_get_type();

		public static new GLib.GType GType { 
			get {
				IntPtr raw_ret = unique_app_get_type();
				var ret = new GLib.GType(raw_ret);
				return ret;
			}
		}

#endregion
#region Customized extensions
#line 1 "App.custom"
// App.custom - customization for App.cs
//
// Author(s):
//	Stephane Delcroix  <stephane@delcroix.org>
//
// Copyright (c) 2009 Stephane Delcroix
//
// This is open source software. See COPYING for details.
//

		public App (string name, string startup_id, params object [] commands) : this (name, startup_id)
		{
			for (int i = 0; i < commands.Length; i+=2)
				AddCommand (commands[i] as string, (int)commands[i+1]);
		}

		[GLib.CDeclCallback]
		delegate int MessageReceivedVMDelegate (IntPtr app, int command, IntPtr message_data, uint time_);

		static MessageReceivedVMDelegate MessageReceivedVMCallback;

		static int messagereceived_cb (IntPtr app, int command, IntPtr message_data, uint time_)
		{
			try {
				var app_managed = GLib.Object.GetObject (app, false) as App;
				var raw_ret = app_managed.OnMessageReceived (command, message_data == IntPtr.Zero ? null : (MessageData) GLib.Opaque.GetOpaque (message_data, typeof (MessageData), false), time_);
				return (int) raw_ret;
			} catch (Exception e) {
				GLib.ExceptionManager.RaiseUnhandledException (e, true);
				// NOTREACHED: above call doesn't return
				throw e;
			}
		}

		static void OverrideMessageReceived (GLib.GType gtype)
		{
			if (MessageReceivedVMCallback == null)
				MessageReceivedVMCallback = new MessageReceivedVMDelegate (messagereceived_cb);
			OverrideVirtualMethod (gtype, "message-received", MessageReceivedVMCallback);
		}

		[GLib.DefaultSignalHandler(Type=typeof(App), ConnectionMethod="OverrideMessageReceived")]
		protected virtual Response OnMessageReceived (int command, MessageData message_data, uint time_)
		{
			var ret = new GLib.Value (Unique.ResponseGType.GType);
			var inst_and_params = new GLib.ValueArray (4);
			var vals = new GLib.Value [4];
			vals [0] = new GLib.Value (this);
			inst_and_params.Append (vals [0]);
			vals [1] = new GLib.Value (command);
			inst_and_params.Append (vals [1]);
			vals [2] = new GLib.Value (message_data);
			inst_and_params.Append (vals [2]);
			vals [3] = new GLib.Value (time_);
			inst_and_params.Append (vals [3]);
			g_signal_chain_from_overridden (inst_and_params.ArrayPtr, ref ret);
			foreach (GLib.Value v in vals)
				v.Dispose ();
			var result = (Response) (Enum) ret;
			ret.Dispose ();
			return result;
		}

		[GLib.Signal("message-received")]
		event MessageReceivedHandler InternalMessageReceived {
			add {
				GLib.Signal sig = GLib.Signal.Lookup (this, "message-received", typeof (MessageReceivedArgs));
				sig.AddDelegate (value);
			}
			remove {
				GLib.Signal sig = GLib.Signal.Lookup (this, "message-received", typeof (MessageReceivedArgs));
				sig.RemoveDelegate (value);
			}
		}

		MessageReceivedHandler received_handler;

		public event MessageReceivedHandler MessageReceived {
			add {
				if (received_handler == null)
					InternalMessageReceived += MessageReceivedWrapper;
				received_handler = (MessageReceivedHandler)Delegate.Combine (received_handler, value);
			}
			remove {
				received_handler = (MessageReceivedHandler)Delegate.Remove (received_handler, value);
				if (received_handler == null)
					InternalMessageReceived -= MessageReceivedWrapper;
			}
		}

		[GLib.ConnectBefore]
		void MessageReceivedWrapper (object sender, MessageReceivedArgs e)
		{
			var eh = received_handler;
			if (eh == null)
				return;
			foreach (MessageReceivedHandler d in eh.GetInvocationList ()) {
				if (e.RetVal != null && (Response)e.RetVal != Response.Passthrough)
					break;
				d (sender, e);
			}
		}



#endregion
	}
}
