using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IL2CDR.Model
{
	public class Hit
	{
		public Hit(string ammo)
		{
			this.Ammo = ammo;
		}

		public string AmmoId { get; set; }

		private string ammo;

		public string Ammo
		{
			get => this.ammo;
			set
			{
				this.ammo = value;
				this.AmmoId = GuidUtility.Create(GuidUtility.IsoOidNamespace, this.ammo).ToString();
			}
		}

		public int HitCount { get; set; }
	}

	public class Hits : List<Hit>
	{
		public double Damage { get; set; }

		public int HitCount
		{
			get { return this.Count <= 0 ? 0 : this.Sum(hit => hit.HitCount); }
		}

		public Hit this[string ammo]
		{
			get { return this.FirstOrDefault(hit => hit.Ammo == ammo); }
			set
			{
				if (string.IsNullOrWhiteSpace(ammo)) {
					return;
				}

				var existing = this[ammo];
				if (existing == null) {
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
			this.Hits = new Hits();
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
			this._types = types;
		}

		public HitsSourceConverter()
		{
			this._types = new Type[] {typeof(HitsSource)};
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null) {
				return;
			}

			if (!(value is HitsSource hitsSource)) {
				var t = JToken.FromObject(value);
				t.WriteTo(writer);
			} else {
				var o = new JObject();

				var playerId = hitsSource.Player == null ? "" : hitsSource.Player.NickId.ToString().Replace("-", "");
				var objectId = hitsSource.Object == null ? "" : hitsSource.Object.ObjectId.ToString().Replace("-", "");
				o.Add("PlayerId", playerId);
				o.Add("ObjectId", objectId);

				var hits = hitsSource.Hits == null ? null : JToken.FromObject(hitsSource.Hits);
				o.Add("Hits", hits);
				o.WriteTo(writer);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			//will be never called because CanRead is false
			throw new NotImplementedException();
		}

		public override bool CanRead => false;

		public override bool CanConvert(Type objectType)
		{
			return this._types.Any(t => t == objectType);
		}
	}
}