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
using System.Text.RegularExpressions;
using System.Threading;
using ProtoBuf;
//解包

namespace QQSDK
{
	public class UnPack
	{
		private static int SendMobile = 0;
		public static void UpdateSyncCoookies()
		{
			var timestamp = long.Parse(Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString().Substring(0, 10));
			SyncCoookies SyncTimeStruct = new SyncCoookies
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
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, SyncTimeStruct);
				API.QQ.SyncCoookies = ms.ToArray();
			}
		}
		#region 解析包体
		public static byte[] UnPackReceiveData(byte[] ReceiveData)
		{
			if (ReceiveData.Length == 0)
			{
				return null;
			}
			byte[] teaBytes = null;
			try
			{
				UpdateSyncCoookies();
				Debug.Print("收到包:" + ReceiveData.Length.ToString() + "\r\n" + BitConverter.ToString(ReceiveData).Replace("-", " "));
				var packType = ReceiveData.Skip(7).Take(1).ToArray()[0];
				var encrptType = ReceiveData.Skip(8).Take(1).ToArray()[0];
				int pos = API.SearchBytes(ReceiveData, API.QQ.UTF8);

				var bytes = ReceiveData.Skip(pos + API.QQ.UTF8.Length).ToArray();
				teaBytes = bytes;
				HashTea Hash = new HashTea();

				if (packType == 0xB && packType == 9)
				{
					if (encrptType == 1)
					{
						bytes = Hash.UNHashTEA(bytes, API.QQ.key, 0, true);
					}
					else if (encrptType == 2)
					{
						bytes = Hash.UNHashTEA(bytes, API.QQ.shareKey, 0, true);
					}
				}
				else if (packType == 0xA && packType == 8)
				{
					if (encrptType == 1)
					{
						bytes = Hash.UNHashTEA(bytes, API.QQ.shareKey, 0, true);
					}
					else if (encrptType == 2)
					{
						bytes = Hash.UNHashTEA(bytes, API.QQ.key, 0, true);
					}
				}
				else if (packType == 0xA && packType == 10)
				{
					if (encrptType == 1)
					{
						bytes = Hash.UNHashTEA(bytes, API.UN_Tlv.T305_SessionKey, 0, true);
					}
					else if (encrptType == 2)
					{
						bytes = Hash.UNHashTEA(bytes, API.QQ.key, 0, true);
					}
				}
				else
				{
					bytes = Hash.UNHashTEA(bytes, API.UN_Tlv.T305_SessionKey, 0, true);
				}

				Debug.Print("解密后:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));

				//取出第一层包头中的包长度值
				if (BitConverter.ToInt16(bytes.Take(2).ToArray(), 0) != 0)
				{
					Debug.Print("未解析" + "\r\n" + BitConverter.ToString(teaBytes).Replace("-", " "));
					Debug.Print("ShareKey" + "\r\n" + BitConverter.ToString(API.QQ.shareKey).Replace("-", " "));
					Debug.Print("SessionKey" + "\r\n" + BitConverter.ToString(API.UN_Tlv.T305_SessionKey).Replace("-", " "));
					return null;
				}
				var head1_len = BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0);
				byte[] bodyBytes = bytes.Skip(head1_len).ToArray();
				Debug.Print("主包体:" + bodyBytes.Length.ToString() + "\r\n" + BitConverter.ToString(bodyBytes).Replace("-", " "));
				if (head1_len > 4)
				{
					bytes = bytes.Skip(4).Take(head1_len - 4).ToArray();
					bytes = bytes.Skip(4).ToArray();
					if (bytes.Skip(4).Take(4).ToArray() == new byte[] { 0, 0, 0, 0 })
					{
						bytes = bytes.Skip(4).ToArray();
					}
					else
					{
						var head3_len = BitConverter.ToInt32(bytes.Skip(4).Take(4).ToArray().Reverse().ToArray(), 0);
						bytes = bytes.Skip(head3_len + 4).ToArray();
					}
					var str_len = BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0);
					if (str_len > 4)
					{
						var serviceCmd = Encoding.UTF8.GetString(bytes.Skip(4).ToArray(), 0, str_len - 4);
						if (serviceCmd.Contains("wtlogin.login"))
						{
							Debug.Print("收到命令:wtlogin.login");
							//解析登录包体
							var status = Un_Pack_Login(bodyBytes);
							Debug.Print("status:" + status.ToString());
							if (status == 1)
							{
								Debug.Print("wtlogin.login");
								SDK.GetLog("登录成功");								
								//发上线包
								API.TClient.SendData(Pack.PackOnlineStatus("StatSvc.register", 0));
							}
							else if (status == 0)
							{
								Microsoft.VisualBasic.Interaction.MsgBox(API.getLastError(), (Microsoft.VisualBasic.MsgBoxStyle)((int)Microsoft.VisualBasic.Constants.vbInformation + (int)Microsoft.VisualBasic.Constants.vbMsgBoxSetForeground + (int)Microsoft.VisualBasic.Constants.vbSystemModal + (int)Microsoft.VisualBasic.Constants.vbCritical + (int)Microsoft.VisualBasic.Constants.vbInformation), "登录失败");
							}
							else if (status == 2)
							{
								API.QQ.loginState = (int)API.LoginState.Logining;
								if (API.UN_Tlv.T143_token_A2 != null && API.QQ.shareKey != null && API.UN_Tlv.T10A_token_A4 != null)
								{
									API.TClient.SendData(Pack.PackOnlineStatus("StatSvc.register", 1));
								}
							}
						}
						else if (serviceCmd.Contains("PushService.register"))
						{
							Debug.Print("服务器注册成功:PushService.register");
						}
						else if (serviceCmd.Contains("RegPrxySvc.PushParam"))
						{
							Debug.Print("RegPrxySvc.PushParam命令");
						}
						else if (serviceCmd.Contains("ConfigPushSvc.PushDomain"))
						{
							Debug.Print("成功登录服务器:ConfigPushSvc.PushDomain");
							SDK.GetLog("上线成功");						
						}
						else if (serviceCmd.Contains("OnlinePush.ReqPush"))
						{
							Debug.Print("OnlinePush.ReqPush系统推送消息、撤回、加好友等");
						}
						else if (serviceCmd.Contains("ConfigPushSvc.PushReq"))
						{
							Debug.Print("回执ConfigPushSvc.PushReq");
							JceStructSDK.ReplyConfigPushSvc(bodyBytes, API.QQ.mRequestID);
						}
						else if (serviceCmd.Contains("StatSvc.SimpleGet"))
						{
							Debug.Print("收到心跳包");
							SDK.GetHeartBeatResult(true);
						}
						else if (serviceCmd.Contains("MessageSvc.PushNotify"))
						{
							Debug.Print("通知好友消息:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
							bytes = bytes.Reverse().ToArray();
							var code = bytes.Skip(15).Take(4).ToArray().Reverse().ToArray();
							var sendByte = Pack.PackFriendMsg(code); //组包获取好友消息
							API.TClient.SendData(sendByte);
						}
						else if (serviceCmd.Contains("MessageSvc.PbGetMsg"))
						{
							Debug.Print("收到好友消息");
							FriendMsg.ParsingFriendMsg(bodyBytes);
						}
						else if (serviceCmd.Contains("OnlinePush.PbPushGroupMsg"))
						{
							Debug.Print("群聊消息");
							GroupMsg.ParsingGroupMsg(bodyBytes);
						}
						else if (serviceCmd.Contains("MessageSvc.RequestPushStatus"))
						{
							Debug.Print("上线下线状态改变");
						}
						else if (serviceCmd.Contains("MessageSvc.PushReaded"))
						{
							Debug.Print("已读私人消息");
						}
						else if (serviceCmd.Contains("OnlinePush.PbC2CMsgSync"))
						{
							Debug.Print("发私人消息");
						}
						else if (serviceCmd.Contains("RegPrxySvc.PbGetGroupMsg"))
						{
							Debug.Print("群消息");
						}
						else if (serviceCmd.Contains("RegPrxySvc.PullGroupMsgSeq"))
						{
							Debug.Print("拉取群消息Seq");
						}
						else if (serviceCmd.Contains("MessageSvc.PbGetOneDayRoamMsg"))
						{
							Debug.Print("解析漫游消息");
						}
						else if (serviceCmd.Contains("RegPrxySvc.PbSyncMsg"))
						{
							Debug.Print("同步历史消息");
						}
						else if (serviceCmd.Contains("SSO.HelloPush"))
						{
							Debug.Print("回执SSO.HelloPush");
							Pack.ReplySSOHelloPush(bodyBytes, API.QQ.mRequestID);
						}
						else if (serviceCmd.Contains("StatSvc.SvcReqMSFLoginNotify"))
						{
							Debug.Print("平板上/下线通知");
						}
						else if (serviceCmd.Contains("MessageSvc.RequestPushStatus"))
						{
							Debug.Print("电脑上/下线通知");
						}
						else if (serviceCmd.Contains("StatSvc.ReqMSFOffline"))
						{
							Debug.Print("异常提醒");
						}
						else if (serviceCmd.Contains("StatSvc.QueryHB"))
						{
							Debug.Print("StatSvc.QueryHB");
						}
						else if (serviceCmd.Contains("MessageSvc.PushForceOffline"))
						{
							Debug.Print("被顶下线");
							SDK.GetLog("被顶下线");
						}
						else if (serviceCmd.Contains("MessageSvc.PbSendMsg"))
						{
							Debug.Print("递增发送信息");
							API.QQ.mRequestID = API.QQ.mRequestID + 1;
						}
						else if (serviceCmd.Contains("OnlinePush.PbC2CMsgSync"))
						{
							Debug.Print("同步递增发送信息");
						}
						else if (serviceCmd.Contains("OnlinePush.PbPushDisMsg"))
						{
							Debug.Print("讨论组消息");
						}
						else if (serviceCmd.Contains("OnlinePush.PbPushTransMsg"))
						{
							Debug.Print("管理员变动");
						}
						else if (serviceCmd.Contains("friendlist.GetTroopListReqV2"))
						{
							Debug.Print("群列表");
							JceStructSDK.GetGrouplist(bodyBytes);
						}
						else if (serviceCmd.Contains("friendlist.getFriendGroupList"))
						{
							Debug.Print("好友列表");
							JceStructSDK.GetFriendlist(bodyBytes);
						}
						else if (serviceCmd.Contains("friendlist.GetTroopMemberList"))
						{
							Debug.Print("群成员列表");
							JceStructSDK.GetGroupMemberlist(bodyBytes);
						}
						else if (serviceCmd.Contains("OidbSvc.0x899_0"))
						{
							Debug.Print("群管理列表");
							ProtoSDK.GetGroupAdminlist(bodyBytes);
						}
						else if (serviceCmd.Contains("ProfileService.Pb.ReqSystemMsgNew.Friend"))
						{
							Debug.Print("新好友提醒");
						}
						else if (serviceCmd.Contains("ProfileService.Pb.ReqNextSystemMsg.Friend"))
						{
							Debug.Print("加好友消息提醒");
						}
						else if (serviceCmd.Contains("ProfileService.Pb.ReqSystemMsgNew.Group"))
						{
							Debug.Print("新群提醒");
						}
						else if (serviceCmd.Contains("QualityTest.PushList"))
						{
							Debug.Print("回执QualityTest.PushList");
							Pack.ReplyQualityTest(API.QQ.mRequestID + 1);
						}
						else if (serviceCmd.Contains("OnlinePush.SidTicketExpired"))
						{
							Debug.Print("回执门票过期");
						}
						else if (serviceCmd.Contains("PubAccountSvc.get_follow_list"))
						{
							Debug.Print("获取列表");
						}
						else if (serviceCmd.Contains("account.RequestQueryQQMobileContactsV3"))
						{
							Debug.Print("查询内容");
						}
						else if (serviceCmd.Contains("ProfileService.GetSimpleInfo"))
						{
							Debug.Print("获取QQ资料信息");
							JceStructSDK.GetSimpleInfo(bodyBytes);
						}
						else if (serviceCmd.Contains("StatSvc.register"))
						{
							Debug.Print("注册上线:StatSvc.register");
						}
						else if (serviceCmd.Contains("PbMessageSvc.PbMsgReadedReport"))
						{
							Debug.Print("已读消息");
						}
						else if (serviceCmd.Contains("OidbSvc.0x59f"))
						{
							Debug.Print("OidbSvc.0x59f命令");
							JceStructSDK.GetRegSync_Info();
						}
						else if (serviceCmd.Contains("RegPrxySvc.infoSync"))
						{
							Debug.Print("RegPrxySvc.infoSync命令");
						}
						else if (serviceCmd.Contains("RegPrxySvc.GetMsgV2"))
						{
							Debug.Print("RegPrxySvc.GetMsgV2命令");
						}
						else if (serviceCmd.Contains("LongConn.OffPicUp"))
						{
							Debug.Print("收到图片反馈LongConn.OffPicUp");
							FriendMsg.SendFriendMsg(API.ThisQQ, API.SendQQ, bodyBytes, API.MsgType.PicMsg);
						}
						else if (serviceCmd.Contains("ImgStore.GroupPicUp"))
						{
							Debug.Print("收到群图片反馈ImgStore.GroupPicUp");
							GroupMsg.SendGroupMsg(API.ThisQQ , API.GroupId, bodyBytes, API.MsgType.PicMsg,API.SendQQ);
						}
						else if (serviceCmd.Contains("PttStore.GroupPttUp"))
						{
							Debug.Print("收到群语音反馈PttStore.GroupPttUp");
							GroupMsg.SendGroupAudio(bodyBytes, API.FileHash);
						}
						else if (serviceCmd.Contains("PttCenterSvr.pb_pttCenter_CMD_REQ_APPLY_UPLOAD-500"))
						{
							Debug.Print("收到好友语音反馈PttCenterSvr.pb_pttCenter_CMD_REQ_APPLY_UPLOAD-500");
							FriendMsg.SendFriendAudio(API.ThisQQ, API.SendQQ , bodyBytes, API.FileHash);
						}
						else if (serviceCmd.Contains("PttCenterSvr.pb_pttCenter_CMD_REQ_APPLY_DOWNLOAD-1200"))
						{
							Debug.Print("取语音下载地址PttCenterSvr.pb_pttCenter_CMD_REQ_APPLY_DOWNLOAD-1200");
							//FriendMsg.GetFriendAudioUrl(bodyBytes)
						}
						else if (serviceCmd.Contains("PbMessageSvc.PbMsgWithDraw"))
						{
							Debug.Print("撤回消息反馈PbMessageSvc.PbMsgWithDraw");
						}
						else if (serviceCmd.Contains("SharpSvr.s2c"))
						{
							Debug.Print("语音视频电话SharpSvr.s2c");
						}
						else
						{
							Debug.Print("其他命令" + serviceCmd);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message.ToString());
			}

			return null;
		}
		#endregion
		#region 解析登录返回包
		public static int Un_Pack_Login(byte[] bytes)
		{

			var head1_len = BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0);
			bytes = bytes.Skip(4).ToArray();
			bytes = bytes.Skip(1).ToArray();
			var head2_len = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
			bytes = bytes.Skip(14).ToArray();
			int result = bytes[0];
			Debug.Print("验证类型码:" + result.ToString());
			bytes = bytes.Skip(1).Take(head2_len - 16 - 1).ToArray();
			HashTea Hash = new HashTea();
			bytes = Hash.UNHashTEA(bytes, API.QQ.shareKey, 0, true);
			Debug.Print("Un_Pack_Login:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			if (bytes.Take(2).ToArray()[1] == 25)
			{
				var len = BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0);
				bytes = bytes.Skip(2).ToArray();
				var publickey = bytes.Take(len).ToArray();
				bytes = bytes.Skip(len).ToArray();
				var sharekey = ECDH.GetECDHKeysEx(publickey, API.QQ.pub_key, API.QQ.prikey);
				bytes = Hash.UNHashTEA(bytes, sharekey, 0, true);
			}
			if (result == 0)
			{
				bytes = bytes.Skip(7).ToArray();
				var len = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).Take(len).ToArray();
				bytes = Hash.UNHashTEA(bytes, API.QQ.TGTKey, 0, true);
				Debug.Print("解密后登录成功数组:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
				DecodeTlv(bytes);
				API.QQ.key = API.UN_Tlv.T305_SessionKey;
				API.QQ.loginState = (int)API.LoginState.LoginSccess;
				return 1;
			}
			if (result == 2)
			{
				DecodeTlv(bytes.Skip(3).ToArray());
				if (API.UN_Tlv.T192_captcha != null)
				{
					WebForm.Url = API.UN_Tlv.T192_captcha;
					WebForm f1 = new WebForm();
					f1.ShowDialog();
					API.QQ.loginState = (int)API.LoginState.Logining;
					if (API.QQ.Ticket != null)
					{
						API.TClient.SendData(Pack.VieryTicket(API.QQ.Ticket));
					}
				}
				API.QQ.loginState = (int)API.LoginState.LoginFaild;
				return 2;
			}
			else if (result == 3)
			{
				Debug.Print("接收异常");
				API.QQ.loginState = (int)API.LoginState.LoginFaild;
			}
			else if (result == 1)
			{
				Debug.Print("登录账号或密码错误");
				API.QQ.loginState = (int)API.LoginState.LoginFaild;
			}
			else if (result == 40)
			{
				Debug.Print("账号被回收或无此账号");
				API.QQ.loginState = (int)API.LoginState.LoginFaild;
			}
			else if (result == 40)
			{
				Debug.Print("账号被冻结,或者账号密码已泄漏,存在被盗风险");
				API.QQ.loginState = (int)API.LoginState.LoginFaild;
			}
			else if (result == 160)
			{
				Debug.Print(result.ToString() + ":" + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
				DecodeTlv(bytes.Skip(3).ToArray());
				SDK.GetLog(API.UN_Tlv.T17E_message);
				Debug.Print(API.UN_Tlv.T17E_message);
				if (API.UN_Tlv.T204_url_safeVerify != null)
				{
					WebForm2.Url = API.UN_Tlv.T204_url_safeVerify;
					WebForm2 f1 = new WebForm2();
					f1.ShowDialog();
					ClearWebDate();
				}
				else
				{
					API.TClient.SendData(Pack.VieryPhoneCode());
					string Answer = Microsoft.VisualBasic.Interaction.InputBox("请输入手机验证码");
					if (!string.IsNullOrEmpty(Answer))
					{
						Pack.SubmitVertificationCode(Answer);
					}
				}
				API.QQ.loginState = (int)API.LoginState.LoginVertify;
				return 2;
			}
			else if (result == 161)
			{
				Debug.Print("今日操作次数过多,请等待一天后再试");
				DecodeTlv(bytes.Skip(3).ToArray());
				WebForm2.Url = API.UN_Tlv.T204_url_safeVerify;
				WebForm f1 = new WebForm();
				f1.ShowDialog();
				ClearWebDate();
				API.QQ.loginState = (int)API.LoginState.LoginVertify;
				return 2;
			}
			else if (result == 162)
			{
				Debug.Print("验证码获取频繁,60s后重试");
				DecodeTlv(bytes.Skip(3).ToArray());
				WebForm2.Url = API.UN_Tlv.T204_url_safeVerify;
				WebForm2 f1 = new WebForm2();
				f1.ShowDialog();
				ClearWebDate();
				SendMobile = 3;
				//Thread.Sleep(60 * 1000)
				API.QQ.loginState = (int)API.LoginState.LoginVertify;
				return 2;
			}
			else if (result == 180)
			{
				Debug.Print("腾讯服务器繁忙,请重启框架重试");
				SDK.GetLog("腾讯服务器繁忙,请重启框架重试.");
				API.QQ.loginState = (int)API.LoginState.LoginVertify;
				return 2;
			}
			else if (result == 204)
			{
				if (API.UN_Tlv.T104 == null)
				{
					Debug.Print("正在验证设备锁...");
					//SDK.GetLog("正在验证设备锁...");
					DecodeTlv(bytes.Skip(3).ToArray());
					API.TClient.SendData(Pack.VieryLock());
					API.QQ.loginState = (int)API.LoginState.LoginVertify;
				}
				return 2;
			}
			else if (result == 235)
			{
				Debug.Print("账号密码错误");
				API.QQ.loginState = (int)API.LoginState.LoginVertify;
			}
			else if (result == 237)
			{
				Debug.Print("验证ticket输入错误或环境异常");
				API.QQ.loginState = (int)API.LoginState.LoginVertify;
			}
			else if (result == 239)
			{
				Debug.Print(result.ToString() + ":" + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
				DecodeTlv(bytes.Skip(3).ToArray());
				SDK.GetLog(API.UN_Tlv.T17E_message);
				Debug.Print(API.UN_Tlv.T17E_message);
				if (API.UN_Tlv.T204_url_safeVerify != null)
				{
					WebForm2.Url = API.UN_Tlv.T204_url_safeVerify;
					WebForm2 f1 = new WebForm2();
					f1.ShowDialog();
					ClearWebDate();
				}
				else
				{
					API.TClient.SendData(Pack.VieryPhoneCode());
					string Answer = Microsoft.VisualBasic.Interaction.InputBox("请输入手机验证码");
					if (!string.IsNullOrEmpty(Answer))
					{
						Pack.SubmitVertificationCode(Answer);
					}
				}
				API.QQ.loginState = (int)API.LoginState.LoginVertify;
				return 2;
			}
			else
			{
				Debug.Print("错误的验证类型:" + result.ToString());
				API.QQ.loginState = (int)API.LoginState.LoginVertify;
			}
			Un_Pack_ErrMsg(result, bytes);
			return 3;
		}
		#endregion
		#region 解析错误内容
		public static void Un_Pack_ErrMsg(int errorType, byte[] bytes)
		{
			try
			{
				bytes = bytes.Skip(2).ToArray();
				bytes = bytes.Skip(1).ToArray();
				bytes = bytes.Skip(4).ToArray();
				bytes = bytes.Skip(2).ToArray();
				var errType = BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0);
				bytes = bytes.Skip(4).ToArray();
				var titleLen = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).ToArray();
				var title = Encoding.UTF8.GetString(bytes, 0, titleLen);
				var messageLen = Convert.ToInt32(BitConverter.ToInt16(bytes.Skip(titleLen).Take(2).ToArray().Reverse().ToArray(), 0));
				var message = Encoding.UTF8.GetString(bytes.Skip(titleLen + 2).ToArray(), 0, messageLen);
				API.last_error = title + ":" + errorType.ToString() + " " + message;
				Debug.Print(API.last_error);
				SDK.GetLog(API.last_error);
			}
			catch (Exception ex)
			{

			}
		}
#endregion

#region 取图像数据
		public static void Un_Pack_VieryImage(byte[] bytes)
		{
			Debug.Print("要解包的图像数据:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			bytes = bytes.Skip(3).ToArray();
			DecodeTlv(bytes);
		}
#endregion

#region 解析TLV
		public static void DecodeTlv(byte[] bytes)
		{
			if (bytes.Length == 0)
			{
				return;
			}
			var tlv_count = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
			Debug.Print("tlv个数:" + tlv_count.ToString());
			bytes = bytes.Skip(2).ToArray();
			byte[] tlv_cmd = null;
			byte[] tlv_byte = null;
			int tlv_len = 0;
			for (int i = 0; i < tlv_count; i++)
			{
				if (bytes.Length == 0)
				{
					return;
				}
				tlv_cmd = bytes.Take(2).ToArray();
				bytes = bytes.Skip(2).ToArray();
				tlv_len = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).ToArray();
				tlv_byte = bytes.Take(tlv_len).ToArray();
				bytes = bytes.Skip(tlv_len).ToArray();
				tlv_get(BitConverter.ToString(tlv_cmd).Replace("-", string.Empty), tlv_byte);
			}
		}
		public static void tlv_get(string tlv_cmd, byte[] bytes)
		{

			if (tlv_cmd.Length == 0)
			{
				return;
			}
			if (tlv_cmd == "0108")
			{
				API.UN_Tlv.T108_ksid = bytes;
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T108_ksid:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "016A")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "token016A:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0106")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "token0106:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "010A")
			{
				API.UN_Tlv.T10A_token_A4 = bytes;
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T10A_token_A4(二次登录或上线用):" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0550")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "固定未知:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "010C")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
			}
			else if (tlv_cmd == "010D")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
			}
			else if (tlv_cmd == "010E")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
			}
			else if (tlv_cmd == "0103")
			{
				API.UN_Tlv.T103_clientkey = bytes;
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T103_clientkey :" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0114")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0118") //RequestId
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "RequestId:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0138")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "011A")
			{
				var face = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).ToArray();
				var age = Convert.ToInt32(bytes.Take(1).ToArray()[0]);
				bytes = bytes.Skip(1).ToArray();
				var gender = Convert.ToInt32(bytes.Take(1).ToArray()[0]);
				bytes = bytes.Skip(1).ToArray();
				var nickLen = bytes[0];
				bytes = bytes.Skip(1).ToArray();
				API.QQ.NickName = Encoding.UTF8.GetString(bytes, 0, nickLen);
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "昵称:" + API.QQ.NickName + " face:" + face.ToString() + " age:" + age.ToString() + " gender:" + gender.ToString());
			}
			else if (tlv_cmd == "011F")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "版本信息:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0120")
			{
				API.UN_Tlv.T120_skey = bytes;
				API.QQ.Cookies = "uin=o" + API.QQ.LongQQ.ToString() + "; T120_skey=" + Encoding.UTF8.GetString(bytes, 0, bytes.Length);
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "skey:" + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
			}
			else if (tlv_cmd == "0136")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "vkey:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0305")
			{
				API.UN_Tlv.T305_SessionKey = bytes;
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "sessionKey(二次登录用):" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0143")
			{
				API.UN_Tlv.T143_token_A2 = bytes;
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T143_token_A2(二次登录用):" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0164")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "sid:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0130")
			{
				Debug.Print("协议号:" + tlv_cmd);
				var times = BitConverter.ToString(bytes.Skip(2).Take(4).ToArray()).Replace("-", "");
				int timestamp = Int32.Parse(times, System.Globalization.NumberStyles.HexNumber);
				DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
				DateTime myDateTime = unixStart.AddSeconds(timestamp).ToLocalTime();
				byte[] ip2 = bytes.Skip(6).Take(4).ToArray().Reverse().ToArray();
				var ip = ip2[0] + "." + ip2[1] + "." + ip2[2] + "." + ip2[3];
				Debug.Print("time:" + myDateTime.ToString("yyyy-MM-dd hh:mm:ss") + " ip:" + ip);
			}
			else if (tlv_cmd == "0105")
			{
				var len1 = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).ToArray();
				API.UN_Tlv.T105_pic_token = bytes.Take(len1).ToArray();
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T105_pic_token:" + BitConverter.ToString(API.UN_Tlv.T105_pic_token).Replace("-", " "));
				bytes = bytes.Skip(len1).ToArray();
				var len2 = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).ToArray();
				API.UN_Tlv.T105_pic_Viery = bytes.Take(len2).ToArray();
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T105_pic_Viery:" + BitConverter.ToString(API.UN_Tlv.T105_pic_Viery).Replace("-", " "));
			}
			else if (tlv_cmd == "0104")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "UN_Tlv.T104:" + BitConverter.ToString(bytes).Replace("-", " "));
				API.UN_Tlv.T104 = bytes;
			}
			else if (tlv_cmd == "0163")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "pskey:" + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0165")
			{
				var errType = Convert.ToInt32(BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0));
				API.UN_Tlv.T165_pic_reason = errType.ToString();
				var titleLen = bytes.Skip(4).Take(1).ToArray().Reverse().ToArray()[0];
				var title = Encoding.UTF8.GetString(bytes.Skip(4 + 1).ToArray(), 0, titleLen);
				bytes = bytes.Skip(5 + titleLen).ToArray();
				var messageLen = Convert.ToInt32(BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0));
				var message = Encoding.UTF8.GetString(bytes.Skip(4).ToArray(), 0, messageLen);
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "ErrorType:" + errType.ToString() + " ErrorMsg:" + title + " " + message);
			}
			else if (tlv_cmd == "0133")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0134")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0204")
			{
				API.UN_Tlv.T204_url_safeVerify = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T204_url_safeVerify:" + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
			}
			else if (tlv_cmd == "0528")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0322")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "011D")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0402")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T402:" + BitConverter.ToString(bytes).Replace("-", " "));
				API.UN_Tlv.T402 = bytes;
			}
			else if (tlv_cmd == "0403")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T403:" + BitConverter.ToString(bytes).Replace("-", " "));
				API.UN_Tlv.T403 = bytes;
			}
			else if (tlv_cmd == "0512")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
				Thread NewThread = new Thread(() =>
				{
											readPSKey(bytes);
				});
				NewThread.Start();
			}
			else if (tlv_cmd == "0522")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "0537")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			}
			else if (tlv_cmd == "016D")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
			}
			else if (tlv_cmd == "0192")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T192_captcha:" + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
				API.UN_Tlv.T192_captcha = Encoding.UTF8.GetString(bytes);
			}
			else if (tlv_cmd == "0178")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
			}
			else if (tlv_cmd == "0174")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T174:" + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
				API.UN_Tlv.T174 = bytes;
			}
			else if (tlv_cmd == "0178")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
				API.UN_Tlv.T178_phone = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			}
			else if (tlv_cmd == "0179")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
				API.UN_Tlv.T179 = bytes;
			}
			else if (tlv_cmd == "017D")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T17D_url_aq:" + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
				API.UN_Tlv.T17D_url_aq = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			}
			else if (tlv_cmd == "017E")
			{
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "T17E_message:" + Encoding.UTF8.GetString(bytes, 0, bytes.Length));
				API.UN_Tlv.T17E_message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			}
			else
			{
				Debug.Print("未知协议号:" + tlv_cmd + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			}

		}
#endregion
		private static void ClearWebDate()
		{
			if (Directory.Exists(Application.StartupPath + "\\LocalStorage"))
			{
				try
				{
					Directory.Delete(Application.StartupPath + "\\LocalStorage");
				}
				catch (Exception ex)
				{
				}
			}
			if (File.Exists(Application.StartupPath + "\\cookies.dat"))
			{
				try
				{
					File.Delete(Application.StartupPath + "\\cookies.dat");
				}
				catch (Exception ex)
				{
				}
			}
		}
		private static void readPSKey(byte[] bytes)
		{
			while (bytes.Length > 2)
			{
				Thread.Sleep(50);
				var len = BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0);
				bytes = bytes.Skip(2).ToArray();
				var len1 = BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0);
				bytes = bytes.Skip(2).ToArray();
				var domain = Encoding.UTF8.GetString(bytes, 0, len1);
				bytes = bytes.Skip(len1).ToArray();
				var len2 = BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0);
				bytes = bytes.Skip(2).ToArray();
				if (bytes.Length > len2)
				{
					switch (domain)
					{
						case "weiyun.com":
							API.QQ.pskey.weiyun = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "tenpay.com":
							API.QQ.pskey.tenpay = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "openmobile.qq.com":
							API.QQ.pskey.openmobile_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "qun.qq.com":
							API.QQ.pskey.qun_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "game.qq.com":
							API.QQ.pskey.game_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "connect.qq.com":
							API.QQ.pskey.connect_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "mail.qq.com":
							API.QQ.pskey.mail_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "qzone.com":
							API.QQ.pskey.qzone = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "qzone.qq.com":
							API.QQ.pskey.qzone_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "imgcache.qq.com":
							API.QQ.pskey.imgcache_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "hall.qq.com":
							API.QQ.pskey.hall_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "ivac.qq.com":
							API.QQ.pskey.ivac_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "vip.qq.com":
							API.QQ.pskey.vip_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "gamecenter.qq.com":
							API.QQ.pskey.gamecenter_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "haoma.qq.com":
							API.QQ.pskey.haoma_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "docs.qq.com":
							API.QQ.pskey.docs_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "b.qq.com":
							API.QQ.pskey.b_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
						case "exmail.qq.com":
							API.QQ.pskey.exmail_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
	
						case "lol.qq.com":
							API.QQ.pskey.lol_qq = Encoding.UTF8.GetString(bytes, 0, len2);
							break;
					}
				}
				bytes = bytes.Skip(len2).ToArray();
			}
		}
		public static string GetPSKey(string domain)
		{
			switch (domain)
			{
				case "weiyun.com":
					return API.QQ.pskey.weiyun;
				case "tenpay.com":
					return API.QQ.pskey.tenpay;
				case "openmobile.qq.com":
					return API.QQ.pskey.openmobile_qq;
				case "qun.qq.com":
					return API.QQ.pskey.qun_qq;
				case "game.qq.com":
					return API.QQ.pskey.game_qq;
				case "connect.qq.com":
					return API.QQ.pskey.connect_qq;
				case "mail.qq.com":
					return API.QQ.pskey.mail_qq;
				case "qzone.com":
					return API.QQ.pskey.qzone;
				case "qzone.qq.com":
					return API.QQ.pskey.qzone_qq;
				case "imgcache.qq.com":
					return API.QQ.pskey.imgcache_qq;
				case "hall.qq.com":
					return API.QQ.pskey.hall_qq;
				case "ivac.qq.com":
					return API.QQ.pskey.ivac_qq;
				case "vip.qq.com":
					return API.QQ.pskey.vip_qq;
				case "gamecenter.qq.com":
					return API.QQ.pskey.gamecenter_qq;
				case "haoma.qq.com":
					return API.QQ.pskey.haoma_qq;
				case "docs.qq.com":
					return API.QQ.pskey.docs_qq;
				case "b.qq.com":
					return API.QQ.pskey.b_qq;
				case "exmail.qq.com":
					return API.QQ.pskey.exmail_qq;
				case "lol.qq.com":
					return API.QQ.pskey.lol_qq;
			}
			return "";
		}


	}

}