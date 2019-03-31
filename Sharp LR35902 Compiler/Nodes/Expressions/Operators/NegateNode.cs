using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class NegateNode : UnaryOperatorNode
	{
		public NegateNode(ExpressionNode expression) { Expression = expression; }

		// Allow valueless construction
		public NegateNode() { }

		public override ushort GetValue() => Expression.GetValue() == 0 ? (ushort)1 : (ushort)0;

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) {
			Expression = Expression.Optimize(knownvariables);
			if (Expression is ConstantNode)
				return new ShortValueNode(BooleanToShort(!IsTrue(Expression.GetValue())));

			return this;
		}

		public override bool Equals(object obj)
		{
			if (obj is NegateNode other)
				return other.Expression.Equals(Expression);

			return false;
		}
	}
}
