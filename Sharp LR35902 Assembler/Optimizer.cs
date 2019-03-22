using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sharp_LR35902_Assembler
{
    public class Optimizer
    {
		public static void Optimize(List<string> instructions, byte level)
		{
			if (level == 0)
				return;

			// Prune
			DeleteUnreachableCode(instructions);

			// Convert
			for (var i = 0; i < instructions.Count; i++) {
				instructions[i] = LD_A_0_TO_XOR_A(instructions[i]);
				// TODO: JP to JR
			}

			IncludeNearIncDec(instructions);
		}

		/*
		Utilizes LDD and LDI optimization instructions by 
		replacing LD A, (HL) or LD (HL), A followed by a increment or decrement with 
		a LDI or LDD respectively.
		Conservative optimization because will not replace if HL is used inbetween.
		*/
		public static int IncludeNearIncDec(IList<string> instructions)
		{
			var modifiedlines = 0;

			for (var i=0; i<instructions.Count; i++)
			{
				var loadinstruction = instructions[i];
				var isloadto = loadinstruction == "LD A (HL)";
				var isloadfrom = loadinstruction == "LD (HL) A";
				if (!(isloadfrom || isloadto))
					continue;

				// Look for near increment or decrement
				// INC or DEC must be the next instruction of HL otherwise may break functionallity
				for (var j=i+1; j<instructions.Count; j++)
				{
					var incdecinstruction = instructions[j];
					if (incdecinstruction.Contains("HL"))
					{
						if (instructions[j] == "INC HL")
						{
							if (isloadto)
								instructions[i] = "LDI A (HL)";
							else
								instructions[i] = "LDI (HL) A";
							modifiedlines++;
							instructions.RemoveAt(j);
							break;
						}
						else if (incdecinstruction == "DEC HL")
						{
							if (isloadto)
								instructions[i] = "LDD A (HL)";
							else
								instructions[i] = "LDD (HL) A";
							modifiedlines++;
							instructions.RemoveAt(j);
							break;
						}

						// Start again from here
						i = j-1; 
						break;
					}
					else if (incdecinstruction.StartsWith("JP") || incdecinstruction.StartsWith("JR"))
					{
						// Start again from the next line
						i = j; 
						break;
					}
				}
			}

			return modifiedlines;
		}

		private static IEnumerable<string> getUsedRegisters(string instruction)
		{
			var registers = new[] { "A", "B", "C", "D", "E", "H", "L", "AF", "BC", "DE", "HL" };

			instruction = instruction.Replace("(", string.Empty).Replace(")", string.Empty).Replace(",", string.Empty);
			var parts = instruction.Split(" ");
			return parts.Skip(1).Intersect(registers);
		}

		// Deletes code between non-conditional jumps/calls and the next label
		public static int DeleteUnreachableCode(List<string> lines)
		{
			var deletedlines = 0;
			var j = 0;

			for(var i=0; i<lines.Count; i=j++)
			{
				var instruction = lines[i].ToUpper();
				// Is Jump or return
				if (!(instruction.StartsWith("JP") || instruction.StartsWith("RET")))
					continue;
				// Skip conditional
				if (instruction.LastIndexOf(' ') > instruction.IndexOf(' '))
					continue;

				// Delete all code between here and the next label or EOF
				// Find next label
				for (; j<lines.Count; j++)
					if (lines[j].EndsWith(":"))
						break;
				// Delete middle
				var linestodelete = j - i - 1;
				lines.RemoveRange(i + 1, linestodelete);
				deletedlines += linestodelete;
			}

			return deletedlines;
		}

		public static string LD_A_0_TO_XOR_A(string instruction)
		{
			if (instruction == "LD A, 0")
				return "XOR A";

			return instruction;
		}
    }
}
