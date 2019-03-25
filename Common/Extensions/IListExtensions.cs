using System.Collections.Generic;

namespace Common.Extensions
{
	public static class IListExtensions
	{
		public static T Get<T>(this IList<T> self, int index) 
			=> index >= self.Count ? default(T) : self[index];
	}
}
