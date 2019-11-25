using System;

namespace IL2CDR.Model
{
	public class Player : GameObjectBase
	{
		public Player()
		{
			this.IsOnline = true;
			this.IsEjected = false;
		}

		public int Id { get; set; }
		public Guid NickId { get; set; }
		public Guid LoginId { get; set; }
		public string NickName { get; set; }
		public Country Country { get; set; }
		public int CoalitionIndex { get; set; }
		public Plane Plane { get; set; }
		public GameObject BotPilot { get; set; }
		public bool IsInAir { get; set; }
		public bool IsOnline { get; set; }
		public PlayerStatus Status { get; set; }
		public int ClientId { get; set; }
		public int Ping { get; set; }
		public bool IsKilled { get; set; }
		public int Hits { get; set; }
		public int Shots { get; set; }
		public Guid SortieId { get; set; }
		public bool IsEjected { get; set; }
	}

	public enum PlayerStatus
	{
		Spectator = 0,
		LobbyReady = 1,
		None = 2,
		DogfightReady = 3,
		CraftSiteReady = 4
	}
}