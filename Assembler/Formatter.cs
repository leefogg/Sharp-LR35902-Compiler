using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler
{
    public class Formatter
    {
		public static void Format(List<string> instructions)
		{
			for (var i = 0; i < instructions.Count; i++)
			{
				//TODO: Convert this all to use StringBuilder to create much less garbage

				var line = instructions[i].Trim();
				while (line.Contains("  "))
					line = line.Replace("  ", " ");

				var lastspace = line.LastIndexOf(' ');
				if (lastspace > line.IndexOf(' '))
					if (line[lastspace-1] != ',')
					line = line.Substring(0, lastspace) + "," + line.Substring(lastspace, line.Length - lastspace);

				instructions[i] = line;
			}
		}
    }
}
