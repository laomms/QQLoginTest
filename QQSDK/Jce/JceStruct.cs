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

using System.Buffers.Binary;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace QQSDK
{
	public class JceStruct
	{
		public static List<Dictionary<string, string>> DicByte = new List<Dictionary<string, string>>();
		public static List<Dictionary<string, string>> DicShort = new List<Dictionary<string, string>>();
		public static List<Dictionary<string, string>> DicInt = new List<Dictionary<string, string>>();
		public static List<Dictionary<string, string>> DicLong = new List<Dictionary<string, string>>();
		public static List<Dictionary<string, string>> DicSingle = new List<Dictionary<string, string>>();
		public static List<Dictionary<string, string>> DicDouble = new List<Dictionary<string, string>>();
		public static List<Dictionary<string, string>> DicString = new List<Dictionary<string, string>>();
		public static List<Dictionary<string, string>> DicSimpleList = new List<Dictionary<string, string>>();
		public static Dictionary<string, string> DicLongString = new Dictionary<string, string>();
		public static List<Dictionary<string, Tuple<string, string>>> DicMAP = new List<Dictionary<string, Tuple<string, string>>>();
		public static List<Dictionary<string, string>> DicList = new List<Dictionary<string, string>>();

		public enum JceType
		{
			TYPE_BYTE = 0,
			TYPE_SHORT = 1,
			TYPE_INT = 2,
			TYPE_LONG = 3,
			TYPE_FLOAT = 4,
			TYPE_DOUBLE = 5,
			TYPE_STRING1 = 6,
			TYPE_STRING4 = 7,
			TYPE_MAP = 8,
			TYPE_LIST = 9,
			TYPE_STRUCT_BEGIN = 10,
			TYPE_STRUCT_END = 11,
			TYPE_ZERO_TAG = 12,
			TYPE_SIMPLE_LIST = 13
		}
		public struct HeadDataStruct
		{
			public int tag; //序号
			public byte typ; //类型
		}
		public static int readHead(byte[] bytesIn, ref HeadDataStruct HeadData)
		{
			byte b = bytesIn[0]; //获取一个byte
			HeadData.typ = (byte)(b & 0xF); //低4位为类型
			HeadData.tag = (b & 0xF0) >> 4; //高4位为tag,
			if (HeadData.tag != 0xF) //如果tag为0xF 则下一个字段为tag
			{
				return 1;
			}
			HeadData.tag = bytesIn[1] & 0xFF;
			return 2;
		}
		public static void StartDecode(byte[] bytesIn)
		{
			DicByte.Clear();
			DicShort.Clear();
			DicInt.Clear();
			DicLong.Clear();
			DicSingle.Clear();
			DicDouble.Clear();
			DicString.Clear();
			DicMAP.Clear();
			DicList.Clear();
			DicSimpleList.Clear();
			DecodeJce(ref bytesIn);
		}


#region Jce解码

		public static byte[] SkipLength(byte[] bytesIn, ref int Length, ref HeadDataStruct HeadData)
		{
			readHead(bytesIn, ref HeadData);
			bytesIn = bytesIn.Skip(1).ToArray();
			switch (HeadData.typ)
			{
				case (byte)JceType.TYPE_ZERO_TAG:
					Length = 0;
					return bytesIn;
				case (byte)JceType.TYPE_BYTE:
					Length = bytesIn[0];
					return bytesIn.Skip(1).ToArray();
				case (byte)JceType.TYPE_SHORT:
					Length = Convert.ToInt32(BitConverter.ToInt16(bytesIn.Take(2).ToArray().Reverse().ToArray(), 0).ToString());
					return bytesIn.Skip(2).ToArray();
				case (byte)JceType.TYPE_INT:
					Length = Convert.ToInt32(BitConverter.ToInt32(bytesIn.Take(4).ToArray().Reverse().ToArray(), 0).ToString());
					return bytesIn.Skip(4).ToArray();
			}
			Length = -1;
			return bytesIn;
		}
		public static void DecodeJce(ref byte[] bytesIn)
		{
			string Value = null;
			byte[] jceData = null;
			HeadDataStruct HeadData = new HeadDataStruct();
			try
			{
				while (bytesIn.Length > 0)
				{
					var len = readHead(bytesIn, ref HeadData);
					var Hex = bytesIn[0].ToString("x2").ToUpper();
					bytesIn = bytesIn.Skip(len).ToArray();
					var typ = HeadData.typ;
					var tag = HeadData.tag;
					switch (typ)
					{
						case (byte)JceType.TYPE_BYTE:
						{
							jceData = bytesIn.Take(1).ToArray();
							Value = jceData[0].ToString();
							bytesIn = bytesIn.Skip(1).ToArray();
							Dictionary<string, string> DicItem = new Dictionary<string, string>()
							{
								{tag.ToString(), Value}
							};
							DicByte.Add(DicItem);
							break;
						}
						case (byte)JceType.TYPE_SHORT:
						{
							jceData = bytesIn.Take(2).ToArray();
							Value = BitConverter.ToInt16(jceData.Reverse().ToArray(), 0).ToString();
							bytesIn = bytesIn.Skip(2).ToArray();
							Dictionary<string, string> DicItem = new Dictionary<string, string>()
							{
								{tag.ToString(), Value}
							};
							DicShort.Add(DicItem);
							break;
						}
						case (byte)JceType.TYPE_INT:
						{
							jceData = bytesIn.Take(4).ToArray();
							Value = BitConverter.ToInt32(jceData.Reverse().ToArray(), 0).ToString();
							bytesIn = bytesIn.Skip(4).ToArray();
							Dictionary<string, string> DicItem = new Dictionary<string, string>()
							{
								{tag.ToString(), Value}
							};
							DicInt.Add(DicItem);
							break;
						}
						case (byte)JceType.TYPE_LONG:
						{
							jceData = bytesIn.Take(8).ToArray();
							Value = BitConverter.ToInt64(jceData.Reverse().ToArray(), 0).ToString();
							bytesIn = bytesIn.Skip(8).ToArray();
							Dictionary<string, string> DicItem = new Dictionary<string, string>()
							{
								{tag.ToString(), Value}
							};
							DicLong.Add(DicItem);
							break;
						}
						case (byte)JceType.TYPE_FLOAT:
						{
							jceData = bytesIn.Take(4).ToArray();
							Value = BitConverter.ToSingle(jceData.Reverse().ToArray(), 0).ToString();
							bytesIn = bytesIn.Skip(4).ToArray();
							Dictionary<string, string> DicItem = new Dictionary<string, string>()
							{
								{tag.ToString(), Value}
							};
							DicSingle.Add(DicItem);
							break;
						}
						case (byte)JceType.TYPE_DOUBLE:
						{
							jceData = bytesIn.Take(8).ToArray();
							Value = BitConverter.ToDouble(jceData.Reverse().ToArray(), 0).ToString();
							bytesIn = bytesIn.Skip(8).ToArray();
							Dictionary<string, string> DicItem = new Dictionary<string, string>()
							{
								{tag.ToString(), Value}
							};
							DicDouble.Add(DicItem);
							break;
						}
						case (byte)JceType.TYPE_STRING1:
						{
							int jceDatalen = bytesIn.Take(1).ToArray()[0];
							bytesIn = bytesIn.Skip(1).ToArray();
							if (jceDatalen > 0)
							{
								if (bytesIn.Length < jceDatalen)
								{
									jceDatalen = bytesIn.Length;
								}
								jceData = bytesIn.Take(jceDatalen).ToArray();
								Value = Encoding.UTF8.GetString(jceData).Replace("\0", "").Replace("\n", "").Replace("\r\n", "").Replace("\r", "").Replace("\b", "").Replace("\f", "").Replace("\v", "");
								bytesIn = bytesIn.Skip(jceDatalen).ToArray();
								Dictionary<string, string> DicItem = new Dictionary<string, string>()
								{
									{tag.ToString(), Value}
								};
								DicString.Add(DicItem);
							}
							break;
						}
						case (byte)JceType.TYPE_STRING4:
						{
							var jceDatalen = BitConverter.ToInt32(bytesIn.Take(4).Reverse().ToArray(), 0);
							bytesIn = bytesIn.Skip(4).ToArray();
							if (jceDatalen > 0)
							{
								if (bytesIn.Length < jceDatalen)
								{
									jceDatalen = bytesIn.Length;
								}
								jceData = bytesIn.Take(jceDatalen).ToArray();
								Value = Encoding.UTF8.GetString(jceData).Replace("\0", "").Replace("\n", "").Replace("\r\n", "").Replace("\r", "").Replace("\b", "").Replace("\f", "").Replace("\v", "");
								bytesIn = bytesIn.Skip(jceDatalen).ToArray();
								Dictionary<string, string> DicItem = new Dictionary<string, string>()
								{
									{tag.ToString(), Value}
								};
								DicString.Add(DicItem);
							}
							break;
						}
						case (byte)JceType.TYPE_MAP:
						{
							var count = 0;
							HeadDataStruct HD = new HeadDataStruct();
							bytesIn = SkipLength(bytesIn, ref count, ref HD);
							if (count > 0)
							{
								DecodeMap(ref bytesIn, tag, count);
							}
							break;
						}
						case (byte)JceType.TYPE_LIST:
						{
							var count = 0;
							HeadDataStruct HD = new HeadDataStruct();
							bytesIn = SkipLength(bytesIn, ref count, ref HD);
							if (count > 0)
							{
								DecodeList(ref bytesIn, tag, count);
							}
							break;
						}
						case (byte)JceType.TYPE_STRUCT_BEGIN:
						{

						break;
						}
						case (byte)JceType.TYPE_STRUCT_END:
						{

						break;
						}
						case (byte)JceType.TYPE_ZERO_TAG:
						{

						break;
						}
						case (byte)JceType.TYPE_SIMPLE_LIST:
						{
							HeadDataStruct HD = new HeadDataStruct();
							readHead(bytesIn, ref HD);
							bytesIn = bytesIn.Skip(1).ToArray();
							var jceDatalen = 0;
							bytesIn = SkipLength(bytesIn, ref jceDatalen, ref HD);
							if (jceDatalen > 0)
							{
								jceData = bytesIn.Take(jceDatalen).ToArray();
								Value = BitConverter.ToString(jceData).Replace("-", "");
								Dictionary<string, string> DicItem = new Dictionary<string, string>()
								{
									{tag.ToString(), Value}
								};
								DicSimpleList.Add(DicItem);
								DecodeSimpleList(ref bytesIn, tag, jceDatalen);
							}
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message.ToString());
			}

		}
		public static void DecodeSubJce(ref byte[] bytesIn)
		{

			byte[] jceData = null;
			HeadDataStruct HeadData = new HeadDataStruct();
			try
			{
				while (bytesIn.Length > 0)
				{
					var len = readHead(bytesIn, ref HeadData);
					var Hex = bytesIn[0].ToString("x2").ToUpper();
					bytesIn = bytesIn.Skip(len).ToArray();
					var typ = HeadData.typ;
					var tag = HeadData.tag;
					switch (typ)
					{
						case (byte)JceType.TYPE_BYTE:
						{
							jceData = bytesIn.Take(1).ToArray();
							bytesIn = bytesIn.Skip(1).ToArray();
							break;
						}
						case (byte)JceType.TYPE_SHORT:
						{
							jceData = bytesIn.Take(2).ToArray();
							bytesIn = bytesIn.Skip(2).ToArray();
							break;
						}
						case (byte)JceType.TYPE_INT:
						{
							jceData = bytesIn.Take(4).ToArray();
							bytesIn = bytesIn.Skip(4).ToArray();
							break;
						}
						case (byte)JceType.TYPE_LONG:
						{
							jceData = bytesIn.Take(8).ToArray();
							bytesIn = bytesIn.Skip(8).ToArray();
							break;
						}
						case (byte)JceType.TYPE_FLOAT:
						{
							jceData = bytesIn.Take(4).ToArray();
							bytesIn = bytesIn.Skip(4).ToArray();
							break;
						}
						case (byte)JceType.TYPE_DOUBLE:
						{
							jceData = bytesIn.Take(8).ToArray();
							bytesIn = bytesIn.Skip(8).ToArray();
							break;
						}
						case (byte)JceType.TYPE_STRING1:
						{
							int jceDatalen = bytesIn.Take(1).ToArray()[0];
							bytesIn = bytesIn.Skip(1).ToArray();
							if (jceDatalen > 0)
							{
								if (bytesIn.Length < jceDatalen)
								{
									jceDatalen = bytesIn.Length;
								}
								jceData = bytesIn.Take(jceDatalen).ToArray();
								bytesIn = bytesIn.Skip(jceDatalen).ToArray();
							}
							break;
						}
						case (byte)JceType.TYPE_STRING4:
						{
							var jceDatalen = BitConverter.ToInt32(bytesIn.Take(4).Reverse().ToArray(), 0);
							bytesIn = bytesIn.Skip(4).ToArray();
							if (jceDatalen > 0)
							{
								if (bytesIn.Length < jceDatalen)
								{
									jceDatalen = bytesIn.Length;
								}
								jceData = bytesIn.Take(jceDatalen).ToArray();
								bytesIn = bytesIn.Skip(jceDatalen).ToArray();
							}
							break;
						}
						case (byte)JceType.TYPE_MAP:
						{
							var count = 0;
							HeadDataStruct HD = new HeadDataStruct();
							bytesIn = SkipLength(bytesIn, ref count, ref HD);
							if (count > 0)
							{
								DecodeMap(ref bytesIn, tag, count);
							}
							break;
						}
						case (byte)JceType.TYPE_LIST:
						{
							var count = 0;
							HeadDataStruct HD = new HeadDataStruct();
							bytesIn = SkipLength(bytesIn, ref count, ref HD);
							if (count > 0)
							{
								DecodeList(ref bytesIn, tag, count);
							}
							break;
						}
						case (byte)JceType.TYPE_STRUCT_BEGIN:
						{

						break;
						}
						case (byte)JceType.TYPE_STRUCT_END:
						{

						break;
						}
						case (byte)JceType.TYPE_ZERO_TAG:
						{

						break;
						}
						case (byte)JceType.TYPE_SIMPLE_LIST:
						{
							HeadDataStruct HD = new HeadDataStruct();
							readHead(bytesIn, ref HD);
							bytesIn = bytesIn.Skip(1).ToArray();
							var jceDatalen = 0;
							bytesIn = SkipLength(bytesIn, ref jceDatalen, ref HD);
							if (jceDatalen > 0)
							{
								jceData = bytesIn.Take(jceDatalen).ToArray();
								if (jceData[0] == (int)JceType.TYPE_STRUCT_BEGIN || jceData[0] == (int)JceType.TYPE_LIST || jceData[0] == (int)JceType.TYPE_SIMPLE_LIST || jceData[0] == (int)JceType.TYPE_MAP && jceData[1] == 0)
								{
									var tempBytes = bytesIn;
									DecodeSubJce(ref tempBytes);
								}
								bytesIn = bytesIn.Skip(jceDatalen).ToArray();
							}
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message.ToString());
			}

		}
		public static int DecodeMap(ref byte[] bytesIn, int tag, int Count)
		{
			var totalByte = bytesIn;
			for (var i = 0; i < Count; i++)
			{
				var tempBytes = bytesIn;
				var key = GetFieldValue(ref bytesIn);
				//DecodeSubJce(tempBytes.Take(tempBytes.Length - bytesIn.Length).ToArray)
				tempBytes = bytesIn;
				var value = GetFieldValue(ref bytesIn);
				//DecodeSubJce(tempBytes.Take(tempBytes.Length - bytesIn.Length).ToArray)
				Dictionary<string, Tuple<string, string>> dic = new Dictionary<string, Tuple<string, string>>();
				dic.Add(tag.ToString(), new Tuple<string, string>(key, value));
				DicMAP.Add(dic);
			}
			return totalByte.Length - bytesIn.Length;
		}
		public static string DecodeList(ref byte[] bytesIn, int tag, int Count)
		{
			var totalByte = bytesIn;
			for (var i = 0; i < Count; i++)
			{
				var tempBytes = bytesIn;
				var item = GetFieldValue(ref bytesIn);
				//DecodeSubJce(tempBytes.Take(tempBytes.Length - bytesIn.Length).ToArray)
				Dictionary<string, string> dic = new Dictionary<string, string>();
				dic.Add(tag.ToString(), item);
				DicList.Add(dic);
			}
			var len = totalByte.Length - bytesIn.Length;
			return BitConverter.ToString(totalByte.Take(len).ToArray()).Replace("-", "");
		}
		public static int DecodeSimpleList(ref byte[] bytesIn, int tag, int jceDatalen)
		{
			var jceData = bytesIn.Take(jceDatalen).ToArray();
			if ((JceStruct.JceType)jceData[0] == JceType.TYPE_STRUCT_BEGIN || (JceStruct.JceType)jceData[0] == JceType.TYPE_LIST || (JceStruct.JceType)jceData[0] == JceType.TYPE_SIMPLE_LIST || (JceStruct.JceType)jceData[0] == JceType.TYPE_MAP && Convert.ToInt32(jceData[1]) == 0)
			{
				var tempBytes = bytesIn;
				//DecodeSubJce(tempBytes.Take(jceDatalen).ToArray)
			}
			bytesIn = bytesIn.Skip(jceDatalen).ToArray();
			return jceDatalen;
		}
		private static string GetFieldValue(ref byte[] bytesIn)
		{
			var retStr = "";
			HeadDataStruct HeadData = new HeadDataStruct();
			var len = readHead(bytesIn, ref HeadData);
			bytesIn = bytesIn.Skip(len).ToArray();
			switch (HeadData.typ)
			{
				case (byte)JceType.TYPE_BYTE:
				{
					retStr = bytesIn[0].ToString();
					bytesIn = bytesIn.Skip(1).ToArray();
					break;
				}
				case (byte)JceType.TYPE_SHORT:
				{
					retStr = BitConverter.ToInt16(bytesIn.Take(2).ToArray().Reverse().ToArray(), 0).ToString();
					bytesIn = bytesIn.Skip(2).ToArray();
					break;
				}
				case (byte)JceType.TYPE_INT:
				{
					retStr = BitConverter.ToInt32(bytesIn.Take(4).ToArray().Reverse().ToArray(), 0).ToString();
					bytesIn = bytesIn.Skip(4).ToArray();
					break;
				}
				case (byte)JceType.TYPE_LONG:
				{
					retStr = BitConverter.ToInt64(bytesIn.Take(8).ToArray().Reverse().ToArray(), 0).ToString();
					bytesIn = bytesIn.Skip(8).ToArray();
					break;
				}
				case (byte)JceType.TYPE_FLOAT:
				{
					retStr = BitConverter.ToSingle(bytesIn.Take(4).ToArray().Reverse().ToArray(), 0).ToString();
					bytesIn = bytesIn.Skip(4).ToArray();
					break;
				}
				case (byte)JceType.TYPE_DOUBLE:
				{
					retStr = BitConverter.ToDouble(bytesIn.Take(8).ToArray().Reverse().ToArray(), 0).ToString();
					bytesIn = bytesIn.Skip(8).ToArray();
					break;
				}
				case (byte)JceType.TYPE_STRING1:
				{
					len = bytesIn.Take(1).ToArray()[0];
					bytesIn = bytesIn.Skip(1).ToArray();
					if (len > 0)
					{
						retStr = Encoding.UTF8.GetString(bytesIn.Take(len).ToArray()).Replace("\0", "").Replace("\n", "").Replace("\r\n", "").Replace("\r", "").Replace("\b", "").Replace("\f", "").Replace("\v", "");
						bytesIn = bytesIn.Skip(len).ToArray();
					}
					break;
				}
				case (byte)JceType.TYPE_STRING4:
				{
					len = bytesIn.Take(1).ToArray()[0];
					bytesIn = bytesIn.Skip(1).ToArray();
					if (len > 0)
					{
						retStr = Encoding.UTF8.GetString(bytesIn.Take(len).ToArray()).Replace("\0", "").Replace("\n", "").Replace("\r\n", "").Replace("\r", "").Replace("\b", "").Replace("\f", "").Replace("\v", "");
						bytesIn = bytesIn.Skip(len).ToArray();
					}
					break;
				}
				case (byte)JceType.TYPE_MAP:
				{
					var count = 0;
					HeadDataStruct HD = new HeadDataStruct();
					bytesIn = SkipLength(bytesIn, ref count, ref HD);
					retStr = DecodeMap(ref bytesIn, HeadData.tag, count).ToString();
					break;
				}
				case (byte)JceType.TYPE_LIST:
				{
					var count = 0;
					HeadDataStruct HD = new HeadDataStruct();
					bytesIn = SkipLength(bytesIn, ref count, ref HD);
					retStr = DecodeList(ref bytesIn, HeadData.tag, count);
					break;
				}
				case (byte)JceType.TYPE_SIMPLE_LIST:
				{
					HeadDataStruct HD = new HeadDataStruct();
					readHead(bytesIn, ref HD);
					bytesIn = bytesIn.Skip(1).ToArray();
					bytesIn = SkipLength(bytesIn, ref len, ref HD);
					if (len > 0)
					{
						retStr = BitConverter.ToString(bytesIn.Take(len).ToArray()).Replace("-", "");
						bytesIn = bytesIn.Skip(len).ToArray();
					}
					break;
				}
				case (byte)JceType.TYPE_STRUCT_BEGIN:
				{
					retStr = GetStuctValue(ref bytesIn);
					break;
				}
				case (byte)JceType.TYPE_STRUCT_END:
				{

				break;
				}
				case (byte)JceType.TYPE_ZERO_TAG:
				{
					retStr = "0";
					break;
				}
			}
			return retStr;
		}
		public static string GetStuctValue(ref byte[] bytesIn)
		{
			var listStr = new List<string>();
			try
			{
				byte tag = 0;
				var TotalBytes = bytesIn;
				HeadDataStruct HeadData = new HeadDataStruct();
				while (tag != (int)JceType.TYPE_STRUCT_END)
				{
					var len = readHead(bytesIn, ref HeadData);
					tag = HeadData.typ;
					if (tag == (int)JceType.TYPE_STRUCT_END)
					{
						bytesIn = bytesIn.Skip(1).ToArray();
						break;
					}
					listStr.Add(GetFieldValue(ref bytesIn));
				}
				return string.Join("/", listStr);
			}
			catch (Exception e1)
			{
			}
			return "";
		}
#endregion

#region Jce加密
		public static byte[] writeHead(byte typ, int tag)
		{
			if (tag < 15)
			{
				List<byte> list = new List<byte>();
				list.Add((byte)((tag << 4) | typ));
				return list.ToArray();
			}
			else if (tag < 256)
			{
				List<byte> list = new List<byte>();
				list.Add((byte)(typ | 0xF0));
				list.Add((byte)tag);
				return list.ToArray();
			}
			else
			{
				return null;
			}
			return null;
		}
		public static byte[] writeFlag(JceType tpe, int tag)
		{
			byte[] bytes = null;
			if (tpe == JceType.TYPE_STRUCT_BEGIN)
			{
				bytes = writeHead((byte)JceType.TYPE_STRUCT_BEGIN, tag);
			}
			else if (tpe == JceType.TYPE_STRUCT_END)
			{
				bytes = writeHead((byte)JceType.TYPE_STRUCT_END, 0);
			}
			return bytes;
		}
		public static byte[] writeZero(int tag)
		{
			var bytes = writeHead((byte)JceType.TYPE_ZERO_TAG, tag);
			return bytes;
		}
		public static byte[] writeBoolean(bool BoolValue, int tag)
		{
			var Bool = (BoolValue == true) ? 1 : 0;
			return writeByte((byte)Bool, tag);
		}
		public static byte[] writeShort(short ShortValue, int tag)
		{
			byte[] bytes = null;
			if (ShortValue >= byte.MinValue && ShortValue <= byte.MaxValue)
			{
				bytes = writeByte((byte)ShortValue, tag);
				return bytes;
			}
			else
			{
				bytes = writeHead((byte)JceType.TYPE_SHORT, tag);
				bytes = bytes.Concat(BitConverter.GetBytes(ShortValue).ToArray().Reverse().ToArray()).ToArray();
			}
			return bytes;
		}
		public static byte[] writeByte(byte ByteValue, int tag)
		{
			byte[] bytes = null;
			if (ByteValue == 0)
			{
				bytes = writeHead((byte)JceType.TYPE_ZERO_TAG, tag);
				return bytes;
			}
			bytes = writeHead((byte)JceType.TYPE_BYTE, tag);
			List<byte> list = new List<byte>();
			list.Add(ByteValue);
			bytes = bytes.Concat(list.ToArray()).ToArray();
			return bytes;
		}
		public static byte[] writeInt(int intValue, int tag)
		{
			byte[] bytes = null;
			if ((intValue >= short.MinValue) && (intValue <= short.MaxValue))
			{
				bytes = writeShort((short)intValue, tag);
				return bytes;
			}
			bytes = writeHead((byte)JceType.TYPE_INT, tag);
			bytes = bytes.Concat(BitConverter.GetBytes(intValue).ToArray().Reverse().ToArray()).ToArray();
			return bytes;
		}
		public static byte[] writeLong(long LongValue, int tag)
		{
			byte[] bytes = null;
			if (LongValue >= int.MinValue && LongValue <= int.MaxValue)
			{
				bytes = writeInt((int)LongValue, tag);
				return bytes;
			}
			else
			{
				bytes = writeHead((byte)JceType.TYPE_LONG, tag);
				bytes = bytes.Concat(BitConverter.GetBytes(LongValue).Reverse().ToArray()).ToArray();
			}
			return bytes;
		}
		internal static byte[] writeFloat(float FloatValue, int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_FLOAT, tag);
			bytes = bytes.Concat(BitConverter.GetBytes(FloatValue).Reverse().ToArray()).ToArray();
			return bytes;
		}
		public static byte[] writeDouble(double DoubleValue, int tag)
		{
			byte[] bytes = null;
			if ((float)DoubleValue >= float.MinValue && (float)DoubleValue <= float.MaxValue)
			{
				bytes = writeFloat((float)DoubleValue, tag);
				return bytes;
			}
			else
			{
				bytes = writeHead((byte)JceType.TYPE_DOUBLE, tag);
				bytes = bytes.Concat(BitConverter.GetBytes(DoubleValue).Reverse().ToArray()).ToArray();
			}
			return bytes;
		}
		public static byte[] writeString(string paramString, int tag)
		{
			byte[] bytes = null;
			if (paramString.Length > 255)
			{
				bytes = writeHead((byte)JceType.TYPE_STRING4, tag);
				bytes = bytes.Concat(BitConverter.GetBytes(paramString.Length).ToArray()).ToArray();
				bytes = bytes.Concat(Encoding.UTF8.GetBytes(paramString)).ToArray();
			}
			else
			{
				bytes = writeHead((byte)JceType.TYPE_STRING1, tag);
				bytes = bytes.Concat(BitConverter.GetBytes(paramString.Length).ToArray().Take(1).ToArray()).ToArray();
				bytes = bytes.Concat(Encoding.UTF8.GetBytes(paramString)).ToArray();
			}
			return bytes;
		}
		public static byte[] writeMap(Dictionary<object, object> MapValue, int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_MAP, tag);
			bytes = bytes.Concat(writeInt(MapValue.Count, 0)).ToArray();
			foreach (var pair in MapValue)
			{
				bytes = bytes.Concat(writeObject(pair.Key, 0)).ToArray();
				bytes = bytes.Concat(writeObject(pair.Value, 1)).ToArray();
			}
			return bytes;
		}
		public static byte[] writeEmptyMap(int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_MAP, tag);
			bytes = bytes.Concat(writeInt(0, 0)).ToArray();
			return bytes;
		}
		public static byte[] writeSimpleList(byte[] ArrayObject, int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_SIMPLE_LIST, tag);
			bytes = bytes.Concat(writeHead((byte)JceType.TYPE_BYTE, 0)).ToArray();
			bytes = bytes.Concat(writeInt(ArrayObject.Length, 0)).ToArray();
			//bytes = bytes.Concat(writeHead(JceType.TYPE_STRUCT_BEGIN, 0)).ToArray
			bytes = bytes.Concat(ArrayObject).ToArray();
			//bytes = bytes.Concat(writeHead(JceType.TYPE_STRUCT_END, 0)).ToArray
			return bytes;
		}
		public static byte[] writeList(byte[] byteIn, int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_LIST, tag);
			bytes = bytes.Concat(writeInt(1, 0)).ToArray();
			bytes = bytes.Concat(byteIn).ToArray();
			return bytes;
		}
		public static byte[] writeObject(object obj, int tag)
		{
			byte[] bytes = null;
			if (obj is byte)
			{
				bytes = writeByte(Convert.ToByte(obj), tag);
			}
			else if (obj is bool)
			{
				bytes = writeBoolean((bool)obj, tag);
			}
			else if (obj is short)
			{
				bytes = writeShort(Convert.ToInt16(obj), tag);
			}
			else if (obj is int)
			{
				bytes = writeInt(Convert.ToInt32(obj), tag);
			}
			else if (obj is long)
			{
				bytes = writeLong(Convert.ToInt64(obj), tag);
			}
			else if (obj is float)
			{
				bytes = writeFloat(Convert.ToSingle(obj), tag);
			}
			else if (obj is double)
			{
				bytes = writeDouble(Convert.ToDouble(obj), tag);
			}
			else if (obj is string)
			{
				bytes = writeString(Convert.ToString(obj), tag);
			}
			else if (obj is IDictionary)
			{
				Dictionary<object, object> mydic = ((IEnumerable)obj).Cast<object>().ToList().ToDictionary((px) => px.GetType().GetProperty("Key").GetValue(px), (pv) => pv.GetType().GetProperty("Value").GetValue(pv));
				bytes = writeMap(mydic, tag);
			}
			else if (obj is JceType)
			{
				bytes = writeFlag((JceType)obj, tag);
			}
			else if (obj is Array)
			{
				bytes = writeSimpleList((byte[])obj, tag);
			}
			else if (obj is IList)
			{
				bytes = writeSimpleList((byte[])obj, tag);
			}
			else if (obj is ICollection)
			{
				bytes = writeList((byte[])obj, tag);
			}
			return bytes;
		}
#endregion
	}
}