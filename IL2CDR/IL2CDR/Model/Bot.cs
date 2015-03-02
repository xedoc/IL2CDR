using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class Bot : GameObject
    {
        public Bot(GameObject obj) : base(obj.Id)
        {

        }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
