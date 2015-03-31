using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR.Model;
using IL2CDR.Scripts;
using IL2CDRTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace IL2CDR.Scripts.Tests
{
    [TestClass()]
    public class GlobalStatisticsTests
    {
        [TestMethod()]
        public void GlobalStatisticsTest()
        {
            Log.WriteInfo( Util.SourceLineNumber );    
        }

        [TestMethod()]
        public void OnApplicationStartupTest()
        {

        }

        [TestMethod()]
        public void OnApplicationShutdownTest()
        {

        }

        [TestMethod()]
        public void OnAnyTest()
        {

        }

        [TestMethod()]
        public void GetNextPacketTest()
        {
        }

        [TestMethod()]
        public void AddToQueueTest()
        {

        }

        [TestMethod()]
        public void SendDataToServerTest()
        {
            var gs = new GlobalStatistics();
            gs.Config = new ScriptConfig()
            {
                ConfigFields = new ConfigFieldList()
                    {
                        { "token", "Token", "Server authentication token", FieldType.Text, "0695da663534558c209f052ac2af4112", true},
                    },
            };

            var startPacket = Data.GetTestMissionEvent(Data.testLines[EventType.MissionStart]);
            gs.AddToQueue(new { Type = 9999, Server = ((MissionLogEventHeader)startPacket).Server });
            gs.AddToQueue(startPacket);
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Hit]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Damage]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Join]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Kill]));
            gs.SendDataToServer();
        }
    }
}
