namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class SubtractImmediate : InstructionVarient
	{
		private byte Immediate;

		public SubtractImmediate(byte immdediate)
		{
			Immediate = immdediate;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xD6, Immediate };
		}
	}
}
