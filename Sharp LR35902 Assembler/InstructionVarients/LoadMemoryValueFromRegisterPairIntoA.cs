namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadMemoryValueFromRegisterPairIntoA : InstructionVarient
	{
		public readonly RegisterPair RegisterPair;

		// Only really valid for BC and DE
		public LoadMemoryValueFromRegisterPairIntoA(RegisterPair registerpair)
		{
			RegisterPair = registerpair;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x0A + (int)RegisterPair * 0x10) };
		}
	}
}
