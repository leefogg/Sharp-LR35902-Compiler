namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class RotateRight : RotateInstruction
	{
		public RotateRight(Register register) : base(0x18, register)
		{
		}
	}
}
