using System.Collections.Generic;
using System.Linq;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class IfNode : Node
    {
		public ExpressionNode Condition { get; }
        public BlockNode IfTrue { get; }

        public IfNode(ExpressionNode condition, BlockNode iftrue)
		{
            Condition = condition;
            IfTrue = iftrue;
		}

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            foreach (var variablename in Condition.GetUsedRegisterNames())
                yield return variablename;
            foreach (var variablename in IfTrue.GetUsedRegisterNames())
                yield return variablename;
        }

		public override bool Equals(object obj)
		{
			if (obj is IfNode other)
				return other.Condition.Equals(Condition) && other.IfTrue.Equals(IfTrue);

			return false;
		}

		public override Node[] GetChildren() => Condition.GetChildren().Concat(IfTrue.GetChildren()).ToArray();
	}
}
