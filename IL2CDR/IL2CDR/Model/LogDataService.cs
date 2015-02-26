using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace IL2CDR.Model
{
    public class LogDataService
    {
        public ObservableCollection<string> LogMessages { get; set; }
        public LogDataService()
        {
            LogMessages = new ObservableCollection<string>();
        }

        public void AddMessage( string text )
        {
            LogMessages.Add(text);
        }
    }
}
