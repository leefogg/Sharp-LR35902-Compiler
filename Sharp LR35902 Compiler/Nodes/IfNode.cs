using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class IfNode : Node
    {
		public ComparisonNode Condition { get; }
        public Node IfTrue { get; }

        public IfNode(ComparisonNode condition, Node iftrue)
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
    }
}
