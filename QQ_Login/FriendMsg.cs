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
	public class FriendMsg
	{
		#region 解析好友消息
		public static bool DeFriendMsg(byte[] byteIn)
		{

			byteIn = byteIn.Skip(4).ToArray();
			using (MemoryStream ms = new MemoryStream(byteIn))
			{
				try
				{
					var result = Serializer.Deserialize<FriendMsgStuct>(ms);
					long qqid = result.MessageInfo[0].QQInfo.QQFromInfo.QQId;
					long qqfromid = result.MessageInfo[0].QQInfo.QQFromInfo.QQFromId;
					try
					{
						var msgByte = result.MessageInfo[0].QQInfo.MsgInfo.MsgTextInfo.MsgContent;
						Debug.Print("朋友消息内容:" + "\r\n" + BitConverter.ToString(msgByte).Replace("-", " "));
						if (msgByte[0] == 0xA) //文字消息
						{
							using (MemoryStream mStream = new MemoryStream(msgByte))
							{
								var MsgResult = Serializer.Deserialize<MessageStruct>(mStream);
								var msg = MsgResult.Message_Content.content;
								Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + "[" + qqfromid.ToString() + "]" + msg + "\r\n")));
							}
						}
						else if (msgByte[0] == 0x12) //表情消息
						{

						}
						else if (msgByte[0] == 0x22) //图片消息
						{
							using (MemoryStream mStream = new MemoryStream(msgByte))
							{
								var MsgResult = Serializer.Deserialize<PicMessageStruct>(mStream);
								var msg = MsgResult.PicMessage_Content.PicContent;
								Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + "[" + qqfromid.ToString() + "]" + msg + "\r\n")));
							}
						}
					}
					catch (Exception ex)
					{

					}
					var MsgTimeStamp = result.MessageInfo[0].QQInfo.QQFromInfo.timestamp6;
					using (var mMemoryStream = new MemoryStream())
					{
						Serializer.Serialize(mMemoryStream, result.SyncCoookie);
						Debug.Print("Serializer" + "\r\n" + BitConverter.ToString(mMemoryStream.ToArray()).Replace("-", ""));
						MakeReadedFriendMsg(qqfromid, mMemoryStream.ToArray(), MsgTimeStamp);
					}

				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message.ToString());
				}
			}
			return true;
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
						timestamp6 = MsgTimeStamp
					},
					SyncCoookie = SyncCoookies
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, ReadedMsg);
				Debug.Print("ReadedMsg" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				var bytes = SDK.PackCmdHeader("PbMessageSvc.PbMsgReadedReport", ms.ToArray());
				TCPIPClient.SendData(SDK.PackAllHeader(bytes));
			}
			return true;
		}
		#endregion

		#region 发送好友消息
		public static bool SendFriendMsg(long QQFromId, string Message, byte MsgType)
		{
			byte[] msgByte = null;
			if (MsgType == 0xA) //文字消息
			{
				MessageStruct msg = new MessageStruct
				{
					Message_Content = new MessageContent { content = Message }
				};
				using (MemoryStream mStream = new MemoryStream())
				{
					Serializer.Serialize(mStream, msg);
					msgByte = mStream.ToArray();
				}
			}
			else if (MsgType == 0x12) //文字消息
			{

			}
			else if (MsgType == 0x22) //图片消息
			{
				PicMessageStruct msg = new PicMessageStruct
				{
					PicMessage_Content = new PicMessageContent { PicContent = Message }
				};
				using (MemoryStream mStream = new MemoryStream())
				{
					Serializer.Serialize(mStream, msg);
					msgByte = mStream.ToArray();
				}
			}

			SendFriendMsgStuct SendMsg = new SendFriendMsgStuct
			{
				SendFromInfo = new SendQQFrom
				{
					FromInfo = new FromInfos { FromId = QQFromId }
				},
				MsgId = new byte[] { 8, 1, 0x10, 0, 0x18, 0 },
				MsgInfo = new MsgInfos
				{
					MsgTextInfo = new MsgTextInfos { MsgContent = msgByte }
				},
				RequestId = DataList.QQ.mRequestID,
				TimeStamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10)),
				count = 1,
				SyncCoookie = DataList.QQ.SyncCoookies
			};


			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, SendMsg);
				Debug.Print("SendMsg" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				var bytes = SDK.PackCmdHeader("MessageSvc.PbSendMsg", ms.ToArray());
				TCPIPClient.SendData(SDK.PackAllHeader(bytes));
			}
			return true;
		}

		#endregion
	}
}

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
	public long RequestId;
	[ProtoMember(5)]
	public long TimeStamp;
	[ProtoMember(6)]
	public byte[] SyncCoookie;
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

#region 解析好友消息结构
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
	public List<MainMessage> MessageInfo { get; set; }
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
	[ProtoMember(14)]
	public int Field10;
}

[ProtoContract]
public class MainMessage
{
	[ProtoMember(1)]
	public long timestamp { get; set; }
	[ProtoMember(2)]
	public long QQFromId { get; set; }
	[ProtoMember(3)]
	public int Gender { get; set; }
	[ProtoMember(4)]
	public QQInfos QQInfo { get; set; }
	[ProtoMember(5)]
	public int Field5 { get; set; }
	[ProtoMember(6)]
	public int Field6 { get; set; }
	[ProtoMember(8)]
	public int Field8 { get; set; }
	[ProtoMember(9)]
	public int Field9 { get; set; }
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
	[ProtoMember(3)]
	public long grade;
	[ProtoMember(4)]
	public long emo;
	[ProtoMember(5)]
	public long req;
	[ProtoMember(6)]
	public long timestamp6;
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
}
[ProtoContract]
public class MsgTextInfos
{
	[ProtoMember(1)]
	public FontInfo fonts;
	[ProtoMember(2)]
	public byte[] MsgContent;
}
[ProtoContract]
public class FontInfo
{
	[ProtoMember(1)]
	public long Field1;
	[ProtoMember(2)]
	public long timestamp;
	[ProtoMember(3)]
	public long Field3;
	[ProtoMember(4)]
	public long Field4;
	[ProtoMember(5)]
	public long fontsize;
	[ProtoMember(6)]
	public long Field6;
	[ProtoMember(7)]
	public long color;
	[ProtoMember(8)]
	public long bold;
	[ProtoMember(9)]
	public string font;
}
#endregion
#region 解析消息内容-文本消息
[ProtoContract]
public class MessageStruct
{
	[ProtoMember(1)]
	public MessageContent Message_Content { get; set; } 
	[ProtoMember(9)]
	public MessageType Message_Type { get; set; } 
	[ProtoMember(37)]
	public MessageOther Message_Other { get; set; } 
}
[ProtoContract]
public class MessageContent
{
	[ProtoMember(1)]
	public string content;
}
[ProtoContract]
public class MessageType
{
	[ProtoMember(1)]
	public long types;
}
[ProtoContract]
public class MessageOther
{
	[ProtoMember(25)]
	public int Field1;
	[ProtoMember(30)]
	public int Field2;
	[ProtoMember(31)]
	public int Field3;
	[ProtoMember(34)]
	public int Field4;
	[ProtoMember(73)]
	public string Field5;
}

#endregion
#region 解析消息内容-图片消息
[ProtoContract]
public class PicMessageStruct
{
	[ProtoMember(4)]
	public PicMessageContent PicMessage_Content { get; set; }

}
[ProtoContract]
public class PicMessageContent
{
	[ProtoMember(1)]
	public string PicName;
	[ProtoMember(2)]
	public long PicSize;
	[ProtoMember(3)]
	public string PicContent;
	[ProtoMember(4)]
	public string PicHash;
	[ProtoMember(8)]
	public int PicWidth;
	[ProtoMember(9)]
	public int PicHeigh;
	[ProtoMember(9)]
	public int PicGuid;
}

#endregion

