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

	public class GroupMsg
	{
		#region 解析群消息
		public static bool DeGroupMsg(byte[] byteIn)
		{

			var byteLen = BitConverter.ToInt32(byteIn.Take(4).ToArray().Reverse().ToArray(), 0);
			byteIn = byteIn.Skip(4).ToArray();
			using (MemoryStream ms = new MemoryStream(byteIn))
			{
				try
				{
					var result = Serializer.Deserialize<GroupMsgStuct>(ms);
					long QQId = result.GroupQQInfo.GroupQQInfo.QQId;
					long QQFromId = result.GroupQQInfo.GroupQQInfo.QQFromId;
					long GroupId = result.GroupQQInfo.GroupQQInfo.GroupInfo.GroupId;
					string GroupName = result.GroupQQInfo.GroupQQInfo.GroupInfo.GroupName;
					try
					{
						string Msg = result.GroupQQInfo.GroupMessageInfo.GroupMsgInfo.GrooupMsgContent.GroupMessage_Content.content;
						Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + "[" + GroupName + "]" + "[" + QQFromId.ToString() + "]" + Msg + "\r\n")));
					}
					catch
					{

					}
					var MsgId = result.GroupQQInfo.GroupQQInfo.MsgId;
					MakeReadedGroupMsg(GroupId, MsgId);
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message.ToString());
				}
			}
			return true;
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
				Debug.Print("ReadedMsg" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				var bytes = SDK.PackCmdHeader("PbMessageSvc.PbMsgReadedReport", ms.ToArray());
				TCPIPClient.SendData(SDK.PackAllHeader(bytes));
			}
			return true;
		}
		#endregion

		#region 发送群消息
		public static bool SendGroupMsg(long GroupId, string Message)
		{
			SendGroupMsgStuct SendGroupMessage = new SendGroupMsgStuct
			{
				GroupInformation = new GroupInformations
				{
					GroupFrom = new GroupFroms { GroupId = GroupId }
				},
				GroupMsgId = new byte[] { 8, 1, 0x10, 0, 0x18, 0 },
				GroupMsgInfo = new GroupMessageInfos
				{
					GroupMsgInfo = new GroupMsgInfos
					{
						GrooupMsgContent = new GroupMessageStruct
						{
							GroupMessage_Content = new GroupMessageContent { content = Message }
						}
					}
				},
				RequestId = DataList.QQ.mRequestID,
				GroupTimeStamp = 0x10000000 + DataList.QQ.mRequestID,
				Groupcount = 1
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, SendGroupMessage);
				Debug.Print("SendGroupMessage" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				var bytes = SDK.PackCmdHeader("MessageSvc.PbSendMsg", ms.ToArray());
				TCPIPClient.SendData(SDK.PackAllHeader(bytes));
			}
			return true;
		}

		#endregion

	}
}

#region 发送群消息结构


[ProtoContract]
public class SendGroupMsgStuct
{
	[ProtoMember(1)]
	public GroupInformations GroupInformation;
	[ProtoMember(2)]
	public byte[] GroupMsgId;
	[ProtoMember(3)]
	public GroupMessageInfos GroupMsgInfo;
	[ProtoMember(4)]
	public long RequestId;
	[ProtoMember(5)]
	public long GroupTimeStamp;
	[ProtoMember(8)]
	public int Groupcount;
}
[ProtoContract]
public class GroupInformations
{
	[ProtoMember(2)]
	public GroupFroms GroupFrom;
}
[ProtoContract]
public class GroupFroms
{
	[ProtoMember(1)]
	public long GroupId;
}
#endregion
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
#region 解析群消息结构
[ProtoContract]
	public class GroupMsgStuct
	{
		[ProtoMember(1)]
		public GroupQQInfos GroupQQInfo;
		[ProtoMember(2)]
		public long Field2;
		[ProtoMember(3)]
		public string Field3;
	}
	[ProtoContract]
	public class GroupQQInfos
	{
		[ProtoMember(1)]
		public GroupQQInfoes GroupQQInfo;
		[ProtoMember(2)]
		public byte[] GroupMsgId;
		[ProtoMember(3)]
		public GroupMessageInfos GroupMessageInfo;
	}
	[ProtoContract]
	public class GroupQQInfoes
	{
		[ProtoMember(1)]
		public long QQFromId;
		[ProtoMember(2)]
		public long QQId;
		[ProtoMember(3)]
		public int Field3;
		[ProtoMember(4)]
		public int Field4;
		[ProtoMember(5)]
		public long MsgId;
		[ProtoMember(6)]
		public long timestamp;
		[ProtoMember(7)]
		public long DeviceId;
		[ProtoMember(9)]
		public GroupInfos GroupInfo;
		[ProtoMember(10)]
		public int Field10;
		[ProtoMember(11)]
		public int Field11;
		[ProtoMember(12)]
		public int Field12;
		[ProtoMember(13)]
		public int Field13;
		[ProtoMember(17)]
		public int Field17;
	}
	[ProtoContract]
	public class GroupInfos
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
		public GroupMessageStruct GrooupMsgContent;
	}
	[ProtoContract]
	public class FontInfos
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

	[ProtoContract]
	[ProtoInclude(21, typeof(GroupMessageStruct))]
	public interface GroupInterface
	{
		GroupMessageContent GroupMsgContent {get; set;}
		GroupMessageType GroupMsgType {get; set;}
		GroupMessageOther GroupMsgOther {get; set;}
		GroupAdminInformation GroupAdminInfo {get; set;}
	}

	[ProtoContract]
	public class GroupMessageStruct : GroupInterface
	{
		GroupMessageContent GroupInterface.GroupMsgContent
		{
			get
			{
				return this.GroupMessage_Content;
			}
			set
			{
				this.GroupMessage_Content = value;
			}
		}
		[ProtoMember(1)]
		public GroupMessageContent GroupMessage_Content {get; set;}
		GroupMessageType GroupInterface.GroupMsgType
		{
			get
			{
				return this.GroupMessage_Type;
			}
			set
			{
				this.GroupMessage_Type = value;
			}
		}
		[ProtoMember(9)]
		public GroupMessageType GroupMessage_Type {get; set;}
		GroupMessageOther GroupInterface.GroupMsgOther
		{
			get
			{
				return this.GroupMessage_Other;
			}
			set
			{
				this.GroupMessage_Other = value;
			}
		}
		[ProtoMember(37)]
		public GroupMessageOther GroupMessage_Other {get; set;}
		GroupAdminInformation GroupInterface.GroupAdminInfo
		{
			get
			{
				return this.GroupAdmin_Information;
			}
			set
			{
				this.GroupAdmin_Information = value;
			}
		}
		[ProtoMember(16)]
		public GroupAdminInformation GroupAdmin_Information {get; set;}
	}
	[ProtoContract]
	public class GroupMessageContent
	{
		[ProtoMember(1)]
		public string content;
		[ProtoMember(2)]
		public long field2;
	}
	[ProtoContract]
	public class GroupMessageType
	{
		[ProtoMember(1)]
		public long Field1;
		[ProtoMember(2)]
		public long Field2;
	}

	[ProtoContract]
	public class GroupAdminInformation
	{
		[ProtoMember(2)]
		public string AdminNick;
		[ProtoMember(3)]
		public long Field2;
		[ProtoMember(4)]
		public long Field3;
		[ProtoMember(5)]
		public long Field4;
	}
	[ProtoContract]
	public class GroupMessageOther
	{
		[ProtoMember(10)]
		public long Field1;
		[ProtoMember(12)]
		public long Field2;
		[ProtoMember(13)]
		public long Field3;
		[ProtoMember(19)]
		public byte[] Field4;
	}
#endregion
