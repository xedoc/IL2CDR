using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class PlayersCollection : Dictionary<int, Player>
    {
        private object lockList = new object();

        /// <summary>
        /// Get Player object by give bot/plane/player ID. 
        /// Set/add works by Player ID only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new Player this[int id]
        {
            get
            {
                Player result = null;
                //Search by player id
                lock (lockList)
                    this.TryGetValue(id, out result);

                //If search failed - search by plane/bot id
                if (result == null)
                {
                    result = this.Values.FirstOrDefault(player =>
                        player != null &&
                        (player.Plane != null && player.Plane.Id == id) ||
                        (player.BotPilot != null && player.BotPilot.Id == id));
                }
                return result;
            }
            set
            {
                if (value == null || value.Id <= 0)
                    return;

                lock (lockList)
                {
                    var existing = this[value.Id];
                    if (existing == null)
                        this.Add(value.Id, value);
                    else
                        existing = value;
                }
            }
        }

        public void PlayerLeave(Guid nickId)
        {
            if (nickId != null || nickId != default(Guid))
            {
                if (this.Any((pair) => pair.Value.NickId.Equals(nickId)))
                {
                    var player = this.FirstOrDefault((pair) => pair.Value.NickId.Equals(nickId)).Value;
                    if (player != null)
                    {
                        player.IsOnline = false;
                        player.IsInAir = false;
                    }

                }
            }
        }
        public void PlayerKilled(int id)
        {
            var player = this[id];
            if (player == null)
                return;

            player.IsInAir = false;
            player.IsKilled = true;
        }
    }


}
