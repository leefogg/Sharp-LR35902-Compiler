using System.Collections.Generic;
using System.Linq;
using Sharp_LR35902_Compiler.Nodes;
using System.Linq;
using static Sharp_LR35902_Compiler.Nodes.ExpressionNode;

namespace Sharp_LR35902_Compiler {
	public static class Optimizer {
		public static void Optimize(BlockNode block) {
			while (PropagateConstants(block) || RemoveUnusedVariables(block) || TransformToIncDec(block)) { }

			foreach (var child in block.GetChildren())
				if (child is IfNode ifnode)
					Simplify(ifnode.IfTrue);
		}

		public static void Simplify(BlockNode block) {
			while (TransformAdditionAssignmentToExpression(block) 
				|| TransformSubtractionAssignmentToExpression(block) 
				|| FlattenExpressions(block)) { }

			foreach (var child in block.GetChildren())
				if (child is IfNode ifnode)
					Simplify(ifnode.IfTrue);
		}

		public static bool TransformToIncDec(BlockNode block) {
			var children = block.GetChildren().ToList();
			var changesmade = false;
			for (var i = 0; i < children.Count; i++) {
				var node = children[i];

				if (!(node is VariableAssignmentNode assignment))
					continue;

				BinaryOperatorNode expression = null;
				if (assignment.Value is AdditionNode add)
					expression = add;
				if (assignment.Value is SubtractionNode sub)
					expression = sub;
				if (expression == null)
					continue;

				ExpressionNode left, right;
				if (expression.Left is ShortValueNode) {
					left = expression.Left;
					right = expression.Right;
				} else {
					right = expression.Left;
					left = expression.Right;
				}
				if (left is ShortValueNode value && value.Value == 1 && right is VariableValueNode var && var.VariableName == assignment.VariableName) {
					children.RemoveAt(i);
					block.RemoveChild(i);
					if (expression is AdditionNode) {
						block.AddChild(new IncrementNode(assignment.VariableName));
					} else {
						block.AddChild(new DecrementNode(assignment.VariableName));
					}

					changesmade = true;
				}
			}

			return changesmade;
		}

		public static bool PropagateConstants(BlockNode block) {
			var changesmade = false;

			var variablevalues = new Dictionary<string, ushort>();
			var children = block.GetChildren().ToList();

			for (var i = 0; i < children.Count; i++) {
				var node = children[i];

				if (node is AssignmentNode assignment) {
					var newvalue = assignment.Value.Optimize(variablevalues);
					if (!assignment.Value.Matches(newvalue))
						changesmade = true;

					assignment.Value = newvalue;
				}

				if (node is VariableAssignmentNode varassignment) {
					if (varassignment.Value is ConstantNode value) {
						if (variablevalues.ContainsKey(varassignment.VariableName))
							variablevalues[varassignment.VariableName] = value.GetValue();
						else
							variablevalues.Add(varassignment.VariableName, value.GetValue());
					}
				} else if (node is IncrementNode inc) {
					if (variablevalues.ContainsKey(inc.VariableName)) {
						variablevalues[inc.VariableName]++;
						children.RemoveAt(i);
						block.RemoveChild(i--);
						changesmade = true;
					}
				} else if (node is DecrementNode dec) {
					if (variablevalues.ContainsKey(dec.VariableName)) {
						variablevalues[dec.VariableName]--;
						children.RemoveAt(i);
						block.RemoveChild(i--);
						changesmade = true;
					}
				} else if (node is IfNode ifnode) {
					if (!(ifnode.Condition is ExpressionNode condition))
						continue;
					ifnode.Condition = condition.Optimize(variablevalues);
					if (!(ifnode.Condition is ConstantNode c))
						continue;

					children.RemoveAt(i);
					block.RemoveChild(i);
					changesmade = true;

					if (IsTrue(c)) {
						children.InsertRange(i, ifnode.IfTrue.GetChildren());
						var ifchildren = ifnode.IfTrue.GetChildren().ToList();
						for (var j = 0; j < ifchildren.Count; j++)
							block.InsertAt(ifchildren[j], i + j);
					} else {
						children.InsertRange(i, ifnode.IfFalse.GetChildren());
						var ifchildren = ifnode.IfFalse.GetChildren().ToList();
						for (var j = 0; j < ifchildren.Count; j++)
							block.InsertAt(ifchildren[j], i + j);
					}
					i--;
				}
			}

			return changesmade;
		}

		public static bool RemoveUnusedVariables(BlockNode block) {
			var usedvariables = block.GetReadVariables().ToArray();

			var changesmade = false;
			var i = 0;
			foreach (var node in block.GetChildren().ToList()) {
				if (node is VariableDeclarationNode dec) {
					if (!usedvariables.Contains(dec.VariableName)) {
						block.RemoveChild(i--);
						changesmade = true;
					}
				} else if (node is VariableAssignmentNode assignment) {
					if (!usedvariables.Contains(assignment.VariableName)) {
						block.RemoveChild(i--);
						changesmade = true;
					}
				}

				i++;
			}

			return changesmade;
		}

		public static bool TransformAdditionAssignmentToExpression(BlockNode block) {
			var changesmade = false;
			var children = block.GetChildren().ToList();
			for (var i = 0; i < children.Count; i++) {
				var node = children[i];
				if (!(node is AdditionAssignmentNode assignment))
					continue;

				block.RemoveChild(i);
				block.InsertAt(new VariableAssignmentNode(assignment.VariableName, new AdditionNode(new VariableValueNode(assignment.VariableName), assignment.Value)), i);
				changesmade = true;
			}

			return changesmade;
		}

		public static bool TransformSubtractionAssignmentToExpression(BlockNode block) {
			var changesmade = false;
			var children = block.GetChildren().ToList();
			for (var i = 0; i < children.Count; i++) {
				var node = children[i];
				if (!(node is SubtractionAssignmentNode assignment))
					continue;

				block.RemoveChild(i);
				block.InsertAt(new VariableAssignmentNode(assignment.VariableName, new SubtractionNode(new VariableValueNode(assignment.VariableName), assignment.Value)), i);
				changesmade = true;
			}

			return changesmade;
		}

		public static bool FlattenExpressions(BlockNode block) {
			var changesmade = false;
			for (var i = 0; i < block.GetChildren().Count(); i++) {
				var addedinstructions = FlattenExpression(block, i);
				if (addedinstructions <= 0)
					continue;

				changesmade = true;
				i += addedinstructions;
			}

			return changesmade;
		}

		public static int FlattenExpression(BlockNode block, int index) {
			var currentblock = block.GetChildren().ToList()[index];
			if (currentblock is VariableAssignmentNode assignmentnode) {
				if (!(assignmentnode.Value is BinaryOperatorNode value)) // Flat enough
					return 0;

				// Single operations per assignment is what we want. This is good, stop.
				if (value.Left is ConstantNode && value.Right is ConstantNode)
					return 0;

				block.RemoveChild(index);
				var count = 0;
				FlattenExpression(block, value, ref count, ref index, 0);
				count *= 2;
				block.InsertAt(assignmentnode, index);
				return count;
			}
			if (currentblock is IfNode ifNode) {
				if (!(ifNode.Condition is BinaryOperatorNode value)) // Flat enough
					return 0;

				var tempvarname = ExtractVariable(block, ifNode.Condition, ref index);
				ifNode.Condition = new VariableValueNode(tempvarname);

				return 2;
			}

			return 0;
		}

		private static string ExtractVariable(BlockNode block, ExpressionNode expression, ref int index, string name = "intermediate") {
			block.InsertAt(new VariableDeclarationNode("byte", name), index++);
			block.InsertAt(new VariableAssignmentNode(name, expression), index);

			return name;
		}

		private static string FlattenExpression(BlockNode block, BinaryOperatorNode op, ref int count, ref int index, int depth) {
			if (op.Left is BinaryOperatorNode left) {
				count++;
				var extractedOperationName = FlattenExpression(block, left, ref count, ref index, depth + 1);
				op.Left = new VariableValueNode(extractedOperationName);
			}

			if (op.Right is BinaryOperatorNode right) {
				count++;
				var extractedOperationName = FlattenExpression(block, right, ref count, ref index, depth + 1);
				op.Right = new VariableValueNode(extractedOperationName);
			}

			if (depth == 0)
				return string.Empty;

			// Left and Right side are known to be constants or variables now
			var intermediateVariableName = $"intermediate{count}";
			block.InsertAt(new VariableDeclarationNode("byte", intermediateVariableName), index++);
			block.InsertAt(new VariableAssignmentNode(intermediateVariableName, op), index++);

			return intermediateVariableName;
		}


		public static IEnumerable<List<Node>> CreateBasicBlocks(BlockNode block) {
			var children = block.GetChildren();
			var currentblock = new List<Node>();
			foreach (var child in children) {
				if (currentblock.Any() && child is LabelNode) {
					yield return currentblock;
					currentblock = new List<Node>();
				}

				currentblock.Add(child);

				if (currentblock.Any() && child is GotoNode) {
					yield return currentblock;
					currentblock = new List<Node>();
				}
			}

			if (currentblock.Any())
				yield return currentblock;
		}
	}
}
