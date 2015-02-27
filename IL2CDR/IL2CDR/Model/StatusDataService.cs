using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class StatusDataService
    {
        private object lockStatusList = new object();
        public  ObservableCollection<Status> StatusList{ get; set; }

        public StatusDataService()
        {
            StatusList = new ObservableCollection<Status>() { 
                new Status() { StatusName = "config", Title = "IL2 configuration file", ErrorDescription = "checking...", Type = StatusType.Error },
                new Status() { StatusName = "dserver", Title = "IL2 Dedicated server", ErrorDescription = "checking...", Type = StatusType.Error },
                new Status() { StatusName = "rcon", Title = "RCON connection", ErrorDescription = "checking...", Type = StatusType.Error },
                new Status() { StatusName = "dserver", Title = "Dedicated server", ErrorDescription = "checking...", Type = StatusType.Error },                
            };
        }
        public StatusType GetStatus(string name)
        {
            var currentStatus = StatusList.FirstOrDefault(x => x.StatusName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (currentStatus != null)
                return currentStatus.Type;
            else
                return StatusType.Error;
        }
        public void ChangeStatus( string name, StatusType type, string error = "")
        {
            var currentStatus = StatusList.FirstOrDefault(x => x.StatusName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (currentStatus == null)
                return;

            UI.Dispatch(() =>
            {
                currentStatus.ErrorDescription = error;
                currentStatus.Type = type; 
            });
        }
        public void AddStatus( Status status )
        {
            var currentStatus = StatusList.FirstOrDefault( x => x.StatusName.Equals(status.StatusName, StringComparison.InvariantCultureIgnoreCase));
            if (currentStatus != null)
            {
                return;
            }
            else
            {
                lock (lockStatusList)
                    UI.Dispatch(() => StatusList.Add(status));
            }
        }
        public void ResetStatus()
        {
            lock(lockStatusList)
                UI.Dispatch(()=>StatusList.Clear());
        }
        public void RemoveStatus( string name )
        {
            lock(lockStatusList)
                UI.Dispatch(()=>StatusList.RemoveAll(x => x.StatusName.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
