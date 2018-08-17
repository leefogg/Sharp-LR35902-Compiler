using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;

namespace Common_Tests
{
	[TestClass]
	public class CharExtensions
	{
		[TestMethod]
		public void IsLetter()
		{
			Assert.IsTrue('a'.IsLetter());
			Assert.IsTrue('A'.IsLetter());
			Assert.IsTrue('z'.IsLetter());
			Assert.IsTrue('Z'.IsLetter());
			Assert.IsFalse('`'.IsLetter());
			Assert.IsFalse('@'.IsLetter());
			Assert.IsFalse('['.IsLetter());
			Assert.IsFalse('}'.IsLetter());
		}

		[TestMethod]
		public void IsNumber()
		{
			Assert.IsTrue('0'.isNumber());
			Assert.IsTrue('9'.isNumber());
			Assert.IsFalse('/'.isNumber());
			Assert.IsFalse(':'.isNumber());
		}
	}
}
