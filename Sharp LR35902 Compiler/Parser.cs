using Common.Exceptions;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using Common.Extensions;

using static Sharp_LR35902_Compiler.TokenType;
using System.Linq;

namespace Sharp_LR35902_Compiler
{
    public class Parser
    {
		public static Node CreateAST(IList<Token> tokenlist)
		{
			var rootnode = new ASTNode();

			var currentnode = rootnode;

			var currentscope = new Scope();

			for(int i=0; i < tokenlist.Count; i++)
			{
				var token = tokenlist[i];

				if (token.Type == Variable)
				{
					// Variable[++/--] or Variable = [Variable/Immediate]
					var nexttoken = tokenlist[++i];
					if (nexttoken.Type != Operator)
						throw new SyntaxException("Expected operator after variable");

					Token valuenode;
					VariableMember assignedvariable;
					switch (nexttoken.Value)
					{
						case "=":
							// TODO: Remove byte cast when added support for ushorts
							assignedvariable = getVariable(token.Value, currentscope);

							valuenode = tokenlist[++i];
							if (valuenode.Type == Variable)
							{
								var valuevariable = getVariable(valuenode.Value, currentscope);
								checkCanConvertTypes(valuevariable.DataType, assignedvariable.DataType);
								currentnode.AddChild(new VariableAssignmentNode(token.Value, new VariableValueNode(valuenode.Value)));
							}
							else if (valuenode.Type == Immediate)
							{
								var existingvariable = currentscope.GetMember(token.Value);

								var immediatevalue = Common.Parser.ParseImmediate(valuenode.Value);
								var immediatedatatype = GetImmedateDataType(immediatevalue);
								checkCanConvertTypes(immediatedatatype, existingvariable.DataType);
								currentnode.AddChild(new VariableAssignmentNode(token.Value, new ImmediateValueNode(immediatevalue)));
							}
							else
								throw new SyntaxException($"Unexpected token '{valuenode.Value} after ='");
							break;
						case "++":
							currentnode.AddChild(new IncrementNode(token.Value));
							break;
						case "--":
							currentnode.AddChild(new DecrementNode(token.Value));
							break;
						case "+=":
							assignedvariable = getVariable(token.Value, currentscope);

							valuenode = tokenlist[++i];
							if (valuenode.Type == Variable)
							{
								var valuevariable = getVariable(valuenode.Value, currentscope);
								currentnode.AddChild(new AdditionAssignmentNode(assignedvariable.Name, new VariableValueNode(valuevariable.Name)));
							}
							else if (valuenode.Type == Immediate)
							{
								var immediatevalue = Common.Parser.ParseImmediate(valuenode.Value);
								currentnode.AddChild(new AdditionAssignmentNode(assignedvariable.Name, new ImmediateValueNode(immediatevalue)));
							}
							break;
						case "-=":
							assignedvariable = getVariable(token.Value, currentscope);

							valuenode = tokenlist[++i];
							if (valuenode.Type == Variable)
							{
								var valuevariable = getVariable(valuenode.Value, currentscope);
								currentnode.AddChild(new SubtractionAssignmentNode(assignedvariable.Name, new VariableValueNode(valuevariable.Name)));
							}
							else if (valuenode.Type == Immediate)
							{
								var immediatevalue = Common.Parser.ParseImmediate(valuenode.Value);
								currentnode.AddChild(new SubtractionAssignmentNode(assignedvariable.Name, new ImmediateValueNode(immediatevalue)));
							}
							break;
						default:
							throw new SyntaxException("Expected operator");
					}
				} 
				else if (token.Type == DataType)
				{
					// DataType Variable = [Variable/Immediate]
					var variabletoken = tokenlist[++i];
					if (variabletoken.Type != Variable)
						throw new SyntaxException("Expected variable name after data type");

					var variabledatatype = BuiltIn.DataTypes.Get(token.Value);
					if (variabledatatype == null)
						throw new SyntaxException($"Datatype {token.Value} does not exist.");

					var operatornode = tokenlist[++i];
					if (operatornode.Value == ";" || operatornode.Value == "=")
					{
						if (currentscope.GetMember(variabletoken.Value) != null)
							throw new SyntaxException($"Variable {variabletoken.Value} already exists in the current scope.");

						currentnode.AddChild(new VariableDeclarationNode(token.Value, variabletoken.Value));
						currentscope.AddMember(new VariableMember(variabledatatype, variabletoken.Value));

						if (operatornode.Value == ";") // Just a decleration
							continue;
					}
					if (operatornode.Value != "=")
						throw new SyntaxException("Expected token '='");

					var valuenode = tokenlist[++i];
					if (valuenode.Type != Immediate && valuenode.Type != Variable)
						throw new SyntaxException($"Unexpected symbol on variable assignment '{valuenode.Value}'");

					if (valuenode.Type == Variable)
					{
						var valuevariable = getVariable(valuenode.Value, currentscope);
						checkCanConvertTypes(valuevariable.DataType, variabledatatype);
						currentnode.AddChild(new VariableAssignmentNode(variabletoken.Value, new VariableValueNode(valuenode.Value)));
					}
					else if (valuenode.Type == Immediate)
					{
						var immediatevalue = Common.Parser.ParseImmediate(valuenode.Value);
						var immediatedatatype = GetImmedateDataType(immediatevalue);
						checkCanConvertTypes(immediatedatatype, variabledatatype);
						currentnode.AddChild(new VariableAssignmentNode(variabletoken.Value, new ImmediateValueNode(immediatevalue)));
					}
				}
                else if (token.Type == ControlFlow)
                {
                    switch(token.Value)
                    {
                        case "while": break;
                        case "do": break;
                        case "for": break;
                        case "return": break;
                        case "continue": break;
                        case "else": break;
                        case "if": break;
						case "goto":
							var nextnode = tokenlist[++i];
							if (nextnode.Type != Variable)
								throw new SyntaxException("Expected label name after goto statement.");
							currentnode.AddChild(new GotoNode(nextnode.Value));
							break;
                        default: // Label
							var colonindex = token.Value.IndexOf(':');
                            currentnode.AddChild(new LabelNode(token.Value.Substring(0, colonindex)));
                        break;
                    }
                }
			}

			return rootnode;
		}

		public static PrimitiveDataType GetImmedateDataType(ushort value)
		{
			PrimitiveDataType smallestDataType = null;
			foreach (var datatype in BuiltIn.DataTypes.All.Reverse())
				if (value < datatype.MaxValue)
					smallestDataType = datatype;

			// Exeption cant possibly throw as ParseImmediate parses up to int data type
			if (smallestDataType == null)
				throw new SyntaxException($"No datatype suitable for value ({value})");

			return smallestDataType;
		}

		private static void checkCanConvertTypes(PrimitiveDataType from, PrimitiveDataType to)
		{
			if (!BuiltIn.DataTypes.CanConvertTo(from, to))
				throw new SyntaxException($"No conversion from {from.Name} to {to.Name}.");
		}
		private static VariableMember getVariable(string name, Scope scope)
		{
			var existingvariable = scope.GetMember(name);
			if (existingvariable == null)
				throw new SyntaxException($"Varible {name} does not exist in the current scope.");

			return existingvariable;
		}
    }
}
