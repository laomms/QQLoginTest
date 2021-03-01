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
using System.Text;

namespace QQSDK
{
	public class TLV
	{
		//协议包结构：包头(四字节):协议号(两字节)+协议大小(两字节)  包体：协议字节

#region tlv001 QQ号及服务器时间
		public static byte[] tlv001(byte[] user, byte[] times)
		{
			var bytes = new byte[] {0, 1}.Concat(API.GetRandByteArray(4)).ToArray();
			bytes = bytes.Concat(user).ToArray();
			bytes = bytes.Concat(times).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0, 0, 0}).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0}).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {0, 1}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv001:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv002 验证码信息
		public static byte[] tlv002(string code, byte[] VieryToken1)
		{
			byte[] bytes = BitConverter.GetBytes(code.Length).Reverse().ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(code)).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(VieryToken1.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(VieryToken1).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {0, 2}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv002:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv008 手机系统语言中文804
		public static byte[] tlv008()
		{
			var bytes = new byte[] {0, 0, 0, 0, 8, 4, 0, 0};
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {0, 8}.Concat(bytesLen).Concat(bytes).ToArray();
			Debug.Print("tlv008:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv018 QQ号
		public static byte[] tlv018(byte[] user)
		{
			var bytes = new byte[] {0, 1, 0, 0, 6, 0, 0, 0, 0, 0x10, 0, 0, 0, 0}.Concat(user).Concat(new byte[] {0, 0, 0, 0}).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {0, 0x18}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv018:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv100 QQ版本
		public static byte[] tlv100(string appId)
		{
			var bytes = new byte[] {0, 1, 0, 0, 0, 0xD, 0, 0, 0, 0x10};
			bytes = bytes.Concat(BitConverter.GetBytes(int.Parse(appId)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0, 0, 0, 0x2, 0x14, 0x10, 0xE0}).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv100:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv104 验证码相关token
		public static byte[] tlv104(byte[] VieryToken2)
		{
			byte[] bytes = new byte[0];
			if (VieryToken2 != null)
			{
				bytes = VieryToken2;
			}
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 4}.Concat(bytesLen).Concat(bytes).ToArray();
			Debug.Print("tlv104:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv106 QQ主要信息
		public static byte[] tlv106(byte[] user, byte[] md5pass, byte[] md5_2pass, byte[] _TGTKey, byte[] guidBytes, byte[] times, int appId, byte[] QQUTF8)
		{
			var bytes = new byte[] {0, 4}.Concat(API.GetRandByteArray(4)).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0, 0, 0xD}).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0, 0, 0x10}).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0, 0, 0}).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0, 0, 0}).ToArray();
			bytes = bytes.Concat(user).ToArray();
			bytes = bytes.Concat(times).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0, 0, 0}).ToArray();
			bytes = bytes.Concat(new byte[] {1}).ToArray();
			bytes = bytes.Concat(md5pass).ToArray();
			bytes = bytes.Concat(_TGTKey).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0, 0, 0, 1}).ToArray();
			bytes = bytes.Concat(guidBytes).ToArray();
			if (appId < 0)
			{
				bytes = bytes.Concat(new byte[] {0, 0, 0, 2}).ToArray();
			}
			else
			{
				bytes = bytes.Concat(BitConverter.GetBytes(appId).Reverse().ToArray()).ToArray();
			}
			bytes = bytes.Concat(new byte[] {0, 0, 0, 1}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(QQUTF8.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(QQUTF8).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0}).ToArray();
			//Debug.Print("tlv106:" + bytes.Length.ToString + Environment.NewLine  + BitConverter.ToString(bytes).Replace("-", ""))
			HashTea Hash = new HashTea();
			var byteHash = Hash.HashTEA(bytes, md5_2pass, 0, true);
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(byteHash.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 6}.Concat(bytesLen).Concat(byteHash).ToArray();
			//Debug.Print("tlv106:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv107
		public static byte[] tlv107()
		{
			var bytes = new byte[] {0, 0, 0, 0, 0, 1};
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 7}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv107:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv108 ksid
		public static byte[] tlv108(byte[] ksid)
		{
			var retByte = new byte[] {1, 8, 0, 0};
			return retByte;
			//Dim bytes = ksid
			//Dim bytesLen() As Byte = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse.ToArray()
			//Dim retByte = New Byte() {1, 8}.Concat(bytesLen).Concat(bytes).ToArray()
			//'Debug.Print("tlv108:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			//Return retByte
		}
#endregion
#region tlv109 androidId
		public static byte[] tlv109(byte[] androidId)
		{
			var bytes = androidId;
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 9}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv109:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv116 验证码共用
		public static byte[] tlv116(int type)
		{
			if (type == 0)
			{
				type = 10;
			}
			var bytes = BitConverter.GetBytes(Convert.ToInt16(type)).Reverse().ToArray();
			bytes = bytes.Concat(new byte[] {0xF7, 0xFF, 0x7C, 0x0, 0x1, 0x4, 0x0, 0x1, 0x5F, 0x5E, 0x10, 0xE2}.ToArray()).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x16}.Concat(bytesLen).Concat(bytes).ToArray();
			Debug.Print("tlv116:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv124 手机系统信息
		public static byte[] tlv124(string os_type, string os_version, string _network_type, string _apn)
		{
			byte[] bytes = BitConverter.GetBytes(Convert.ToInt16(os_type.Length)).Reverse().ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(os_type)).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(os_version.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(os_version)).ToArray();
			bytes = bytes.Concat(new byte[] {0, 2}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(_network_type.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(_network_type)).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(_apn.Length).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(_apn)).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x24}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv124:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv128 手机硬件信息
		public static byte[] tlv128(string model, byte[] guidBytes, string brands)
		{
			var bytes = new byte[] {0, 0, 1, 1, 0, 0x11, 0, 0, 0};
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(model.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(model)).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(guidBytes.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(guidBytes).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(brands.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(brands)).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x28}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv128:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv141 手机营业商
		public static byte[] tlv141(string _network_type, string _apn)
		{
			var bytes = new byte[] {0, 1};
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(_network_type.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(_network_type)).ToArray();
			bytes = bytes.Concat(new byte[] {0, 2}).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(_apn.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(_apn)).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x41}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv141:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv142 com.tencent.mobileqq
		public static byte[] tlv142()
		{
			var bytes = new byte[] {0, 0, 0, 0x14};
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("com.tencent.mobileqq")).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x42}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv142:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv144 手机信息集合
		public static byte[] tlv144(byte[] TGTKey, byte[] tlv109, byte[] tlv52d, byte[] tlv124, byte[] tlv128, byte[] tlv16e)
		{
			var bytes = new byte[] {0, 5}.Concat(tlv109).ToArray();
			bytes = bytes.Concat(tlv52d).ToArray();
			bytes = bytes.Concat(tlv124).ToArray();
			bytes = bytes.Concat(tlv128).ToArray();
			bytes = bytes.Concat(tlv16e).ToArray();
			//Debug.Print("tlv144:" + bytes.Length.ToString + Environment.NewLine  + BitConverter.ToString(bytes).Replace("-", " "))
			HashTea Hash = new HashTea();
			var byteHash = Hash.HashTEA(bytes, TGTKey, 0, true);
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(byteHash.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x44}.Concat(bytesLen).Concat(byteHash).ToArray();
			//Debug.Print("tlv144:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv145 手机GUID
		public static byte[] tlv145(byte[] guidBytes)
		{
			var bytes = guidBytes;
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x45}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv145:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv147 安卓系统版本token
		public static byte[] tlv147(string _apk_v, byte[] _apk_sig)
		{
			var bytes = new byte[] {0, 0, 0, 0x10};
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(_apk_v.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(_apk_v)).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(_apk_sig.Length)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(_apk_sig).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x47}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv147:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv154 RequestId
		public static byte[] tlv154(int RequestIds)
		{
			var bytes = BitConverter.GetBytes(RequestIds).Reverse().ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x54}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv154:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv16b game.qq.com
		public static byte[] tlv16b()
		{
			var str = "game.qq.com";
			var bytes = new byte[] {0, 1};
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(str.Length)).Reverse().ToArray()).ToArray().Concat(Encoding.UTF8.GetBytes(str)).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x6B}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv16b:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv16e 手机型号
		public static byte[] tlv16e(string model)
		{
			var bytes = Encoding.UTF8.GetBytes(model);
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x6E}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv16e:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv174 手机验证码
		public static byte[] tlv174(byte[] token)
		{
			byte[] bytes = new byte[0];
			if (token != null)
			{
				bytes = token;
			}
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x74}.Concat(bytesLen).Concat(bytes).ToArray();
			Debug.Print("tlv174:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv177 手机BUILD_TIME/SDK_VERSION
		public static byte[] tlv177(byte[] times)
		{
			var bytes = new byte[] {1};
			bytes = bytes.Concat(times).ToArray();
			bytes = bytes.Concat(new byte[] {0, 0xA}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("6.0.0.2438")).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x77}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv177:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv17A 手机验证码
		public static byte[] tlv17a()
		{
			var retByte = new byte[] {1, 0x7A, 0, 4, 0, 0, 0, 9};
			Debug.Print("tlv17a:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv17c
		public static byte[] tlv17c(string Code)
		{
			var bytes = Encoding.UTF8.GetBytes(Code);
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x7C}.Concat(bytesLen).ToArray().Concat(bytes).ToArray();
			Debug.Print("tlv17c:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv198
		public static byte[] tlv198()
		{
			var retByte = new byte[] {1, 0x98, 0};
			Debug.Print("tlv198:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv187 手机网卡信息
		public static byte[] tlv187(byte[] Mac)
		{
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(Mac.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x87}.Concat(bytesLen).Concat(Mac).ToArray();
			//Debug.Print("tlv187:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv188 AndroidID 
		public static byte[] tlv188(byte[] AndroidID)
		{
			var bytes = AndroidID;
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x88}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv188:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv191 01表示不用web 验证码
		public static byte[] tlv191()
		{
			var bytes = new byte[] {0x82};
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x91}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv191:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv193 滑块验证
		public static byte[] tlv193(string ticket)
		{
			var bytes = Encoding.UTF8.GetBytes(ticket);
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x93}.Concat(bytesLen).Concat(bytes).ToArray();
			Debug.Print("tlv193:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv194 手机IMSI信息
		public static byte[] tlv194(string imsi)
		{
			var bytes = new byte[] {0xDE, 0x99, 0x6F, 0x72, 0x8, 0x45, 0x79, 0x4, 0xDE, 0xB5, 0xAF, 0x92, 0x27, 0x8E, 0x40, 0xA2};
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {1, 0x94}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv194:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv197 手机验证码
		public static byte[] tlv197()
		{
			var retByte = new byte[] {1, 0x97, 0, 1, 0};
			Debug.Print("tlv0197:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv202 手机SSID信息
		public static byte[] tlv202(byte[] BSSID, string SSID)
		{
			var bytes = new byte[] {0, 0x10};
			bytes = bytes.Concat(BSSID).ToArray();
			bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(SSID.Length + 2)).Reverse().ToArray()).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("\"" + SSID + "\"")).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {2, 2}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv202:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", ""))
			return retByte;
		}
#endregion
#region tlv401 
		public static byte[] tlv401()
		{
			var retByte = new byte[] {0x4, 0x1, 0x0, 0x10, 0x5, 0x59, 0x35, 0xEA, 0x34, 0xA2, 0x1B, 0xD0, 0xAE, 0x90, 0xDA, 0x97, 0x24, 0x22, 0x53, 0x63};
			Debug.Print("tlv401:" + retByte.Length.ToString() + "\r\n" + BitConverter.ToString(retByte).Replace("-", " "));
			return retByte;
		}
#endregion
#region tlv511 qq域名获取pskey,返回tlv512
		public static byte[] tlv511()
		{
			var strArr = new[] {"weiyun.com", "qzone.qq.com", "docs.qq.com", "lol.qq.com", "vip.qq.com", "b.qq.com", "qun.qq.com", "mail.qq.com", "qzone.com", "game.qq.com", "tenpay.com", "qzone.qq.com", "exmail.qq.com", "openmobile.qq.com"};
			var bytes = BitConverter.GetBytes(Convert.ToInt16(strArr.Count())).Reverse().ToArray();
			for (var i = 0; i < strArr.Length; i++)
			{
				bytes = bytes.Concat(new byte[] {1}).ToArray();
				bytes = bytes.Concat(BitConverter.GetBytes(Convert.ToInt16(strArr[i].Length)).Reverse().ToArray()).ToArray();
				bytes = bytes.Concat(Encoding.UTF8.GetBytes(strArr[i])).ToArray();
			}
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {5, 0x11}.Concat(bytesLen).ToArray().Concat(bytes).ToArray();
			//Debug.Print("tlv511:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv516
		public static byte[] tlv516()
		{
			var bytes = new byte[] {0, 0, 0, 0};
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {5, 0x16}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv516:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv521
		public static byte[] tlv521()
		{
			var bytes = new byte[] {0, 0, 0, 0, 0, 0};
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {5, 0x21}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv521:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv525 登录类型
		public static byte[] tlv525(int flag, byte[] time, int Appid) //1=普通登录 2=假锁登录
		{
			byte[] bytes = null;
			if (flag == 1)
			{
				bytes = new byte[] {0, 1, 5, 0x36, 0, 2, 1, 0};
			}
			else
			{
				bytes = new byte[] {0, 1, 5, 0x36, 0, 0x41, 1, 3, 0, 0, 0, 0};
				bytes = bytes.Concat(new byte[] {0x3A, 0x4, 0x3, 0x9E}).ToArray();
				bytes = bytes.Concat(new byte[] {4}).ToArray();
				bytes = bytes.Concat(new byte[] {0x65, 0x56, 0xAB, 0xB}).ToArray(); // 参数还未知,等等以后解
				bytes = bytes.Concat(time).ToArray();
				bytes = bytes.Concat(BitConverter.GetBytes(Appid).Reverse().ToArray()).ToArray();
				bytes = bytes.Concat(new byte[] {0, 0, 0, 0}).ToArray();
				bytes = bytes.Concat(API.GetRandByteArray(4)).ToArray();
				bytes = bytes.Concat(new byte[] {4}).ToArray();
				bytes = bytes.Concat(new byte[] {0x65, 0x56, 0xAB, 0xB}).ToArray(); // 参数还未知,等等以后解
				bytes = bytes.Concat(time).ToArray();
				bytes = bytes.Concat(BitConverter.GetBytes(Appid).Reverse().ToArray()).ToArray();
				bytes = bytes.Concat(new byte[] {0, 0, 0, 0}).ToArray();
				bytes = bytes.Concat(new byte[] {0x3A}).ToArray();
				bytes = bytes.Concat(API.GetRandByteArray(3)).ToArray();
				bytes = bytes.Concat(new byte[] {4}).ToArray();
				bytes = bytes.Concat(new byte[] {0x65, 0x56, 0xAB, 0xB}).ToArray();
				bytes = bytes.Concat(time).ToArray();
				bytes = bytes.Concat(BitConverter.GetBytes(Appid).Reverse().ToArray()).ToArray();
			}
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {5, 0x25}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv525:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv52D linux系统
		public static byte[] tlv52D()
		{
			var bytes = new byte[] {0xA, 7};
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("unknown")).ToArray();
			bytes = bytes.Concat(new byte[] {0x12, 0x92, 1}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("Linux version 4.0.9-android-x86 (denglibo@ubuntu) (gcc version 4.8.5 (Ubuntu 4.8.5-4ubuntu8~14.04.2) )")).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes(" #1 SMP PREEMPT Sat Jun 29 11:31:29 CST 2019")).ToArray();
			bytes = bytes.Concat(new byte[] {0x1A, 3}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("REL")).ToArray();
			bytes = bytes.Concat(new byte[] {0x22, 6}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("8.3.19")).ToArray();
			bytes = bytes.Concat(new byte[] {0x2A, 0x3A}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("asus/android_x86/x86:5.1.1/LYZ28N/8.3.19:user/release-keys2$724592da-b3b0-41f5-ad6a-d8d337ae2118")).ToArray();
			bytes = bytes.Concat(new byte[] {0x3A, 0x10}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("3148513af17e0571B")).ToArray();
			bytes = bytes.Concat(new byte[] {0xA}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("no message")).ToArray();
			bytes = bytes.Concat(new byte[] {0x4A, 0x6}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("8.3.19")).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {5, 0x2D}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv52D:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion
#region tlv542 获取手机验证码用
		public static byte[] tlv542()
		{
			return new byte[] {5, 0x42, 0, 0};
		}
#endregion
#region tlv544 com.tencent.mobileqq
		public static byte[] tlv544()
		{
			byte[] bytes = new byte[] {0, 0, 7, 0xD9, 0, 0, 0, 0, 0, 0x2E, 0, 0x20};
			bytes = bytes.Concat(API.GetRandByteArray(32)).ToArray();
			bytes = bytes.Concat(new byte[] {0x0, 8, 0, 0, 0, 0, 0, 0, 0x50, 0xC9, 0, 3, 1, 0, 0, 0}).ToArray();
			bytes = bytes.Concat(new byte[] {0x4, 0, 0, 0, 1, 0, 0, 0, 1}).ToArray();
			bytes = bytes.Concat(API.GetRandByteArray(15)).ToArray();
			bytes = bytes.Concat(new byte[] {0x0, 0x14}).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("com.tencent.mobileqq")).ToArray();
			bytes = bytes.Concat(Encoding.UTF8.GetBytes("A6B745BF24A2C277527716F6F36EB68D")).ToArray();
			bytes = bytes.Concat(new byte[] {6, 0xC, 0xAE, 0x6F, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x3, 0x0, 0x0, 0x1, 0x0, 0x10, 0xC, 0x0, 0x3F, 0x99, 0x46, 0xA5, 0xAA, 0x62, 0x43, 0xA3, 0xA8, 0xDA, 0x89, 0x53, 0x63, 0x81}).ToArray();
			byte[] bytesLen = BitConverter.GetBytes(Convert.ToInt16(bytes.Length)).Reverse().ToArray();
			var retByte = new byte[] {5, 0x44}.Concat(bytesLen).Concat(bytes).ToArray();
			//Debug.Print("tlv544:" + retByte.Length.ToString + Environment.NewLine  + BitConverter.ToString(retByte).Replace("-", " "))
			return retByte;
		}
#endregion

	}

}