namespace IL2CDR.Model
{
	public class Plane : GameObject
	{
		public Plane(int id, string name) : base(id, name)
		{
			this.Id = id;
			this.Name = name;
		}

		public string Payload { get; set; }
		public int Rockets { get; set; }
		public int Shells { get; set; }
		public int Bullets { get; set; }
		public int Bombs { get; set; }
		public double Fuel { get; set; }
		public string WeaponMods { get; set; }
		public string Skin { get; set; }
	}
}