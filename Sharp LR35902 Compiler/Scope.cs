using System.Collections.Generic;
using System.Linq;

namespace Sharp_LR35902_Compiler
{
	public class Scope
	{
		private List<Member> Members = new List<Member>();
		private Scope ParentScope;

		public Scope() { }
		public Scope(Scope parentscope)
		{
			ParentScope = parentscope;
		}

		public void AddMember(Member member) => Members.Add(member);

		public Member GetLocalMember(string name) => Members.FirstOrDefault(m => m.Name == name);
		public Member GetMember(string name)
		{
			var localmember = GetLocalMember(name);
			if (localmember != null)
				return localmember;

			return ParentScope.GetMember(name);
		}
	}
}
