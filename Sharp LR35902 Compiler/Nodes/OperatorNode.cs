using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public abstract class OperatorNode : ExpressionNode
	{
		public ExpressionNode Left { get; set; }
		public ExpressionNode Right { get; set; }

		public OperatorNode(ExpressionNode left, ExpressionNode right)
		{
			Left = left;
			Right = right;
		}
		public OperatorNode() { }

		public override IEnumerable<string> GetUsedRegisterNames()
		{
			foreach (var variablename in Left.GetUsedRegisterNames())
				yield return variablename;
			foreach (var variablename in Right.GetUsedRegisterNames())
				yield return variablename;
		}

		public override Node[] GetChildren() => new[] { Left, Right };
	}
}