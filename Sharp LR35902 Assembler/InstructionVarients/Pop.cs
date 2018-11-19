using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class Pop : InstructionVarient
	{
		private readonly int RegisterPairIndex;

		public Pop(int registerpairindex)
		{
			RegisterPairIndex = registerpairindex;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0xC1 + 0x10 * RegisterPairIndex) };
		}
	}
}
