namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class IncrementRegisterPair : InstructionVarient
	{
		private RegisterPair RegisterPair;

		public IncrementRegisterPair(RegisterPair pairindex)
		{
			RegisterPair = pairindex;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x03 + 0x10 * (int)RegisterPair) };
		}
	}
}
