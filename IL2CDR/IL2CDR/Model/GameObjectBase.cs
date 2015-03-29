using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IL2CDR.Model
{
    public class GameObjectBase
    { 
        public List<HitsSource> HitsSources { get; set; }
        
        public GameObjectBase()
        {
            HitsSources = new List<HitsSource>();
        }
        public int GetHitsCountBy( object obj )
        {
            if (obj == null)
                return 0;

            if (obj is Player)
            {
                var source = HitsSources.FirstOrDefault(p => p.Player != null && p.Player.Id == (obj as Player).Id);
                if (source != null)
                    return source.Hits.HitCount;
            }
            else if (obj is GameObject)
            {
                var source = HitsSources.FirstOrDefault(p => p.Object != null && p.Object.Id == (obj as GameObject).Id);
                if (source != null)
                    return source.Hits.HitCount;
            }

            
            return 0;
        }
        public double GetDamageBy(object obj)
        {
            if (obj == null)
                return 0;

            if (obj is Player)
            {
                var source = HitsSources.FirstOrDefault(p => p.Player != null && p.Player.Id == (obj as Player).Id);
                if (source != null)
                    return source.Hits.Damage;
            }
            else if (obj is GameObject)
            {
                var source = HitsSources.FirstOrDefault(p => p.Object != null && p.Object.Id == (obj as GameObject).Id);
                if (source != null)
                    return source.Hits.Damage;
            }


            return 0;
        }
        public void AddHit(object source, MissionLogEventHit e)
        {
            if (source == null || e == null || String.IsNullOrWhiteSpace(e.AmmoName) )
                return;

            if (source is GameObject)
            {
                var existing = HitsSources.FirstOrDefault(o =>
                    o != null &&
                    o.Hits != null &&
                    o.Object != null &&
                    o.Object.Id == (source as GameObject).Id );

                if (existing == null)
                {
                    existing = new HitsSource()
                    {
                        Object = source as GameObject,
                    };

                    HitsSources.Add(existing);
                }
                var hits = existing.Hits[e.AmmoName];
                if (hits == null)
                    existing.Hits[e.AmmoName] = new Hit(e.AmmoName) { HitCount = 1 };
                else
                    existing.Hits[e.AmmoName].HitCount++;
            }
            else if (source is Player)
            {
                var existing = HitsSources.FirstOrDefault(o =>
                    o != null &&
                    o.Hits != null &&
                    o.Player != null &&
                    o.Player.Id == (source as Player).Id 
                    );

                if (existing == null)
                {
                    existing = new HitsSource()
                    {
                        Player = source as Player,
                    };

                    HitsSources.Add(existing);
                }
                var hits = existing.Hits[e.AmmoName];
                if (hits == null)
                    existing.Hits[e.AmmoName] = new Hit(e.AmmoName) { HitCount = 1 };
                else
                    existing.Hits[e.AmmoName].HitCount++;
            }
        }
        public void AddDamage(object source, MissionLogEventDamage e)
        {
            if (source == null || e.Damage <= 0)
                return;

            if (source is GameObject)
            {

                var existing = HitsSources.FirstOrDefault(o =>
                    o != null &&
                    o.Hits != null &&
                    o.Object != null &&
                    o.Object.Id == (source as GameObject).Id
                    );

                if (existing != null)
                    existing.Hits.Damage += e.Damage;
            }
            else if (source is Player)
            {
                var existing = HitsSources.FirstOrDefault(o =>
                    o != null &&
                    o.Hits != null &&
                    o.Player != null &&
                    o.Player.Id == (source as Player).Id 
                   );

                if (existing != null)
                    existing.Hits.Damage += e.Damage;
            }

        }
    }
}
