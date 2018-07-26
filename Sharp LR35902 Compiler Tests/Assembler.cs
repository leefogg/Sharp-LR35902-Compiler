using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler.Exceptions;
using static Sharp_LR35902_Compiler.Assembler;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Assembler
	{
		[TestMethod]
		[ExpectedException(typeof(NotFoundException))]
		public void UnrecognizedInstruction()
		{
			CompileInstruction("Something strange");
		}

		[TestMethod]
		public void ParsesHex_8Bit()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseConstant("0xE1", ref val));
			Assert.AreEqual(0xE1, val);
		}
		[TestMethod]
		public void ParsesHex_16Bit()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseConstant("0xE1E1", ref val));
			Assert.AreEqual(0xE1E1, val);
		}

		[TestMethod]
		public void ParsesBinary_8Bit()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseConstant("B11100001", ref val));
			Assert.AreEqual(0B11100001, val);
		}
		[TestMethod]
		public void ParsesBinary_16Bit()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseConstant("B1110000111100001", ref val));
			Assert.AreEqual(0b1110000111100001, val);
		}
		[TestMethod]
		public void ParsesBinary_CSyntax()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseConstant("0B11100001", ref val));
			Assert.AreEqual(0B11100001, val);
		}
		[TestMethod]
		public void ParsesBinary_LowerCase()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseConstant("0b11100001", ref val));
			Assert.AreEqual(0b11100001, val);
		}
		[TestMethod]
		public void ParsesBinary_InvalidLength()
		{
			ushort val = 0;
			Assert.IsFalse(TryParseConstant("0b1110000111", ref val));
		}
		[TestMethod]
		public void ParseBinary_InvalidChar()
		{
			ushort val = 0;
			Assert.IsFalse(TryParseConstant("0b00123012", ref val));
		}
	}
}
