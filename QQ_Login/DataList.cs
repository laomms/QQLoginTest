
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

using System.Runtime.InteropServices;
using System.Text;

namespace QQ_Login
{
	public static class DataList
	{
		public static TCPIPClient TClient;
		public static byte[] RecBytes;
		public static QQ_Information QQ = new QQ_Information();
		public static DeviceInfo Device = new DeviceInfo();
		public static string last_error;
		public static long mRequestID = 32597;
		public static string getLastError()
		{
			return last_error;
		}
		public enum LoginState
		{
			Logining = 0,
			LoginVertify = 1,
			LoginSccess = 2,
			LoginFaild = 3
		}
		public struct ECDH_Struct
		{
			public byte[] PrivateKey;
			public byte[] PublicKey;
			public byte[] Sharekey;
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
			public byte[] time;
			public string NickName;
			public byte[] Token0143; //二次登录参数1
			public byte[] Token010A; //二次登录参数2
			public byte[] sessionKey; //二次登录参数2
			public byte[] Exkey;
			public byte[] TGTKey;
			public byte[] shareKey;
			public byte[] pub_key;
			public byte[] prikey;
			public byte[] randKey;
			public string stweb;
			public byte[] sid;
			public byte[] clientkey;
			public int mRequestID;
			public int Appid;
			public int Appid2;
			public int IfSuccessful;
			public byte[] VToken;
			public byte[] VToken2;
			public byte[] VerificationCode;
			public byte[] ScanKey010E;
			public byte[] ScanKey0134;
			public byte[] DeviceID;
			public byte[] DeviceToken;
			public string Capcd;
			public byte[] ksid;
			public byte[] Token016A;
			public byte[] Token0106;
			public byte[] Token0058;
			public byte[] mST1Key;
			public byte[] VieryToken1;
			public byte[] VieryToken2;
			public byte[] Verify;
			public byte[] key;
			public byte[] skey;
			public byte[] pskey;
			public byte[] superkey;
			public byte[] vkey;
			public int loginState;
			public byte[] MsgCookies;
			public byte[] SyncCoookies;
		}
		public struct DeviceInfo
		{
			public string GUID;
			public string imei;
			public string Ver;
			public byte[] Version;
			public string Mac;
			public byte[] MacBytes;
			public string AndroidID;
			public string Imsi;
			public string BSSID;
			public string SSID;
			public string Phone;
			public long AppId;
			public string pc_ver;
			public string os_type;
			public string os_version;
			public string network_type;
			public string apn;
			public string model;
			public string brands;
			public string Apk_Id;
			public string Apk_V;
			public byte[] ApkSig;
		}
		public struct VerifyMessage
		{
			public string Msg;
			public string Phone;
			public string Url;
			public string str_url;
			public byte[] SecondVerify;
		}

		public struct GroupMessage
		{
			public long fromUin;
			public long myUin;
			public int fromReq;
			public int fromRecvTime;
			public long fromGroup;
			public string groupName;
			public string fromCard;
			public int fromSendTime;
			public long fromRandom;
			public int pieceIndex;
			public int pieceCount;
			public long pieceFlag;
			public int subType;
			public string title;
			public string message_content;
			public string reply_info;
			public int buddleId;
			public int lon;
			public int lat;

		}


		public static byte[] GetRandByteArray(int size)
		{
			Random rnd = new Random();
			byte[] b = new byte[size];
			rnd.NextBytes(b);
			return b;
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

		public static string FromHexString(string hexString)
		{
			var bytes = new byte[(hexString.Length / 2)];
			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			}
			return Encoding.UTF8.GetString(bytes); 
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

		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length).Where((x) => x % 2 == 0).Select((x) => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
		}

		public static byte[] MD5Hash(byte[] bytesToHash)
		{
			System.Security.Cryptography.MD5CryptoServiceProvider md5Obj = new System.Security.Cryptography.MD5CryptoServiceProvider();
			return md5Obj.ComputeHash(bytesToHash);
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


	}

}