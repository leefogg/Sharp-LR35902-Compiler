namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class RotateLeftWithCarry : RotateInstruction
	{
		public RotateLeftWithCarry(Register register) : base(0x00, register)
		{
		}
	}
}
