namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ShiftRightPreserveSign : InstructionVarient
	{
		public Register Register;

		public ShiftRightPreserveSign(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCB, (byte)(0x28 + (int)Register) };
		}
	}
}
