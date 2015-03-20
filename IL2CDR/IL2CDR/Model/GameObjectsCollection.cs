using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class GameObjectsCollection : Dictionary<int, GameObject>
    {
        private object lockList = new object();
        public new GameObject this[int id]
        {
            get
            {
                GameObject result = null;
                lock (lockList)
                    this.TryGetValue(id, out result);
                return result;
            }
            set
            {
                if (value == null || value.Id <= 0)
                    return;
                lock (lockList)
                {
                    var existing = this[value.Id];
                    if (existing == null)
                        this.Add(value.Id, value);
                    else
                        existing = value;
                }
            }
        }
        public void ObjectDestroyed(int id)
        {
            var gameObject = this[id];
            if (gameObject == null)
                return;
            
            gameObject.IsDestroyed = true;
            
            //TODO: handle object destruction
        }
    }

}
