using System;

namespace Common.Extensions
{
	public static class PrimitiveExtensions
	{
		public static bool isByte(this ushort self)
		{
			return self < 256;
		}

		public static byte[] ToByteArray(this ushort self)
		{
			return new [] {
				(byte)( self & (0xFF << (8 * 0))),
				(byte)((self & (0xFF << (8 * 1))) >> (8 * 1)),
			};
		}

		public static bool StartsWith(this string self, char character) => self.Length > 0 && self[0] == character;
		public static bool EndsWith(this string self, char character) => self.Length > 0 && self[self.Length-1] == character;

		public static byte[] GetHexBytes(this string hex)
		{
			if (hex.Length % 2 == 1)
				throw new ArgumentException("The hex code cannot have an odd number of digits");

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
				arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));

			return arr;
		}

		private static int GetHexVal(char hex) => hex - (hex <= '9' ? 48 : 55);
	}
}
