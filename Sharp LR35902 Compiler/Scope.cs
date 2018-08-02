using System.Collections.Generic;
using System.Linq;

namespace Sharp_LR35902_Compiler
{
	public class Scope
	{
		private List<VariableMember> Members = new List<VariableMember>();
		private Scope ParentScope;

		public Scope() { }
		public Scope(Scope parentscope)
		{
			ParentScope = parentscope;
		}

		public void AddMember(VariableMember member) => Members.Add(member);

		public VariableMember GetLocalMember(string name) => Members.FirstOrDefault(m => m.Name == name);
		public VariableMember GetMember(string name)
		{
			var localmember = GetLocalMember(name);
			if (localmember != null)
				return localmember;

			return ParentScope?.GetMember(name);
		}

		public bool MemberExists(string name) => GetMember(name) != null;
	}
}
