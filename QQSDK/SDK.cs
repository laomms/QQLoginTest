using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QQSDK
{
  public class SDK
    {
        public delegate string DGetResult(string szGroupID, string szQQID, string szContent);
        public static DGetResult GetResult = null;
        public delegate string DGetLog(string szContent);
        public static DGetLog GetLog= null;
        public void GetResultCallBack(DGetResult GetResultFunc)
        {
            GetResult = GetResultFunc;
        }
        public void GetLogCallBack(DGetLog GetLogFunc)
        {
            GetLog= GetLogFunc;
        }
        private static void ExtractEmbeddedResource(string outputDir, string resourceLocation, List<string> files)
        {
            foreach (string file in files)
            {
                if (File.Exists(file))
                    break;
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file))
                {
                    using (FileStream fileStream = new FileStream(Path.Combine(outputDir, file), System.IO.FileMode.Create))
                    {
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
            }
        }
        public static void InitSdk(string userId, string password)
        {
            ExtractEmbeddedResource(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), Assembly.GetExecutingAssembly().GetName().Name.Replace("-", "_") + ".Files", new List<string> { "libeay32.dll", "node.dll", "test.amr" });
            API.Initialization(userId, password);
            API.TClient = new TCPIPClient(Dns.GetHostEntry("msfwifi.3g.qq.com").AddressList[0].ToString(), 8080);

        }
        public static void LoginIn(string userId, string password)
        {
            API.ThisQQ = long.Parse(userId);
            InitSdk(userId, password);
            API.QQ.loginState = (int)API.LoginState.Logining;
            API.TClient.SendData(Pack.LoginPackage());
        }
        public static void LoginOff()
        {
            API.TClient.SendData(Pack.PackOnlineStatus("StatSvc.register", 2));
            API.TClient.Dispose();
            API.TClient.DisConnect();
        }
        /// <summary>
        /// 发送好友消息.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="sendQQ">好友QQ号.</param>
        /// <param name="szMsg">发送的内容.</param>
        /// <returns>FriendWithdrawInfo.用于撤回消息</returns>
        public static API.FriendWithdrawInfo SendPrivateMsg(long thisQQ,long sendQQ,string szMsg )
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            return FriendMsg.SendFriendMsg(thisQQ,sendQQ, Encoding.UTF8.GetBytes(szMsg), API.MsgType.TextMsg);
        }
        /// <summary>
        /// 发送好友图片.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="sendQQ">好友QQ号.</param>
        /// <param name="PicData">图片的字节内容.</param>
        public static void SendPrivatePicMsg(long thisQQ, long sendQQ, byte[] PicData)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.SendFriendPic(thisQQ, sendQQ,  PicData);
        }
        /// <summary>
        /// 发送好友语音.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="sendQQ">好友QQ号.</param>
        /// <param name="AudioData">amr格式语音字节内容.</param>
        public static void SendPrivateAudioMsg(long thisQQ, long sendQQ, byte[] AudioData)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.SendFriendAudio(thisQQ, sendQQ, AudioData);
        }
        /// <summary>
        /// 发送好友XML消息.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="sendQQ">好友QQ号.</param>
        /// <param name="xmlMsg">xml文本内容.</param>
        public static void SendPrivateXmlMsg(long thisQQ, long sendQQ, string xmlMsg)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            var zipByte = API.CompressData(Encoding.UTF8.GetBytes(xmlMsg));
            FriendMsg.SendFriendMsg(thisQQ,sendQQ, zipByte, API.MsgType.XmlMsg);
        }
        /// <summary>
        /// 撤回消息.
        /// </summary>
        /// <param name="sendQQ">QQ号.</param>
        /// <param name="MsgReqId">消息ID.</param>
        /// <param name="MsgRandomId">识别ID.</param>
        /// <param name="MsgTimeStamp">发送时间.</param>
        public static void PrivateMsgWithdraw(long sendQQ, long MsgReqId, long MsgTimeStamp, long MsgRandomId=0)
        {
            var WithdrawInfo = new API.FriendWithdrawInfo();
            FriendMsg.WithdrawFriendMsg(sendQQ, MsgReqId, MsgTimeStamp, MsgRandomId);
        }
        /// <summary>
        /// 获取昵称.
        /// </summary>
        /// <param name="AnyQQ">要获取的QQ号.</param>
        public static void GetQQNick(long AnyQQ)
        {
            JceStructSDK.GetNick(AnyQQ);
        }
        /// <summary>
        /// 获取好友列表.
        /// </summary>
        /// <param name="thisQQ">字节的QQ号.</param>
        public static void GetFriendList(long thisQQ)
        {
            API.ThisQQ = thisQQ;
            JceStructSDK.GetFriendList(0, 30);
        }
        /// <summary>
        /// 发送群消息.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="groupId">群号.</param>
        /// <param name="szMsg">消息文本内容.</param>
        /// <param name="sendQQ">要@对方加入对方QQ号.</param>
        public static API.GroupWithdrawInfo SendGroupMsg(long thisQQ, long groupId, string szMsg, long sendQQ = 0)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.GroupId = groupId; 
            return GroupMsg.SendGroupMsg(thisQQ, groupId, Encoding.UTF8.GetBytes(szMsg), API.MsgType.TextMsg, sendQQ);
        }
        /// <summary>
        /// 发送群图片.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="groupId">群号.</param>
        /// <param name="PicData">图片字节内容.</param>
        /// <param name="sendQQ">要@对方加入对方QQ号.</param>
        public static void SendGroupPicMsg(long thisQQ,  long groupId, byte[] PicData, long sendQQ= 0 )
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.GroupId = groupId;
            API.SendGroupPic(thisQQ, sendQQ, groupId, PicData);
        }
        /// <summary>
        /// 发送群语音.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="groupId">群号.</param>
        /// <param name="AudioData">amr格式语音字节内容.</param>
        /// <param name="sendQQ">要@对方加入对方QQ号.</param>
        public static void SendGroupAudioMsg(long thisQQ, long groupId, byte[] AudioData, long sendQQ = 0)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.GroupId = groupId;
            API.SendGroupAudio(thisQQ, sendQQ,groupId , AudioData);
        }
        /// <summary>
        /// 撤回群消息.
        /// </summary>
        /// <param name="groupId">群号.</param>
        /// <param name="MsgReqId">消息ID.</param>
        /// <param name="MsgRandomId">辨认ID.</param>
        public static void GroupMsgWithdraw(long groupId, long MsgReqId, long MsgRandomId)
        {
            API.GroupId = groupId;
            var WithdrawInfo = new API.GroupWithdrawInfo();
            GroupMsg.WithdrawGroupMsg(groupId, MsgReqId, MsgRandomId);
        }
        /// <summary>
        /// 发送群XML消息.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="groupId">群号.</param>
        /// <param name="xmlMsg">xml文本内容.</param>
        /// <param name="sendQQ">要@对方加入对方QQ号.</param>
        public static void SendGroupXmlMsg(long thisQQ,  long groupId, string xmlMsg, long sendQQ = 0)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.GroupId = groupId;
            var zipByte = API.CompressData(Encoding.UTF8.GetBytes(xmlMsg));
            GroupMsg.SendGroupMsg(thisQQ,groupId, zipByte, API.MsgType.XmlMsg, sendQQ);
        }
        /// <summary>
        /// 获取群列表.
        /// </summary>
        /// <param name="groupId">群号.</param>
        /// <returns>返回群号集合</returns>
        public static List<string> GetGroupList(long groupId)
         {
            API.GroupId = groupId;
            byte[] retByte= JceStructSDK.GetGroupList();
            return  JceStructSDK.GetGrouplist(retByte);
        }
        public static void GetGroupMemberList(long groupId)
        {
            API.GroupId = groupId;
            JceStructSDK.GetGroupMemberList(groupId);
        }
        public static void GetGroupAdminList(long groupId)
        {
            API.GroupId = groupId;
            ProtoSDK.GetGroupAdminList(groupId);
        }
        public static void ShutDownGroup(long groupId)
        {
            API.GroupId = groupId;
            ProtoSDK.ShutAll(groupId, API.Mute.Open);
        }
        public static void ShutUpGroup(long groupId)
        {
            API.GroupId = groupId;
            ProtoSDK.ShutAll(groupId, API.Mute.Close);
        }
        public static void ShutUpMember(long GroupId, long HisId, long times)
        {
           ProtoSDK.ShutUp( GroupId,  HisId, times);
        }
        public static void RemoveGroupMember(long GroupId, long HisId, bool IsRefuseNext)
        {
            ProtoSDK.RemoveMember(GroupId, HisId, IsRefuseNext);
        }
        public static void DisbandGroup(long thisQQ, long GroupId)
        {
            JceStructSDK.DeleteGroup(thisQQ, GroupId);
        }
        public static void LeavingGroup(long sendQQ, long GroupId)
        {
            JceStructSDK.ExitGroup( sendQQ, GroupId);
        }
        public static String GetPKey(string domain)
        {
          return  UnPack.GetPSKey(domain);
        }
        public static String GetSKey()
        {
            return Encoding.UTF8.GetString(API.UN_Tlv.T120_skey);
        }
    }
}
