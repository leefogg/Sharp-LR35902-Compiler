namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ShiftLeftPreserveSign : InstructionVarient
	{
		public Register Register;

		public ShiftLeftPreserveSign(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCB, (byte)(0x20 + (int)Register) };
		}
	}
}
