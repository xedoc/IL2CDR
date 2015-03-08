using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IL2CDR.Model
{
    public class RconConnection : NotifyPropertyChangeBase, IStopStart
    {
        private TcpClient connection;
        private NetworkStream netStream;
        private bool isStopped = false;
        
        public RconConnection(IL2StartupConfig config)
        {
            Config = config;
        }


        /// <summary>
        /// The <see cref="Config" /> property's name.
        /// </summary>
        public const string ConfigPropertyName = "Config";

        private IL2StartupConfig _config = null;

        /// <summary>
        /// Sets and gets the Config property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IL2StartupConfig Config
        {
            get
            {
                return _config;
            }

            set
            {
                if (_config == value)
                {
                    return;
                }

                _config = value;
                RaisePropertyChanged(ConfigPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsConnected" /> property's name.
        /// </summary>
        public const string IsConnectedPropertyName = "IsConnected";

        private bool _isConnected = false;

        /// <summary>
        /// Sets and gets the IsConnected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }

            set
            {
                if (_isConnected == value)
                {
                    return;
                }

                _isConnected = value;
                RaisePropertyChanged(IsConnectedPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsAuthorized" /> property's name.
        /// </summary>
        public const string IsAuthorizedPropertyName = "IsAuthorized";

        private bool _isAuthorized = false;

        /// <summary>
        /// Sets and gets the IsAuthorized property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAuthorized
        {
            get
            {
                return _isAuthorized;
            }

            set
            {
                if (_isAuthorized == value)
                {
                    return;
                }

                _isAuthorized = value;
                RaisePropertyChanged(IsAuthorizedPropertyName);
            }
        }

        private void Connect()
        {
            if( Config.RconIP != null && 
                Config.RconPort >= 1 || 
                Config.RconPort <= 65535 )
            {
                Task.Factory.StartNew(() => {
                    Util.Try(() => {
                        connection = new TcpClient(Config.RconIP.ToString(), Config.RconPort);
                        netStream = connection.GetStream();
                    });
                    IsConnected = true;
                }).Wait(2000);
                
            }
        }
        private void Disconnect()
        {
            if (connection != null)
            {
                Util.Try(() => netStream.Close());
                Util.Try(() => connection.Close());
            }
        }
        private void Authorize()
        {
            var result = WriteLine(String.Format("auth {0} {1}", Config.Login, Config.Password));
            //Log.WriteInfo(result["STATUS"]);
        }

        private NameValueCollection WriteLine(string line)
        {
            if (isStopped)
                return new NameValueCollection();

            if(netStream == null || !netStream.CanWrite)
                Start();

            if (netStream.CanWrite)
            {                
                Byte[] sendBytes = Encoding.UTF8.GetBytes(String.Concat(line));
                Byte[] length = BitConverter.GetBytes((ushort)(line.Length+1));
                Byte[] zero = { 0 };
                Byte[] packet = length.Concat(sendBytes).Concat(zero).ToArray();                                
                Util.Try(() => netStream.Write(packet, 0, packet.Length),false);
            }
            else
            {
                Disconnect();
                return null;
            }

            if (netStream.CanRead )
            {
                if( netStream.DataAvailable )
                {
                    byte[] bytes = new byte[connection.ReceiveBufferSize];
                    Util.Try(() => netStream.Read(bytes, 0, (int)connection.ReceiveBufferSize), false);
                    UInt16 length = BitConverter.ToUInt16(bytes.Take(2).ToArray(), 0);
                    string response = null;
                    if (length > 2)
                        response = Encoding.UTF8.GetString(bytes.Skip(2).Take((int)length - 1).ToArray());

                    if (!String.IsNullOrWhiteSpace(response))
                        return HttpUtility.ParseQueryString(response);
                    else
                        return new NameValueCollection();

                }
            }
            else
            {
                Disconnect();
                Start();
            }
            return new NameValueCollection();
        }

        public string GetConsole()
        {
            return WriteLine("getconsole")["console"];
        }

        public void Start()
        {
            isStopped = false;
            Connect();
            Authorize();
        }

        public void Stop()
        {
            Disconnect();
            isStopped = true;
        }

        public void Restart()
        {
            
        }
    }
}
