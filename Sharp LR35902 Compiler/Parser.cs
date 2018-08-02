using Common.Exceptions;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using Common.Extensions;

using static Sharp_LR35902_Compiler.TokenType;

namespace Sharp_LR35902_Compiler
{
    public class Parser
    {
		public static Node CreateAST(List<Token> tokenlist)
		{
			var rootnode = new Node();

			var currentnode = rootnode;

			var currentscope = new Scope();

			for(int i=0; i < tokenlist.Count; i++)
			{
				var token = tokenlist[i];

				if (token.Type == Variable)
				{
					// Variable[++/--] or Variable = [Variable/Immediate]
					var nexttoken = tokenlist[i + 1];
					if (nexttoken.Type != Operator)
						throw new SyntaxException("Expected operator after variable");
					
					switch (nexttoken.Value)
					{
						case "=":
							// TODO: Remove byte cast when added support for ushorts
							var valuenode = tokenlist[i + 2];
							if (valuenode.Type == Variable)
								currentnode.AddChild(new VariableAssignmentNode(token.Value, new VariableValueNode(valuenode.Value))); 
							else if (valuenode.Type == Immediate)
								currentnode.AddChild(new VariableAssignmentNode(token.Value, new ImmediateValueNode((byte)ParseImmediate(valuenode.Value))));
							else
								throw new SyntaxException($"Unexpected token '{valuenode.Value} after ='");

							i++;
							break;
						case "++":
							currentnode.AddChild(new IncrementNode(token.Value));
							break;
						case "--":
							currentnode.AddChild(new DecrementNode(token.Value));
							break;
						// TODO: Add += and -=
						default:
							throw new SyntaxException("Expected operator");
					}

					i++;
				} 
				else if (token.Type == DataType)
				{
					// DataType Variable = [Variable/Immediate]
					var variablenode = tokenlist[i + 1];
					if (variablenode.Type != Variable)
						throw new SyntaxException("Expected variable name after data type");

					var operatornode = tokenlist[i + 2];
					if ((operatornode.Type == Grammar && operatornode.Value == ";") || (operatornode.Type == Operator && operatornode.Value == "="))
					{
						if (currentscope.MemberExists(variablenode.Value))
							throw new SyntaxException($"A variable named '{variablenode.Value}' is already defined in this scope");

						currentnode.AddChild(new VariableDeclarationNode(token.Value, variablenode.Value));
						currentscope.AddMember(new VariableMember(token.Value, variablenode.Value));

						if (operatornode.Type == Grammar) // Mus not be =. In which case we want a value too
						{
							i += 2;
							continue;
						}
					}
					if (operatornode.Value != "=")
						throw new SyntaxException("Expected token '='");

					var valuenode = tokenlist[i + 3];
					if (valuenode.Type != Immediate && valuenode.Type != Variable)
						throw new SyntaxException($"Unexpected symbol on variable assignment '{valuenode.Value}'");

					if (valuenode.Type == Variable)
						currentnode.AddChild(new VariableAssignmentNode(variablenode.Value, new VariableValueNode(valuenode.Value)));
					else if (valuenode.Type == Immediate)
						currentnode.AddChild(new VariableAssignmentNode(variablenode.Value, new ImmediateValueNode((byte)ParseImmediate(valuenode.Value))));

					i += 3;
				}
			}

			return rootnode;
		}

		public static ushort ParseImmediate(string immediate)
		{
			ushort result = 0;
			if (ushort.TryParse(immediate, out result))
				return result;

			if (immediate.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
			{
				if (immediate.Length == 2)
					throw new FormatException("Expected 2 hex characters after 0x");

				var stripped = immediate.Substring(2);
				var bytes = stripped.GetHexBytes();

				result = 0;
				for (byte i = 0; i < Math.Min(2, bytes.Length); i++)
					result |= (ushort)(bytes[i] << i * 8);

				return result;
			}
			else if (immediate.StartsWith("0b", StringComparison.InvariantCultureIgnoreCase))
			{
				immediate = immediate.ToLower();
				var bitsindex = immediate.IndexOf("b") + 1;
				immediate = immediate.Substring(bitsindex);

				if (immediate.Length != 8 && immediate.Length != 16)
					throw new FormatException("Expected 8 or 16 binary digits after 0b");

				ushort bits = 0;
				for (byte i = 0; i < immediate.Length; i++)
				{
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
		public static bool TryParseImmediate(string immediate, ref ushort value)
		{
			try
			{
				value = ParseImmediate(immediate);
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}
    }
}
