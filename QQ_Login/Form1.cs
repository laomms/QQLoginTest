
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

using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ProtoBuf;

namespace QQ_Login
{
	public partial class Form1
	{
		public Form1()
		{
			InitializeComponent();
		}

		internal static Form1 MyInstance;

		private void Button20_Click(object sender, EventArgs e)
		{
			Module1.TClient.SendData(Pack.PackOnlineStatus("StatSvc.register", 2));
			RichTextBox1.Text = "【" + DateTime.Now + "】" + "已下线" + "\r\n";
			Module1.TClient.Close();
		}
		private void Button1_Click(object sender, EventArgs e)
		{
			string errMsg = "";
			if (string.IsNullOrEmpty(TextBox1.Text) || string.IsNullOrEmpty(TextBox2.Text))
			{
				return;
			}
			if (Button1.Text == "登录")
			{
				Module1.ThisQQ = long.Parse(TextBox1.Text);
				Module1.QQ.loginState = (int)Module1.LoginState.Logining;
				RichTextBox1.Text = "【" + DateTime.Now + "】" + "开始登录QQ：" + TextBox1.Text + "\r\n";
				if (Module1.UN_Tlv.T143_token_A2 != null && Module1.QQ.shareKey != null && Module1.UN_Tlv.T10A_token_A4 != null)
				{
					Module1.reLogin();
				}
				else
				{
					Initialization(TextBox1.Text, TextBox2.Text); //初始化赋值

					Module1.QQ.loginState = (int)Module1.LoginState.Logining;

					Module1.TClient.SendData(Pack.LoginPackage());
				}


			}
			else if (Button1.Text == "再次登录")
			{
				//Dim PackImage = Pack.Pack_VieryImage(textBox3.Text)
				//If QQ.mRequestID > 2147483647 Then
				//    QQ.mRequestID = 10000
				//Else
				//    QQ.mRequestID += 1
				//End If
				//TClient.SendData(PackImage)

			}
		}

		public void Initialization(string Account, string Password)
		{
			Module1.QQ.Account = Account;
			Module1.QQ.LongQQ = long.Parse(Module1.QQ.Account);
			Module1.QQ.UTF8 = Encoding.UTF8.GetBytes(Module1.QQ.Account);
			Module1.QQ.user = Module1.HexStrToByteArray(Module1.QQ.LongQQ.ToString("X"));
			Module1.QQ.pass = Password;
			Module1.QQ.md5_1 = Module1.MD5Hash(Encoding.UTF8.GetBytes(Module1.QQ.pass));
			byte[] md5_byte = Module1.QQ.md5_1.Concat(new byte[] {0, 0, 0, 0}).Concat(Module1.QQ.user).ToArray();
			Module1.QQ.md5_2 = Module1.MD5Hash(md5_byte);
			Module1.ECDH_Struct _ECDH = ECDH.GetECDHKeys();
			Module1.QQ.pub_key = _ECDH.PublicKey;
			Module1.QQ.shareKey = _ECDH.Sharekey;
			Module1.QQ.prikey = _ECDH.PrivateKey;

			Module1.QQ.mRequestID = 10000;
			Module1.QQ.key = new byte[16];
			var timestampHex = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();

			Module1.QQ.login_Time = Module1.HexStrToByteArray(int.Parse(timestampHex).ToString("X"));


			Module1.QQ.TGTKey = Module1.MD5Hash(Module1.QQ.pub_key);
			Module1.QQ.randKey = Module1.MD5Hash(Module1.QQ.shareKey);
			Module1.QQ.MsgCookies = Module1.GetRandByteArray(4);
			Module1.QQ.Appid = 537065990;
			Module1.UN_Tlv.T108_ksid = Module1.HexStrToByteArray("93AC689396D57E5F9496B81536AAFE91");
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
				Module1.QQ.SyncCoookies = ms.ToArray();
			}

			Module1.Device.imei = "865166024867445";
			Module1.Device.Imsi = "460001330114682";
			Module1.Device.WIFIByteSSID = Module1.MD5Hash(Encoding.UTF8.GetBytes("5c:11:21:11:19:1f"));
			Module1.Device.WIFISSID = "dlb";
			Module1.Device.Ver = "|" + Module1.Device.imei + "|A8.5.0.4003a808"; //手机串号加QQ版本
			Module1.Device.Version = Encoding.UTF8.GetBytes("A8.5.0.4003a808");
			Module1.Device.MacBytes = Encoding.UTF8.GetBytes("DA-EB-D5-1C-7B-CD");
			Module1.Device.MacId = "84:18:38:38:96:36";
			Module1.Device.GUIDBytes = Module1.MD5Hash(Encoding.UTF8.GetBytes("b7981398-337d-4d2c-ab64-22b5b6f297dc"));
			Module1.Device.AndroidID = Module1.MD5Hash(Encoding.UTF8.GetBytes("95dcc49a9434f65a"));
			Module1.Device.AppId = 537042771;
			Module1.Device.os_type = "android"; //'安卓版本
			Module1.Device.os_version = "5.1.1";
			Module1.Device.network_type = "China Mobile GSM";
			Module1.Device.apn = "wifi";
			Module1.Device.model = "oppo r9 plustm a"; //手机型号
			Module1.Device.brands = "oppo"; //手机品牌
			Module1.Device.Apk_Id = "com.tencent.mobileqq";
			Module1.Device.Apk_V = "8.5.0"; //安卓版本
			Module1.Device.ApkSig = new byte[] {0xA6, 0xB7, 0x45, 0xBF, 0x24, 0xA2, 0xC2, 0x77, 0x52, 0x77, 0x16, 0xF6, 0xF3, 0x6E, 0xB6, 0x8D}; //固定app_sign

		}

		public void Extract(System.Reflection.Assembly assembly, string outDirectory, string resourceName)
		{
			using (Stream s = assembly.GetManifestResourceStream($"{assembly.GetName().Name.Replace("-", "_")}.{resourceName}"))
			{
				using (BinaryReader r = new BinaryReader(s))
				{
					using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
					{
						using (BinaryWriter w = new BinaryWriter(fs))
						{
							w.Write(r.ReadBytes((int)s.Length));
						}
					}
				}
			}
			File.SetAttributes(outDirectory + "\\" + resourceName, (System.IO.FileAttributes)((int)Microsoft.VisualBasic.Constants.vbArchive + (int)Microsoft.VisualBasic.Constants.vbHidden + (int)Microsoft.VisualBasic.Constants.vbSystem));
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			MyInstance = this;
			if (File.Exists(Application.StartupPath + "\\libeay32.dll") == false)
			{
				Extract(Assembly.GetExecutingAssembly(), Application.StartupPath, "libeay32.dll");
			}
			if (File.Exists(Application.StartupPath + "\\node.dll") == false)
			{
				Extract(Assembly.GetExecutingAssembly(), Application.StartupPath, "node.dll");
			}
			if (File.Exists(Application.StartupPath + "\\test.amr") == false)
			{
				Extract(Assembly.GetExecutingAssembly(), Application.StartupPath, "test.amr");
			}
		}

		private void Button3_Click(object sender, EventArgs e)
		{

			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			var WithdrawInfo = FriendMsg.SendFriendMsg(long.Parse(TextBox6.Text), Encoding.UTF8.GetBytes(RichTextBox2.Text), Module1.MsgType.TextMsg); //文字消息
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			//JceStructSDK.GetNick(SendQQ)
			GroupMsg.SendGroupMsg(Convert.ToInt64(TextBox5.Text), Encoding.UTF8.GetBytes(RichTextBox2.Text), Module1.MsgType.TextMsg); //文字消息
		}

		private void Button4_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			byte[] picBytes = null;
			System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
			openFileDialog.Filter = "Image Files(*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.png; *.jpeg; *.gif; *.bmp";
			openFileDialog.RestoreDirectory = true;
			openFileDialog.FilterIndex = 1;
			DialogResult result = openFileDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Bitmap image1 = new Bitmap(openFileDialog.FileName);
				picBytes = File.ReadAllBytes(openFileDialog.FileName);
				Module1.FilePath = openFileDialog.FileName;
			}
			if (picBytes == null)
			{
				return;
			}
			var picMd5 = Module1.MD5Hash(picBytes);
			Module1.FileHash = picMd5;
			Module1.FileBytes = picBytes;
			SendPicMsgStruct PicInfo = new SendPicMsgStruct
			{
				Amount = 1,
				PicType = 3,
				PicInfo = new PicInfos
				{
					thisQQ = Module1.QQ.LongQQ,
					SendQQ = long.Parse(TextBox6.Text),
					StartFlag = 0,
					PicHash = picMd5,
					PicLengh = picBytes.Length,
					PicName = Module1.HashMD5(picBytes) + ".jpg",
					Field7 = 5,
					Field8 = 9,
					Field10 = 0,
					Field12 = 1,
					Field13 = 0,
					PicWidth = 647,
					PicHeigh = 980,
					PicPix = 1000,
					PicVer = "7.9.8.999",
					Field21 = 0,
					Field22 = 0
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, PicInfo);
				Debug.Print(BitConverter.ToString(ms.ToArray()).Replace("-", " "));
				var bytes = Module1.PackCmdHeader("LongConn.OffPicUp", ms.ToArray());
				Module1.TClient.SendData(Module1.PackAllHeader(bytes));
			}
		}

		private void Button5_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			byte[] picBytes = null;
			System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
			openFileDialog.Filter = "Image Files(*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.png; *.jpeg; *.gif; *.bmp";
			openFileDialog.RestoreDirectory = true;
			openFileDialog.FilterIndex = 1;
			DialogResult result = openFileDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Bitmap image1 = new Bitmap(openFileDialog.FileName);
				picBytes = File.ReadAllBytes(openFileDialog.FileName);
				Module1.FilePath = openFileDialog.FileName;
			}
			if (picBytes == null)
			{
				return;
			}
			var picMd5 = Module1.MD5Hash(picBytes);
			Module1.FileHash = picMd5;
			Module1.FileBytes = picBytes;
			SendGroupPicMsgStruct PicInfo = new SendGroupPicMsgStruct
			{
				Amount = 1,
				PicType = 3,
				GroupPicInfo = new GroupPicInfos
				{
					GroupId = long.Parse(TextBox5.Text),
					SendQQ = Module1.QQ.LongQQ,
					StartFlag = 0,
					PicHash = picMd5,
					PicSize = picBytes.Length,
					PicName = Module1.HashMD5(picBytes) + ".jpg",
					Field7 = 5,
					Field8 = 9,
					Field9 = 1,
					PicWidth = 647,
					PicHeigh = 980,
					PicPix = 1000,
					PicVer = "7.9.8.999",
					Field15 = 1007,
					Field19 = 0
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, PicInfo);
				var bytes = Module1.PackCmdHeader("ImgStore.GroupPicUp", ms.ToArray());
				Module1.TClient.SendData(Module1.PackAllHeader(bytes));
			}
		}


		private void TextBox6_TextChanged(object sender, EventArgs e)
		{
			Module1.SendQQ = long.Parse(TextBox6.Text);
		}

		private void TextBox5_TextChanged(object sender, EventArgs e)
		{
			Module1.GroupId = long.Parse(TextBox5.Text);
		}

		private void Button6_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			byte[] AudioBytes = File.ReadAllBytes(Application.StartupPath + "\\test.amr");
			using (var audioStream = new MemoryStream(AudioBytes))
			{
				AudioBytes = audioStream.ToArray();
			}
			Module1.FileHash = Module1.MD5Hash(AudioBytes);
			SendGroupAudioMsgStruct AudioInfo = new SendGroupAudioMsgStruct
			{
				AudioType = 3,
				Field2 = 3,
				GroupAudioInfo = new GroupAudioInfos
				{
					GroupId = long.Parse(TextBox5.Text),
					SendQQ = Module1.QQ.LongQQ,
					StartFlag = 0,
					AudioHash = Module1.FileHash,
					AudioSize = AudioBytes.Length,
					AudioName = Module1.HashMD5(AudioBytes) + ".amr",
					Field7 = 5,
					Field8 = 9,
					Field9 = 3,
					AudioVer = "7.9.8.999",
					Field12 = 1,
					Field13 = 1,
					Field14 = 1,
					Field15 = 1
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, AudioInfo);
				var bytes = Module1.PackCmdHeader("PttStore.GroupPttUp", ms.ToArray());
				Module1.TClient.SendData(Module1.PackAllHeader(bytes));
			}
		}

		private void Button7_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			byte[] AudioBytes = File.ReadAllBytes(Application.StartupPath + "\\test.amr");
			using (var audioStream = new MemoryStream(AudioBytes))
			{
				AudioBytes = audioStream.ToArray();
			}
			Module1.FileHash = Module1.MD5Hash(AudioBytes);
			SendAudioMsgStruct AudioInfo = new SendAudioMsgStruct
			{
				AudioType = 500,
				Field2 = 0,
				AudioInfo = new AudioInfos
				{
					thisQQ = Module1.QQ.LongQQ,
					SendQQ = long.Parse(TextBox6.Text),
					StartFlag = 2,
					AudioSize = AudioBytes.Length,
					AudioName = Module1.HashMD5(AudioBytes) + ".amr",
					AudioHash = Module1.FileHash
				},
				Field101 = 17,
				Field102 = 104,
				AudioOtherInfo = new AudioOtherInfos
				{
					Field1 = 3,
					Field2 = 0,
					Field90300 = 1,
					Field90500 = 3,
					Field90600 = 2,
					Field90800 = 5
				}
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, AudioInfo);
				Debug.Print("构造语音:" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", " "));
				var bytes = Module1.PackCmdHeader("PttCenterSvr.pb_pttCenter_CMD_REQ_APPLY_UPLOAD-500", ms.ToArray());
				Module1.TClient.SendData(Module1.PackAllHeader(bytes));
			}
		}

		private void Button9_Click(object sender, EventArgs e)
		{
			//Dim WithdrawInfo = FriendMsg.SendFriendMsg(Long.Parse(TextBox6.Text), Encoding.UTF8.GetBytes(RichTextBox2.Text), &HA)
			var WithdrawInfo = new Module1.FriendWithdrawInfo();
			FriendMsg.WithdrawFriendMsg(Module1.QQ.LongQQ, long.Parse(TextBox6.Text), Module1.FriendWithdraw.MsgReqId, Module1.FriendWithdraw.MsgRandomId, Module1.FriendWithdraw.MsgTimeStamp);
		}

		private void Button8_Click(object sender, EventArgs e)
		{
			GroupMsg.WithdrawGroupMsg(long.Parse(TextBox5.Text), Module1.GroupWithdraw.MsgReqId, Module1.GroupWithdraw.MsgRandomId);
		}

		private async void Button10_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			await JceStructSDK.GetNickAsync(long.Parse(TextBox6.Text));
		}

		private void Button11_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}

			var xmlMsg = "<?xml version='1.0' encoding='UTF-8' standalone='yes' ?><msg serviceID=\"83\" templateID=\"12345\" action=\"web\" brief=\"标题\" sourceMsgId=\"0\" url=\"https://post.mp.qq.com/group/article/33303433373836353238-35707230.html?_wv\" flag=\"3\" adverSign=\"0\" multiMsgFlag=\"0\"><item layout=\"2\" advertiser_id=\"0\" aid=\"0\"><picture cover=\"http://ww3.sinaimg.cn/mw690/96174781gw1fblx4dxa0lj20dw0dwmxp.jpg\" w=\"0\" h=\"0\" /><title>用户名</title><summary>钱包:余额</summary></item><source name=\"排名:第几名 \" icon=\"\" action=\"web\" appid=\"0\" /></msg>";
			var zipByte = Module1.CompressData(Encoding.UTF8.GetBytes(xmlMsg));
			GroupMsg.SendGroupMsg(Convert.ToInt64(TextBox5.Text), zipByte, Module1.MsgType.XmlMsg);
		}

		private void Button14_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}

			var xmlMsg = "<?xml version='1.0' encoding='UTF-8' standalone='yes' ?><msg serviceID=\"83\" templateID=\"12345\" action=\"web\" brief=\"标题\" sourceMsgId=\"0\" url=\"https://post.mp.qq.com/group/article/33303433373836353238-35707230.html?_wv\" flag=\"3\" adverSign=\"0\" multiMsgFlag=\"0\"><item layout=\"2\" advertiser_id=\"0\" aid=\"0\"><picture cover=\"http://ww3.sinaimg.cn/mw690/96174781gw1fblx4dxa0lj20dw0dwmxp.jpg\" w=\"0\" h=\"0\" /><title>用户名</title><summary>钱包:余额</summary></item><source name=\"排名:第几名 \" icon=\"\" action=\"web\" appid=\"0\" /></msg>";
			var zipByte = Module1.CompressData(Encoding.UTF8.GetBytes(xmlMsg));
			FriendMsg.SendFriendMsg(Convert.ToInt64(TextBox6.Text), zipByte, Module1.MsgType.XmlMsg);
		}

		private void Button17_Click(object sender, EventArgs e)
		{
			JceStructSDK.GetFriendList(0, 30); //取好友列表

		}

		private void Button18_Click(object sender, EventArgs e)
		{
			JceStructSDK.GetGroupList(); //取群列表
		}

		private void Button15_Click(object sender, EventArgs e)
		{
			JceStructSDK.GetGroupMemberList(long.Parse(TextBox5.Text));
		}

		private void Button19_Click(object sender, EventArgs e)
		{
			ProtoSDK.GetGroupAdminList(long.Parse(TextBox5.Text));
		}

		private void Button12_Click(object sender, EventArgs e)
		{

		}

		private void Button13_Click(object sender, EventArgs e)
		{

		}

		private void TextBox1_DoubleClick(object sender, EventArgs e)
		{
			TextBox1.Text = Module1.GetText();
		}

		private void TextBox2_DoubleClick(object sender, EventArgs e)
		{
			TextBox2.Text = Module1.GetText();
		}

		private void Button21_Click(object sender, EventArgs e)
		{
			ProtoSDK.ShutAll(long.Parse(TextBox5.Text), Module1.Mute.Close);
		}

		private void Button22_Click(object sender, EventArgs e)
		{
			ProtoSDK.ShutAll(long.Parse(TextBox5.Text), Module1.Mute.Open);
		}

		
	}



}