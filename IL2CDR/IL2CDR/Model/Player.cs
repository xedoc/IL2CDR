using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class Player
    {
        public int PlayerId { get; set; }
        public Guid NickId { get; set; }
        public Guid LoginId { get; set; }
        public string NickName { get; set; }
        public Country Country { get; set; }
        public Plane Plane { get; set; }
        public bool IsInAir { get; set; }        
    }
}
