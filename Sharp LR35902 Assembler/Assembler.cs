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

		private readonly Dictionary<string, Func<string[], InstructionVarient>> Instructions;
		private readonly Dictionary<string, ushort> Definitions = new Dictionary<string, ushort>();
		private readonly Dictionary<string, ushort> LabelLocations = new Dictionary<string, ushort>();
		private ushort	 CurrentLocation = 0;

		private static ArgumentException TooFewOprandsException(int expectednumber) => new ArgumentException($"Expected {expectednumber} oprands");
		private static ArgumentException NoOprandMatchException => new ArgumentException("No known oprand match found");
		private static ArgumentException UnexpectedInt16Exception => throw new ArgumentException($"Unexpected 16-bit immediate, expected 8-bit immediate.");
		
		// Common patterns for opcode ranges
		private InstructionVarient Pattern_BIT(string[] oprands, Func<Register, byte, InstructionVarient> creator)
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

				return creator((Register)registerindex, (byte)bit);
			}

			throw new ArgumentException("No known oprand match found");
		}
		private static InstructionVarient Pattern_Line(string[] oprands, Func<Register, InstructionVarient> creator)
		{
			if (oprands.Length != 1)
				throw new ArgumentException("Expected 1 register oprand");

			var registerindex = Registers.IndexOf(oprands[0]);
			if (registerindex == -1)
				throw new ArgumentException("Oprand 1 is not a register");

			return creator((Register)registerindex);
		}
		private static InstructionVarient Pattern_LineWithFastA(string[] oprands, Func<Register, InstructionVarient> creator)
		{
			if (oprands.Length != 1)
				throw TooFewOprandsException(1);

			var registerindex = Registers.IndexOf(oprands[0]);
			if (registerindex == -1)
				throw new ArgumentException("Unexpected expression.");

			return creator((Register)registerindex);
		}
		private InstructionVarient Pattern_RegisterOrByte(string[] oprands, Func<byte, InstructionVarient> awithimmediatecreator, Func<Register, InstructionVarient> creator)
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

				return awithimmediatecreator((byte)immediate);
			}

			return creator((Register)registerindex);
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
			InstructionVarient NoOp(string[] oprands) => new NoOp();
			InstructionVarient Stop(string[] oprands) => new Stop();
			InstructionVarient BCDAdjustA(string[] oprands) => new BCDAdjustA();
			InstructionVarient ComplementA(string[] oprands) => new ComplementA();
			InstructionVarient SetCarryFlag(string[] oprands) => new SetCarryFlag();
			InstructionVarient ClearCarryFlag(string[] oprands) => new ClearCarryFlag();
			InstructionVarient Halt(string[] oprands) => new Halt();
			InstructionVarient ReturnWithInterrrupts(string[] oprands) => new ReturnWithInterrupts();
			InstructionVarient DisableInterrupts(string[] oprands) => new DisableInterrupts();
			InstructionVarient EnableInterrupts(string[] oprands) => new EnableInterrupts();
			InstructionVarient Return(string[] oprands)
			{
				if (oprands.Length > 1)
					throw new ArgumentException("Unexpected oprands");

				if (oprands.Length == 0)
					return new Return();

				var conditionindex = Conditions.IndexOf(oprands[0]);
				if (conditionindex == -1)
					throw new ArgumentException($"Unexpected condition '{oprands[0]}'");

				return new ConditionalReturn((Condition)conditionindex);
			}
			InstructionVarient Load(string[] oprands)
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

					return new LoadRegisterIntoMemoryAddressFromRegisterPair(oprand2const, (RegisterPair)pairindex);
				}

				// Loading A into memory location at register pair
				if (oprands[0] == "(BC)")
					return new LoadAIntoMemoryAddressAtBC();
				if (oprands[0] == "(DE)")
					return new LoadAIntoMemoryAddressAtDE();

				// Loading HL into SP
				if (oprands[0] == "SP" && oprands[1] == "HL")
					return new LoadHLIntoSP();

				var oprand1offset = Registers.IndexOf(oprands[0]);
				var oprand2offset = Registers.IndexOf(oprands[1]);
				// TODO: throw if oprand[1] isn't a immediate
				if (oprand1offset == -1)
				{
					ushort location = 0;
					if (!TryParseImmediate(TrimBrackets(oprands[0]), ref location))
						throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint16.");

					if (oprands[1] == "A")
						return new LoadAIntoMemoryAddress(location);
					if (oprands[1] == "SP")
						return new LoadSPIntoMemoryAddress(location);
				}
				if (oprands[0] == "A" && oprand2offset == -1 && IsLocation(oprands[1])) // Loading memory value into A
				{
					// Pointed to by register pair
					if (oprands[1] == "(BC)" || oprands[1] == "(DE)")
						return new LoadMemoryValueFromRegisterPair((RegisterPair)RegisterPairs.IndexOf(TrimBrackets(oprands[1])));

					// Pointed to by immediate
					ushort location = 0;
					if (!TryParseImmediate(TrimBrackets(oprands[1]), ref location))
						throw new ArgumentException($"Unexpected expression '{oprands[1]}', expected uint16.");

					return new LoadMemoryValueFromImmediate(location);
				}
				else if (oprand1offset != -1 && oprand2offset == -1) // Loading immediate into register (0xn6/0xnE)
				{
					// Assume its a immediate
					ushort immediate = 0;
					if (!TryParseImmediate(oprands[1], ref immediate))
						throw new FormatException($"Oprand 2 '{oprands[1]}' is not a valid immediate");
					return new LoadImmediateIntoRegister((byte)immediate, (Register)oprand1offset);
				}
				else // 0x40 - 0x6F
				{
					// Both oprands are registers
					return new LoadRegisterIntoRegister((Register)oprand1offset, (Register)oprand2offset);
				}
			}
			InstructionVarient Add(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "HL") // ADD HL rr
				{
					var pairindex = RegisterPairs.IndexOf(oprands[1]);
					if (pairindex == -1)
						throw new ArgumentException($"Unrecognised register pair '{oprands[1]}'");

					return new AddRegisterPairToHL((RegisterPair)pairindex);
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

					return new AddImmediateToSP((byte)immediate);
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

					return new AddImmediateToA((byte)immediate);
				}

				return new AddRegisterToA((Register)registerindex); // ADD A r
			}
			InstructionVarient AddWithCarry(string[] oprands)
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

					return new AddWithCarryImmediate((byte)immediate);
				}

				return new AddWithCarryRegister((Register)registerindex);
			}
			InstructionVarient Subtract(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] != "A")
					throw new ArgumentException($"Cannot subtract into register '{oprands[0]}'. Can only subtract into register A");

				var registerindex = Registers.IndexOf(oprands[1]);
				if (registerindex > -1)
					return new SubtractRegister((Register)registerindex);

				ushort immediate = 0;
				if (TryParseImmediate(oprands[1], ref immediate))
					if (immediate.isByte())
						return new SubtractImmediate((byte)immediate);
					else
						throw UnexpectedInt16Exception;

				throw new ArgumentException($"Unrecognised register '{oprands[1]}'");
			}
			InstructionVarient SubtractWithCarry(string[] oprands)
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

					return new SubtractWithCarryImmediate((byte)immediate);
				}

				return new SubtractWithCarryRegister((Register)registerindex);
			}
			InstructionVarient XOR(string[] oprands) => Pattern_RegisterOrByte(oprands, i => new XORAWithImmediate(i), r => new XORAWithRegister(r));
			InstructionVarient Increment(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var registerindex = Registers.IndexOf(oprands[0]);
				if (registerindex == -1)
				{
					var pairindex = RegisterPairs.IndexOf(oprands[0]);
					if (pairindex == -1)
						throw new ArgumentException($"Unknown register '{oprands[0]}'");

					return new IncrementRegisterPair((RegisterPair)pairindex);
				}

				return new IncrementRegister((Register)registerindex);
			}
			InstructionVarient Decrement(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var registerindex = Registers.IndexOf(oprands[0]);
				if (registerindex == -1)
				{
					var pairindex = RegisterPairs.IndexOf(oprands[0]);
					if (pairindex == -1)
						throw new ArgumentException($"Unknown register '{oprands[0]}'");

					return new DecrementRegisterPair((RegisterPair)pairindex);
				}

				return new DecrementRegister((Register)registerindex);
			}
			InstructionVarient Compare(string[] oprands) => Pattern_RegisterOrByte(oprands, i => new CompareAWithImmediate(i), r => new CompareAWithRegister(r));
			InstructionVarient And(string[] oprands) => Pattern_RegisterOrByte(oprands, i => new AndAWithImmediate(i), r => new AndAWithRegister(r));
			InstructionVarient Or(string[] oprands) => Pattern_RegisterOrByte(oprands, i => new OrAWithImmediate(i), r => new OrAWithRegister(r));
			InstructionVarient Reset(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var vectors = InstructionVarients.Reset.Vectors;
				var vectorindex = vectors.IndexOf(oprands[0]);
				if (vectorindex == -1)
					throw new ArgumentException($"Unknown reset vector '{oprands[0]}'");

				return new Reset(vectorindex);
			}
			InstructionVarient Call(string[] oprands)
			{
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				if (Enum.IsDefined(typeof(Condition), oprands[0]))
				{
					if (oprands.Length < 2)
						throw new ArgumentException("Expected location after condition");

					ushort immediate = 0;
					if (!TryParseImmediate(oprands[1], ref immediate, true))
						throw new ArgumentException($"Unknown expression '{oprands[1]}'.");

					return new ConditionalCall(Enum.Parse<Condition>(oprands[0], true), immediate);
				}

				ushort address = 0;
				if (!TryParseImmediate(oprands[0], ref address, true))
					throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

				return new Call(address);
			}
			InstructionVarient Push(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var pairs = InstructionVarients.Push.RegisterPairs;
				var pairindex = pairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register pair '{oprands[0]}'");

				return new Push(pairindex);
			}
			InstructionVarient Pop(string[] oprands)
			{
				if (oprands.Length != 1)
					throw TooFewOprandsException(1);

				var pairs = InstructionVarients.Push.RegisterPairs;
				var pairindex = pairs.IndexOf(oprands[0]);
				if (pairindex == -1)
					throw new ArgumentException($"Unknown register pair '{oprands[0]}'");

				return new Pop(pairindex);
			}
			InstructionVarient Jump(string[] oprands)
			{
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				if (oprands[0] == "(HL)")
					return new JumpToLocationAtHL();

				ushort address = 0;
				if (Enum.IsDefined(typeof(Condition), oprands[0]))
				{
					if (oprands.Length < 2)
						throw new ArgumentException("Expected location after condition");

					if (!TryParseImmediate(oprands[1], ref address, true))
						throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

					return new ConditionalJump(Enum.Parse<Condition>(oprands[0], true), address);
				}

				if (!TryParseImmediate(oprands[0], ref address, true))
					throw new ArgumentException($"Unknown expression '{oprands[0]}'.");

				return new Jump(address);
			}
			InstructionVarient JumpRelative(string[] oprands)
			{
				if (oprands.Length == 0)
					throw new ArgumentException("Expected at least 1 oprand");

				if (!Enum.IsDefined(typeof(Condition), oprands[0]))
				{
					ushort address = 0;
					if (!TryParseImmediate(oprands[0], ref address))
						throw new ArgumentException($"Unknown expression '{oprands[0]}'");

					if (!address.isByte())
						throw new ArgumentException("Can only jump back 127 and forward 128"); // TODO: Test this range

					var addressbytes = address.ToByteArray();
					return new ReletiveJump((byte)address);
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
				var condition = Enum.Parse<Condition>(oprands[0]);
				return new ConditionalReletiveJump(condition, (byte)immediate);
			}
			InstructionVarient LoadAndIncrement(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "A" && oprands[1] == "(HL)")
					return new LoadMemoryValueFromHLAndIncrement();
				if (oprands[0] == "(HL)" && oprands[1] == "A")
					return new LoadAIntoMemoryLocationAtHLAndIncrement();

				throw new ArgumentException("No known oprand match found");
			}
			InstructionVarient LoadAndDecrement(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				if (oprands[0] == "A" && oprands[1] == "(HL)")
					return new LoadMemoryValueFromHLAndDecrement();
				if (oprands[0] == "(HL)" && oprands[1] == "A")
					return new LoadAIntoMemoryLocationAtHLAndDecrement();

				throw new ArgumentException("No known oprand match found");
			}
			InstructionVarient LoadHiger(string[] oprands)
			{
				if (oprands.Length != 2)
					throw TooFewOprandsException(2);

				ushort addressoffset = 0;
				if (oprands[1] == "A")
				{
					if (oprands[0] == "(C)")
						return new LoadAIntoMemoryAddressC();

					if (TryParseImmediate(TrimBrackets(oprands[0]), ref addressoffset))
					{
						if (!addressoffset.isByte())
							throw UnexpectedInt16Exception;

						return new LoadAIntoHigherMemoryAddress((byte)addressoffset);
					}
				}
				else if (TryParseImmediate(TrimBrackets(oprands[1]), ref addressoffset))
				{
					if (!addressoffset.isByte())
						throw UnexpectedInt16Exception;

					return new LoadFromHigherMemoryAddressIntoA((byte)addressoffset);
				}

				throw new ArgumentException("No known oprand match found");
			}
			InstructionVarient AddToSPAndSaveToHL(string[] oprands)
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
					//TODO: immediate is signed, check for range and convert

					return new AddImmediateToSPSaveToHL((byte)immediate);
				}

				throw NoOprandMatchException;
			}
			// CB instructions
			InstructionVarient RotateLeftWithCarry(string[] oprands) => Pattern_LineWithFastA(oprands, r => new RotateLeftWithCarry(r));
			InstructionVarient RotateRightWithCarry(string[] oprands) => Pattern_LineWithFastA(oprands, r => new RotateRightWithCarry(r));
			InstructionVarient RotateLeft(string[] oprands) => Pattern_LineWithFastA(oprands, r => new RotateLeft(r));
			InstructionVarient RotateRight(string[] oprands) => Pattern_LineWithFastA(oprands, r => new RotateRight(r));
			InstructionVarient ShiftLeftPreserveSign(string[] oprands) => Pattern_Line(oprands, r => new ShiftLeftPreserveSign(r));
			InstructionVarient ShiftRightPreserveSign(string[] oprands) => Pattern_Line(oprands, r => new ShiftRightPreserveSign(r));
			InstructionVarient SwapNybbles(string[] oprands) => Pattern_Line(oprands, r => new SwapNybbles(r));
			InstructionVarient ShiftRight(string[] oprands) => Pattern_Line(oprands, r => new ShiftRight(r));
			InstructionVarient TestBit(string[] oprands) => Pattern_BIT(oprands, (reg, bit) => new TestBit(reg, bit));
			InstructionVarient ClearBit(string[] oprands) => Pattern_BIT(oprands, (reg, bit) => new ClearBit(reg, bit));
			InstructionVarient SetBit(string[] oprands) => Pattern_BIT(oprands, (reg, bit) => new SetBit(reg, bit));

			Instructions = new Dictionary<string, Func<string[], InstructionVarient>> {
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
				{ "LDHL", AddToSPAndSaveToHL },
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
				if (!Instructions.TryGetValue(opcode, out Func<string[], InstructionVarient> method))
					throw new NotFoundException($"Instruction '{opcode}' not found");

				return method(oprands).Compile();
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
