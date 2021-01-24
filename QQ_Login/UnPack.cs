
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
//解包

namespace QQ_Login
{
	public class UnPack
	{
		private static void GetLoginResult(string code = "")
		{
			if (DataList.QQ.loginState == (int)DataList.LoginState.LoginSccess)
			{
				Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + "登录成功" + "\r\n")));
				//MsgBox("skey: " + Encoding.UTF8.GetString(QQ.skey) + vbNewLine + "pskey: " + Encoding.UTF8.GetString(QQ.pskey) + vbNewLine + "SessionKey: " + Encoding.UTF8.GetString(QQ.sessionKey), vbInformation + vbMsgBoxSetForeground + vbSystemModal + vbCritical + vbInformation, "登录成功")
			}
			else if (DataList.QQ.loginState == (int)DataList.LoginState.LoginVertify)
			{
				Debug.Print("需要验证码");
				if (DataList.QQ.mRequestID > 2147483647)
				{
					DataList.QQ.mRequestID = 10000;
				}
				else
				{
					DataList.QQ.mRequestID += 1;
				}
				TCPIPClient.SendData(Pack.Pack_VieryImage(code));
			}
			else
			{
				Microsoft.VisualBasic.Interaction.MsgBox(DataList.getLastError(), (Microsoft.VisualBasic.MsgBoxStyle)((int)Microsoft.VisualBasic.Constants.vbInformation + (int)Microsoft.VisualBasic.Constants.vbMsgBoxSetForeground + (int)Microsoft.VisualBasic.Constants.vbSystemModal + (int)Microsoft.VisualBasic.Constants.vbCritical + (int)Microsoft.VisualBasic.Constants.vbInformation), "登录失败");
			}
		}

		public static int UnPackReceiveData(byte[] ReceiveData)
		{
			if (ReceiveData.Length == 0)
			{
				return 1;
			}
			Debug.Print("收到包:" + ReceiveData.Length.ToString() + "\r\n" + BitConverter.ToString(ReceiveData).Replace("-", " "));
			var packType = int.Parse(ReceiveData.Skip(7).Take(1).ToArray()[0].ToString());
			var encrptType = int.Parse(ReceiveData.Skip(8).Take(1).ToArray()[0].ToString());
			int pos = DataList.SearchBytes(ReceiveData, DataList.QQ.UTF8);

			var bytes = ReceiveData.Skip(pos + DataList.QQ.UTF8.Length).ToArray();

			HashTea Hash = new HashTea();
			if (packType == 0xB || packType == 9)
			{
				if (encrptType == 1)
				{
					bytes = Hash.UNHashTEA(bytes, DataList.QQ.sessionKey, 0, true);
				}
				else if (encrptType == 2)
				{
					bytes = Hash.UNHashTEA(bytes, DataList.QQ.sessionKey, 0, true);
				}
			}
			else if (packType == 0xA || packType == 8)
			{
				if (encrptType == 1)
				{
					bytes = Hash.UNHashTEA(bytes, DataList.QQ.sessionKey, 0, true);
				}
				else if (encrptType == 2)
				{
					bytes = Hash.UNHashTEA(bytes, DataList.QQ.sessionKey, 0, true);
				}
			}
			Debug.Print("解密后:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));

			//取出第一层包头中的包长度值
			var head1_len = BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0);
			byte[] bodyBytes = bytes.Skip(head1_len).ToArray();
			Debug.Print("主包体:" + bodyBytes.Length.ToString() + "\r\n" + BitConverter.ToString(bodyBytes).Replace("-", " "));

			bytes = bytes.Skip(4).Take(head1_len - 4).ToArray();
			bytes = bytes.Skip(4).ToArray();
			if (bytes.Skip(4).Take(4).ToArray() == new byte[] {0, 0, 0, 0})
			{
				bytes = bytes.Skip(4).ToArray();
			}
			else
			{
				var head3_len = BitConverter.ToInt32(bytes.Skip(4).Take(4).ToArray().Reverse().ToArray(), 0);
				bytes = bytes.Skip(head3_len + 4).ToArray();
			}
			var str_len = BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0);
			var serviceCmd = Encoding.UTF8.GetString(bytes.Skip(4).ToArray(), 0, str_len - 4);
			if (serviceCmd.Contains("wtlogin.login"))
			{
				Debug.Print("登录命令:wtlogin.login");
				var status = UnPack.Un_Pack_Login(bodyBytes);
				Debug.Print("status:" + status.ToString());
				if (status == 1)
				{
					Debug.Print("wtlogin.login");
					var sendBytes = Pack.PackOnlineStatus("StatSvc.register", 0); //发上线包
					TCPIPClient.socket_send(sendBytes);
					//Pack.PackOidbSvc_0x59f()
					DataList.QQ.loginState = (int)DataList.LoginState.LoginSccess;
				}
				else if (status == 0)
				{
					Microsoft.VisualBasic.Interaction.MsgBox(DataList.getLastError(), (Microsoft.VisualBasic.MsgBoxStyle)((int)Microsoft.VisualBasic.Constants.vbInformation + (int)Microsoft.VisualBasic.Constants.vbMsgBoxSetForeground + (int)Microsoft.VisualBasic.Constants.vbSystemModal + (int)Microsoft.VisualBasic.Constants.vbCritical + (int)Microsoft.VisualBasic.Constants.vbInformation), "登录失败");
				}
			}
			else if (serviceCmd.Contains("PushService.register"))
			{
				Debug.Print("服务器注册成功:PushService.register");
				DataList.QQ.loginState = (int)DataList.LoginState.LoginSccess;
				GetLoginResult(((int)DataList.LoginState.LoginSccess).ToString());
			}
			else if (serviceCmd.Contains("RegPrxySvc.PushParam"))
			{
				Debug.Print("RegPrxySvc.PushParam");

			}
			else if (serviceCmd.Contains("ConfigPushSvc.PushDomain"))
			{
				Debug.Print("成功登录服务器:ConfigPushSvc.PushDomain");
				Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + "上线成功" + "\r\n")));
			}
			else if (serviceCmd.Contains("OnlinePush.ReqPush"))
			{
				Debug.Print("系统推送消息、撤回、加好友等");
			}
			else if (serviceCmd.Contains("ConfigPushSvc.PushReq"))
			{
				Debug.Print("系统提示消息");
			}
			else if (serviceCmd.Contains("StatSvc.SimpleGet"))
			{
				Debug.Print("收到心跳包");
			}
			else if (serviceCmd.Contains("MessageSvc.PushNotify"))
			{
				Debug.Print("通知好友消息:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
				bytes = bytes.Reverse().ToArray();
				var code = bytes.Skip(15).Take(4).ToArray().Reverse().ToArray();
				var TimeProtobuf = bytes.Take(7).ToArray().Reverse().ToArray();
				var sendByte = Pack.PackFriendMsg(code, TimeProtobuf);
				TCPIPClient.SendData(sendByte);
			}
			else if (serviceCmd.Contains("MessageSvc.PbGetMsg"))
			{
				Debug.Print("收到好友消息");
				FriendMsg.DeFriendMsg(bodyBytes);
			}
			else if (serviceCmd.Contains("OnlinePush.PbPushGroupMsg"))
			{
				Debug.Print("群聊消息");				
				GroupMsg.DeGroupMsg(bodyBytes);
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
				Debug.Print("回执");
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
			}
			else if (serviceCmd.Contains("MessageSvc.PbSendMsg"))
			{
				Debug.Print("递增发送信息");

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
			}
			else if (serviceCmd.Contains("friendlist.getFriendGroupList"))
			{
				Debug.Print("好友列表");
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
				Debug.Print("回执清单");
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
				Debug.Print("获取信息");
			}
			else if (serviceCmd.Contains("StatSvc.register"))
			{
				Debug.Print("注册上线:StatSvc.register");
			}
			else if (serviceCmd.Contains("PbMessageSvc.PbMsgReadedReport"))
			{
				Debug.Print("已读消息");
			}
			else
			{
				Debug.Print("其他命令" + serviceCmd);
			}
			return DataList.QQ.loginState;
		}

		public static int Un_Pack_Login(byte[] bytes)
		{
			Debug.Print("wtlogin.login包体:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			var head1_len = BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0);
			bytes = bytes.Skip(4).ToArray();
			bytes = bytes.Skip(1).ToArray();
			var head2_len = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
			bytes = bytes.Skip(14).ToArray();
			int result = bytes[0];
			Debug.Print("验证类型码:" + result.ToString());
			bytes = bytes.Skip(1).Take(head2_len - 16 - 1).ToArray();
			HashTea Hash = new HashTea();
			bytes = Hash.UNHashTEA(bytes, DataList.QQ.shareKey, 0, true);
			Debug.Print("解密后包体:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			if (result == 0)
			{
				bytes = bytes.Skip(7).ToArray();
				var len = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).Take(len).ToArray();
				bytes = Hash.UNHashTEA(bytes, DataList.QQ.TGTKey, 0, true);
				Debug.Print("解密后登录成功数组:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
				DecodeTlv(bytes);
				DataList.QQ.key = DataList.QQ.sessionKey;
				DataList.QQ.loginState = (int)DataList.LoginState.LoginSccess;
				return 1;
			}
			else if (result == 2)
			{
				Un_Pack_VieryImage(bytes);
				DataList.last_error = "需要输入验证码";
				DataList.QQ.loginState = (int)DataList.LoginState.LoginVertify;
				bytes = DataList.QQ.Verify;
				FileStream fs = new FileStream("vcode.png", FileMode.Create);
				fs.Write(bytes, 0, bytes.Length);
				fs.Flush();
				fs.Close();
				Bitmap bp = new Bitmap("vcode.png");
				Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.Button1.Text = "再次登录"));
				Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.PictureBox1.Image = bp));
				DataList.QQ.loginState = (int)DataList.LoginState.LoginFaild;
				return 2;
			}
			else if (result == 1)
			{
				DataList.last_error = "登录账号或密码错误";
				Debug.Print("登录账号或密码错误");
				DataList.QQ.loginState = (int)DataList.LoginState.LoginFaild;
			}
			else if (result == 40)
			{
				DataList.last_error = "账号被冻结";
				Debug.Print("账号被冻结");
				DataList.QQ.loginState = (int)DataList.LoginState.LoginFaild;
			}
			else if (result == 160)
			{
				DataList.last_error = "需要验证设备锁";
				Debug.Print("需要验证设备锁");
				DataList.QQ.loginState = (int)DataList.LoginState.LoginVertify;
			}
			else if (result == 180)
			{
				DataList.last_error = "腾讯服务器繁忙,请重启框架重试";
				Debug.Print("腾讯服务器繁忙,请重启框架重试");
				DataList.QQ.loginState = (int)DataList.LoginState.LoginVertify;
			}
			else if (result == 204)
			{
				DataList.last_error = "需要二次登录";
				Debug.Print("需要二次登录");
				DataList.QQ.loginState = (int)DataList.LoginState.LoginVertify;
			}
			else if (result == 235)
			{
				DataList.last_error = "账号密码错误";
				Debug.Print("账号密码错误");
				DataList.QQ.loginState = (int)DataList.LoginState.LoginVertify;
			}
			else
			{
				DataList.last_error = "错误的验证类型:" + result.ToString() + ",请打开此账号的设备锁然后重试";
				Debug.Print("错误的验证类型:" + result.ToString() + ",请打开此账号的设备锁然后重试");
				DataList.QQ.loginState = (int)DataList.LoginState.LoginVertify;
			}
			Un_Pack_ErrMsg(bytes);
			return 0;
		}
		public static void Un_Pack_ErrMsg(byte[] bytes)
		{
			Debug.Print("收到的错误提示包:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
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
			DataList.last_error = title + ":" + message;
			Debug.Print(DataList.last_error);
		}
		public static void Un_Pack_VieryImage(byte[] bytes)
		{
			Debug.Print("要解包的图像数据:" + Environment.NewLine + BitConverter.ToString(bytes).Replace("-", " "));
			bytes = bytes.Skip(3).ToArray();
			DecodeTlv(bytes);
		}
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
				DataList.QQ.ksid = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "ksid:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "016A")
			{
				DataList.QQ.Token016A = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "token016A:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0106")
			{
				DataList.QQ.Token0106 = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "token0106:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "010A")
			{
				DataList.QQ.Token010A = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "Token010A(二次登录或上线用):" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0550")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "固定未知:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "010C") //tgtkey
			{
				var token010C = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "tgtkey:" + token010C)
			}
			else if (tlv_cmd == "010D")
			{
				var token010D = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "token010D:" + token010D)
			}
			else if (tlv_cmd == "010E")
			{
				DataList.QQ.mST1Key = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "mST1Key:" + Encoding.UTF8.GetString(bytes, 0, bytes.Length))
			}
			else if (tlv_cmd == "0103")
			{
				DataList.QQ.stweb = BitConverter.ToString(bytes).Replace("-", " ");
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "clientkey:" + QQ.stweb)
			}
			else if (tlv_cmd == "0114")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "unknown_info:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0118") //RequestId
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "RequestId:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0138")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "固定未知:" + BitConverter.ToString(bytes).Replace("-", " "))
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
				var nick = Encoding.UTF8.GetString(bytes, 0, nickLen);
				//Debug.Print("协议号:" + tlv_cmd +Environment.NewLine+ "昵称:" + nick + " face:" + face.ToString + " age:" + age.ToString + " gender:" + gender.ToString)
			}
			else if (tlv_cmd == "011F")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "版本信息:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0120")
			{
				DataList.QQ.skey = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "skey:" + Encoding.UTF8.GetString(bytes, 0, bytes.Length))
			}
			else if (tlv_cmd == "0136")
			{
				DataList.QQ.vkey = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "vkey:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0305")
			{
				DataList.QQ.sessionKey = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "sessionKey(二次登录用):" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0143")
			{
				DataList.QQ.Token0143 = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "Token0143(二次登录用):" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0164")
			{
				DataList.QQ.sid = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "sid:" + BitConverter.ToString(bytes).Replace("-", " "))
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
				//Debug.Print("time:" + myDateTime.ToString("yyyy-MM-dd hh:mm:ss") + " ip:" + ip)
			}
			else if (tlv_cmd == "0105")
			{
				var len1 = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).ToArray();
				DataList.QQ.VieryToken1 = bytes.Take(len1).ToArray();
				Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "VieryToken1:" + BitConverter.ToString(DataList.QQ.VieryToken1).Replace("-", " "));
				bytes = bytes.Skip(len1).ToArray();
				var len2 = Convert.ToInt32(BitConverter.ToInt16(bytes.Take(2).ToArray().Reverse().ToArray(), 0));
				bytes = bytes.Skip(2).ToArray();
				DataList.QQ.Verify = bytes.Take(len2).ToArray();
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "Verify:" + BitConverter.ToString(QQ.Verify).Replace("-", " "))
			}
			else if (tlv_cmd == "0104")
			{
				DataList.QQ.VieryToken2 = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "VieryToken2:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0163")
			{
				DataList.QQ.pskey = bytes;
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "pskey:" + BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0165")
			{
				var errType = Convert.ToInt32(BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0));
				var titleLen = bytes.Skip(4).Take(1).ToArray().Reverse().ToArray()[0];
				var title = Encoding.UTF8.GetString(bytes.Skip(4 + 1).ToArray(), 0, Convert.ToInt32(titleLen));
				bytes = bytes.Skip(5 + titleLen).ToArray();
				var messageLen = Convert.ToInt32(BitConverter.ToInt32(bytes.Take(4).ToArray().Reverse().ToArray(), 0));
				var message = Encoding.UTF8.GetString(bytes.Skip(4).ToArray(), 0, messageLen);
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "ErrorType:" + errType.ToString + " ErrorMsg:" + title + " " + message)
			}
			else if (tlv_cmd == "0133")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine +  BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0134")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine +  BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0528")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "{"QIM_invitation_bit":"1"}" +  BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0322")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine +  BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "011D")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine +  BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0512")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "域名信息："+ BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0522")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine +  BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "0537")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine +  BitConverter.ToString(bytes).Replace("-", " "))
			}
			else if (tlv_cmd == "016D")
			{
				//Debug.Print("协议号:" + tlv_cmd + Environment.NewLine + "Key:"+ Encoding.UTF8.GetString(bytes, 0, bytes.Length))
			}
			else
			{
				Debug.Print("未知协议号:" + tlv_cmd + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			}

		}

	}

}