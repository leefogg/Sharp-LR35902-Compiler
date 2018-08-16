using System;
using System.Collections.Generic;
using Common.Extensions;

namespace Sharp_LR35902_Assembler
{
	public class Formatter
    {
		public static void Format(List<string> instructions)
		{
			LineBreakLabels(instructions);
			RemoveWhitespaceAndAddComma(instructions);
		}

		public static void LineBreakLabels(List<string> instructions)
		{
			for (var i = 0; i < instructions.Count; i++)
			{
				var line = instructions[i];
				var colonindex = line.IndexOf(':');
				if (colonindex == -1) // Not a label
					continue;
				// Is a label..
				if (colonindex == line.Length-1) // Is just a label
					continue;

				instructions[i] = line.Substring(0, colonindex+1);
				instructions.Insert(i + 1, line.Substring(colonindex+1, line.Length - 1 - colonindex));
			}
		}

		public static void RemoveWhitespaceAndAddComma(List<string> instructions)
		{
			for (var i = 0; i < instructions.Count; i++)
			{
				var line = instructions[i].Trim();
				while (line.Contains("  "))
					line = line.Replace("  ", " ");

				var lastspace = line.LastIndexOf(' ');
				if (lastspace > line.IndexOf(' '))
					if (line[lastspace - 1] != ',')
						line = line.Substring(0, lastspace) + "," + line.Substring(lastspace, line.Length - lastspace);

				instructions[i] = line;
			}
		}
	}
}
