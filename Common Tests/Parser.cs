using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
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
		public void ParseImmediate_HexNumber()
		{
			var num = 0x11;

			Assert.AreEqual(num, ParseImmediate("0x11"));
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void ParseImmediate_HexNumber_WrongLength()
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
	}
}
