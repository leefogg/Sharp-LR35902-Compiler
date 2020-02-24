using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common.Extensions;

namespace Sharp_LR35902_Assembler {
	public class Formatter {
		private static readonly Regex WhitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
		private static readonly char[] CommentPrefixes = new[] { ';', '}' };

		public static void Format(IList<string> instructions) {
			RemoveComments(instructions);
			LineBreakLabels(instructions);
			RemoveComma(instructions);
			RemoveWhitespace(instructions); // Isn't needed but good for consistant formatting for debugging
			RemoveBlankLines(instructions);
		}

		public static void RemoveComma(IList<string> instructions) {
			for (var i = 0; i < instructions.Count; i++)
				instructions[i] = instructions[i].Replace(',', ' ');
		}

		public static void RemoveComments(IList<string> instructions) {
			for (var i = 0; i < instructions.Count; i++) {
				var instruction = instructions[i];
				var indexofcomment = instruction.IndexOfFirst(CommentPrefixes);
				if (indexofcomment == -1)
					continue;

				instructions[i] = instruction.Substring(0, indexofcomment);
			}
		}

		public static void RemoveBlankLines(IList<string> instructions) {
			for (var i = 0; i < instructions.Count; i++)
				if (instructions[i].Length == 0) {
					instructions.RemoveAt(i);
					i--;
				}
		}

		public static void RemoveWhitespace(IList<string> instructions) {
			for (var i = 0; i < instructions.Count; i++) {
				var line = instructions[i];
				line = WhitespaceRegex.Replace(line, " ").Trim();
				instructions[i] = line;
			}
		}

		public static void LineBreakLabels(IList<string> instructions) {
			for (var i = 0; i < instructions.Count; i++) {
				var line = instructions[i];
				var colonindex = line.IndexOf(':');
				if (colonindex == -1) // Not a label
					continue;
				// Is a label..
				if (colonindex == line.Length - 1) // Is just a label
					continue;

				instructions[i] = line.Substring(0, colonindex + 1);
				instructions.Insert(i + 1, line.Substring(colonindex + 1, line.Length - 1 - colonindex));
			}
		}

		public static void EnsureNOPAfterSTOPOrHALT(IList<string> instructions) {
			for (var i = 0; i < instructions.Count; i++) {
				var line = instructions[i].ToUpper();
				if (!(line == "STOP" || line == "HALT"))
					continue;
				if (i + 1 == instructions.Count)
					continue;

				var nextline = instructions[i + 1];
				if (nextline != "NOP")
					instructions.Insert(i + 1, "NOP");
			}
		}
	}
}
