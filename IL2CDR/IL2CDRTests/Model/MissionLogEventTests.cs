using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR.Model;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using IL2CDRTests;

namespace IL2CDR.Model.Tests
{
    [TestClass()]
    public class MissionLogEventTests
    {

        private bool EventSerializationTest(EventType type )
        {
            var serializer = new JavaScriptSerializer();
            var wrapped = Data.GetTestMissionEvent(Data.testLines[type]);
            var json = serializer.Serialize(wrapped);
            Debug.Print(json);
            var unwrapped = (Dictionary<string, object>)serializer.DeserializeObject(json);

            return unwrapped.ContainsKey("Server");
        }

        [TestMethod()]
        public void MissionLogEventHitTest()
        {
            var result = EventSerializationTest(EventType.Hit);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventStartTest()
        {
            var result = EventSerializationTest(EventType.MissionStart);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventDamageTest()
        {
            var result = EventSerializationTest(EventType.Damage);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventKillTest()
        {
            var result = EventSerializationTest(EventType.Kill);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventPlayerAmmoTest()
        {
            var result = EventSerializationTest(EventType.PlayerAmmo);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventTakeOffTest()
        {
            var result = EventSerializationTest(EventType.TakeOff);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventLandingTest()
        {
            var result = EventSerializationTest(EventType.Landing);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventMissionEndTest()
        {
            var result = EventSerializationTest(EventType.MissionEnd);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventObjectiveCompletedTest()
        {
            var result = EventSerializationTest(EventType.ObjectiveCompleted);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventAirfieldInfoTest()
        {
            var result = EventSerializationTest(EventType.AirfieldInfo);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventPlaneSpawnTest()
        {
            var result = EventSerializationTest(EventType.PlaneSpawn);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventGroupInitInfoTest()
        {
            var result = EventSerializationTest(EventType.GroupInit);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventGameObjectSpawnTest()
        {
            var result = EventSerializationTest(EventType.GameObjectSpawn);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventInfluenceAreaInfoTest()
        {
            var result = EventSerializationTest(EventType.InfluenceAreaInfo);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventInfluenceAreaBoundaryTest()
        {
            var result = EventSerializationTest(EventType.InfluenceAreaBoundary);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventVersionTest()
        {
            var result = EventSerializationTest(EventType.Version);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogBotSpawnTest()
        {
            var result = EventSerializationTest(EventType.BotPilotSpawn);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventPlayerJoinTest()
        {
            var result = EventSerializationTest(EventType.Join);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void MissionLogEventPlayerLeaveTest()
        {
            var result = EventSerializationTest(EventType.Leave);
            Assert.IsTrue(result);
        }

    }
}
