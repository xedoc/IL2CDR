using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class Hits
    {
        public Hits(string ammo)
        {
            Ammo = ammo;
        }
        public string AmmoId { get; set; }

        private string ammo;
        public string Ammo
        {
            get { return ammo; }
            set {                 
                ammo = value;
                AmmoId = GuidUtility.Create(GuidUtility.IsoOidNamespace, ammo).ToString(); 
            }
        }        
        public int HitCount { get; set; }
        public double Damage { get; set; }
    }

    public class HitsSource
    {
        public Player Player { get; set; }
        public GameObject Object { get; set; }
        public Hits Hits { get; set; }
    }
}
