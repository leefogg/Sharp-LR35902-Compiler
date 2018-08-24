namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadAIntoMemoryAddressAtBC : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0x02 };
		}
	}
}
