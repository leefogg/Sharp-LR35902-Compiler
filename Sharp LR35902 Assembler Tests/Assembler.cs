using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Sharp_LR35902_Compiler_Tests.Utils;
using static Sharp_LR35902_Assembler.Assembler;
using Common.Exceptions;

namespace Sharp_LR35902_Assembler_Tests
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

			Assert.IsTrue(TryParseImmediate("0xE1", ref val));
			Assert.AreEqual(0xE1, val);
		}
		[TestMethod]
		public void ParsesHex_16Bit()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseImmediate("0xE1E1", ref val));
			Assert.AreEqual(0xE1E1, val);
		}

		[TestMethod]
		public void ParsesBinary_8Bit()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseImmediate("0B11100001", ref val));
			Assert.AreEqual(0B11100001, val);
		}
		[TestMethod]
		public void ParsesBinary_16Bit()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseImmediate("0B1110000111100001", ref val));
			Assert.AreEqual(0b1110000111100001, val);
		}
		[TestMethod]
		public void ParsesBinary_LowerCase()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseImmediate("0b11100001", ref val));
			Assert.AreEqual(0b11100001, val);
		}
		[TestMethod]
		public void ParsesBinary_InvalidLength()
		{
			ushort val = 0;
			Assert.IsFalse(TryParseImmediate("0b1110000111", ref val));
		}
		[TestMethod]
		public void ParseBinary_InvalidChar()
		{
			ushort val = 0;
			Assert.IsFalse(TryParseImmediate("0b00123012", ref val));
		}

		[TestMethod]
		public void CompileProgram_MultipleLines()
		{
			var result = CompileProgram(new[]{ "EI", "LD A, 0xE1" });
			Assert.AreEqual(3, result.Length);
		}

		[TestMethod]
		public void TryParseConstant_GetDefinition_FindsIt()
		{
			ushort expectedvalue = 0x7F00;
			SetDefintion("XX", expectedvalue);

			ushort value = 0;
			Assert.IsTrue(TryParseImmediate("XX", ref value));
			Assert.AreEqual(expectedvalue, value);
		}

		[TestMethod]
		public void AddDefinition_Overrides()
		{
			SetDefintion("X", 1);
			SetDefintion("X", 2);

			ushort val = 0;
			Assert.IsTrue(TryParseImmediate("X", ref val));
			Assert.AreEqual(2, val);
		}

		[TestMethod]
		public void CompileProgram_AddsDefintition()
		{
			ushort expectedvalue = 0x7F;
			CompileProgram(new[] { $"#DEFINE X {expectedvalue}" });

			ushort value = 0;
			Assert.IsTrue(TryParseImmediate("X", ref value));
			Assert.AreEqual(expectedvalue, value);
		}

		[TestMethod]
		public void CompileProgram_AddsDefintition_RequiresParsing()
		{
			ushort expectedvalue = 0x7F;
			CompileProgram(new[] { $"#DEFINE X 0x7F" });

			ushort value = 0;
			Assert.IsTrue(TryParseImmediate("X", ref value));
			Assert.AreEqual(expectedvalue, value);
		}

		[TestMethod]
		public void TryParseConstant_GetDefinition_DefaultValue()
		{
			SetDefintion("B");

			ushort value = 11;
			Assert.IsTrue(TryParseImmediate("B", ref value));
			Assert.AreEqual(0, value);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void TryParseConstant_GetDefinition_ThrowsOnParseFail()
		{
			SetDefintion("VRAM", "Hi there");
		}

		[TestMethod]
		public void CompileInstruction_FindsDefinition()
		{
			ushort val = 11;
			SetDefintion("X", val);

			var result = CompileInstruction("LD A, X");
			Is(result, new byte[] { 0x3E, (byte)val });
		}

		[TestMethod]
		public void CompileInstruction_FindsDefinition_CaseInsensitive()
		{
			ushort val = 11;
			SetDefintion("x", val);

			var result = CompileInstruction("LD A, x");
			Is(result, new byte[] { 0x3E, (byte)val });
		}

		[TestMethod]
		public void TryParseConstant_Math_Addition()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseImmediate("1+1", ref val));
			Assert.AreEqual(2, val);
		}
		[TestMethod]
		public void TryParseConstant_Math_Subtraction()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseImmediate("10-5", ref val));
			Assert.AreEqual(5, val);
		}
		[TestMethod]
		public void TryParseConstant_Math_WithWhitespace()
		{
			ushort val = 0;

			Assert.IsTrue(TryParseImmediate("10 - 5", ref val));
			Assert.AreEqual(5, val);
		}
	}
}
