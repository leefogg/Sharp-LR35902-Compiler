namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AddImmediateToA : InstructionVarient
	{
		public readonly byte Immediate;

		public AddImmediateToA(byte immdiate)
		{
			Immediate = immdiate;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)0xC6, Immediate };
		}
	}
}
