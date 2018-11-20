using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class DecrementRegister : InstructionVarient
	{
		private Register Register;

		public DecrementRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x05 + 8 * (int)Register) };
		}
	}
}
