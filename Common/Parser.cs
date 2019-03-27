using System;
using Common.Extensions;

namespace Common {
	public class Parser {
		public static ushort ParseImmediate(string immediate) {
			if (ushort.TryParse(immediate, out var result))
				return result;

			if (immediate.Equals("true", StringComparison.InvariantCultureIgnoreCase))
				return 1;

			if (immediate.Equals("false", StringComparison.InvariantCultureIgnoreCase))
				return 0;

			if (immediate.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) {
				if (immediate.Length == 2)
					throw new FormatException("Expected 2 hex characters after 0x");

				var stripped = immediate.Substring(2);
				var bytes = stripped.GetHexBytes();

				result = 0;
				for (byte i = 0; i < Math.Min(2, bytes.Length); i++)
					result |= (ushort)(bytes[i] << (i * 8));

				return result;
			}

			if (immediate.StartsWith("0b", StringComparison.InvariantCultureIgnoreCase)) {
				immediate = immediate.ToLower();
				var bitsindex = immediate.IndexOf('b') + 1;
				immediate = immediate.Substring(bitsindex);

				if (immediate.Length != 8 && immediate.Length != 16)
					throw new FormatException("Expected 8 or 16 binary digits after 0b");

				ushort bits = 0;
				for (byte i = 0; i < immediate.Length; i++) {
					var character = immediate[immediate.Length - 1 - i];
					if (character == '1')
						bits |= (ushort)(1 << i);
					else if (character != '0')
						throw new FormatException($"Unexpected binary digit '{character}'");
				}

				result = bits;

				return result;
			}

			throw new FormatException("Unknown immediate value format");
		}

		public static bool TryParseImmediate(string immediate, ref ushort value) {
			try {
				value = ParseImmediate(immediate);
				return true;
			} catch // This is a Try* method, we expect failure and just swallow the exception. Use non Try* method if you want the exception details
			{
				return false;
			}
		}
	}
}
