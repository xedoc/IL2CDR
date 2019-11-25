using System;
using System.Collections.Generic;

namespace IL2CDR.Model
{
	public class GameObject : GameObjectBase
	{
		public GameObject(int id, string name)
		{
			this.Id = id;
			this.Name = name;
			this.Classification = GameObjectClass.Other.ToString("g");
			this.Purpose = string.Empty;
			this.ObjectId = GuidUtility.Create(GuidUtility.IsoOidNamespace, this.Name);

			if (GameInfo.ObjectsClassification.TryGetValue(name, out var objInfo)) {
				this.Classification = objInfo.Classification.ToString("g");
				this.Purpose = objInfo.Purpose;
			}

			this.HitsSources = new List<HitsSource>();
		}

		public int Id { get; set; }
		public Guid ObjectId { get; set; }
		public string Name { get; set; }
		public string Classification { get; set; }
		public string Purpose { get; set; }
		public Country Country { get; set; }
		public int CoalitionIndex { get; set; }
		public bool IsDestroyed { get; set; }
	}
}