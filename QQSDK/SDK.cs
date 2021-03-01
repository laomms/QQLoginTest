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
        public static API.FriendWithdrawInfo SendPrivateMsg(long thisQQ,long sendQQ,string szMsg )
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            return FriendMsg.SendFriendMsg(thisQQ,sendQQ, Encoding.UTF8.GetBytes(szMsg), API.MsgType.TextMsg);
        }
        public static void SendPrivatePicMsg(long thisQQ, long sendQQ, byte[] PicData)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.SendFriendPic(thisQQ, sendQQ,  PicData);
        }
        public static void SendPrivateAudioMsg(long thisQQ, long sendQQ, byte[] AudioData)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.SendFriendAudio(thisQQ, sendQQ, AudioData);
        }
        public static void SendPrivateXmlMsg(long thisQQ, long sendQQ, string xmlMsg)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            var zipByte = API.CompressData(Encoding.UTF8.GetBytes(xmlMsg));
            FriendMsg.SendFriendMsg(thisQQ,sendQQ, zipByte, API.MsgType.XmlMsg);
        }
        public static void PrivateMsgWithdraw( long sendQQ)
        {
            API.ThisQQ = sendQQ;
            var WithdrawInfo = new API.FriendWithdrawInfo();
            FriendMsg.WithdrawFriendMsg(API.QQ.LongQQ, sendQQ, API.FriendWithdraw.MsgReqId, API.FriendWithdraw.MsgRandomId, API.FriendWithdraw.MsgTimeStamp);
        }
        public static void GetQQNick(long sendQQ)
        {
            API.ThisQQ = sendQQ;
            JceStructSDK.GetNick(sendQQ);
        }
        public static void GetFriendList(long thisQQ)
        {
            API.ThisQQ = thisQQ;
            JceStructSDK.GetFriendList(0, 30);
        }

        public static API.GroupWithdrawInfo SendGroupMsg(long thisQQ, long groupId, string szMsg, long sendQQ = 0)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.GroupId = groupId; 
            return GroupMsg.SendGroupMsg(thisQQ, groupId, Encoding.UTF8.GetBytes(szMsg), API.MsgType.TextMsg, sendQQ);
        }
        public static void SendGroupPicMsg(long thisQQ,  long groupId, byte[] PicData, long sendQQ= 0 )
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.GroupId = groupId;
            API.SendGroupPic(thisQQ, sendQQ, groupId, PicData);
        }
        public static void SendGroupAudioMsg(long thisQQ, long groupId, byte[] AudioData, long sendQQ = 0)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.GroupId = groupId;
            API.SendGroupAudio(thisQQ, sendQQ,groupId , AudioData);
        }
        public static void GroupMsgWithdraw(long groupId)
        {
            API.GroupId = groupId;
            var WithdrawInfo = new API.GroupWithdrawInfo();
            GroupMsg.WithdrawGroupMsg(groupId, WithdrawInfo.MsgReqId, WithdrawInfo.MsgRandomId);
        }
        public static void SendGroupXmlMsg(long thisQQ,  long groupId, string xmlMsg, long sendQQ = 0)
        {
            API.ThisQQ = thisQQ;
            API.ThisQQ = sendQQ;
            API.GroupId = groupId;
            var zipByte = API.CompressData(Encoding.UTF8.GetBytes(xmlMsg));
            GroupMsg.SendGroupMsg(thisQQ,groupId, zipByte, API.MsgType.XmlMsg, sendQQ);
        }
         public static void GetGroupList(long groupId)
         {
            API.GroupId = groupId;
            JceStructSDK.GetGroupList();
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
    }
}
