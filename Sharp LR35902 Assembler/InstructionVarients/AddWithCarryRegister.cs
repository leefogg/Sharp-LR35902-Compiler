namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AddWithCarryRegister : InstructionVarient
	{
		private Register Register;

		public AddWithCarryRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x88 + (int)Register) };
		}
	}
}
