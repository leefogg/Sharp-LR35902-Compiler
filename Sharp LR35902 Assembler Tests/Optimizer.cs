using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Test_Common.Utils;
using static Sharp_LR35902_Assembler.Optimizer;

namespace Sharp_LR35902_Assembler_Tests {
	[TestClass]
	public class Optimizer {
		[TestMethod]
		public void LD_A_0_TO_XOR_A_NotApplicable() {
			var result = LD_A_0_TO_XOR_A("EI");

			Assert.AreEqual("EI", result);
		}

		[TestMethod]
		public void LD_A_0_TO_XOR_A_Converts() {
			var result = LD_A_0_TO_XOR_A("LD A, 0");

			Assert.AreEqual("XOR A", result);
		}

		[TestMethod]
		public void DeleteUnreachableCode_EndOfFile() {
			var lines = new List<string> {
				"JP SOMELABEL",
				"NOP",
				"NOP"
			};

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(2, linesremoved);
			ListEqual(lines.ToArray(), new[] {
				"JP SOMELABEL"
			});
		}

		[TestMethod]
		public void DeleteUnreachableCode_NotCaseSensitive() {
			var lines = new List<string> {
				"ret",
				"NOP",
				"NOP",
				"ANOTHERLABEL:",
				"DI"
			};

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(2, linesremoved);
			ListEqual(lines.ToArray(), new[] {
				"ret",
				"ANOTHERLABEL:",
				"DI"
			});
		}

		[TestMethod]
		public void DeleteUnreachableCode_CropsMiddle_Jump() {
			var lines = new List<string> {
				"JP SOMELABEL",
				"NOP",
				"NOP",
				"ANOTHERLABEL:",
				"DI"
			};

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(2, linesremoved);
			ListEqual(lines.ToArray(), new[] {
				"JP SOMELABEL",
				"ANOTHERLABEL:",
				"DI"
			});
		}

		[TestMethod]
		public void DeleteUnreachableCode_CropsMiddle_Return() {
			var lines = new List<string> {
				"RET",
				"NOP",
				"NOP",
				"ANOTHERLABEL:",
				"DI"
			};

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(2, linesremoved);
			ListEqual(lines.ToArray(), new[] {
				"RET",
				"ANOTHERLABEL:",
				"DI"
			});
		}

		[TestMethod]
		public void DeleteUnreachableCode_IgnoresConditional() {
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
			ListEqual(lines.ToArray(), instructions);
		}

		[TestMethod]
		public void DeleteUnreachableCode_ContinuesCorrectly() {
			var lines = new List<string> {
				"JP SOMELABEL",
				"NOP",
				"ANOTHERLABEL:",
				"JP SOMELABEL",
				"NOP",
				"ANOTHERLABEL:"
			};

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(2, linesremoved);
			ListEqual(lines.ToArray(), new[] {
				"JP SOMELABEL",
				"ANOTHERLABEL:",
				"JP SOMELABEL",
				"ANOTHERLABEL:"
			});
		}

		[TestMethod]
		public void DeleteUnreachableCode_NoGap() {
			var instructions = new[] {
				"JP SOMELABEL",
				"ANOTHERLABEL:",
				"JP SOMELABEL",
				"ANOTHERLABEL:"
			};
			var lines = new List<string>(instructions);

			var linesremoved = DeleteUnreachableCode(lines);

			Assert.AreEqual(0, linesremoved);
			ListEqual(lines.ToArray(), instructions);
		}

		[TestMethod]
		public void IncludeNearIncDec_Increment_LoadFrom() {
			var instructions = new List<string> {
				"LD (HL) A",
				"INC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(1, changedlines);
			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual("LDI (HL) A", instructions[0]);
		}

		[TestMethod]
		public void IncludeNearIncDec_Increment_LoadTo() {
			var instructions = new List<string> {
				"LD A (HL)",
				"INC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(1, changedlines);
			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual("LDI A (HL)", instructions[0]);
		}

		[TestMethod]
		public void IncludeNearIncDec_Decrement_LoadFrom() {
			var instructions = new List<string> {
				"LD (HL) A",
				"DEC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(1, changedlines);
			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual("LDD (HL) A", instructions[0]);
		}

		[TestMethod]
		public void IncludeNearIncDec_Decrement_LoadTo() {
			var instructions = new List<string> {
				"LD A (HL)",
				"DEC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(1, changedlines);
			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual("LDD A (HL)", instructions[0]);
		}

		[TestMethod]
		public void IncludeNearIncDec_Latest() {
			var instructions = new List<string> {
				"LD A (HL)",
				"LD A (HL)",
				"DEC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(1, changedlines);
			Assert.AreEqual(2, instructions.Count);
			Assert.AreEqual("LDD A (HL)", instructions[1]);
		}

		[TestMethod]
		public void IncludeNearIncDec_Searches() {
			var instructions = new List<string> {
				"LD A (HL)",
				"NOP",
				"INC A",
				"DEC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(1, changedlines);
			Assert.AreEqual(3, instructions.Count);
			Assert.AreEqual("LDD A (HL)", instructions[0]);
		}

		[TestMethod]
		public void IncludeNearIncDec_WriteCancels() {
			var instructions = new List<string> {
				"LD A (HL)",
				"LD HL, 10",
				"DEC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(0, changedlines);
		}

		[TestMethod]
		public void IncludeNearIncDec_JumpCancels_JP() {
			var instructions = new List<string> {
				"LD A (HL)",
				"JP 100",
				"DEC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(0, changedlines);
		}

		[TestMethod]
		public void IncludeNearIncDec_JumpCancels_JR() {
			var instructions = new List<string> {
				"LD A (HL)",
				"JR 100",
				"DEC HL"
			};

			var changedlines = IncludeNearIncDec(instructions);

			Assert.AreEqual(0, changedlines);
		}

		[TestMethod]
		public void RemoveRedundantWrites_Simple() {
			var instructions = new List<string> {
				"LD A 10",
				"LD A 5",
				"LD A 1"
			};

			var changesmade = RemoveRedundantWrites(instructions);

			Assert.IsTrue(changesmade);
			ListEqual(new[] {
				"LD A 1"
			}, instructions);
		}

		[TestMethod]
		public void RemoveRedundantWrites_Searches() {
			var instructions = new List<string> {
				"LD A 10",
				"NOP",
				"CP 1",
				"LD A 1"
			};

			var changesmade = RemoveRedundantWrites(instructions);

			Assert.IsTrue(changesmade);
			ListEqual(new[] {
				"NOP",
				"CP 1",
				"LD A 1"
			}, instructions);
		}

		[TestMethod]
		public void RemoveRedundantWrites_JumpCancels_JP() {
			var instructions = new List<string> {
				"LD A 10",
				"JP 100",
				"LD A 1"
			};

			var changesmade = RemoveRedundantWrites(instructions);

			Assert.IsFalse(changesmade);
		}

		[TestMethod]
		public void RemoveRedundantWrites_JumpCancels_JR() {
			var instructions = new List<string> {
				"LD A 10",
				"JR 100",
				"LD A 1"
			};

			var changesmade = RemoveRedundantWrites(instructions);

			Assert.IsFalse(changesmade);
		}

		[TestMethod]
		public void RemoveRedundantWrites_ReadCancels() {
			var instructions = new List<string> {
				"LD A 10",
				"LD B A",
				"LD A 1"
			};

			var changesmade = RemoveRedundantWrites(instructions);

			Assert.IsFalse(changesmade);
		}
	}
}
