using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Exceptions;
using Common.Extensions;
using Sharp_LR35902_Assembler.InstructionVarients;
using static Common.Extensions.IEnumerableExtensions;

namespace Sharp_LR35902_Assembler
{
	public class Assembler
	{
		private static readonly string[] Registers = new[] { "B", "C", "D", "E", "H", "L", "(HL)", "A" };
		private static readonly string[] RegisterPairs = new[] { "BC", "DE", "HL", "SP" };
		private static readonly string[] Conditions = new[] { "NZ", "Z", "NC", "C" };

		private readonly Dictionary<string, Func<string[], byte[]>> Instructions;
		private readonly Dictionary<string, ushort> Definitions = new Dictionary<string, ushort>();
		private readonly Dictionary<string, ushort> LabelLocations = new Dictionary<string, ushort>();
		private ushort	 CurrentLocation = 0;

		private static ArgumentException TooFewOprandsException(int expectednumber) => new ArgumentException($"Expected {expectednumber} oprands");
		private static ArgumentException NoOprandMatchException => new ArgumentException("No known oprand match found");
		private static ArgumentException UnexpectedInt16Exception => throw new ArgumentException($"Unexpected 16-bit immediate, expected 8-bit immediate.");
		
		// Common patterns for opcode ranges
		private byte[] Pattern_BIT(string[] oprands, byte startopcode)
		{
			if (oprands.Length != 2)
				throw TooFewOprandsException(2);

			var registerindex = Registers.IndexOf(oprands[1]);
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

			var registerindex = Registers.IndexOf(oprands[0]);
			if (registerindex == -1)
				throw new ArgumentException("Oprand 1 is not a register");

			return ListOf<byte>(0xCB, (byte)(startopcode + registerindex));
		}
		private static byte[] Pattern_LineWithFastA(string[] oprands, Func<Register, InstructionVarient> creator)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

			var registerindex = Registers.IndexOf(oprands[0]);
			if (registerindex == -1)
				throw new ArgumentException("Unexpected expression.");

			return creator((Register)registerindex).Compile();
		}
		private byte[] Pattern_RegisterOrByte(string[] oprands, Func<byte, InstructionVarient> awithimmediatecreator, Func<Register, InstructionVarient> awithregistercreator)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

			var registerindex = Registers.IndexOf(oprands[0]);
			if (registerindex == -1)
			{
				ushort immediate = 0;
				if (!TryParseImmediate(oprands[0], ref immediate))
					throw new ArgumentException($"Unknown register '{oprands[0]}'");
				if (!immediate.isByte())
					throw UnexpectedInt16Exception;

				return awithimmediatecreator((byte)immediate).Compile();
			}

			return awithregistercreator((Register)registerindex).Compile();
		}

		public static void Main(string[] args)
		{
			if (args.Length == 0 || (args.Length == 1 && (args[0] == "/?" || args[0] == "-?")))
			{
				Console.WriteLine("Compiles assembly code that uses the Sharp LR35902 instruction-set into a binary.");
				Console.WriteLine();
				Console.WriteLine("Compiler [options] [-i inputfilepath] [-o outputfilepath]");
				Console.WriteLine("Options:");
				Console.WriteLine("-FHS:	Fix currupted HALTs and STOPs by ensuring a following NOP");
				// TODO: Add any switches here
				return;
			}

			byte optimizationlevel = 1;
			string inputpath = null, outputpath = null;
			var fixhaltsandstops = false;

			for (var i = 0; i < args.Length; i++)
			{
				switch (args[i].ToLower())
				{
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

			var assembler = new Assembler();
			Formatter.Format(instructions);
			if (fixhaltsandstops)
				Formatter.EnsureNOPAfterSTOPOrHALT(instructions);
			Optimizer.Optimize(instructions, optimizationlevel);
			byte[] bytecode;
			try
			{
				bytecode = assembler.CompileProgram(instructions);

				using (var outputfile = File.Create(outputpath))
					outputfile.Write(bytecode, 0, bytecode.Length);
			}
			catch (SyntaxException e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("Output file has not been written.");
				return;
			}

		}

		public Assembler()
		{
			byte[] NoOp(string[] oprands) => new NoOp().Compile();
			byte[] Stop(string[] oprands) => new Stop().Compile();
			byte[] BCDAdjustA(string[] oprands) => new BCDAdjustA().Compile();
			byte[] ComplementA(string[] oprands) => new ComplementA().Compile();
			byte[] SetCarryFlag(string[] oprands) => new SetCarryFlag().Compile();
			byte[] ClearCarryFlag(string[] oprands) => new ClearCarryFlag().Compile();
			byte[] Halt(string[] oprands) => new Halt().Compile();
			byte[] ReturnWithInterrrupts(string[] oprands) => new ReturnWithInterrupts().Compile();
			byte[] DisableInterrupts(string[] oprands) => new DisableInterrupts().Compile();
			byte[] EnableInterrupts(string[] oprands) => new EnableInterrupts().Compile();
			byte[] Return(string[] oprands)
			{
				if (oprands.Length > 1)
					throw new ArgumentException("Unexpected oprands");

				if (oprands.Length == 0)
					return new Return().Compile();

				var conditionindex = Conditions.IndexOf(oprands[0]);
				if (conditionindex == -1)
					throw new ArgumentException($"Unexpected condition '{oprands[0]}'");

				return new ConditionalReturn((Condition)conditionindex).Compile();
			}
			byte[] Load(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				// Assigning ushort to register pair
				// 0xn1
				ushort oprand2const = 0;
				if (TryParseImmediate(oprands[1], ref oprand2const) && !oprand2const.isByte())
				{
					var pairindex = RegisterPairs.IndexOf(oprands[0]);
					if (pairindex == -1)
						throw new ArgumentException($"Register pair '{oprands[1]}' doesn't exist");

					return new LoadRegisterIntoMemoryAddressFromRegisterPair(oprand2const, (RegisterPair)pairindex).Compile();
				}

				// Loading A into memory location at register pair
				if (oprands[0] == "(BC)")
					return new LoadAIntoMemoryAddressAtBC().Compile();
				if (oprands[0] == "(DE)")
					return new LoadAIntoMemoryAddressAtDE().Compile();

				// Loading HL into SP
				if (oprands[0] == "SP" && oprands[1] == "HL")
					return new LoadHLIntoSP().Compile();

				var oprand1offset = Registers.IndexOf(oprands[0]);
				var oprand2offset = Registers.IndexOf(oprands[1]);
				// TODO: throw if oprand[1] isn't a immediate
				if (oprand1offset == -1)
				{
					ushort location = 0;
					if (!TryParseImmediate(TrimBrackets(oprands[0]), ref location))
						throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint16.");

					if (oprands[1] == "A")
						return new LoadAIntoMemoryAddress(location).Compile();
					if (oprands[1] == "SP")
						return new LoadSPIntoMemoryAddress(location).Compile();
				}
				if (oprands[0] == "A" && oprand2offset == -1 && IsLocation(oprands[1])) // Loading memory value into A
				{
					// Pointed to by register pair
					if (oprands[1] == "(BC)" || oprands[1] == "(DE)")
						return new LoadMemoryValueFromRegisterPairIntoA((RegisterPair)RegisterPairs.IndexOf(TrimBrackets(oprands[1]))).Compile();

					// Pointed to by immediate
					ushort location = 0;
					if (!TryParseImmediate(TrimBrackets(oprands[1]), ref location))
						throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint16.");

					return new LoadMemoryValueFromImmediateIntoA(location).Compile();
				}
				else if (oprand1offset != -1 && oprand2offset == -1) // Loading immediate into register (0xn6/0xnE)
				{
					// Assume its a immediate
					ushort immediate = 0;
					if (!TryParseImmediate(oprands[1], ref immediate))
						throw new FormatException($"Oprand 2 '{oprands[1]}' is not a valid immediate");
					return new LoadImmediateIntoRegister((byte)immediate, (Register)oprand1offset).Compile();
				}
				else // 0x40 - 0x6F
				{
					// Both oprands are registers
					return new LoadRegisterIntoRegister((Register)oprand1offset, (Register)oprand2offset).Compile();
				}
			}
			byte[] Add(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "HL") // ADD HL rr
				{
					var pairindex = RegisterPairs.IndexOf(oprands[1]);
					if (pairindex == -1)
						throw new ArgumentException($"Unrecognised register pair '{oprands[1]}'");

					return new AddRegisterPairToHL((RegisterPair)pairindex).Compile();
				}

				if (oprands[0] == "SP") // ADD SP n
				{
					ushort immediate = 0;
					if (TryParseImmediate(oprands[1], ref immediate))
					{
						if (!immediate.isByte())
							throw UnexpectedInt16Exception;
					}
					else
						throw new ArgumentException($"Unexpected expression '{oprands[1]}'");

					return new AddImmediateToSP((byte)immediate).Compile();
				}

				if (oprands[0] != "A") 
					throw new ArgumentException($"Cannot add into register '{oprands[0]}'. Can only add into register A and HL");

				var registerindex = Registers.IndexOf(oprands[1]);
				if (registerindex == -1) // ADD a n
				{
					ushort immediate = 0;
					if (TryParseImmediate(oprands[1], ref immediate))
					{
						if (!immediate.isByte())
							throw UnexpectedInt16Exception;
					}
					else
						throw new ArgumentException($"Unknown register '{oprands[1]}'");

					return new AddImmediateToA((byte)immediate).Compile();
				}

				return new AddRegisterToA((Register)registerindex).Compile(); // ADD A r
			}
			byte[] AddWithCarry(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] != "A")
					throw new ArgumentException($"Cannot add into register '{oprands[0]}'. Can only add into register A");

				var registerindex = Registers.IndexOf(oprands[1]);
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
			byte[] Subtract(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] != "A")
					throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

				var registerindex = Registers.IndexOf(oprands[1]);
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
			byte[] SubtractWithCarry(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] != "A")
					throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

				var registerindex = Registers.IndexOf(oprands[1]);
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
			byte[] XOR(string[] oprands) => Pattern_RegisterOrByte(oprands, i => new XORAWithImmediate(i), r => new XORAWithRegister(r));
			byte[] Increment(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var registerindex = Registers.IndexOf(oprands[0]);
				if (registerindex == -1)
				{
					var pairindex = RegisterPairs.IndexOf(oprands[0]);
					if (pairindex == -1)
						throw new ArgumentException($"Unknown register '{oprands[0]}'");

					return ListOf((byte)(0x03 + 0x10 * pairindex));
				}

				return ListOf((byte)(0x04 + 8 * registerindex));
			}
			byte[] Decrement(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var registerindex = Registers.IndexOf(oprands[0]);
				if (registerindex == -1)
				{
					var pairindex = RegisterPairs.IndexOf(oprands[0]);
					if (pairindex == -1)
						throw new ArgumentException($"Unknown register '{oprands[0]}'");

					return ListOf((byte)(0x0B + 0x10 * pairindex));
				}

				return ListOf((byte)(0x05 + 8 * registerindex));
			}
			byte[] Compare(string[] oprands) => Pattern_RegisterOrByte(oprands, i => new CompareAWithImmediate(i), r => new CompareAWithRegister(r));
			byte[] And(string[] oprands) => Pattern_RegisterOrByte(oprands, i => new AndAWithImmediate(i), r => new AndAWithRegister(r));
			byte[] Or(string[] oprands) => Pattern_RegisterOrByte(oprands, i => new OrAWithImmediate(i), r => new OrAWithRegister(r));
			byte[] Reset(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var vectors = InstructionVarients.Reset.Vectors;
				var vectorindex = vectors.IndexOf(oprands[0]);
				if (vectorindex == -1)
					throw new ArgumentException($"Unknown reset vector '{oprands[0]}'");

				return new Reset(vectorindex).Compile();
			}
			byte[] Call(string[] oprands)
			{
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				var conditionindex = Conditions.IndexOf(oprands[0]);
				if (conditionindex == -1)
				{
					ushort address = 0;
					if (!TryParseImmediate(oprands[0], ref address, true))
						throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

					var addressbytes = address.ToByteArray();
					return ListOf<byte>(0xCD, addressbytes[0], addressbytes[1]);
				}

				ushort immediate = 0;
				if (!TryParseImmediate(oprands[1], ref immediate, true))
					throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

				var immediatebytes = immediate.ToByteArray();
				return ListOf((byte)(0xC4 + 8 * conditionindex), immediatebytes[0], immediatebytes[1]);
			}
			byte[] Push(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				string[] pairs = new[] { "BC", "DE", "HL", "AF" }; // Different to static list
				var pairindex = pairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register pair '{oprands[0]}'");

				return ListOf((byte)(0xC5 + 0x10 * pairindex));
			}
			byte[] Pop(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				string[] pairs = new[] { "BC", "DE", "HL", "AF" }; // Different to static list
				var pairindex = pairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register pair '{oprands[0]}'");

				return ListOf((byte)(0xC1 + 0x10 * pairindex));
			}
			byte[] Jump(string[] oprands)
			{
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				if (oprands[0] == "(HL)")
					return ListOf<byte>(0xE9);

				var conditionindex = Conditions.IndexOf(oprands[0]);
				ushort address = 0;
				if (conditionindex == -1)
				{
					if (!TryParseImmediate(oprands[0], ref address, true))
						throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

					var addressbytes = address.ToByteArray();
					return ListOf<byte>(0xC3, addressbytes[0], addressbytes[1]);
				}

				if (!TryParseImmediate(oprands[1], ref address, true))
					throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

				var immediatebytes = address.ToByteArray();
				return ListOf((byte)(0xC2 + 8 * conditionindex), immediatebytes[0], immediatebytes[1]);
			}
			byte[] JumpRelative(string[] oprands)
			{
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				var conditionindex = Conditions.IndexOf(oprands[0]);
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
				}
				else
					throw new ArgumentException($"Unknown condition '{oprands[1]}'");

				var immediatebytes = immediate.ToByteArray();
				return ListOf((byte)(0x20 + 8 * conditionindex), immediatebytes[0]);
			}
			byte[] LoadAndIncrement(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "A" && oprands[1] == "(HL)")
					return ListOf<byte>(0x2A);
				if (oprands[0] == "(HL)" && oprands[1] == "A")
					return ListOf<byte>(0x22);

				throw new ArgumentException("No known oprand match found");
			}
			byte[] LoadAndDecrement(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "A" && oprands[1] == "(HL)")
					return ListOf<byte>(0x3A);
				if (oprands[0] == "(HL)" && oprands[1] == "A")
					return ListOf<byte>(0x32);

				throw new ArgumentException("No known oprand match found");
			}
			byte[] LoadHiger(string[] oprands)
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
			byte[] AddSPIntoHL(string[] oprands)
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
			byte[] RotateLeftWithCarry(string[] oprands) => Pattern_LineWithFastA(oprands, r => new RotateLeftWithCarry(r));
			byte[] RotateRightWithCarry(string[] oprands) => Pattern_LineWithFastA(oprands, r => new RotateRightWithCarry(r));
			byte[] RotateLeft(string[] oprands) => Pattern_LineWithFastA(oprands, r => new RotateLeft(r));
			byte[] RotateRight(string[] oprands) => Pattern_LineWithFastA(oprands, r => new RotateRight(r));
			byte[] ShiftLeftPreserveSign(string[] oprands) => Pattern_Line(oprands, 0x20);
			byte[] ShiftRightPreserveSign(string[] oprands) => Pattern_Line(oprands, 0x28);
			byte[] SwapNybbles(string[] oprands) => Pattern_Line(oprands, 0x30);
			byte[] ShiftRight(string[] oprands) => Pattern_Line(oprands, 0x38);
			byte[] TestBit(string[] oprands) => Pattern_BIT(oprands, 0x40);
			byte[] ClearBit(string[] oprands) => Pattern_BIT(oprands, 0x80);
			byte[] SetBit(string[] oprands) => Pattern_BIT(oprands, 0xC0);

			Instructions = new Dictionary<string, Func<string[], byte[]>> {
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
		}

		public byte[] CompileProgram(List<string> instructions)
		{
			byte[] rom;
			try
			{
				rom = getROM(instructions);
			}
			catch {
				/*  Swallow all exceptions as they'll be raised on 2nd pass.
					Doing a 2-pass assembler as we need to find label locations 
					and we cant do that without at least knowing the length of each instruction.
					So I assemble once and track length to populate label locations then
					compile again with the now-known label locations.
					This seems to be the best balance between performance to code-duplication.
				*/
			}
			CurrentLocation = 0;
			Definitions.Clear();
			try
			{
				rom = getROM(instructions);
			}
			catch (AggregateException ex)
			{
				foreach (var exception in ex.InnerExceptions)
					Console.WriteLine(exception.Message);
				throw new SyntaxException("One or more errors occured while compiling source code.");
			}

			return rom;
		}

		private byte[] getROM(List<string> instructions)
		{
			var exceptions = new List<Exception>();
			var rom = new ROM();

			foreach (var instruction in instructions) { 
				try
				{
					var upperinstruction = instruction.ToUpper();
					if (instruction.EndsWith(':'))
					{
						var labelname = upperinstruction.Substring(0, upperinstruction.Length-1);
						AddLabelLocation(labelname, CurrentLocation);
						continue;
					}
					else if (instruction.StartsWith('.') || instruction.StartsWith('#')) // Compiler directives
					{
						ParseDirective(instruction, rom, ref CurrentLocation);
						continue;
					}

					var assembledinstructions = CompileInstruction(upperinstruction);
					for (int i = 0; i < assembledinstructions.Length; i++, CurrentLocation++)
						rom[CurrentLocation] = assembledinstructions[i];
				} 
				catch (Exception e)
				{
					exceptions.Add(e);
				}
			}

			if (exceptions.Any())
				throw new AggregateException(exceptions);

			return rom;
		}

		public void AddLabelLocation(string labelname, ushort location)
		{
			if (!LabelLocations.ContainsKey(labelname))
				LabelLocations.Add(labelname, location);
		}

		public void ParseDirective(string instruction, ROM rom, ref ushort currentlocation)
		{
			var upperinstruction = instruction.ToUpper();
			var directive = upperinstruction.Substring(1, Math.Max(upperinstruction.IndexOf(' ') - 1, 2));
			switch (directive)
			{
				case "ORG":
					var immediate = upperinstruction.Substring(upperinstruction.IndexOf(' ') + 1);
					if (!TryParseImmediate(immediate, ref currentlocation))
						throw new ArgumentException("Expected uint16 location on org instruction.");
					break;
				case "BYTE":
					var stringvalues = instruction.Substring(instruction.IndexOf(' ') + 1).Split(' ');
					var values = new byte[stringvalues.Length];
					for (var i = 0; i < stringvalues.Length; i++)
					{
						var value = stringvalues[i];
						ushort val = 0;
						if (!TryParseImmediate(value, ref val))
							throw new ArgumentException($"Unable to parse expression '{value}'.");
						if (!val.isByte())
							throw new ArgumentException("All values in BYTE directive must be uint8s.");

						values[i] = (byte)val;
					}

					for (int i = 0; i < values.Length; i++, currentlocation++)
						rom[currentlocation] = values[i];

					break;
				case "TEXT":
					var text = instruction.Substring(instruction.IndexOf(' ') + 1);
					for (int i = 0; i < text.Length; i++, currentlocation++)
						rom[currentlocation] = (byte)text[i];

					break;
				case "DEFINE":
					var parts = upperinstruction.Split(' ');
					SetDefintion(parts[1], parts[2]);

					break;
				default:
					throw new NotFoundException($"Compiler directive '{directive}' not found.");
			}
		}

		public byte[] CompileInstruction(string code) {
			var parts = code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			return CompileInstruction(parts[0], parts.Skip(1).ToArray());
		}
		public byte[] CompileInstruction(string opcode, string[] oprands)
		{
			try
			{
				if (!Instructions.TryGetValue(opcode, out Func<string[], byte[]> method))
					throw new NotFoundException($"Instruction '{opcode}' not found");

				return method(oprands);
			}
			catch (ArgumentException ee)
			{
				throw new SyntaxException("Unable to compile malformed instruction", ee);
			}
		}

		public void SetDefintion(string key, string value = "0")
		{
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

		public bool TryParseImmediate(string immediate, ref ushort result, bool trylabels = false)
		{
			if (string.IsNullOrEmpty(immediate))
				return false;

			var res = result; // Working copy. Both to avoid global writes and to ensure result isn't changed unless successful

			var parts = immediate
				.SplitAndKeep(new[] { '+', '-' })
				.Select(p => p.Trim())
				.ToList();
			immediate = parts[0];

			if (parts.Count > 1)
				res = parseExpression(parts, trylabels);
			else if (!Common.Parser.TryParseImmediate(immediate, ref res))
			{
				if (Definitions.ContainsKey(immediate))
					res = Definitions[immediate];
				else if (trylabels && LabelLocations.ContainsKey(immediate))
					res = LabelLocations[immediate];
				else
					return false;
			}

			result = res;
			return true;
		}

		private ushort parseExpression(List<string> parts, bool trylabels)
		{
			if (parts.Count < 3 || (parts.Count - 3) % 2 == 1)
				throw new ArgumentException("Unbalanced expression");

			while (parts.Count >= 3)
			{
				var left = parts[0];
				var op = parts[1];
				var right = parts[2];

				ushort leftushort = 0;
				if (!TryParseImmediate(left, ref leftushort, trylabels))
					throw new FormatException($"Unable to parse expression '{left}' to uint16.");
				ushort rightushort = 0;
				if (!TryParseImmediate(right, ref rightushort, trylabels))
					throw new FormatException($"Unable to parse expression '{rightushort}' to uint16.");
				
				switch (op)
				{
					case "+":
						leftushort += rightushort;
						break;
					case "-":
						leftushort -= rightushort;
						break;
					default:
						throw new NotImplementedException($"Operator '{parts[1]}' not supported");
				}

				parts.RemoveRange(0, 3);
				parts.Insert(0, leftushort.ToString());
			}

			// The expression has collapsed. 
			// parts[0] has alread been successfully parsed so its safe
			return Common.Parser.ParseImmediate(parts[0]);
		}

		private static string TrimBrackets(string location) => location.Substring(1, location.Length - 2);
		private static bool IsLocation(string expression) => expression.StartsWith('(') && expression.EndsWith(')');
	}
}
