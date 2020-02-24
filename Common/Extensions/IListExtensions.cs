﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Extensions {
	public static class IListExtensions {
		public static T Get<T>(this IList<T> self, int index) => index >= self.Count ? default(T) : self[index];

		public static string ToHumanList(this IList<string> self)
		{
			if (self.Count == 0)
				return string.Empty;

			var sb = new StringBuilder();
			sb.Append(self[0]);
			for (var i=1; i<self.Count; i++) {
				sb.Append(", ");
				sb.Append(self[i]);
			}

			return sb.ToString();
		}

		public static int LastIndexOf<T>(this IList<T> self, Func<T, bool> critera)
		{
			var lastGoodIndex = 0;
			for (int i = 0; i < self.Count; i++)
				if (critera(self[i]))
					lastGoodIndex = i;

			return lastGoodIndex;
		}
	}
}
