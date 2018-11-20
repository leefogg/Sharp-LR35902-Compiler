namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AddWithCarryImmediate : InstructionVarient
	{
		private byte Immediate;

		public AddWithCarryImmediate(byte immdediate)
		{
			Immediate = immdediate;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xCE, Immediate };
		}
	}
}
