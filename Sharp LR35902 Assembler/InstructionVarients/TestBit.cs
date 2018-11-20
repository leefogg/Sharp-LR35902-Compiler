namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class TestBit : InstructionVarient
	{
		public Register Register;
		public byte Bit;

		public TestBit(Register register, byte bit)
		{
			Register = register;
			Bit = bit;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCB, (byte)(0x40 + 8 * Bit + (int)Register) };
		}
	}
}
