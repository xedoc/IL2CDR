using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class Player : GameObject
    {
        public Player(GameObject obj) : base(obj.Id)
        {
            this.Id = obj.Id;
        }
        public Guid NickId { get; set; }
        public Guid LoginId { get; set; }
        public string NickName { get; set; }
    }
}
