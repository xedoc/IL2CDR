using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace IL2CDR.Model
{
    public class AreaCollection : Dictionary<int, Area>
    {
        private object lockList = new object();
        public new Area this[int id]
        {
            get
            {
                Area result = null;
                lock (lockList)
                    this.TryGetValue(id, out result);
                return result;
            }
            set
            {
                if (value == null || value.Id <= 0)
                    return;

                AddArea(value);
            }
        }
        public void AddArea( Area newArea)
        {
            lock (lockList)
            {
                var existing = this[newArea.Id];
                if (existing == null)
                    this.Add(newArea.Id, newArea);
            }             
        }


        public Area FindAreaByPos( Vector3D pos )
        {
            foreach( Area area in this.Values)
            {
                if (area.InBounds(pos))
                    return area;
            }
            return null;
        }
    }
}
