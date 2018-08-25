namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AddRegisterToA : InstructionVarient
	{
		private readonly Register From;

		public AddRegisterToA(Register from)
		{
			From = from;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x80 + From) };
		}
	}
}
