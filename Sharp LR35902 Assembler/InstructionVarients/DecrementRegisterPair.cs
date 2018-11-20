namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class DecrementRegisterPair : InstructionVarient
	{
		private RegisterPair RegisterPair;

		public DecrementRegisterPair(RegisterPair pairindex)
		{
			RegisterPair = pairindex;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x0B + 0x10 * (int)RegisterPair) };
		}
	}
}
