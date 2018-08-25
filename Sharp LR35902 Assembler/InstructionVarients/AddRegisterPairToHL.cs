namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AddRegisterPairToHL : InstructionVarient
    {
		public readonly RegisterPair RegisterPair;

		public AddRegisterPairToHL(RegisterPair registerpair)
		{
			RegisterPair = registerpair;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x09 + 0x10 * (int)RegisterPair) };
		}
	}
}
