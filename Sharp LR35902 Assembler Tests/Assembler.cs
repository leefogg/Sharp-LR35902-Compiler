using System;
using System.Collections.Generic;
using Common.Exceptions;
using Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Assembler;
using Sharp_LR35902_Assembler.Exceptions;
using static Test_Common.Utils;

namespace Sharp_LR35902_Assembler_Tests {
	[TestClass]
	public class Assembler {
		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public void UnrecognizedInstruction() { new Sharp_LR35902_Assembler.Assembler().CompileInstruction("Something strange"); }

		[TestMethod]
		public void ParsesHex_8Bit() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("0xE1", ref val));
			Assert.AreEqual(0xE1, val);
		}

		[TestMethod]
		public void ParsesHex_16Bit() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("0xE1E1", ref val));
			Assert.AreEqual(0xE1E1, val);
		}

		[TestMethod]
		public void ParsesBinary_8Bit() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("0B11100001", ref val));
			Assert.AreEqual(0B11100001, val);
		}

		[TestMethod]
		public void ParsesBinary_16Bit() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("0B1110000111100001", ref val));
			Assert.AreEqual(0b1110000111100001, val);
		}

		[TestMethod]
		public void ParsesBinary_LowerCase() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("0b11100001", ref val));
			Assert.AreEqual(0b11100001, val);
		}

		[TestMethod]
		public void ParsesBinary_InvalidLength() {
			ushort val = 0;
			Assert.IsFalse(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("0b1110000111", ref val));
		}

		[TestMethod]
		public void ParseBinary_InvalidChar() {
			ushort val = 0;
			Assert.IsFalse(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("0b00123012", ref val));
		}

		[TestMethod]
		public void TryParseConstant_GetDefinition_FindsIt() {
			ushort expectedvalue = 0x7F00;
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.SetDefintion("XX", expectedvalue);

			ushort value = 0;
			Assert.IsTrue(assembler.TryParseImmediate("XX", ref value));
			Assert.AreEqual(expectedvalue, value);
		}

		[TestMethod]
		public void AddDefinition_Overrides() {
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.SetDefintion("X", 1);
			assembler.SetDefintion("X", 2);

			ushort val = 0;
			Assert.IsTrue(assembler.TryParseImmediate("X", ref val));
			Assert.AreEqual(2, val);
		}

		[TestMethod]
		public void CompileProgram_MultipleLines() {
			var result = new Sharp_LR35902_Assembler.Assembler().CompileProgram(new[] {"EI", "EI"}, null);
			StartsWith(
				new byte[] {0xFB, 0xFB, 0x00},
				result
			);
		}

		[TestMethod]
		public void CompileProgram_AddsDefintition() {
			ushort expectedvalue = 0x7F;
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.CompileProgram(new[] {$"#DEFINE X {expectedvalue}"}, null);

			ushort value = 0;
			Assert.IsTrue(assembler.TryParseImmediate("X", ref value));
			Assert.AreEqual(expectedvalue, value);
		}

		[TestMethod]
		public void CompileProgram_AddsDefintition_RequiresParsing() {
			ushort expectedvalue = 0x7F;
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.CompileProgram(new[] {"#DEFINE X 0x7F"}, null);

			ushort value = 0;
			Assert.IsTrue(assembler.TryParseImmediate("X", ref value));
			Assert.AreEqual(expectedvalue, value);
		}

		[TestMethod]
		public void CompileProgram_ReplacesLabelLocation_Jump() {
			var instructions = new[] {
				"XOR A",
				"jumplabel:",
				"JP jumplabel"
			};

			var binary = new Sharp_LR35902_Assembler.Assembler().CompileProgram(instructions, null);

			StartsWith(
				new byte[] {0xAF, 0xC3, 0x01, 0x00},
				binary
			);
		}

		[TestMethod]
		public void CompileProgram_ReplacesLabelLocation_Call() {
			var instructions = new[] {
				"XOR A",
				"calllabel:",
				"CALL calllabel"
			};

			var binary = new Sharp_LR35902_Assembler.Assembler().CompileProgram(instructions, null);

			StartsWith(
				new byte[] {0xAF, 0xCD, 0x01, 0x00},
				binary
			);
		}

		[TestMethod]
		public void CompileProgram_ReplacesLabelLocation_NonCaseSensitive() {
			var instructions = new[] {
				"XOR A",
				"AbCdEFDG:",
				"JP AbCdEFDG"
			};

			var binary = new Sharp_LR35902_Assembler.Assembler().CompileProgram(instructions, null);

			StartsWith(
				new byte[] {0xAF, 0xC3, 0x01, 0x00},
				binary
			);
		}

		[TestMethod]
		[ExpectedException(typeof(AggregateException))]
		public void CompileProgram_ReplacesLabelLocation_ThrowIfNotFound() {
			var instructions = new[] {
				"XOR A",
				"top:",
				"JP bottom"
			};

			new Sharp_LR35902_Assembler.Assembler().CompileProgram(instructions, null);
		}

		[TestMethod]
		public void ParseDirective_Org() {
			ushort currentlocation = 0;

			new Sharp_LR35902_Assembler.Assembler().ParseDirective(".ORG 0x03", ref currentlocation);

			Assert.AreEqual(0x03, currentlocation);
		}

		[TestMethod]
		public void ParseDirective_Org_Align()
		{
			ushort currentlocation = 10;

			var assembler = new Sharp_LR35902_Assembler.Assembler();
			assembler.ParseDirective(".ORG align 16", ref currentlocation);

			Assert.AreEqual(16, currentlocation);
		}

		[TestMethod]
		public void ParseDirective_SupportsHash() {
			ushort currentlocation = 0;

			new Sharp_LR35902_Assembler.Assembler().ParseDirective("#ORG 0x03", ref currentlocation);

			Assert.AreEqual(0x03, currentlocation);
		}

		[TestMethod]
		public void ParseDirective_Byte() {
			ushort currentlocation = 0;

			var rom = new Sharp_LR35902_Assembler.Assembler().ParseDirective(".byte 1 0x01 0b00000001", ref currentlocation);
			
			StartsWith(
				new byte[] {
					0x01,
					0x01,
					0x01
				},
				rom
			);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseDirective_Byte_MustBe8Bit() {
			ushort currentlocation = 1;

			new Sharp_LR35902_Assembler.Assembler().ParseDirective(".byte 256", ref currentlocation);

			Assert.AreEqual(2, currentlocation);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseDirective_Byte_FailedToParseThrows() {
				ushort currentlocation = 1;

			new Sharp_LR35902_Assembler.Assembler().ParseDirective(".byte 255 x",  ref currentlocation);

			Assert.AreEqual(2, currentlocation);
		}

		[TestMethod]
		public void ParseDirective_Text() {
			ushort currentlocation = 0;

			var rom = new Sharp_LR35902_Assembler.Assembler().ParseDirective(".text hello", ref currentlocation);

			StartsWith(
				new byte[] {
					(byte)'h',
					(byte)'e',
					(byte)'l',
					(byte)'l',
					(byte)'o'
				},
				rom
			);
		}

		[TestMethod]
		[ExpectedException(typeof(NotFoundException))]
		public void CompileProgram_CompilerDirective_NotFound() {
			ushort currentlocation = 0;

			new Sharp_LR35902_Assembler.Assembler().ParseDirective("#somedirective", ref currentlocation);
		}


		[TestMethod]
		public void CompileProgram_OverwriteInstruction_ThrowsWarning()
		{
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			var exceptions = new List<Exception>();
			assembler.CompileProgram(new[]
			{
				".byte 255",
				".org 0",
				".byte 0"
			}, exceptions);

			Assert.AreEqual(1, exceptions.Count);
			Assert.IsInstanceOfType(exceptions[0], typeof(OverwriteException));
		}

		[TestMethod]
		public void CompileProgram_Padding_FillsROM()
		{
			byte padding = 255;
			var rom = new Sharp_LR35902_Assembler.Assembler().CompileProgram(new string[0], null, padding);

			ListEqual(IEnumerableExtensions.ListOf(padding, rom.Length), rom);
		}

		[TestMethod]
		public void TryParseConstant_GetDefinition_DefaultValue() {
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.SetDefintion("B");

			ushort value = 11;
			Assert.IsTrue(assembler.TryParseImmediate("B", ref value));
			Assert.AreEqual(0, value);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TryParseConstant_GetDefinition_ThrowsOnParseFail() { new Sharp_LR35902_Assembler.Assembler().SetDefintion("VRAM", "Hi there"); }

		[TestMethod]
		public void CompileInstruction_FindsDefinition() {
			ushort val = 11;
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.SetDefintion("X", val);

			var result = assembler.CompileInstruction("LD A X");
			Is(result, 0x3E, (byte)val);
		}

		[TestMethod]
		public void CompileInstruction_FindsDefinition_CaseInsensitive() {
			ushort val = 11;
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.SetDefintion("x", val);

			var result = assembler.CompileInstruction("LD A x");
			Is(result, 0x3E, (byte)val);
		}

		[TestMethod]
		public void TryParseConstant_Math_Addition() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("1+1", ref val));
			Assert.AreEqual(2, val);
		}

		[TestMethod]
		public void TryParseConstant_Math_Subtraction() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("10-5", ref val));
			Assert.AreEqual(5, val);
		}

		[TestMethod]
		public void TryParseConstant_Math_TwoOperators() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("10-5+1", ref val));
			Assert.AreEqual(6, val);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TryParseConstant_Math_Unblanced() {
			ushort val = 0;

			new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("10-", ref val);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TryParseConstant_Math_Unblanced2() {
			ushort val = 0;

			new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("10-3+", ref val);
		}

		[TestMethod]
		public void TryParseConstant_Math_WithWhitespace() {
			ushort val = 0;

			Assert.IsTrue(new Sharp_LR35902_Assembler.Assembler().TryParseImmediate("10 - 5", ref val));
			Assert.AreEqual(5, val);
		}

		[TestMethod]
		public void TryParseConstant_Math_WithConstant() {
			ushort currentlocation = 0;
			ushort val = 0;
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.ParseDirective("#DEFINE O 77", ref currentlocation);
			Assert.IsTrue(assembler.TryParseImmediate("O + 3", ref val));
			Assert.AreEqual(80, val);
		}

		[TestMethod]
		public void TryParseConstant_Math_WithLabel() {
			ushort val = 0;
			var assembler = new Sharp_LR35902_Assembler.Assembler();

			assembler.AddLabelLocation("P", 77);
			Assert.IsTrue(assembler.TryParseImmediate("P + 3", ref val));
			Assert.AreEqual(80, val);
		}
	}
}
