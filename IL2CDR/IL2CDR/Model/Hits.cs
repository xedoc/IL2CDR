using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IL2CDR.Model
{
    public class Hit
    {
        public Hit(string ammo)
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
    }

    public class Hits : List<Hit>
    {
        public double Damage {get;set;}

        public int HitCount {
            get { return this.Count <= 0 ? 0 : this.Sum(hit => hit.HitCount); }
        }
                
        public Hit this[string ammo]
        {
            get
            {
                return this.FirstOrDefault( hit => hit.Ammo == ammo );
            }
            set
            {
                if (String.IsNullOrWhiteSpace(ammo))
                    return;

                var existing = this[ammo];
                if (existing == null)
                {
                    this.Add(new Hit(ammo));
                    existing = this[ammo];
                }                
                existing.HitCount = value.HitCount;
            }
        }
    }

    [JsonConverter(typeof(HitsSourceConverter))]
    public class HitsSource
    {
        public HitsSource()
        {
            Hits = new Hits();
        }
        public Player Player { get; set; }
        public GameObject Object { get; set; }
        public Hits Hits { get; set; }
    }

    public class HitsSourceConverter : JsonConverter
    {
        private readonly Type[] _types;
        public HitsSourceConverter(params Type[] types)
        {
            _types = types;            
        }
        public HitsSourceConverter()
        {
            _types = new Type[] { typeof(HitsSource) };
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            if (!(value is HitsSource) )
            {
                JToken t = JToken.FromObject(value);
                t.WriteTo(writer);
            }    
            else
            {
                JObject o = new JObject();
                var hitsSource = value as HitsSource;

                var player_id = hitsSource.Player == null ? "" : hitsSource.Player.NickId.ToString().Replace("-", "");
                var object_id = hitsSource.Object == null ? "" : hitsSource.Object.ObjectId.ToString().Replace("-", "");
                o.Add("PlayerId", player_id);
                o.Add("ObjectId", object_id);

                var hits = hitsSource.Hits == null ? null : JToken.FromObject(hitsSource.Hits);
                o.Add("Hits", hits);
                o.WriteTo(writer);
            }
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //will be never called because CanRead is false
            throw new NotImplementedException();
        }
        public override bool CanRead
        {
            get { return false; }
        }
    
        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }
    }
}
