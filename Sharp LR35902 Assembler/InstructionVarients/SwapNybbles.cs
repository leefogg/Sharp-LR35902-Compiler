namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class SwapNybbles : InstructionVarient
	{
		public Register Register;

		public SwapNybbles(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCB, (byte)(0x30 + (int)Register) };
		}
	}
}
