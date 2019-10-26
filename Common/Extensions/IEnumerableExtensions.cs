using System.Collections.Generic;

namespace Common.Extensions {
	public static class IEnumerableExtensions {
		public static T[] ListOf<T>(params T[] args) => args;
		public static T[] ListOf<T>(T filler, int count)
		{
			var array = new T[count];
			for (var i = 0; i < count; i++)
				array[i] = filler;

			return array;
		}

		public static int IndexOf<T>(this IEnumerable<T> self, T itemtofind) where T : class {
			var index = 0;
			foreach (var item in self) {
				if (item == itemtofind)
					return index;

				index++;
			}

			return -1;
		}

		public static int IndexOf(this IEnumerable<string> self, string itemtofind) {
			var index = 0;
			foreach (var item in self) {
				if (item == itemtofind)
					return index;

				index++;
			}

			return -1;
		}
	}
}
