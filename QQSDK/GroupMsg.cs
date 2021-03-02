
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
using System.Text.RegularExpressions;
using ProtoBuf;

namespace QQSDK
{
	public class GroupMsg
	{

#region 解析群消息
		public static API.GroupWithdrawInfo ParsingGroupMsg(byte[] byteIn)
		{

			string TextMsg = "";
			string FaceMsg = "";
			string HotPicMsg = "";
			string PicMsg = "";
			string AudioMsg = "";
			string VideoMsg = "";
			string ReplyMsg = "";
			var byteLen = BitConverter.ToInt32(byteIn.Take(4).ToArray().Reverse().ToArray(), 0);
			byteIn = byteIn.Skip(4).ToArray();
			using (MemoryStream ms = new MemoryStream(byteIn))
			{
				try
				{
					var result = Serializer.Deserialize<GroupMsgStuct>(ms);
					long QQId = result.GroupInfo.GroupQQInfo.thisQQId;
					long QQFromId = result.GroupInfo.GroupQQInfo.QQFromId;					
					long GroupId = result.GroupInfo.GroupQQInfo.GroupInfo.GroupId;
					string GroupName = result.GroupInfo.GroupQQInfo.GroupInfo.GroupName;
					API.GroupWithdraw.MsgReqId = result.GroupInfo.GroupQQInfo.fromReq;
					API.GroupWithdraw.MsgRandomId = result.GroupInfo.GroupMessageInfo.GroupMsgInfo.FontInfo.fromRandom;
					if (QQId == QQFromId)
						return API.GroupWithdraw;
					var msgByte = result.GroupInfo.GroupMessageInfo.GroupMsgInfo.GroupMsgContent;
					Debug.Print("群消息内容:" + "\r\n" + BitConverter.ToString(msgByte).Replace("-", " "));
					using (MemoryStream mStream = new MemoryStream(msgByte))
					{
						var MsgResult = Serializer.Deserialize<GroupMessageStruct>(mStream);
						API.NickName = MsgResult.SendNick.NickName;
						if (MsgResult.GroupFileMessage != null) //有文件上传的消息
						{
							var FileHash = MsgResult.GroupFileMessage.FileHash;
						}
						if (MsgResult.GroupReplyMessage != null) //是带回复的消息
						{
							string replyContent = "";
							for (var i = 0; i < MsgResult.GroupReplyMessage.OldMsgContent.Count; i++)
							{
								replyContent = replyContent + MsgResult.GroupReplyMessage.OldMsgContent[i].content.Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
							}
							ReplyMsg = " [Reply=@" + MsgResult.GroupReplyMessage.thisQQId.ToString() + "内容:" + replyContent + "]";
						}
						if (MsgResult.GroupTextMsgContent != null) //有文本或小表情或热图内容
						{
							for (var i = 0; i < MsgResult.GroupTextMsgContent.Count; i++)
							{
								TextMsg = TextMsg + MsgResult.GroupTextMsgContent[i].content;
							}
						}
						if (MsgResult.BigFace != null) //有大表情内容
						{
							for (var i = 0; i <= MsgResult.BigFace.Count; i++)
							{
								var FaceId = MsgResult.BigFace[i].FaceId;
								FaceMsg = FaceMsg + " [BigFaceId=" + FaceId.ToString() + "]";
							}
						}
						if (MsgResult.GroupPicMessageContent != null) //有图片内容
						{
							for (var i = 0; i < MsgResult.GroupPicMessageContent.Count; i++)
							{
								PicMsg = PicMsg + "https://gchat.qpic.cn" + MsgResult.GroupPicMessageContent[i].PicAddr1.Substring(0, MsgResult.GroupPicMessageContent[i].PicAddr1.LastIndexOf("/")) + "/0 ";
							}
							PicMsg = "[pic,link=" + PicMsg + "]";
						}
						if (result.GroupInfo.GroupMessageInfo.GroupMsgInfo.GroupAudioMsgContent != null) //有语音内容
						{
							using (MemoryStream msStream = new MemoryStream(result.GroupInfo.GroupMessageInfo.GroupMsgInfo.GroupAudioMsgContent))
							{
								var Ret = Serializer.Deserialize<AudioMessageStruct>(msStream);
								AudioMsg = "[Audio,link=" + "http://grouptalk.c2c.qq.com" + Ret.AudioUrl;
							}
						}
						if (MsgResult.GroupVideoMessageContent != null) //有视频内容
						{
							for (var i = 0; i < MsgResult.GroupVideoMessageContent.Count; i++)
							{
								VideoMsg = VideoMsg + "[video,guid=" + MsgResult.GroupVideoMessageContent[i].VideoGuid + "]";
							}
						}
					}
					SDK.GetResult(GroupId.ToString(), QQFromId.ToString(), "[" + GroupName + "]" + ReplyMsg + FaceMsg + HotPicMsg + TextMsg + PicMsg + AudioMsg + VideoMsg);
					var MsgSeq = result.GroupInfo.GroupQQInfo.fromReq;
					MakeReadedGroupMsg(GroupId, MsgSeq);
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message.ToString());
				}
			}
			return API.GroupWithdraw;
		}
#endregion

#region 置群消息已读
		public static bool MakeReadedGroupMsg(long GroupId, long MsgId)
		{
			MakeReadedGroupMessage ReadedMsg = new MakeReadedGroupMessage
			{
				ReadedGroupMsg = new ReadedGroupMsg
				{
					GroupId = GroupId,
					MsgId = MsgId
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, ReadedMsg);
				var bytes = API.PackCmdHeader("PbMessageSvc.PbMsgReadedReport", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}
			return true;
		}
#endregion

#region 发送群消息
		public static API.GroupWithdrawInfo SendGroupMsg(long thisQQ, long GroupId, byte[] MsgBytes, API.MsgType MsgTypes , long sendQQ=0)
		{
			byte[] bytes = null;

			var timestamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10));
			List<SendMessages> MsgList = new List<SendMessages>();
			if (MsgTypes == API.MsgType.TextMsg) //文字消息
			{
				SendMessages MsgListStruct = new SendMessages
				{
					SendContent = new SendContents {Content = Encoding.UTF8.GetString(MsgBytes)}
				};
				if (sendQQ !=0)
                {
					SendMessages MsgListAtStruct = new SendMessages
					{
						SendContent = new SendContents
						{
							Content = "@" + sendQQ.ToString(),
							AtHash = new byte[] { 0x0, 0x1, 0x0, 0x0, 0x0, 0x4, 0x0, 0x2, 0x3B, 0xD7, 0x86, 0x0, 0x0 }
						}
					};
					MsgList.Add(MsgListAtStruct);
				}
				
				MsgList.Add(MsgListStruct);
			}
			else if (MsgTypes == API.MsgType.XmlMsg)
			{
				SendMessages MsgListStruct = new SendMessages
				{
					GroupXmlMsg = new GroupXmlContents
					{
						Content = MsgBytes,
						flag = 1
					}
				};

				SendMessages XmlFlagStruct = new SendMessages
				{
					GroupXmlFlag = new GroupXmlFlags {BuddleID = (new Random()).Next(1000001, 1000037)}
				};
				MsgList.Add(MsgListStruct);
				MsgList.Add(XmlFlagStruct);
			}
			else if (MsgTypes == API.MsgType.PicMsg) //图片消息
			{
				MsgBytes = MsgBytes.Skip(4).ToArray();
				using (var ms = new MemoryStream(MsgBytes))
				{
					var result = Serializer.Deserialize<GroupPicGuidStruct>(ms);
					if (result.GroupPicGuidInfo.uKey != null) //'服务器没有该图片的hash
					{
						var uKey = result.GroupPicGuidInfo.uKey;
						var Ip = API.Int32ToIP(result.GroupPicGuidInfo.Ip[0]);
						var Port = result.GroupPicGuidInfo.Port[0];
						UploadGroupPicByTCP(sendQQ,GroupId, uKey, Ip, Port);
					}
					SendMessages msg = new SendMessages
					{
						SendGroupPicInfo = new SendGroupPicInfos
						{
							PicName = BitConverter.ToString(API.FileHash).Replace("-", "") + ".jpg",
							PicId1 = 3013326518,
							PicId2 = 1883293792,
							picIconWidth = 80,
							picIconHeigh = 66,
							picMD5 = "VdcrgQM3T3AaJPtM",
							picAmount = 1,
							picHash = API.FileHash,
							picType = 4,
							picPix = 1000,
							picWidth = 100,
							picHeigh = 104,
							picSize = 2500,
							Field26 = 0,
							Field29 = 0,
							Field30 = 0
						}
					};
					MsgList.Add(msg);

				}

			}

			SendGroupMsgStuct SendGroupMessage = new SendGroupMsgStuct
			{
				GroupInformation = new GroupInformations
				{
					GroupFrom = new GroupFroms {GroupId = GroupId}
				},
				GroupMsgId = new byte[] {8, 1, 0x10, 0, 0x18, 0},
				SendGroupMsgInfo = new SendGroupMsgInfos
				{
					SendGroupMsg = new SendGroupMsgs {SendMessage = MsgList}
				},
				MsgReqId = API.QQ.mRequestID,
				MsgRandomId = (new Random()).Next(1, 1879048191) + 268435457,
				Groupcount = 0
			};
			API.GroupWithdraw.MsgReqId = API.QQ.mRequestID;
			API.GroupWithdraw.MsgRandomId = SendGroupMessage.MsgRandomId;
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, SendGroupMessage);
				Debug.Print("发送群消息:" + ms.ToArray().Length.ToString() + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", " "));
				var SendBytes = API.PackCmdHeader("MessageSvc.PbSendMsg", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(SendBytes));
			}
			return API.GroupWithdraw;
		}

		#endregion

#region 上传图片
		public static void UploadGroupPicByTCP(long sendQQ, long GroupId, byte[] uKey, string Ip, int Port)
		{
			var TcpClient = new TCPIPClient(Ip, Port);
			byte[] SendBytes = null;
			int UploadLen = 0;
			var TempFileBytes = API.FileBytes;
			byte[] PBBytes = null;
			UploadGroupPicStruct UploadPicBytes = new UploadGroupPicStruct();
			while (TempFileBytes.Length > 0)
			{
				if (TempFileBytes.Length >= 8192)
				{
					SendBytes = TempFileBytes.Take(8192).ToArray();
				}
				else
				{
					SendBytes = TempFileBytes;
				}
				UploadPicBytes = new UploadGroupPicStruct
				{
					UploadGroupPicSendInfo = new UploadGroupPicSendInfos
					{
						amout = 1,
						sendQQ = sendQQ.ToString(),
						SendCmd = "PicUp.DataUp",
						RaqId = (new Random()).Next(90000, 99999),
						field5 = 0,
						appId = 537061440,
						sendSize = 4096,
						flag = 2
					},
					UploadGroupPicFileInfo = new UploadGroupPicFileInfos
					{
						fileSize = API.FileBytes.Length,
						uploadSize = UploadLen,
						sendSize = SendBytes.Length,
						uKey = uKey,
						sendFileHash = API.MD5Hash(SendBytes),
						TotalHash = API.FileHash
					}
				};
				using (var ms = new MemoryStream())
				{
					Serializer.Serialize(ms, UploadPicBytes);
					Debug.Print("上传群图片:" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", " "));
					PBBytes = ms.ToArray();
				}

				var bytes = new byte[] {0x28};
				bytes = bytes.Concat(BitConverter.GetBytes(PBBytes.Length).Reverse().ToArray()).ToArray();
				bytes = bytes.Concat(BitConverter.GetBytes(SendBytes.Length).Reverse().ToArray()).ToArray();
				bytes = bytes.Concat(PBBytes).ToArray();
				bytes = bytes.Concat(SendBytes).ToArray();
				bytes = bytes.Concat(new byte[] {0x29}).ToArray();
				Debug.Print("上传群图片打包:" + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
				TcpClient.SendData(bytes);
				TempFileBytes = TempFileBytes.Skip(SendBytes.Length).ToArray();
				UploadLen = UploadLen + SendBytes.Length;
			}
			Debug.Print("上传群图片完成.");

		}
#endregion

#region 发送群语音
		public static API.GroupWithdrawInfo SendGroupAudio(byte[] BytesIn, byte[] filekey)
		{
			var timestamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10));
			List<SendMessages> MsgList = new List<SendMessages>();
			BytesIn = BytesIn.Skip(4).ToArray();
			using (var ms = new MemoryStream(BytesIn))
			{
				var result = Serializer.Deserialize<GroupAudioHashStruct>(ms);
				var sAddr = API.Int32ToIP(result.GroupAudioHashInfo.IP);
				var Port = result.GroupAudioHashInfo.Port;
				var uKey = result.GroupAudioHashInfo.ukey;
				var token = result.GroupAudioHashInfo.token;
				SendGroupMsgInfos msg = new SendGroupMsgInfos
				{
					SendGroupMsg = new SendGroupMsgs
					{
						SendGroupAudioInfo = new SendGroupAudioInfos
						{
							Field1 = 4, AudioHash = filekey, AudioName = BitConverter.ToString(filekey).Replace("-", "") + ".amr", AudioSize = 4590, uKey = new byte[] {0x16, 0x36, 0x20, 0x38, 0x36, 0x65, 0x41, 0x31, 0x4, 0x34, 0x38, 0x36, 0x34, 0x63, 0x32, 0x33, 0x63, 0x62, 0x34, 0x61, 0x33, 0x31, 0x36, 0x31, 0x30, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x35, 0x30, 0x1A, 0x88, 0xF9, 0x2C, 0x88, 0x4D, 0xB1, 0xF5, 0xC, 0xB1, 0x5B, 0x33, 0xF8, 0xFA, 0x10, 0x6D, 0x31, 0x41, 0x38, 0x38, 0x46, 0x39, 0x32, 0x43, 0x38, 0x38, 0x34, 0x44, 0x42, 0x31, 0x46, 0x35, 0x30, 0x43, 0x42, 0x31, 0x35, 0x42, 0x33, 0x33, 0x46, 0x38, 0x46, 0x41, 0x31, 0x30, 0x36, 0x44, 0x2E, 0x61, 0x6D, 0x72, 0x41},
							IP = 1214562876, Field9 = 3030586896, Port = 80, Field11 = 1, token = token, Field19 = 0, AudioUrl = "/?ver=0&rkey=3062020101045b3059020101020101020416525dfe042439306a33676f4e4551795062466248363843666238536b49525067497264332357336872020457f8edde041f0000000866696c6574797065000000013100000005636f64656300000001310400&filetype=1&voice_codec=1", Field29 = 1, Field30 = new AudioUnKnownInfos {Field1= 0}
						}
					}
				};

				SendGroupMsgStuct SendGroupMessage = new SendGroupMsgStuct
				{
					GroupInformation = new GroupInformations
					{
						GroupFrom = new GroupFroms {GroupId = API.GroupId}
					},
					GroupMsgId = new byte[] {8, 1, 0x10, 0, 0x18, 0},
					SendGroupMsgInfo = msg,
					MsgReqId = API.QQ.mRequestID,
					MsgRandomId = 0x10000000 + API.QQ.mRequestID,
					Groupcount = 1
				};
				API.GroupWithdraw.MsgReqId = API.QQ.mRequestID;
				API.GroupWithdraw.MsgRandomId = SendGroupMessage.MsgRandomId;
				using (var mStream = new MemoryStream())
				{
					Serializer.Serialize(mStream, SendGroupMessage);
					Debug.Print("发送群语音消息:" + mStream.ToArray().Length.ToString() + "\r\n" + BitConverter.ToString(mStream.ToArray()).Replace("-", " "));
					var SendBytes = API.PackCmdHeader("MessageSvc.PbSendMsg", mStream.ToArray());
					API.TClient.SendData(API.PackAllHeader(SendBytes));
				}

				//上传群语音
				//var url = "http://" + sAddr + "/?ver=4679&ukey=" + BitConverter.ToString(uKey).Replace("-", "") + "&filekey=" + BitConverter.ToString(filekey).Replace("-", "") + "&filesize=" + fileSize.ToString() + "&range=0&bmd5=" + BitConverter.ToString(filekey).Replace("-", "") + "&mType=pttGu&Audio_codec=1";
				//Dictionary<string, object> Headerdics = new Dictionary<string, object>()
				//{
				//	{"Accept-Encoding", "identity"},
				//	{"User-Agent", "Dalvik/2.1.0 (Linux; U; Android 5.0; SM-G9009D Build/LRX21T"},
				//	{"Content-Type", "application/x-www-form-urlencoded"},
				//	{"Host", sAddr}
				//};
				//var res = HttpClientPostAsync2(url, Headerdics, fileByte, "application/x-www-form-urlencoded", mycookiecontainer, RedirectUrl).Result;


			}

			return API.GroupWithdraw;
		}

#endregion

#region 撤回群消息
		public static bool WithdrawGroupMsg(long GroupId, long MsgReqId, long MsgRandomId)
		{
			WithdrawGroupMsgStuct WithdrawMsg = new WithdrawGroupMsgStuct
			{
				WithdrawGroupMsg = new WithdrawGroupMsgs
				{
					Amount = 1, types = 0, GroupId = GroupId, endFlag = new endFlags {flag = 0},
					WithdrawInfo = new WithdrawInfos
					{
						reqId = MsgReqId,
						RandomId = MsgRandomId
					}
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, WithdrawMsg);
				Debug.Print("撤回群消息结构:" + ms.ToArray().Length.ToString() + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", " "));
				var bytes = API.PackCmdHeader("PbMessageSvc.PbMsgWithDraw", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}
			return true;
		}
#endregion
	}



#region 置已读群消息结构
	[ProtoContract]
	public class MakeReadedGroupMessage
	{
		[ProtoMember(1)]
		public ReadedGroupMsg ReadedGroupMsg;
	}
	[ProtoContract]
	public class ReadedGroupMsg
	{
		[ProtoMember(1)]
		public long GroupId;
		[ProtoMember(2)]
		public long MsgId;
	}
#endregion

#region 解析群消息主结构
	[ProtoContract]
	public class GroupMsgStuct
	{
		[ProtoMember(1)]
		public GroupInfos GroupInfo;
		[ProtoMember(2)]
		public long GroupId;
		[ProtoMember(3)]
		public string Field3;
	}
	[ProtoContract]
	public class GroupInfos
	{
		[ProtoMember(1)]
		public GroupQQInfos GroupQQInfo;
		[ProtoMember(2)]
		public byte[] GroupMsgId;
		[ProtoMember(3)]
		public GroupMessageInfos GroupMessageInfo;
	}
	[ProtoContract]
	public class GroupQQInfos
	{
		[ProtoMember(1)]
		public long QQFromId;
		[ProtoMember(2)]
		public long thisQQId;
		[ProtoMember(5)]
		public long fromReq; //用于撤回
		[ProtoMember(6)]
		public long RecvTime;
		[ProtoMember(7)]
		public long MsgUid;
		[ProtoMember(9)]
		public GroupInfomations GroupInfo;
	}
	[ProtoContract]
	public class GroupInfomations
	{
		[ProtoMember(1)]
		public long GroupId;
		[ProtoMember(2)]
		public int Field2;
		[ProtoMember(3)]
		public int Field3;
		[ProtoMember(4)]
		public string AdminNick;
		[ProtoMember(6)]
		public int MemberAmount;
		[ProtoMember(7)]
		public int AdminAmount;
		[ProtoMember(8)]
		public string GroupName;
	}

	[ProtoContract]
	public class GroupMessageInfos
	{
		[ProtoMember(1)]
		public GroupMsgInfos GroupMsgInfo;
	}
	[ProtoContract]
	public class GroupMsgInfos
	{
		[ProtoMember(1)]
		public FontInfos FontInfo;
		[ProtoMember(2)]
		public byte[] GroupMsgContent;
		[ProtoMember(4)]
		public byte[] GroupAudioMsgContent;
	}
	[ProtoContract]
	public class FontInfos
	{
		[ProtoMember(1)]
		public long ErrorCode;
		[ProtoMember(2)]
		public long RecvTime; //用于撤回
		[ProtoMember(3)]
		public long fromRandom; //用于撤回
		[ProtoMember(4)]
		public long Field4;
		[ProtoMember(5)]
		public long fontSize;
		[ProtoMember(6)]
		public long fontType;
		[ProtoMember(7)]
		public long fontColor;
		[ProtoMember(8)]
		public long fontBold;
		[ProtoMember(9)]
		public string fontName;
	}

#endregion

#region 解析消息内容-文本/图片/视频消息
	[ProtoContract]
	public class GroupMessageStruct
	{
		[ProtoMember(1)]
		public List<GroupTextMessageContent> GroupTextMsgContent; //文本消息
		[ProtoMember(2)]
		public List<BigFaceInfos> BigFace; //表情消息
		[ProtoMember(5)]
		public GroupFileMessages GroupFileMessage; //文件消息
		[ProtoMember(6)]
		public byte[] HotPic; //热图消息
		[ProtoMember(8)]
		public List<GroupPicMessageContents> GroupPicMessageContent; //图片消息
		[ProtoMember(9)]
		public byte[] GroupMessageType;
		[ProtoMember(12)]
		public byte[] GroupXMLMessage; //xml卡片消息、发送位置
		[ProtoMember(16)]
		public GroupNickInfos SendNick;
		[ProtoMember(19)]
		public List<GroupVideoMessageContents> GroupVideoMessageContent; //视频消息
		[ProtoMember(24)]
		public byte[] RedEnvelope; //转账、红包
		[ProtoMember(37)]
		public byte[] GroupMessageOther;
		[ProtoMember(42)]
		public byte[] LiMiXiuMessage; //厘米秀
		[ProtoMember(45)]
		public GroupReplyMessages GroupReplyMessage; //回复消息
		[ProtoMember(51)]
		public byte[] GroupJsonMessage; //JOSN消息
		[ProtoMember(53)]
		public byte[] FlashPic; //戳一戳、闪照
	}
	[ProtoContract]
	public class GroupTextMessageContent
	{
		[ProtoMember(1)]
		public string content;
		[ProtoMember(2)]
		public byte[] field2;
		[ProtoMember(3)]
		public byte[] At;
	}
	[ProtoContract]
	public class GroupVideoMessageContents
	{
		[ProtoMember(1)]
		public string VideoGuid;
		[ProtoMember(3)]
		public string VideoAddr;
		[ProtoMember(9)]
		public string VideoHash;
	}
	[ProtoContract]
	public class GroupReplyMessages //带回复的消息
	{
		[ProtoMember(1)]
		public long MsgReqId;
		[ProtoMember(2)]
		public long thisQQId;
		[ProtoMember(3)]
		public long SendTimeStamp;
		[ProtoMember(4)]
		public int MsgAmount;
		[ProtoMember(5)]
		public List<GroupTextMessageContent> OldMsgContent;
		[ProtoMember(8)]
		public byte[] OldMsgId;
		[ProtoMember(10)]
		public long[] GroupId;
	}
	[ProtoContract]
	public class GroupFileMessages
	{
		[ProtoMember(2)]
		public byte[] FileHash;
	}
	[ProtoContract]
	public class GroupNickInfos
	{
		[ProtoMember(2)]
		public string NickName;
	}
	[ProtoContract]
	public class BigFaceInfos
	{
		[ProtoMember(1)]
		public int FaceId;
		[ProtoMember(2)]
		public byte[] FaceName;
		[ProtoMember(11)]
		public byte[] FaceHash;
	}
	[ProtoContract]
	public class GroupPicMessageContents
	{
		[ProtoMember(2)]
		public string PicContent;
		[ProtoMember(7)]
		public int GetIp;
		[ProtoMember(8)]
		public int GetIp2;
		[ProtoMember(9)]
		public int Port;
		[ProtoMember(14)]
		public string PicAddr1;
	}
#endregion


#region 解析消息内容-语音消息
	[ProtoContract]
	public class GroupAudioMessageStruct
	{
		[ProtoMember(4)]
		public byte[] AudioHash;
		[ProtoMember(5)]
		public string AudioName;
		[ProtoMember(20)]
		public string AudioUrl;
	}

#endregion

#region 解析群图片取GUID
	[ProtoContract]
	public class GroupPicGuidStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int ImageId;
		[ProtoMember(2, IsRequired=true)]
		public int Amount;
		[ProtoMember(3, IsRequired=true)]
		public GroupPicGuidInfos GroupPicGuidInfo;
	}
	[ProtoContract]
	public class GroupPicGuidInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public int field1;
		[ProtoMember(2, IsRequired=true)]
		public int field2;
		[ProtoMember(4, IsRequired=true)]
		public int field4;
		[ProtoMember(5)]
		public GroupPicHashs GroupPicHash;
		[ProtoMember(6, IsRequired=true)]
		public List<int> Ip;
		[ProtoMember(7, IsRequired=true)]
		public List<int> Port;
		[ProtoMember(8, IsRequired=true)]
		public byte[] uKey;

	}
	[ProtoContract]
	public class GroupPicHashs
	{
		[ProtoMember(1, IsRequired=true)]
		public byte[] Hash;
		[ProtoMember(2, IsRequired=true)]
		public int picPix;
		[ProtoMember(3, IsRequired=true)]
		public int picSize;
		[ProtoMember(4, IsRequired=true)]
		public int picWidth;
		[ProtoMember(5, IsRequired=true)]
		public int picHeigh;
	}
#endregion

#region 解析群语音取Hash
	[ProtoContract]
	public class GroupAudioHashStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public long AudioType;
		[ProtoMember(5, IsRequired=true)]
		public GroupAudioHashInfos GroupAudioHashInfo;
	}
	[ProtoContract]
	public class GroupAudioHashInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public int Field1;
		[ProtoMember(2, IsRequired=true)]
		public int Field2;
		[ProtoMember(4, IsRequired=true)]
		public int Field4;
		[ProtoMember(5, IsRequired=true)]
		public int IP;
		[ProtoMember(6, IsRequired=true)]
		public int Port;
		[ProtoMember(7, IsRequired=true)]
		public byte[] ukey;
		[ProtoMember(8, IsRequired=true)]
		public long AudioSize;
		[ProtoMember(11, IsRequired=true)]
		public byte[] token;
	}
#endregion


#region 构造图片消息
	[ProtoContract]
	public class SendGroupPicMsgStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int PicType;
		[ProtoMember(2, IsRequired=true)]
		public int Amount;
		[ProtoMember(3, IsRequired=true)]
		public GroupPicInfos GroupPicInfo;
	}
	[ProtoContract]
	public class GroupPicInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public long GroupId;
		[ProtoMember(2, IsRequired=true)]
		public long SendQQ;
		[ProtoMember(3, IsRequired=true)]
		public int StartFlag;
		[ProtoMember(4, IsRequired=true)]
		public byte[] PicHash;
		[ProtoMember(5, IsRequired=true)]
		public int PicSize;
		[ProtoMember(6, IsRequired=true)]
		public string PicName;
		[ProtoMember(7, IsRequired=true)]
		public int Field7;
		[ProtoMember(8, IsRequired=true)]
		public int Field8;
		[ProtoMember(9, IsRequired=true)]
		public int Field9;
		[ProtoMember(10, IsRequired=true)]
		public int PicWidth;
		[ProtoMember(11, IsRequired=true)]
		public int PicHeigh;
		[ProtoMember(12, IsRequired=true)]
		public int PicPix;
		[ProtoMember(13, IsRequired=true)]
		public string PicVer;
		[ProtoMember(15, IsRequired=true)]
		public int Field15;
		[ProtoMember(19, IsRequired=true)]
		public int Field19;
	}
#endregion

#region 发送群图片
	[ProtoContract]
	public class SendGroupPicStruct
	{
		[ProtoMember(8, IsRequired=true)]
		public SendGroupPicInfos SendGroupPicInfo;
	}
	[ProtoContract]
	public class SendGroupPicInfos
	{
		[ProtoMember(2, IsRequired=true)]
		public string PicName;
		[ProtoMember(7, IsRequired=true)]
		public long PicId1;
		[ProtoMember(8, IsRequired=true)]
		public long PicId2;
		[ProtoMember(9, IsRequired=true)]
		public int picIconWidth;
		[ProtoMember(10, IsRequired=true)]
		public int picIconHeigh;
		[ProtoMember(11, IsRequired=true)]
		public string picMD5;
		[ProtoMember(12, IsRequired=true)]
		public int picAmount;
		[ProtoMember(13, IsRequired=true)]
		public byte[] picHash;
		[ProtoMember(17, IsRequired=true)]
		public int picType;
		[ProtoMember(20, IsRequired=true)]
		public int picPix;
		[ProtoMember(22, IsRequired=true)]
		public int picWidth;
		[ProtoMember(24, IsRequired=true)]
		public int picHeigh;
		[ProtoMember(25, IsRequired=true)]
		public int picSize;
		[ProtoMember(26, IsRequired=true)]
		public long Field26;
		[ProtoMember(29, IsRequired=true)]
		public long Field29;
		[ProtoMember(30, IsRequired=true)]
		public long Field30;
	}
#endregion

#region 上传群图片消息
	[ProtoContract]
	public class UploadGroupPicStruct
	{
		[ProtoMember(1)]
		public UploadGroupPicSendInfos UploadGroupPicSendInfo;
		[ProtoMember(2)]
		public UploadGroupPicFileInfos UploadGroupPicFileInfo;
	}
	[ProtoContract]
	public class UploadGroupPicSendInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public int amout;
		[ProtoMember(2, IsRequired=true)]
		public string sendQQ;
		[ProtoMember(3, IsRequired=true)]
		public string SendCmd;
		[ProtoMember(4, IsRequired=true)]
		public int RaqId;
		[ProtoMember(5, IsRequired=true)]
		public int field5;
		[ProtoMember(6, IsRequired=true)]
		public int appId;
		[ProtoMember(7, IsRequired=true)]
		public int sendSize;
		[ProtoMember(8, IsRequired=true)]
		public int flag;
	}
	[ProtoContract]
	public class UploadGroupPicFileInfos
	{
		[ProtoMember(2, IsRequired=true)]
		public int fileSize;
		[ProtoMember(3, IsRequired=true)]
		public int uploadSize;
		[ProtoMember(4, IsRequired=true)]
		public int sendSize;
		[ProtoMember(6, IsRequired=true)]
		public byte[] uKey;
		[ProtoMember(8, IsRequired=true)]
		public byte[] sendFileHash;
		[ProtoMember(9, IsRequired=true)]
		public byte[] TotalHash;
	}
#endregion

#region 构造语音消息
	[ProtoContract]
	public class SendGroupAudioMsgStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int AudioType;
		[ProtoMember(2, IsRequired=true)]
		public int Field2;
		[ProtoMember(5, IsRequired=true)]
		public GroupAudioInfos GroupAudioInfo;
	}
	[ProtoContract]
	public class GroupAudioInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public long GroupId;
		[ProtoMember(2, IsRequired=true)]
		public long SendQQ;
		[ProtoMember(3, IsRequired=true)]
		public int StartFlag;
		[ProtoMember(4, IsRequired=true)]
		public byte[] AudioHash;
		[ProtoMember(5, IsRequired=true)]
		public int AudioSize;
		[ProtoMember(6, IsRequired=true)]
		public string AudioName;
		[ProtoMember(7, IsRequired=true)]
		public int Field7;
		[ProtoMember(8, IsRequired=true)]
		public int Field8;
		[ProtoMember(9, IsRequired=true)]
		public int Field9;
		[ProtoMember(10, IsRequired=true)]
		public string AudioVer;
		[ProtoMember(12, IsRequired=true)]
		public int Field12;
		[ProtoMember(13, IsRequired=true)]
		public int Field13;
		[ProtoMember(14, IsRequired=true)]
		public int Field14;
		[ProtoMember(15, IsRequired=true)]
		public int Field15;
	}
#endregion

#region 发送群语音
	[ProtoContract]
	public class SendGroupAudioInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public int Field1;
		[ProtoMember(4, IsRequired=true)]
		public byte[] AudioHash;
		[ProtoMember(5, IsRequired=true)]
		public string AudioName;
		[ProtoMember(6, IsRequired=true)]
		public long AudioSize;
		[ProtoMember(7, IsRequired=true)]
		public byte[] uKey;
		[ProtoMember(8, IsRequired=true)]
		public int IP;
		[ProtoMember(9, IsRequired=true)]
		public long Field9;
		[ProtoMember(10, IsRequired=true)]
		public int Port;
		[ProtoMember(11, IsRequired=true)]
		public int Field11;
		[ProtoMember(18, IsRequired=true)]
		public byte[] token;
		[ProtoMember(19, IsRequired=true)]
		public int Field19;
		[ProtoMember(20, IsRequired=true)]
		public string AudioUrl;
		[ProtoMember(29, IsRequired=true)]
		public int Field29;
		[ProtoMember(30, IsRequired=true)]
		public AudioUnKnownInfos Field30;
	}
	[ProtoContract]
	public class AudioUnKnownInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public int Field1;
	}
#endregion


#region 发送群消息结构
	[ProtoContract]
	public class SendGroupMsgStuct
	{
		[ProtoMember(1)]
		public GroupInformations GroupInformation;
		[ProtoMember(2)]
		public byte[] GroupMsgId;
		[ProtoMember(3)]
		public SendGroupMsgInfos SendGroupMsgInfo;
		[ProtoMember(4, IsRequired=true)]
		public long MsgReqId;
		[ProtoMember(5, IsRequired=true)]
		public long MsgRandomId;
		[ProtoMember(8, IsRequired=true)]
		public int Groupcount;
	}
	[ProtoContract]
	public class SendGroupMsgInfos
	{
		[ProtoMember(1)]
		public SendGroupMsgs SendGroupMsg;
	}
	[ProtoContract]
	public class SendGroupMsgs
	{
		[ProtoMember(2)]
		public List<SendMessages> SendMessage;
		[ProtoMember(4)]
		public SendGroupAudioInfos SendGroupAudioInfo;
	}
	[ProtoContract]
	public class SendMessages
	{
		[ProtoMember(1)]
		public SendContents SendContent;
		[ProtoMember(8)]
		public SendGroupPicInfos SendGroupPicInfo;
		[ProtoMember(9)]
		public GroupXmlFlags GroupXmlFlag;
		[ProtoMember(12)]
		public GroupXmlContents GroupXmlMsg;
	}
	[ProtoContract]
	public class SendContents
	{
		[ProtoMember(1)]
		public string Content;
		[ProtoMember(3)]
		public byte[] AtHash;
		[ProtoMember(2)]
		public BigFaceInfos BigFace;
		[ProtoMember(6)]
		public byte[] HotPic;
		[ProtoMember(9)]
		public byte[] GroupMessageType;
		[ProtoMember(16)]
		public byte[] GroupSenderInfo;
		[ProtoMember(37)]
		public byte[] GroupMessageOther;
	}
	[ProtoContract]
	public class GroupInformations
	{
		[ProtoMember(2)]
		public GroupFroms GroupFrom;
	}
	[ProtoContract]
	public class GroupXmlContents
	{
		[ProtoMember(1)]
		public byte[] Content;
		[ProtoMember(2)]
		public int flag;
	}
	[ProtoContract]
	public class GroupXmlFlags
	{
		[ProtoMember(1)]
		public long BuddleID;
	}
	[ProtoContract]
	public class GroupFroms
	{
		[ProtoMember(1)]
		public long GroupId;
	}
#endregion

#region 撤回群消息结构
	[ProtoContract]
	public class WithdrawGroupMsgStuct
	{
		[ProtoMember(2)]
		public WithdrawGroupMsgs WithdrawGroupMsg;
	}
	[ProtoContract]
	public class WithdrawGroupMsgs
	{
		[ProtoMember(1, IsRequired=true)]
		public int Amount;
		[ProtoMember(2, IsRequired=true)]
		public int types;
		[ProtoMember(3, IsRequired=true)]
		public long GroupId;
		[ProtoMember(4, IsRequired=true)]
		public WithdrawInfos WithdrawInfo;
		[ProtoMember(5)]
		public endFlags endFlag;
	}
	[ProtoContract]
	public class WithdrawInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public long reqId;
		[ProtoMember(2, IsRequired=true)]
		public long RandomId;
	}
	[ProtoContract]
	public class endFlags
	{
		[ProtoMember(1, IsRequired=true)]
		public int flag;
	}
#endregion


}