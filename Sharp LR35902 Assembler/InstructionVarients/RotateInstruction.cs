namespace Sharp_LR35902_Assembler.InstructionVarients
{
	abstract class RotateInstruction : InstructionVarient
	{
		private readonly Register Register;
		private readonly byte StartRowOpcode;

		internal RotateInstruction(byte startrowopcode, Register register)
		{
			StartRowOpcode = startrowopcode;
			Register = register;
		}

		public override byte[] Compile()
		{
			if (Register == Register.A)
				return new[] { (byte)(StartRowOpcode + 7) };

			return new[] { (byte)0xCB, (byte)(StartRowOpcode + Register) };
		}
	}
}
