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
        public Bot(GameObject obj, string type, string name)
            : base(obj.Id)
        {
            Type = type;
            Name = name;
        }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
