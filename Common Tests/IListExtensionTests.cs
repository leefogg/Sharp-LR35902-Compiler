using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;

namespace Common_Tests
{
	[TestClass]
	public class IListExtensionTests
	{
		[TestMethod]
		public void Get_InRange()
		{
			var array = new int[10];
			for (var i = 0; i < array.Length; i++)
				array[i] = i;

			var num = array.Get(1);

			Assert.AreEqual(num, 1);
		}

		[TestMethod]
		public void Get_OutRange()
		{
			var array = new int[10];
			for (var i = 0; i < array.Length; i++)
				array[i] = i;

			var num = array.Get(11);

			Assert.AreEqual(num, default(int));
		}

		[TestMethod]
		public void ToHumanList_EmptyList()
		{
			var names = new string[0];

			Assert.AreEqual(names.ToHumanList(), string.Empty);
		}

		[TestMethod]
		public void ToHumanList_SingleItem()
		{
			var names = new[] { "bob" };

			Assert.AreEqual(names.ToHumanList(), "bob");
		}

		[TestMethod]
		public void ToHumanList_MultipleItem()
		{
			var names = new[] { "bob", "alice" };

			Assert.AreEqual(names.ToHumanList(), "bob, alice");
		}
	}
}
