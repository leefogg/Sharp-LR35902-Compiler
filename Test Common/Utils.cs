using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Extensions;

namespace Test_Common
{
	public class Utils
	{
		public static void Is(byte[] result, byte onlybyte)
		{
			listEqual(IEnumerableExtensions.ListOf(onlybyte), result);
		}
		public static void Is(byte[] actual, params byte[] expected)
			=> listEqual(expected, actual);
		public static void StartsWith(byte[] expected, byte[] actual)
		{
			if (expected.Length > actual.Length)
				Assert.Fail("expected array is longer than actual array");

			for (var i = 0; i < expected.Length; i++)
				Assert.AreEqual(expected[i], actual[i]);
		}

		public static void listEqual<T>(T [] left, T[] right)
		{
			if (left.Length != right.Length)
				Assert.Fail("Lists do not match in length");

			for (int i = 0; i < left.Length; i++)
				Assert.AreEqual(left[i], right[i]);
		}
	}
}
