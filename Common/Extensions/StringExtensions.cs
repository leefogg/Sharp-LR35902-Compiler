using System.Collections.Generic;

namespace Common.Extensions {
	public static class StringExtensions {
		public static IEnumerable<string> SplitAndKeep(this string s, char[] delims) {
			int start = 0, index;

			while ((index = s.IndexOfAny(delims, start)) != -1) {
				if (index - start > 0)
					yield return s.Substring(start, index - start);
				yield return s.Substring(index, 1);
				start = index + 1;
			}

			if (start < s.Length)
				yield return s.Substring(start);
		}

		public static bool Contains(this string self, char character) => self.IndexOf(character) != -1;

		public static int IndexOfFirst(this string self, IEnumerable<char> other) {
			foreach (var item in other) {
				var index = self.IndexOf(item);
				if (index != -1)
					return index;
			}

			return -1;
		}

		public static int IndexOfFirst(this string self, IEnumerable<string> other) {
			foreach (var item in other) {
				var index = self.IndexOf(item);
				if (index != -1)
					return index;
			}

			return -1;
		}
	}
}
