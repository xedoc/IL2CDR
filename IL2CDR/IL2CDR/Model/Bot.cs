using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
	public class Bot : GameObject
	{
		public Bot(GameObject obj, string editorName, string name)
			: base(obj.Id, name)
		{
			this.NameInEdtor = editorName;
			this.Name = name;
		}

		public string NameInEdtor { get; set; }
	}
}