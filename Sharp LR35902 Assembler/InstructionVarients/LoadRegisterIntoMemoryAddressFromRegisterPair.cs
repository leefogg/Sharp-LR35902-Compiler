using Common.Extensions;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadRegisterIntoMemoryAddressFromRegisterPair : InstructionVarient
	{
		public readonly RegisterPair From;
		public readonly ushort Location;

		public LoadRegisterIntoMemoryAddressFromRegisterPair(ushort location, RegisterPair from)
		{
			From = from;
			Location = location;
		}

		public override byte[] Compile()
		{
			var locationbytes = Location.ToByteArray();
			return new[] { (byte)(0x01 + (0x10 * (int)From)), locationbytes[0], locationbytes[1] };
		}
	}
}
