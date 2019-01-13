using System;

namespace Sharp_LR35902_Compiler
{
	public class PrimitiveDataType
	{
		public string Name { get; }
		public int NumBits { get; }
		public int MaxValue => (int)Math.Pow(2, NumBits) - 1;

		public PrimitiveDataType(string name, int numBits)
		{
			Name = name;
			if (!IsPowerOfTwo(numBits))
				throw new ArgumentException("Number of bits must be a power of two.");
			NumBits = numBits;
		}

		bool IsPowerOfTwo(long x) => (x & (x - 1)) == 0;
	}
}
