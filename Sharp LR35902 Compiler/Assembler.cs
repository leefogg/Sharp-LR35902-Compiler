using Sharp_LR35902_Compiler.Extensions;
using static Sharp_LR35902_Compiler.Extensions.IEnumerableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharp_LR35902_Compiler
{
	public class Assembler
	{
		private static readonly string[] registers = new[] { "B", "C", "D", "E", "H", "L", "(HL)", "A" };
		private static readonly string[] registerPairs = new[] { "BC", "DE", "HL", "SP" };

		private static readonly Dictionary<string, Func<string[], byte[]>> Instructions = new Dictionary<string, Func<string[], byte[]>> { 
			{ "NOP", NoOp },
			{ "STOP", Stop },
			{ "DI", DisableInterrupts },
			{ "EI", EnableInterrupts },
			{ "RET", Return },
			{ "RETI", ReturnWithInterrrupts },
			{ "LD", Load },
			{ "HALT", Halt },
			{ "XOR", XOR },
			{ "ADD", Add },
			{ "ADC", AddWithCarry },
			{ "SUB", Subtract },
			{ "SBC", SubtractWithCarry },
			{ "INC", Increment },
			{ "DEC", Decrement },
			{ "CP", Compare },
			{ "OR", OR },
			{ "RL", RotateLeft},
			{ "RR", RotateRight},
			{ "RST", Reset },
			{ "SCF", SetCarryFlag },
			{ "CPL", ComplementA },
			{ "DAA", BCDAdjustA },
			{ "CCF", ClearCarryFlag }
		};

		private static byte[] NoOp(string[] oprands) => ListOf<byte>(0x00);
		private static byte[] DisableInterrupts(string[] oprands) => ListOf<byte>(0xF3);
		private static byte[] EnableInterrupts(string[] oprands) => ListOf<byte>(0xFB);
		private static byte[] ReturnWithInterrrupts(string[] oprands) => ListOf<byte>(0xD9);
		private static byte[] Return(string[] oprands) {
			if (oprands.Length == 0)
				return ListOf<byte>(0xC9);

			string[] conditions = new[] { "NZ", "Z", "NC", "C" };

			var conditionindex = conditions.IndexOf(oprands[0]);
			if (conditionindex == -1)
				throw new ArgumentException($"Unexpected condition '{oprands[0]}'");

			return ListOf((byte)(0xC0 + (conditionindex * 8)));
		}
		private static byte[] Halt(string[] oprands) => ListOf<byte>(0x76);
		private static byte[] Stop(string[] oprands) => ListOf<byte>(0x10);
		private static byte[] Load(string[] oprands)
		{
			// Assigning ushort to register pair
			// 0xn1
			ushort oprand2const = 0;
			if (TryParseConstant(oprands[1], ref oprand2const) && !oprand2const.isByte())
			{
				var pairindex = registerPairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Register pair '{oprands[1]}' doesn't exist");

				var oprand2bytes = oprand2const.ToByteArray();
				return new[] { (byte)(0x01 + (0x10 * pairindex)), oprand2bytes[0], oprand2bytes[1]};
			}

			// Loading L into memory location at register pair
			if (oprands[0] == "(BC)")
				return ListOf<byte>(0x02);
			if (oprands[0] == "(DE)")
				return ListOf<byte>(0x12);

			// Loading HL into SP
			if (oprands[0] == "SP" && oprands[1] == "HL")
				return ListOf<byte>(0xF9);

			var oprand1offset = registers.IndexOf(oprands[0]);
			var oprand2offset = registers.IndexOf(oprands[1]);
			// TODO: throw if oprand[1] isn't a constant
			if (oprand1offset == -1)
			{
				ushort location = 0;
				if (!TryParseConstant(TrimBrackets(oprands[0]), ref location))
					throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint16.");

				var locationbytes = location.ToByteArray();
				if (oprands[1] == "A")
					return ListOf<byte>(0xEA, locationbytes[0], locationbytes[1]);
				if (oprands[1] == "SP")
					return ListOf<byte>(0x08, locationbytes[0], locationbytes[1]);
			}
			if (oprands[0] == "A" && oprand2offset == -1 && IsLocation(oprands[1])) // Loading memory value into A
			{
				// Pointed to by register pair
				if (oprands[1] == "(BC)")
					return ListOf<byte>(0x0A);
				if (oprands[1] == "(DE)")
					return ListOf<byte>(0x1A);

				// Pointed to by constant
				ushort location = 0;
				if (!TryParseConstant(TrimBrackets(oprands[1]), ref location))
					throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint16.");
				var locationbytes = location.ToByteArray();
				return new byte[] { 0xFA, locationbytes[0], locationbytes[1] };
			}
			else if (oprand2offset == -1) // Loading constant into register (0xn6/0xnE)
			{
				// Assume its a constant
				byte start = 0x06;
				var bytecode = start + (oprand1offset * 8);

				ushort constant = 0;
				if (!TryParseConstant(oprands[1], ref constant))
					throw new FormatException($"Oprand 2 '{oprands[1]}' is not a valid constant");
				return new[] { (byte)bytecode, (byte)constant };
			}
			else // 0x40 - 0x6F
			{
				// Both oprands are registers
				byte topleft = 0x40;
				var bytecode = topleft + (oprand1offset * registers.Length) + oprand2offset;
				return new[] { ((byte)bytecode) };
			}
		}
		private static byte[] Add(string[] oprands)
		{
			if (oprands[0] == "HL")
			{
				var pairindex = registerPairs.IndexOf(oprands[1]);
				if (pairindex == -1)
					throw new ArgumentException($"Unrecognised register pair '{oprands[1]}'");

				return ListOf((byte)(0x09 + 0x10 * pairindex));
			}

			if (oprands[0] == "SP")
			{
				ushort constant = 0;
				if (!TryParseConstant(oprands[1], ref constant))
					throw new ArgumentException($"Unexpected expression '{oprands[1]}'");

				return ListOf<byte>(0xE8, (byte)constant);
			}

			if (oprands[0] != "A")
				throw new ArgumentException($"Cannot add into register '{oprands[0]}'. Can only add into register A and HL");

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex == -1)
			{
				ushort constant = 0;
				if (!TryParseConstant(oprands[1], ref constant))
					throw new ArgumentException($"Unknown register '{oprands[1]}'");

				return ListOf<byte>(0xC6, (byte)constant);
			}


			return ListOf((byte)(0x80 + registerindex));
		}
		private static byte[] AddWithCarry(string[] oprands)
		{
			if (oprands[0] != "A")
				throw new ArgumentException($"Cannot add into register '{oprands[0]}'. Can only add into register A");

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex == -1)
			{
				ushort constant = 0;
				if (!TryParseConstant(oprands[1], ref constant))
					throw new ArgumentException($"Unknown register '{oprands[1]}'");

				return ListOf<byte>(0xCE, (byte)constant);
			}

			return ListOf((byte)(0x88 + registerindex));
		}
		private static byte[] Subtract(string[] oprands)
		{
			ushort constant = 0;
			if (TryParseConstant(oprands[1], ref constant))
				return ListOf<byte>(0xD6, (byte)constant);

			if (oprands[0] != "A")
				throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex == -1)
				throw new ArgumentException($"Unrecognised register '{oprands[1]}'");

			return ListOf((byte)(0x90 + registerindex));
		}
		private static byte[] SubtractWithCarry(string[] oprands)
		{
			if (oprands[0] != "A")
				throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex == -1)
			{
				ushort constant = 0;
				if (!TryParseConstant(oprands[1], ref constant))
					throw new ArgumentException($"Unknown register '{oprands[1]}'");

				return ListOf<byte>(0xDE, (byte)constant);
			}

			return ListOf((byte)(0x98 + registerindex));
		}
		private static byte[] XOR(string[] oprands)
		{
			var registerindex = registers.IndexOf(oprands[0]);
			if (registerindex == -1) {
				ushort constant = 0;
				if (!TryParseConstant(oprands[0], ref constant))
					throw new ArgumentException($"Unknown register '{oprands[0]}'");

				return ListOf<byte>(0xEE, (byte)constant);
			}
				

			return ListOf((byte)(0xA8 + registerindex));
		}
		private static byte[] Increment(string[] oprands)
		{
			var registerindex = registers.IndexOf(oprands[0]);
			if (registerindex == -1)
			{
				var pairindex = registerPairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register '{oprands[0]}'");

				return ListOf((byte)(0x03 + 0x10 * pairindex));
			}

			return ListOf((byte)(0x04 + 8 * registerindex));
		}
		private static byte[] Decrement(string[] oprands)
		{
			var registerindex = registers.IndexOf(oprands[0]);
			if (registerindex == -1)
			{
				var pairindex = registerPairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register '{oprands[0]}'");

				return ListOf((byte)(0x0B + 0x10 * pairindex));
			}

			return ListOf((byte)(0x05 + 8 * registerindex));
		}
		private static byte[] Compare(string[] oprands)
		{
			var registerindex = registers.IndexOf(oprands[0]);
			if (registerindex == -1)
			{
				ushort constant = 0;
				if (!TryParseConstant(oprands[0], ref constant))
					throw new ArgumentException($"Unknown register '{oprands[0]}'");

				return ListOf<byte>(0xFE, (byte)constant);
			}


			return ListOf((byte)(0xB8 + registerindex));
		}
		private static byte[] OR(string[] oprands)
		{
			var registerindex = registers.IndexOf(oprands[0]);
			if (registerindex == -1)
			{
				ushort constant = 0;
				if (!TryParseConstant(oprands[0], ref constant))
					throw new ArgumentException($"Unknown register '{oprands[0]}'");

				return ListOf<byte>(0xF6, (byte)constant);
			}


			return ListOf((byte)(0xB0 + registerindex));
		}
		private static byte[] RotateLeft(string[] oprands)
		{
			if (oprands.Length != 1)
				throw new ArgumentException("Register expected");

			if (oprands[0] == "A")
				return ListOf<byte>(0x17);

			var registerindex = registers.IndexOf(oprands[0]);
			return ListOf<byte>(0xCB, (byte)(0x10 + registerindex));
		}
		private static byte[] RotateRight(string[] oprands)
		{
			if (oprands.Length != 1)
				throw new ArgumentException("Register expected");

			if (oprands[0] == "A")
				return ListOf<byte>(0x1F);

			var registerindex = registers.IndexOf(oprands[0]);
			return ListOf<byte>(0xCB, (byte)(0x18 + registerindex));
		}
		private static byte[] Reset(string[] oprands)
		{
			var vectors = new[] {"0", "8", "10", "18", "20", "28", "30", "38" };
			var vectorindex = vectors.IndexOf(oprands[0]);
			if (vectorindex == -1)
				throw new ArgumentException($"Unknown reset vector '{oprands[0]}'");

			return ListOf((byte)(0xC7 + 8 * vectorindex));
		}
		public static byte[] SetCarryFlag(string[] oprands) => ListOf<byte>(0x37);
		public static byte[] ComplementA(string[] oprands) => ListOf<byte>(0x2F);
		public static byte[] BCDAdjustA(string[] oprands) => ListOf<byte>(0x27);
		public static byte[] ClearCarryFlag(string[] oprands) => ListOf<byte>(0x3F);

		public static void Main(string[] args)
		{
			
		}

		public static byte[] CompileInstruction(string code) {
			var parts = code
				.ToUpper()
				.Replace(',',' ')
				.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			return CompileInstruction(parts[0], parts.Skip(1).ToArray());
		}

		public static byte[] CompileInstruction(string opcode, string[] oprands)
		{
			var bestguess = Instructions[opcode];
			return bestguess(oprands);
		}


		private static bool TryParseConstant(string constant, ref ushort result)
		{
			if (constant.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
			{
				var stripped = constant.Substring(2);
				result = stripped.GetHexBytes()[0];
				return true;
			}
			else if (constant.StartsWith("0b", StringComparison.InvariantCultureIgnoreCase))
			{
				// TODO: implement
				return false;
			}
			else
			{
				return ushort.TryParse(constant, out result);
			}
		}

		private static string TrimBrackets(string location) => location.Substring(1, location.Length - 2);
		public static bool IsLocation(string expression) => expression.StartsWith('(') && expression.EndsWith(')');
	}
}
