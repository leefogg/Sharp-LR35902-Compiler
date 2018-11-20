namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadFromHigherMemoryAddressIntoA : InstructionVarient
	{
		private byte Address;

		public LoadFromHigherMemoryAddressIntoA(byte address)
		{
			Address = address;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xF0, Address };
		}
	}
}
