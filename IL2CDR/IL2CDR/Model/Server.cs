using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class Server : NotifyPropertyChangeBase
    {
        private RconConnection rcon;
        public Server(RconConnection rcon)
        {
            this.rcon = rcon;

            Name = "Unknown";
            ServerId = default(Guid);
            IsConfigSet = false;
            IsRconConnected = false;
        }
        public Server(string name, Guid guid, bool isConfigSet, bool isRconConnected )
        {
            Name = name;
            ServerId = guid;
            IsConfigSet = isConfigSet;
            IsRconConnected = isRconConnected;
        }
        ~Server()
        {
            rcon.Stop();
        }
        public void Login()
        {
            while( true)
            {
                var result = rcon.GetConsole();
                Name = Re.GetSubString(result, @"Server name '(.*?)'");
                if (!String.IsNullOrWhiteSpace(Name))
                    break;
                else
                    Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// The <see cref="ServerId" /> property's name.
        /// </summary>
        public const string ServerIdPropertyName = "ServerId";

        private Guid _serverId = default(Guid);

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

    }
}
