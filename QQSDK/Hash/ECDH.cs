
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

using System.Security.Cryptography;
using System.Text;

namespace QQSDK
{
	public class ECDH
	{
		public static API.ECDH_Struct GetECDHKeys()
		{
			API.ECDH_Struct ECDH = new API.ECDH_Struct();
			byte[] PrivateKey = new byte[1024];
			byte[] PublicKey = new byte[1024];
			byte[] Sharekey = new byte[16];
			byte[] SvrPubKey = API.HexStrToByteArray("04EBCA94D733E399B2DB96EACDD3F69A8BB0F74224E2B44E3357812211D2E62EFBC91BB553098E25E33A799ADC7F76FEB208DA7C6522CDB0719A305180CC54A82E");
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
			ECDH.Sharekey = API.MD5Hash(Sharekey);
			return ECDH;
		}
		public static API.ECDH_Struct GetECDHKeys2()
		{
			API.ECDH_Struct ECDH = new API.ECDH_Struct();
			byte[] Sharekey = new byte[16];
			byte[] SvrPubKey = new byte[] {0x4, 0xBF, 0x47, 0xA1, 0xCF, 0x78, 0xA6, 0x29, 0x66, 0x8B, 0xB, 0xC3, 0x9F, 0x8E, 0x54, 0xC9, 0xCC, 0xF3, 0xB6, 0x38, 0x4B, 0x8, 0xB8, 0xAE, 0xEC, 0x87, 0xDA, 0x9F, 0x30, 0x48, 0x5E, 0xDF, 0xE7, 0x67, 0x96, 0x9D, 0xC1, 0xA3, 0xAF, 0x11, 0x15, 0xFE, 0xD, 0xCC, 0x8E, 0xB, 0x17, 0xCA, 0xCF};
			ECDH.PublicKey = new byte[25];
			ECDH.Sharekey = new byte[16];
			var eckey = OpenSSL.EC_KEY_new_by_curve_name(711);
			var ec_group = OpenSSL.EC_KEY_get0_group(eckey);
			var ec_point = OpenSSL.EC_POINT_new(ec_group);
			OpenSSL.EC_KEY_generate_key(eckey);
			OpenSSL.EC_POINT_point2oct(ec_group, (System.IntPtr)OpenSSL.EC_KEY_get0_public_key(eckey), 2, ECDH.PublicKey, 25, (System.IntPtr)0);
			OpenSSL.EC_POINT_oct2point(ec_group, ec_point, SvrPubKey, 49, (System.IntPtr)0);
			OpenSSL.ECDH_compute_key(ECDH.Sharekey, 16, ec_point, eckey, (System.IntPtr)0);
			ECDH.Sharekey = API.MD5Hash(Sharekey);
			return ECDH;
		}
		public static byte[] GetECDHKeysEx(byte[] peerRawPublicKey, byte[] PublicKey, byte[] PrivateKey)
		{
			API.ECDH_Struct ECDH = new API.ECDH_Struct();
			//Dim PrivateKey(1023) As Byte
			// Dim PublicKey(1023) As Byte
			//Dim Sharekey(15) As Byte

			var ec_key = OpenSSL.EC_KEY_new_by_curve_name(415);
			var bn = OpenSSL.BN_new();
			OpenSSL.BN_mpi2bn(PrivateKey, PrivateKey.Length, bn);
			OpenSSL.EC_KEY_set_private_key(ec_key, bn);
			OpenSSL.BN_free(bn);
			var ec_group = OpenSSL.EC_KEY_get0_group(ec_key);
			var ec_point = OpenSSL.EC_POINT_new(ec_group);
			OpenSSL.EC_POINT_oct2point(ec_group, ec_point, peerRawPublicKey, peerRawPublicKey.Length, (System.IntPtr)0);
			OpenSSL.ECDH_compute_key(PublicKey, 16, ec_point, ec_key, (System.IntPtr)0);
			return API.MD5Hash(PublicKey);

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
	}

}