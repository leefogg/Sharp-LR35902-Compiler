﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

			listEqual(new[] { "EI" }, instructions.ToArray());
		}

		[TestMethod]
		public void RemoveWhitespace_SingleSpacesOnly()
		{
			var instructions = new List<string>()
			{
				"LD   A,   01"
			};

			RemoveWhitespace(instructions);

			listEqual(new[] { "LD A, 01" }, instructions.ToArray());
		}

		[TestMethod]
		public void AddsComma()
		{
			var instructions = new List<string>()
			{
				"LD A 01"
			};

			AddComma(instructions);

			listEqual(new[] { "LD A, 01" }, instructions.ToArray());
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

			listEqual(
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

			listEqual(
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

			listEqual(
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
				"; This is a comment",
				"EI; Same line comment",
				"",
			};

			RemoveComments(instructions);

			listEqual(
				new[]
				{
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

			listEqual(
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

			listEqual(
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

			listEqual(
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

			listEqual(
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

			listEqual(
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
