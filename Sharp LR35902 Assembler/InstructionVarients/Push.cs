using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class Push : InstructionVarient
	{
		public static readonly string[] RegisterPairs = new[] { "BC", "DE", "HL", "AF" };

		private readonly int RegisterPairIndex;

		public Push(int registerpairindex)
		{
			RegisterPairIndex = registerpairindex;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0xC5 + 0x10 * RegisterPairIndex) };
		}
	}
}
