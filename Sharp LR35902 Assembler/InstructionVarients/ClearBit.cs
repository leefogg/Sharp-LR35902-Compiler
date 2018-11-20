namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ClearBit : InstructionVarient
	{
		public Register Register;
		public byte Bit;

		public ClearBit(Register register, byte bit)
		{
			Register = register;
			Bit = bit;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCB, (byte)(0x80 + 8 * Bit + (int)Register) };
		}
	}
}
