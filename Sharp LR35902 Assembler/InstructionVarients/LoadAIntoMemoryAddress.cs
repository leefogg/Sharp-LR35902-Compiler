using Common.Extensions;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadAIntoMemoryAddress : InstructionVarient
	{
		public readonly ushort Location;

		public LoadAIntoMemoryAddress(ushort location)
		{
			Location = location;
		}

		public override byte[] Compile()
		{
			var bytes = Location.ToByteArray();
			return new byte[] { 0xEA, bytes[0], bytes[1] };
		}
	}
}
