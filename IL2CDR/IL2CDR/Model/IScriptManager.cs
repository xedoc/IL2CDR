using System.Collections.Generic;

namespace IL2CDR.Model
{
	public interface IScriptManager
	{
		List<IActionScript> ActionScripts { get; }
	}
}