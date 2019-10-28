using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using Common.Exceptions;
using Common.Extensions;
using static Common.Extensions.IEnumerableExtensions;

namespace Sharp_LR35902_Assembler {
	public class Assembler {
		private static readonly string[] Registers = {"B", "C", "D", "E", "H", "L", "(HL)", "A"};
		private static readonly string[] RegisterPairs = {"BC", "DE", "HL", "SP"};
		private static readonly string[] Conditions = {"NZ", "Z", "NC", "C"};
		private static ArgumentException NoOprandMatchException => new ArgumentException("No known oprand match found");
		private static ArgumentException UnexpectedInt16Exception => throw new ArgumentException("Unexpected 16-bit immediate, expected 8-bit immediate.");
		private readonly Dictionary<string, ushort> Definitions = new Dictionary<string, ushort>();

		private readonly Dictionary<string, Func<string[], byte[]>> Instructions;
		private readonly Dictionary<string, ushort> LabelLocations = new Dictionary<string, ushort>();
		private ushort CurrentLocation;
		private bool FirstPass;

		// Common patterns for opcode ranges
		private byte[] Pattern_BIT(IReadOnlyList<string> oprands, byte startopcode)
		{
			if (oprands.Count != 2)
				throw TooFewOprandsException(2);

			var registerindex = Registers.IndexOf(oprands[1]);
			if (registerindex == -1)
				throw new ArgumentException("Expected register for oprand 2");

			ushort bit = 0;
			if (!TryParseImmediate(oprands[0], ref bit))
				throw new ArgumentException("No known oprand match found");

			if (bit > 7)
				throw new ArgumentException("Unkown bit '{oprands[0]}'. Expected bit 0-7 inclusive");

			return ListOf<byte>(0xCB, (byte)(startopcode + 8 * bit + registerindex));
		}

		private static byte[] Pattern_Line(IReadOnlyList<string> oprands, byte startopcode)
		{
			if (oprands.Count != 1)
				throw new ArgumentException("Expected 1 register oprand");

			var registerindex = Registers.IndexOf(oprands[0]);
			if (registerindex == -1)
				throw new ArgumentException("Oprand 1 is not a register");

			return ListOf<byte>(0xCB, (byte)(startopcode + registerindex));
		}

		private static byte[] Pattern_LineWithFastA(IReadOnlyList<string> oprands, byte rowstartopcode)
		{
			if (oprands.Count != 1)
				throw TooFewOprandsException(1);

			if (oprands[0] == "A")
				return ListOf((byte)(rowstartopcode + 7));

			var registerindex = Registers.IndexOf(oprands[0]);
			return ListOf<byte>(0xCB, (byte)(rowstartopcode + registerindex));
		}

		private byte[] Pattern_RegisterOrImmediateOnRegister(IReadOnlyList<string> oprands, byte rowstartopcode, byte nopcode)
		{
			if (oprands.Count != 1)
				throw TooFewOprandsException(1);

			var registerindex = Registers.IndexOf(oprands[0]);
			if (registerindex != -1)
				return ListOf((byte)(rowstartopcode + registerindex));

			ushort immediate = 0;
			if (!TryParseImmediate(oprands[0], ref immediate))
				throw new ArgumentException($"Unknown register '{oprands[0]}'");
			if (!immediate.isByte())
				throw UnexpectedInt16Exception;

			return ListOf(nopcode, (byte)immediate);
		}

		public Assembler() {
			byte[] NoOp(string[] oprands) => ListOf<byte>(0x00);
			byte[] Stop(string[] oprands) => ListOf<byte>(0x10);
			byte[] BCDAdjustA(string[] oprands) => ListOf<byte>(0x27);
			byte[] ComplementA(string[] oprands) => ListOf<byte>(0x2F);
			byte[] SetCarryFlag(string[] oprands) => ListOf<byte>(0x37);
			byte[] ClearCarryFlag(string[] oprands) => ListOf<byte>(0x3F);
			byte[] Halt(string[] oprands) => ListOf<byte>(0x76);
			byte[] ReturnWithInterrrupts(string[] oprands) => ListOf<byte>(0xD9);
			byte[] DisableInterrupts(string[] oprands) => ListOf<byte>(0xF3);
			byte[] EnableInterrupts(string[] oprands) => ListOf<byte>(0xFB);
			byte[] Return(string[] oprands) {
				if (oprands.Length > 1)
					throw new ArgumentException("Unexpected oprands");

				if (oprands.Length == 0)
					return ListOf<byte>(0xC9);

				var conditionindex = Conditions.IndexOf(oprands[0]);
				if (conditionindex == -1)
					throw new ArgumentException($"Unexpected condition '{oprands[0]}'");

				return ListOf((byte)(0xC0 + conditionindex * 8));
			}

			byte[] Load(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				// Assigning ushort to register pair
				// 0xn1
				ushort oprand2const = 0;
				if (RegisterPairs.Contains(oprands[0]) && TryParseImmediate(oprands[1], ref oprand2const)) {
					var pairindex = RegisterPairs.IndexOf(oprands[0]);
					if (pairindex == -1)
						throw new ArgumentException($"Register pair '{oprands[1]}' doesn't exist");

					var oprand2bytes = oprand2const.ToByteArray();
					return new[] {(byte)(0x01 + 0x10 * pairindex), oprand2bytes[0], oprand2bytes[1]};
				}

				// Loading L into memory location at register pair
				if (oprands[0] == "(BC)")
					return ListOf<byte>(0x02);
				if (oprands[0] == "(DE)")
					return ListOf<byte>(0x12);

				// Loading HL into SP
				if (oprands[0] == "SP" && oprands[1] == "HL")
					return ListOf<byte>(0xF9);

				var oprand1offset = Registers.IndexOf(oprands[0]);
				var oprand2offset = Registers.IndexOf(oprands[1]);
				// TODO: throw if oprand[1] isn't a immediate
				if (oprand1offset == -1) {
					ushort location = 0;
					if (!TryParseImmediate(TrimBrackets(oprands[0]), ref location))
						throw new ArgumentException($"Unexpected expression '{oprands[0]}', expected uint16.");

					var locationbytes = location.ToByteArray();
					if (oprands[1] == "A")
						return ListOf<byte>(0xEA, locationbytes[0], locationbytes[1]);
					if (oprands[1] == "SP")
						return ListOf<byte>(0x08, locationbytes[0], locationbytes[1]);
				}

				if (oprands[0] == "A" && oprand2offset == -1 && IsLocation(oprands[1])) // Loading memory value into A
				{
					// Pointed to by register pair
					switch (oprands[1]) {
						case "(BC)": return ListOf<byte>(0x0A);
						case "(DE)": return ListOf<byte>(0x1A);
					}

					// Pointed to by immediate
					ushort location = 0;
					if (!TryParseImmediate(TrimBrackets(oprands[1]), ref location))
						throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint16.");
					var locationbytes = location.ToByteArray();
					return new byte[] {0xFA, locationbytes[0], locationbytes[1]};
				}

				if (oprand1offset != -1 && oprand2offset == -1) // Loading immediate into register (0xn6/0xnE)
				{
					// Assume its a immediate
					var bytecode = 0x06 + oprand1offset * 8;

					ushort immediate = 0;
					if (!TryParseImmediate(oprands[1], ref immediate) || !immediate.isByte())
						throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint8.");

					return new[] {(byte)bytecode, (byte)immediate};
				} else // 0x40 - 0x6F
				{
					// Both oprands are registers
					var bytecode = 0x40 + oprand1offset * Registers.Length + oprand2offset;
					return new[] {(byte)bytecode};
				}
			}

			byte[] Add(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "HL") {
					var pairindex = RegisterPairs.IndexOf(oprands[1]);
					if (pairindex == -1)
						throw new ArgumentException($"Unrecognised register pair '{oprands[1]}'");

					return ListOf((byte)(0x09 + 0x10 * pairindex));
				}

				if (oprands[0] == "SP") {
					ushort immediate = 0;
					if (TryParseImmediate(oprands[1], ref immediate)) {
						if (!immediate.isByte())
							throw UnexpectedInt16Exception;
					} else {
						throw new ArgumentException($"Unexpected expression '{oprands[1]}'");
					}

					return ListOf<byte>(0xE8, (byte)immediate);
				}

				if (oprands[0] != "A")
					throw new ArgumentException($"Cannot add into register '{oprands[0]}'. Can only add into register A and HL");

				var registerindex = Registers.IndexOf(oprands[1]);
				if (registerindex == -1) {
					ushort immediate = 0;
					if (TryParseImmediate(oprands[1], ref immediate)) {
						if (!immediate.isByte())
							throw UnexpectedInt16Exception;
					} else {
						throw new ArgumentException($"Unknown register '{oprands[1]}'");
					}

					return ListOf<byte>(0xC6, (byte)immediate);
				}


				return ListOf((byte)(0x80 + registerindex));
			}

			byte[] AddWithCarry(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] != "A")
					throw new ArgumentException($"Cannot add into register '{oprands[0]}'. Can only add into register A");

				var registerindex = Registers.IndexOf(oprands[1]);
				if (registerindex != -1)
					return ListOf((byte)(0x88 + registerindex));

				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate))
					throw new ArgumentException($"Unknown register '{oprands[1]}'");
				if (!immediate.isByte())
					throw UnexpectedInt16Exception;

				return ListOf<byte>(0xCE, (byte)immediate);

			}

			byte[] Subtract(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] != "A")
					throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

				var registerindex = Registers.IndexOf(oprands[1]);
				if (registerindex > -1)
					return ListOf((byte)(0x90 + registerindex));

				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate))
					throw new ArgumentException($"Unrecognised register '{oprands[1]}'");
				if (immediate.isByte())
					return ListOf<byte>(0xD6, (byte)immediate);

				throw UnexpectedInt16Exception;
			}

			byte[] SubtractWithCarry(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] != "A")
					throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

				var registerindex = Registers.IndexOf(oprands[1]);
				if (registerindex != -1)
					return ListOf((byte)(0x98 + registerindex));

				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate))
					throw new ArgumentException($"Unknown register '{oprands[1]}'");
				if (!immediate.isByte())
					throw UnexpectedInt16Exception;

				return ListOf<byte>(0xDE, (byte)immediate);

			}

			byte[] XOR(string[] oprands) => Pattern_RegisterOrImmediateOnRegister(oprands, 0xA8, 0xEE);

			byte[] Increment(string[] oprands) {
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var registerindex = Registers.IndexOf(oprands[0]);
				if (registerindex != -1)
					return ListOf((byte)(0x04 + 8 * registerindex));

				var pairindex = RegisterPairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register '{oprands[0]}'");

				return ListOf((byte)(0x03 + 0x10 * pairindex));

			}

			byte[] Decrement(string[] oprands) {
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var registerindex = Registers.IndexOf(oprands[0]);
				if (registerindex != -1)
					return ListOf((byte)(0x05 + 8 * registerindex));

				var pairindex = RegisterPairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register '{oprands[0]}'");

				return ListOf((byte)(0x0B + 0x10 * pairindex));
			}

			byte[] Compare(string[] oprands) => Pattern_RegisterOrImmediateOnRegister(oprands, 0xB8, 0xFE);
			byte[] And(string[] oprands) => Pattern_RegisterOrImmediateOnRegister(oprands, 0xA0, 0xE6);
			byte[] Or(string[] oprands) => Pattern_RegisterOrImmediateOnRegister(oprands, 0xB0, 0xF6);
			byte[] Reset(string[] oprands) {
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var vectors = new[] {"0", "8", "10", "18", "20", "28", "30", "38"};
				var vectorindex = vectors.IndexOf(oprands[0]);
				if (vectorindex == -1)
					throw new ArgumentException($"Unknown reset vector '{oprands[0]}'");

				return ListOf((byte)(0xC7 + 8 * vectorindex));
			}

			byte[] Call(string[] oprands) {
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				var conditionindex = Conditions.IndexOf(oprands[0]);
				if (conditionindex == -1) {
					ushort address = 0;
					if (!TryParseImmediate(oprands[0], ref address))
						throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

					var addressbytes = address.ToByteArray();
					return ListOf<byte>(0xCD, addressbytes[0], addressbytes[1]);
				}

				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate))
					throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

				var immediatebytes = immediate.ToByteArray();
				return ListOf((byte)(0xC4 + 8 * conditionindex), immediatebytes[0], immediatebytes[1]);
			}

			byte[] Push(string[] oprands) {
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				string[] pairs = {"BC", "DE", "HL", "AF"}; // Different to static list
				var pairindex = pairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register pair '{oprands[0]}'");

				return ListOf((byte)(0xC5 + 0x10 * pairindex));
			}

			byte[] Pop(string[] oprands) {
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				string[] pairs = {"BC", "DE", "HL", "AF"}; // Different to static list
				var pairindex = pairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register pair '{oprands[0]}'");

				return ListOf((byte)(0xC1 + 0x10 * pairindex));
			}

			byte[] Jump(string[] oprands) {
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				if (oprands[0] == "(HL)")
					return ListOf<byte>(0xE9);

				var conditionindex = Conditions.IndexOf(oprands[0]);
				ushort address = 0;
				if (conditionindex == -1) {
					if (!TryParseImmediate(oprands[0], ref address))
						throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

					var addressbytes = address.ToByteArray();
					return ListOf<byte>(0xC3, addressbytes[0], addressbytes[1]);
				}

				if (!TryParseImmediate(oprands[1], ref address))
					throw new ArgumentException($"Unknown expression '{oprands[1]}'.");

				var immediatebytes = address.ToByteArray();
				return ListOf((byte)(0xC2 + 8 * conditionindex), immediatebytes[0], immediatebytes[1]);
			}

			byte[] JumpRelative(string[] oprands) {
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				var conditionindex = Conditions.IndexOf(oprands[0]);
				if (conditionindex == -1) {
					ushort address = 0;
					if (!TryParseImmediate(oprands[0], ref address))
						throw new ArgumentException($"Unknown expression '{oprands[0]}'");

					if (!address.isByte())
						throw new ArgumentException("Can only jump back 127 and forward 128");

					var addressbytes = address.ToByteArray();
					return ListOf<byte>(0x18, addressbytes[0]);
				}

				ushort immediate = 0;
				if (TryParseImmediate(oprands[1], ref immediate)) {
					if (!immediate.isByte())
						throw UnexpectedInt16Exception;
				} else {
					throw new ArgumentException($"Unknown condition '{oprands[1]}'");
				}

				var immediatebytes = immediate.ToByteArray();
				return ListOf((byte)(0x20 + 8 * conditionindex), immediatebytes[0]);
			}

			byte[] LoadAndIncrement(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "A" && oprands[1] == "(HL)")
					return ListOf<byte>(0x2A);
				if (oprands[0] == "(HL)" && oprands[1] == "A")
					return ListOf<byte>(0x22);

				throw new ArgumentException("No known oprand match found");
			}

			byte[] LoadAndDecrement(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "A" && oprands[1] == "(HL)")
					return ListOf<byte>(0x3A);
				if (oprands[0] == "(HL)" && oprands[1] == "A")
					return ListOf<byte>(0x32);

				throw new ArgumentException("No known oprand match found");
			}

			byte[] LoadHigher(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				ushort addressoffset = 0;
				if (oprands[1] == "A") {
					if (oprands[0] == "(C)")
						return ListOf<byte>(0xE2);

					if (!TryParseImmediate(TrimBrackets(oprands[0]), ref addressoffset))
						throw new ArgumentException("No known oprand match found");

					if (!addressoffset.isByte())
						throw UnexpectedInt16Exception;

					return ListOf<byte>(0xE0, (byte)addressoffset);
				} else if (TryParseImmediate(TrimBrackets(oprands[1]), ref addressoffset)) {
					if (!addressoffset.isByte())
						throw UnexpectedInt16Exception;

					return ListOf<byte>(0xF0, (byte)addressoffset);
				}

				throw new ArgumentException("No known oprand match found");
			}

			byte[] AddSPIntoHL(string[] oprands) {
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] != "SP")
					throw new ArgumentException("Can only add 8-bit immediate to SP");

				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate))
					throw NoOprandMatchException;

				if (!immediate.isByte())
					throw UnexpectedInt16Exception;

				return ListOf<byte>(0xF8, (byte)immediate);
			}

			// CB instructions
			byte[] RotateLeftWithCarry(string[] oprands) => Pattern_LineWithFastA(oprands, 0x00);
			byte[] RotateRightWithCarry(string[] oprands) => Pattern_LineWithFastA(oprands, 0x08);
			byte[] RotateLeft(string[] oprands) => Pattern_LineWithFastA(oprands, 0x10);
			byte[] RotateRight(string[] oprands) => Pattern_LineWithFastA(oprands, 0x18);
			byte[] ShiftLeftPreserveSign(string[] oprands) => Pattern_Line(oprands, 0x20);
			byte[] ShiftRightPreserveSign(string[] oprands) => Pattern_Line(oprands, 0x28);
			byte[] SwapNybbles(string[] oprands) => Pattern_Line(oprands, 0x30);
			byte[] ShiftRight(string[] oprands) => Pattern_Line(oprands, 0x38);
			byte[] TestBit(string[] oprands) => Pattern_BIT(oprands, 0x40);
			byte[] ClearBit(string[] oprands) => Pattern_BIT(oprands, 0x80);
			byte[] SetBit(string[] oprands) => Pattern_BIT(oprands, 0xC0);
			Instructions = new Dictionary<string, Func<string[], byte[]>> {
				{"NOP", NoOp},
				{"STOP", Stop},
				{"HALT", Halt},
				{"DI", DisableInterrupts},
				{"EI", EnableInterrupts},
				{"RET", Return},
				{"RETI", ReturnWithInterrrupts},
				{"DAA", BCDAdjustA},
				{"LD", Load},
				{"XOR", XOR},
				{"ADD", Add},
				{"ADC", AddWithCarry},
				{"SUB", Subtract},
				{"SBC", SubtractWithCarry},
				{"INC", Increment},
				{"DEC", Decrement},
				{"CP", Compare},
				{"AND", And},
				{"OR", Or},
				{"RST", Reset},
				{"SCF", SetCarryFlag},
				{"CPL", ComplementA},
				{"CCF", ClearCarryFlag},
				{"CALL", Call},
				{"PUSH", Push},
				{"POP", Pop},
				{"JP", Jump},
				{"JR", JumpRelative},
				{"LDI", LoadAndIncrement},
				{"LDD", LoadAndDecrement},
				{"LDH", LoadHigher},
				{"LDHL", AddSPIntoHL},
				// CB instructions
				{"RL", RotateLeft},
				{"RR", RotateRight},
				{"BIT", TestBit},
				{"RES", ClearBit},
				{"SET", SetBit},
				{"SWAP", SwapNybbles},
				{"SLA", ShiftLeftPreserveSign},
				{"SRA", ShiftRightPreserveSign},
				{"SRL", ShiftRight},
				{"RRC", RotateRightWithCarry},
				{"RLC", RotateLeftWithCarry}
			};
		}

		private static ArgumentException TooFewOprandsException(int expectednumber) => new ArgumentException($"Expected {expectednumber} oprands");

		public static void Main(string[] args) {
			if (args.Length == 0 || args.Length == 1 && (args[0] == "/?" || args[0] == "-?")) {
				Console.WriteLine("Compiles assembly code that uses the Sharp LR35902 instruction-set into a binary.");
				Console.WriteLine();
				Console.WriteLine("Compiler [options] [-in inputfilepath] [-out outputfilepath]");
				Console.WriteLine("Options:");
				Console.WriteLine("-FHS:	Fix currupted HALTs and STOPs by ensuring a following NOP");
				Console.WriteLine("-P n:	Set unset bytes to constant value n");
				// TODO: Add any switches here
				return;
			}

			byte optimizationlevel = 1;
			string inputpath = null, outputpath = null;
			var fixhaltsandstops = false;
			ushort padding = 0;

			for (var i = 0; i < args.Length; i++)
				switch (args[i].ToLower()) {
					case "-in":
						inputpath = args[++i];
						break;
					case "-out":
						outputpath = args[++i];
						break;
					case "-o":
						optimizationlevel = byte.Parse(args[++i]);
						break;
					case "-fhs":
						fixhaltsandstops = true;
						break;
					case "-p":
						if (!Parser.TryParseImmediate(args[++i], ref padding))
						{
							Console.WriteLine("Error: Padding value could not be parsed");
							return;
						}
						break;
					default:
						Console.WriteLine($"Unknown switch '{args[i]}'");
						return;
				}

			if (inputpath == null) {
				Console.WriteLine("Input path not set");
				return;
			}

			if (outputpath == null) {
				Console.WriteLine("Output path not set");
				return;
			}


			var instructions = new List<string>(File.ReadAllLines(inputpath));

			var assembler = new Assembler();
			Formatter.Format(instructions);
			if (fixhaltsandstops)
				Formatter.EnsureNOPAfterSTOPOrHALT(instructions);
			Optimizer.Optimize(instructions, optimizationlevel);
			byte[] bytecode;
			try {
				bytecode = assembler.CompileProgram(instructions, (byte)padding);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine("Output file has not been written.");
				return;
			}

			using (var outputfile = File.Create(outputpath)) {
				outputfile.Write(bytecode, 0, bytecode.Length);
			}
		}

		public byte[] CompileProgram(IEnumerable<string> instructions, byte padding = 0) {
			byte[] rom;
			try {
				FirstPass = true;
				rom = getROM(instructions, padding);
			} catch {
				/*  Swallow all exceptions as they'll be raised on 2nd pass.
					Doing a 2-pass assembler as we need to find label locations 
					and we cant do that without at least knowing the length of each instruction.
					So I assemble once and track length to populate label locations then
					compile again with the now-known label locations.
					This seems to be the best balance between performance to code-duplication.
				*/
			}

			FirstPass = false;
			CurrentLocation = 0;
			Definitions.Clear();
			try {
				rom = getROM(instructions, padding);
			} catch (AggregateException ex) {
				foreach (var exception in ex.InnerExceptions)
				{
					if (exception is WarningException)
						Console.Write("Warning: ");
					else if (exception is ErrorException)
						Console.Write("Error: ");
					Console.WriteLine(exception.Message);
				}
				throw new Exception("One or more errors occured while compiling source code.", ex);
			}

			return rom;
		}

		private byte[] getROM(IEnumerable<string> instructions, byte padding = 0) {
			var exceptions = new List<Exception>();
			var rom = new ROM(padding);

			foreach (var instruction in instructions)
				try {
					var upperinstruction = instruction.ToUpper();
					if (instruction.EndsWith(':')) {
						var labelname = upperinstruction.Substring(0, upperinstruction.Length - 1);
						AddLabelLocation(labelname, CurrentLocation);
						continue;
					}

					byte[] compiledInstructions = null;
					if (instruction.StartsWith('.') || instruction.StartsWith('#')) // Compiler directives
					{
						compiledInstructions = ParseDirective(instruction, ref CurrentLocation);
						if (compiledInstructions.Length == 0)
							continue;
					}

					var assembledinstructions = compiledInstructions ?? CompileInstruction(upperinstruction);
					for (var i = 0; i < assembledinstructions.Length; i++, CurrentLocation++)
					{
						if (rom[CurrentLocation] != 0)
							exceptions.Add(new OverwriteException($"Overwrote value {rom[CurrentLocation]} at location {CurrentLocation}."));
						rom[CurrentLocation] = assembledinstructions[i];
					}
				} catch (Exception e) {
					exceptions.Add(e);
				}

			if (exceptions.Any())
				throw new AggregateException(exceptions);

			return rom;
		}

		public void AddLabelLocation(string labelname, ushort location) {
			if (!LabelLocations.ContainsKey(labelname))
				LabelLocations.Add(labelname, location);
		}

		public byte[] ParseDirective(string instruction, ref ushort currentlocation) {
			var upperinstruction = instruction.ToUpper();
			var directive = upperinstruction.Substring(1, Math.Max(upperinstruction.IndexOf(' ') - 1, 2));
			var parts = upperinstruction.Split(' ');
			switch (directive) {
				case "ORG":
					if (parts[1] == "ALIGN") {
						ushort alignment = 0;
						if (!TryParseImmediate(parts[2], ref alignment))
							throw new ArgumentException("Expected uint16 location on org instruction.");
						currentlocation += (ushort)(alignment - (currentlocation % alignment));
					} else {
						if (!TryParseImmediate(parts[1], ref currentlocation))
							throw new ArgumentException("Expected uint16 location on org instruction.");
					}
					return new byte[0];
				case "BYTE":
					var stringvalues = instruction.Substring(instruction.IndexOf(' ') + 1).Split(' ');
					var values = new byte[stringvalues.Length];
					for (var i = 0; i < stringvalues.Length; i++) {
						var value = stringvalues[i];
						ushort val = 0;
						if (!TryParseImmediate(value, ref val))
							throw new ArgumentException($"Unable to parse expression '{value}'.");
						if (!val.isByte())
							throw new ArgumentException("All values in BYTE directive must be uint8s.");

						values[i] = (byte)val;
					}

					return values;
				case "TEXT":
					var text = instruction.Substring(instruction.IndexOf(' ') + 1);
					return Encoding.UTF8.GetBytes(text);
				case "DEFINE":
					SetDefintion(parts[1], parts[2]);
					return new byte[0];
				default: throw new NotFoundException($"Compiler directive '{directive}' not found.");
			}
		}

		public byte[] CompileInstruction(string code) {
			var parts = code.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			try {
				return CompileInstruction(parts[0], parts.Skip(1).ToArray());
			} catch (ArgumentException ee) {
				throw new SyntaxException($"Unable to compile malformed instruction {code}", ee);
			}
		}

		public byte[] CompileInstruction(string opcode, string[] oprands) {
			if (!Instructions.TryGetValue(opcode, out var method))
				throw new NotFoundException($"Instruction '{opcode}' not found");

			return method(oprands);
		}

		public void SetDefintion(string key, string value = "0") {
			ushort ushortval = 0;
			if (!TryParseImmediate(value, ref ushortval))
				throw new ArgumentException("Definitions may only be integer");

			SetDefintion(key, ushortval);
		}

		public void SetDefintion(string key, ushort value) {
			if (Definitions.ContainsKey(key))
				Console.WriteLine($"Warning: Redeclaration of global constant '{key}.'");

			Definitions[key] = value;
		}

		public bool TryParseImmediate(string immediate, ref ushort result) {
			if (string.IsNullOrEmpty(immediate))
				return false;

			var res = result; // Working copy. Both to avoid global writes and to ensure result isn't changed unless successful

			var parts = immediate
				.SplitAndKeep(new[] {'+', '-'})
				.Select(p => p.Trim())
				.ToList();
			immediate = parts[0];

			if (parts.Count > 1) {
				res = parseExpression(parts);
			} else if (!Parser.TryParseImmediate(immediate, ref res)) {
				if (Definitions.ContainsKey(immediate))
					res = Definitions[immediate];
				else if (LabelLocations.ContainsKey(immediate))
					res = LabelLocations[immediate];
				else
					return FirstPass;
			}

			result = res;
			return true;
		}

		private ushort parseExpression(List<string> parts) {
			if (parts.Count < 3 || (parts.Count - 3) % 2 == 1)
				throw new ArgumentException("Unbalanced expression");

			while (parts.Count >= 3) {
				var left = parts[0];
				var op = parts[1];
				var right = parts[2];

				ushort leftushort = 0;
				if (!TryParseImmediate(left, ref leftushort))
					throw new FormatException($"Unable to parse expression '{left}' to uint16.");
				ushort rightushort = 0;
				if (!TryParseImmediate(right, ref rightushort))
					throw new FormatException($"Unable to parse expression '{rightushort}' to uint16.");

				switch (op) {
					case "+":
						leftushort += rightushort;
						break;
					case "-":
						leftushort -= rightushort;
						break;
					default: throw new NotImplementedException($"Operator '{parts[1]}' not supported");
				}

				parts.RemoveRange(0, 3);
				parts.Insert(0, leftushort.ToString());
			}

			// The expression has collapsed. 
			// parts[0] has alread been successfully parsed so its safe
			return Parser.ParseImmediate(parts[0]);
		}

		private static string TrimBrackets(string location) => location.Substring(1, location.Length - 2);
		private static bool IsLocation(string expression) => expression.StartsWith('(') && expression.EndsWith(')');
	}
}
