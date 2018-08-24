namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ComplementA : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0x2F };
		}
	}
}
