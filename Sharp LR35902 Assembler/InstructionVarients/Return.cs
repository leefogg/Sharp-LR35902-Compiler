namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class Return : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0xC9 };
		}
	}
}
