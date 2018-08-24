﻿namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ClearCarryFlag : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0x3F };
		}
	}
}
