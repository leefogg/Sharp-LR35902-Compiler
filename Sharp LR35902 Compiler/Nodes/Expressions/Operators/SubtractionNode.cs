﻿using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class SubtractionNode : BinaryOperatorNode {
		public SubtractionNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

		// Allow valueless construction
		public SubtractionNode() { }

		public override ushort GetValue() => (ushort)(Left - Right);

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) {
			var left = Left.Optimize(knownvariables);
			var right = Right.Optimize(knownvariables);
			if (left is ConstantNode && right is ConstantNode)
				return new ShortValueNode((ushort)(left.GetValue() - right.GetValue()));

			return new SubtractionNode(left, right);
		}

		public override bool Matches(Node obj) {
			if (obj is SubtractionNode other)
				return Left.Matches(other.Left) && Right.Matches(other.Right);

			return false;
		}

		protected override string GetSymbol() => BuiltIn.Operators.Subtract;
	}
}
