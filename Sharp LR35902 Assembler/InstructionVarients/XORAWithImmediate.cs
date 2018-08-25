namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class XORAWithImmediate : InstructionVarient
	{
		public readonly byte Byte;

		public XORAWithImmediate(byte @byte)
		{
			Byte = @byte;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xEE, Byte };
		}
	}
}
