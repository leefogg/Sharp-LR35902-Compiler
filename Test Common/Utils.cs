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
		public static void Is(byte[] result, params byte[] bytes)
			=> listEqual(bytes, result);

		public static void listEqual<T>(T [] left, T[] right)
		{
			if (left.Length != right.Length)
				Assert.Fail();

			for (int i = 0; i < left.Length; i++)
				Assert.AreEqual(left[i], right[i]);
		}
	}
}
