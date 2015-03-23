﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace IL2CDR.Model
{
    public class Server : NotifyPropertyChangeBase
    {
        [ScriptIgnore]
        public RconConnection Rcon { get; set; }
        [ScriptIgnore]
        public MissionLogDataService MissionLogService { get; set; }
        [ScriptIgnore]
        public ProcessItem Process { get; set; }

        public string CurrentMissionId { get; set; }
        public int TimeZoneOffset { get; set; }

        public Server(RconConnection rcon, ProcessItem process)
        {
            Name = String.Format(@"PID: {0} {1}\DServer.exe", process.ProcessId, process.ProcessPath);
            Process = process;
            Rcon = rcon;
            MissionLogService = new MissionLogDataService(this);
            ServerId = default(Guid);
            IsConfigSet = false;
            IsRconConnected = false;
            TimeZoneOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
            Initialize();

        }
        public Server(string name, bool isConfigSet, bool isRconConnected )
        {
            Name = name;
            ServerId = GuidUtility.Create(GuidUtility.IsoOidNamespace, name); 
            IsConfigSet = isConfigSet;
            IsRconConnected = isRconConnected;
            Initialize();
        }
        private void Initialize()
        {
            Players = new PlayersCollection();
            GameObjects = new GameObjectsCollection();
            AirFields = new AirFieldCollection();
            CoalitionIndexes = new List<CoalitionIndex>();
        }
        public void Login()
        {
            while( true)
            {
                var result = Rcon.GetConsole();
                var newName = Re.GetSubString(result, @"Server name '(.*?)'");
                if (!String.IsNullOrWhiteSpace(newName))
                {
                    Name = newName;
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }
        public void ResetMission()
        {
            Players.Clear();
            AirFields.Clear();
            GameObjects.Clear();
        }
        public int GetCoalitionIndex( Country country )
        {
            var coalition = CoalitionIndexes.FirstOrDefault(c => c.Country.Id == country.Id);
            if (coalition != null)
                return coalition.Index;
            else
                return -1;
        }

        /// <summary>
        /// The <see cref="CoalitionIndexes" /> property's name.
        /// </summary>
        public const string CoalitionIndexesPropertyName = "CoalitionIndexes";

        private List<CoalitionIndex> _coalitionIndexes;

        /// <summary>
        /// Sets and gets the CoalitionIndexes property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public List<CoalitionIndex> CoalitionIndexes
        {
            get
            {
                return _coalitionIndexes;
            }

            set
            {
                if (_coalitionIndexes == value)
                {
                    return;
                }

                _coalitionIndexes = value;
                RaisePropertyChanged(CoalitionIndexesPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ServerId" /> property's name.
        /// </summary>
        public const string ServerIdPropertyName = "ServerId";

        private Guid _serverId;

        /// <summary>
        /// Sets and gets the ServerId property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Guid ServerId
        {
            get
            {
                return _serverId;
            }

            set
            {
                if (_serverId == value)
                {
                    return;
                }

                _serverId = value;
                RaisePropertyChanged(ServerIdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="GameObjects" /> property's name.
        /// </summary>
        public const string GameObjectsPropertyName = "GameObjects";

        private GameObjectsCollection _gameObjects;

        /// <summary>
        /// Sets and gets the GameObjects property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public GameObjectsCollection GameObjects
        {
            get
            {
                return _gameObjects;
            }

            set
            {
                if (_gameObjects == value)
                {
                    return;
                }

                _gameObjects = value;
                RaisePropertyChanged(GameObjectsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="AirFields" /> property's name.
        /// </summary>
        public const string AirFieldsPropertyName = "AirFields";

        private AirFieldCollection _airFields;

        /// <summary>
        /// Sets and gets the AirFields property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public AirFieldCollection AirFields
        {
            get
            {
                return _airFields;
            }

            set
            {
                if (_airFields == value)
                {
                    return;
                }

                _airFields = value;
                RaisePropertyChanged(AirFieldsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Players" /> property's name.
        /// </summary>
        public const string PlayersPropertyName = "Players";

        private PlayersCollection _players;

        /// <summary>
        /// Sets and gets the Players property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public PlayersCollection Players
        {
            get
            {
                return _players;
            }

            set
            {
                if (_players == value)
                {
                    return;
                }

                _players = value;
                RaisePropertyChanged(PlayersPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = null;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }
        /// <summary>
        /// The <see cref="IsConfigSet" /> property's name.
        /// </summary>
        public const string IsConfigSetPropertyName = "IsConfigSet";

        private bool _isConfigSet = false;

        /// <summary>
        /// Sets and gets the IsConfigSet property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public bool IsConfigSet
        {
            get
            {
                return _isConfigSet;
            }

            set
            {
                if (_isConfigSet == value)
                {
                    return;
                }

                _isConfigSet = value;
                RaisePropertyChanged(IsConfigSetPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsRconConnected" /> property's name.
        /// </summary>
        public const string IsRconConnectedPropertyName = "IsRconConnected";

        private bool _IsRconConnected = false;

        /// <summary>
        /// Sets and gets the IsRconConnected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [ScriptIgnore]
        public bool IsRconConnected
        {
            get
            {
                return _IsRconConnected;
            }

            set
            {
                if (_IsRconConnected == value)
                {
                    return;
                }

                _IsRconConnected = value;
                RaisePropertyChanged(IsRconConnectedPropertyName);
            }
        }

        public void DestroyObject( int id )
        {
            Players.PlayerKilled(id);
            GameObjects.ObjectDestroyed(id);
        }


        public void AddHit(MissionLogEventHit e)
        {
            if (e == null)
                return;

            if( e.TargetObject != null )
                e.TargetObject.AddHit((object)e.AttackerPlayer ?? e.AttackerObject, e);
            else if( e.TargetPlayer != null )
                e.TargetPlayer.AddHit((object)e.AttackerPlayer ?? e.AttackerObject, e);

            if (e.AttackerPlayer != null)
                e.AttackerPlayer.Hits++;
        }

        public void AddDamage( MissionLogEventDamage e)
        {
            if (e == null)
                return;

            if (e.TargetObject != null)
                e.TargetObject.AddDamage((object)e.AttackerPlayer ?? e.AttackerObject, e);
            else if (e.TargetPlayer != null)
                e.TargetPlayer.AddDamage((object)e.AttackerPlayer ?? e.AttackerObject, e);
        }


    }



    
    
}
