using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class Player
    {
        public Player()
        {
            IsOnline = true;
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
