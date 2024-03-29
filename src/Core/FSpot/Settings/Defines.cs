//
// Defines.cs
// Get the locale directory
//
// Authors:
//	Martin Willemoes Hansen
//
// (C) 2004 Martin Willemoes Hansen
//

namespace FSpot.Settings
{
	public struct Defines
	{
		//public const string LOCALE_DIR = "@prefix@/share/locale";
		public const string VERSION = "@VERSION@";
		public const string PACKAGE = "@PACKAGE@";
		public const string PREFIX = "@prefix@";
		public const string APP_DATA_DIR = "@prefix@/share/@PACKAGE@";
		//public const string BINDIR = PREFIX + "/bin";
	}
}
