namespace IL2CDR.Model
{
	public interface IStopStart
	{
		bool IsRunning { get; }

		void Start();
		void Stop();
		void Restart();
	}
}