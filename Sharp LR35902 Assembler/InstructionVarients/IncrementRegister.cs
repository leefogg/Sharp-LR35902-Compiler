using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class IncrementRegister : InstructionVarient
	{
		private Register Register;

		public IncrementRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x04 + 8 * (int)Register) };
		}
	}
}
