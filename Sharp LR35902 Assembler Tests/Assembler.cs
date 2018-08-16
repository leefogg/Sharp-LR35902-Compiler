using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Test_Common.Utils;
using static Sharp_LR35902_Assembler.Assembler;
using Common.Exceptions;
using System.Collections.Generic;
using System;

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
		public void CompileProgram_MultipleLines()
		{
			var result = CompileProgram(new List<string>() { "EI", "EI" });
			StartsWith(
				new byte[] { 0xFB, 0xFB, 0x00 },
				result
			);
		}

		[TestMethod]
		public void CompileProgram_AddsDefintition()
		{
			ushort expectedvalue = 0x7F;
			CompileProgram(new List<string>() { $"#DEFINE X {expectedvalue}" });

			ushort value = 0;
			Assert.IsTrue(TryParseImmediate("X", ref value));
			Assert.AreEqual(expectedvalue, value);
		}

		[TestMethod]
		public void CompileProgram_AddsDefintition_RequiresParsing()
		{
			ushort expectedvalue = 0x7F;
			CompileProgram(new List<string>() { $"#DEFINE X 0x7F" });

			ushort value = 0;
			Assert.IsTrue(TryParseImmediate("X", ref value));
			Assert.AreEqual(expectedvalue, value);
		}

		[TestMethod]
		public void CompileProgram_ReplacesLabelLocation_Jump()
		{
			var instructions = new List<string>()
			{
				"XOR A",
				"jumplabel:",
				"JP jumplabel"
			};

			var binary = CompileProgram(instructions);

			StartsWith(
				new byte[] { 0xAF, 0xC3, 0x00, 0x01 },
				binary
			);
		}

		[TestMethod]
		public void CompileProgram_ReplacesLabelLocation_Call()
		{
			var instructions = new List<string>()
			{
				"XOR A",
				"calllabel:",
				"CALL calllabel"
			};

			var binary = CompileProgram(instructions);

			StartsWith(
				new byte[] { 0xAF, 0xCD, 0x00, 0x01 },
				binary
			);
		}

		[TestMethod]
		public void CompileProgram_ReplacesLabelLocation_NonCaseSensitive()
		{
			var instructions = new List<string>()
			{
				"XOR A",
				"AbCdEFDG:",
				"JP AbCdEFDG"
			};

			var binary = CompileProgram(instructions);

			StartsWith(
				new byte[] { 0xAF, 0xC3, 0x00, 0x01 },
				binary
			);
		}

		[TestMethod]
		[ExpectedException(typeof(NotFoundException))]
		public void CompileProgram_ReplacesLabelLocation_ThrowIfNotFound()
		{
			var instructions = new List<string>()
			{
				"XOR A",
				"top:",
				"JP bottom"
			};

			CompileProgram(instructions);
		}

		[TestMethod]
		public void CompileProgram_CompilerDirective_Org()
		{
			var instructions = new List<string>()
			{
				".ORG 0x03",
				"EI"
			};

			var binary = CompileProgram(instructions);

			StartsWith(
				new byte[]
				{
					0x00,
					0x00,
					0x00,
					0xFB
				},
				binary
			);
		}

		[TestMethod]
		public void CompileProgram_CompilerDirective_SupportsHash()
		{
			var instructions = new List<string>()
			{
				"#ORG 0x03",
				"EI"
			};

			var binary = CompileProgram(instructions);

			StartsWith(
				new byte[]
				{
					0x00,
					0x00,
					0x00,
					0xFB
				},
				binary
			);
		}

		[TestMethod]
		public void CompileProgram_CompilerDirective_Byte()
		{
			var instructions = new List<string>()
			{
				".byte 1 0x01 0b00000001",
			};

			var binary = CompileProgram(instructions);

			StartsWith(
				new byte[]
				{
					0x01,
					0x01,
					0x01,
				},
				binary
			);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CompileProgram_CompilerDirective_Byte_MustBe8Bit()
		{
			var instructions = new List<string>()
			{
				".byte 256",
			};

			CompileProgram(instructions);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void CompileProgram_CompilerDirective_Byte_FailedToParseThrows()
		{
			var instructions = new List<string>()
			{
				".byte 255 x",
			};

			CompileProgram(instructions);
		}

		[TestMethod]
		public void CompileProgram_CompilerDirective_Text()
		{
			var instructions = new List<string>()
			{
				".text hello",
			};

			var binary = CompileProgram(instructions);

			StartsWith(
				new byte[]
				{
					(byte)'h',
					(byte)'e',
					(byte)'l',
					(byte)'l',
					(byte)'o',
				},
				binary
			);
		}

		[TestMethod]
		[ExpectedException(typeof(NotFoundException))]
		public void CompileProgram_CompilerDirective_NotFound()
		{
			var instructions = new List<string>()
			{
				"#somedirective",
			};

			CompileProgram(instructions);
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
