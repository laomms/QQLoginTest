
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
using System.Text.RegularExpressions;
using System.IO;


namespace QQ_Login
{
	public class TCPIPClient
	{
		private static int fileLength = 0;
		private static string filename = "";
		private delegate void delSocketDataArrival(byte[] data);
		private static delSocketDataArrival socketDataArrival = socketDataArrivalHandler;
		private delegate void delSocketDisconnected();
		private static delSocketDisconnected socketDisconnected = socketDisconnectedHandler;
		public static Socket MySocketClient = null;
		private static string remoteHost;
		private static int remotePort;
		private static string SockErrorStr = null;
		private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);
		private static bool IsconnectSuccess = false; //异步连接情况,由异步连接回调函数置位
		private static object lockObj_IsConnectSuccess = new object();
		private static byte[] buff = new byte[(1024 * 1024 * 4) + 1];
		[StructLayout(LayoutKind.Sequential)]
		internal struct TcpKeepAlive
		{
			internal uint onoff;
			internal uint keepalivetime;
			internal uint keepaliveinterval;
		}
		public static void SetKeepAliveEx(Socket socket, uint keepAliveInterval, uint keepAliveTime)
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
			remoteHost = strIp;
			remotePort = iPort;
			socket_create_connect();
		}
		private static bool socket_create_connect()
		{
			IPAddress ipAddress = IPAddress.Parse(remoteHost);
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, remotePort);
			MySocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			MySocketClient.SendTimeout = 1000;
			SetKeepAliveEx(MySocketClient, 30000, 1);
			TimeoutObject.Reset(); //复位timeout事件
			try
			{
				MySocketClient.BeginConnect(remoteEP, connectedCallback, MySocketClient);
			}
			catch (Exception err)
			{
				SockErrorStr = err.ToString();
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
				Debug.Print(SockErrorStr);
				SockErrorStr = "Time Out";
				return false;
			}
			//			#End Region
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
				//print se.ErrorCode
				throw;
			}
		}
		public static bool socket_send(byte[] sendByte)
		{
			if (checkSocketState())
			{
				return SendData(sendByte);
			}
			return false;
		}
		private static bool Reconnect()
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
			}
			IsconnectSuccess = false;
			//创建socket
			return socket_create_connect();
		}
		private bool IsSocketConnected()
		{
			bool connectState = true;
			bool blockingState = MySocketClient.Blocking;
			try
			{
				byte[] tmp = new byte[1];
				MySocketClient.Blocking = false;
				MySocketClient.Send(tmp, 0, 0);
				//Console.WriteLine("Connected!");
				connectState = true; //若Send错误会跳去执行catch体,而不会执行其try体里其之后的代码
			}
			catch (SocketException e)
			{
				// 10035 == WSAEWOULDBLOCK
				if (e.NativeErrorCode.Equals(10035))
				{
					//Console.WriteLine("Still Connected, but the Send would block");
					connectState = true;
				}
				else
				{
					//Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
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
		public static bool IsSocketConnected(Socket s)
		{
			if (s == null)
			{
				return false;
			}
			return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
		}
		private static void connectedCallback(IAsyncResult iar)
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
		private static void StartKeepAlive()
		{
			MySocketClient.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(OnReceiveCallback), MySocketClient);
		}

		private static void OnReceiveCallback(IAsyncResult ar)
		{
			try
			{
				Socket peerSock = (Socket)ar.AsyncState;
				int BytesRead = peerSock.EndReceive(ar);
				Thread.Sleep(200);
				if (BytesRead > 0)
				{
					byte[] tmp = new byte[BytesRead];
					Array.ConstrainedCopy(buff, 0, tmp, 0, BytesRead);
					//Debug.Print("收到包:" + tmp.ToString + vbNewLine + BitConverter.ToString(tmp).Replace("-", " "))
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
					//socketDisconnected() 'Keepalive检测网线断开引发的异常在这里捕获
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
		private static void socketDataArrivalHandler(byte[] data)
		{
			if (data.Length > 0)
			{
				UnPack.UnPackReceiveData(data);
			}

		}
		private static void socketDisconnectedHandler()
		{
			Debug.Print("socket由于连接中断(软/硬中断)的后续工作处理器");
		}

		public static bool checkSocketState()
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
					//					#Region "异步连接代码"
					TimeoutObject.Reset(); //复位timeout事件
					try
					{
						IPAddress ipAddress = IPAddress.Parse(remoteHost);
						IPEndPoint remoteEP = new IPEndPoint(ipAddress, remotePort);
						MySocketClient.BeginConnect(remoteEP, connectedCallback, MySocketClient);
						SetKeepAliveEx(MySocketClient, 30000, 1);
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
				return false;
			}
		}
		public static bool SendData(byte[] dataByte)
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
				result = false;
			}
			return result;
		}
	}


}