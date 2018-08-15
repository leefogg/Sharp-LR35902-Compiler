using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler
{
    public class Optimizer
    {
		public static void Optimize(List<string> instructions)
		{
			// Prune
			DeleteUnreachableCode(instructions);

			// Convert
			for (var i = 0; i < instructions.Count; i++) {
				instructions[i] = LD_A_0_TO_XOR_A(instructions[i]);
				// TODO: JP to JR
			}
		}

		// Deletes code between non-conditional jumps/calls and the next label
		public static int DeleteUnreachableCode(List<string> lines)
		{
			var deletedlines = 0;
			var j = 0;

			for(var i=0; i<lines.Count; i=j++)
			{
				var instruction = lines[i];
				// Is Jump
				if (!(instruction.StartsWith("JP")))
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
