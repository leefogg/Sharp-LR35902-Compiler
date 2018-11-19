namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ReletiveJump : InstructionVarient
	{
		private byte Direction; // TODO: Change to sbyte

		public ReletiveJump(byte direction)
		{
			Direction = direction;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0x18, Direction };
		}
	}
}
