
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
//组包

namespace QQSDK
{
	public class Pack
	{
		private static int pc_sub_cmd;

#region 组登陆包
		public static byte[] LoginPackage()
		{
			var bytes = TlvPackage("AndroidQQ");
			//Dim bytes = TLV_bake.PackTlv()
			bytes = PackLoginHeader("wtlogin.login", bytes, 0);
			return bytes;
		}
#endregion
#region 组Tlv包
		public static byte[] TlvPackage(string LoginType = "")
		{
			var bytes = new byte[0];
			if (LoginType == "AndroidQQ")
			{
				bytes = new byte[] {0, 9, 0, 24}; //24个包
				bytes = bytes.Concat(TLV.tlv018(API.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(API.QQ.user, API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(API.QQ.user, API.QQ.md5_1, API.QQ.md5_2, API.QQ.TGTKey, API.Device.GUIDBytes, API.QQ.login_Time, API.QQ.Appid, API.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116(0)).ToArray();
				bytes = bytes.Concat(TLV.tlv100(API.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (API.UN_Tlv.T108_ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(API.UN_Tlv.T108_ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv144(API.QQ.TGTKey, TLV.tlv109(API.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(API.Device.os_type, API.Device.os_version, API.Device.network_type.ToString(), API.Device.apn), TLV.tlv128(API.Device.model, API.Device.GUIDBytes, API.Device.brands), TLV.tlv16e(API.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv145(API.Device.GUIDBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(API.Device.Apk_V, API.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(API.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(API.Device.network_type.ToString(), API.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
				bytes = bytes.Concat(TLV.tlv187(API.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(API.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv194(API.Device.Imsi)).ToArray();
				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(API.Device.WIFIByteSSID, API.Device.WIFISSID)).ToArray();
				bytes = bytes.Concat(TLV.tlv177(API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv516()).ToArray();
				bytes = bytes.Concat(TLV.tlv521()).ToArray();
				bytes = bytes.Concat(TLV.tlv525(1, API.QQ.login_Time, API.QQ.Appid)).ToArray();
				bytes = bytes.Concat(TLV.tlv544()).ToArray();
			}
			else if (LoginType == "HDQQ")
			{
				bytes = new byte[] {0, 9, 0, 0x18};
				bytes = bytes.Concat(TLV.tlv018(API.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(API.QQ.user, API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(API.QQ.user, API.QQ.md5_1, API.QQ.md5_2, API.QQ.TGTKey, API.Device.GUIDBytes, API.QQ.login_Time, (int)API.Device.AppId, API.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116(0)).ToArray();
				bytes = bytes.Concat(TLV.tlv100(API.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (API.UN_Tlv.T108_ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(API.UN_Tlv.T108_ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv144(API.QQ.TGTKey, TLV.tlv109(API.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(API.Device.os_type, API.Device.os_version, API.Device.network_type.ToString(), API.Device.apn), TLV.tlv128(API.Device.model, API.Device.GUIDBytes, API.Device.brands), TLV.tlv16e(API.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv145(API.Device.GUIDBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(API.Device.Apk_V, API.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(API.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(API.Device.network_type.ToString(), API.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
				bytes = bytes.Concat(TLV.tlv187(API.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(API.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv194(API.Device.Imsi)).ToArray();

				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(API.Device.WIFIByteSSID, API.Device.WIFISSID)).ToArray(); // BSSID MD5 WIFISSID/NAME
				bytes = bytes.Concat(TLV.tlv177(API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv516()).ToArray();
				bytes = bytes.Concat(TLV.tlv521()).ToArray();
				bytes = bytes.Concat(TLV.tlv525(1, API.QQ.login_Time, API.QQ.Appid)).ToArray();

			}
			else if (LoginType == "企业QQ")
			{
				bytes = new byte[] {0, 9, 0, 0x16};
				bytes = bytes.Concat(TLV.tlv018(API.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(API.QQ.user, API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(API.QQ.user, API.QQ.md5_1, API.QQ.md5_2, API.QQ.TGTKey, API.Device.GUIDBytes, API.QQ.login_Time, (int)API.Device.AppId, API.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116(0)).ToArray();
				bytes = bytes.Concat(TLV.tlv100(API.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (API.UN_Tlv.T108_ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(API.UN_Tlv.T108_ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv144(API.QQ.TGTKey, TLV.tlv109(API.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(API.Device.os_type, API.Device.os_version, API.Device.network_type.ToString(), API.Device.apn), TLV.tlv128(API.Device.model, API.Device.GUIDBytes, API.Device.brands), TLV.tlv16e(API.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv145(API.Device.GUIDBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(API.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(API.Device.network_type.ToString(), API.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv106(API.QQ.user, API.QQ.md5_1, API.QQ.md5_2, API.QQ.TGTKey, API.Device.GUIDBytes, API.QQ.login_Time, (int)API.Device.AppId, API.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(API.Device.Apk_V, API.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv177(API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv187(API.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(API.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(API.Device.WIFIByteSSID, API.Device.WIFISSID)).ToArray();
				bytes = bytes.Concat(TLV.tlv194(API.Device.Imsi)).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
			}
			else if (LoginType == "企点QQ")
			{
				bytes = new byte[] {0, 9, 0, 0x16};
				bytes = bytes.Concat(TLV.tlv018(API.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(API.QQ.user, API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(API.QQ.user, API.QQ.md5_1, API.QQ.md5_2, API.QQ.TGTKey, API.Device.GUIDBytes, API.QQ.login_Time, API.QQ.Appid, API.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116(0)).ToArray();
				bytes = bytes.Concat(TLV.tlv100(API.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (API.UN_Tlv.T108_ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(API.UN_Tlv.T108_ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv144(API.QQ.TGTKey, TLV.tlv109(API.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(API.Device.os_type, API.Device.os_version, API.Device.network_type.ToString(), API.Device.apn), TLV.tlv128(API.Device.model, API.Device.GUIDBytes, API.Device.brands), TLV.tlv16e(API.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv145(API.Device.GUIDBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(API.Device.Apk_V, API.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(API.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(API.Device.network_type.ToString(), API.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
				bytes = bytes.Concat(TLV.tlv187(API.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(API.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(API.Device.WIFIByteSSID, API.Device.WIFISSID)).ToArray();
				bytes = bytes.Concat(TLV.tlv177(API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv516()).ToArray();
				bytes = bytes.Concat(TLV.tlv194(API.Device.Imsi)).ToArray();
			}
			else
			{
				bytes = new byte[] {0, 9, 0, 0x18};
				bytes = bytes.Concat(TLV.tlv018(API.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(API.QQ.user, API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(API.QQ.user, API.QQ.md5_1, API.QQ.md5_2, API.QQ.TGTKey, API.Device.GUIDBytes, API.QQ.login_Time, (int)API.Device.AppId, API.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116(0)).ToArray();
				bytes = bytes.Concat(TLV.tlv100(API.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (API.UN_Tlv.T108_ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(API.UN_Tlv.T108_ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv144(API.QQ.TGTKey, TLV.tlv109(API.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(API.Device.os_type, API.Device.os_version, API.Device.network_type.ToString(), API.Device.apn), TLV.tlv128(API.Device.model, API.Device.GUIDBytes, API.Device.brands), TLV.tlv16e(API.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv145(API.Device.GUIDBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(API.Device.Apk_V, API.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(API.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(API.Device.network_type.ToString(), API.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
				bytes = bytes.Concat(TLV.tlv187(API.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(API.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv194(API.Device.Imsi)).ToArray();
				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(API.Device.WIFIByteSSID, API.Device.WIFISSID)).ToArray(); // BSSID MD5 WIFISSID/NAME
				bytes = bytes.Concat(TLV.tlv177(API.QQ.login_Time)).ToArray();
				bytes = bytes.Concat(TLV.tlv516()).ToArray();
				bytes = bytes.Concat(TLV.tlv521()).ToArray();
				bytes = bytes.Concat(TLV.tlv525(1, API.QQ.login_Time, API.QQ.Appid)).ToArray();
			}
			HashTea Hash = new HashTea();
			bytes = Hash.HashTEA(bytes, API.QQ.shareKey, 0, true);
			return bytes;
		}
#endregion
#region 组登陆包头
		public static byte[] PackLoginHeader(string servicecmd, byte[] bytesIn, int loginType) //0 = 普通登录 1 = 验证码登录
		{

			var HeaderBytes = BitConverter.GetBytes(API.QQ.mRequestID).Reverse().ToArray();
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(API.QQ.Appid).Reverse().ToArray()).ToArray();
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(API.QQ.Appid).Reverse().ToArray()).ToArray();
			HeaderBytes = HeaderBytes.Concat(new byte[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 4}).ToArray();
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(servicecmd.Length + 4).Reverse().ToArray()).ToArray();
			HeaderBytes = HeaderBytes.Concat(Encoding.UTF8.GetBytes(servicecmd)).ToArray();
			HeaderBytes = HeaderBytes.Concat(new byte[] {0, 0, 0, 8}).ToArray();
			if (loginType == 0)
			{
				HeaderBytes = HeaderBytes.Concat(new byte[] {0x6C, 0x6A, 0xED, 0x65}).ToArray();
			}
			else if (loginType == 1)
			{
				HeaderBytes = HeaderBytes.Concat(new byte[] {0xB, 0x63, 0xE0, 0x82}).ToArray();
			}
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(API.Device.imei.Length + 4).Reverse().ToArray()).ToArray();
			HeaderBytes = HeaderBytes.Concat(Encoding.UTF8.GetBytes(API.Device.imei)).ToArray();
			HeaderBytes = HeaderBytes.Concat(new byte[] {0, 0, 0, 4}).ToArray();
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(Convert.ToInt16(API.Device.Ver.Length + 2)).Reverse().ToArray()).ToArray();
			HeaderBytes = HeaderBytes.Concat(Encoding.UTF8.GetBytes(API.Device.Ver)).ToArray();
			HeaderBytes = HeaderBytes.Concat(new byte[] {0, 0, 0, 0x2A}).ToArray();
			HeaderBytes = HeaderBytes.Concat(Encoding.UTF8.GetBytes("b$1ebc85de7365de4d155ce40110001581471d")).ToArray();
			var HeaderLen = BitConverter.GetBytes(Convert.ToInt16(HeaderBytes.Length + 4)).Reverse().ToArray();
			HeaderBytes = new byte[] {0, 0}.Concat(HeaderLen).ToArray().Concat(HeaderBytes).ToArray();


			var bytes2 = new byte[] {0x1F, 0x41, 8, 0x10, 0, 1};
			bytes2 = bytes2.Concat(API.QQ.user).ToArray();
			if (loginType == 0)
			{
				bytes2 = bytes2.Concat(new byte[] {0x3, 0x87, 0x0, 0x0, 0x0, 0x0, 0x2, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x2, 0x1}).ToArray();
			}
			else if (loginType == 1)
			{
				bytes2 = bytes2.Concat(new byte[] {0x3, 0x7, 0x0, 0x0, 0x0, 0x0, 0x2, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x2, 0x1}).ToArray();
			}
			bytes2 = bytes2.Concat(API.GetRandByteArray(16)).ToArray();
			bytes2 = bytes2.Concat(new byte[] {1, 0x31, 0, 1}).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0, 0x41}).ToArray();
			bytes2 = bytes2.Concat(API.QQ.pub_key).ToArray(); //65字节
			bytes2 = bytes2.Concat(bytesIn).ToArray(); //1392
			var BodyLen = BitConverter.GetBytes(Convert.ToInt16(bytes2.Length + 4)).Reverse().ToArray();
			bytes2 = new byte[] {2}.Concat(BodyLen).ToArray().Concat(bytes2).ToArray().Concat(new byte[] {3}).ToArray();

			var bytes = HeaderBytes.Concat(BitConverter.GetBytes(bytes2.Length + 4).Reverse().ToArray()).ToArray().Concat(bytes2).ToArray();

			//Debug.Print("PackLoginHeader:" + bytes.Length.ToString + Environment.NewLine  + BitConverter.ToString(bytes).Replace("-", " "))
			HashTea Hash = new HashTea();
			byte[] EncodeByte = Hash.HashTEA(bytes, API.QQ.key, 0, true);
			//Debug.Print("EncodeByte:" + EncodeByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(EncodeByte).Replace("-", " "))
			var retByte = PackHeader(EncodeByte, loginType);
			return retByte;
		}

#endregion
#region 组包头
		public static byte[] PackHeader(byte[] bytesIn, int loginType)
		{
			byte[] bytes = new byte[0];
			if (loginType == 0)
			{
				bytes = new byte[] {0, 0, 0, 8, 2, 0, 0, 0, 4};
			}
			else if (loginType == 1)
			{
				bytes = new byte[] {0, 0, 0, 0xA, 2, 0, 0, 0, 4};
			}
			else if (loginType == 2)
			{
				bytes = new byte[] {0, 0, 0, 0xA, 1}; //包类型和加密类型
				if (API.UN_Tlv.T143_token_A2 != null)
				{
					bytes = bytes.Concat(BitConverter.GetBytes(API.UN_Tlv.T143_token_A2.Length + 4).Reverse().ToArray()).ToArray();
					bytes = bytes.Concat(API.UN_Tlv.T143_token_A2).ToArray();
				}
			}
			else if (loginType == 3)
			{
				bytes = new byte[] {0, 0, 0, 0xB, 1};
				bytes = bytes.Concat(BitConverter.GetBytes(API.QQ.mRequestID).Reverse().ToArray()).ToArray();
			}
			else if (loginType == 4)
			{
				bytes = new byte[] {0, 0, 0, 0xB, 2};
				bytes = bytes.Concat(BitConverter.GetBytes(API.QQ.mRequestID).Reverse().ToArray()).ToArray();
			}
			bytes = bytes.Concat(new byte[] {0, 0, 0}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(API.QQ.UTF8.Length + 4)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(API.QQ.UTF8).ToArray();
			bytes = bytes.Concat(bytesIn).ToArray();
			var retByte = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();
			return retByte;
		}
#endregion
#region 组包_上线
		public static byte[] PackOnlineStatus(string cmd, int LonginType)
		{
			if (API.QQ.mRequestID > 2147483647)
			{
				API.QQ.mRequestID = 10000;
			}
			else
			{
				API.QQ.mRequestID += 1;
			}
			var bytes1 = BitConverter.GetBytes(API.QQ.mRequestID).Reverse().ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.QQ.Appid).Reverse().ToArray()).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.QQ.Appid).Reverse().ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0}).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.UN_Tlv.T10A_token_A4.Length + 4).Reverse().ToArray()).ToArray().Concat(API.UN_Tlv.T10A_token_A4).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(cmd.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(cmd)).ToArray(); //命令行
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.QQ.MsgCookies.Length + 4).Reverse().ToArray()).ToArray().Concat(API.QQ.MsgCookies).ToArray(); //MsgCookies
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.Device.imei.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(API.Device.imei)).ToArray(); //Device.imei
			if (LonginType == 0)
			{
				bytes1 = bytes1.Concat(new byte[] {0, 0, 0, 4}).ToArray();
			}
			else if (LonginType == 1)
			{
				bytes1 = bytes1.Concat(API.GetRandByteArray(16)).ToArray();
			}
			else if (LonginType == 2)
			{
				bytes1 = bytes1.Concat(new byte[16]).ToArray();
			}
			bytes1 = bytes1.Concat(new byte[] {0, 0x23}).ToArray();
			bytes1 = bytes1.Concat(Encoding.UTF8.GetBytes(API.Device.Ver)).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0, 0, 0, 0x2A}).ToArray();
			bytes1 = bytes1.Concat(Encoding.UTF8.GetBytes("b$1ebc85de7365de4d155ce40110001581471d")).ToArray();

			var bytes2 = new byte[] {0x10, 3, 0x2C, 0x3C, 0x4C, 0x56, 0xB};
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes("PushService")).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0x66, 0xE}).ToArray();
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes("SvcReqRegister}")).ToArray();
			if (LonginType == 0)
			{
				bytes2 = bytes2.Concat(new byte[] {0, 1, 0, 0xE6, 8, 0}).ToArray();
			}
			else if (LonginType == 1)
			{
				bytes2 = bytes2.Concat(new byte[] {0, 1, 0, 0xEA, 8, 0}).ToArray();
			}
			else if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0, 1, 0, 0xBD, 8, 0}).ToArray();
			}
			bytes2 = bytes2.Concat(new byte[] {1, 6, 0xE}).ToArray();
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes("SvcReqRegister")).ToArray();
			if (LonginType == 0)
			{
				bytes2 = bytes2.Concat(new byte[] {0x1D, 0, 1, 0, 0xCE, 0xA, 3, 0, 0, 0, 0}).ToArray();
			}
			else if (LonginType == 1)
			{
				bytes2 = bytes2.Concat(new byte[] {0x1D, 0, 1, 0, 0xD2, 0xA, 3, 0, 0, 0, 0}).ToArray();
			}
			else if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0x1D, 0, 1, 0, 0xA5, 0xA, 3, 0, 0, 0, 0}).ToArray();
			}
			bytes2 = bytes2.Concat(API.QQ.user).ToArray();
			if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0x1C}).ToArray();
			}
			else
			{
				bytes2 = bytes2.Concat(new byte[] {0x10, 7}).ToArray();
			}
			bytes2 = bytes2.Concat(new byte[] {0x2C, 0x36, 0}).ToArray();
			if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0x40, 0x15}).ToArray();
			}
			else
			{
				bytes2 = bytes2.Concat(new byte[] {0x40, 0xB}).ToArray();
			}
			bytes2 = bytes2.Concat(new byte[] {0x5C, 0x6C, 0x7C, 0x8C, 0x9C, 0xA0, 0x75, 0xB0, 0x16, 0xC0, 1, 0xD6, 0, 0xEC, 0xFD, 0x10, 0, 0, 0x10}).ToArray();
			bytes2 = bytes2.Concat(API.Device.GUIDBytes).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF1, 0x11, 8, 4, 0xFC, 0x12}).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF6, 0x13}).ToArray();
			bytes2 = bytes2.Concat(BitConverter.GetBytes(Convert.ToInt16(API.Device.model.Length)).ToArray().Take(1).ToArray()).ToArray();
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes(API.Device.model)).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF6, 0x14}).ToArray();
			bytes2 = bytes2.Concat(BitConverter.GetBytes(Convert.ToInt16(API.Device.model.Length)).ToArray().Take(1).ToArray()).ToArray();
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes(API.Device.model)).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF6, 0x15}).ToArray();
			bytes2 = bytes2.Concat(BitConverter.GetBytes(Convert.ToInt16(API.Device.os_version.Length)).ToArray().Take(1).ToArray()).ToArray();
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes(API.Device.os_version)).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF0, 0x16, 1}).ToArray();
			if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0xF1, 0x17}).ToArray();
			}
			else
			{
				bytes2 = bytes2.Concat(new byte[] {0xF1, 0x17, 0, 0xD7}).ToArray();
			}
			bytes2 = bytes2.Concat(new byte[] {0xFC, 0x18}).ToArray();
			if (LonginType == 0)
			{
				bytes2 = bytes2.Concat(new byte[] {0xF3, 0x1A, 0, 0, 0, 0, 0xA6, 0x3C, 0x5E, 0x7D, 0xF2, 0x1B, 0x5F, 0xD, 0x60, 0x71}).ToArray();
			}
			else if (LonginType == 1)
			{
				bytes2 = bytes2.Concat(new byte[] {0xF3, 0x1A, 0, 0, 0, 0, 0xD9, 0xC, 0x60, 0x71, 0xF3, 0x1B, 0, 0, 0, 0, 0xA6, 0x3C, 0x5E, 0x7D}).ToArray();
			}
			else if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0xFC, 0x1A, 0xFC, 0x1B}).ToArray();
			}
			bytes2 = bytes2.Concat(new byte[] {0xF6, 0x1C, 0, 0xFC, 0x1D}).ToArray();
			if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0xF6, 0x1E, 0, 0xF6, 0x1F, 0}).ToArray();
			}
			else
			{
				bytes2 = bytes2.Concat(new byte[] {0xF6, 0x1E, 7, 0x5B, 0x75, 0x5D, 0x6F, 0x70, 0x70, 0x6F, 0xF6, 0x1F, 0x14, 0x3F, 0x4C, 0x59, 0x5A, 0x32, 0x38, 0x4E, 0x3B, 0x61, 0x6E, 0x64, 0x72, 0x6F, 0x69, 0x64, 0x5F, 0x78, 0x38, 0x36, 0x2D}).ToArray();
			}
			bytes2 = bytes2.Concat(new byte[] {0xF6, 0x20, 0x0, 0xFD, 0x21, 0x0, 0x0, 0x11, 0xA, 0x8, 0x8, 0x2E, 0x10, 0x9A, 0xEF, 0x9C, 0xFB, 0x5, 0xA, 0x5, 0x8, 0x9B, 0x2, 0x10, 0x0, 0xFC, 0x22, 0xFC, 0x24}).ToArray();
			if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0xF0, 0x26, 0xFF}).ToArray();
			}
			else
			{
				bytes2 = bytes2.Concat(new byte[] {0xFC, 0x26}).ToArray();
			}

			bytes2 = bytes2.Concat(new byte[] {0xFC, 0x27, 0xFA, 0x2A, 0x0, 0x1, 0xB, 0xB, 0x8C, 0x98, 0xC, 0xA8, 0xC}).ToArray();

			var bytes = BitConverter.GetBytes(bytes1.Length + 4).Reverse().ToArray();
			bytes = bytes.Concat(bytes1).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(bytes2.Length + 4).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(bytes2).ToArray();
			Debug.Print("在线状态包:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));

			HashTea Hash = new HashTea();
			byte[] retByte = Hash.HashTEA(bytes, API.UN_Tlv.T305_SessionKey, 0, true);
			return PackHeader(retByte, 2);

		}
#endregion
#region 组包_OidbSvc_0x59f上线 
		public static void PackOidbSvc_0x59f()
		{
			var cmd = "OidbSvc.0x59f";
			if (API.QQ.mRequestID > 2147483647)
			{
				API.QQ.mRequestID = 10000;
			}
			else
			{
				API.QQ.mRequestID += 1;
			}

			var bytes1 = BitConverter.GetBytes(API.QQ.mRequestID).Reverse().ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.QQ.Appid).Reverse().ToArray()).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.QQ.Appid).Reverse().ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0}).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.UN_Tlv.T10A_token_A4.Length + 4).Reverse().ToArray()).ToArray().Concat(API.UN_Tlv.T10A_token_A4).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(cmd.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(cmd)).ToArray(); //命令行
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.QQ.MsgCookies.Length + 4).Reverse().ToArray()).ToArray().Concat(API.QQ.MsgCookies).ToArray(); //MsgCookies
			bytes1 = bytes1.Concat(BitConverter.GetBytes(API.Device.imei.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(API.Device.imei)).ToArray(); //Device.imei
			bytes1 = bytes1.Concat(new byte[] {0, 0, 0, 4}).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(Convert.ToInt16(API.Device.Ver.Length + 2)).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(API.Device.Ver)).ToArray(); //Device.Ver
			bytes1 = bytes1.Concat(new byte[] {0, 0, 0, 4}).ToArray();
			bytes1 = BitConverter.GetBytes(bytes1.Length + 4).Reverse().ToArray().Concat(bytes1).ToArray();

			var bytes2 = new byte[] {0x8, 0x9F, 0xB, 0x10, 0x1, 0x18, 0x0, 0x22, 0x0, 0x32};
			bytes2 = bytes2.Concat(BitConverter.GetBytes((API.Device.os_type + " " + API.Device.os_version).Length).ToArray().Take(1).ToArray()).Concat(Encoding.UTF8.GetBytes(API.Device.os_type + " " + API.Device.os_version)).ToArray(); //os_type
			bytes2 = BitConverter.GetBytes(bytes2.Length + 4).Reverse().ToArray().Concat(bytes2).ToArray();

			HashTea Hash = new HashTea();
			byte[] retByte = Hash.HashTEA(bytes1.Concat(bytes2).ToArray(), API.UN_Tlv.T305_SessionKey, 0, true);

			var bytes3 = new byte[] {0, 0, 0, 0xA, 1};
			bytes3 = bytes3.Concat(BitConverter.GetBytes(API.UN_Tlv.T143_token_A2.Length + 4).Reverse().ToArray()).ToArray().Concat(API.UN_Tlv.T143_token_A2).ToArray();
			bytes3 = bytes3.Concat(new byte[] {0}).ToArray();
			bytes3 = bytes3.Concat(BitConverter.GetBytes(API.QQ.UTF8.Length + 4).Reverse().ToArray()).ToArray().Concat(API.QQ.UTF8).ToArray();

			var bytes = bytes3.Concat(retByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();

			Debug.Print("OidbSvc_0x59f:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));

			API.TClient.SendData(bytes);
		}
#endregion
#region 组包获取好友消息
		public static byte[] PackFriendMsg(byte[] bytesIn)
		{
			var timeStamp = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
			var Str = "model:" + API.Device.model + ";os:22;version:v2man:" + API.Device.brands + "sys:LYZ28N";
			var bytes1 = new byte[] {8, 0, 0x12}.Concat(BitConverter.GetBytes(Str.Length).ToArray().Take(1).ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(Str)).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0x18}).ToArray();
			bytes1 = bytes1.Concat(GetBytesFromLong(timeStamp + 0x18).Skip(1).ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0x20}).ToArray();
			bytes1 = bytes1.Concat(GetBytesFromLong(timeStamp + 0x20).Skip(1).ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0x28}).ToArray();
			bytes1 = bytes1.Concat(GetBytesFromLong(timeStamp + 0x28).Skip(1).ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0x30}).ToArray();
			bytes1 = bytes1.Concat(GetBytesFromLong(timeStamp + 0x30).Skip(1).ToArray()).ToArray();

			API.QQ.SyncCoookies = bytes1;

			var bytes2 = Encoding.UTF8.GetBytes("MessageSvc.PbGetMsg");
			bytes2 = bytes2.Concat(new byte[] {0, 0, 0, 8}).ToArray();
			bytes2 = bytes2.Concat(bytesIn).ToArray();
			bytes2 = bytes2.Concat(BitConverter.GetBytes(bytes1.Length + 4).Reverse().ToArray()).ToArray().Concat(bytes1).ToArray();
			bytes2 = new byte[] {0, 0, 0, 0x17}.ToArray().Concat(bytes2).ToArray();
			var bytes = BitConverter.GetBytes(bytes2.Length + 4).Reverse().ToArray().Concat(bytes2).ToArray();

			var bytes3 = new byte[] {8}.Concat(GetBytesFromLong(timeStamp).Skip(2).ToArray()).ToArray().Concat(new byte[] {0x10}).ToArray().Concat(GetBytesFromLong(timeStamp + 0x10).Skip(2).ToArray()).ToArray();
			bytes3 = bytes3.Concat(API.HexStrToByteArray("18DE8DA980032082D7E1BA0C28E482B9DC0A48A8D28AC6015896A182DE076051")).ToArray();
			bytes3 = bytes3.Concat(new byte[] {0x68}).ToArray().Concat(GetBytesFromLong(timeStamp).Skip(2).ToArray()).ToArray();
			bytes3 = bytes3.Concat(new byte[] { 0x70, 0 }).ToArray();
			bytes3 = BitConverter.GetBytes(Convert.ToInt16(bytes3.Length)).ToArray().Take(1).ToArray().Concat(bytes3).ToArray();

			var bytes4 = new byte[] {8, 0, 0x12}.Concat(bytes3).ToArray().Concat(new byte[] {0x18, 0, 0x20, 0x14, 0x28, 3, 0x30, 1, 0x38, 1, 0x48, 0, 0x62, 0}).ToArray();
			var bytes4Len = BitConverter.GetBytes(bytes4.Length + 4).Reverse().ToArray();

			bytes = bytes.Concat(bytes4Len).ToArray().Concat(bytes4).ToArray();

			Debug.Print("通知好友消息:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			HashTea Hash = new HashTea();
			byte[] encodeByte = Hash.HashTEA(bytes, API.UN_Tlv.T305_SessionKey, 0, true);
			API.QQ.mRequestID = API.QQ.mRequestID + 1;
			return PackHeader(encodeByte, 3);
		}
#endregion
#region 组包获取好友历史消息
		public static void PackFriendHistoryMsg()
		{
			var timeStamp = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
			var Str = "model:SM-G9009D;os:21;version:v2man:samsungsys:LRX21T.G9009DKES1BPG2";
			var cmd = "MessageSvc.PbGetMsg";
			API.QQ.mRequestID = API.QQ.mRequestID + 1;

			byte[] bytesSyncCoookie1 = null;
			SyncCoookie1Struct SyncCoookie1 = new SyncCoookie1Struct
			{
				DeviceInfo = Str,
				ErrorCode = 0,
				TimeStamp = timeStamp,
				TimeStamp1 = timeStamp - 300,
				TimeStamp2 = timeStamp - 300,
				TimeStamp3 = timeStamp - 300
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, SyncCoookie1);
				bytesSyncCoookie1 = ms.ToArray();
			}
			byte[] bytesSyncCoookie2 = null;
			SyncCoookie2Struct SyncCoookie2 = new SyncCoookie2Struct
			{
				ErrorCode = 0,
				Field3 = 0,
				Field4 = 20,
				Field5 = 3,
				Field6 = 1,
				Field7 = 1,
				Field9 = 0,
				TimeInfo = API.QQ.SyncCoookies
			};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, SyncCoookie2);
				bytesSyncCoookie2 = ms.ToArray();
			}

			var bytes = BitConverter.GetBytes(cmd.Length + 4).Reverse().ToArray().Concat(Encoding.UTF8.GetBytes(cmd).ToArray()).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(API.QQ.MsgCookies.Length + 4).Reverse().ToArray()).Concat(API.QQ.MsgCookies).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(bytesSyncCoookie1.Length + 4).Reverse().ToArray()).Concat(bytesSyncCoookie1).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();

			bytes = bytes.Concat(BitConverter.GetBytes(bytesSyncCoookie2.Length + 4).Reverse().ToArray()).Concat(bytesSyncCoookie2).ToArray();

			Debug.Print("获取好友历史消息:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));

			HashTea Hash = new HashTea();
			byte[] encodeByte = Hash.HashTEA(bytes, API.UN_Tlv.T305_SessionKey, 0, true);

			bytes = new byte[] {0x0, 0x0, 0x0, 0xB, 0x1}.Concat(BitConverter.GetBytes(API.QQ.mRequestID).Reverse().ToArray()).ToArray().Concat(new byte[] {0x0, 0x0, 0x0}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(API.QQ.UTF8.Length)).ToArray()).ToArray().Concat(API.QQ.UTF8).ToArray();
			bytes = bytes.Concat(encodeByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();
			Debug.Print("获取好友历史消息全部:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));

			API.TClient.SendData(bytes);
		}
#endregion

#region 滑块验证
		public static byte[] VieryTicket(string Ticket)
		{
			byte[] bytes = new byte[] {0, 2, 0, 4};
			bytes = bytes.Concat(TLV.tlv193(Ticket)).ToArray();
			bytes = bytes.Concat(TLV.tlv008()).ToArray();
			bytes = bytes.Concat(TLV.tlv104(API.UN_Tlv.T104)).ToArray();
			bytes = bytes.Concat(TLV.tlv116(10)).ToArray();
			HashTea Hash = new HashTea();
			bytes = Hash.HashTEA(bytes, API.QQ.shareKey, 0, true);
			bytes = PackLoginHeader("wtlogin.login", bytes, 0);
			return bytes;
		}
#endregion
#region 四字验证
		public static byte[] VieryCode(string code)
		{
			byte[] bytes = new byte[] {0, 2, 0, 4};
			bytes = bytes.Concat(TLV.tlv193(code)).ToArray();
			bytes = bytes.Concat(TLV.tlv008()).ToArray();
			bytes = bytes.Concat(TLV.tlv104(API.UN_Tlv.T104)).ToArray();
			bytes = bytes.Concat(TLV.tlv116(10)).ToArray();
			HashTea Hash = new HashTea();
			bytes = Hash.HashTEA(bytes, API.QQ.shareKey, 0, true);
			bytes = PackLoginHeader("wtlogin.login", bytes, 1);
			return bytes;
		}
#endregion
#region 发送请求手机验证 tlv106
		public static byte[] VieryPhoneCode()
		{
			byte[] bytes = new byte[] {0, 8, 0, 6};
			bytes = bytes.Concat(TLV.tlv008()).ToArray();
			bytes = bytes.Concat(TLV.tlv104(API.UN_Tlv.T104)).ToArray();
			bytes = bytes.Concat(TLV.tlv116(8)).ToArray();
			bytes = bytes.Concat(TLV.tlv174(API.UN_Tlv.T174)).ToArray();
			bytes = bytes.Concat(TLV.tlv17a()).ToArray();
			bytes = bytes.Concat(TLV.tlv197()).ToArray();
			//bytes = bytes.Concat(TLV.tlv542).ToArray
			Debug.Print("VieryPhoneCode1:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			HashTea Hash = new HashTea();
			bytes = Hash.HashTEA(bytes, API.QQ.shareKey, 0, true);
			bytes = PackLoginHeader("wtlogin.login", bytes, 1);
			return bytes;

		}
#endregion
#region 验证设备锁 tlv204
		public static byte[] VieryLock()
		{
			byte[] bytes = new byte[] {0, 0x14, 0, 4};
			bytes = bytes.Concat(TLV.tlv008()).ToArray();
			bytes = bytes.Concat(TLV.tlv104(API.UN_Tlv.T104)).ToArray();
			bytes = bytes.Concat(TLV.tlv116(10)).ToArray();
			bytes = bytes.Concat(TLV.tlv401()).ToArray();
			Debug.Print("VieryLock:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			HashTea Hash = new HashTea();
			bytes = Hash.HashTEA(bytes, API.QQ.shareKey, 0, true);
			bytes = PackLoginHeader("wtlogin.login", bytes, 0);
			return bytes;
		}
#endregion
#region 提交手机验证码
		public static byte[] SubmitVertificationCode(string code)
		{
			byte[] bytes = new byte[] {0, 7, 0, 7};
			bytes = bytes.Concat(TLV.tlv008()).ToArray();
			bytes = bytes.Concat(TLV.tlv104(API.UN_Tlv.T104)).ToArray();
			bytes = bytes.Concat(TLV.tlv116(8)).ToArray();
			bytes = bytes.Concat(TLV.tlv174(API.UN_Tlv.T174)).ToArray();
			bytes = bytes.Concat(TLV.tlv17c(code)).ToArray();
			bytes = bytes.Concat(TLV.tlv401()).ToArray();
			bytes = bytes.Concat(TLV.tlv198()).ToArray();
			HashTea Hash = new HashTea();
			bytes = Hash.HashTEA(bytes, API.QQ.shareKey, 0, true);
			bytes = PackLoginHeader("wtlogin.login", bytes, 1);
			return bytes;
		}
#endregion

#region 回执_QualityTest
		public static void ReplyQualityTest(int ssoseq)
		{
			var bytes = API.PackCmdHeader("QualityTest.PushList", new byte[0]);
			HashTea Hash = new HashTea();
			byte[] encodeByte = Hash.HashTEA(bytes, API.UN_Tlv.T305_SessionKey, 0, true);
			bytes = new byte[] {0, 0, 0, 0xB, 1}.Concat(BitConverter.GetBytes(ssoseq).Reverse().ToArray()).Concat(new byte[] {0, 0, 0}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(API.QQ.UTF8.Length + 4)).Reverse().ToArray()).ToArray().Concat(API.QQ.UTF8).ToArray();
			bytes = bytes.Concat(encodeByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();
			API.TClient.SendData(bytes);
		}
#endregion
#region 回执_SSOHelloPush
		public static void ReplySSOHelloPush(byte[] BytesIn, int ssoseq)
		{
			var bytes = API.PackCmdHeader("SSO.HelloPush", BytesIn);
			HashTea Hash = new HashTea();
			byte[] encodeByte = Hash.HashTEA(bytes, API.UN_Tlv.T305_SessionKey, 0, true);
			bytes = new byte[] {0, 0, 0, 0xB, 1}.Concat(BitConverter.GetBytes(ssoseq).Reverse().ToArray()).Concat(new byte[] {0, 0, 0}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(API.QQ.UTF8.Length + 4)).Reverse().ToArray()).ToArray().Concat(API.QQ.UTF8).ToArray();
			bytes = bytes.Concat(encodeByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();
			API.TClient.SendData(bytes);
		}
#endregion
#region 回执_SidTicketExpired
		public static void ReplySidTicketExpired(int ssoseq)
		{
			var bytes = API.PackCmdHeader("OnlinePush.SidTicketExpired", new byte[0]);
			HashTea Hash = new HashTea();
			byte[] encodeByte = Hash.HashTEA(bytes, API.UN_Tlv.T305_SessionKey, 0, true);
			bytes = new byte[] {0, 0, 0, 0xB, 1}.Concat(BitConverter.GetBytes(ssoseq).Reverse().ToArray()).Concat(new byte[] {0, 0, 0}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(API.QQ.UTF8.Length + 4)).Reverse().ToArray()).ToArray().Concat(API.QQ.UTF8).ToArray();
			bytes = bytes.Concat(encodeByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();
			API.TClient.SendData(bytes);
		}
#endregion
#region 心跳包
		public static byte[] HeartbeatPack()
		{
			var bytes = API.PackCmdHeader("StatSvc.SimpleGet", null);
			Debug.Print("心跳包:" + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
			bytes = API.PackAllHeader(bytes);
			return bytes;
		}
#endregion

		public static int getSubCmd()
		{
			if (pc_sub_cmd > 2147483647)
			{
				pc_sub_cmd = 0;
			}
			pc_sub_cmd = pc_sub_cmd + 1;
			return pc_sub_cmd;
		}
		public static byte[] GetBytesFromLong(long value)
		{
			byte[] retBytes = null;
			Long2Bytes Proto = new Long2Bytes {value = value};
			using (var ms = new MemoryStream())
			{
				Serializer.Serialize(ms, Proto);
				retBytes = ms.ToArray();
			}
			return retBytes;
		}
		public static long GetLongFromBytes(byte[] bytes)
		{
			long retLong = 0;
			try
			{
				using (var ms = new MemoryStream(bytes))
				{
					var result = Serializer.Deserialize<Bytes2Long>(ms);
					retLong = result.Value;
				}
			}
			catch (Exception ex)
			{
				return 0;
			}

			return retLong;
		}
	}


	[ProtoContract]
	public class Long2Bytes
	{
		[ProtoMember(1)]
		public long value {get; set;}
	}
	[ProtoContract]
	public class Bytes2Long
	{
		[ProtoMember(1)]
		public long Value {get; set;}
	}



}