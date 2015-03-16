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
    public class MissionLogEventTests
    {
        [TestMethod()]
        public void MissionLogEventStartTest()
        {
            var result = EventSerializationTest(EventType.MissionStart);
            Assert.IsTrue(result);
        }

        private bool EventSerializationTest(EventType type )
        {
            var serializer = new JavaScriptSerializer();
            var wrapped = GetTestMissionEvent(testLines[type]);
            var json = serializer.Serialize(wrapped);
            var unwrapped = (Dictionary<string, object>)serializer.DeserializeObject(json);

            return unwrapped.ContainsKey("Server");
        }
        private object GetTestMissionEvent( string text )
        {
            var server = new Server("Test server", Guid.NewGuid(), true, true);
            return MissionLogDataBuilder.GetData(text, DateTime.Now, 1, server);
        }
        private static Dictionary<EventType, string> testLines = new Dictionary<EventType, string>()
        {
            {EventType.MissionStart,"T:0 AType:0 GDate:1942.11.19 GTime:14:30:0 MFile:Multiplayer/Ranked/BoS_MP_RD_Stalingrad.msnbin MID: GType:2 CNTRS:0:0,101:1,201:2,202:2 SETTS:1111110111101001000000011 MODS:0 PRESET:1 AQMID:0"},
            {EventType.Hit,"T:26455 AType:1 AMMO:BULLET_RUS_7-62x54_AP AID:311297 TID:454656"},
            {EventType.Damage,"T:26458 AType:2 DMG:0.030 AID:311297 TID:454656 POS(123603.250,145.485,242323.359)"},
            {EventType.Kill,"T:16459 AType:3 AID:886784 TID:630784 POS(123722.586,132.251,239770.719)"},
            {EventType.PlayerAmmo,"T:31598 AType:4 PLID:311297 PID:394241 BUL:1158 SH:0 BOMB:0 RCT:0 (124204.711,131.871,240163.422)"},
            {EventType.TakeOff,"T:8500 AType:5 PID:311297 POS(112283.617, 46.384, 260226.063)"},
            {EventType.Landing,"T:30670 AType:6 PID:311297 POS(124200.461, 131.916, 240163.281)"},
            {EventType.MissionEnd,"T:38919 AType:7"},
            {EventType.ObjectiveCompleted,"T:37907 AType:8 OBJID:39 POS(273490.000,32.018,95596.297) COAL:1 TYPE:0 RES:1"},
            {EventType.AirfieldInfo,"T:10 AType:9 AID:96256 COUNTRY:201 POS(133798.813, 82.420, 185350.141) IDS()"},
            {EventType.PlaneSpawn,"T:8120 AType:10 PLID:675841 PID:676865 BUL:1620 SH:0 BOMB:0 RCT:0 (133952.922,83.792,185683.047) IDS:00000000-0000-0000-0000-000000000000 LOGIN:00000000-0000-0000-0000-000000000000 NAME:NIK TYPE:Yak-1 ser.69 COUNTRY:101 FORM:0 FIELD:861184 INAIR:2 PARENT:-1 PAYLOAD:0 FUEL:0.380 SKIN: WM:1"},
            {EventType.GroupInit,"T:1 AType:11 GID:927744 IDS:640000,646144,655360,664576,670720,676864,683008 LID:640000"},
            {EventType.GameObjectSpawn,"T:16459 AType:12 ID:886784 TYPE:ZiS-6 BM-13 COUNTRY:101 NAME:Vehicle PID:-1"},
            {EventType.InfluenceAreaInfo,"T:92117 AType:13 AID:16384 COUNTRY:201 ENABLED:1 BC(0,1,0)"},
            {EventType.InfluenceAreaBoundary,"T:1 AType:14 AID:16384 BP((243.0,98.8,183.0),(230365.0,98.8,133.0),(230106.0,98.8,75641.0),(190900.0,98.8,73379.0),(163347.0,98.8,151570.0),(181083.0,98.8,166110.0),(183280.0,98.8,188216.0),(157123.0,98.8,221512.0),(149370.0,98.8,259567.0),(131925.0,98.8,253301.0),(121149.0,98.8,241594.0),(117297.0,98.8,226568.0),(124000.0,98.8,209280.0),(110644.0,98.8,197291.0),(83500.0,98.8,211773.0),(54934.0,98.8,215035.0),(29687.0,98.8,227373.0),(820.0,98.8,231540.0))"},
            {EventType.Version,"T:0 AType:15 VER:17"},
            {EventType.BotPilotSpawn,"T:28250 AType:16 BOTID:182273 POS(113655.180,129.202,243216.594)"},
            {EventType.Join,"T:28250 AType:20 USERID:00000000-0000-0000-0000-000000000000 USERNICKID:00000000-0000-0000-0000-000000000000"},
            {EventType.Leave,"T:28250 AType:21 USERID:00000000-0000-0000-0000-000000000000 USERNICKID:00000000-0000-0000-0000-000000000000"},
        };

        [TestMethod()]
        public void MissionLogEventHitTest()
        {
            var result = EventSerializationTest(EventType.Hit);
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
