﻿using System.Collections.Generic;
using System.Linq;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Sharp_LR35902_Compiler.Nodes;
using static Sharp_LR35902_Compiler.Nodes.ExpressionNode;

namespace Sharp_LR35902_Compiler {
	public static class Optimizer {
		public static void Optimize(BlockNode block) {
			while (PropagateConstants(block) || RemoveUnusedVariables(block)) { }

			foreach (var child in block.GetChildren())
				if (child is IfNode ifnode)
					Simplify(ifnode.IfTrue);
		}

		public static void Simplify(BlockNode block) {
			while (TransformAdditionAssignmentToExpression(block) || TransformSubtractionAssignmentToExpression(block) || FlattenExpressions(block)) { }

			foreach (var child in block.GetChildren())
				if (child is IfNode ifnode)
					Simplify(ifnode.IfTrue);
		}

		public static bool PropagateConstants(BlockNode block) {
			var changesmade = false;

			var variablevalues = new Dictionary<string, ushort>();
			var children = block.GetChildren().ToList();

			for (var i = 0; i < children.Count; i++) {
				var node = children[i];

				if (node is VariableAssignmentNode assignment) {
					var newvalue = assignment.Value.Optimize(variablevalues);
					if (!assignment.Value.Equals(newvalue))
						changesmade = true;

					assignment.Value = newvalue;

					if (assignment.Value is ConstantNode value) {
						if (variablevalues.ContainsKey(assignment.VariableName))
							variablevalues[assignment.VariableName] = value.GetValue();
						else
							variablevalues.Add(assignment.VariableName, value.GetValue());
					}
				} else if (node is IncrementNode inc) {
					variablevalues[inc.VariableName]++;
					children.RemoveAt(i);
					block.RemoveChild(i--);
					changesmade = true;
				} else if (node is DecrementNode dec) {
					variablevalues[dec.VariableName]--;
					children.RemoveAt(i);
					block.RemoveChild(i--);
					changesmade = true;
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
						var ifchildren = ifnode.IfTrue.GetChildren();
						for (var j = 0; j < ifchildren.Length; j++)
							block.InsertAt(ifchildren[j], i + j);
					}
					i--;
				}
			}

			return changesmade;
		}

		public static bool RemoveUnusedVariables(BlockNode block) {
			// Dictionary should be safe as parser checks for redeclarations
			var usedvariables = new Dictionary<string, bool>();

			var children = block.GetChildren();

			// Scan for variables and uses
			foreach (var node in children) {
				if (node is VariableDeclarationNode dec)
					usedvariables.Add(dec.VariableName, false);

				// Check for reads, writes aren't a "use"
				if (node is VariableAssignmentNode assignment)
					foreach (var usedvar in assignment.Value.GetUsedRegisterNames())
						usedvariables[usedvar] = true; // Should be safe as parser checks for variable existence before assignment
			}

			var changesmade = false;
			var i = 0;
			foreach (var node in children) {
				if (node is VariableDeclarationNode dec) {
					if (usedvariables[dec.VariableName] == false) {
						block.RemoveChild(i--);
						changesmade = true;
					}
				} else if (node is VariableAssignmentNode assignment) {
					if (usedvariables[assignment.VariableName] == false) {
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
			var children = block.GetChildren();
			for (var i = 0; i < children.Length; i++) {
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
			var children = block.GetChildren();
			for (var i = 0; i < children.Length; i++) {
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
			for (var i = 0; i < block.GetChildren().Length; i++) {
				var addedinstructions = FlattenExpression(block, i);
				if (addedinstructions <= 0)
					continue;

				changesmade = true;
				i += addedinstructions;
			}

			return changesmade;
		}

		public static int FlattenExpression(BlockNode block, int index) {
			var currentblock = block.GetChildren()[index];
			if (currentblock is VariableAssignmentNode assignmentnode) {
				if (!(assignmentnode.Value is BinaryOperatorNode value)) // Flat enough
					return 0;

				// Single operations per assignment is what we want. This is good, stop.
				if (value.Left is ConstantNode && value.Right is ConstantNode)
					return 0;

				block.RemoveChild(index);
				var count = 0;
				FlattenExpression(block, value, ref count, 0);
				count *= 2;
				index += count;
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

		private static string FlattenExpression(BlockNode block, BinaryOperatorNode op, ref int count, int depth) {
			if (op.Left is BinaryOperatorNode left) {
				count++;
				var extractedOperationName = FlattenExpression(block, left, ref count, depth + 1);
				op.Left = new VariableValueNode(extractedOperationName);
			}

			if (op.Right is BinaryOperatorNode right) {
				count++;
				var extractedOperationName = FlattenExpression(block, right, ref count, depth + 1);
				op.Right = new VariableValueNode(extractedOperationName);
			}

			if (depth == 0)
				return string.Empty;

			// Left and Right side are known to be constants or variables now
			var intermediateVariableName = $"intermediate{count}";
			block.AddChild(new VariableDeclarationNode("byte", intermediateVariableName));
			block.AddChild(new VariableAssignmentNode(intermediateVariableName, op));

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
