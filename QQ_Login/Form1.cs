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
using System.IO;
using System.Net;
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
		public struct ECDH_Struct
		{
			public byte[] PrivateKey;
			public byte[] PublicKey;
			public byte[] Sharekey;
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TextBox1.Text) || string.IsNullOrEmpty(TextBox2.Text))
			{
				return;
			}
			if (Button1.Text == "登录")
			{
				DataList.QQ.loginState = (int)DataList.LoginState.Logining;
				RichTextBox1.Text = "【" + DateTime.Now + "】" + "开始登录QQ：" + TextBox1.Text + "\r\n";
				Initialization(TextBox1.Text, TextBox2.Text); //初始化赋值
				var loginPack = Pack.LoginPackage(); //组登陆包
				DataList.QQ.loginState = (int)DataList.LoginState.Logining;

				string host = "msfwifi.3g.qq.com";
				IPHostEntry hostInfo = Dns.GetHostEntry("msfwifi.3g.qq.com");
				var ip = hostInfo.AddressList[0].ToString();
				DataList.TClient = new TCPIPClient(ip, 8080);
				TCPIPClient.socket_send(loginPack);
			}
			else if (Button1.Text == "再次登录")
			{
				var PackImage = Pack.Pack_VieryImage(textBox3.Text);
				if (DataList.QQ.mRequestID > 2147483647)
				{
					DataList.QQ.mRequestID = 10000;
				}
				else
				{
					DataList.QQ.mRequestID += 1;
				}
				TCPIPClient.SendData(PackImage);

			}
		}

		public void Initialization(string Account, string Password)
		{
			DataList.QQ.Account = Account;
			DataList.QQ.LongQQ = long.Parse(DataList.QQ.Account);
			DataList.QQ.UTF8 = Encoding.UTF8.GetBytes(DataList.QQ.Account);
			DataList.QQ.user = DataList.HexStrToByteArray(DataList.QQ.LongQQ.ToString("X"));
			DataList.QQ.pass = Password;
			DataList.QQ.md5_1 = DataList.MD5Hash(Encoding.UTF8.GetBytes(DataList.QQ.pass));
			byte[] md5_byte = DataList.QQ.md5_1.Concat(new byte[] {0, 0, 0, 0}).Concat(DataList.QQ.user).ToArray();
			DataList.QQ.md5_2 = DataList.MD5Hash(md5_byte);
			DataList.ECDH_Struct ECDH = GetECDHKeys();
			DataList.QQ.pub_key = ECDH.PublicKey;
			DataList.QQ.shareKey = ECDH.Sharekey;
			DataList.QQ.prikey = ECDH.PrivateKey;

			DataList.QQ.mRequestID = 10000;
			DataList.QQ.key = new byte[16];
			var timestampHex = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
			DataList.QQ.time = DataList.HexStrToByteArray(int.Parse(timestampHex).ToString("X"));

			DataList.QQ.TGTKey = DataList.MD5Hash(DataList.QQ.pub_key);
			DataList.QQ.randKey = DataList.MD5Hash(DataList.QQ.shareKey);
			DataList.QQ.MsgCookies = DataList.GetRandByteArray(4);
			DataList.QQ.Appid = 537065990;
			DataList.QQ.Appid2 = 537065990;
			DataList.QQ.ksid = DataList.HexStrToByteArray("93AC689396D57E5F9496B81536AAFE91");
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
				Debug.Print("Serializer" + "\r\n" + BitConverter.ToString(ms.ToArray()).Replace("-", ""));
				DataList.QQ.SyncCoookies = ms.ToArray();
			}


			DataList.Device.imei = "865166024867445";
			DataList.Device.Imsi = "460001330114682";
			DataList.Device.BSSID = "41D857AFBD54DC49DB4214447D095D13";
			DataList.Device.SSID = "dlb";
			DataList.Device.Ver = "|865166024867445|A8.4.10.b8c39faf"; //手机串号加QQ版本
			DataList.Device.Version = Encoding.UTF8.GetBytes("5.8.0.2505");
			DataList.Device.MacBytes = new Byte[] {0x54, 0x44, 0x61, 0x90, 0xFC, 0x9C, 0x7E, 0x8, 0xC4, 0x13, 0x59, 0x26, 0xB8, 0x73, 0x4B, 0xC2};
			DataList.Device.Mac = "84:18:38:38:96:36";
			DataList.Device.GUID = "C36D03706B7C4EDDC0774691C1FB91F8";
			DataList.Device.AndroidID = "B02E8F4515CEE9413B92F1F8B0694B7B";
			DataList.Device.AppId = 537042771;
			DataList.Device.pc_ver = "1F41";
			DataList.Device.os_type = "android"; //'安卓版本
			DataList.Device.os_version = "5.1.1";
			DataList.Device.network_type = "China Mobile GSM";
			DataList.Device.apn = "wifi";
			DataList.Device.model = "oppo r9 plustm a"; //手机型号
			DataList.Device.brands = "oppo"; //手机品牌
			DataList.Device.Apk_Id = "com.tencent.mobileqq";
			DataList.Device.Apk_V = "8.4.10"; //安卓版本
			DataList.Device.ApkSig = new byte[] {0xA6, 0xB7, 0x45, 0xBF, 0x24, 0xA2, 0xC2, 0x77, 0x52, 0x77, 0x16, 0xF6, 0xF3, 0x6E, 0xB6, 0x8D}; //固定app_sign

		}


		public DataList.ECDH_Struct GetECDHKeys()
		{
			DataList.ECDH_Struct ECDH = new DataList.ECDH_Struct();
			byte[] PrivateKey = new byte[1024];
			byte[] PublicKey = new byte[1024];
			byte[] Sharekey = new byte[16];
			byte[] SvrPubKey = DataList.HexStrToByteArray("04EBCA94D733E399B2DB96EACDD3F69A8BB0F74224E2B44E3357812211D2E62EFBC91BB553098E25E33A799ADC7F76FEB208DA7C6522CDB0719A305180CC54A82E");
			var eckey = OpenSSL.EC_KEY_new_by_curve_name(415);
			if (eckey == IntPtr.Zero)
			{
				return ECDH;
			}
			var res = OpenSSL.EC_KEY_generate_key(eckey);
			var ec_group = OpenSSL.EC_KEY_get0_group(eckey);
			var ec_point = OpenSSL.EC_KEY_get0_public_key(eckey);
			var PublicKeyLen = OpenSSL.EC_POINT_point2oct(ec_group, (System.IntPtr)ec_point, 4, PublicKey, 65, (System.IntPtr)0);
			Array.Resize(ref PublicKey, PublicKeyLen);
			ECDH.PublicKey = PublicKey;
			ec_point = (int)OpenSSL.EC_KEY_get0_private_key(eckey);
			var PrivateKeyLen = OpenSSL.BN_bn2mpi((System.IntPtr)ec_point, PrivateKey);
			Array.Resize(ref PrivateKey, (System.Int32)PrivateKeyLen);
			ECDH.PrivateKey = PrivateKey;
			eckey = OpenSSL.EC_KEY_new_by_curve_name(415);
			if (eckey == IntPtr.Zero)
			{
				return ECDH;
			}
			var bn = OpenSSL.BN_new();
			OpenSSL.BN_mpi2bn(ECDH.PrivateKey, ECDH.PrivateKey.Length, bn);
			OpenSSL.EC_KEY_set_private_key(eckey, bn);
			OpenSSL.BN_free(bn);
			ec_group = OpenSSL.EC_KEY_get0_group(eckey);
			ec_point = (int)OpenSSL.EC_POINT_new(ec_group);
			OpenSSL.EC_POINT_oct2point(ec_group, (System.IntPtr)ec_point, SvrPubKey, SvrPubKey.Length, (System.IntPtr)0);
			OpenSSL.ECDH_compute_key(Sharekey, 16, (System.IntPtr)ec_point, eckey, IntPtr.Zero);
			ECDH.Sharekey = DataList.MD5Hash(Sharekey);
			return ECDH;
		}
		public static string GetMd5Hash(string input)
		{
			MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			MyInstance = this;
			if (File.Exists(Application.StartupPath + "\\libeay32.dll") == false)
			{
				System.IO.File.WriteAllBytes(Application.StartupPath + "\\libeay32.dll", Properties.Resources.libeay32);
			}
		
		}

		private void Button3_Click(object sender, EventArgs e)
		{
			FriendMsg.SendFriendMsg(long.Parse(TextBox6.Text), RichTextBox2.Text);
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			GroupMsg.SendGroupMsg(long.Parse( TextBox5.Text), RichTextBox2.Text);;
		}



		private static Form1 _DefaultInstance;
		public static Form1 DefaultInstance
		{
			get
			{
				if (_DefaultInstance == null || _DefaultInstance.IsDisposed)
					_DefaultInstance = new Form1();

				return _DefaultInstance;
			}
		}
	}



}