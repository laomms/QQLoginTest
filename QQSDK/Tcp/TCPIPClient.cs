
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


namespace QQSDK
{
	public class TCPIPClient : IDisposable
	{
		#region 自定义变量
		private  string hostname;
		private  int hostport;
		private  NetworkStream SocketStream;
		private  TcpClient Client;
		public delegate void OnDataArrivalEventHandler(byte[] msg);
		public  event OnDataArrivalEventHandler OnDataArrival;
		public bool IsConnected;

		#endregion

		#region 初始化远程服务器端口
		public TCPIPClient(string SerIP, int Port)
		{
			hostname = SerIP;
			hostport = Port;
			Client = new TcpClient();
			try
			{
				Client.Connect(IPAddress.Parse(SerIP), Port);
				SocketStream = Client.GetStream();
				IsConnected = true;
				//RaiseEvent OnText("服务器连接成功")
				Debug.Print("服务器连接成功");
			}
			catch (Exception ex)
			{
				//RaiseEvent OnText("服务器连接失败，请检查")
				Debug.Print("服务器连接失败，请检查");
				API.reLogin();
			}

		}
		public void DisConnect()
		{
			IsConnected = false;
			if (Client != null)
			{
				Client.Close();
			}
			if (SocketStream != null)
			{
				SocketStream.Close();
			}
		}

		public byte[] GetMessage()
		{
			byte[] buffer = new byte[4096];
			int bytesRead = 0;
			if (SocketStream == null)
			{
				IsConnected = false;
				//RaiseEvent OnText("与服务器的连接已断开")
				Debug.Print("与服务器的连接已断开");
				if (Client!=null)
                {
					Client.Dispose();
					Client.Close();
					Client = null;
				}
				Thread.Sleep(100);
				Client = new TcpClient();
				API.reLogin();
				return null;
			}
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					do
					{
						if (SocketStream == null) break; 
						bytesRead = SocketStream.Read(buffer, 0, buffer.Length);
						memoryStream.Write(buffer, 0, bytesRead);
					} while (bytesRead == 0);
				}
				if (OnDataArrival != null)
					OnDataArrival(buffer.Take(bytesRead).ToArray());
				return buffer.Take(bytesRead).ToArray();
			}
			catch (IOException ex)
			{
				SocketStream = null;
				System.Threading.Thread.Sleep(50);
				IsConnected = false;
				//RaiseEvent OnText(ex.Message)
			}
			return null;
		}
		public void SendData(byte[] data)
		{
			if (SocketStream != null)
			{
				IAsyncResult IR = SocketStream.BeginWrite(data, 0, data.Length, null, null);
				IR.AsyncWaitHandle.WaitOne();
				SocketStream.EndWrite(IR);
				SocketStream.Flush();
			}
			else
			{
				IsConnected = false;
			}
			byte[] resBytes = new byte[0];
			var CThread = new Thread(() =>
			{
				API.CReciveMsg();
			});
			CThread.Start();
		}
		#endregion

		#region IDisposable Support
		private bool disposedValue; // 检测冗余的调用
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					// TODO: 释放托管状态(托管对象)。
				}
				if (Client != null)
				{
					Client.Close();
				}
				if (SocketStream != null)
				{
					SocketStream.Close();
				}
				Client = null;
				SocketStream = null;
				// TODO: 释放非托管资源(非托管对象)并重写下面的 Finalize()。
				// TODO: 将大型字段设置为 null。
			}
			this.disposedValue = true;
		}

		~TCPIPClient()
		{
			Dispose(false);
			//INSTANT C# NOTE: The base class Finalize method is automatically called from the destructor:
			//base.Finalize();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion	
	}
}