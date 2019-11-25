using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
	public static class Net
	{
		public static void DemandTCPPermission()
		{
			var permitSocket = new SocketPermission(NetworkAccess.Connect, TransportType.Tcp, string.Empty,
				SocketPermission.AllPorts);
			permitSocket.Demand();
		}

		public static void TestTCPPort(string hostName, int portNumber, Action<IPHostEntry, Exception> callback,
			int timeoutMs = 2000)
		{
			IPHostEntry hosts = null;
			try {
				hosts = Dns.GetHostEntry(hostName);
			} catch {
				callback(null, new Exception("Host resolution error " + hostName));
				return;
			}


			var portTestTasks = new Task[hosts.AddressList.Length];

			for (var i = 0; i < portTestTasks.Length; i++) {
				var index = i;
				portTestTasks[i] = Task.Factory.StartNew(() => {
										if (index >= hosts.AddressList.Length) {
											return;
										}

										var host = hosts.AddressList[index];
										hosts.AddressList[index] = null;
										var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
										try {
											sock.Connect(host, portNumber);
											if (sock.Connected) {
												hosts.AddressList[index] = host;
												sock.Close();
											}
										} catch {
											// ignored
										}
				});

				Thread.Sleep(16);
			}

			Task.WaitAll(portTestTasks, timeoutMs);

			hosts.AddressList = hosts.AddressList.Where(entry => entry != null).ToArray();
			callback(hosts, null);
		}
	}
}