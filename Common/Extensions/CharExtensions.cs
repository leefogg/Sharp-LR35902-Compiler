using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
	public static class CharExtensions
	{
		public static bool IsLetter(this char self)
		{
			return (self >= 'a' && self <= 'z') || (self >= 'A' && self <= 'Z');
		}

		public static bool isNumber(this char self)
		{
			return self >= '0' && self <= '9';
		}
	}
}
