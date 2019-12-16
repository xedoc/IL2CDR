using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Web.UI.WebControls;

namespace IL2CDR.Model
{
	public class AppLogDataService
	{
		private readonly object lockMessages = new object();
		public ObservableCollection<string> LogMessages { get; set; }

		/**
		 * Maximum number of lines hold in the buffer. 
		 */
		public int MaxNumberOfLines { get; set; } = 300;


		public AppLogDataService()
		{
			this.LogMessages = new ObservableCollection<string>();

		}

		public void AddMessage(string text)
		{
			lock (this.lockMessages) {
				this.LogMessages.Add(text);

				if (this.LogMessages.Count > this.MaxNumberOfLines) {
					this.TruncateFirstLinesUntil(this.MaxNumberOfLines); 
				}
			}
		}


		/// <summary>
		/// Truncates first lines from this.LogMessages, so there are only maxLinesAllowed left.
		///
		/// Beware! The minimum number of lines is 10! (to prevent 'strange' behavior if somebody asks to allow only 0 lines due to error in initialisation of the config). 
		/// 
		/// </summary>
		/// <param name="maxLinesAllowed"></param>
		private void TruncateFirstLinesUntil(int maxLinesAllowed)
		{
			int maxLinesSanitized = Math.Max(maxLinesAllowed, 10);
			while (this.LogMessages.Count > maxLinesSanitized) {
				this.LogMessages.RemoveAt(0);
			}
			
		}

	}
}