using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using static Sharp_LR35902_Assembler.Formatter;
using static Test_Common.Utils;

namespace Sharp_LR35902_Assembler_Tests
{
	[TestClass]
    public class Formatter
    {
		[TestMethod]
		public void RemoveWhitespace_TrimsWhitespace()
		{
			var instructions = new List<string>()
			{
				"	 EI		"
			};

			RemoveWhitespace(instructions);

			ListEqual(new[] { "EI" }, instructions.ToArray());
		}

		[TestMethod]
		public void RemoveWhitespace_SingleSpacesOnly()
		{
			var instructions = new List<string>()
			{
				"LD   A,   01"
			};

			RemoveWhitespace(instructions);

			ListEqual(new[] { "LD A, 01" }, instructions.ToArray());
		}

		[TestMethod]
		public void RemoveComma_Removes()
		{
			var instructions = new List<string>()
			{
				"LD A,01"
			};

			RemoveComma(instructions);

			ListEqual(new[] { "LD A 01" }, instructions.ToArray());
		}

		[TestMethod]
		public void LineBreakLabels_Breaks()
		{
			var instructions = new List<string>()
			{
				"label1:	XOR A",
				"label2:	XOR A"
			};

			LineBreakLabels(instructions);

			ListEqual(
				new[]
				{
					"label1:",
					"	XOR A",
					"label2:",
					"	XOR A"
				},
				instructions.ToArray()
			);
		}

		[TestMethod]
		public void LineBreakLabels_IgnoresCorrectLabels()
		{
			var instructions = new List<string>()
			{
				"label1:	XOR A",
				"label2:",
				"XOR A",
			};

			LineBreakLabels(instructions);

			ListEqual(
				new[]
				{
					"label1:",
					"	XOR A",
					"label2:",
					"XOR A"
				},
				instructions.ToArray()
			);
		}

		[TestMethod]
		public void RemoveBlankLines_Removes()
		{
			var instructions = new List<string>()
			{
				"",
				"XOR A",
				"",
			};

			RemoveBlankLines(instructions);

			ListEqual(
				new[]
				{
					"XOR A"
				},
				instructions.ToArray()
			);
		}

		[TestMethod]
		public void RemoveComments_Removes()
		{
			var instructions = new List<string>()
			{
				"NOP",
				"; This is a comment",
				"EI; Same line comment",
				"",
			};

			RemoveComments(instructions);

			ListEqual(
				new[]
				{
					"NOP",
					"",
					"EI",
					""
				},
				instructions.ToArray()
			);
		}

		[TestMethod]
		public void EnsureNOPAfterSTOPOrHALT_Halt()
		{
			var instructions = new List<string>()
			{
				"HALT",
				"EI",
			};

			EnsureNOPAfterSTOPOrHALT(instructions);

			ListEqual(
				new[]
				{
					"HALT",
					"NOP",
					"EI",
				},
				instructions.ToArray()
			);
		}

		[TestMethod]
		public void EnsureNOPAfterSTOPOrHALT_Stop()
		{
			var instructions = new List<string>()
			{
				"STOP",
				"EI",
			};

			EnsureNOPAfterSTOPOrHALT(instructions);

			ListEqual(
				new[]
				{
					"STOP",
					"NOP",
					"EI",
				},
				instructions.ToArray()
			);
		}

		[TestMethod]
		public void EnsureNOPAfterSTOPOrHALT_NotCaseSensitive()
		{
			var instructions = new List<string>()
			{
				"haLt",
				"EI",
			};

			EnsureNOPAfterSTOPOrHALT(instructions);

			ListEqual(
				new[]
				{
					"haLt",
					"NOP",
					"EI",
				},
				instructions.ToArray()
			);
		}

		[TestMethod]
		public void EnsureNOPAfterSTOPOrHALT_EOF()
		{
			var instructions = new List<string>()
			{
				"STOP",
			};

			EnsureNOPAfterSTOPOrHALT(instructions);

			ListEqual(
				new[]
				{
					"STOP",
				},
				instructions.ToArray()
			);
		}

		[TestMethod]
		public void EnsureNOPAfterSTOPOrHALT_ExistingNOP()
		{
			var instructions = new List<string>()
			{
				"STOP",
				"NOP"
			};

			EnsureNOPAfterSTOPOrHALT(instructions);

			ListEqual(
				new[]
				{
					"STOP",
					"NOP"
				},
				instructions.ToArray()
			);
		}
	}
}
