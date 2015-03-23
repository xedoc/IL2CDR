using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class GameObjectBase
    {
        public string LastHitAmmoName { get; set; }
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
            if (source == null)
                return;
            LastHitAmmoName = e.AmmoName;

            if (source is GameObject)
            {
                var existing = HitsSources.FirstOrDefault(o =>
                    o != null &&
                    o.Object.Id == (source as GameObject).Id &&
                    e.AmmoName == o.Hits.Ammo);

                if (existing != null)
                {
                    existing.Hits.HitCount++;
                }
                else
                {
                    HitsSources.Add(new HitsSource()
                    {
                        Object = source as GameObject,
                        Hits = new Hits(e.AmmoName)
                        {
                            HitCount = 1,
                        }
                    });
                }
            }
            else if (source is Player)
            {
                var existing = HitsSources.FirstOrDefault(o =>
                    o != null &&
                    o.Player.Id == (source as Player).Id &&
                    e.AmmoName == o.Hits.Ammo);

                if (existing != null)
                {
                    existing.Hits.HitCount++;
                }
                else
                {
                    HitsSources.Add(new HitsSource()
                    {
                        Player = source as Player,
                        Hits = new Hits(e.AmmoName)
                        {
                            HitCount = 1,
                        }
                    });
                }
            }
        }
        public void AddDamage(object source, MissionLogEventDamage e)
        {
            if (source == null)
                return;

            if (source is GameObject)
            {
                if (String.IsNullOrWhiteSpace(LastHitAmmoName))
                    LastHitAmmoName = HitsSources.FirstOrDefault().With(o => o.Hits.Ammo);

                if (String.IsNullOrWhiteSpace(LastHitAmmoName))
                    LastHitAmmoName = "Unknown";

                var existing = HitsSources.FirstOrDefault(o =>
                    o != null &&
                    o.Object.Id == (source as GameObject).Id &&
                    o.Hits.Ammo == LastHitAmmoName
                    );

                if (existing != null)
                    existing.Hits.Damage += e.Damage;
            }
            else if (source is Player)
            {
                var existing = HitsSources.FirstOrDefault(o =>
                    o != null &&
                    o.Player.Id == (source as Player).Id &&
                   o.Hits.Ammo == LastHitAmmoName
                   );

                if (existing != null)
                    existing.Hits.Damage += e.Damage;
            }

        }
    }
}
