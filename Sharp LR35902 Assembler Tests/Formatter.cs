using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using static Sharp_LR35902_Assembler.Formatter;
using static Sharp_LR35902_Compiler_Tests.Utils;

namespace Sharp_LR35902_Assembler_Tests
{
	[TestClass]
    public class Formatter
    {
		[TestMethod]
		public void Format_TrimsWhitespace()
		{
			var instructions = new List<string>()
			{
				"	 EI		"
			};

			Format(instructions);

			listEqual(new[] { "EI" }, instructions.ToArray());
		}

		[TestMethod]
		public void Format_SingleSpacesOnly()
		{
			var instructions = new List<string>()
			{
				"LD   A,   01"
			};

			Format(instructions);

			listEqual(new[] { "LD A, 01" }, instructions.ToArray());
		}

		[TestMethod]
		public void Format_AddsComma()
		{
			var instructions = new List<string>()
			{
				"LD A 01"
			};

			Format(instructions);

			listEqual(new[] { "LD A, 01" }, instructions.ToArray());
		}
	}
}
