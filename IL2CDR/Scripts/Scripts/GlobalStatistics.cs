﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IL2CDR;
using IL2CDR.Model;

namespace IL2CDR.Scripts
{
    public class GlobalStatistics : ActionScriptBase
    {
        //private const string DOMAIN = "localhost";
        //private const string EVENTURL = "http://" + DOMAIN + ":49191/e/?XDEBUG_SESSION_START=55A2686E";
        //private const string PLAYERURL = "http://" + DOMAIN + ":49191/update/players/?XDEBUG_SESSION_START=55A2686E";
        //private const string EVENTURL = "http://" + DOMAIN + ":3992/e/?XDEBUG_SESSION_START=F3623ADB";
        //private const string PLAYERURL = "http://" + DOMAIN + ":3992/update/players/?XDEBUG_SESSION_START=55A2686E";

        private const string DOMAIN = "il2.info";
        private const string EVENTURL = "http://" + DOMAIN + "/e";
        private const string PLAYERURL = "http://" + DOMAIN + "/update/players";
        private const string BACKLOGFILE = "eventback.log";
        private ConcurrentQueue<object> events;
        private Timer sendTimer;
        private const int SENDTIMEOUT = 10000;
        private object lockSend = new object();
        private string lastPacket = String.Empty;
        private string token = String.Empty;
        private bool dictionarySent = false;
        private bool firstPacketSent = false;


        public GlobalStatistics()
        {
            events = new ConcurrentQueue<object>();
            sendTimer = new Timer(SendTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        }
        public override ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    //Enable events processing by this script
                    IsEnabled = true,
                    //Title for script list
                    Title = "Global statistics",
                    //Script description for GUI
                    Description = "Send events to il2.info",
                    //Settings in GUI
                    ConfigFields = new ConfigFieldList()
                    {
                        //name, label, watermark, type, value, isVisible
                        { "token", "Token", "Server authentication token", FieldType.Text, String.Empty, true},
                    },
                };
            }
        }
        public override void OnServerLogStart(Server server)
        {
            var packet = new { Type = 9999, Server = server };
            AddToQueue(packet);
        }

        public string Token {
            get
            {
                if (Config == null)
                    return String.Empty;

                return Config.GetString("token");
            }

        }

        public override void OnApplicationStartup(object data)
        {
            sendTimer.Change(0, SENDTIMEOUT);
        }
        public override void OnApplicationShutdown(object data)
        {
            sendTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void SendDataToServer(object sender)
        {
            var obj = sender as GlobalStatistics;
            if (obj == null)
                return;

            lock( obj.lockSend )
            {

                string result = "FAIL";
                if( String.IsNullOrWhiteSpace( obj.lastPacket ) )
                    obj.lastPacket = GetNextPacket();

                if (String.IsNullOrWhiteSpace(obj.lastPacket))                
                    return;


                using( WebClientBase webClient = new WebClientBase())
                {
                    var data = webClient.GZipBytes(obj.lastPacket);
                    webClient.Timeout = 59000;
                    webClient.ContentType = ContentType.JsonUTF8;
                    webClient.KeepAlive = false;
                    webClient.SetCookie("srvtoken", Token, DOMAIN);
                    result = webClient.UploadCompressed(EVENTURL, obj.lastPacket);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        if (result.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!obj.firstPacketSent)
                            {
                                obj.firstPacketSent = true;
                                Log.WriteInfo("Successfuly connected to il2.info");
                            }
                            obj.lastPacket = String.Empty;
                        }                            
                        else
                            Log.WriteInfo("Send result: {0}", result);

                    }
                    else
                    {
                        Log.WriteInfo("Send result: FAIL (HTTP error)");
                    }
                }

            }
        }

        public string GetNextPacket()
        {
            if (events.IsEmpty)
                return null;

            string content = String.Empty;
            var jsonPackets = new List<object>();

            var limit = 5;
            while (!events.IsEmpty)
            {
                limit--;
                if (limit <= 0)
                    break;

                object obj;
                if (events.TryDequeue(out obj))
                {
                    jsonPackets.Add(obj);
                }
            }

            return Json.Serialize(jsonPackets.ToList());
        }

        public void AddToQueue( object data )
        {
            if (String.IsNullOrWhiteSpace(Token))
                return;

            if (data is MissionLogEventStart ||
                data is MissionLogEventMissionEnd ||
                data is MissionLogEventKill ||
                data is MissionLogEventPlayerAmmo ||
                data is MissionLogEventTakeOff ||
                data is MissionLogEventLanding ||
                data is MissionLogEventPlaneSpawn ||
                data is MissionLogEventGameObjectSpawn ||
                data is MissionLogEventObjectiveCompleted ||
                data is MissionLogEject ||
                data is MissionLogRemoveBot ||
                !(data is MissionLogEventHeader))
            {
                //if( data is MissionLogEventHeader )
                //{
                //    var header = data as MissionLogEventHeader;
                //    Log.WriteInfo("{0} {1} {2}",header.MissionFile, header.EventTime, header.Type);
                //}
                if (data is MissionLogEventKill)
                {
                    var kill = data as MissionLogEventKill;
                    //Record kill only if player participate
                    if (kill.TargetPlayer == null && kill.AttackerPlayer == null)
                        return;

                }
                //var length = Json.Serialize(data).Length;
                //Log.WriteInfo("{0} JSON length: {1}", data.GetType(), length);
                events.Enqueue(data);


            }

            if (events.Count >= 5 || data is MissionLogEventMissionEnd || data is MissionLogEventPlayerAmmo)
                Task.Factory.StartNew( (obj) => SendDataToServer(obj), this);

        }

        private void SendTimerCallback( object sender )
        {
            if (events.Count > 0)
                SendDataToServer(this);
        }
        public override void OnHistory(object data)
        {
            if (data == null )
                return;

            if (!dictionarySent)
            {
                //Send game objects classification info
                AddGameObjectsToQueue();
            }
            AddToQueue(data);
            dictionarySent = true;
        }

        private void AddGameObjectsToQueue()
        {
            var packet = new
            {
                Token = Token,
                Type = 9998,
                ObjectInfo = GameInfo.ObjectsClassification.Select(
                pair => new
                {
                    ObjectId = GuidUtility.Create(GuidUtility.IsoOidNamespace, pair.Key),
                    Name = pair.Key,
                    Class = pair.Value.Classification.ToString("g"),
                    Purpose = pair.Value.Purpose
                }).ToArray()
            };

            AddToQueue(packet);
        }
        public override void OnPlayerListChange(Server server, List<Player> players)
        {
            var packet = new
            {
                ServerId = server.ServerId.ToString(),
                Players = players.Where(p => p != null && p.ClientId != 0 && p.NickId != null ).Select(p => new
                {
                    NickId = p.NickId.ToString(),
                    CountryId = p.Country == null ? 0 : p.Country.Id,
                    Ping = p.Ping,
                    Status = p.Status.ToString(),
                }).ToArray()
            };
            var json = Json.Serialize(packet);
            using (WebClientBase webClient = new WebClientBase())
            {
                webClient.Timeout = 9000;
                webClient.ContentType = ContentType.JsonUTF8;
                webClient.KeepAlive = false;
                webClient.SetCookie("srvtoken", Token, DOMAIN);
                webClient.Upload( PLAYERURL, json );
            }
        }

        


    }
   

}
