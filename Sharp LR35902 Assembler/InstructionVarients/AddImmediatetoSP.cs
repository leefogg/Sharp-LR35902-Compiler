namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AddImmediateToSP : InstructionVarient
    {
		public readonly byte Immediate;

		public AddImmediateToSP(byte immediate)
		{
			Immediate = immediate;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xE8, Immediate };
		}
	}
}
