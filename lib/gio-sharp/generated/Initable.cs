// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;

#region Autogenerated code
	public interface Initable : GLib.IWrapper {

		bool Init(Cancellable cancellable);
	}

	[GLib.GInterface (typeof (InitableAdapter))]
	public interface InitableImplementor : GLib.IWrapper {

		bool Init (Cancellable cancellable);
	}
#endregion
}
