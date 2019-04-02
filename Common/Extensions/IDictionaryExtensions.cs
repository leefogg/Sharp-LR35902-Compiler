using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Extensions
{
	public static class IDictionaryExtensions
	{
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<KeyValuePair<TKey, TValue>> pairs) {
			foreach (var pair in pairs)
				self.Add(pair.Key, pair.Value);
		}
	}
}
