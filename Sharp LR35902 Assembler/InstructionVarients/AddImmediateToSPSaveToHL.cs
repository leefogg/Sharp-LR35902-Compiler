namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AddImmediateToSPSaveToHL : InstructionVarient
	{
		public byte Immediate;

		public AddImmediateToSPSaveToHL(byte immedaite)
		{
			Immediate = immedaite;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xF8, Immediate };
		}
	}
}
