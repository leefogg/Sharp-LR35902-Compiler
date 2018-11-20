namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class SetBit : InstructionVarient
	{
		public Register Register;
		public byte Bit;

		public SetBit(Register register, byte bit)
		{
			Register = register;
			Bit = bit;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCB, (byte)(0xC0 + 8 * Bit + (int)Register) };
		}
	}
}
