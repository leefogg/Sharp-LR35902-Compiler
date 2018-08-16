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
								currentnode.AddChild(new VariableAssignmentNode(token.Value, new ImmediateValueNode((byte)Common.Parser.ParseImmediate(valuenode.Value))));
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
						currentnode.AddChild(new VariableAssignmentNode(variablenode.Value, new ImmediateValueNode((byte)Common.Parser.ParseImmediate(valuenode.Value))));

					i += 3;
				}
			}

			return rootnode;
		}
    }
}
