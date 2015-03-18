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
            var gs = new GlobalStatistics();
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.MissionStart]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Hit]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Damage]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Join]));

            var packet = gs.GetNextPacket();
            Debug.Print("{0}\nuncompressed length:{1}, compressed length:{2}",packet,packet.Length, new WebClientBase().GZipBytes(packet).Length);
            
            Assert.IsNotNull(packet);

        }

        [TestMethod()]
        public void AddToQueueTest()
        {

        }

        [TestMethod()]
        public void SendDataToServerTest()
        {
            var gs = new GlobalStatistics();
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.MissionStart]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Hit]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Damage]));
            gs.AddToQueue(Data.GetTestMissionEvent(Data.testLines[EventType.Join]));
            gs.SendDataToServer();
        }
    }
}
