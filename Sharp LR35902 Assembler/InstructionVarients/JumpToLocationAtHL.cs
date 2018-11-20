namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class JumpToLocationAtHL : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0xE9 };
		}
	}
}
