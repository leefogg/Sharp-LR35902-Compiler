using System.Collections.Generic;
using System.Linq;
using Common.Extensions;

namespace Sharp_LR35902_Assembler {
	public class Optimizer {
		private static readonly string[] AllRegisters = {"A", "B", "C", "D", "E", "H", "L", "AF", "BC", "DE", "HL"};

		public static void Optimize(List<string> instructions, byte level) {
			if (level == 0)
				return;

			// Prune
			DeleteUnreachableCode(instructions);
			IncludeNearIncDec(instructions);
			while (RemoveRedundantWrites(instructions)) { }


			// Convert
			for (var i = 0; i < instructions.Count; i++)
				instructions[i] = LD_A_0_TO_XOR_A(instructions[i]);
			// TODO: JP to JR
		}

		private static bool RemoveRedundantWrites(IList<string> instructions) => RemoveSelfWrites(instructions) || RemoveOverwrittenWrites(instructions);

		public static bool RemoveSelfWrites(IList<string> instructions) {
			var changesmade = false;

			for (var i = 0; i < instructions.Count; i++) {
				var instruction = instructions[i];
				if (!instruction.StartsWith("LD "))
					continue;

				var parts = instruction.Split(' ');
				if (parts[1] == parts[2]) { // All loads have 2 oprands so this is safe
					instructions.RemoveAt(i);
					i--;
					changesmade = true;
				}
			}

			return changesmade;
		}

		// Removes writes to a register if the data is unconditionally overwritten
		// TODO: Use new basic blocks function to remove the need to check for jumps
		public static bool RemoveOverwrittenWrites(IList<string> instructions) {
			var changesmade = false;
			for (var i = 0; i < instructions.Count; i++)
				if (instructions[i].StartsWith("LD ")) {
					var parts = instructions[i].Split(' ');
					var destinationregister = parts.Get(1);
					if (!IsRegister(destinationregister))
						continue;

					for (var j = i + 1; j < instructions.Count; j++) {
						var otherinstructionparts = instructions[j].Split(' ');
						if (IsJump(instructions[j]))
							break;
						if (otherinstructionparts[0].StartsWith("LD") && otherinstructionparts.Get(1) == destinationregister) {
							instructions.RemoveAt(i--);
							changesmade = true;
							break;
						}

						if (otherinstructionparts.Get(1) == destinationregister || otherinstructionparts.Get(2) == destinationregister)
							break;
					}
				}

			return changesmade;
		}

		private static bool IsJump(string instruction) => instruction.StartsWith("JP") || instruction.StartsWith("JR");

		private static bool IsRegister(string possibleregister) => AllRegisters.Contains(possibleregister);

		/*
		Utilizes LDD and LDI optimization instructions by 
		replacing LD A, (HL) or LD (HL), A followed by a increment or decrement with 
		a LDI or LDD respectively.
		Conservative optimization because will not replace if HL is used inbetween.
		*/
		public static int IncludeNearIncDec(IList<string> instructions) {
			var modifiedlines = 0;

			for (var i = 0; i < instructions.Count; i++) {
				var loadinstruction = instructions[i];
				var isloadto = loadinstruction == "LD A (HL)";
				var isloadfrom = loadinstruction == "LD (HL) A";
				if (!(isloadfrom || isloadto))
					continue;

				// Look for near increment or decrement
				// INC or DEC must be the next instruction of HL otherwise may break functionallity
				for (var j = i + 1; j < instructions.Count; j++) {
					var incdecinstruction = instructions[j];
					if (incdecinstruction.Contains("HL")) {
						if (instructions[j] == "INC HL") {
							if (isloadto)
								instructions[i] = "LDI A (HL)";
							else
								instructions[i] = "LDI (HL) A";
							modifiedlines++;
							instructions.RemoveAt(j);
							break;
						}

						if (incdecinstruction == "DEC HL") {
							if (isloadto)
								instructions[i] = "LDD A (HL)";
							else
								instructions[i] = "LDD (HL) A";
							modifiedlines++;
							instructions.RemoveAt(j);
							break;
						}

						// Start again from here
						i = j - 1;
						break;
					}

					if (IsJump(incdecinstruction)) {
						// Start again from the next line
						i = j;
						break;
					}
				}
			}

			return modifiedlines;
		}

		private static IEnumerable<string> GetUsedRegisters(string instruction) {
			instruction = instruction.Replace("(", string.Empty).Replace(")", string.Empty).Replace(",", string.Empty);
			var parts = instruction.Split(" ");
			return parts.Skip(1).Intersect(AllRegisters);
		}

		// Deletes code between non-conditional jumps/calls and the next label
		public static int DeleteUnreachableCode(List<string> lines) {
			var deletedlines = 0;
			var j = 0;

			for (var i = 0; i < lines.Count; i = j++) {
				var instruction = lines[i].ToUpper();
				// Is Jump or return
				if (!(instruction.StartsWith("JP") || instruction.StartsWith("RET")))
					continue;
				// Skip conditional
				if (instruction.LastIndexOf(' ') > instruction.IndexOf(' '))
					continue;

				// Delete all code between here and the next label or EOF
				// Find next label
				for (; j < lines.Count; j++)
					if (lines[j].EndsWith(":"))
						break;
				// Delete middle
				var linestodelete = j - i - 1;
				lines.RemoveRange(i + 1, linestodelete);
				deletedlines += linestodelete;
			}

			return deletedlines;
		}

		public static string LD_A_0_TO_XOR_A(string instruction) => instruction == "LD A, 0" ? "XOR A" : instruction;
	}
}
