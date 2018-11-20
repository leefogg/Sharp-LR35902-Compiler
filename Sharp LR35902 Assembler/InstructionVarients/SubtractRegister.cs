namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class SubtractRegister : InstructionVarient
	{
		private Register Register;

		public SubtractRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x90 + (int)Register) };
		}
	}
}
