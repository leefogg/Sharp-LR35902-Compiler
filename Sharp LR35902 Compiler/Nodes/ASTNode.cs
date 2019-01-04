using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class ASTNode : Node
    {
        public override IEnumerable<string> GetUsedRegisterNames()
        {
            foreach (var child in Children)
                foreach (var variablename in child.GetUsedRegisterNames())
                    yield return variablename;
        }
    }
}
