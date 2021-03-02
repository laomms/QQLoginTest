
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

using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ProtoBuf;
using zlib;
using System.Globalization;

namespace QQSDK
{
	public static class API
	{
		public static long ThisQQ;
		public static long SendQQ;
		public static long GroupId;
		public static TCPIPClient TClient = new TCPIPClient(Dns.GetHostEntry("msfwifi.3g.qq.com").AddressList[0].ToString(), 8080);
		public static byte[] RecBytes;
		public static QQ_Information QQ = new QQ_Information();
		public static FriendWithdrawInfo FriendWithdraw = new FriendWithdrawInfo();
		public static GroupWithdrawInfo GroupWithdraw = new GroupWithdrawInfo();
		public static DeviceInfo Device = new DeviceInfo();
		public static string last_error;
		public static long mRequestID = 32597;
		public static string NickName = "";		
		public static string FilePath;
		public static byte[] FileBytes;
		public static byte[] FileHash;
		public static CookieContainer mycookiecontainer = new CookieContainer();
		public static string RedirectUrl = "";
		public static UN_Tlv_list UN_Tlv = new UN_Tlv_list();
		public static string getLastError()
		{
			return last_error;
		}
		public struct ECDH_Struct
		{
			public byte[] PrivateKey;
			public byte[] PublicKey;
			public byte[] Sharekey;
		}
		public enum LoginState
		{
			Logining = 0,
			LoginVertify = 1,
			LoginSccess = 2,
			LoginFaild = 3
		}
		public enum OnlineStaus
		{
			online = 11,
			leave = 31,
			hide = 41,
			busy = 51,
			QMe = 61,
			NoDisturb = 71
		}
		public enum MsgType
		{
			TextMsg = 1,
			PicMsg = 2,
			AudioMsg = 3,
			XmlMsg = 4,
			JsonMsg = 5
		}
		public enum Mute
		{
			Open = 1,
			Close = 0
		}
		public struct QQ_Information
		{
			public string Account;
			public long LongQQ;
			public byte[] user;
			public byte[] UTF8;
			public string pass;
			public byte[] md5_1;
			public byte[] md5_2;
			public byte[] login_Time;
			public string NickName;
			public byte[] Exkey;
			public byte[] TGTKey;
			public byte[] shareKey;
			public byte[] pub_key;
			public byte[] prikey;
			public byte[] randKey;
			public string stweb;
			public int mRequestID;
			public int Appid;
			public int IfSuccessful;
			public byte[] VToken;
			public byte[] VToken2;
			public byte[] VerificationCode;
			public byte[] ScanKey010E;
			public byte[] ScanKey0134;
			public byte[] DeviceID;
			public byte[] DeviceToken;
			public string Capcd;
			public string VerifyUrl;
			public byte[] key;
			public string Cookies;
			public struct_pskey pskey;
			public byte[] superkey;
			public int loginState;
			public byte[] MsgCookies;
			public byte[] SyncCoookies;
			public string apk_version; //协议版本
			public string Ticket;
		}
		public struct DeviceInfo
		{
			public byte[] GUIDBytes;
			public byte[] GUID_MD5;
			public string imei;
			public string Ver;
			public byte[] Version;
			public string MacId;
			public byte[] MacBytes;
			public byte[] AndroidID;
			public string Imsi;
			public byte[] WIFIByteSSID;
			public string WIFISSID;
			public string Phone;
			public long AppId;
			public string os_type;
			public string os_version;
			public string network_type;
			public string apn;
			public string model;
			public string brands;
			public string Apk_Id;
			public string Apk_V;
			public byte[] ApkSig;
			public long BuildTime;
			public string SdkVersion;
			public long mMiscBitmap;
			public long mSubSigmap;
			public long main_sig_map;
		}
		public struct UN_Tlv_list
		{
			public byte[] T103_clientkey;
			public byte[] T104;
			public byte[] T108_ksid;
			public byte[] T120_skey;
			public byte[] T174;
			public string T178_phone;
			public byte[] T179;
			public string T17D_url_aq;
			public byte[] T305_SessionKey;
			public byte[] T402;
			public byte[] T403;
			public string T204_url_safeVerify;
			public string T17E_message;
			public byte[] T10A_token_A4;
			public byte[] T143_token_A2;
			public string T192_captcha;
			public string T165_pic_reason;
			public byte[] T105_pic_Viery;
			public byte[] T105_pic_token;
		}
		public struct VerifyMessage
		{
			public string Msg;
			public string Phone;
			public string Url;
			public string str_url;
			public byte[] SecondVerify;
		}

		public struct FriendWithdrawInfo
		{
			public long MsgReqId;
			public long MsgRandomId;
			public long MsgTimeStamp;
		}
		public struct GroupWithdrawInfo
		{
			public long MsgReqId;
			public long MsgRandomId;
		}

		public struct struct_pskey
		{
			public string weiyun;
			public string docs_qq;
			public string tenpay;
			public string openmobile_qq;
			public string qun_qq;
			public string game_qq;
			public string mail_qq;
			public string exmail_qq;
			public string qzone;
			public string qzone_qq;
			public string connect_qq;
			public string imgcache_qq;
			public string hall_qq;
			public string ivac_qq;
			public string vip_qq;
			public string gamecenter_qq;
			public string haoma_qq;
			public string b_qq;
			public string openmobile;
			public string lol_qq;
		}

		public static byte[] PackCmdHeader(string cmd, byte[] protoBytes)
		{
			var cmdbytes = BitConverter.GetBytes(cmd.Length + 4).Reverse().ToArray().Concat(Encoding.UTF8.GetBytes(cmd)).ToArray().Concat(new byte[] {0, 0, 0, 8}).ToArray().Concat(QQ.MsgCookies).ToArray().Concat(new byte[] {0, 0, 0, 4}).ToArray();
			cmdbytes = BitConverter.GetBytes(cmdbytes.Length + 4).Reverse().ToArray().Concat(cmdbytes).ToArray(); //拼包识别命令
			if (protoBytes == null)
			{
				var bytes = BitConverter.GetBytes(4).Reverse().ToArray().Concat(new byte[0]).ToArray(); //拼包消息
				return cmdbytes.Concat(bytes).ToArray();
			}
			else
			{
				var bytes = BitConverter.GetBytes(protoBytes.Length + 4).Reverse().ToArray().Concat(protoBytes).ToArray(); //拼包消息
				return cmdbytes.Concat(bytes).ToArray();
			}

		}
		public static byte[] PackAllHeader(byte[] bodyBytes)
		{
			HashTea Hash = new HashTea();
			byte[] encodeByte = Hash.HashTEA(bodyBytes, UN_Tlv.T305_SessionKey, 0, true); //加密包体
			if (QQ.mRequestID > 2147483647)
			{
				QQ.mRequestID = 10000;
			}
			else
			{
				QQ.mRequestID += 1;
			}
			var headerBytes = new byte[] {0, 0, 0, 0xB, 1}.Concat(BitConverter.GetBytes(QQ.mRequestID).Reverse().ToArray()).ToArray().Concat(new byte[] {0, 0, 0}).ToArray();
			var qqBytes = BitConverter.GetBytes(Convert.ToInt16(QQ.UTF8.Length + 4)).Reverse().ToArray().Concat(QQ.UTF8).ToArray();
			headerBytes = headerBytes.Concat(qqBytes).ToArray();

			var bytes = headerBytes.Concat(encodeByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();
			return bytes;
		}
		public static byte[] GetRandByteArray(int size)
		{
			Random rnd = new Random();
			byte[] b = new byte[size];
			rnd.NextBytes(b);
			return b;
		}
		public static int IPToInt32(string IP)
		{
			if (IP.Contains(".") == false)
			{
				return 0;
			}
			int[] IntArray = IP.Split('.').Select((n) => Convert.ToInt32(n)).ToArray();
			byte[] bytes = IntArray.Select((x) => Convert.ToByte(x)).ToArray();
			var str = BitConverter.ToString(bytes).Replace("-", "");
			return Convert.ToInt32(str, 16);
		}
		public static string Int32ToIP(int IP)
		{
			if (IP == 0)
			{
				return "0";
			}
			var bytes = BitConverter.GetBytes(IP).ToArray();
			return string.Join(".", bytes.Select((i) => i.ToString()).ToArray());
		}
		public static long Gid2Int(long GroupId)
		{
			var i = int.Parse(GroupId.ToString().Substring(0, GroupId.ToString().Length - 6));
			if (i <= 10)
			{
				i = 202 + i;
			}
			else if (i <= 19)
			{
				i = 480 + i - 11;
			}
			else if (i <= 66)
			{
				i = 2100 + i - 20;
			}
			else if (i <= 156)
			{
				i = 2010 + i - 67;
			}
			else if (i <= 209)
			{
				i = 2147 + i - 157;
			}
			else if (i <= 309)
			{
				i = 4100 + i - 210;
			}
			else if (i <= 499)
			{
				i = 3800 + i - 310;
			}
			return long.Parse(i.ToString() + GroupId.ToString().Substring(GroupId.ToString().Length - 6, 6));
		}
		public static long Int2Gid(long GId)
		{
			var i = int.Parse(GId.ToString().Substring(0, GId.ToString().Length - 6));
			if (i >= 202 && i <= 212)
			{
				i = i - 202;
			}
			else if (i >= 480 && i <= 488)
			{
				i = i - 480 + 11;
			}
			else if (i >= 2100 && i <= 2144)
			{
				i = i - 2100 + 20;
			}
			else if (i >= 2010 && i <= 2099)
			{
				i = i - 2010 + 67;
			}
			else if (i >= 2147 && i <= 2198)
			{
				i = i - 2147 + 157;
			}
			else if (i >= 4100 && i <= 4199)
			{
				i = i - 3800 + 310;
			}
			return long.Parse(i.ToString() + GId.ToString().Substring(GId.ToString().Length - 6, 6));
		}
		public static string FromHexString(string hexString)
		{
			var bytes = new byte[(hexString.Length / 2)];
			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			}
			return Encoding.UTF8.GetString(bytes); 
		}
		public static string ToHexString(string str)
		{
			var sb = new StringBuilder();
			var bytes = Encoding.UTF8.GetBytes(str);
			foreach (var t in bytes)
			{
				sb.Append(t.ToString("X2"));
			}
			return sb.ToString();
		}
		public static byte[] HexStrToByteArray(string str)
		{
			Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
			for (int i = 0; i <= 255; i++)
			{
				hexindex.Add(i.ToString("X2"), (byte)i);
			}
			if (str.Length % 2 == 1)
			{
				str = "0" + str;
			}
			List<byte> hexres = new List<byte>();
			for (int i = 0; i < str.Length; i += 2)
			{
				hexres.Add(hexindex[str.Substring(i, 2)]);
			}
			return hexres.ToArray();
		}
		public static byte[] StringToByteArray(string hexString)
		{
			if (hexString.Length % 2 != 0)
			{
				throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
			}

			byte[] data = new byte[hexString.Length / 2];
			for (int index = 0; index < data.Length; index++)
			{
				string byteValue = hexString.Substring(index * 2, 2);
				data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}

			return data;
		}
		public static byte[] MD5Hash(byte[] bytesToHash)
		{
			System.Security.Cryptography.MD5CryptoServiceProvider md5Obj = new System.Security.Cryptography.MD5CryptoServiceProvider();
			return md5Obj.ComputeHash(bytesToHash);
		}
		public static string HashMD5(byte[] ByteIn)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(ByteIn);

			StringBuilder strBuilder = new StringBuilder();

			for (int i = 0; i < result.Length; i++)
			{
				strBuilder.Append(result[i].ToString("x2").ToUpper());
			}

			return strBuilder.ToString();
		}			
		public static int SearchBytes(byte[] src, byte[] pattern)
		{
			int c = src.Length - pattern.Length + 1;
			int j = 0;
			for (int i = 0; i < c; i++)
			{
				if (src[i] != pattern[0])
				{
					continue;
				}
				j = pattern.Length - 1;
				while (j >= 1 && src[i + j] == pattern[j])
				{

					j -= 1;
				}
				if (j == 0)
				{
					return i;
				}
			}
			return -1;
		}
		public static byte[] CompressData(byte[] inData)
		{
			using (MemoryStream outMemoryStream = new MemoryStream())
			{
				using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
				{
					using (Stream inMemoryStream = new MemoryStream(inData))
					{
						CopyStream(inMemoryStream, outZStream);
						outZStream.finish();
						return outMemoryStream.ToArray();
					}
				}
			}
			return null;
		}
		public static byte[] DecompressData(byte[] inData)
		{
			using (MemoryStream outMemoryStream = new MemoryStream())
			{
				using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
				{
					using (Stream inMemoryStream = new MemoryStream(inData))
					{
						CopyStream(inMemoryStream, outZStream);
						outZStream.finish();
						return outMemoryStream.ToArray();
					}
				}
			}
			return null;
		}
		public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
		{
			byte[] buffer = new byte[2000];
			int len = input.Read(buffer, 0, 2000);
			while (len > 0)
			{
				output.Write(buffer, 0, len);
				len = input.Read(buffer, 0, 2000);
			}
			output.Flush();
		}
		public static void SetText(string p_Text)
		{
			Thread STAThread = new Thread(() =>
			{
												 System.Windows.Forms.Clipboard.SetText(p_Text);
			});
			STAThread.SetApartmentState(ApartmentState.STA);
			STAThread.Start();
			STAThread.Join();
		}
		public static string GetText()
		{
			string ReturnValue = string.Empty;
			Thread STAThread = new Thread(() =>
			{
												 ReturnValue = System.Windows.Forms.Clipboard.GetText();
			});
			STAThread.SetApartmentState(ApartmentState.STA);
			STAThread.Start();
			STAThread.Join();
			return ReturnValue;
		}
		public static void reLogin()
		{
			if (UN_Tlv.T143_token_A2 != null && QQ.shareKey != null && UN_Tlv.T10A_token_A4 != null)
			{
				var timestampHex = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
				QQ.login_Time = HexStrToByteArray(int.Parse(timestampHex).ToString("X"));
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
					QQ.SyncCoookies = ms.ToArray();
				}	
				if (TClient.IsConnected()==false )
					TClient = new TCPIPClient(Dns.GetHostEntry("msfwifi.3g.qq.com").AddressList[0].ToString(), 8080);
				TClient.SendData(Pack.PackOnlineStatus("StatSvc.register", 1));
			}
		}
	
		public static void Initialization(string Account, string Password)
		{
			API.QQ.Account = Account;
			API.QQ.LongQQ = long.Parse(API.QQ.Account);
			API.QQ.UTF8 = Encoding.UTF8.GetBytes(API.QQ.Account);
			API.QQ.user = API.HexStrToByteArray(API.QQ.LongQQ.ToString("X"));
			API.QQ.pass = Password;
			API.QQ.md5_1 = API.MD5Hash(Encoding.UTF8.GetBytes(API.QQ.pass));
			byte[] md5_byte = API.QQ.md5_1.Concat(new byte[] { 0, 0, 0, 0 }).Concat(API.QQ.user).ToArray();
			API.QQ.md5_2 = API.MD5Hash(md5_byte);
			API.ECDH_Struct _ECDH = ECDH.GetECDHKeys();
			API.QQ.pub_key = _ECDH.PublicKey;
			API.QQ.shareKey = _ECDH.Sharekey;
			API.QQ.prikey = _ECDH.PrivateKey;

			API.QQ.mRequestID = 10000;
			API.QQ.key = new byte[16];
			var timestampHex = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();

			API.QQ.login_Time = API.HexStrToByteArray(int.Parse(timestampHex).ToString("X"));


			API.QQ.TGTKey = API.MD5Hash(API.QQ.pub_key);
			API.QQ.randKey = API.MD5Hash(API.QQ.shareKey);
			API.QQ.MsgCookies = API.GetRandByteArray(4);
			API.QQ.Appid = 537065990;
			API.UN_Tlv.T108_ksid = API.HexStrToByteArray("93AC689396D57E5F9496B81536AAFE91");
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

			API.Device.imei = "865166024867445";
			API.Device.Imsi = "460001330114682";
			API.Device.WIFIByteSSID = API.MD5Hash(Encoding.UTF8.GetBytes("5c:11:21:11:19:1f"));
			API.Device.WIFISSID = "dlb";
			API.Device.Ver = "|" + API.Device.imei + "|A8.5.0.4003a808"; //手机串号加QQ版本
			API.Device.Version = Encoding.UTF8.GetBytes("A8.5.0.4003a808");
			API.Device.MacBytes = Encoding.UTF8.GetBytes("DA-EB-D5-1C-7B-CD");
			API.Device.MacId = "84:18:38:38:96:36";
			API.Device.GUIDBytes = API.MD5Hash(Encoding.UTF8.GetBytes("b7981398-337d-4d2c-ab64-22b5b6f297dc"));
			API.Device.AndroidID = API.MD5Hash(Encoding.UTF8.GetBytes("95dcc49a9434f65a"));
			API.Device.AppId = 537042771;
			API.Device.os_type = "android"; //'安卓版本
			API.Device.os_version = "5.1.1";
			API.Device.network_type = "China Mobile GSM";
			API.Device.apn = "wifi";
			API.Device.model = "oppo r9 plustm a"; //手机型号
			API.Device.brands = "oppo"; //手机品牌
			API.Device.Apk_Id = "com.tencent.mobileqq";
			API.Device.Apk_V = "8.5.0"; //安卓版本
			API.Device.ApkSig = new byte[] { 0xA6, 0xB7, 0x45, 0xBF, 0x24, 0xA2, 0xC2, 0x77, 0x52, 0x77, 0x16, 0xF6, 0xF3, 0x6E, 0xB6, 0x8D }; //固定app_sign

		}
		public static void SendFriendPic(long thisQQ, long sendQQ, byte[] picBytes)
		{
			var picMd5 = API.MD5Hash(picBytes);
			API.FileHash = picMd5;
			API.FileBytes = picBytes;
			SendPicMsgStruct PicInfo = new SendPicMsgStruct
			{
				Amount = 1,
				PicType = 3,
				PicInfo = new PicInfos
				{
					SendQQ = sendQQ,
					StartFlag = 0,
					PicHash = picMd5,
					PicLengh = picBytes.Length,
					PicName = API.HashMD5(picBytes) + ".jpg",
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
				var bytes = API.PackCmdHeader("LongConn.OffPicUp", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}
		}
		public static void SendFriendAudio(long thisQQ, long sendQQ, byte[] AudioBytes)
		{
			using (var audioStream = new MemoryStream(AudioBytes))
			{
				AudioBytes = audioStream.ToArray();
			}
			API.FileHash = API.MD5Hash(AudioBytes);
			SendAudioMsgStruct AudioInfo = new SendAudioMsgStruct
			{
				AudioType = 500,
				Field2 = 0,
				AudioInfo = new AudioInfos
				{
					thisQQ = thisQQ,
					SendQQ = sendQQ,
					StartFlag = 2,
					AudioSize = AudioBytes.Length,
					AudioName = API.HashMD5(AudioBytes) + ".amr",
					AudioHash = API.FileHash
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
				var bytes = API.PackCmdHeader("PttCenterSvr.pb_pttCenter_CMD_REQ_APPLY_UPLOAD-500", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}
		}
		public static void SendGroupPic(long thisQQ, long sendQQ, long groupId, byte[] picBytes)
		{
			var picMd5 = API.MD5Hash(picBytes);
			API.FileHash = picMd5;
			API.FileBytes = picBytes;
			SendGroupPicMsgStruct PicInfo = new SendGroupPicMsgStruct
			{
				Amount = 1,
				PicType = 3,
				GroupPicInfo = new GroupPicInfos
				{
					GroupId = groupId,
					SendQQ = sendQQ,
					StartFlag = 0,
					PicHash = picMd5,
					PicSize = picBytes.Length,
					PicName = API.HashMD5(picBytes) + ".jpg",
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
				var bytes = API.PackCmdHeader("ImgStore.GroupPicUp", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}
		}
		public static void SendGroupAudio(long thisQQ, long sendQQ, long groupId, byte[] AudioBytes)
		{
			using (var audioStream = new MemoryStream(AudioBytes))
			{
				AudioBytes = audioStream.ToArray();
			}
			API.FileHash = API.MD5Hash(AudioBytes);
			SendGroupAudioMsgStruct AudioInfo = new SendGroupAudioMsgStruct
			{
				AudioType = 3,
				Field2 = 3,
				GroupAudioInfo = new GroupAudioInfos
				{
					GroupId = groupId,
					SendQQ = sendQQ,
					StartFlag = 0,
					AudioHash = API.FileHash,
					AudioSize = AudioBytes.Length,
					AudioName = API.HashMD5(AudioBytes) + ".amr",
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
				var bytes = API.PackCmdHeader("PttStore.GroupPttUp", ms.ToArray());
				API.TClient.SendData(API.PackAllHeader(bytes));
			}
		}

	}

}