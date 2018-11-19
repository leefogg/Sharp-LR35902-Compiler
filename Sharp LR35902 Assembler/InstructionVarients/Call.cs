using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class Call : InstructionVarient
	{
		private readonly byte LowerAddress, HigherAddress;

		public Call(ushort address)
		{
			var addressbytes = address.ToByteArray();
			LowerAddress = addressbytes[0];
			HigherAddress = addressbytes[1];
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCD, LowerAddress, HigherAddress };
		}
	}
}
