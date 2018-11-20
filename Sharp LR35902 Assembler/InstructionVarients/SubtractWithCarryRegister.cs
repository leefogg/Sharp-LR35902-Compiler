namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class SubtractWithCarryRegister : InstructionVarient
	{
		private Register Register;

		public SubtractWithCarryRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x98 + (int)Register) };
		}
	}
}
