﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Extensions;

namespace Sharp_LR35902_Compiler_Tests
{
	public class Utils
	{
		public static void Is(byte[] result, byte onlybyte)
		{
			listEqual(IEnumerableExtensions.ListOf(onlybyte), result);
		}
		public static void Is(byte[] result, params byte[] bytes)
			=> listEqual(bytes, result);

		public static void listEqual(byte[] left, byte[] right)
		{
			if (left.Length != right.Length)
				Assert.Fail();

			for (int i = 0; i < left.Length; i++)
				Assert.AreEqual(left[i], right[i]);
		}
	}
}
