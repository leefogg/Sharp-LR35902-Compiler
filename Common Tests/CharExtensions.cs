using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			Assert.IsTrue('0'.IsNumber());
			Assert.IsTrue('9'.IsNumber());
			Assert.IsFalse('/'.IsNumber());
			Assert.IsFalse(':'.IsNumber());
		}
	}
}
