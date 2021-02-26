//jce加解密相关函数


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
using System.Text;
using ProtoBuf;

namespace QQ_Login
{
	public class JceStructSDK
	{
		public static byte[] account_RequestQueryQQMobileContactsV3()
		{
			var str = Module1.Device.imei + "|" + Module1.Device.MacId;
			var bytes = new byte[] {0xA, 0xC, 0x1C, 0x2D, 0x0, 0xC, 0x30, 0x1, 0x40, 0x1, 0x5C, 0x66};
			bytes = bytes.Concat(BitConverter.GetBytes(str.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(str)).ToArray();
			bytes = bytes.Concat(new byte[] {0x70, 0x1, 0xB}).ToArray();
			Dictionary<object, object> dic = new Dictionary<object, object>();
			dic.Add("RequestHeader", new byte[] {0xA, 0x0, 0x64, 0x10, 0x1E, 0x2C, 0x3C, 0x46, 0x0, 0x5C, 0x66, 0x0, 0x76, 0x0, 0x86, 0x0, 0x9C, 0xAC, 0xB});
			dic.Add("RequestQueryQQMobileContactsV3", bytes);
			bytes = JceStruct.writeMap(dic, 0);
			bytes = Pack_HeadJce(Module1.QQ.mRequestID, "AccountServer", "AccountServerFunc", bytes);
			bytes = Module1.PackCmdHeader("account.RequestQueryQQMobileContactsV3", bytes);
			Module1.TClient.SendData(Module1.PackAllHeader(bytes));
			return bytes;
		}
		public static byte[] Pack_HeadJce(int req, string cmd1, string cmd2, byte[] bytesIn)
		{
			Dictionary<object, object> dic = new Dictionary<object, object>()
			{
				{"", ""}
			};
			var bytes = JceStruct.writeByte(3, 1);
			bytes = bytes.Concat(JceStruct.writeByte(0, 2)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(0, 3)).ToArray();
			bytes = bytes.Concat(JceStruct.writeInt(req, 4)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString(cmd1, 5)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString(cmd2, 6)).ToArray();
			bytes = bytes.Concat(JceStruct.writeSimpleList(bytesIn, 7)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(0, 8)).ToArray();
			bytes = bytes.Concat(JceStruct.writeEmptyMap(9)).ToArray();
			bytes = bytes.Concat(JceStruct.writeEmptyMap(10)).ToArray();
			//Debug.Print("Pack_HeadJce" + vbNewLine + BitConverter.ToString(bytes).Replace("-", " "))
			return bytes;
		}

#region 取好友列表
		public static byte[] GetFriendList(int start, int amount)
		{
			var bytes = JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_BEGIN, 0);
			bytes = bytes.Concat(JceStruct.writeByte(3, 0)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 1)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(Module1.QQ.LongQQ, 2)).ToArray();
			if (start == 0)
			{
				bytes = bytes.Concat(JceStruct.writeZero(3)).ToArray();
			}
			else
			{
				bytes = bytes.Concat(JceStruct.writeByte((byte)start, 3)).ToArray();
			}
			bytes = bytes.Concat(JceStruct.writeByte((byte)amount, 4)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(5)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 6)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(7)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(8)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(9)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 10)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(16, 11)).ToArray();
			bytes = bytes.Concat(JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_END, 12)).ToArray();

			Dictionary<object, object> dic = new Dictionary<object, object>()
			{
				{"FL", bytes}
			};
			bytes = JceStruct.writeMap(dic, 0);
			bytes = Pack_HeadJce(Module1.QQ.mRequestID, "mqq.IMService.FriendListServiceServantObj", "GetFriendListReq", bytes);
			//Debug.Print("GetFriendList" + vbNewLine + BitConverter.ToString(bytes).Replace("-", " "))

			bytes = Module1.PackCmdHeader("friendlist.getFriendGroupList", bytes);
			bytes = Module1.PackAllHeader(bytes);
			Module1.TClient.SendData(bytes);
			return bytes;
		}
#endregion
#region 取群列表
		public static byte[] GetGroupList()
		{
			var bytes = JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_BEGIN, 0);
			bytes = bytes.Concat(JceStruct.writeLong(Module1.QQ.LongQQ, 0)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(1)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 4)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(5, 5)).ToArray();
			bytes = bytes.Concat(JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_END, 6)).ToArray();

			Dictionary<object, object> dic = new Dictionary<object, object>()
			{
				{"GetTroopListReqV2", bytes}
			};
			bytes = JceStruct.writeMap(dic, 0);

			bytes = Pack_HeadJce(Module1.QQ.mRequestID, "mqq.IMService.FriendListServiceServantObj", "GetTroopListReqV2", bytes);
			//Debug.Print("GetGroupList" + vbNewLine + BitConverter.ToString(bytes).Replace("-", " "))

			bytes = Module1.PackCmdHeader("friendlist.GetTroopListReqV2", bytes);
			bytes = Module1.PackAllHeader(bytes);
			Module1.TClient.SendData(bytes);
			return bytes;
		}
#endregion
#region 取群成员列表
		public static byte[] GetGroupMemberList(long GroupId)
		{
			var bytes = JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_BEGIN, 0);
			bytes = bytes.Concat(JceStruct.writeLong(Module1.QQ.LongQQ, 0)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(GroupId, 1)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(2)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(Module1.Gid2Int(GroupId), 3)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(2, 4)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 5)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(6)).ToArray();
			bytes = bytes.Concat(JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_END, 7)).ToArray();
			Dictionary<object, object> dic = new Dictionary<object, object>()
			{
				{"GTML", bytes}
			};
			bytes = JceStruct.writeMap(dic, 0);
			bytes = Pack_HeadJce(Module1.QQ.mRequestID, "mqq.IMService.FriendListServiceServantObj", "GetTroopMemberListReq", bytes);
			//Debug.Print("GetGroupMemberList" + vbNewLine + BitConverter.ToString(bytes).Replace("-", " "))
			bytes = Module1.PackCmdHeader("friendlist.GetTroopMemberList", bytes);
			bytes = Module1.PackAllHeader(bytes);
			Module1.TClient.SendData(bytes);
			return bytes;
		}
#endregion

#region GetRegSync_Info
		public static byte[] GetRegSync_Info()
		{

			Dictionary<object, object> dic = new Dictionary<object, object>()
			{
				{
					"req_PbPubMsg", new byte[] {0xD, 0x0, 0x0, 0x14, 0x0, 0x0, 0x0, 0x0, 0x8, 0x0, 0x18, 0x0, 0x20, 0x14, 0x28, 0x3, 0x30, 0x0, 0x38, 0x1, 0x48, 0x2, 0x52, 0x0}
				},
				{
					"req_PbOffMsg", new byte[] {0xD, 0x0, 0x0, 0x12, 0x0, 0x0, 0x0, 0x0, 0x8, 0x0, 0x18, 0x0, 0x20, 0x14, 0x28, 0x3, 0x30, 0x0, 0x38, 0x1, 0x48, 0x1}
				},
				{
					"req_OffMsg", new byte[] {0xA, 0x1, 0x61, 0xC2, 0x1A}.Concat(JceStruct.writeLong( Module1.QQ.LongQQ, 0)).Concat(new byte[] {0x10, 0x1, 0x26, 0x0, 0x3C, 0x40, 0x1, 0x5C, 0x60, 0xF, 0x7C, 0x8C, 0x90, 0x4, 0xBC, 0xCC, 0xEC, 0xFC, 0xF, 0xF0, 0x10, 0x1, 0xF0, 0x11, 0x1, 0xFC, 0x13, 0xFC, 0x14, 0xF0, 0x15, 0x14, 0xF0, 0x16, 0x3, 0xFC, 0x17, 0xF0, 0x18, 0x1, 0xFC, 0x19, 0xF0, 0x1A, 0x1, 0xB, 0x5C, 0x6C, 0x7C, 0x8C, 0xE0, 0x1, 0xF0, 0xF, 0x2, 0xF2, 0x10, 0x43, 0x4, 0x5F, 0xDD, 0xFC, 0x11, 0xFC, 0x12, 0xB}).ToArray()
				}
			};
			var bytes = JceStruct.writeMap(dic, 0);

			bytes = Pack_HeadJce(Module1.QQ.mRequestID, "RegPrxySvc", "", bytes);

			bytes = Module1.PackCmdHeader("RegPrxySvc.infoSync", bytes);
			bytes = Module1.PackAllHeader(bytes);

			Debug.Print("GetRegSync_Info" + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));

			Module1.TClient.SendData(bytes);
			return bytes;
		}
#endregion

#region 上线包注册服务
		public static byte[] StatSvcRegister()
		{

			var bytes = JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_BEGIN, 0);
			bytes = bytes.Concat(JceStruct.writeLong(Module1.QQ.LongQQ, 0)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(7, 1)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 2)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 3)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong((long)Module1.OnlineStaus.hide, 4)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 5)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 6)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 7)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 8)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 9)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 10)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(21, 11)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(1, 12)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 13)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 14)).ToArray();
			bytes = bytes.Concat(JceStruct.writeSimpleList(Module1.Device.GUIDBytes, 16)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(2052, 17)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 18)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString(Module1.Device.model, 19)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString(Module1.Device.model, 20)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString(Module1.Device.os_version, 21)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(1, 22)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(14798, 23)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 24)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(0, 26)).ToArray();
			bytes = bytes.Concat(JceStruct.writeInt(Module1.IPToInt32("111.30.181.202"), 27)).ToArray();
			bytes = bytes.Concat(JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_END, 6)).ToArray();

			Dictionary<object, object> dic = new Dictionary<object, object>()
			{
				{"SvcReqRegister", bytes}
			};
			bytes = JceStruct.writeMap(dic, 0);

			bytes = Pack_HeadJce(0, "PushService", "SvcReqRegister", bytes);
			//Debug.Print("StatSvcRegister" + vbNewLine + BitConverter.ToString(bytes).Replace("-", " "))
			bytes = Module1.PackCmdHeader("StatSvc.register", bytes);

			HashTea Hash = new HashTea();
			byte[] EncodeByte = Hash.HashTEA(bytes, Module1.UN_Tlv.T305_SessionKey, 0, true);

			bytes = new byte[] {0x0, 0x0, 0x0, 0xB, 1};
			bytes = bytes.Concat(BitConverter.GetBytes(Module1.QQ.mRequestID + 1).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(new byte[] {0x0}).ToArray();
			bytes = bytes.Concat(Module1.QQ.UTF8).ToArray();
			bytes = bytes.Concat(EncodeByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();

			Module1.TClient.SendData(bytes);
			return bytes;
		}
#endregion

#region 回执_ConfigPushSvc
		public static byte[] ConfigPushSvc()
		{



			return null;
		}
#endregion

#region 回执_ConfigPushSvc
		public static void ReplyConfigPushSvc(byte[] BytesIn, int ssoseq)
		{
			JceStruct.StartDecode(BytesIn);
			if (JceStruct.DicSimpleList.Count > 0)
			{
				var Hex = JceStruct.DicSimpleList[0].ElementAt(0).Value; //取第一层第一个SimpleList
				//Debug.Print("ReplyConfigPushSvc1:" + Hex.Length.ToString + vbNewLine + Hex)
				JceStruct.StartDecode(Module1.HexStrToByteArray(Hex));
				if (JceStruct.DicSimpleList.Count > 0)
				{
					Hex = JceStruct.DicSimpleList[0].ElementAt(0).Value; //取第二层第一个SimpleList
					//Debug.Print("ReplyConfigPushSvc1:" + Hex.Length.ToString + vbNewLine + Hex)
					JceStruct.StartDecode(Module1.HexStrToByteArray(Hex));
					byte P1 = new byte();
					long P2 = 0;
					if (JceStruct.DicByte.Count > 0)
					{
						byte.TryParse(JceStruct.DicByte[0].ElementAt(0).Value, out P1); //取第三层第一个Byte
						if (P1 != 2)
						{
							return;
						}
					}
					if (JceStruct.DicInt.Count > 0) //取第三层第一个Int
					{
						P2 = int.Parse(JceStruct.DicInt[0].ElementAt(0).Value);
					}
					else if (JceStruct.DicLong.Count > 0)
					{
						P2 = long.Parse(JceStruct.DicLong[0].ElementAt(0).Value);
					}
					var bytes = JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_BEGIN, 0);
					bytes = bytes.Concat(JceStruct.writeByte(P1, 1)).ToArray();
					bytes = bytes.Concat(JceStruct.writeLong(P2, 2)).ToArray();
					var dic = new Dictionary<object, object>()
					{
						{"PushResp", bytes}
					};
					bytes = JceStruct.writeMap(dic, 1);
					Debug.Print("ReplyConfigPushSvc:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
					bytes = JceStructSDK.Pack_HeadJce(Module1.QQ.mRequestID, "QQService.ConfigPushSvc.MainServant", "PushResp", bytes);
					bytes = Module1.PackCmdHeader("ConfigPushSvc.PushResp", bytes);
					Module1.TClient.SendData(Module1.PackAllHeader(bytes));
				}
			}
		}
#endregion

#region 取昵称
		public static async Task<string> GetNickAsync(long QQId)
		{
			var bytes = JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_BEGIN, 0);
			bytes = bytes.Concat(JceStruct.writeByte(0, 1)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 2)).ToArray();
			bytes = bytes.Concat(JceStruct.writeList(JceStruct.writeLong(QQId, 0), 3)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 4)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 5)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(0, 6)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(0, 7)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(0, 8)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 9)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(0, 10)).ToArray();
			bytes = bytes.Concat(JceStruct.writeByte(1, 11)).ToArray();
			bytes = bytes.Concat(JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_END, 0)).ToArray();

			var dic = new Dictionary<object, object>()
			{
				{"req", bytes}
			};
			bytes = JceStruct.writeMap(dic, 0);
			bytes = JceStructSDK.Pack_HeadJce(Module1.QQ.mRequestID, "KQQ.ProfileService.ProfileServantObj", "GetSimpleInfo", bytes);
			//Debug.Print("取昵称:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "))
			bytes = Module1.PackCmdHeader("ProfileService.GetSimpleInfo", bytes);
			//TClient.SendData(PackAllHeader(bytes))
			var retBytes = await Module1.TClient.SendAndGetReply(Module1.PackAllHeader(bytes));
			retBytes = UnPack.UnPackReceiveData(retBytes);
			if (retBytes != null)
			{
				if (retBytes.Length > 0)
				{
					Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + "昵称:" + Encoding.UTF8.GetString(retBytes) + "\r\n")));
				}
			}
			return Encoding.UTF8.GetString(retBytes);
		}
#endregion

#region 退群
		public static void ExitGroup(long GroupId)
		{
			var bytes = JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_BEGIN, 0);
			bytes = bytes.Concat(JceStruct.writeByte(2, 0)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(Module1.ThisQQ, 1)).ToArray();
			bytes = bytes.Concat(JceStruct.writeSimpleList(Module1.QQ.user.Concat(Module1.HexStrToByteArray(GroupId.ToString("X"))).ToArray(), 2)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(3)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 4)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(5)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(6)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(7)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(8)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(9)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(10)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 11)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 12)).ToArray();
			bytes = bytes.Concat(JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_END, 0)).ToArray();
			var dic = new Dictionary<object, object>()
			{
				{"GroupMngReq", bytes}
			};
			bytes = JceStruct.writeMap(dic, 0);
			//Debug.Print("退群:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "))
			bytes = JceStructSDK.Pack_HeadJce(Module1.QQ.mRequestID, "KQQ.ProfileService.ProfileServantObj", "GroupMngReq", bytes);
			bytes = Module1.PackCmdHeader("ProfileService.GroupMngReq", bytes);
			Module1.TClient.SendData(Module1.PackAllHeader(bytes));
		}
#endregion

#region 解散群
		public static void DeleteGroup(long GroupId)
		{
			var bytes = JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_BEGIN, 0);
			bytes = bytes.Concat(JceStruct.writeByte(9, 0)).ToArray();
			bytes = bytes.Concat(JceStruct.writeLong(Module1.ThisQQ, 1)).ToArray();
			bytes = bytes.Concat(JceStruct.writeSimpleList(Module1.HexStrToByteArray(GroupId.ToString("X")).Concat(Module1.QQ.user).ToArray(), 2)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(3)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 4)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(5)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(6)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(7)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(8)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(9)).ToArray();
			bytes = bytes.Concat(JceStruct.writeZero(10)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 11)).ToArray();
			bytes = bytes.Concat(JceStruct.writeString("", 12)).ToArray();
			bytes = bytes.Concat(JceStruct.writeFlag(JceStruct.JceType.TYPE_STRUCT_END, 0)).ToArray();
			var dic = new Dictionary<object, object>()
			{
				{"GroupMngReq", bytes}
			};
			bytes = JceStruct.writeMap(dic, 0);
			//Debug.Print("解散群:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "))
			bytes = JceStructSDK.Pack_HeadJce(Module1.QQ.mRequestID, "KQQ.ProfileService.ProfileServantObj", "GroupMngReq", bytes);
			bytes = Module1.PackCmdHeader("ProfileService.GroupMngReq", bytes);
			Module1.TClient.SendData(Module1.PackAllHeader(bytes));
		}
#endregion

#region 取QQ资料信息 回执GetSimpleInfo
		public static byte[] GetSimpleInfo(byte[] BytesIn)
		{
			BytesIn = BytesIn.Skip(4).ToArray();
			JceStruct.StartDecode(BytesIn);
			if (JceStruct.DicSimpleList.Count > 0)
			{
				var hex = JceStruct.DicSimpleList[0].ElementAt(0).Value; //取第一层第一个SimpleList
				JceStruct.StartDecode(Module1.HexStrToByteArray(hex));
				if (JceStruct.DicMAP.Count > 0)
				{
					hex = JceStruct.DicMAP[0].ElementAt(0).Value.Item2; //取第一层第一个SimpleList
					JceStruct.StartDecode(Module1.HexStrToByteArray(hex));
					if (JceStruct.DicList.Count > 0)
					{
						var info = JceStruct.DicList[0].ElementAt(0).Value.Trim().Split('/');
						//Form1.MyInstance.Invoke(New MethodInvoker(Sub() Form1.MyInstance.RichTextBox1.AppendText("【" & Date.Now & "】" + "昵称:" + info(4) + vbNewLine)))
						Module1.NickName = info[4];
						return Encoding.UTF8.GetBytes(info[4]);
					}
				}
			}
			return null;
		}
#endregion

#region 取好友列表 回执getFriendGroupListV2
		public static List<string> GetFriendlist(byte[] BytesIn)
		{
			List<string> list = new List<string>();
			BytesIn = BytesIn.Skip(4).ToArray();
			JceStruct.StartDecode(BytesIn);
			if (JceStruct.DicSimpleList.Count > 0)
			{
				var hex = JceStruct.DicSimpleList[0].ElementAt(0).Value;
				JceStruct.StartDecode(Module1.HexStrToByteArray(hex));
				if (JceStruct.DicMAP.Count > 0)
				{
					hex = JceStruct.DicMAP[0].ElementAt(0).Value.Item2;
					JceStruct.StartDecode(Module1.HexStrToByteArray(hex));
					if (JceStruct.DicList.Count > 0)
					{
						for (var i = 0; i <= JceStruct.DicList.Count - 5; i++)
						{
							var info = JceStruct.DicList[i].ElementAt(0).Value.ToString().Trim().Split('/');
							if (info.Count() < 65)
							{
								break;
							}
							list.Add(info[0] + "(" + info[3] + ")");
						}
					}
				}
			}
			Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + string.Join(",", list) + "\r\n")));
			return list;
		}
#endregion

#region 取群列表 回执GetTroopListReq
		public static List<string> GetGrouplist(byte[] BytesIn)
		{
			List<string> list = new List<string>();
			BytesIn = BytesIn.Skip(4).ToArray();
			JceStruct.StartDecode(BytesIn);
			if (JceStruct.DicSimpleList.Count > 0)
			{
				var hex = JceStruct.DicSimpleList[0].ElementAt(0).Value;
				JceStruct.StartDecode(Module1.HexStrToByteArray(hex));
				if (JceStruct.DicMAP.Count > 0)
				{
					hex = JceStruct.DicMAP[0].ElementAt(0).Value.Item2;
					JceStruct.StartDecode(Module1.HexStrToByteArray(hex));
					if (JceStruct.DicList.Count > 0)
					{
						for (var i = 0; i <= JceStruct.DicList.Count - 81; i++)
						{
							var info = JceStruct.DicList[i].ElementAt(0).Value.Trim().Split('/');
							if (info.Count() < 39)
							{
								break;
							}
							list.Add(info[1] + "(" + info[4] + ")");
						}
					}
				}
			}
			Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + string.Join(",", list) + "\r\n")));
			return list;
		}
#endregion

#region 取群成员列表 回执GetTroopMemberList
		public static List<string> GetGroupMemberlist(byte[] BytesIn)
		{
			List<string> list = new List<string>();
			BytesIn = BytesIn.Skip(4).ToArray();
			JceStruct.StartDecode(BytesIn);
			if (JceStruct.DicSimpleList.Count > 0) //取SimpleList
			{
				var hex = JceStruct.DicSimpleList[0].ElementAt(0).Value;
				JceStruct.StartDecode(Module1.HexStrToByteArray(hex));
				if (JceStruct.DicMAP.Count > 0) //取Map
				{
					hex = JceStruct.DicMAP[0].ElementAt(0).Value.Item2;
					JceStruct.StartDecode(Module1.HexStrToByteArray(hex));
					if (JceStruct.DicList.Count > 0) //取List
					{
						for (var i = 0; i < JceStruct.DicList.Count; i++)
						{
							var info = JceStruct.DicList[i].ElementAt(0).Value.Trim().Split('/');
							if (info.Count() < 42)
							{
								break;
							}
							list.Add(info[0] + "(" + info[4] + ")");
						}
					}
				}
			}
			Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + string.Join(",", list) + "\r\n")));
			return list;
		}
#endregion


	}

}