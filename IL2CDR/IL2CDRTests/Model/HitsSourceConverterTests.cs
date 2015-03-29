using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IL2CDR.Model.Tests
{
    [TestClass()]
    public class HitsSourceConverterTests
    {
        [TestMethod()]
        public void HitsSourceConverterTest()
        {
            var player = new Player();
            player.HitsSources = new List<HitsSource>();
            player.HitsSources.Add(new HitsSource() 
            { 
                Hits = new Hits(), 
                Object = new GameObject(1, "test"), 
                Player = new Player() 
            });
            var json = Json.Serialize(player, new HitsSourceConverter());
            Log.WriteInfo( Json.Serialize( player, new HitsSourceConverter()).ToString() );


        }

        [TestMethod()]
        public void WriteJsonTest()
        {

        }

        [TestMethod()]
        public void ReadJsonTest()
        {

        }

        [TestMethod()]
        public void CanConvertTest()
        {

        }
    }
}
