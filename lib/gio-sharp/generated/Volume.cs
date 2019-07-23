// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;

#region Autogenerated code
	public interface Volume : GLib.IWrapper {

		event EventHandler Changed;
		event EventHandler Removed;
		Icon Icon { 
			get;
		}
		bool MountFinish(AsyncResult result);
		bool CanEject();
		string EnumerateIdentifiers();
		void EjectWithOperation(MountUnmountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb);
		void Mount(MountMountFlags flags, MountOperation mount_operation, Cancellable cancellable, AsyncReadyCallback cb);
		void Eject(MountUnmountFlags flags, Cancellable cancellable, AsyncReadyCallback cb);
		string Name { 
			get;
		}
		Drive Drive { 
			get;
		}
		string GetIdentifier(string kind);
		bool ShouldAutomount();
		bool CanMount();
		File ActivationRoot { 
			get;
		}
		Mount MountInstance { 
			get;
		}
		bool EjectWithOperationFinish(AsyncResult result);
		string Uuid { 
			get;
		}
		bool EjectFinish(AsyncResult result);
	}
#endregion
}