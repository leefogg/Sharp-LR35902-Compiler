using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Exceptions;
using Common.Extensions;
using static Common.Extensions.IEnumerableExtensions;

namespace Sharp_LR35902_Assembler
{
	public class Assembler
	{
		private static readonly Dictionary<string, ushort> Definitions = new Dictionary<string, ushort>();

		private static readonly string[] registers = new[] { "B", "C", "D", "E", "H", "L", "(HL)", "A" };
		private static readonly string[] registerPairs = new[] { "BC", "DE", "HL", "SP" };
		private static readonly string[] conditions = new[] { "NZ", "Z", "NC", "C" };

		private static readonly Dictionary<string, Func<string[], byte[]>> Instructions = new Dictionary<string, Func<string[], byte[]>> {
			{ "NOP", NoOp },
			{ "STOP", Stop },
			{ "HALT", Halt },
			{ "DI", DisableInterrupts },
			{ "EI", EnableInterrupts },
			{ "RET", Return },
			{ "RETI", ReturnWithInterrrupts },
			{ "DAA", BCDAdjustA },
			{ "LD", Load },
			{ "XOR", XOR },
			{ "ADD", Add },
			{ "ADC", AddWithCarry },
			{ "SUB", Subtract },
			{ "SBC", SubtractWithCarry },
			{ "INC", Increment },
			{ "DEC", Decrement },
			{ "CP", Compare },
			{ "AND", And },
			{ "OR", Or },
			{ "RST", Reset },
			{ "SCF", SetCarryFlag },
			{ "CPL", ComplementA },
			{ "CCF", ClearCarryFlag },
			{ "CALL", Call },
			{ "PUSH", Push },
			{ "POP", Pop },
			{ "JP", Jump },
			{ "JR", JumpRelative },
			{ "LDI", LoadAndIncrement },
			{ "LDD", LoadAndDecrement },
			{ "LDH", LoadHiger },
			{ "LDHL", AddSPIntoHL },
			// CB instructions
			{ "RL", RotateLeft},
			{ "RR", RotateRight},
			{ "BIT", TestBit },
			{ "RES", ClearBit },
			{ "SET", SetBit },
			{ "SWAP", SwapNybbles },
			{ "SLA", ShiftLeftPreserveSign },
			{ "SRA", ShiftRightPreserveSign },
			{ "SRL", ShiftRight },
			{ "RRC", RotateRightWithCarry },
			{ "RLC", RotateLeftWithCarry }
		};
		private static readonly Dictionary<string, ushort> LabelLocations = new Dictionary<string, ushort>();
		private static readonly List<Tuple<ushort, string>> UnknownLocations = new List<Tuple<ushort, string>>(); // Tuple<location, immediateExpression>
		private static ushort	CurrentLocation = 0;

		public static void Main(string[] args)
		{
			string inputpath = null, outputpath = null;
			for (var i=0; i<args.Length; i++)
			{
				switch(args[i])
				{
					case "-i":
						inputpath = args[++i];
						break;
					case "-o":
						outputpath = args[++i];
						break;
					default:
						Console.WriteLine($"Unknown switch '{args[i]}'");
						return;
				}
			}

			if (inputpath == null)
			{
				Console.WriteLine("Input path not set");
				return;
			}
			if (outputpath == null)
			{
				Console.WriteLine("Output path not set");
				return;
			}


			var instructions = new List<string>(File.ReadAllLines(inputpath));
			Formatter.Format(instructions);
			Optimizer.Optimize(instructions);
			var bytecode = CompileProgram(instructions);
			using (var outputfile = File.Create(outputpath))
				outputfile.Write(bytecode, 0, bytecode.Length);
		}

		private static ArgumentException TooFewOprandsException(int expectednumber) => new ArgumentException($"Expected {expectednumber} oprands");
		private static ArgumentException NoOprandMatchException => new ArgumentException("No known oprand match found");
		private static ArgumentException UnexpectedInt16Exception => throw new ArgumentException($"Unexpected 16-bit immediate, expected 8-bit immediate.");

		#region Instructions
		// Common patterns for opcode ranges
		private static byte[] Pattern_BIT(string[] oprands, byte startopcode)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex == -1)
				throw new ArgumentException("Expected register for oprand 2");

			ushort bit = 0;
			if (TryParseImmediate(oprands[0], ref bit))
			{
				if (bit > 7)
					throw new ArgumentException("Unkown bit '{oprands[0]}'. Expected bit 0-7 inclusive");

				return ListOf<byte>(0xCB, (byte)(startopcode + (8 * bit) + registerindex));
			}

			throw new ArgumentException("No known oprand match found");
		}
		private static byte[] Pattern_Line(string[] oprands, byte startopcode)
		{
			if (oprands.Length != 1)
				throw new ArgumentException("Expected 1 register oprand");

			var registerindex = registers.IndexOf(oprands[0]);
			if (registerindex == -1)
				throw new ArgumentException("Oprand 1 is not a register");

			return ListOf<byte>(0xCB, (byte)(startopcode + registerindex));
		}
		private static byte[] Pattern_LineWithFastA(string[] oprands, byte rowstartopcode)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

			if (oprands[0] == "A")
				return ListOf((byte)(rowstartopcode + 7));

			var registerindex = registers.IndexOf(oprands[0]);
			return ListOf<byte>(0xCB, (byte)(rowstartopcode + registerindex));
		}
		private static byte[] Pattern_RegisterOrImmediateOnRegister(string[] oprands, byte rowstartopcode, byte nopcode)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

			var registerindex = registers.IndexOf(oprands[0]);
			if (registerindex == -1)
			{
				ushort immediate = 0;
				if (!TryParseImmediate(oprands[0], ref immediate))
					throw new ArgumentException($"Unknown register '{oprands[0]}'");
				if (!immediate.isByte())
					throw UnexpectedInt16Exception;

				return ListOf(nopcode, (byte)immediate);
			}


			return ListOf((byte)(rowstartopcode + registerindex));
		}

		private static byte[] NoOp(string[] oprands) => ListOf<byte>(0x00);
		private static byte[] Stop(string[] oprands) => ListOf<byte>(0x10);
		private static byte[] BCDAdjustA(string[] oprands) => ListOf<byte>(0x27);
		private static byte[] ComplementA(string[] oprands) => ListOf<byte>(0x2F);
		private static byte[] SetCarryFlag(string[] oprands) => ListOf<byte>(0x37);
		private static byte[] ClearCarryFlag(string[] oprands) => ListOf<byte>(0x3F);
		private static byte[] Halt(string[] oprands) => ListOf<byte>(0x76);
		private static byte[] ReturnWithInterrrupts(string[] oprands) => ListOf<byte>(0xD9);
		private static byte[] DisableInterrupts(string[] oprands) => ListOf<byte>(0xF3);
		private static byte[] EnableInterrupts(string[] oprands) => ListOf<byte>(0xFB);
		private static byte[] Return(string[] oprands) {
			if (oprands.Length > 1)
				throw new ArgumentException("Unexpected oprands");

			if (oprands.Length == 0)
				return ListOf<byte>(0xC9);

			var conditionindex = conditions.IndexOf(oprands[0]);
			if (conditionindex == -1)
				throw new ArgumentException($"Unexpected condition '{oprands[0]}'");

			return ListOf((byte)(0xC0 + (conditionindex * 8)));
		}
		private static byte[] Load(string[] oprands)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			// Assigning ushort to register pair
			// 0xn1
			ushort oprand2const = 0;
			if (TryParseImmediate(oprands[1], ref oprand2const) && !oprand2const.isByte())
			{
				var pairindex = registerPairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Register pair '{oprands[1]}' doesn't exist");

				var oprand2bytes = oprand2const.ToByteArray();
				return new[] { (byte)(0x01 + (0x10 * pairindex)), oprand2bytes[0], oprand2bytes[1] };
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
			// TODO: throw if oprand[1] isn't a immediate
			if (oprand1offset == -1)
			{
				ushort location = 0;
				if (!TryParseImmediate(TrimBrackets(oprands[0]), ref location))
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

				// Pointed to by immediate
				ushort location = 0;
				if (!TryParseImmediate(TrimBrackets(oprands[1]), ref location))
					throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint16.");
				var locationbytes = location.ToByteArray();
				return new byte[] { 0xFA, locationbytes[0], locationbytes[1] };
			}
			else if (oprand1offset != -1 && oprand2offset == -1) // Loading immediate into register (0xn6/0xnE)
			{
				// Assume its a immediate
				byte start = 0x06;
				var bytecode = start + (oprand1offset * 8);

				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate))
					throw new FormatException($"Oprand 2 '{oprands[1]}' is not a valid immediate");
				return new[] { (byte)bytecode, (byte)immediate };
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
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			if (oprands[0] == "HL")
			{
				var pairindex = registerPairs.IndexOf(oprands[1]);
				if (pairindex == -1)
					throw new ArgumentException($"Unrecognised register pair '{oprands[1]}'");

				return ListOf((byte)(0x09 + 0x10 * pairindex));
			}

			if (oprands[0] == "SP")
			{
				ushort immediate = 0;
				if (TryParseImmediate(oprands[1], ref immediate))
				{
					if (!immediate.isByte())
						throw UnexpectedInt16Exception;
				}
				else
					throw new ArgumentException($"Unexpected expression '{oprands[1]}'");

				return ListOf<byte>(0xE8, (byte)immediate);
			}

			if (oprands[0] != "A")
				throw new ArgumentException($"Cannot add into register '{oprands[0]}'. Can only add into register A and HL");

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex == -1)
			{
				ushort immediate = 0;
				if (TryParseImmediate(oprands[1], ref immediate))
				{
					if (!immediate.isByte())
						throw UnexpectedInt16Exception;
				} else
					throw new ArgumentException($"Unknown register '{oprands[1]}'");

				return ListOf<byte>(0xC6, (byte)immediate);
			}


			return ListOf((byte)(0x80 + registerindex));
		}
		private static byte[] AddWithCarry(string[] oprands)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			if (oprands[0] != "A")
				throw new ArgumentException($"Cannot add into register '{oprands[0]}'. Can only add into register A");

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex == -1)
			{
				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate))
					throw new ArgumentException($"Unknown register '{oprands[1]}'");
				if (!immediate.isByte())
					throw UnexpectedInt16Exception;

				return ListOf<byte>(0xCE, (byte)immediate);
			}

			return ListOf((byte)(0x88 + registerindex));
		}
		private static byte[] Subtract(string[] oprands)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			if (oprands[0] != "A")
				throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex > -1)
				return ListOf((byte)(0x90 + registerindex));

			ushort immediate = 0;
			if (TryParseImmediate(oprands[1], ref immediate))
				if (immediate.isByte())
					return ListOf<byte>(0xD6, (byte)immediate);
				else
					throw UnexpectedInt16Exception;

			throw new ArgumentException($"Unrecognised register '{oprands[1]}'");
		}
		private static byte[] SubtractWithCarry(string[] oprands)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			if (oprands[0] != "A")
				throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

			var registerindex = registers.IndexOf(oprands[1]);
			if (registerindex == -1)
			{
				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate))
					throw new ArgumentException($"Unknown register '{oprands[1]}'");
				if (!immediate.isByte())
					throw UnexpectedInt16Exception;

				return ListOf<byte>(0xDE, (byte)immediate);
			}

			return ListOf((byte)(0x98 + registerindex));
		}
		private static byte[] XOR(string[] oprands) => Pattern_RegisterOrImmediateOnRegister(oprands, 0xA8, 0xEE);
		private static byte[] Increment(string[] oprands)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

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
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

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
		private static byte[] Compare(string[] oprands) => Pattern_RegisterOrImmediateOnRegister(oprands, 0xB8, 0xFE);
		private static byte[] And(string[] oprands) => Pattern_RegisterOrImmediateOnRegister(oprands, 0xA0, 0xE6);
		private static byte[] Or(string[] oprands) => Pattern_RegisterOrImmediateOnRegister(oprands, 0xB0, 0xF6);
		private static byte[] Reset(string[] oprands)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

			var vectors = new[] { "0", "8", "10", "18", "20", "28", "30", "38" };
			var vectorindex = vectors.IndexOf(oprands[0]);
			if (vectorindex == -1)
				throw new ArgumentException($"Unknown reset vector '{oprands[0]}'");

			return ListOf((byte)(0xC7 + 8 * vectorindex));
		}
		private static byte[] Call(string[] oprands)
		{
			if (oprands.Length == 0)
				throw new ArgumentException("Expected at least 1 oprand");

			var conditionindex = conditions.IndexOf(oprands[0]);
			if (conditionindex == -1)
			{
				ushort address = 0;
				if (!TryParseImmediate(oprands[0], ref address))
					UnknownLocations.Add(new Tuple<ushort, string>((ushort)(CurrentLocation + 1), oprands[0]));

				var addressbytes = address.ToByteArray();
				return ListOf<byte>(0xCD, addressbytes[0], addressbytes[1]);
			}

			ushort immediate = 0;
			if (!TryParseImmediate(oprands[1], ref immediate))
				UnknownLocations.Add(new Tuple<ushort, string>((ushort)(CurrentLocation + 1), oprands[1]));

			var immediatebytes = immediate.ToByteArray();
			return ListOf((byte)(0xC4 + 8 * conditionindex), immediatebytes[0], immediatebytes[1]);
		}
		private static byte[] Push(string[] oprands)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

			string[] pairs = new[] { "BC", "DE", "HL", "AF" };  // Different to static list
			var pairindex = pairs.IndexOf(oprands[0]);
			if (pairindex == -1)
				throw new ArgumentException($"Unknown register pair '{oprands[0]}'");

			return ListOf((byte)(0xC5 + 0x10 * pairindex));
		}
		private static byte[] Pop(string[] oprands)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

			string[] pairs = new[] { "BC", "DE", "HL", "AF" }; // Different to static list
			var pairindex = pairs.IndexOf(oprands[0]);
			if (pairindex == -1)
				throw new ArgumentException($"Unknown register pair '{oprands[0]}'");

			return ListOf((byte)(0xC1 + 0x10 * pairindex));
		}
		private static byte[] Jump(string[] oprands)
		{
			if (oprands.Length == 0)
				throw new ArgumentException("Expected at least 1 oprand");

			if (oprands[0] == "(HL)")
				return ListOf<byte>(0xE9);

			var conditionindex = conditions.IndexOf(oprands[0]);
			ushort address = 0;
			if (conditionindex == -1)
			{
				if (!TryParseImmediate(oprands[0], ref address))
					UnknownLocations.Add(new Tuple<ushort, string>((ushort)(CurrentLocation + 1), oprands[0]));

				var addressbytes = address.ToByteArray();
				return ListOf<byte>(0xC3, addressbytes[0], addressbytes[1]);
			}

			if (!TryParseImmediate(oprands[1], ref address))
				UnknownLocations.Add(new Tuple<ushort, string>((ushort)(CurrentLocation+1), oprands[1]));

			var immediatebytes = address.ToByteArray();
			return ListOf((byte)(0xC2 + 8 * conditionindex), immediatebytes[0], immediatebytes[1]);
		}
		private static byte[] JumpRelative(string[] oprands)
		{
			if (oprands.Length == 0)
				throw new ArgumentException("Expected at least 1 oprand");

			var conditionindex = conditions.IndexOf(oprands[0]);
			if (conditionindex == -1)
			{
				ushort address = 0;
				if (!TryParseImmediate(oprands[0], ref address))
					throw new ArgumentException($"Unknown expression '{oprands[0]}'");

				if (!address.isByte())
					throw new ArgumentException("Can only jump back 127 and forward 128");

				var addressbytes = address.ToByteArray();
				return ListOf<byte>(0x18, addressbytes[0]);
			}

			ushort immediate = 0;
			if (TryParseImmediate(oprands[1], ref immediate))
			{
				if (!immediate.isByte())
					throw UnexpectedInt16Exception;
			} else
				throw new ArgumentException($"Unknown condition '{oprands[1]}'");

			var immediatebytes = immediate.ToByteArray();
			return ListOf((byte)(0x20 + 8 * conditionindex), immediatebytes[0]);
		}
		private static byte[] LoadAndIncrement(string[] oprands)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			if (oprands[0] == "A" && oprands[1] == "(HL)")
				return ListOf<byte>(0x2A);
			if (oprands[0] == "(HL)" && oprands[1] == "A")
				return ListOf<byte>(0x22);

			throw new ArgumentException("No known oprand match found");
		}
		private static byte[] LoadAndDecrement(string[] oprands) {
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			if (oprands[0] == "A" && oprands[1] == "(HL)")
				return ListOf<byte>(0x3A);
			if (oprands[0] == "(HL)" && oprands[1] == "A")
				return ListOf<byte>(0x32);

			throw new ArgumentException("No known oprand match found");
		}
		private static byte[] LoadHiger(string[] oprands)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			ushort addressoffset = 0;
			if (oprands[1] == "A")
			{
				if (oprands[0] == "(C)")
					return ListOf<byte>(0xE2);

				if (TryParseImmediate(TrimBrackets(oprands[0]), ref addressoffset))
				{
					if (!addressoffset.isByte())
						throw UnexpectedInt16Exception;

					return ListOf<byte>(0xE0, (byte)addressoffset);
				}
			}
			else if (TryParseImmediate(TrimBrackets(oprands[1]), ref addressoffset))
			{
				if (!addressoffset.isByte())
					throw UnexpectedInt16Exception;

				return ListOf<byte>(0xF0, (byte)addressoffset);
			}

			throw new ArgumentException("No known oprand match found");
		}
		private static byte[] AddSPIntoHL(string[] oprands)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			if (oprands[0] != "SP")
				throw new ArgumentException("Can only add 8-bit immediate to SP");

			ushort immediate = 0;
			if (TryParseImmediate(oprands[1], ref immediate))
			{
				if (!immediate.isByte())
					throw UnexpectedInt16Exception;

				return ListOf<byte>(0xF8, (byte)immediate);
			}

			throw NoOprandMatchException;
		}
		// CB instructions
		private static byte[] RotateLeftWithCarry(string[] oprands) => Pattern_LineWithFastA(oprands, 0x00);
		private static byte[] RotateRightWithCarry(string[] oprands) => Pattern_LineWithFastA(oprands, 0x08);
		private static byte[] RotateLeft(string[] oprands) => Pattern_LineWithFastA(oprands, 0x10);
		private static byte[] RotateRight(string[] oprands) => Pattern_LineWithFastA(oprands, 0x18);
		private static byte[] ShiftLeftPreserveSign(string[] oprands) => Pattern_Line(oprands, 0x20);
		private static byte[] ShiftRightPreserveSign(string[] oprands) => Pattern_Line(oprands, 0x28);
		private static byte[] SwapNybbles(string[] oprands) => Pattern_Line(oprands, 0x30);
		private static byte[] ShiftRight(string[] oprands) => Pattern_Line(oprands, 0x38);
		private static byte[] TestBit(string[] oprands) => Pattern_BIT(oprands, 0x40);
		private static byte[] ClearBit(string[] oprands) => Pattern_BIT(oprands, 0x80);
		private static byte[] SetBit(string[] oprands) => Pattern_BIT(oprands, 0xC0);
		#endregion

		public static byte[] CompileProgram(List<string> instructions)
		{
			Definitions.Clear();
			UnknownLocations.Clear();
			LabelLocations.Clear();

			// Assume resulting binary will be 2x number of instructions.
			// On average, instructions are 2-bytes long
			// If we under-estimated, array shouldn't resize more than once
			var bytecode = new List<byte>(instructions.Count * 2);

			foreach (var instruction in instructions)
			{
				var upperinstruction = instruction.ToUpper();
				if (upperinstruction.StartsWith("#DEFINE")) {
					var parts = upperinstruction.Split(' ');
					SetDefintion(parts[1], parts[2]);
					continue;
				} 
				else if (instruction.EndsWith(':'))
				{
					var labelname = upperinstruction.Substring(0, upperinstruction.LastIndexOf(':'));
					LabelLocations.Add(labelname, (ushort)bytecode.Count);
					continue;
				}

				var assembledinstruction = CompileInstruction(upperinstruction);
				bytecode.AddRange(assembledinstruction);
				CurrentLocation = (ushort)bytecode.Count;
			}

			// Resolve unknown label locations now we should have seen them all
			foreach (var locationdetails in UnknownLocations)
			{
				var binarylocation = locationdetails.Item1;
				var labelname = locationdetails.Item2;

				if (!LabelLocations.ContainsKey(labelname))
					throw new NotFoundException($"Label {labelname} not found.");

				var labellocation = LabelLocations[labelname];
				var labellocationbytes = labellocation.ToByteArray();
				bytecode[binarylocation] = labellocationbytes[0];
				bytecode[binarylocation + 1] = labellocationbytes[1];
			}

			return bytecode.ToArray();
		}

		public static byte[] CompileInstruction(string code) {
			var parts = code
				.Replace(',', ' ')
				.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			return CompileInstruction(parts[0], parts.Skip(1).ToArray());
		}
		public static byte[] CompileInstruction(string opcode, string[] oprands)
		{
			try
			{
				Func<string[], byte[]> method = NoOp;

				if (!Instructions.TryGetValue(opcode, out method))
					throw new NotFoundException($"Instruction '{opcode}' not found");

				return method(oprands);
			}
			catch (ArgumentException ee)
			{
				throw new SyntaxException("Unable to compile malformed instruction", ee);
			}
		}

		public static void SetDefintion(string key, string value = "0")
		{
			ushort ushortval = 0;
			if (!TryParseImmediate(value, ref ushortval))
				throw new ArgumentException("Definitions may only be integer");

			SetDefintion(key, ushortval);
		}
		public static void SetDefintion(string key, ushort value) {
			if (Definitions.ContainsKey(key))
				Console.WriteLine($"Warning: Redeclaration of global constant '{key}.'");

			Definitions[key] = value;
		}

		public static bool TryParseImmediate(string immediate, ref ushort result)
		{
			var res = result; // Working copy. Both to avoid global writes and to ensure result isn't changed unless successful

			var parts = immediate.SplitAndKeep(new[] { '+', '-' }).ToArray();
			immediate = parts[0];

			if (!Sharp_LR35902_Compiler.Parser.TryParseImmediate(immediate, ref res))
			{
				if (!Definitions.ContainsKey(immediate))
					return false;

				res = Definitions[immediate];
			}

			if (parts.Length > 1)
			{
				ushort oprand = 0;
				if (!TryParseImmediate(parts[2], ref oprand))
					return false;

				switch(parts[1])
				{
					case "+":
						res += oprand;
						break;
					case "-":
						res -= oprand;
						break;
					default:
						throw new NotImplementedException("Oprand not supported");
				}
			}

			result = res;
			return true;
		}

		private static string TrimBrackets(string location) => location.Substring(1, location.Length - 2);
		private static bool IsLocation(string expression) => expression.StartsWith('(') && expression.EndsWith(')');
	}
}
