using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Test_Common.Utils;
using static Sharp_LR35902_Assembler.Optimizer;
using System.Collections.Generic;

namespace Sharp_LR35902_Assembler_Tests
{
	[TestClass]
	public class Optimizer
	{
		[TestMethod]
		public void LD_A_0_TO_XOR_A_NotApplicable()
		{
			var result = LD_A_0_TO_XOR_A("EI");

			Assert.AreEqual("EI", result);
		}

		[TestMethod]
		public void LD_A_0_TO_XOR_A_Converts()
		{
			var result = LD_A_0_TO_XOR_A("LD A, 0");

			Assert.AreEqual("XOR A", result);
		}

		[TestMethod]
		public void DeleteUnreachableCode_EndOfFile()
		{
			var lines = new List<string>() {
				"JP SOMELABEL",
				"NOP",
				"NOP",
			};

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(2, linesremoved);
			listEqual(lines.ToArray(), new[] { 
				"JP SOMELABEL"
			});
		}

		[TestMethod]
		public void DeleteUnreachableCode_CropsMiddle()
		{
			var lines = new List<string>() {
				"JP SOMELABEL",
				"NOP",
				"NOP",
				"ANOTHERLABEL:",
				"DI"
			};

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(2, linesremoved);
			listEqual(lines.ToArray(), new[] {
				"JP SOMELABEL",
				"ANOTHERLABEL:",
				"DI"
			});
		}

		[TestMethod]
		public void DeleteUnreachableCode_IgnoresConditional()
		{
			var instructions = new[] {
				"JP Z SOMELABEL",
				"CALL Z LABEL",
				"NOP",
				"ANOTHERLABEL:",
				"DI"
			};
			var lines = new List<string>(instructions);

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(0, linesremoved);
			listEqual(lines.ToArray(), instructions);
		}

		[TestMethod]
		public void DeleteUnreachableCode_ContinuesCorrectly()
		{
			var lines = new List<string>(){
				"JP SOMELABEL",
				"NOP",
				"ANOTHERLABEL:",
				"JP SOMELABEL",
				"NOP",
				"ANOTHERLABEL:",
			};

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(2, linesremoved);
			listEqual(lines.ToArray(), new[] {
				"JP SOMELABEL",
				"ANOTHERLABEL:",
				"JP SOMELABEL",
				"ANOTHERLABEL:",
			});
		}

		[TestMethod]
		public void DeleteUnreachableCode_NoGap()
		{
			var instructions = new[] {
				"JP SOMELABEL",
				"ANOTHERLABEL:",
				"JP SOMELABEL",
				"ANOTHERLABEL:",
			};
			var lines = new List<string>(instructions);

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(0, linesremoved);
			listEqual(lines.ToArray(), instructions);
		}
	}
}
