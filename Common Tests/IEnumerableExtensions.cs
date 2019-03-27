using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Common.Extensions.IEnumerableExtensions;
using Common.Extensions;

namespace Common_Tests
{
	class TestClass {
		public int Prop { get; set; }
		public TestClass(int prop)
		{
			Prop = prop;
		}
	}

	[TestClass]
	public class IEnumerableExtensions
	{
		[TestMethod]
		public void ListOf_ToList() { ListOf('a', 'b'); }

		[TestMethod]
		public void IndexOf()
		{
			var target = new TestClass(3);
			var list = new[] {
				new TestClass(0),
				new TestClass(1),
				new TestClass(2),
				target
			};
			Assert.AreEqual(3, list.IndexOf(target));
		}

		[TestMethod]
		public void IndexOf_StringTemplate()
		{
			var target = "3";
			var list = new[] {
				"0",
				"1",
				"2",
				target
			};
			Assert.AreEqual(3, list.IndexOf(target));
		}
	}
}
