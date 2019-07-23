using System.IO;

using NUnit.Framework;

namespace FSpot.Tests
{
	public static class TestHelpers
	{
		public static string TestData => Path.Combine (TestContext.CurrentContext.TestDirectory, "TestData");
	}
}
