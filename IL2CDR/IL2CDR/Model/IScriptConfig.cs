namespace IL2CDR.Model
{
	public interface IScriptConfig
	{
		ScriptConfig DefaultConfig { get; }
		ScriptConfig Config { get; set; }
	}
}