using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadImmediateIntoRegister : InstructionVarient
	{
		private readonly Register To;
		private readonly byte Immediate;

		public LoadImmediateIntoRegister(byte immediate, Register to)
		{
			To = to;
			Immediate = immediate;
		}

		public override byte[] Compile()
		{
			var bytecode = 0x06 + ((int)To * NumRegisters);
			return new[] { (byte)bytecode, Immediate };
		}
	}
}
