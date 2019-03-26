using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharp_LR35902_Compiler
{
    public static class BuiltIn
	{
		public static class DataTypes
		{
			public static readonly PrimitiveDataType Byte = new PrimitiveDataType("byte", 8);
			public static readonly PrimitiveDataType Int =  new PrimitiveDataType("int", 16);

			public static readonly PrimitiveDataType[] All = { Byte, Int };

			public static bool Exists(string name) => All.Any(dt => dt.Name == name);
			public static PrimitiveDataType Get(string name) => All.FirstOrDefault(dt => dt.Name == name);
			public static bool CanConvertTo(PrimitiveDataType from, PrimitiveDataType to) => to.NumBits >= from.NumBits;
		}

		public static class Operators
		{
			public static readonly string
				Add = "+",
				Subtract = "-",
				Assign = "=",
				Equal = "==",
				Increment = "++",
				Decrement = "--",
				MoreThan = ">",
				LessThan = "<",
				And = "&&",
				Or = "||",
				Not = "!";
		}
	}
}
