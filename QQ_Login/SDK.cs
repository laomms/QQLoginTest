
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
using System.Text;
using ProtoBuf;

namespace QQ_Login
{
	public class SDK
	{

#region 获取公众号列表
		public void GetFollowList()
		{
			var timestamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10));
			GetFollowListStruct FollowList = new GetFollowListStruct
			{
				oldtimestamp = timestamp - 206323410,
				timestamp = timestamp,
				start = 0,
				amount = 20,
				types = 0,
				count = 0
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, FollowList);
				Debug.Print("ReadedMsg" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				var bytes = SDK.PackCmdHeader("PubAccountSvc.get_follow_list", ms.ToArray());
				TCPIPClient.SendData(SDK.PackAllHeader(bytes));
			}
		}
#endregion
#region 提交设备硬件信息
		public void SendDeviceHDInfo()
		{
			var timestamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10));
			DeviceHWInfoStruct DeviceInfo = new DeviceHWInfoStruct
			{
				DeviceHWInfo = new DeviceHWInfos
				{
					DeviceHWId = new DeviceHWIds
					{
						status = 1,
						ver = "5.0",
						appID = DataList.Device.AppId,
						guid = DataList.Device.GUID,
						imei = DataList.Device.imei,
						androidId = DataList.Device.AndroidID,
						imsi = DataList.Device.Imsi,
						macID = DataList.Device.Mac,
						valid = 0
					},
					InfoNo = 2
				},
				DeviceNo = 1
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, DeviceInfo);
				Debug.Print("ReadedMsg" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				var bytes = SDK.PackCmdHeader("OidbSvc.0x6de", ms.ToArray());
				TCPIPClient.SendData(SDK.PackAllHeader(bytes));
			}
		}
#endregion
#region 提交设备软件信息
		public void SendDeviceSWInfo()
		{
			var timestamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10));
			DeviceSWInfoStruct DeviceInfo = new DeviceSWInfoStruct
			{
				deviceNo = 1758,
				deviceStatus = 0,
				DeviceInfo = new DeviceInfos
				{
					deviceType = 0,
					deviceSubId = 31,
					DeviceId = new DeviceIds
					{
						appID = DataList.Device.AppId,
						imei = DataList.Device.imei,
						guid = DataList.Device.GUID,
						androidId = DataList.Device.AndroidID
					}
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, DeviceInfo);
				Debug.Print("ReadedMsg" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				var bytes = SDK.PackCmdHeader("OidbSvc.0x6de", ms.ToArray());
				TCPIPClient.SendData(SDK.PackAllHeader(bytes));
			}
		}
#endregion
		public static byte[] PackCmdHeader(string cmd, byte[] protoBytes)
		{
			var cmdbytes = BitConverter.GetBytes(cmd.Length + 4).Reverse().ToArray().Concat(Encoding.UTF8.GetBytes(cmd)).ToArray().Concat(new byte[] {0, 0, 0, 8}).ToArray().Concat(DataList.QQ.MsgCookies).ToArray().Concat(new byte[] {0, 0, 0, 4}).ToArray();
			cmdbytes = BitConverter.GetBytes(cmdbytes.Length + 4).Reverse().ToArray().Concat(cmdbytes).ToArray(); //拼包识别命令
			var bytes = BitConverter.GetBytes(protoBytes.Length + 4).Reverse().ToArray().Concat(protoBytes).ToArray(); //拼包消息
			return cmdbytes.Concat(bytes).ToArray();
		}
		public static byte[] PackAllHeader(byte[] bodyBytes)
		{
			HashTea Hash = new HashTea();
			byte[] encodeByte = Hash.HashTEA(bodyBytes, DataList.QQ.sessionKey, 0, true); //加密包体
			if (DataList.QQ.mRequestID > 2147483647)
			{
				DataList.QQ.mRequestID = 10000;
			}
			else
			{
				DataList.QQ.mRequestID += 1;
			}
			var headerBytes = new byte[] {0, 0, 0, 0xB, 1}.Concat(BitConverter.GetBytes(DataList.QQ.mRequestID).Reverse().ToArray()).ToArray().Concat(new byte[] {0, 0, 0}).ToArray();
			var qqBytes = BitConverter.GetBytes(Convert.ToInt16(DataList.QQ.UTF8.Length + 4)).Reverse().ToArray().Concat(DataList.QQ.UTF8).ToArray();
			headerBytes = headerBytes.Concat(qqBytes).ToArray();

			var bytes = headerBytes.Concat(encodeByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();
			return bytes;
		}
	}

#region 获取公众号列表结构
	[ProtoContract]
	public class GetFollowListStruct
	{
		[ProtoMember(1)]
		public long oldtimestamp;
		[ProtoMember(2)]
		public long timestamp;
		[ProtoMember(3)]
		public long start;
		[ProtoMember(4)]
		public long amount;
		[ProtoMember(5)]
		public long types;
		[ProtoMember(7)]
		public long count;
	}
#endregion

#region 提交设备软件信息
	[ProtoContract]
	public class DeviceSWInfoStruct
	{
		[ProtoMember(1)]
		public long deviceNo;
		[ProtoMember(2)]
		public long deviceStatus;
		[ProtoMember(4)]
		public DeviceInfos DeviceInfo;
	}
	[ProtoContract]
	public class DeviceInfos
	{
		[ProtoMember(1)]
		public long deviceType;
		[ProtoMember(2)]
		public long deviceSubId;
		[ProtoMember(5)]
		public DeviceIds DeviceId;
	}
	[ProtoContract]
	public class DeviceIds
	{
		[ProtoMember(1)]
		public long appID;
		[ProtoMember(2)]
		public string imei;
		[ProtoMember(3)]
		public string guid;
		[ProtoMember(5)]
		public string androidId;
		[ProtoMember(6)]
		public int valid;
	}
#endregion

#region 提交设备硬件信息
	[ProtoContract]
	public class DeviceHWInfoStruct
	{
		[ProtoMember(1)]
		public DeviceHWInfos DeviceHWInfo;
		[ProtoMember(1)]
		public int DeviceNo;
	}
	[ProtoContract]
	public class DeviceHWInfos
	{
		[ProtoMember(1)]
		public DeviceHWIds DeviceHWId;
		[ProtoMember(2)]
		public int InfoNo;
	}
	[ProtoContract]
	public class DeviceHWIds
	{
		[ProtoMember(1)]
		public long status;
		[ProtoMember(2)]
		public string ver;
		[ProtoMember(3)]
		public long appID;
		[ProtoMember(4)]
		public string guid;
		[ProtoMember(5)]
		public string imei;
		[ProtoMember(6)]
		public string androidId;
		[ProtoMember(7)]
		public int valid;
		[ProtoMember(9)]
		public string imsi;
		[ProtoMember(10)]
		public string macID;
	}
#endregion
}