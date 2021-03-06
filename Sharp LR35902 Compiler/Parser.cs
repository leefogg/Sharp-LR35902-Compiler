﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Exceptions;
using Common.Extensions;
using Sharp_LR35902_Compiler.Nodes;
using Sharp_LR35902_Compiler.Nodes.Assignment;
using static Sharp_LR35902_Compiler.TokenType;
using static Common.Parser;

namespace Sharp_LR35902_Compiler {
	public static class Parser {
		private static readonly Dictionary<string, Func<ExpressionNode>> Operators = new Dictionary<string, Func<ExpressionNode>> {
			{BuiltIn.Operators.Add, () => new AdditionNode()},
			{BuiltIn.Operators.Subtract, () => new SubtractionNode()},
			{BuiltIn.Operators.Equal, () => new EqualsComparisonNode()},
			{BuiltIn.Operators.MoreThan, () => new MoreThanComparisonNode()},
			{BuiltIn.Operators.LessThan, () => new LessThanComparisonNode()},
			{BuiltIn.Operators.And, () => new AndComparisonNode()},
			{BuiltIn.Operators.Or, () => new OrComparisonNode()},
			{BuiltIn.Operators.Not, () => new NegateNode()}
		};


		public static BlockNode CreateAST(IList<Token> tokens) {
			var i = 0;
			return CreateAST(tokens, new ASTNode(), ref i, null);
		}
		public static BlockNode CreateAST(IList<Token> tokens, BlockNode rootnode, ref int i, Scope previousScope) {
			var currentnode = rootnode;

			var currentscope = new Scope(previousScope);

			for (; i < tokens.Count; i++) {
				var token = tokens[i];

				if (token.Type == Variable) {
					// Variable[++/--] or Variable = [Variable/Immediate]
					var nexttoken = tokens[++i];
					if (nexttoken.Type != Operator)
						throw new SyntaxException("Expected operator after variable");

					Token valuenode;
					VariableMember assignedvariable;
					switch (nexttoken.Value) {
						case "=":
							i++;
							var expression = CreateExpression(tokens, currentscope, ref i);
							if (expression == null)
								throw new SyntaxException($"Unknown expression after =");

							getVariable(token.Value, currentscope); // Check it exists
							//var existingvariable = currentscope.GetMember(token.Value);
							//var immediatedatatype = GetImmedateDataType(expression);
							//checkCanConvertTypes(immediatedatatype, existingvariable.DataType);
							currentnode.AddChild(new VariableAssignmentNode(token.Value, expression));
							i--;

							break;
						case "++":
							currentnode.AddChild(new IncrementNode(token.Value));
							break;
						case "--":
							currentnode.AddChild(new DecrementNode(token.Value));
							break;
						case "+=":
							assignedvariable = getVariable(token.Value, currentscope);

							valuenode = tokens[++i];
							if (valuenode.Type == Variable) {
								var valuevariable = getVariable(valuenode.Value, currentscope);
								currentnode.AddChild(new AdditionAssignmentNode(assignedvariable.Name, new VariableValueNode(valuevariable.Name)));
							} else if (valuenode.Type == Immediate) {
								var expr = CreateExpression(tokens, currentscope, ref i);
								i--;
								currentnode.AddChild(new AdditionAssignmentNode(assignedvariable.Name, new ShortValueNode(expr)));
							}

							break;
						case "-=":
							assignedvariable = getVariable(token.Value, currentscope);

							valuenode = tokens[++i];
							if (valuenode.Type == Variable) {
								var valuevariable = getVariable(valuenode.Value, currentscope);
								currentnode.AddChild(new SubtractionAssignmentNode(assignedvariable.Name, new VariableValueNode(valuevariable.Name)));
							} else if (valuenode.Type == Immediate) {
								var exp = CreateExpression(tokens, currentscope, ref i);
								i--;
								currentnode.AddChild(new SubtractionAssignmentNode(assignedvariable.Name, new ShortValueNode(exp)));
							}

							break;
						default: throw new SyntaxException("Expected operator");
					}
				} else if (token.Type == DataType) {
					// DataType Variable = [Variable/Immediate]
					var variabletoken = tokens[++i];
					if (variabletoken.Type != Variable)
						throw new SyntaxException("Expected variable name after data type");

					var variabledatatype = BuiltIn.DataTypes.Get(token.Value);
					if (variabledatatype == null)
						throw new SyntaxException($"Datatype {token.Value} does not exist.");

					var operatornode = tokens[++i];
					if (operatornode.Value == ";" || operatornode.Value == "=") {
						if (currentscope.GetMember(variabletoken.Value) != null)
							throw new SyntaxException($"Variable {variabletoken.Value} already exists in the current scope.");

						currentnode.AddChild(new VariableDeclarationNode(token.Value, variabletoken.Value));

						if (operatornode.Value == ";") { // Just a decleration
							currentnode.AddChild(new VariableAssignmentNode(variabletoken.Value, new ShortValueNode(0)));
							currentscope.AddMember(new VariableMember(variabledatatype, variabletoken.Value));
							continue;
						}
					}

					if (operatornode.Value != "=")
						throw new SyntaxException("Expected token '='");

					var valuenode = tokens[++i];
					var expression = CreateExpression(tokens, currentscope, ref i);
					if (expression == null)
						throw new SyntaxException($"Unexpected symbol on variable assignment '{valuenode.Value}'");
					i--;
					//var immediatedatatype = GetImmedateDataType(expression);
					//checkCanConvertTypes(immediatedatatype, variabledatatype);
					currentnode.AddChild(new VariableAssignmentNode(variabletoken.Value, expression));
					currentscope.AddMember(new VariableMember(variabledatatype, variabletoken.Value));
				} else if (token.Type == ControlFlow) {
					switch (token.Value) {
						case "while": break;
						case "do": break;
						case "for": break;
						case "return": break;
						case "continue": break;
						case "else": break;
						case "if":
							if (tokens[i + 1].Value != "(")
								throw new SyntaxException("Expected expression after IF statement.");

							i += 2;
							var expression = CreateExpression(tokens, currentscope, ref i);

							if (tokens[i].Value != "{")
								throw new SyntaxException("Expected open block after IF statement.");

							i++;
							var iftrue = CreateAST(tokens, new BlockNode(), ref i, currentscope);
							var iffalse = new BlockNode();
							var elsetoken = tokens.Get(++i);
							if (elsetoken?.Value == "else")
								iffalse = CreateAST(tokens, iffalse, ref i, currentscope);

							currentnode.AddChild(new IfNode(expression, iftrue, iffalse));
							break;
						case "goto":
							var nextnode = tokens[++i];
							if (nextnode.Type != Variable)
								throw new SyntaxException("Expected label name after goto statement.");
							currentnode.AddChild(new GotoNode(nextnode.Value));
							break;
						default: // Label
							var colonindex = token.Value.IndexOf(':');
							currentnode.AddChild(new LabelNode(token.Value.Substring(0, colonindex)));
							break;
					}
				} else if (token.Value == "*") {
					var addresstoken = tokens[++i];
					if (addresstoken.Type != Immediate)
						throw new SyntaxException("Expected address after pointer.");
					var address = ParseImmediate(addresstoken.Value);

					var equals = tokens[++i];
					if (equals.Value != "=")
						throw new SyntaxException("Expected assignment after memory address.");

					i++;
					var value = CreateExpression(tokens, currentscope, ref i);
					currentnode.AddChild(new MemoryAssignmentNode(address, value));
				} else if (token.Value == "}") {
					return rootnode;
				}
			}

			return rootnode;
		}

		public static ExpressionNode CreateExpression(IList<Token> tokens) {
			var x = 0;
			return CreateExpression(tokens, new Scope(), ref x);
		}

		public static ExpressionNode CreateExpression(IList<Token> tokens, Scope scope, ref int index) {
			var nodes = new List<ExpressionNode>();

			Token currenttoken;
			do {
				currenttoken = tokens[index++];

				switch (currenttoken.Type) {
					case Immediate:
						nodes.Add(new ShortValueNode(ParseImmediate(currenttoken.Value)));
						break;
					case Operator:
					case Comparison:
						nodes.Add(CreateOperator(currenttoken.Value));
						break;
					case Variable:
						var var = getVariable(currenttoken.Value, scope);
						// TODO: Decide on what intermedate values' types are and check for type compatibility
						nodes.Add(new VariableValueNode(var.Name));
						break;
					default:
						if (currenttoken.Value == "*") {
							var addresstoken = tokens[index++];
							if (addresstoken.Type != Immediate)
								throw new SyntaxException("Expected address after pointer.");
							var address = ParseImmediate(addresstoken.Value);
							nodes.Add(new MemoryValueNode(address));
						} else if (currenttoken.Value == "(") {
							nodes.Add(CreateExpression(tokens, scope, ref index));
						}

						//TODO: Throw as token was unexpected at this point
						break;
				}
			} while (index < tokens.Count && currenttoken.Value != ")" && currenttoken.Value != ";");

			return ConvergeOperators(nodes);
		}

		private static ExpressionNode CreateOperator(string op) => Operators[op]();

		private static ExpressionNode ConvergeOperators(IList<ExpressionNode> nodes) {
			if (nodes.Count == 0)
				return null;
			if (nodes.Count == 1) {
				if (nodes[0] is ValueNode)
					return nodes[0];
				if (nodes[0] is BinaryOperatorNode op && (op.Left == null || op.Right == null))
					throw new SyntaxException("Left or right hand side of expression is missing.");
			}

			// Boolean operators
			// Should be converged in order of appearance
			ConvergeOperators<ComparisonNode>(nodes);
			ConvergeNegateOperator(nodes);
			// Math operators
			ConvergeOperators<AdditionNode>(nodes);
			ConvergeOperators<SubtractionNode>(nodes);

			if (nodes.Count > 1)
				throw new SyntaxException("Not a valid expression. Operators are not balanced.");

			return nodes[0];
		}

		private static void ConvergeOperators<T>(IList<ExpressionNode> nodes) where T : BinaryOperatorNode {
			for (var i = 1; i < nodes.Count - 1; i++) {
				if (!(nodes[i] is T op))
					continue;

				if (op.Left != null || op.Right != null)
					continue;

				// Validate the left and right sides
				if (i - 1 < 0)
					throw new SyntaxException("No expression found on the left of the operator.");
				if (i + 1 >= nodes.Count)
					throw new SyntaxException("No expression found on the right of the operator.");
				if (nodes[i - 1] is ExpressionNode left) {
					if (nodes[i + 1] is ExpressionNode right) {
						op.Left = left;
						op.Right = right;

						nodes.RemoveAt(i - 1);
						i--;
						nodes.RemoveAt(i + 1);
					} else {
						throw new SyntaxException("The right side of the operator is not an expresison.");
					}
				} else {
					throw new SyntaxException("The left side of the operator is not an expresison.");
				}
			}
		}

		// Annoyingly the negate operator is the only one that requires a right side expression only
		private static void ConvergeNegateOperator(IList<ExpressionNode> nodes) {
			for (var i = 0; i < nodes.Count - 1; i++) {
				if (!(nodes[i] is NegateNode negate))
					continue;

				// No need to check bounds as its enforced in the loop condition

				if (nodes[i + 1] is ExpressionNode right) {
					negate.Expression = right;

					nodes.RemoveAt(i + 1);
				} else {
					throw new SyntaxException("The right side of the operator is not an expresison.");
				}
			}
		}

		public static PrimitiveDataType GetImmedateDataType(ushort value) {
			PrimitiveDataType smallestDataType = null;
			foreach (var datatype in BuiltIn.DataTypes.All.Reverse())
				if (value < datatype.MaxValue)
					smallestDataType = datatype;

			// Exeption cant possibly throw as ParseImmediate parses up to int data type
			if (smallestDataType == null)
				throw new SyntaxException($"No datatype suitable for value ({value})");

			return smallestDataType;
		}

		private static void checkCanConvertTypes(PrimitiveDataType from, PrimitiveDataType to) {
			if (!BuiltIn.DataTypes.CanConvertTo(from, to))
				throw new SyntaxException($"No conversion from {from.Name} to {to.Name}.");
		}

		private static VariableMember getVariable(string name, Scope scope) {
			var existingvariable = scope.GetMember(name);
			if (existingvariable == null)
				throw new SyntaxException($"Varible {name} does not exist in the current scope.");

			return existingvariable;
		}
	}
}
