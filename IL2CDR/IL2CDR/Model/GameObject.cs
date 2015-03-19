using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class GameObject
    {
        public GameObject(int id, string name )
        {
            Id = id;
            Name = name;
            Classification = GameObjectClass.Other;
            Purpose = String.Empty;

            GameObjectItem objInfo;
            if( GameInfo.ObjectsClassification.TryGetValue(name, out objInfo) )
            {
                Classification = objInfo.Classification;
                Purpose = objInfo.Purpose;
            }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public GameObjectClass Classification { get; set; }
        public string Purpose { get; set; }
        public Country Country { get; set; }
        public int CoalitionIndex {get; set;}
    }
}
