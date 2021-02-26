
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
			Data.TClient.SendData(Pack.PackOnlineStatus("StatSvc.register", 2));
			RichTextBox1.Text = "【" + DateTime.Now + "】" + "已下线" + "\r\n";
			Data.TClient.Close();
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
				Data.ThisQQ = long.Parse(TextBox1.Text);
				Data.QQ.loginState = (int)Data.LoginState.Logining;
				RichTextBox1.Text = "【" + DateTime.Now + "】" + "开始登录QQ：" + TextBox1.Text + "\r\n";
				if (Data.UN_Tlv.T143_token_A2 != null && Data.QQ.shareKey != null && Data.UN_Tlv.T10A_token_A4 != null)
				{
					Data.reLogin();
				}
				else
				{
					Initialization(TextBox1.Text, TextBox2.Text); //初始化赋值

					Data.QQ.loginState = (int)Data.LoginState.Logining;

					Data.TClient.SendData(Pack.LoginPackage());
				}


			}
			else if (Button1.Text == "再次登录")
			{

			}
		}

		public void Initialization(string Account, string Password)
		{
			Data.QQ.Account = Account;
			Data.QQ.LongQQ = long.Parse(Data.QQ.Account);
			Data.QQ.UTF8 = Encoding.UTF8.GetBytes(Data.QQ.Account);
			Data.QQ.user = Data.HexStrToByteArray(Data.QQ.LongQQ.ToString("X"));
			Data.QQ.pass = Password;
			Data.QQ.md5_1 = Data.MD5Hash(Encoding.UTF8.GetBytes(Data.QQ.pass));
			byte[] md5_byte = Data.QQ.md5_1.Concat(new byte[] {0, 0, 0, 0}).Concat(Data.QQ.user).ToArray();
			Data.QQ.md5_2 = Data.MD5Hash(md5_byte);
			Data.ECDH_Struct _ECDH = ECDH.GetECDHKeys();
			Data.QQ.pub_key = _ECDH.PublicKey;
			Data.QQ.shareKey = _ECDH.Sharekey;
			Data.QQ.prikey = _ECDH.PrivateKey;

			Data.QQ.mRequestID = 10000;
			Data.QQ.key = new byte[16];
			var timestampHex = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();

			Data.QQ.login_Time = Data.HexStrToByteArray(int.Parse(timestampHex).ToString("X"));


			Data.QQ.TGTKey = Data.MD5Hash(Data.QQ.pub_key);
			Data.QQ.randKey = Data.MD5Hash(Data.QQ.shareKey);
			Data.QQ.MsgCookies = Data.GetRandByteArray(4);
			Data.QQ.Appid = 537065990;
			Data.UN_Tlv.T108_ksid = Data.HexStrToByteArray("93AC689396D57E5F9496B81536AAFE91");
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
				Data.QQ.SyncCoookies = ms.ToArray();
			}

			Data.Device.imei = "865166024867445";
			Data.Device.Imsi = "460001330114682";
			Data.Device.WIFIByteSSID = Data.MD5Hash(Encoding.UTF8.GetBytes("5c:11:21:11:19:1f"));
			Data.Device.WIFISSID = "dlb";
			Data.Device.Ver = "|" + Data.Device.imei + "|A8.5.0.4003a808"; //手机串号加QQ版本
			Data.Device.Version = Encoding.UTF8.GetBytes("A8.5.0.4003a808");
			Data.Device.MacBytes = Encoding.UTF8.GetBytes("DA-EB-D5-1C-7B-CD");
			Data.Device.MacId = "84:18:38:38:96:36";
			Data.Device.GUIDBytes = Data.MD5Hash(Encoding.UTF8.GetBytes("b7981398-337d-4d2c-ab64-22b5b6f297dc"));
			Data.Device.AndroidID = Data.MD5Hash(Encoding.UTF8.GetBytes("95dcc49a9434f65a"));
			Data.Device.AppId = 537042771;
			Data.Device.os_type = "android"; //'安卓版本
			Data.Device.os_version = "5.1.1";
			Data.Device.network_type = "China Mobile GSM";
			Data.Device.apn = "wifi";
			Data.Device.model = "oppo r9 plustm a"; //手机型号
			Data.Device.brands = "oppo"; //手机品牌
			Data.Device.Apk_Id = "com.tencent.mobileqq";
			Data.Device.Apk_V = "8.5.0"; //安卓版本
			Data.Device.ApkSig = new byte[] {0xA6, 0xB7, 0x45, 0xBF, 0x24, 0xA2, 0xC2, 0x77, 0x52, 0x77, 0x16, 0xF6, 0xF3, 0x6E, 0xB6, 0x8D}; //固定app_sign

		}

		private static void ExtractEmbeddedResource(string outputDir, string resourceLocation, List<string> files)
		{
			foreach (string file in files)
			{
				if (File.Exists(file))
					break;
				using (System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file))
				{
					using (System.IO.FileStream fileStream = new System.IO.FileStream(System.IO.Path.Combine(outputDir, file), System.IO.FileMode.Create))
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
		private void Form1_Load(object sender, EventArgs e)
		{
			MyInstance = this;
			ExtractEmbeddedResource(Application.StartupPath, Assembly.GetExecutingAssembly().GetName().Name.Replace("-", "_") +".Files", new List<string> { "libeay32.dll", "node.dll", "test.amr" });
		}

		private void Button3_Click(object sender, EventArgs e)
		{

			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			var WithdrawInfo = FriendMsg.SendFriendMsg(long.Parse(TextBox6.Text), Encoding.UTF8.GetBytes(RichTextBox2.Text), Data.MsgType.TextMsg); //文字消息
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			//JceStructSDK.GetNick(SendQQ)
			GroupMsg.SendGroupMsg(Convert.ToInt64(TextBox5.Text), Encoding.UTF8.GetBytes(RichTextBox2.Text), Data.MsgType.TextMsg); //文字消息
		}

		private void Button4_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
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
				Data.FilePath = openFileDialog.FileName;
			}
			if (picBytes == null)
			{
				return;
			}
			var picMd5 = Data.MD5Hash(picBytes);
			Data.FileHash = picMd5;
			Data.FileBytes = picBytes;
			SendPicMsgStruct PicInfo = new SendPicMsgStruct
			{
				Amount = 1,
				PicType = 3,
				PicInfo = new PicInfos
				{
					thisQQ = Data.QQ.LongQQ,
					SendQQ = long.Parse(TextBox6.Text),
					StartFlag = 0,
					PicHash = picMd5,
					PicLengh = picBytes.Length,
					PicName = Data.HashMD5(picBytes) + ".jpg",
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
				var bytes = Data.PackCmdHeader("LongConn.OffPicUp", ms.ToArray());
				Data.TClient.SendData(Data.PackAllHeader(bytes));
			}
		}

		private void Button5_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
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
				Data.FilePath = openFileDialog.FileName;
			}
			if (picBytes == null)
			{
				return;
			}
			var picMd5 = Data.MD5Hash(picBytes);
			Data.FileHash = picMd5;
			Data.FileBytes = picBytes;
			SendGroupPicMsgStruct PicInfo = new SendGroupPicMsgStruct
			{
				Amount = 1,
				PicType = 3,
				GroupPicInfo = new GroupPicInfos
				{
					GroupId = long.Parse(TextBox5.Text),
					SendQQ = Data.QQ.LongQQ,
					StartFlag = 0,
					PicHash = picMd5,
					PicSize = picBytes.Length,
					PicName = Data.HashMD5(picBytes) + ".jpg",
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
				var bytes = Data.PackCmdHeader("ImgStore.GroupPicUp", ms.ToArray());
				Data.TClient.SendData(Data.PackAllHeader(bytes));
			}
		}


		private void TextBox6_TextChanged(object sender, EventArgs e)
		{
			Data.SendQQ = long.Parse(TextBox6.Text);
		}

		private void TextBox5_TextChanged(object sender, EventArgs e)
		{
			Data.GroupId = long.Parse(TextBox5.Text);
		}

		private void Button6_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			byte[] AudioBytes = File.ReadAllBytes(Application.StartupPath + "\\test.amr");
			using (var audioStream = new MemoryStream(AudioBytes))
			{
				AudioBytes = audioStream.ToArray();
			}
			Data.FileHash = Data.MD5Hash(AudioBytes);
			SendGroupAudioMsgStruct AudioInfo = new SendGroupAudioMsgStruct
			{
				AudioType = 3,
				Field2 = 3,
				GroupAudioInfo = new GroupAudioInfos
				{
					GroupId = long.Parse(TextBox5.Text),
					SendQQ = Data.QQ.LongQQ,
					StartFlag = 0,
					AudioHash = Data.FileHash,
					AudioSize = AudioBytes.Length,
					AudioName = Data.HashMD5(AudioBytes) + ".amr",
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
				var bytes = Data.PackCmdHeader("PttStore.GroupPttUp", ms.ToArray());
				Data.TClient.SendData(Data.PackAllHeader(bytes));
			}
		}

		private void Button7_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			byte[] AudioBytes = File.ReadAllBytes(Application.StartupPath + "\\test.amr");
			using (var audioStream = new MemoryStream(AudioBytes))
			{
				AudioBytes = audioStream.ToArray();
			}
			Data.FileHash = Data.MD5Hash(AudioBytes);
			SendAudioMsgStruct AudioInfo = new SendAudioMsgStruct
			{
				AudioType = 500,
				Field2 = 0,
				AudioInfo = new AudioInfos
				{
					thisQQ = Data.QQ.LongQQ,
					SendQQ = long.Parse(TextBox6.Text),
					StartFlag = 2,
					AudioSize = AudioBytes.Length,
					AudioName = Data.HashMD5(AudioBytes) + ".amr",
					AudioHash = Data.FileHash
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
				var bytes = Data.PackCmdHeader("PttCenterSvr.pb_pttCenter_CMD_REQ_APPLY_UPLOAD-500", ms.ToArray());
				Data.TClient.SendData(Data.PackAllHeader(bytes));
			}
		}

		private void Button9_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			var WithdrawInfo = new Data.FriendWithdrawInfo();
			FriendMsg.WithdrawFriendMsg(Data.QQ.LongQQ, long.Parse(TextBox6.Text), Data.FriendWithdraw.MsgReqId, Data.FriendWithdraw.MsgRandomId, Data.FriendWithdraw.MsgTimeStamp);
		}

		private void Button8_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			GroupMsg.WithdrawGroupMsg(long.Parse(TextBox5.Text), Data.GroupWithdraw.MsgReqId, Data.GroupWithdraw.MsgRandomId);
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
			var zipByte = Data.CompressData(Encoding.UTF8.GetBytes(xmlMsg));
			GroupMsg.SendGroupMsg(Convert.ToInt64(TextBox5.Text), zipByte, Data.MsgType.XmlMsg);
		}

		private void Button14_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}

			var xmlMsg = "<?xml version='1.0' encoding='UTF-8' standalone='yes' ?><msg serviceID=\"83\" templateID=\"12345\" action=\"web\" brief=\"标题\" sourceMsgId=\"0\" url=\"https://post.mp.qq.com/group/article/33303433373836353238-35707230.html?_wv\" flag=\"3\" adverSign=\"0\" multiMsgFlag=\"0\"><item layout=\"2\" advertiser_id=\"0\" aid=\"0\"><picture cover=\"http://ww3.sinaimg.cn/mw690/96174781gw1fblx4dxa0lj20dw0dwmxp.jpg\" w=\"0\" h=\"0\" /><title>用户名</title><summary>钱包:余额</summary></item><source name=\"排名:第几名 \" icon=\"\" action=\"web\" appid=\"0\" /></msg>";
			var zipByte = Data.CompressData(Encoding.UTF8.GetBytes(xmlMsg));
			FriendMsg.SendFriendMsg(Convert.ToInt64(TextBox6.Text), zipByte, Data.MsgType.XmlMsg);
		}

		private void Button17_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			JceStructSDK.GetFriendList(0, 30); //取好友列表
		}

		private void Button18_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			JceStructSDK.GetGroupList(); //取群列表
		}

		private void Button15_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			JceStructSDK.GetGroupMemberList(long.Parse(TextBox5.Text));
		}

		private void Button19_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
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
			TextBox1.Text = Data.GetText();
		}

		private void TextBox2_DoubleClick(object sender, EventArgs e)
		{
			TextBox2.Text = Data.GetText();
		}

		private void Button21_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			ProtoSDK.ShutAll(long.Parse(TextBox5.Text), Data.Mute.Close);
		}

		private void Button22_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
			{
				return;
			}
			ProtoSDK.ShutAll(long.Parse(TextBox5.Text), Data.Mute.Open);
		}

        private void Button16_Click(object sender, EventArgs e)
        {

        }
    }



}