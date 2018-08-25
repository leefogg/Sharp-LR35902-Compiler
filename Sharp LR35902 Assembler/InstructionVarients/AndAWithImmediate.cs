namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AndAWithImmediate : InstructionVarient
	{
		public readonly byte Byte;

		public AndAWithImmediate(byte @byte)
		{
			Byte = @byte;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xE6, Byte };
		}
	}
}
