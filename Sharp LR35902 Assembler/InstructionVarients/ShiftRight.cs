namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ShiftRight : InstructionVarient
	{
		public Register Register;

		public ShiftRight(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCB, (byte)(0x38 + (int)Register) };
		}
	}
}
