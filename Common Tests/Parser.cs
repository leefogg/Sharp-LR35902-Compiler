using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Common.Parser;

namespace Common_Tests
{
	[TestClass]
	public class Parser
	{
		[TestMethod]
		public void ParseImmediate_BasicNumber()
		{
			var num = 123;

			Assert.AreEqual(num, ParseImmediate(num.ToString()));
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseImmediate_HexNumber_0x_NoData()
		{
			ParseImmediate("0x");
		}

		[TestMethod]
		public void ParseImmediate_HexNumber_0x_byte()
		{
			var num = 0x11;

			Assert.AreEqual(num, ParseImmediate("0x11"));
		}

		[TestMethod]
		public void ParseImmediate_HexNumber_0x_uint16()
		{
			var num = 0x1111;

			Assert.AreEqual(num, ParseImmediate("0x1111"));
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseImmediate_HexNumber_0x_InvalidLength()
		{
			ParseImmediate("0x111");
		}

		[TestMethod]
		public void ParseImmediate_BinaryNumber_8bit()
		{
			var num = 0b11100111;

			Assert.AreEqual(num, ParseImmediate("0b11100111"));
		}
		[TestMethod]
		public void ParseImmediate_BinaryNumber_16bit()
		{
			var num = 0b1110011111100111;

			Assert.AreEqual(num, ParseImmediate("0b1110011111100111"));
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseImmediate_BinaryNumber_UnknownDigit()
		{
			ParseImmediate("0b11100x11");
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseImmediate_BinaryNumber_WrongLength()
		{
			ParseImmediate("0b111001111");
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseImmediate_UnknownFormat()
		{
			ParseImmediate("x");
		}

		[TestMethod]
		public void ParseImmediate_HexNumber_h_byte()
		{
			var num = 0x71;

			var result = ParseImmediate("71h");

			Assert.AreEqual(num, result);
		}

		[TestMethod]
		public void ParseImmediate_HexNumber_h_Uppercase()
		{
			var num = 0x71;

			var result = ParseImmediate("71H");

			Assert.AreEqual(num, result);
		}

		[TestMethod]
		public void ParseImmediate_HexNumber_uint16()
		{
			var num = 0x1171;

			var result = ParseImmediate("1171h");

			Assert.AreEqual(num, result);
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseImmediate_HexNumber_InvalidLength()
		{
			ParseImmediate("711h");
		}


		[TestMethod]
		public void ParseImmediate_True()
		{
			Assert.AreEqual(1, ParseImmediate("true"));
		}

		[TestMethod]
		public void ParseImmediate_False()
		{
			Assert.AreEqual(0, ParseImmediate("false"));
		}

		[TestMethod]
		public void ParseImmediate_True_IgnoreCase()
		{
			Assert.AreEqual(1, ParseImmediate("True"));
		}

		[TestMethod]
		public void ParseImmediate_False_IgnoreCase()
		{
			Assert.AreEqual(0, ParseImmediate("False"));
		}
	}
}
