using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace IL2CDR.Model
{
    public class AppLogDataService
    {
        public ObservableCollection<string> LogMessages { get; set; }
        public AppLogDataService()
        {
            LogMessages = new ObservableCollection<string>();
        }

        public void AddMessage( string text )
        {
            LogMessages.Add(text);
        }
    }
}
