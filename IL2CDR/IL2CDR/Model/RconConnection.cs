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
        private IL2StartupConfig config;
        public RconConnection(IL2StartupConfig config)
        {
            this.config = config;
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
            if( config.RconIP != default(IPAddress) && 
                config.RconPort >= 1 || 
                config.RconPort <= 65535 )
            {
                Task.Factory.StartNew(() => {
                    Util.Try(() => {
                        connection = new TcpClient(config.RconIP.ToString(), config.RconPort);
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
            var result = WriteLine(String.Format("auth {0} {1}", config.Login, config.Password));
            Log.WriteInfo(result["STATUS"]);
        }

        private NameValueCollection WriteLine(string line)
        {
            if(netStream == null || !netStream.CanWrite)
                Start();

            if (netStream.CanWrite)
            {
                Byte[] sendBytes = Encoding.UTF8.GetBytes(line);
                netStream.Write(sendBytes, 0, sendBytes.Length);
            }
            else
            {
                Disconnect();
                return null;
            }

            if (netStream.CanRead)
            {
                byte[] bytes = new byte[connection.ReceiveBufferSize];
                netStream.Read(bytes, 0, (int)connection.ReceiveBufferSize);
                var response = Encoding.UTF8.GetString(bytes);
                if (!String.IsNullOrWhiteSpace(response))
                    return HttpUtility.ParseQueryString(response);
                else
                    return new NameValueCollection();

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
            Connect();
            Authorize();
        }

        public void Stop()
        {
            Disconnect();
        }

        public void Restart()
        {
            
        }
    }
}
