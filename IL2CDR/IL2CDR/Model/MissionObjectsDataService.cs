using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class MissionObjectsDataService
    {
        private object lockObjects = new object();

        public MissionObjectsDataService()
        {
            Objects = new List<object>();
        }
        public List<object> Objects { get; set; }

        public void AddObject( GameObject obj )
        {

        }
    }
}
