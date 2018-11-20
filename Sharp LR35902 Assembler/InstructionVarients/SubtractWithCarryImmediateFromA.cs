namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class SubtractWithCarryImmediate : InstructionVarient
	{
		private byte Immediate;

		public SubtractWithCarryImmediate(byte immdediate)
		{
			Immediate = immdediate;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xDE, Immediate };
		}
	}
}
