
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
	public class FriendMsg
	{

#region 解析好友消息
		public static API.FriendWithdrawInfo ParsingFriendMsg(byte[] byteIn)
		{

			string TextMsg = "";
			string FaceMsg = "";
			string HotPicMsg = "";
			string PicMsg = "";
			string AudioMsg = "";
			string VideoMsg = "";
			string ReplyMsg = "";
			string XmlMsg = "";
			byteIn = byteIn.Skip(4).ToArray();
			using (MemoryStream ms = new MemoryStream(byteIn))
			{
				try
				{
					var result = Serializer.Deserialize<FriendMsgStuct>(ms);
					long qqid = result.MessageInfo[0].QQInfo.QQFromInfo.QQId;
					long qqfromid = result.MessageInfo[0].QQInfo.QQFromInfo.QQFromId;
					API.FriendWithdraw.MsgReqId = result.MessageInfo[0].QQInfo.QQFromInfo.fromReq;
					API.FriendWithdraw.MsgTimeStamp = result.MessageInfo[0].QQInfo.QQFromInfo.RecvTime;
					if (result.MessageInfo[0].QQInfo.MsgInfo.FileUploadInfo != null) //文件上传消息
					{
						Debug.Print("朋友文件上传:" + "\r\n" + BitConverter.ToString(result.MessageInfo[0].QQInfo.MsgInfo.FileUploadInfo).Replace("-", " "));
						using (MemoryStream mStream = new MemoryStream(result.MessageInfo[0].QQInfo.MsgInfo.FileUploadInfo))
						{
							var MsgResult = Serializer.Deserialize<FileMessageStruct>(mStream);
							var fileName = MsgResult.FileInfo.FileName;
							var fileSize = MsgResult.FileInfo.FileSize;
							API.FriendWithdraw.MsgRandomId = MsgResult.FileInfo.FileRandId;
						}
					}
					if (result.MessageInfo[0].QQInfo.MsgInfo.MsgTextInfo.fonts != null)
					{
						API.FriendWithdraw.MsgRandomId = result.MessageInfo[0].QQInfo.MsgInfo.MsgTextInfo.fonts.fromRandom;
					}
					if (result.MessageInfo[0].QQInfo.MsgInfo.MsgTextInfo.MsgContent != null)
					{
						var msgByte = result.MessageInfo[0].QQInfo.MsgInfo.MsgTextInfo.MsgContent;
						Debug.Print("朋友消息内容:" + "\r\n" + BitConverter.ToString(msgByte).Replace("-", " "));
						using (MemoryStream mStream = new MemoryStream(msgByte))
						{
							var MsgResult = Serializer.Deserialize<MessageStruct>(mStream);
							if (MsgResult.ReplyMessage != null) //是带回复的消息
							{
								string replyContent = "";
								for (var i = 0; i < MsgResult.ReplyMessage.OldMsgContent.Count; i++)
								{
									replyContent = replyContent + MsgResult.ReplyMessage.OldMsgContent[i].content.Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
								}
								ReplyMsg = " [Reply=@" + MsgResult.ReplyMessage.thisQQId.ToString() + "内容:" + replyContent + "]";
							}
							if (MsgResult.TextMessageContent != null) //有文本或小表情或热图内容
							{
								for (var i = 0; i < MsgResult.TextMessageContent.Count; i++)
								{
									TextMsg = TextMsg + MsgResult.TextMessageContent[i].content;
								}
							}
							if (MsgResult.BigFace != null) //有大表情内容
							{
								for (var i = 0; i < MsgResult.BigFace.Count; i++)
								{
									var FaceId = MsgResult.BigFace[i].FaceId;
									FaceMsg = FaceMsg + " [BigFaceId=" + FaceId.ToString() + "]";
								}
							}
							if (MsgResult.PicMessageContent != null) //有图片内容
							{
								for (var i = 0; i < MsgResult.PicMessageContent.Count; i++)
								{
									PicMsg = PicMsg + "https://c2cpicdw.qpic.cn/offpic_new/" + MsgResult.PicMessageContent[i].PicGuid + "/0 ";
								}
								PicMsg = "[pic,link=" + PicMsg + "]";
							}
							//If Not MsgResult.XMLMessage Is Nothing Then '有XML内容
							//    Dim xmlByte = MsgResult.XMLMessage
							//    XmlMsg = Encoding.UTF8.GetString(xmlByte)
							//End If
							if (result.MessageInfo[0].QQInfo.MsgInfo.MsgTextInfo.AudioContent != null) //有语音内容
							{
								using (MemoryStream msStream = new MemoryStream(result.MessageInfo[0].QQInfo.MsgInfo.MsgTextInfo.AudioContent))
								{
									var Ret = Serializer.Deserialize<AudioMessageStruct>(msStream);
									AudioMsg = Ret.AudioUrl;
								}
							}
							if (MsgResult.VideoMessageContent != null) //有视频内容
							{
								for (var i = 0; i < MsgResult.VideoMessageContent.Count; i++)
								{
									VideoMsg = VideoMsg + "[video,guid=" + MsgResult.VideoMessageContent[i].VideoGuid + "]";
								}
							}
							SDK.GetResult(qqfromid.ToString(), qqfromid.ToString(), ReplyMsg + FaceMsg + HotPicMsg + TextMsg + PicMsg + AudioMsg + VideoMsg);
						}
					}
					var MsgTimeStamp = result.MessageInfo[0].QQInfo.QQFromInfo.RecvTime;
					using (var mMemoryStream = new MemoryStream())
					{
						Serializer.Serialize(mMemoryStream, result.SyncCoookie);
						MakeReadedFriendMsg(qqfromid, mMemoryStream.ToArray(), MsgTimeStamp);
					}
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message.ToString());
				}
			}
			return API.FriendWithdraw;
		}
#endregion

#region 置好友消息已读
		public static bool MakeReadedFriendMsg(long qqfromid, byte[] SyncCoookies, long MsgTimeStamp)
		{
			MakeReadedFriendMessage ReadedMsg = new MakeReadedFriendMessage
			{
				ReadedMsg = new ReadedFriendMsg
				{
					QQFromInfo = new QQFromInfos
					{
						QQFromId = qqfromid,
						RecvTime = MsgTimeStamp
					},
					SyncCoookie = SyncCoookies
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

#region 发送好友消息
		public static API.FriendWithdrawInfo SendFriendMsg(long thisQQ,long sendQQ, byte[] MsgBytes, API.MsgType MsgTypes)
		{
			byte[] bytes = null;
			List<TextMessageContents> MsgList = new List<TextMessageContents>();
			List<byte[]> ListBytes = new List<byte[]>();
			var timestamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10));
			if (MsgTypes == API.MsgType.TextMsg) //文字消息
			{
				TextMessageContents MsgListStruct = new TextMessageContents {content = Encoding.UTF8.GetString(MsgBytes)};
				MsgList.Add(MsgListStruct);
				MessageStruct msg = new MessageStruct {TextMessageContent = MsgList};
				using (MemoryStream mStream = new MemoryStream())
				{
					Serializer.Serialize(mStream, msg);
					bytes = mStream.ToArray();
				}
			}
			else if (MsgTypes == API.MsgType.XmlMsg)
			{
				TextMessageContents MsgListStruct = new TextMessageContents
				{
					XmlContent = new XmlContents
					{
						Content = MsgBytes,
						flag = 1
					}
				};
				MsgList.Add(MsgListStruct);
				MessageStruct msg = new MessageStruct {TextMessageContent = MsgList};
				using (MemoryStream mStream = new MemoryStream())
				{
					Serializer.Serialize(mStream, msg);
					bytes = mStream.ToArray();
				}
			}
			else if (MsgTypes == API.MsgType.PicMsg) //图片消息
			{
				MsgBytes = MsgBytes.Skip(4).ToArray();
				try
				{
					using (var ms = new MemoryStream(MsgBytes))
					{
						var result = Serializer.Deserialize<PicGuidStruct>(ms);
						if (result.PicGuidInfo.uKey != null) //服务器没有该图片的hash
						{
							var uKey = result.PicGuidInfo.uKey;
							var Ip = API.Int32ToIP(result.PicGuidInfo.Ip[0]);
							var Port = result.PicGuidInfo.Port[0];
							UploadFriendPicByTCP(sendQQ, uKey, Ip, Port);
						}
						SendFriendPicStruct msg = new SendFriendPicStruct
						{
							SendFriendPicInfo = new SendFriendPicInfos
							{
								PicName = BitConverter.ToString(API.FileHash).Replace("-", "") + ".jpg",
								PicHash = API.FileHash,
								PicGuid = result.PicGuidInfo.PicGuid,
								PicPix = 1000,
								PicWidth = 647,
								PicHeigh = 980
							}
						};
						using (MemoryStream mStream = new MemoryStream())
						{
							Serializer.Serialize(mStream, msg);
							bytes = mStream.ToArray();
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message.ToString());
				}
			}
			ListBytes.Add(bytes);
			SendFriendMsgStuct SendMsg = new SendFriendMsgStuct
			{
				SendFromInfo = new SendQQFrom
				{
					FromInfo = new FromInfos {FromId = sendQQ}
				},
				MsgId = new byte[] {8, 1, 0x10, 0, 0x18, 0},
				MsgInfo = new MsgInfos
				{
					MsgTextInfo = new MsgTextInfos {MsgContent = bytes}
				},
				RequestId = API.QQ.mRequestID,
				TimeStamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10)),
				count = 1,
				SyncCoookie = new SyncCoookies
				{
					timestamp1 = timestamp,
					timestamp2 = timestamp,
					timestamp3 = timestamp,
					Field3 = 805979870,
					Field4 = 3344460674,
					Field5 = 82343012,
					Field6 = 3281833389,
					Field7 = 2696570484,
					Field8 = 81,
					Field10 = 0
				}
			};
			API.FriendWithdraw.MsgReqId = API.QQ.mRequestID;
			API.FriendWithdraw.MsgTimeStamp = SendMsg.TimeStamp;
			API.FriendWithdraw.MsgRandomId = SendMsg.TimeStamp;
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, SendMsg);
				Debug.Print("发送好友消息:" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", " "));
				var SendBytes = API.PackCmdHeader("MessageSvc.PbSendMsg", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(SendBytes));
			}
			return API.FriendWithdraw;
		}

		#endregion

#region 上传图片		

		public static void UploadFriendPicByTCP(long sendQQ, byte[] uKey, string Ip, int Port)
		{
			var TcpClient = new TCPIPClient(Ip, Port);
			byte[] SendBytes = null;
			int UploadLen = 0;
			var TempFileBytes = API.FileBytes;
			byte[] PBBytes = null;
			UploadFriendPicStruct UploadPicBytes = new UploadFriendPicStruct();
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
				UploadPicBytes = new UploadFriendPicStruct
				{
					UploadFriendPicSendInfo = new UploadFriendPicSendInfos
					{
						amout = 1,
						sendQQ = sendQQ.ToString(),
						SendCmd = "PicUp.DataUp",
						RaqId = (new Random()).Next(90000, 99999),
						field5 = 0,
						appId = 537061440,
						sendSize = 4096,
						flag = 1
					},
					UploadFriendPicFileInfo = new UploadFriendPicFileInfos
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
					Debug.Print("上传好友图片:" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", " "));
					PBBytes = ms.ToArray();
				}

				var bytes = new byte[] {0x28};
				bytes = bytes.Concat(BitConverter.GetBytes(PBBytes.Length).Reverse().ToArray()).ToArray();
				bytes = bytes.Concat(BitConverter.GetBytes(SendBytes.Length).Reverse().ToArray()).ToArray();
				bytes = bytes.Concat(PBBytes).ToArray();
				bytes = bytes.Concat(SendBytes).ToArray();
				bytes = bytes.Concat(new byte[] {0x29}).ToArray();
				Debug.Print("上传好友图片打包:" + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
				TcpClient.SendData(bytes);
				TempFileBytes = TempFileBytes.Skip(SendBytes.Length).ToArray();
				UploadLen = UploadLen + SendBytes.Length;
			}
			Debug.Print("上传好友图片完成.");

		}
#endregion

#region 发送好友语音
		public static bool SendFriendAudio(long thisQQ,long sendQQ, byte[] BytesIn, byte[] filekey)
		{
			byte[] bytes = null;
			List<byte[]> ListBytes = new List<byte[]>();
			ListBytes.Add(new byte[] {0x4A, 0x2, 0x28, 0x0});
			ListBytes.Add(new byte[] {0xAA, 0x2, 0x3, 0x88, 1, 0});
			byte[] array = ListBytes.SelectMany((a) => a).ToArray();

			BytesIn = BytesIn.Skip(4).ToArray();
			using (var ms = new MemoryStream(BytesIn))
			{
				var result = Serializer.Deserialize<AudioHashStruct>(ms);
				var sAddr = result.AudioHashInfo.IP;
				var Port = result.AudioHashInfo.Port;
				var uKey = result.AudioHashInfo.ukey;
				var token = result.AudioHashInfo.token;
				SendAudioInfos msg = new SendAudioInfos
				{
					Field1 = 4,
					thisQQ = thisQQ,
					token = "[Audio,hash=" + BitConverter.ToString(filekey).Replace("-", "") + ",token=" + Encoding.UTF8.GetString(token) + "]",
					AudioHash = filekey,
					AudioName = BitConverter.ToString(filekey).Replace("-", "") + ".amr",
					AudioSize = 6390,
					uKey = new byte[] {0x3, 0x8, 0x0, 0x4, 0x0, 0x0, 0x0, 0x1, 0x9, 0x0, 0x4, 0x0, 0x0, 0x0, 0x5, 0xA, 0x0, 0x2, 0x8, 0x0},
					Field9 = 3030587005,
					Port = 80,
					Field11 = 1
				};
				using (MemoryStream mStream = new MemoryStream())
				{
					Serializer.Serialize(mStream, msg);
					bytes = mStream.ToArray();
					Debug.Print("发送好友语音结构:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
				}
				SendFriendMsgStuct SendMsg = new SendFriendMsgStuct
				{
					SendFromInfo = new SendQQFrom
					{
						FromInfo = new FromInfos {FromId = sendQQ}
					},
					MsgId = new byte[] {8, 1, 0x10, 0, 0x18, 0},
					MsgInfo = new MsgInfos
					{
						MsgTextInfo = new MsgTextInfos
						{
							MsgContent = array,
							AudioContent = bytes
						}
					},
					RequestId = API.QQ.mRequestID,
					TimeStamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10)),
					SyncCoookie = new SyncCoookies
					{
						timestamp2 = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10)),
						Field5 = 1634440201,
						Field6 = 916698971,
						Field7 = 2800362740,
						Field10 = 0
					}
				};
				using (var mStream = new MemoryStream())
				{
					Serializer.Serialize(mStream, SendMsg);
					Debug.Print("发送好友语音消息:" + mStream.ToArray().Length.ToString() + "\r\n" + BitConverter.ToString(mStream.ToArray()).Replace("-", " "));
					var SendBytes = API.PackCmdHeader("MessageSvc.PbSendMsg", mStream.ToArray());
					API.TClient.SendData(API.PackAllHeader(SendBytes));
				}
			}
			return true;
		}

#endregion

#region 获取语音地址
		public static string GetFriendAudioUrl(byte[] BytesIn, byte[] token)
		{
			BytesIn = BytesIn.Skip(4).ToArray();

			SendAudioMsgStruct GetAudioInfo = new SendAudioMsgStruct
			{
				AudioType = 1200,
				Field2 = 0,
				GetAudioInfo = new GetAudioInfos
				{
					thisQQ = API.QQ.LongQQ,
					token = token
				},
				Field101 = 17,
				Field102 = 104,
				AudioOtherInfo = new AudioOtherInfos
				{
					Field1 = 3,
					Field2= 104,
					Field90300 = 0,
					Field90700 = 0,
					Field91100 = 1
				}
			};
			using (var mStream = new MemoryStream())
			{
				Serializer.Serialize(mStream, GetAudioInfo);
				Debug.Print("构造语音:" + "\r\n" + BitConverter.ToString(mStream.ToArray()).Replace("-", " "));
				var bytes = API.PackCmdHeader("PttCenterSvr.pb_pttCenter_CMD_REQ_APPLY_DOWNLOAD-1200", mStream.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}

			///上传语音
			//var url = "http://" + sAddr + "/?ver=2&ukey=" + BitConverter.ToString(uKey).Replace("-", "") + "&filekey=" + BitConverter.ToString(filekey).Replace("-", "") + "&filesize=" + fileSize.ToString() + "&bmd5=" + BitConverter.ToString(filekey).Replace("-", "") + "&range=0&Audio_codec=1&mType=pttCu";
			//Dictionary<string, object> Headerdics = new Dictionary<string, object>()
			//{
			//	{"User-Agent", "Dalvik/2.1.0 (Linux; U; Android 5.0; SM-G9009D Build/LRX21T"},
			//	{"Content-Type", "application/x-www-form-urlencoded"},
			//	{"Host", sAddr}
			//};
			//var res = HttpHelper.HttpClientPostAsync2(url, Headerdics, fileByte, "application/x-www-form-urlencoded", mycookiecontainer, RedirectUrl).Result;

			using (var ms = new MemoryStream(BytesIn))
			{
				var result = Serializer.Deserialize<AudioHashStruct>(ms);
				var sAddr = result.AudioUrlInfo.AudioUrl.IP;
				var Port = result.AudioUrlInfo.AudioUrl.Port;
				var url = result.AudioUrlInfo.AudioUrl.Url;
				//取语音
				Dictionary<string, object> Headerdics = new Dictionary<string, object>()
				{
					{"User-Agent", "Dalvik/2.1.0 (Linux; U; Android 5.0; SM-G9009D Build/LRX21T"},
					{"Content-Type", "application/x-www-form-urlencoded"},
					{"Host", sAddr}
				};
				var res = HttpHelper.HttpClientGetAsync(url, Headerdics, API.mycookiecontainer, API.RedirectUrl).Result;
				return res;
			}
			return "";
		}

#endregion

#region 撤回好友消息
		public static bool WithdrawFriendMsg(long SendQQId, long MsgReqId, long MsgTimeStamp, long MsgRandomId=0)
		{
		   if (MsgRandomId == 0) MsgRandomId = MsgTimeStamp;
		   WithdrawFriendMsgStuct WithdrawMsg = new WithdrawFriendMsgStuct
			{
				WithdrawFriendMsg = new WithdrawFriendMsgs
				{
					WithdrawFriendInfo = new WithdrawFriendInfos
					{
						thisQQ = API.SendQQ, SendQQ = SendQQId, ReqId = MsgReqId, MsgUid = 72057594617390532, SendTime = MsgTimeStamp, RandomId = MsgRandomId, SendInfo = new SendInfos
						{
							from = new Froms {QQId = SendQQId}
						}
					},
					types = 0, Flag = new Flags {flag = 0},
					EndFlag = 1
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, WithdrawMsg);
				Debug.Print("撤回好友消息结构:" + ms.ToArray().Length.ToString() + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", " "));
				var bytes = API.PackCmdHeader("PbMessageSvc.PbMsgWithDraw", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}
			return true;
		}
#endregion

	}


#region 用于获取历史消息结构
	[ProtoContract]
	public class SyncCoookie1Struct
	{
		[ProtoMember(1, IsRequired=true)]
		public int ErrorCode;
		[ProtoMember(2)]
		public string DeviceInfo;
		[ProtoMember(3)]
		public long TimeStamp;
		[ProtoMember(4)]
		public long TimeStamp1;
		[ProtoMember(5)]
		public long TimeStamp2;
		[ProtoMember(6)]
		public long TimeStamp3;
	}
	[ProtoContract]
	public class SyncCoookie2Struct
	{
		[ProtoMember(1, IsRequired=true)]
		public int ErrorCode;
		[ProtoMember(2, IsRequired=true)]
		public byte[] TimeInfo;
		[ProtoMember(3, IsRequired=true)]
		public int Field3;
		[ProtoMember(4, IsRequired=true)]
		public int Field4;
		[ProtoMember(5, IsRequired=true)]
		public int Field5;
		[ProtoMember(6, IsRequired=true)]
		public int Field6;
		[ProtoMember(7, IsRequired=true)]
		public int Field7;
		[ProtoMember(9, IsRequired=true)]
		public int Field9;
	}

#endregion

#region 置已读好友消息结构
	[ProtoContract]
	public class MakeReadedFriendMessage
	{
		[ProtoMember(3)]
		public ReadedFriendMsg ReadedMsg;
	}
	[ProtoContract]
	public class ReadedFriendMsg
	{
		[ProtoMember(1)]
		public byte[] SyncCoookie;
		[ProtoMember(2)]
		public QQFromInfos QQFromInfo;
	}
#endregion

#region 解析好友消息主结构
	[ProtoContract]
	public class FriendMsgStuct
	{
		[ProtoMember(1)]
		public int Field1;
		[ProtoMember(2)]
		public string Field2;
		[ProtoMember(3)]
		public SyncCoookies SyncCoookie;
		[ProtoMember(4)]
		public int Field4;
		[ProtoMember(5)]
		public List<MainMessage> MessageInfo;
		[ProtoMember(7)]
		public int Field6;
		[ProtoMember(8)]
		public string Field7;
		[ProtoMember(9)]
		public int Field8;
	}
	[ProtoContract]
	public class SyncCoookies
	{
		[ProtoMember(1)]
		public long timestamp1;
		[ProtoMember(2)]
		public long timestamp2;
		[ProtoMember(3)]
		public long Field3;
		[ProtoMember(4)]
		public long Field4;
		[ProtoMember(5)]
		public long Field5;
		[ProtoMember(9)]
		public long Field6;
		[ProtoMember(11)]
		public long Field7;
		[ProtoMember(12)]
		public long Field8;
		[ProtoMember(13)]
		public long timestamp3;
		[ProtoMember(14, IsRequired=true)]
		public int Field10;
	}
	[ProtoContract]
	public class MainMessage
	{
		[ProtoMember(1)]
		public long timestamp;
		[ProtoMember(2)]
		public long QQFromId;
		[ProtoMember(3)]
		public int Gender;
		[ProtoMember(4)]
		public QQInfos QQInfo;
		[ProtoMember(5)]
		public int Field5;
		[ProtoMember(6)]
		public int Field6;
		[ProtoMember(8)]
		public int Field8;
		[ProtoMember(9)]
		public int Field9;
	}
	[ProtoContract]
	public class QQInfos
	{
		[ProtoMember(1)]
		public QQFromInfos QQFromInfo;
		[ProtoMember(2)]
		public byte[] RequestId;
		[ProtoMember(3)]
		public MsgInfos MsgInfo;
	}
	[ProtoContract]
	public class QQFromInfos
	{
		[ProtoMember(1)]
		public long QQFromId;
		[ProtoMember(2)]
		public long QQId;
		[ProtoMember(5)]
		public long fromReq; //用于撤回
		[ProtoMember(6)]
		public long RecvTime; //用于撤回
		[ProtoMember(7)]
		public long DeviceId;
		[ProtoMember(23)]
		public long MsgId;
	}
	[ProtoContract]
	public class MsgInfos
	{
		[ProtoMember(1)]
		public MsgTextInfos MsgTextInfo;
		[ProtoMember(2)]
		public byte[] FileUploadInfo;
	}
	[ProtoContract]
	public class MsgTextInfos
	{
		[ProtoMember(1)]
		public FontInfo fonts;
		[ProtoMember(2)]
		public byte[] MsgContent;
		[ProtoMember(4)]
		public byte[] AudioContent;
	}
	[ProtoContract]
	public class FontInfo
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
	[ProtoContract]
	public class MsgContents
	{
		[ProtoMember(2)]
		public List<Contents> Content;
	}
	[ProtoContract]
	public class Contents
	{
		[ProtoMember(1)]
		public byte[] Content;
		[ProtoMember(12)]
		public XmlContents XmlContent;
	}
	[ProtoContract]
	public class XmlContents
	{
		[ProtoMember(1)]
		public byte[] Content;
		[ProtoMember(2)]
		public int flag;
	}
#endregion

#region 解析消息内容-文本/图片/视频消息
	[ProtoContract]
	public class MessageStruct
	{
		[ProtoMember(1)]
		public List<TextMessageContents> TextMessageContent; //文本消息
		[ProtoMember(2)]
		public List<BigFaces> BigFace; //表情消息
		[ProtoMember(4)]
		public List<PicMessageContents> PicMessageContent; //图片消息
		[ProtoMember(6)]
		public List<byte[]> HotPic; //热图消息
		[ProtoMember(9)]
		public MessageType Message_Type;
		[ProtoMember(12)]
		public byte[] XMLMessage; //xml卡片消息、发送位置、名片...
		[ProtoMember(19)]
		public List<VideoMessageContents> VideoMessageContent; //视频消息
		[ProtoMember(24)]
		public byte[] RedEnvelope; //转账、红包
		[ProtoMember(37)]
		public byte[] MessageOther;
		[ProtoMember(42)]
		public byte[] LiMiXiuMessage; //厘米秀
		[ProtoMember(45)]
		public ReplyMessages ReplyMessage; //回复消息
		[ProtoMember(51)]
		public byte[] JsonMessage; //JOSN消息、歌曲、签到...
		[ProtoMember(53)]
		public byte[] FlashPic; //戳一戳、闪照
	}
	[ProtoContract]
	public class TextMessageContents
	{
		[ProtoMember(1)]
		public string content;
		[ProtoMember(4)]
		public byte[] HotPicHash;
		[ProtoMember(12)]
		public XmlContents XmlContent;
	}
	[ProtoContract]
	public class BigFaces
	{
		[ProtoMember(1)]
		public int FaceId;
		[ProtoMember(2)]
		public byte[] FaceName;
		[ProtoMember(11)]
		public byte[] FaceHash;
	}
	[ProtoContract]
	public class ReplyMessages //带回复的消息
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
		public List<TextMessageContents> OldMsgContent;
		[ProtoMember(9)]
		public byte[] OldMsgId;
		[ProtoMember(8)]
		public byte[] OldMsgInfo;
	}
	[ProtoContract]
	public class MessageType
	{
		[ProtoMember(1)]
		public long types;
	}
	[ProtoContract]
	public class PicMessageContents
	{
		[ProtoMember(3)]
		public string PicGuid;
		[ProtoMember(12)]
		public string PicAddr;
	}
	[ProtoContract]
	public class VideoMessageContents
	{
		[ProtoMember(1)]
		public string VideoGuid;
		[ProtoMember(3)]
		public string VideoAddr;
		[ProtoMember(9)]
		public string VideoHash;
	}
#endregion

#region 解析消息内容-语音消息
	[ProtoContract]
	public class AudioMessageStruct
	{
		[ProtoMember(2)]
		public long FromId;
		[ProtoMember(5)]
		public string AudioName;
		[ProtoMember(20)]
		public string AudioUrl;
	}

#endregion

#region 解析文件上传内容-文件消息
	[ProtoContract]
	public class FileMessageStruct
	{
		[ProtoMember(1)]
		public FileInfos FileInfo;
		[ProtoMember(2)]
		public long uploadtime;
	}
	[ProtoContract]
	public class FileInfos
	{
		[ProtoMember(3)]
		public byte[] FileHash;
		[ProtoMember(5)]
		public string FileName;
		[ProtoMember(6)]
		public int FileSize;
		[ProtoMember(51)]
		public long FileRandId;
		[ProtoMember(52)]
		public long SendTime;
		[ProtoMember(57)]
		public string FileGuid;
	}
#endregion

#region 解析图片取GUID
	[ProtoContract]
	public class PicGuidStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int Amount;
		[ProtoMember(2, IsRequired=true)]
		public PicGuidInfos PicGuidInfo;
		[ProtoMember(3, IsRequired=true)]
		public int EndFlag;
	}
	[ProtoContract]
	public class PicGuidInfos
	{
		[ProtoMember(7)]
		public List<int> Ip;
		[ProtoMember(8)]
		public List<int> Port;
		[ProtoMember(9)]
		public byte[] uKey;
		[ProtoMember(10)]
		public string PicGuid;
	}
#endregion

#region 解析语音取Hash
	[ProtoContract]
	public class AudioHashStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int bir;
		[ProtoMember(2, IsRequired=true)]
		public int Field2;
		[ProtoMember(7, IsRequired=true)]
		public AudioHashInfos AudioHashInfo;
		[ProtoMember(14, IsRequired=true)]
		public AudioUrlInfos AudioUrlInfo;
		[ProtoMember(99999, IsRequired=true)]
		public string EndFlag;
	}
	[ProtoContract]
	public class AudioHashInfos
	{
		[ProtoMember(10, IsRequired=true)]
		public int Field10;
		[ProtoMember(20, IsRequired=true)]
		public string Field20;
		[ProtoMember(60, IsRequired=true)]
		public string IP;
		[ProtoMember(80, IsRequired=true)]
		public int Port;
		[ProtoMember(90, IsRequired=true)]
		public byte[] token;
		[ProtoMember(100, IsRequired=true)]
		public byte[] ukey;
		[ProtoMember(110, IsRequired=true)]
		public int Field110;
		[ProtoMember(120, IsRequired=true)]
		public int Field120;
		[ProtoMember(130, IsRequired=true)]
		public string Field130;
	}
	[ProtoContract]
	public class AudioUrlInfos
	{
		[ProtoMember(10, IsRequired=true)]
		public int Field10;
		[ProtoMember(20, IsRequired=true)]
		public string Status;
		[ProtoMember(30, IsRequired=true)]
		public AudioUrls AudioUrl;
	}
	[ProtoContract]
	public class AudioUrls
	{
		[ProtoMember(20, IsRequired=true)]
		public string IP;
		[ProtoMember(40, IsRequired=true)]
		public int Port;
		[ProtoMember(50, IsRequired=true)]
		public string Url;
	}
#endregion

#region 构造图片消息
	[ProtoContract]
	public class SendPicMsgStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int Amount;
		[ProtoMember(2, IsRequired=true)]
		public PicInfos PicInfo;
		[ProtoMember(10, IsRequired=true)]
		public int PicType;
	}
	[ProtoContract]
	public class PicInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public long thisQQ;
		[ProtoMember(2, IsRequired=true)]
		public long SendQQ;
		[ProtoMember(3, IsRequired=true)]
		public byte StartFlag;
		[ProtoMember(4, IsRequired=true)]
		public byte[] PicHash;
		[ProtoMember(5, IsRequired=true)]
		public int PicLengh;
		[ProtoMember(6, IsRequired=true)]
		public string PicName;
		[ProtoMember(7, IsRequired=true)]
		public int Field7;
		[ProtoMember(8, IsRequired=true)]
		public int Field8;
		[ProtoMember(10, IsRequired=true)]
		public int Field10;
		[ProtoMember(12, IsRequired=true)]
		public int Field12;
		[ProtoMember(13, IsRequired=true)]
		public int Field13;
		[ProtoMember(14, IsRequired=true)]
		public int PicWidth;
		[ProtoMember(15, IsRequired=true)]
		public int PicHeigh;
		[ProtoMember(16, IsRequired=true)]
		public int PicPix;
		[ProtoMember(17, IsRequired=true)]
		public string PicVer;
		[ProtoMember(21, IsRequired=true)]
		public int Field21;
		[ProtoMember(22, IsRequired=true)]
		public int Field22;
	}

#endregion

#region 发送好友图片消息
	[ProtoContract]
	public class SendFriendPicStruct
	{
		[ProtoMember(4)]
		public SendFriendPicInfos SendFriendPicInfo;

	}
	[ProtoContract]
	public class SendFriendPicInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public string PicName;
		[ProtoMember(2, IsRequired=true)]
		public long PicSize;
		[ProtoMember(3, IsRequired=true)]
		public string PicGuid;
		[ProtoMember(5, IsRequired=true)]
		public int PicPix;
		[ProtoMember(6)]
		public byte[] PicHash;
		[ProtoMember(7)]
		public byte[] PicHash2;
		[ProtoMember(8, IsRequired=true)]
		public int PicWidth;
		[ProtoMember(9, IsRequired=true)]
		public int PicHeigh;
		[ProtoMember(10)]
		public string PicGuid2;
		[ProtoMember(13, IsRequired=true)]
		public int field13;
		[ProtoMember(14, IsRequired=true)]
		public int field16;
		[ProtoMember(24, IsRequired=true)]
		public int field24;
		[ProtoMember(25, IsRequired=true)]
		public int field25;
	}
#endregion

#region 上传好友图片消息
	[ProtoContract]
	public class UploadFriendPicStruct
	{
		[ProtoMember(1)]
		public UploadFriendPicSendInfos UploadFriendPicSendInfo;
		[ProtoMember(2)]
		public UploadFriendPicFileInfos UploadFriendPicFileInfo;
	}
	[ProtoContract]
	public class UploadFriendPicSendInfos
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
	public class UploadFriendPicFileInfos
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
	public class SendAudioMsgStruct
	{
		[ProtoMember(1, IsRequired=true)]
		public int AudioType;
		[ProtoMember(2, IsRequired=true)]
		public int Field2;
		[ProtoMember(7)]
		public AudioInfos AudioInfo;
		[ProtoMember(14)]
		public GetAudioInfos GetAudioInfo;
		[ProtoMember(101, IsRequired=true)]
		public int Field101;
		[ProtoMember(102, IsRequired=true)]
		public int Field102;
		[ProtoMember(99999, IsRequired=true)]
		public AudioOtherInfos AudioOtherInfo;
	}
	[ProtoContract]
	public class AudioInfos
	{
		[ProtoMember(10, IsRequired=true)]
		public long thisQQ;
		[ProtoMember(20, IsRequired=true)]
		public long SendQQ;
		[ProtoMember(30, IsRequired=true)]
		public int StartFlag;
		[ProtoMember(40, IsRequired=true)]
		public int AudioSize;
		[ProtoMember(50, IsRequired=true)]
		public string AudioName;
		[ProtoMember(60, IsRequired=true)]
		public byte[] AudioHash;
	}
	[ProtoContract]
	public class GetAudioInfos
	{
		[ProtoMember(10, IsRequired=true)]
		public long thisQQ;
		[ProtoMember(20, IsRequired=true)]
		public byte[] token;
		[ProtoMember(30, IsRequired=true)]
		public int StartFlag;
	}
	[ProtoContract]
	public class AudioOtherInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public int Field1;
		[ProtoMember(2, IsRequired=true)]
		public int Field2;
		[ProtoMember(90300, IsRequired=true)]
		public int Field90300;
		[ProtoMember(90500)]
		public int Field90500;
		[ProtoMember(90600)]
		public int Field90600;
		[ProtoMember(90700, IsRequired=true)]
		public int Field90700;
		[ProtoMember(90800)]
		public int Field90800;
		[ProtoMember(91100)]
		public int Field91100;
	}
#endregion

#region 发送好友语音
	[ProtoContract]
	public class SendAudioStruct
	{
		[ProtoMember(4, IsRequired=true)]
		public SendAudioInfos SendAudioInfo;
	}
	[ProtoContract]
	public class SendAudioInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public int Field1;
		[ProtoMember(2, IsRequired=true)]
		public long thisQQ;
		[ProtoMember(3, IsRequired=true)]
		public string token;
		[ProtoMember(4, IsRequired=true)]
		public byte[] AudioHash;
		[ProtoMember(5, IsRequired=true)]
		public string AudioName;
		[ProtoMember(6, IsRequired=true)]
		public long AudioSize;
		[ProtoMember(7, IsRequired=true)]
		public byte[] uKey;
		[ProtoMember(9, IsRequired=true)]
		public long Field9;
		[ProtoMember(10, IsRequired=true)]
		public int Port;
		[ProtoMember(11, IsRequired=true)]
		public int Field11;
	}

#endregion


#region 发送好友消息结构
	[ProtoContract]
	public class SendFriendMsgStuct
	{
		[ProtoMember(1)]
		public SendQQFrom SendFromInfo;
		[ProtoMember(2)]
		public byte[] MsgId;
		[ProtoMember(3)]
		public MsgInfos MsgInfo;
		[ProtoMember(4)]
		public long RequestId; //req 可用于撤回
		[ProtoMember(5)]
		public long TimeStamp; //sendtime 可以用于撤回
		[ProtoMember(6)]
		public SyncCoookies SyncCoookie;
		[ProtoMember(8)]
		public int count;
	}
	[ProtoContract]
	public class SendQQFrom
	{
		[ProtoMember(1)]
		public FromInfos FromInfo;
	}
	[ProtoContract]
	public class FromInfos
	{
		[ProtoMember(1)]
		public long FromId;
	}

#endregion

#region 撤回好友消息结构
	[ProtoContract]
	public class WithdrawFriendMsgStuct
	{
		[ProtoMember(1)]
		public WithdrawFriendMsgs WithdrawFriendMsg;
	}
	[ProtoContract]
	public class WithdrawFriendMsgs
	{
		[ProtoMember(1)]
		public WithdrawFriendInfos WithdrawFriendInfo;
		[ProtoMember(2, IsRequired=true)]
		public int types;
		[ProtoMember(3)]
		public Flags Flag;
		[ProtoMember(4, IsRequired=true)]
		public int EndFlag;
	}
	[ProtoContract]
	public class WithdrawFriendInfos
	{
		[ProtoMember(1, IsRequired=true)]
		public long thisQQ;
		[ProtoMember(2, IsRequired=true)]
		public long SendQQ;
		[ProtoMember(3, IsRequired=true)]
		public long ReqId;
		[ProtoMember(4, IsRequired=true)]
		public long MsgUid;
		[ProtoMember(5, IsRequired=true)]
		public long SendTime;
		[ProtoMember(6, IsRequired=true)]
		public long RandomId;
		[ProtoMember(20)]
		public SendInfos SendInfo;
	}
	[ProtoContract]
	public class SendInfos
	{
		[ProtoMember(1)]
		public Froms from;
	}
	[ProtoContract]
	public class Froms
	{
		[ProtoMember(1, IsRequired=true)]
		public long QQId;
	}
	[ProtoContract]
	public class Flags
	{
		[ProtoMember(1, IsRequired=true)]
		public int flag;
	}
#endregion

}