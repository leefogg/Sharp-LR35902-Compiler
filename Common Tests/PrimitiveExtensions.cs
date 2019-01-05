using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Extensions;
using static Test_Common.Utils;
using System;

namespace Common_Tests
{
	[TestClass]
	public class PrimitiveExtensions
	{
		[TestMethod]
		public void IsByte()
		{
			Assert.IsTrue(((ushort)0).isByte());
			Assert.IsTrue(((ushort)10).isByte());
			Assert.IsTrue(((ushort)255).isByte());
			Assert.IsFalse(((ushort)256).isByte());
		}

		[TestMethod]
		public void ToByteArray_Endianness()
		{
			var bytes = ((ushort)0x1DFF).ToByteArray();
			Assert.AreEqual(bytes[0], 0xFF);
			Assert.AreEqual(bytes[1], 0x1D);
		}

		[TestMethod]
		public void StartsWith_Empty()
		{
			var text = string.Empty;
			Assert.IsFalse(text.StartsWith('1'));
		}

		[TestMethod]
		public void StartsWith()
		{
			Assert.IsTrue("1234".StartsWith('1'));
		}

		[TestMethod]
		public void EndsWith_Empty()
		{
			var text = string.Empty;
			Assert.IsFalse(text.EndsWith('1'));
		}

		[TestMethod]
		public void EndsWith()
		{
			Assert.IsTrue("1234".EndsWith('4'));
		}

		[TestMethod]
		public void GetHexBytes()
		{
			ListEqual(
				new byte[]
				{
					0xFF,
					0x1D
				},
				"1DFF".GetHexBytes()
			);
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void GetHexBytes_NotWholeByte()
		{
			"111".GetHexBytes();
		}
	}
}
