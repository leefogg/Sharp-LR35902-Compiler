using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler
{
	public class SymbolTable
	{
		private readonly Dictionary<string, ushort> LabelLocations = new Dictionary<string, ushort>();
		private readonly Dictionary<string, ushort> Definitions = new Dictionary<string, ushort>();

		public void AddLabelLocation(string name, ushort location) {
			LabelLocations[name] = location;
		}
		public bool TryGetLabelLocation(string name, out ushort location) => LabelLocations.TryGetValue(name, out location);
		public void ClearLabelLocations() => LabelLocations.Clear();
		public void AddDefinition(string name, ushort location) {
			Definitions[name] = location;
		}
		public bool TryGetDefinition(string name, out ushort location) => Definitions.TryGetValue(name, out location);
		public void ClearDefinitions() => Definitions.Clear();

		public string CreateSymbolFile() {
			var sb = new StringBuilder(LabelLocations.Count * (3 + 5 + 8) + Definitions.Count * (3 + 5 + 8));
			foreach (var key in LabelLocations.Keys) {
				sb.Append("00:"); // We dont support banks yet
				var location = LabelLocations[key];
				sb.Append(location.ToString("X4"));
				sb.Append(' ');
				sb.AppendLine(key);
			}

			return sb.ToString();
		}
	}
}
