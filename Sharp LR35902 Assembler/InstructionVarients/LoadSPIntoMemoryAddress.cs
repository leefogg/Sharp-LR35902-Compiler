using Common.Extensions;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadSPIntoMemoryAddress : InstructionVarient
	{
		private readonly ushort Location;

		public LoadSPIntoMemoryAddress(ushort location)
		{
			Location = location;
		}

		public override byte[] Compile()
		{
			var bytes = Location.ToByteArray();
			return new byte[] { 0x08, bytes[0], bytes[1] };
		}
	}
}
