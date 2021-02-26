
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QQ_Login
{
	public class TcpClients
	{
		public delegate void TcpClientsEventHandler<TEventArgs>(object sender, TEventArgs e);
		public delegate void TcpClientsEventHandler(object sender);

		private TcpClient client;
		private TcpListener listener;
		private List<Task> tasks = new List<Task>();
		public event TcpClientsEventHandler OnConnect;
		public event TcpClientsEventHandler OnDisconnect;
		public event TcpClientsEventHandler<byte[]> OnDataReceived;
		public int BufferSize = 4096;

		public EndPoint RemoteEndPoint
		{
			get
			{
				if (client == null)
				{
					throw new InvalidOperationException("Client is not connected");
				}
				return client.Client.RemoteEndPoint;
			}
		}

		private TcpClients(TcpClient client)
		{
			this.client = client;
		}

		public TcpClients(string hostname, int port)
		{
			CheckServerUsedAsClient();
			CheckClientAlreadyConnected();
			client = new TcpClient();
			client.Connect(hostname, port);
			client.NoDelay = true;
			CallOnConnect();
			StartReceiveFrom();
		}
		public void Connect(IPAddress address, int port)
		{
			CheckServerUsedAsClient();
			CheckClientAlreadyConnected();
			client = new TcpClient();
			client.Connect(address, port);
			CallOnConnect();
			StartReceiveFrom();
		}
		public void Disconnect()
		{
			CheckServerUsedAsClient();
			CallOnDisconnect();
			client.Close();
		}
		public void Start(int port)
		{
			Start(IPAddress.Any, port);
		}
		public void Start(IPAddress address, int port)
		{
			CheckClientUsedAsServer();
			CheckServerAlreadyStarted();
			listener = new TcpListener(address, port);
			listener.Start();
			StartListen();
		}
		public void Close()
		{
			CheckClientUsedAsServer();
			listener.Stop();
			Task.WhenAll(tasks).Wait();
		}

		public void SendData(byte[] data)
		{
			CheckServerUsedAsClient();
			client.GetStream().Write(data, 0, data.Length);
		}

		private void CallOnDataReceived(byte[] data)
		{
			if (OnDataReceived != null)
				OnDataReceived(this, data);
			UnPack.UnPackReceiveData(data);
		}
		private void CallOnConnect()
		{
			if (OnConnect != null)
				OnConnect(this);
		}
		private void CallOnDisconnect()
		{
			if (OnDisconnect != null)
				OnDisconnect(this);
		}

		private void CheckServerUsedAsClient()
		{
			if (listener != null)
			{
				throw new InvalidOperationException("Cannot use a server connection as a client");
			}
		}
		private void CheckClientUsedAsServer()
		{
			if (client != null)
			{
				throw new InvalidOperationException("Cannot use a client connection as a server");
			}
		}
		private void CheckServerAlreadyStarted()
		{
			if (listener != null)
			{
				throw new InvalidOperationException("Server is already started");
			}
		}
		private void CheckClientAlreadyConnected()
		{
			if (client != null)
			{
				throw new InvalidOperationException("Client is already connected");
			}
		}

		private void StartListen()
		{
			tasks.Add(ListenAsync());
		}
		private async Task ListenAsync()
		{
			do
			{
				TcpClient client = await listener.AcceptTcpClientAsync();
				TcpClients connection = new TcpClients(client);
				StartReceiveFrom();
				if (OnConnect != null)
					OnConnect(this);
			} while (true);
		}

		private void StartReceiveFrom()
		{
			tasks.Add(ReceiveFromAsync());
		}
		private async Task ReceiveFromAsync(int timeout = 5000)
		{
			try
			{
				client.Client.ReceiveTimeout = timeout;
				NetworkStream stream = client.GetStream();
				byte[] buffer = new byte[BufferSize];
				MemoryStream ms = new MemoryStream();
				while (client.Client.Connected)
				{
					int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
					ms.Write(buffer, 0, bytesRead);
					if (!stream.DataAvailable)
					{
						CallOnDataReceived(ms.ToArray());
						ms.Seek(0, SeekOrigin.Begin);
						ms.SetLength(0);
					}
				}
			}
			catch
			{
			}

		}
		public async Task<byte[]> SendAndGetReply(byte[] data, int timeout = 1000)
		{
			byte[] buffer = new byte[4096];
			int bytesRead = 0;
			NetworkStream stream = client.GetStream();
			using (var writeCts = new CancellationTokenSource(timeout))
			{
				await stream.WriteAsync(data, 0, data.Length, writeCts.Token);
				await stream.FlushAsync();
				try
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						do
						{
							Debug.Print(stream.DataAvailable.ToString()); // DataAvailable return false value
							bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); //suspended exception
							memoryStream.Write(buffer, 0, bytesRead);
						} while (bytesRead == 0);
					}
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message.ToString());
				}
			}
			return buffer.Take(bytesRead).ToArray();
		}

		public static async Task<byte[]> getTcpClientHttpDataRequestAsync(string ipAddress, int port, string request)
		{
			string result = string.Empty;
			List<byte> arrayList = new List<byte>();

			using (var tcp = new TcpClient(ipAddress, port))
			{
				using (var stream = tcp.GetStream())
				{
					using (var memory = new MemoryStream())
					{
						tcp.SendTimeout = 500;
						tcp.ReceiveTimeout = 10000;
						tcp.NoDelay = true;
						// Send request headers
						var builder = new StringBuilder();
						builder.AppendLine("GET /request.htm?x01011920000000000001 HTTP/1.1");
						builder.AppendLine("Host: 192.168.1.89");
						builder.AppendLine("Connection: Close");
						builder.AppendLine();
						var header = Encoding.ASCII.GetBytes(builder.ToString());

						Console.WriteLine("======");
						Console.WriteLine(builder.ToString());
						Console.WriteLine("======");

						await stream.WriteAsync(header, 0, header.Length);

						do
						{
						} while (stream.DataAvailable == false);

						Console.WriteLine("Data available");

						bool done = false;
						do
						{
							int nextByte = stream.ReadByte();

							if (nextByte < 0)
							{
								done = true;
							}
							else
							{
								arrayList.Add(Convert.ToByte(nextByte));
							}

						} while (stream.DataAvailable && !done);

						byte[] data = arrayList.ToArray();

						return data;
					}
				}
			}
		}

	}




}