namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class Reset : InstructionVarient
	{
		public static readonly string[] Vectors = new[] { "0", "8", "10", "18", "20", "28", "30", "38" };

		private readonly int VectorIndex;

		public Reset(int vectorindex)
		{
			VectorIndex = vectorindex;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0xC7 + 8 * VectorIndex) };
		}
	}
}
