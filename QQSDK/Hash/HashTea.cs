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

namespace QQSDK
{
	public class HashTea
	{


#region 定义变量
		private bool IsHeaderPos;
		private byte[] Simple;
		private byte[] PreSimple;
		private byte[] OutPutBytes;
		private int Sticks;
		private long PreSticks;
		private long PosNow;
		private long PosStart;
		private long Stuff;
		private byte[] PubKey;
#endregion

		/// <summary>
		/// Tea开始循环加密
		/// </summary>
		public byte[] HashTEA(byte[] value, byte[] key, int offset, bool is16Rounds)
		{
			try
			{
				if (key == null || key.Length == 0)
				{
					key = new byte[16];
				}
				IsHeaderPos = true;
				Stuff = 0;
				Sticks = 0;
				PreSticks = 0;
				PubKey = key;
				int J_Length = value.Length;
				PosNow = (J_Length + 10) % 8;
				int i = 0;
				if (PosNow != 0)
				{
					PosNow = 8 - PosNow;
				}
				OutPutBytes = new byte[J_Length + PosNow + 10];
				Simple = new byte[8];
				PreSimple = new byte[8];
				Random rd = new Random();
				//Simple[0] = (byte)((rd.Next(1000, 5000) & 248) ^ PosNow);
				Simple[0] = Convert.ToByte((1000 & 248) ^ PosNow);
				for (int s = 0; s < PosNow; s++)
				{
					//{ Simple[s + 1] = (byte)(rd.Next(1000, 5000) & 255); }
					Simple[s + 1] = (byte)(1000 & 255);
				}
				PosNow += 1;
				Stuff = 1;
				while (Stuff < 3)
				{
					if (PosNow < 8)
					{
						//{ Simple[PosNow] = (byte)(rd.Next(1000, 5000) & 255); PosNow++; Stuff++; }
						Simple[PosNow] = (byte)(1000 & 255);
						PosNow += 1;
						Stuff += 1;
					}
					else
					{
						Encrypt8Bytes(is16Rounds);
					}
				}
				i = offset;
				while (J_Length > 0)
				{
					if (PosNow < 8)
					{
						Simple[PosNow] = value[i];
						PosNow += 1;
						J_Length -= 1;
						i += 1;
					}
					else
					{
						Encrypt8Bytes(is16Rounds);
					}
				}
				Stuff = 1;
				while (Stuff < 8)
				{
					if (PosNow < 8)
					{
						Simple[PosNow] = 0;
						Stuff += 1;
						PosNow += 1;
					}
					if (PosNow == 8)
					{
						Encrypt8Bytes(is16Rounds);
					}
				}
			}
			catch (Exception e1)
			{

			}
			return OutPutBytes;
		}

		/// <summary>
		/// Tea开始循环解密
		/// </summary>
		public byte[] UNHashTEA(byte[] value, byte[] key, int offset, bool is16Rounds)
		{
			try
			{

				if (key == null || key.Length == 0)
				{
					key = new byte[16];
				}

				Sticks = 0;
				PreSticks = 0;
				PubKey = key;
				byte[] NewBytes = new byte[offset + 8];
				int J_Length = value.Length;
				long J_Count = 0;
				int i2 = 0;
				Decipher(value, key, true, ref PreSimple);
				PosNow = PreSimple[0] & 7;
				J_Count = J_Length - PosNow - 10;
				OutPutBytes = new byte[J_Count];
				PreSticks = 0;
				Sticks = 8;
				PosStart = 8;
				PosNow += 1;
				Stuff = 1;

				while (Stuff < 3)
				{

					if (PosNow < 8)
					{
						PosNow += 1;
						Stuff += 1;
					}

					if (PosNow == 8)
					{
						NewBytes = value;
						Decrypt8Bytes(value, offset, J_Length);
					}
				}

				while (J_Count != 0)
				{

					if (PosNow < 8)
					{

						if (i2 < OutPutBytes.Length)
						{

							if (PosNow < PreSimple.Length)
							{

								if ((offset + PreSticks + PosNow) < NewBytes.Length)
								{
									OutPutBytes[i2] = Convert.ToByte((NewBytes[offset + PreSticks + PosNow] ^ PreSimple[PosNow]));
								}
								else
								{
									return new byte[0];
								}
							}
							else
							{
								return new byte[0];
							}
						}
						else
						{
							return new byte[0];
						}

						i2 += 1;
						PosNow += 1;
						J_Count -= 1;
					}

					if (PosNow == 8)
					{
						NewBytes = value;
						PreSticks = Sticks - 8;
						Decrypt8Bytes(value, offset, J_Length);
					}
				}

				for (int i = 0; i < 8; i++)
				{

					if (PosNow < 8)
					{
						PosNow += 1;

						if (PosNow == 8)
						{
							NewBytes = value;
							Decrypt8Bytes(value, offset, J_Length);
						}
					}
				}

			}
			catch (Exception __unusedException1__)
			{
				return new byte[0];
			}

			return OutPutBytes;
		}

		/// <summary>
		/// 循环解密8字节
		/// </summary>
		private bool Decrypt8Bytes(byte[] input, int offset, int J_Length)
		{
			try
			{
				for (int i = 0; i <= 7; i++)
				{
					if ((PosStart + i) > J_Length)
					{
						return true;
					}
					if ((offset + Sticks + i + 1) > input.Length)
					{
						return false;
					}
					PreSimple[i] = Convert.ToByte(PreSimple[i] ^ input[offset + Sticks + i]);
				}
				Decipher(PreSimple, PubKey, true, ref PreSimple);
				if (PreSimple.Length == 0)
				{
					return false;
				}
				PosStart += 8;
				Sticks += 8;
				PosNow = 0;
			}
			catch (Exception e1)
			{
			}
			return true;
		}

		/// <summary>
		/// 解密8字节
		/// </summary>
		private void Decipher(byte[] input, byte[] key, bool is16Rounds, ref byte[] outBytes)
		{
			long a = GetUInt(PubKey, 0, 4);
			long b = GetUInt(PubKey, 4, 4);
			long c = GetUInt(PubKey, 8, 4);
			long d = GetUInt(PubKey, 12, 4);
			long y = GetUInt(input, 0, 4);
			long z = GetUInt(input, 4, 4);
			try
			{
				if (PubKey == null)
				{
					PubKey = new byte[16];
				}
				long sum = 3816266640;
				long rounds = 0;
				long t = 0;
				long prev = 4294967295;
				long sev = 2654435769;
				if (is16Rounds)
				{
					rounds = 16;
				}
				else
				{
					rounds = 32;
				}
				for (int i = 0; i < rounds; i++)
				{
					t = (((y << 4) + c) ^ (y + sum)) ^ ((y >> 5) + d);
					z = z - t;
					z &= prev;
					t = (((z << 4) + a) ^ (z + sum)) ^ ((z >> 5) + b);
					y = y - t;
					y &= prev;
					sum = sum - sev;
					sum &= prev;
				}
			}
			catch (Exception e1)
			{
			}
			ToBytes(y, z, ref outBytes);
		}

		/// <summary>
		/// 循环加密8字节
		/// </summary>
		private void Encrypt8Bytes(bool is16Rounds)
		{
			try
			{
				PosNow = 1;
				byte[] Crypted = new byte[8];
				for (int i = 0; i <= 7; i++)
				{
					if (IsHeaderPos)
					{
						Simple[i] = Convert.ToByte(Simple[i] ^ PreSimple[1]);
					}
					else
					{
						if (PreSticks + i > OutPutBytes.Length)
						{
							return;
						}
						Simple[i] = Convert.ToByte(Simple[i] ^ OutPutBytes[PreSticks + i]);
					}
				}
				Encipher(Simple, PubKey, true, ref Crypted);
				for (int i = 0; i < Crypted.Length; i++)
				{
					if ((Sticks + i) > OutPutBytes.Length)
					{
						return;
					}
					OutPutBytes[Sticks + i] = Crypted[i];
				}
				for (int i = 0; i <= 7; i++)
				{
					if ((Sticks + i) > OutPutBytes.Length)
					{
						return;
					}
					OutPutBytes[Sticks + i] = Convert.ToByte(OutPutBytes[Sticks + i] ^ PreSimple[i]);
				}
				for (int i = 0; i < Simple.Length; i++)
				{
					if (i > PreSimple.Length)
					{
						return;
					}
					PreSimple[i] = Simple[i];
				}
				PreSticks = Sticks;
				Sticks += 8;
				PosNow = 0;
				IsHeaderPos = false;
			}
			catch (Exception e1)
			{
			}
		}

		/// <summary>
		/// 加密8字节
		/// </summary>
		private void Encipher(byte[] input, byte[] key, bool is16Rounds, ref byte[] outBytes)
		{
			long a = GetUInt(PubKey, 0, 4);
			long b = GetUInt(PubKey, 4, 4);
			long c = GetUInt(PubKey, 8, 4);
			long d = GetUInt(PubKey, 12, 4);
			long e = GetUInt(input, 0, 4);
			long f = GetUInt(input, 4, 4);
			try
			{
				long tmp = 0;
				long rounds = 0;
				long prev = 4294967295;
				long sev = 2654435769;
				if (is16Rounds)
				{
					rounds = 16;
				}
				else
				{
					rounds = 32;
				}
				for (int i = 0; i < rounds; i++)
				{
					tmp = (tmp & prev);
					tmp += sev;
					f = (f & prev);
					e = e + ((((f << 4) + a) ^ (f + tmp)) ^ ((f >> 5) + b));
					e = (e & prev);
					f = f + ((((e << 4) + c) ^ (e + tmp)) ^ ((e >> 5) + d));
				}

			}
			catch (Exception e1)
			{
			}
			ToBytes(e, f, ref outBytes);
		}

		/// <summary>
		/// 转到长整形
		/// </summary>
		private long GetUInt(byte[] input, int ioffset, int J_Length)
		{
			long res = 0;
			int length = 0;
			try
			{
				if (J_Length > 4)
				{
					length = ioffset + 4;
				}
				else
				{
					length = ioffset + J_Length;
				}
				while (ioffset < length)
				{
					res = res << 8;
					res |= input[ioffset];
					ioffset += 1;
				}
			}
			catch (Exception e1)
			{
				//MessageBox.Show(ep.Message);
			}
			return res;
		}

		/// <summary>
		/// 整数转换到字节集
		/// </summary>
		private void ToBytes(long a, long b, ref byte[] outBytes)
		{
			byte[] newBytes = new byte[8];
			try
			{
				newBytes[0] = (byte)((a >> 24) & 255);
				newBytes[1] = (byte)((a >> 16) & 255);
				newBytes[2] = (byte)((a >> 8) & 255);
				newBytes[3] = (byte)(a & 255);
				newBytes[4] = (byte)((b >> 24) & 255);
				newBytes[5] = (byte)((b >> 16) & 255);
				newBytes[6] = (byte)((b >> 8) & 255);
				newBytes[7] = (byte)(b & 255);
			}
			catch (Exception e1)
			{
			}
			outBytes = newBytes;
		}




	}

}