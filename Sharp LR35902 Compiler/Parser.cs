using Common.Exceptions;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

using static Sharp_LR35902_Compiler.TokenType;
using static Common.Parser;

namespace Sharp_LR35902_Compiler
{
    public static class Parser
    {

		private static readonly Dictionary<string, Func<ExpressionNode>> Operators = new Dictionary<string, Func<ExpressionNode>>()
		{
			{ "+",  () => new AdditionNode() },
			{ "-",  () => new SubtractionNode() },
			{ "==", () => new EqualsComparisonNode() },
			{ ">",  () => new MoreThanComparisonNode() },
			{ "<",  () => new LessThanComparisonNode() },
			{ "!",	() => new NegateNode() }
		};


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

								var immediatevalue = ParseImmediate(valuenode.Value);
								var immediatedatatype = GetImmedateDataType(immediatevalue);
								checkCanConvertTypes(immediatedatatype, existingvariable.DataType);
								currentnode.AddChild(new VariableAssignmentNode(token.Value, new ShortValueNode(immediatevalue)));
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
								var immediatevalue = ParseImmediate(valuenode.Value);
								currentnode.AddChild(new AdditionAssignmentNode(assignedvariable.Name, new ShortValueNode(immediatevalue)));
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
								var immediatevalue = ParseImmediate(valuenode.Value);
								currentnode.AddChild(new SubtractionAssignmentNode(assignedvariable.Name, new ShortValueNode(immediatevalue)));
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
						var immediatevalue = ParseImmediate(valuenode.Value);
						var immediatedatatype = GetImmedateDataType(immediatevalue);
						checkCanConvertTypes(immediatedatatype, variabledatatype);
						currentnode.AddChild(new VariableAssignmentNode(variabletoken.Value, new ShortValueNode(immediatevalue)));
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

		public static ExpressionNode CreateExpression(IList<Token> tokens, int index = 0)
			=> CreateExpression(tokens, out var tokensused);
		public static ExpressionNode CreateExpression(IList<Token> tokens, out int tokensused, int startindex = 0)
		{
			var index = startindex;
			var nodes = new List<ExpressionNode>();

			Token currenttoken;
			do
			{
				currenttoken = tokens[index++];

				if (currenttoken.Type == Immediate)
					nodes.Add(new ShortValueNode(ParseImmediate(currenttoken.Value)));
				else if (currenttoken.Type == Operator)
					nodes.Add(createOperator(currenttoken.Value));
				else if (currenttoken.Value == "(")
				{
					nodes.Add(CreateExpression(tokens, out tokensused, index));
					index += tokensused;
				}

			} while (index < tokens.Count && currenttoken.Value != ")" && currenttoken.Value != ";");

			tokensused = index - startindex;

			return ConvergeOperators(nodes);
		}

		private static ExpressionNode createOperator(string op)
		{
			return Operators[op]();
		}

		private static ExpressionNode ConvergeOperators(List<ExpressionNode> nodes)
		{
			if (nodes.Count == 0)
				return null;
			if (nodes.Count == 1)
				return nodes[0];

			// Boolean operators
			// Should be converged in order of appearance
			ConvergeOperators<ComparisonNode>(nodes);
			ConvergeNegateOperator(nodes);
			// Math operators
			ConvergeOperators<AdditionNode>(nodes);
			ConvergeOperators<SubtractionNode>(nodes);

			// TODO: Throw if there is more than one as expresion isn't complete
			return nodes[0];
		}

		private static void ConvergeOperators<T>(List<ExpressionNode> nodes) where T : OperatorNode
		{
			for(var i=1; i<nodes.Count - 1; i++)
			{
				if (!(nodes[i] is T))
					continue;

				// Validate the left and right sides
				if (i - 1 < 0)
					throw new SyntaxException("No expression found on the left of the operator.");
				if (i + 1 >= nodes.Count)
					throw new SyntaxException("No expression found on the right of the operator.");
				if (nodes[i-1] is ExpressionNode left)
				{
					if (nodes[i+1] is ExpressionNode right)
					{
						var op = nodes[i] as OperatorNode;
						op.Left = left;
						op.Right = right;

						nodes.RemoveAt(i - 1);
						i--;
						nodes.RemoveAt(i + 1);
					} 
					else
					{
						throw new SyntaxException("The right side of the operator is not an expresison.");
					}
				} 
				else
				{
					throw new SyntaxException("The left side of the operator is not an expresison.");
				}
			}
		}

		// Annoyingly the negate operator is the only one that requires a right side expression only
		private static void ConvergeNegateOperator(List<ExpressionNode> nodes)
		{
			for (var i=0; i<nodes.Count-1; i++)
			{
				if (!(nodes[i] is NegateNode))
					continue;

				// No need to check bounds as its enforced in the loop condition

				if (nodes[i+1] is ExpressionNode right)
				{
					var negate = nodes[i] as NegateNode;
					negate.Expression = right;

					nodes.RemoveAt(i+1);
				}
				else
				{
					throw new SyntaxException("The right side of the operator is not an expresison.");
				}
			}
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
