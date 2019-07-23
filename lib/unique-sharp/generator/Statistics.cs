// Statistics.cs : Generation statistics class implementation
//
// Author: Mike Kestner  <mkestner@ximian.com>
//
// Copyright (c) 2002 Mike Kestner
// Copyright (c) 2004 Novell, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the GNU General Public
// License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
// Boston, MA 02110-1301


namespace GtkSharp.Generation {
	
	using System;
	using System.Collections;
	
	public class Statistics {
		static int ctors = 0;
		static int ignored = 0;
		static bool vm_ignored = false;

        public static int CBCount { get; set; } = 0;

        public static int EnumCount { get; set; } = 0;

        public static int ObjectCount { get; set; } = 0;

        public static int StructCount { get; set; } = 0;

        public static int BoxedCount { get; set; } = 0;

        public static int OpaqueCount { get; set; } = 0;

		public static int CtorCount {
			get {
				return ctors;
			}
			set {
				ctors = value;
			}
		}

        public static int MethodCount { get; set; } = 0;

        public static int PropCount { get; set; } = 0;

        public static int SignalCount { get; set; } = 0;

        public static int IFaceCount { get; set; } = 0;

        public static int ThrottledCount { get; set; } = 0;

		public static int IgnoreCount {
			get {
				return ignored;
			}
			set {
				ignored = value;
			}
		}
		
		public static bool VMIgnored {
			get {
				return vm_ignored;
			}
			set {
				if (value)
					vm_ignored = value;
			}
		}
		
		public static void Report()
		{
			if (VMIgnored) {
				Console.WriteLine();
				Console.WriteLine("Warning: Generation throttled for Virtual Methods.");
				Console.WriteLine("  Consider regenerating with --gluelib-name and --glue-filename.");
			}
			Console.WriteLine();
			Console.WriteLine("Generation Summary:");
			Console.Write("  Enums: " + EnumCount);
			Console.Write("  Structs: " + StructCount);
			Console.Write("  Boxed: " + BoxedCount);
			Console.Write("  Opaques: " + OpaqueCount);
			Console.Write("  Interfaces: " + IFaceCount);
			Console.Write("  Objects: " + ObjectCount);
			Console.WriteLine("  Callbacks: " + CBCount);
			Console.Write("  Properties: " + PropCount);
			Console.Write("  Signals: " + SignalCount);
			Console.Write("  Methods: " + MethodCount);
			Console.Write("  Constructors: " + ctors);
			Console.WriteLine("  Throttled: " + ThrottledCount);
			Console.WriteLine("Total Nodes: " + (EnumCount+StructCount+BoxedCount+OpaqueCount+IFaceCount+CBCount+ObjectCount+PropCount+SignalCount+MethodCount+ctors+ThrottledCount));
			Console.WriteLine();
		}
	}
}
