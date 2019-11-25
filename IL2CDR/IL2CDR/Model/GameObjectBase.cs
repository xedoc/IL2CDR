using System.Collections.Generic;
using System.Linq;

namespace IL2CDR.Model
{
	public class GameObjectBase
	{
		public List<HitsSource> HitsSources { get; set; }

		public GameObjectBase()
		{
			this.HitsSources = new List<HitsSource>();
		}

		public int GetHitsCountBy(object obj)
		{
			if (obj == null) {
				return 0;
			}

			if (obj is Player player) {
				var source =
					this.HitsSources.FirstOrDefault(hs => hs.Player != null && hs.Player.Id == player.Id);
				if (source != null) {
					return source.Hits.HitCount;
				}
			} else if (obj is GameObject gameObj) {
				var source =
					this.HitsSources.FirstOrDefault(hs => hs.Object != null && hs.Object.Id == gameObj.Id);
				if (source != null) {
					return source.Hits.HitCount;
				}
			}


			return 0;
		}

		public Player MostDamageByPlayer()
		{
			var obj = this.HitsSources.OrderByDescending(x => x.Hits.Damage).FirstOrDefault();
			return obj?.Player;
		}

		public GameObject MostDamageByObject()
		{
			var obj = this.HitsSources.OrderByDescending(x => x.Hits.Damage).FirstOrDefault();
			return obj?.Object;
		}

		public double GetDamageBy(object obj)
		{
			if (obj == null) {
				return 0;
			}

			if (obj is Player player) {
				var source =
					this.HitsSources.FirstOrDefault(hs => hs.Player != null && hs.Player.Id == player.Id);
				if (source != null) {
					return source.Hits.Damage;
				}
			} else if (obj is GameObject gameObj) {
				var source =
					this.HitsSources.FirstOrDefault(hs => hs.Object != null && hs.Object.Id == gameObj.Id);
				if (source != null) {
					return source.Hits.Damage;
				}
			}


			return 0;
		}

		public void AddHit(object source, MissionLogEventHit e)
		{
			if (source == null || e == null || string.IsNullOrWhiteSpace(e.AmmoName)) {
				return;
			}

			if (source is GameObject sourceGo) {
				var existing = this.HitsSources.FirstOrDefault(o =>
					o?.Hits != null && o.Object != null && o.Object.Id == sourceGo.Id);

				if (existing == null) {
					existing = new HitsSource() {
						Object = sourceGo,
					};

					this.HitsSources.Add(existing);
				}

				var hits = existing.Hits[e.AmmoName];
				if (hits == null) {
					existing.Hits[e.AmmoName] = new Hit(e.AmmoName) {HitCount = 1};
				} else {
					existing.Hits[e.AmmoName].HitCount++;
				}
			} else if (source is Player player) {
				var existing = this.HitsSources.FirstOrDefault(o =>
					o?.Hits != null && o.Player != null && o.Player.Id == player.Id
				);

				if (existing == null) {
					existing = new HitsSource() {
						Player = player,
					};

					this.HitsSources.Add(existing);
				}

				var hits = existing.Hits[e.AmmoName];
				if (hits == null) {
					existing.Hits[e.AmmoName] = new Hit(e.AmmoName) {HitCount = 1};
				} else {
					existing.Hits[e.AmmoName].HitCount++;
				}
			}
		}

		public void AddDamage(object source, MissionLogEventDamage e)
		{
			if (source == null || e.Damage <= 0) {
				return;
			}

			if (source is GameObject sourceGo) {
				var existing = this.HitsSources.FirstOrDefault(hs =>
					hs?.Hits != null && hs.Object != null && hs.Object.Id == sourceGo.Id
				);

				if (existing != null) {
					existing.Hits.Damage += e.Damage;
				}
			} else if (source is Player player) {
				var existing = this.HitsSources.FirstOrDefault(hs =>
					hs?.Hits != null && hs.Player != null && hs.Player.Id == player.Id
				);

				if (existing != null) {
					existing.Hits.Damage += e.Damage;
				}
			}
		}
	}
}