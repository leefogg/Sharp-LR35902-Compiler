using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Extensions
{
	public static class IEnumerableExtensions
	{
		public static T[] ListOf<T>(params T[] args)
		{
			return args;
		}

		public static int IndexOf<T>(this IEnumerable<T> self, T itemtofind) where T : class
		{
			var index = 0;
			foreach (T item in self)
			{
				if (item == itemtofind)
					return index;

				index++;
			}

			return -1;
		}

		public static int IndexOf(this IEnumerable<string> self, string itemtofind)
		{
			var index = 0;
			foreach (string item in self)
			{
				if (item == itemtofind)
					return index;

				index++;
			}

			return -1;
		}
	}
}
