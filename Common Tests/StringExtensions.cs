using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Extensions;
using System.Linq;
using static Test_Common.Utils;

namespace Common_Tests
{
	[TestClass]
	public class StringExtensions
	{
		[TestMethod]
		public void SplitAndKeep()
		{
			var output = "0x0".SplitAndKeep(new char[] { 'x' });

			ListEqual(
				new[]
				{
					"0",
					"x",
					"0"
				},
				output.ToArray()
			);
		}

		[TestMethod]
		public void Contains()
		{
			Assert.IsTrue("Hello there!".Contains('!'));
			Assert.IsFalse("Hello there!".Contains('#'));
		}
	}
}
