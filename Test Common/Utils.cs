using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Extensions;
using System.Collections.Generic;

namespace Test_Common
{
	public class Utils
	{
		public static void Is(byte[] result, byte onlybyte)
		{
			ListEqual(IEnumerableExtensions.ListOf(onlybyte), result);
		}
		public static void Is(byte[] actual, params byte[] expected)
			=> ListEqual(expected, actual);
		public static void StartsWith(byte[] expected, byte[] actual)
		{
			if (expected.Length > actual.Length)
				Assert.Fail("expected array is longer than actual array");

			for (var i = 0; i < expected.Length; i++)
				Assert.AreEqual(expected[i], actual[i]);
		}

		public static void ListEqual<T>(IList<T> expected, IList<T> actual)
		{
			if (expected.Count != actual.Count)
				Assert.Fail("Lists do not match in length");

			for (int i = 0; i < expected.Count; i++)
				Assert.AreEqual(expected[i], actual[i]);
		}
	}
}
