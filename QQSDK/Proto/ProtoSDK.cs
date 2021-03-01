//https://github.com/laomms/QQLogin
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
//Proto加解密相关函数

namespace QQSDK
{
	public class ProtoSDK
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
				var bytes = API.PackCmdHeader("PubAccountSvc.get_follow_list", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
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
						appID = API.Device.AppId,
						guid = API.Device.GUIDBytes,
						imei = API.Device.imei,
						androidId = API.Device.AndroidID,
						imsi = API.Device.Imsi,
						macID = API.Device.MacId,
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
				var bytes = API.PackCmdHeader("OidbSvc.0x6de", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
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
						appID = API.Device.AppId,
						imei = API.Device.imei,
						guid = API.Device.GUIDBytes,
						androidId = API.Device.AndroidID
					}
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, DeviceInfo);
				Debug.Print("ReadedMsg" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				var bytes = API.PackCmdHeader("OidbSvc.0x6de", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}
		}
#endregion

#region 取群管理列表
		public static byte[] GetGroupAdminList(long GroupId)
		{
			byte[] bytes = null;
			GetGroupAdminStruct pbData = new GetGroupAdminStruct
			{
				field1 = 2201,
				field3 = 0,
				GroupAdminInfo = new GroupAdminInfos
				{
					groupId = GroupId,
					field2 = 0,
					field3= 2,
					field5 = new byte[] {0x2A, 2, 8, 0}
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, pbData);
				Debug.Print("取群管理列表:" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", " "));
				bytes = ms.ToArray();
			}
			bytes = API.PackCmdHeader("OidbSvc.0x899_0", bytes);
			bytes = API.PackAllHeader(bytes);
			API.TClient.SendData(bytes);
			return bytes;
		}
#endregion

#region 取群管理列表 回执OidbSvc.0x899_0
		public static List<long> GetGroupAdminlist(byte[] BytesIn)
		{
			using (MemoryStream ms = new MemoryStream(BytesIn.Skip(4).ToArray()))
			{
				var result = Serializer.Deserialize<GetGroupAdminStruct>(ms);
				var list = result.GroupAdminInfo.AdminList.AdminId;
				return list;
			}
		}
#endregion

#region 禁言包
		public static void ShutUp(long GroupId, long HisId, long times)
		{
			if (times < 0 || times > 2591940)
			{
				times = 2591940;
			}
			var bytes = Pack.GetBytesFromLong(GroupId);
			bytes = bytes.Concat(new byte[] {0x20, 0, 1}).ToArray();
			bytes = bytes.Concat(Pack.GetBytesFromLong(HisId)).ToArray();
			bytes = bytes.Concat(Pack.GetBytesFromLong(times)).ToArray();
			ShutUpStruct pbData = new ShutUpStruct
			{
				field1 = 1392,
				field2 = 8,
				field3 = 0,
				info = bytes
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, pbData);
				bytes = ms.ToArray();
			}
			bytes = API.PackCmdHeader("OidbSvc.0x570_8", bytes);
			bytes = API.PackAllHeader(bytes);
			API.TClient.SendData(bytes);
		}
#endregion

#region 全员禁言
		public static void ShutAll(long GroupId, API.Mute Op)
		{
			if (Op == API.Mute.Close)
			{
				Op = (API.Mute)268435455;
			}
			else
			{
				Op = 0;
			}
			byte[] bytes = null;
			ShutAllStruct pbData = new ShutAllStruct
			{
				field1 = 2202,
				field2 = 0,
				field3 = 0,
				info = new ShutAllInfos
				{
					GroupId = GroupId,
					status = new ShutAllFlags {op =1}
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, pbData);
				bytes = ms.ToArray();
			}
			bytes = API.PackCmdHeader("OidbSvc.0x89a_0", bytes);
			bytes = API.PackAllHeader(bytes);
			API.TClient.SendData(bytes);
		}
#endregion

#region 删除成员
		public static void RemoveMember(long GroupId, long HisId, bool IsRefuseNext)
		{
			byte[] bytes = null;
			RemoveMemberStruct pbData = new RemoveMemberStruct
			{
				field1 = 2208,
				field2 = 0,
				field3 = 0,
				info = new RemoveMemberInfos
				{
					GroupId = GroupId,
					MemberInfo = new MemberInfos
					{
						HisId = HisId,
						field1 = 5,
						field3 = IsRefuseNext
					}
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, pbData);
				bytes = ms.ToArray();
			}
			bytes = API.PackCmdHeader("OidbSvc.0x8a0_0", bytes);
			bytes = API.PackAllHeader(bytes);
			API.TClient.SendData(bytes);
		}
#endregion
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
		public byte[] guid;
		[ProtoMember(5)]
		public byte[] androidId;
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
		public byte[] guid;
		[ProtoMember(5)]
		public string imei;
		[ProtoMember(6)]
		public byte[] androidId;
		[ProtoMember(7)]
		public int valid;
		[ProtoMember(9)]
		public string imsi;
		[ProtoMember(10)]
		public string macID;
	}
#endregion

#region 取群管理结构
	[ProtoContract]
	public class GetGroupAdminStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int field1;
		[ProtoMember(3, IsRequired=true)]
		public int field3;
		[ProtoMember(4, IsRequired=true)]
		public GroupAdminInfos GroupAdminInfo;
	}
	[ProtoContract]
	public class GroupAdminInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public long groupId;
		[ProtoMember(2, IsRequired=true)]
		public int field2;
		[ProtoMember(3, IsRequired=true)]
		public int field3;
		[ProtoMember(4)]
		public AdminIdInfos AdminList;
		[ProtoMember(5, IsRequired=true)]
		public byte[] field5;
	}
	[ProtoContract]
	public class AdminIdInfos
	{
		[ProtoMember(1)]
		public List<long> AdminId;
	}
#endregion

#region 禁言结构
	[ProtoContract]
	public class ShutUpStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int field1;
		[ProtoMember(2, IsRequired=true)]
		public int field2;
		[ProtoMember(3, IsRequired=true)]
		public int field3;
		[ProtoMember(4, IsRequired=true)]
		public byte[] info;
	}

#endregion

#region 全员禁言结构
	[ProtoContract]
	public class ShutAllStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int field1;
		[ProtoMember(2, IsRequired=true)]
		public int field2;
		[ProtoMember(3, IsRequired=true)]
		public int field3;
		[ProtoMember(4, IsRequired=true)]
		public ShutAllInfos info;
	}
	[ProtoContract]
	public class ShutAllInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public long GroupId;
		[ProtoMember(2, IsRequired=true)]
		public ShutAllFlags status;
	}
	[ProtoContract]
	public class ShutAllFlags
	{
		[ProtoMember(17, IsRequired=true)]
		public int op;
	}
#endregion

#region 踢人结构
	[ProtoContract]
	public class RemoveMemberStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int field1;
		[ProtoMember(2, IsRequired=true)]
		public int field2;
		[ProtoMember(3, IsRequired=true)]
		public int field3;
		[ProtoMember(4, IsRequired=true)]
		public RemoveMemberInfos info;
	}
	[ProtoContract]
	public class RemoveMemberInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public long GroupId;
		[ProtoMember(2, IsRequired=true)]
		public MemberInfos MemberInfo;
	}
	[ProtoContract]
	public class MemberInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public int field1;
		[ProtoMember(2, IsRequired=true)]
		public long HisId;
		[ProtoMember(3, IsRequired=true)]
		public bool field3;
	}
#endregion

}