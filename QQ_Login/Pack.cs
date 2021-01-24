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

namespace QQ_Login
{
	public class Pack
	{
		private static int pc_sub_cmd;

#region 组登陆包
		public static byte[] LoginPackage()
		{
			var bytes = TlvPackage("AndroidQQ");
			bytes = PackLoginHeader("wtlogin.login", bytes, 0);
			return bytes;
		}
		public static byte[] LoginPackage2()
		{
			var bytes = new byte[] {0x1F, 0x41};
			bytes = bytes.Concat(new byte[] {8, 0x10}).ToArray();
			bytes = bytes.Concat(new byte[] {0, 1}).ToArray();
			bytes = bytes.Concat(DataList.QQ.user).ToArray();
			bytes = bytes.Concat(new byte[] {3, 7, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0}).ToArray();
			bytes = bytes.Concat(new byte[] {1, 1}).ToArray();
			bytes = bytes.Concat(DataList.QQ.randKey).ToArray();
			bytes = bytes.Concat(new byte[] {1, 2}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(DataList.QQ.pub_key.Length)).Reverse().ToArray()).ToArray().Concat(DataList.QQ.pub_key).ToArray();
			bytes = bytes.Concat(TlvPackage()).ToArray();
			Debug.Print("bytes:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			bytes = new byte[] {2}.ToArray().Concat(BitConverter.GetBytes(Convert.ToInt16(bytes.Length + 4)).Reverse().ToArray()).ToArray().Concat(bytes).ToArray();
			bytes = bytes.Concat(new byte[] {3}).ToArray();
			Debug.Print("Pack_Login:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			bytes = PackLoginHeader("wtlogin.login", bytes, DataList.QQ.mRequestID);
			Debug.Print("Pack_LoginHead:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			HashTea Hash = new HashTea();
			byte[] EncodeByte = Hash.HashTEA(bytes, DataList.QQ.key, 0, true); //QQ.shareKey
			Debug.Print("tea:" + EncodeByte.Length.ToString() + "\r\n" + BitConverter.ToString(EncodeByte).Replace("-", " "));
			var retBytes = PackLoginHeader("wtlogin.login", EncodeByte, 0);
			Debug.Print("retBytes:" + retBytes.Length.ToString() + "\r\n" + BitConverter.ToString(retBytes).Replace("-", " "));
			bytes = new byte[] {0, 0, 0, 0xA};
			bytes = bytes.Concat(new byte[] {2}).ToArray();
			bytes = bytes.Concat(new byte[] {0x7B, 0x7D}).ToArray();
			bytes = bytes.Concat(new byte[] {0}).ToArray();
			bytes = bytes.Concat(DataList.QQ.UTF8).ToArray();
			bytes = bytes.Concat(retBytes).ToArray();
			return bytes;
		}
#endregion
#region 组Tlv包
		public static byte[] TlvPackage(string LoginType = "")
		{
			var bytes = new byte[0];
			if (LoginType == "AndroidQQ")
			{
				bytes = new byte[] {0, 9, 0, 0x18};
				bytes = bytes.Concat(TLV.tlv018(DataList.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(DataList.QQ.user, DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(DataList.QQ.user, DataList.QQ.md5_1, DataList.QQ.md5_2, DataList.QQ.TGTKey, DataList.Device.GUID, DataList.QQ.time, DataList.QQ.Appid, DataList.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116()).ToArray();
				bytes = bytes.Concat(TLV.tlv100(DataList.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (DataList.QQ.ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(DataList.QQ.ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv144(DataList.QQ.TGTKey, TLV.tlv109(DataList.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(DataList.Device.os_type, DataList.Device.os_version, DataList.Device.network_type.ToString(), DataList.Device.apn), TLV.tlv128(DataList.Device.model, DataList.Device.GUID, DataList.Device.brands), TLV.tlv16e(DataList.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv145(DataList.Device.GUID)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(DataList.Device.Apk_V, DataList.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(DataList.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(DataList.Device.network_type.ToString(), DataList.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
				bytes = bytes.Concat(TLV.tlv187(DataList.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(DataList.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv194(DataList.Device.Imsi)).ToArray();
				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(DataList.Device.BSSID, DataList.Device.SSID)).ToArray();
				bytes = bytes.Concat(TLV.tlv177(DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv516()).ToArray();
				bytes = bytes.Concat(TLV.tlv521()).ToArray();
				bytes = bytes.Concat(TLV.tlv525(1, DataList.QQ.time, DataList.QQ.Appid2)).ToArray();
				bytes = bytes.Concat(TLV.tlv544()).ToArray();
			}
			else if (LoginType == "HDQQ")
			{
				bytes = new byte[] {0, 9, 0, 0x18};
				bytes = bytes.Concat(TLV.tlv018(DataList.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(DataList.QQ.user, DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(DataList.QQ.user, DataList.QQ.md5_1, DataList.QQ.md5_2, DataList.QQ.TGTKey, DataList.Device.GUID, DataList.QQ.time, (int)DataList.Device.AppId, DataList.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116()).ToArray();
				bytes = bytes.Concat(TLV.tlv100(DataList.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (DataList.QQ.ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(DataList.QQ.ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv144(DataList.QQ.TGTKey, TLV.tlv109(DataList.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(DataList.Device.os_type, DataList.Device.os_version, DataList.Device.network_type.ToString(), DataList.Device.apn), TLV.tlv128(DataList.Device.model, DataList.Device.GUID, DataList.Device.brands), TLV.tlv16e(DataList.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv145(DataList.Device.GUID)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(DataList.Device.Apk_V, DataList.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(DataList.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(DataList.Device.network_type.ToString(), DataList.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
				bytes = bytes.Concat(TLV.tlv187(DataList.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(DataList.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv194(DataList.Device.Imsi)).ToArray();

				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(DataList.Device.BSSID, DataList.Device.SSID)).ToArray(); // BSSID MD5 WIFISSID/NAME
				bytes = bytes.Concat(TLV.tlv177(DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv516()).ToArray();
				bytes = bytes.Concat(TLV.tlv521()).ToArray();
				bytes = bytes.Concat(TLV.tlv525(1, DataList.QQ.time, DataList.QQ.Appid2)).ToArray();

			}
			else if (LoginType == "企业QQ")
			{
				bytes = new byte[] {0, 9, 0, 0x16};
				bytes = bytes.Concat(TLV.tlv018(DataList.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(DataList.QQ.user, DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(DataList.QQ.user, DataList.QQ.md5_1, DataList.QQ.md5_2, DataList.QQ.TGTKey, DataList.Device.GUID, DataList.QQ.time, (int)DataList.Device.AppId, DataList.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116()).ToArray();
				bytes = bytes.Concat(TLV.tlv100(DataList.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (DataList.QQ.ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(DataList.QQ.ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv144(DataList.QQ.TGTKey, TLV.tlv109(DataList.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(DataList.Device.os_type, DataList.Device.os_version, DataList.Device.network_type.ToString(), DataList.Device.apn), TLV.tlv128(DataList.Device.model, DataList.Device.GUID, DataList.Device.brands), TLV.tlv16e(DataList.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv145(DataList.Device.GUID)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(DataList.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(DataList.Device.network_type.ToString(), DataList.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv106(DataList.QQ.user, DataList.QQ.md5_1, DataList.QQ.md5_2, DataList.QQ.TGTKey, DataList.Device.GUID, DataList.QQ.time, (int)DataList.Device.AppId, DataList.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(DataList.Device.Apk_V, DataList.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv177(DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv187(DataList.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(DataList.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(DataList.Device.BSSID, DataList.Device.BSSID)).ToArray();
				bytes = bytes.Concat(TLV.tlv194(DataList.Device.Imsi)).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
			}
			else if (LoginType == "企点QQ")
			{
				bytes = new byte[] {0, 9, 0, 0x16};
				bytes = bytes.Concat(TLV.tlv018(DataList.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(DataList.QQ.user, DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(DataList.QQ.user, DataList.QQ.md5_1, DataList.QQ.md5_2, DataList.QQ.TGTKey, DataList.Device.GUID, DataList.QQ.time, DataList.QQ.Appid, DataList.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116()).ToArray();
				bytes = bytes.Concat(TLV.tlv100(DataList.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (DataList.QQ.ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(DataList.QQ.ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv144(DataList.QQ.TGTKey, TLV.tlv109(DataList.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(DataList.Device.os_type, DataList.Device.os_version, DataList.Device.network_type.ToString(), DataList.Device.apn), TLV.tlv128(DataList.Device.model, DataList.Device.GUID, DataList.Device.brands), TLV.tlv16e(DataList.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv145(DataList.Device.GUID)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(DataList.Device.Apk_V, DataList.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(DataList.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(DataList.Device.network_type.ToString(), DataList.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
				bytes = bytes.Concat(TLV.tlv187(DataList.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(DataList.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(DataList.Device.BSSID, DataList.Device.BSSID)).ToArray();
				bytes = bytes.Concat(TLV.tlv177(DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv516()).ToArray();
				bytes = bytes.Concat(TLV.tlv194(DataList.Device.Imsi)).ToArray();
			}
			else
			{
				bytes = new byte[] {0, 9, 0, 0x18};
				bytes = bytes.Concat(TLV.tlv018(DataList.QQ.user)).ToArray();
				bytes = bytes.Concat(TLV.tlv001(DataList.QQ.user, DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv106(DataList.QQ.user, DataList.QQ.md5_1, DataList.QQ.md5_2, DataList.QQ.TGTKey, DataList.Device.GUID, DataList.QQ.time, (int)DataList.Device.AppId, DataList.QQ.UTF8)).ToArray();
				bytes = bytes.Concat(TLV.tlv116()).ToArray();
				bytes = bytes.Concat(TLV.tlv100(DataList.QQ.Appid.ToString())).ToArray();
				bytes = bytes.Concat(TLV.tlv107()).ToArray();
				if (DataList.QQ.ksid.Length > 0)
				{
					bytes = bytes.Concat(TLV.tlv108(DataList.QQ.ksid)).ToArray();
				}
				bytes = bytes.Concat(TLV.tlv142()).ToArray();
				bytes = bytes.Concat(TLV.tlv144(DataList.QQ.TGTKey, TLV.tlv109(DataList.Device.AndroidID), TLV.tlv52D(), TLV.tlv124(DataList.Device.os_type, DataList.Device.os_version, DataList.Device.network_type.ToString(), DataList.Device.apn), TLV.tlv128(DataList.Device.model, DataList.Device.GUID, DataList.Device.brands), TLV.tlv16e(DataList.Device.model))).ToArray();
				bytes = bytes.Concat(TLV.tlv145(DataList.Device.GUID)).ToArray();
				bytes = bytes.Concat(TLV.tlv147(DataList.Device.Apk_V, DataList.Device.ApkSig)).ToArray();
				bytes = bytes.Concat(TLV.tlv154(DataList.QQ.mRequestID)).ToArray();
				bytes = bytes.Concat(TLV.tlv141(DataList.Device.network_type.ToString(), DataList.Device.apn)).ToArray();
				bytes = bytes.Concat(TLV.tlv008()).ToArray();
				bytes = bytes.Concat(TLV.tlv511()).ToArray();
				bytes = bytes.Concat(TLV.tlv187(DataList.Device.MacBytes)).ToArray();
				bytes = bytes.Concat(TLV.tlv188(DataList.Device.AndroidID)).ToArray();
				bytes = bytes.Concat(TLV.tlv194(DataList.Device.Imsi)).ToArray();
				bytes = bytes.Concat(TLV.tlv191()).ToArray();
				bytes = bytes.Concat(TLV.tlv202(DataList.Device.BSSID, DataList.Device.SSID)).ToArray(); // BSSID MD5 WIFISSID/NAME
				bytes = bytes.Concat(TLV.tlv177(DataList.QQ.time)).ToArray();
				bytes = bytes.Concat(TLV.tlv516()).ToArray();
				bytes = bytes.Concat(TLV.tlv521()).ToArray();
				bytes = bytes.Concat(TLV.tlv525(1, DataList.QQ.time, DataList.QQ.Appid2)).ToArray();
			}
			HashTea Hash = new HashTea();
			bytes = Hash.HashTEA(bytes, DataList.QQ.shareKey, 0, true);
			return bytes;
		}
#endregion
#region 组登陆包头
		public static byte[] PackLoginHeader(string servicecmd, byte[] bytesIn, int loginType) //0 = 普通登录 1 = 验证码登录
		{

			var HeaderBytes = BitConverter.GetBytes(DataList.QQ.mRequestID).Reverse().ToArray();
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(DataList.QQ.Appid).Reverse().ToArray()).ToArray();
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(DataList.QQ.Appid2).Reverse().ToArray()).ToArray();
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
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(DataList.Device.imei.Length + 4).Reverse().ToArray()).ToArray();
			HeaderBytes = HeaderBytes.Concat(Encoding.UTF8.GetBytes(DataList.Device.imei)).ToArray();
			HeaderBytes = HeaderBytes.Concat(new byte[] {0, 0, 0, 4}).ToArray();
			HeaderBytes = HeaderBytes.Concat(BitConverter.GetBytes(Convert.ToInt16(DataList.Device.Ver.Length + 2)).Reverse().ToArray()).ToArray();
			HeaderBytes = HeaderBytes.Concat(Encoding.UTF8.GetBytes(DataList.Device.Ver)).ToArray();
			HeaderBytes = HeaderBytes.Concat(new byte[] {0, 0, 0, 0x2A}).ToArray();
			HeaderBytes = HeaderBytes.Concat(Encoding.UTF8.GetBytes("b$1ebc85de7365de4d155ce40110001581471d")).ToArray();
			var HeaderLen = BitConverter.GetBytes(Convert.ToInt16(HeaderBytes.Length + 4)).Reverse().ToArray();
			HeaderBytes = new byte[] {0, 0}.Concat(HeaderLen).ToArray().Concat(HeaderBytes).ToArray();


			var bytes2 = new byte[] {0x1F, 0x41, 8, 0x10, 0, 1};
			bytes2 = bytes2.Concat(DataList.QQ.user).ToArray();
			if (loginType == 0)
			{
				bytes2 = bytes2.Concat(new byte[] {0x3, 0x87, 0x0, 0x0, 0x0, 0x0, 0x2, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x2, 0x1}).ToArray();
			}
			else if (loginType == 1)
			{
				bytes2 = bytes2.Concat(new byte[] {0x3, 0x7, 0x0, 0x0, 0x0, 0x0, 0x2, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x2, 0x1}).ToArray();
			}
			bytes2 = bytes2.Concat(DataList.GetRandByteArray(16)).ToArray();
			bytes2 = bytes2.Concat(new byte[] {1, 0x31, 0, 1}).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0, 0x41}).ToArray();
			bytes2 = bytes2.Concat(DataList.QQ.pub_key).ToArray(); //65字节
			bytes2 = bytes2.Concat(bytesIn).ToArray(); //1392
			var BodyLen = BitConverter.GetBytes(Convert.ToInt16(bytes2.Length + 4)).Reverse().ToArray();
			bytes2 = new byte[] {2}.Concat(BodyLen).ToArray().Concat(bytes2).ToArray().Concat(new byte[] {3}).ToArray();

			var bytes = HeaderBytes.Concat(BitConverter.GetBytes(bytes2.Length + 4).Reverse().ToArray()).ToArray().Concat(bytes2).ToArray();

			Debug.Print("PackLoginHeader:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			HashTea Hash = new HashTea();
			byte[] EncodeByte = Hash.HashTEA(bytes, DataList.QQ.key, 0, true);
			Debug.Print("EncodeByte:" + EncodeByte.Length.ToString() + "\r\n" + BitConverter.ToString(EncodeByte).Replace("-", " "));
			var retByte = PackHeader(EncodeByte, 1);
			return retByte;
		}


		public static byte[] Pack_Pc(string cmd, byte[] bytesIn)
		{
			var ext_bin_null = 0;
			var bytes = DataList.HexStrToByteArray(DataList.Device.pc_ver);
			bytes = bytes.Concat(DataList.HexStrToByteArray(cmd)).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(getSubCmd().ToString())).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(DataList.QQ.user).ToArray();
			bytes = bytes.Concat(new byte[] {3, 7, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0}).ToArray();
			if (DataList.QQ.pub_key.Length > 0)
			{
				ext_bin_null = 0;
				bytes = bytes.Concat(new byte[] {1, 1}).ToArray();
			}
			else
			{
				ext_bin_null = 1;
				bytes = bytes.Concat(new byte[] {1, 2}).ToArray();
			}
			bytes = bytes.Concat(DataList.QQ.randKey).ToArray();
			bytes = bytes.Concat(new byte[] {1, 2}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(DataList.QQ.pub_key.Length)).Reverse().ToArray()).ToArray();
			if (ext_bin_null == 1)
			{
				bytes = bytes.Concat(new byte[] {0, 0}).ToArray();
			}
			else
			{
				bytes = bytes.Concat(DataList.QQ.pub_key).ToArray();
			}
			bytes = bytes.Concat(bytesIn).ToArray();
			bytes = bytes.Concat(new byte[] {3}).ToArray();
			var retByte = new byte[] {2}.Concat(BitConverter.GetBytes(Convert.ToInt16(bytes.Length + 3)).Reverse().ToArray()).Concat(bytes).ToArray();
			return retByte;
		}
#endregion
#region 组包_在线状态
		public static byte[] PackOnlineStatus(string cmd, int LonginType)
		{
			if (DataList.QQ.mRequestID > 2147483647)
			{
				DataList.QQ.mRequestID = 10000;
			}
			else
			{
				DataList.QQ.mRequestID += 1;
			}
			var bytes1 = BitConverter.GetBytes(DataList.QQ.mRequestID).Reverse().ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.QQ.Appid).Reverse().ToArray()).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.QQ.Appid2).Reverse().ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0}).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.QQ.Token010A.Length + 4).Reverse().ToArray()).ToArray().Concat(DataList.QQ.Token010A).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(cmd.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(cmd)).ToArray(); //命令行
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.QQ.MsgCookies.Length + 4).Reverse().ToArray()).ToArray().Concat(DataList.QQ.MsgCookies).ToArray(); //MsgCookies
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.Device.imei.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(DataList.Device.imei)).ToArray(); //Device.imei
			if (LonginType == 0)
			{
				bytes1 = bytes1.Concat(new byte[] {0, 0, 0, 4}).ToArray();
			}
			else if (LonginType == 1)
			{
				bytes1 = bytes1.Concat(DataList.GetRandByteArray(16)).ToArray();
			}
			else if (LonginType == 2)
			{
				bytes1 = bytes1.Concat(DataList.GetRandByteArray(16)).ToArray();
			}
			bytes1 = bytes1.Concat(new byte[] {0, 0x23}).ToArray();
			bytes1 = bytes1.Concat(Encoding.UTF8.GetBytes(DataList.Device.Ver)).ToArray();
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
				bytes2 = bytes2.Concat(new byte[] {0, 1, 0, 0xED, 8, 0}).ToArray();
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
			bytes2 = bytes2.Concat(DataList.QQ.user).ToArray();
			if (LonginType == 2)
			{
				bytes2 = bytes2.Concat(new byte[] {0x10}).ToArray();
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
			bytes2 = bytes2.Concat(DataList.HexStrToByteArray(DataList.Device.GUID)).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF1, 0x11, 8, 4, 0xFC, 0x12}).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF6, 0x13}).ToArray();
			bytes2 = bytes2.Concat(BitConverter.GetBytes(Convert.ToInt16(DataList.Device.model.Length)).ToArray().Take(1).ToArray()).ToArray();
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes(DataList.Device.model)).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF6, 0x14}).ToArray();
			bytes2 = bytes2.Concat(BitConverter.GetBytes(Convert.ToInt16(DataList.Device.model.Length)).ToArray().Take(1).ToArray()).ToArray();
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes(DataList.Device.model)).ToArray();
			bytes2 = bytes2.Concat(new byte[] {0xF6, 0x15}).ToArray();
			bytes2 = bytes2.Concat(BitConverter.GetBytes(Convert.ToInt16(DataList.Device.os_version.Length)).ToArray().Take(1).ToArray()).ToArray();
			bytes2 = bytes2.Concat(Encoding.UTF8.GetBytes(DataList.Device.os_version)).ToArray();
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
			byte[] retByte = Hash.HashTEA(bytes, DataList.QQ.sessionKey, 0, true);
			return PackHeader(retByte, 2);

		}
#endregion
#region 组包_OidbSvc_0x59f 
		public static void PackOidbSvc_0x59f()
		{
			var cmd = "OidbSvc.0x59f";
			if (DataList.QQ.mRequestID > 2147483647)
			{
				DataList.QQ.mRequestID = 10000;
			}
			else
			{
				DataList.QQ.mRequestID += 1;
			}

			var bytes1 = BitConverter.GetBytes(DataList.QQ.mRequestID).Reverse().ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.QQ.Appid).Reverse().ToArray()).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.QQ.Appid2).Reverse().ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0}).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.QQ.Token010A.Length + 4).Reverse().ToArray()).ToArray().Concat(DataList.QQ.Token010A).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(cmd.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(cmd)).ToArray(); //命令行
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.QQ.MsgCookies.Length + 4).Reverse().ToArray()).ToArray().Concat(DataList.QQ.MsgCookies).ToArray(); //MsgCookies
			bytes1 = bytes1.Concat(BitConverter.GetBytes(DataList.Device.imei.Length + 4).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(DataList.Device.imei)).ToArray(); //Device.imei
			bytes1 = bytes1.Concat(new byte[] {0, 0, 0, 4}).ToArray();
			bytes1 = bytes1.Concat(BitConverter.GetBytes(Convert.ToInt16(DataList.Device.Ver.Length + 2)).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(DataList.Device.Ver)).ToArray(); //Device.Ver
			bytes1 = bytes1.Concat(new byte[] {0, 0, 0, 4}).ToArray();
			bytes1 = BitConverter.GetBytes(bytes1.Length + 4).Reverse().ToArray().Concat(bytes1).ToArray();

			var bytes2 = new byte[] {0x8, 0x9F, 0xB, 0x10, 0x1, 0x18, 0x0, 0x22, 0x0, 0x32};
			bytes2 = bytes2.Concat(BitConverter.GetBytes((DataList.Device.os_type + " " + DataList.Device.os_version).Length).ToArray().Take(1).ToArray()).Concat(Encoding.UTF8.GetBytes(DataList.Device.os_type + " " + DataList.Device.os_version)).ToArray(); //os_type
			bytes2 = BitConverter.GetBytes(bytes2.Length + 4).Reverse().ToArray().Concat(bytes2).ToArray();

			HashTea Hash = new HashTea();
			byte[] retByte = Hash.HashTEA(bytes1.Concat(bytes2).ToArray(), DataList.QQ.sessionKey, 0, true);

			var bytes3 = new byte[] {0, 0, 0, 0xA, 1};
			bytes3 = bytes3.Concat(BitConverter.GetBytes(DataList.QQ.Token0143.Length + 4).Reverse().ToArray()).ToArray().Concat(DataList.QQ.Token0143).ToArray();
			bytes3 = bytes3.Concat(new byte[] {0}).ToArray();
			bytes3 = bytes3.Concat(BitConverter.GetBytes(DataList.QQ.UTF8.Length + 4).Reverse().ToArray()).ToArray().Concat(DataList.QQ.UTF8).ToArray();

			var bytes = bytes3.Concat(retByte).ToArray();
			bytes = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();

			Debug.Print("OidbSvc_0x59f:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));

			TCPIPClient.SendData(bytes);

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
				bytes = new byte[] {0, 0, 0, 0xA, 1};
				bytes = bytes.Concat(BitConverter.GetBytes(DataList.QQ.Token0143.Length + 4).Reverse().ToArray()).ToArray();
				bytes = bytes.Concat(DataList.QQ.Token0143).ToArray();
			}
			else if (loginType == 3)
			{
				bytes = new byte[] {0, 0, 0, 0xB, 1};
				bytes = bytes.Concat(BitConverter.GetBytes(DataList.QQ.mRequestID).Reverse().ToArray()).ToArray();
			}
			else if (loginType == 4)
			{
				bytes = new byte[] {0, 0, 0, 0xB, 2};
				bytes = bytes.Concat(BitConverter.GetBytes(DataList.QQ.mRequestID).Reverse().ToArray()).ToArray();
			}
			bytes = bytes.Concat(new byte[] {0, 0, 0}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(DataList.QQ.UTF8.Length + 4)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(DataList.QQ.UTF8).ToArray();
			bytes = bytes.Concat(bytesIn).ToArray();
			var retByte = BitConverter.GetBytes(bytes.Length + 4).Reverse().ToArray().Concat(bytes).ToArray();
			return retByte;
		}
#endregion
#region 组包获取好友消息
		public static byte[] PackFriendMsg(byte[] bytesIn, byte[] TimeProtobuf)
		{
			var timeStamp = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
			var Str = "model:" + DataList.Device.model + ";os:22;version:v2man:" + DataList.Device.brands + "sys:LYZ28N";
			var bytes1 = new byte[] {8, 0, 0x12}.Concat(BitConverter.GetBytes(Str.Length).ToArray().Take(1).ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(Str)).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0x18}).ToArray();
			bytes1 = bytes1.Concat(GetBytesFromLong(timeStamp + 0x18).Skip(1).ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0x20}).ToArray();
			bytes1 = bytes1.Concat(GetBytesFromLong(timeStamp + 0x20).Skip(1).ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0x28}).ToArray();
			bytes1 = bytes1.Concat(GetBytesFromLong(timeStamp + 0x28).Skip(1).ToArray()).ToArray();
			bytes1 = bytes1.Concat(new byte[] {0x30}).ToArray();
			bytes1 = bytes1.Concat(GetBytesFromLong(timeStamp + 0x30).Skip(1).ToArray()).ToArray();

			DataList.QQ.SyncCoookies = bytes1;

			var bytes2 = Encoding.UTF8.GetBytes("MessageSvc.PbGetMsg");
			bytes2 = bytes2.Concat(new byte[] {0, 0, 0, 8}).ToArray();
			bytes2 = bytes2.Concat(bytesIn).ToArray();
			bytes2 = bytes2.Concat(BitConverter.GetBytes(bytes1.Length + 4).Reverse().ToArray()).ToArray().Concat(bytes1).ToArray();
			bytes2 = new byte[] {0, 0, 0, 0x17}.ToArray().Concat(bytes2).ToArray();
			var bytes = BitConverter.GetBytes(bytes2.Length + 4).Reverse().ToArray().Concat(bytes2).ToArray();

			var bytes3 = new byte[] {8}.Concat(GetBytesFromLong(timeStamp).Skip(2).ToArray()).ToArray().Concat(new byte[] {0x10}).ToArray().Concat(GetBytesFromLong(timeStamp + 0x10).Skip(2).ToArray()).ToArray();
			bytes3 = bytes3.Concat(DataList.HexStrToByteArray("18DE8DA980032082D7E1BA0C28E482B9DC0A48A8D28AC6015896A182DE076051")).ToArray();
			bytes3 = bytes3.Concat(new byte[] {0x68}).ToArray().Concat(GetBytesFromLong(timeStamp).Skip(2).ToArray()).ToArray();
			bytes3 = bytes3.Concat(new byte[] {0x70, 0}).ToArray();
			bytes3 = BitConverter.GetBytes(Convert.ToInt16(bytes3.Length)).ToArray().Take(1).ToArray().Concat(bytes3).ToArray();

			var bytes4 = new byte[] {8, 0, 0x12}.Concat(bytes3).ToArray().Concat(new byte[] {0x18, 0, 0x20, 0x14, 0x28, 3, 0x30, 1, 0x38, 1, 0x48, 0, 0x62, 0}).ToArray();
			var bytes4Len = BitConverter.GetBytes(bytes4.Length + 4).Reverse().ToArray();

			bytes = bytes.Concat(bytes4Len).ToArray().Concat(bytes4).ToArray();
			HashTea Hash = new HashTea();
			byte[] encodeByte = Hash.HashTEA(bytes, DataList.QQ.sessionKey, 0, true);
			DataList.QQ.mRequestID = DataList.QQ.mRequestID + 1;
			return PackHeader(encodeByte, 3);
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
		public static byte[] Pack_VieryImage(string code)
		{
			var bytes = new byte[] {0, 2, 0, 4};
			bytes = bytes.Concat(TLV.tlv002(code, DataList.QQ.VieryToken1)).ToArray();
			bytes = bytes.Concat(TLV.tlv008()).ToArray();
			bytes = bytes.Concat(TLV.tlv104(DataList.QQ.VieryToken2)).ToArray();
			bytes = bytes.Concat(TLV.tlv116()).ToArray();
			Debug.Print("图片组包:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			HashTea Hash = new HashTea();
			bytes = Pack_Pc("0810", Hash.HashTEA(bytes, DataList.QQ.shareKey, 0, true));
			Debug.Print("要发的图形验证包:" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", " "));
			byte[] retByte = PackLoginHeader("wtlogin.login", bytes, 0);
			return retByte;
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