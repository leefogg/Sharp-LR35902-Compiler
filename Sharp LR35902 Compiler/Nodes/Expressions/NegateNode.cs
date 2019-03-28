﻿using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class NegateNode : ExpressionNode {
		public ExpressionNode Expression { get; set; }

		public NegateNode(ExpressionNode expression) { Expression = expression; }

		// Allow valueless construction
		public NegateNode() { }

		public override ushort GetValue() => Expression.GetValue() == 0 ? (ushort)1 : (ushort)0;

		public override Node[] GetChildren() => NoChildren;

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) {
			Expression = Expression.Optimize(knownvariables);
			if (Expression is ConstantNode)
				return new ShortValueNode(booleanToShort(!isTrue(Expression.GetValue())));

			return this;
		}
	}
}