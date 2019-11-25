using System.Collections.ObjectModel;

namespace IL2CDR.Model
{
	public class AppLogDataService
	{
		private readonly object lockMessages = new object();
		public ObservableCollection<string> LogMessages { get; set; }

		public AppLogDataService()
		{
			this.LogMessages = new ObservableCollection<string>();
		}

		public void AddMessage(string text)
		{
			lock (this.lockMessages) {
				this.LogMessages.Add(text);
			}
		}
	}
}