using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class Jump : InstructionVarient
	{
		private readonly byte LowerAddress, HigherAddress;

		public Jump(ushort address)
		{
			var addressbytes = address.ToByteArray();
			LowerAddress = addressbytes[0];
			HigherAddress = addressbytes[1];
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xC3, LowerAddress, HigherAddress };
		}
	}
}
