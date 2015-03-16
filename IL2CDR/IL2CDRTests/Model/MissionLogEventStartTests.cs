using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR.Model;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace IL2CDR.Model.Tests
{
    [TestClass()]
    public class MissionLogEventStartTests
    {
        [TestMethod()]
        public void MissionLogEventStartTest()
        {
            var server = new Server("Test server", Guid.NewGuid(), true, true);
            var serializer = new JavaScriptSerializer();
            var wrapped = MissionLogDataBuilder.GetData(testLines[EventType.MissionStart], DateTime.Now, 1, server);
            var json = serializer.Serialize(wrapped);
            var unwrapped = (Dictionary<string,object>)serializer.DeserializeObject(json);

            Assert.IsTrue(unwrapped.ContainsKey("Server"));

        }
        private static Dictionary<EventType, string> testLines = new Dictionary<EventType, string>()
        {
            {EventType.MissionStart,"T:0 AType:0 GDate:1942.11.19 GTime:14:30:0 MFile:Multiplayer/Ranked/BoS_MP_RD_Stalingrad.msnbin MID: GType:2 CNTRS:0:0,101:1,201:2,202:2 SETTS:1111110111101001000000011 MODS:0 PRESET:1 AQMID:0"},
        };

    }
}
