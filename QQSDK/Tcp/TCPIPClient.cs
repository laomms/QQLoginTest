
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
using SimpleTcp;

namespace QQSDK
{
	public class TCPIPClient
	{
		public SimpleTcpClient _Client;
		public TCPIPClient(string SerIP, int Port)
		{
			_Client = new SimpleTcpClient(SerIP, Port);
			_Client.Events.Connected += Connected;
			_Client.Events.Disconnected += Disconnected;
			_Client.Events.DataReceived += DataReceived;
			//_Client.Keepalive.EnableTcpKeepAlives = true;
			//_Client.Settings.MutuallyAuthenticate = false;
			//_Client.Settings.AcceptInvalidCertificates = true;
			_Client.Logger = Logger;
			_Client.Connect();

		}
		public bool IsConnected()
		{
			return _Client.IsConnected;
		}

		public void Connected(object sender, EventArgs e)
		{
			Console.WriteLine("*** Server connected");
		}

		public void Disconnected(object sender, EventArgs e)
		{
			Console.WriteLine("*** Server disconnected");
			API.reLogin(); 
		}

		public void DataReceived(object sender, SimpleTcp.DataReceivedEventArgs e)
		{
			UnPack.UnPackReceiveData(e.Data);  
		}
		public void SendData(byte[] data)
		{
			if (data != null) _Client.Send(data);
		}

		public void SendAsync(byte[] data)
		{
			if (data!=null) _Client.SendAsync(data).Wait();
		}
		public void Logger(string msg)
		{
			Console.WriteLine(msg);
		}
	}
}