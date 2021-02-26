
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

using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;


namespace QQ_Login
{
	public class TCPIPClient
	{
		private bool InstanceFieldsInitialized = false;

			private void InitializeInstanceFields()
			{
				socketDataArrival = socketDataArrivalHandler;
				socketDisconnected = socketDisconnectedHandler;
			}

		public TcpClient _Client;
		public delegate void TcpClientEventHandler<TEventArgs>(object sender, TCPIPClient connection, TEventArgs e);
		public delegate void TcpClientEventHandler(object sender, TCPIPClient connection);
		private delegate void delSocketDataArrival(byte[] data);
		private delSocketDataArrival socketDataArrival;
		private delegate void delSocketDisconnected();
		private delSocketDisconnected socketDisconnected;
		public event TcpClientEventHandler<byte[]> OnDataReceived;
		public Socket MySocketClient = null;
		private string remoteHost;
		private int remotePort;
		private string SockErrorStr = null;
		private ManualResetEvent TimeoutObject = new ManualResetEvent(false);
		private bool IsconnectSuccess = false; //异步连接情况,由异步连接回调函数置位
		private object lockObj_IsConnectSuccess = new object();
		private byte[] buff = new byte[(1024 * 1024 * 4) + 1];
		[StructLayout(LayoutKind.Sequential)]
		internal struct TcpKeepAlive
		{
			internal uint onoff;
			internal uint keepalivetime;
			internal uint keepaliveinterval;
		}
		public void SetKeepAliveEx(Socket socket, uint keepAliveInterval, uint keepAliveTime)
		{
			var keepAlive = new TcpKeepAlive
			{
				onoff = 1,
				keepaliveinterval = keepAliveInterval,
				keepalivetime = keepAliveTime
			};
			int size = Marshal.SizeOf(keepAlive);
			IntPtr keepAlivePtr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(keepAlive, keepAlivePtr, true);
			var buff = new byte[size];
			Marshal.Copy(keepAlivePtr, buff, 0, size);
			Marshal.FreeHGlobal(keepAlivePtr);
			socket.IOControl(IOControlCode.KeepAliveValues, buff, null);
		}
		public TCPIPClient(string strIp, int iPort)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			remoteHost = strIp;
			remotePort = iPort;
			socket_create_connect();
		}
		private bool socket_create_connect()
		{
			IPAddress ipAddress = IPAddress.Parse(remoteHost);
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, remotePort);
			MySocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			MySocketClient.SendTimeout = 1000;
			//SetKeepAliveEx(MySocketClient, 30000, 1)
			_Client = new TcpClient();
			_Client.Client = MySocketClient;
			TimeoutObject.Reset(); //复位timeout事件
			try
			{
				MySocketClient.BeginConnect(remoteEP, connectedCallback, MySocketClient);
			}
			catch (Exception err)
			{
				SockErrorStr = err.ToString();
				Debug.Print("创建sokcet异常:" + err.Message.ToString());
				return false;
			}
			if (TimeoutObject.WaitOne(10000, false)) //直到timeout,或者TimeoutObject.set()
			{
				if (IsconnectSuccess)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				Debug.Print("创建sokcet异常:" + SockErrorStr);
				SockErrorStr = "Time Out";
				return false;
			}
		}
		public bool socket_receive(byte[] readbuff)
		{
			try
			{
				if (MySocketClient == null)
				{
					socket_create_connect();
				}
				else if (!MySocketClient.Connected)
				{
					if (!IsSocketConnected())
					{
						Reconnect();
					}
				}
				int bytesRec = MySocketClient.Receive(readbuff);
				if (bytesRec == 0)
				{
					return true;
				}
				return true;
			}
			catch (SocketException se)
			{
				Debug.Print("接收异常:" + se.Message.ToString());
				throw;
			}
		}
		public bool socket_send(byte[] sendByte)
		{
			if (checkSocketState())
			{
				return SendData(sendByte);
			}
			return false;
		}
		private bool Reconnect()
		{
			//关闭socket
			try
			{
				MySocketClient.Shutdown(SocketShutdown.Both);
				MySocketClient.Disconnect(true);
				MySocketClient.Close();
			}
			catch (Exception ex)
			{
				Debug.Print("已断开,准备重连..." + ex.Message.ToString());
			}
			IsconnectSuccess = false;
			//创建socket
			return socket_create_connect();
		}
		public bool Close()
		{
			//关闭socket
			try
			{
				_Client.Dispose();
				_Client.Close();
				MySocketClient.Shutdown(SocketShutdown.Both);
				MySocketClient.Disconnect(true);
				MySocketClient.Dispose();
				MySocketClient.Close();
				return true;
			}
			catch (Exception ex)
			{
				Debug.Print("Socket已关闭" + ex.Message.ToString());
			}
			return false;
		}
		public bool IsSocketConnected()
		{
			bool connectState = true;
			bool blockingState = MySocketClient.Blocking;
			try
			{
				byte[] tmp = new byte[1];
				MySocketClient.Blocking = false;
				MySocketClient.Send(tmp, 0, 0);
				Console.WriteLine("Connected!");
				connectState = true; //若Send错误会跳去执行catch体,而不会执行其try体里其之后的代码
			}
			catch (SocketException e)
			{
				// 10035 == WSAEWOULDBLOCK
				if (e.NativeErrorCode.Equals(10035))
				{
					Console.WriteLine("依然连接, 但是发送受阻.");
					connectState = true;
				}
				else
				{
					Console.WriteLine("断开,错误代码: error code {0}!", e.NativeErrorCode);
					connectState = false;
				}
			}
			finally
			{
				MySocketClient.Blocking = blockingState;
			}
			//Console.WriteLine("Connected: {0}", client.Connected);
			return connectState;
		}
		public bool IsSocketConnected(Socket s)
		{
			if (s == null)
			{
				return false;
			}
			return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
		}
		private void connectedCallback(IAsyncResult iar)
		{
			lock (lockObj_IsConnectSuccess)
			{
				Socket client = (Socket)iar.AsyncState;
				try
				{
					client.EndConnect(iar);
					IsconnectSuccess = true;

					StartKeepAlive(); //开始KeppAlive检测
				}
				catch (Exception e)
				{
					Debug.Print(e.ToString());
					SockErrorStr = e.ToString();
					IsconnectSuccess = false;
					Reconnect();
				}
				finally
				{
					TimeoutObject.Set();
				}
			}
		}
		private void StartKeepAlive()
		{
			MySocketClient.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), MySocketClient);
		}

		private void OnReceiveCallback(IAsyncResult ar)
		{
			try
			{
				Socket peerSock = (Socket)ar.AsyncState;
				int BytesRead = peerSock.EndReceive(ar);
				Thread.Sleep(500);
				if (BytesRead > 0)
				{
					byte[] tmp = new byte[BytesRead];
					Array.ConstrainedCopy(buff, 0, tmp, 0, BytesRead);
					Debug.Print("收到包长度=" + tmp.Length.ToString());
					if (socketDataArrival != null)
					{
						socketDataArrival(tmp);
					}
				}
				else //对端gracefully关闭一个连接
				{
					if (MySocketClient.Connected) //上次socket的状态
					{
						if (socketDisconnected != null)
						{
							//1-重连
							Reconnect();
							//socketDisconnected()
							//2-退出,不再执行BeginReceive
							return;
						}
					}
				}
				//此处buff似乎要清空--待实现 zq
				MySocketClient.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), MySocketClient);
			}
			catch (Exception ex)
			{
				if (socketDisconnected != null)
				{
					socketDisconnected(); //Keepalive检测网线断开引发的异常在这里捕获
					Debug.Print("TCP连接异常断开");
					Reconnect();
					return;
				}
				else
				{
					Reconnect();
				}
			}
		}

		private void socketDataArrivalHandler(byte[] data)
		{
			if (data.Length > 0)
			{
				UnPack.UnPackReceiveData(data);
			}

		}
		private void socketDisconnectedHandler()
		{
			Debug.Print("socket由于连接中断(软/硬中断)的后续工作处理器");
		}

		public bool checkSocketState()
		{
			try
			{
				if (MySocketClient == null)
				{
					return socket_create_connect();
				}
				else if (IsconnectSuccess)
				{
					return true;
				}
				else //已创建套接字,但未connected
				{
					TimeoutObject.Reset(); //复位timeout事件
					try
					{
						IPAddress ipAddress = IPAddress.Parse(remoteHost);
						IPEndPoint remoteEP = new IPEndPoint(ipAddress, remotePort);
						MySocketClient.BeginConnect(remoteEP, connectedCallback, MySocketClient);
						//SetKeepAliveEx(MySocketClient, 30000, 1)
						//SetKeepAlive(MySocketClient, 30000, 1)

					}
					catch (Exception err)
					{
						SockErrorStr = err.ToString();
						return false;
					}
					if (TimeoutObject.WaitOne(1000, false)) //直到timeout,或者TimeoutObject.set()
					{
						if (IsconnectSuccess)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
					else
					{
						SockErrorStr = "Time Out";
						return false;
					}
				}
			}
			catch (SocketException se)
			{
				SockErrorStr = se.ToString();
				Debug.Print(se.Message.ToString());
				return false;
			}
		}
		public bool SendData(byte[] dataByte)
		{
			bool result = false;
			if (dataByte.Length == 0)
			{
				return result;
			}
			try
			{
				int n = MySocketClient.Send(dataByte);
				if (n < 1)
				{
					result = false;
				}
			}
			catch (Exception ee)
			{
				SockErrorStr = ee.ToString();
				Debug.Print(ee.Message.ToString());
				result = false;
			}
			return result;
		}

		public async Task<byte[]> SendAndWait(byte[] data, int timeout = 5000)
		{
			SendData(data);
			NetworkStream ns = new NetworkStream(MySocketClient);
			//ns.Write(data, 0, data.Length)
			byte[] buffer = new byte[1024];
			Task<Tuple<bool, int>> readWithTimeoutTask = ReadWithTimeoutAsync(ns, MySocketClient, 3000, buffer, 0, 1024);
			Tuple<bool, int> result = await readWithTimeoutTask;
			Console.WriteLine("Read succeeded without timeout? " + result.Item1 + ";  Amount read=" + result.Item2);
			return buffer.Take(result.Item2).ToArray();
		}

		public static async Task<Tuple<bool, int>> ReadWithTimeoutAsync(NetworkStream ns, Socket s, int timeoutMillis, byte[] buffer, int offset, int amountToRead)
		{
			Task<int> readTask = ns.ReadAsync(buffer, offset, amountToRead);
			Task timeoutTask = Task.Delay(timeoutMillis);
			int amountRead = 0;
			bool result = await Task.Factory.ContinueWhenAny<bool>(new Task[] {readTask, timeoutTask}, (completedTask) =>
			{
																													   if (completedTask == timeoutTask)
																													   {
																														   s.Close();
																														   return false;
																													   }
																													   else
																													   {
																														   amountRead = readTask.Result;
																														   return true;
																													   }
			});

			return new Tuple<bool, int>(result, amountRead);
		}
	}


}