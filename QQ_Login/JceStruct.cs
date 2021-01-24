
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

namespace QQ_Login
{
	public class JceStruct
	{

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
				Debug.Print("[STRUCT_BEGIN]");
			}
			else if (tpe == JceType.TYPE_STRUCT_END)
			{
				bytes = writeHead((byte)JceType.TYPE_STRUCT_END, 0);
				Debug.Print("[STRUCT_END]");
			}
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
			Debug.Print("[" + tag.ToString() + "](SHORT)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
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
			Debug.Print("[" + tag.ToString() + "](BYTE)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
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
			Debug.Print("[" + tag.ToString() + "](INT)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
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
			Debug.Print("[" + tag.ToString() + "](LONG)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
			return bytes;
		}
		internal static byte[] writeFloat(float FloatValue, int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_FLOAT, tag);
			bytes = bytes.Concat(BitConverter.GetBytes(FloatValue).Reverse().ToArray()).ToArray();
			Debug.Print("[" + tag.ToString() + "](Float)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
			return bytes;
		}
		public static byte[] writeDouble(double DoubleValue, int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_DOUBLE, tag);
			bytes = bytes.Concat(BitConverter.GetBytes(DoubleValue).Reverse().ToArray()).ToArray();
			Debug.Print("[" + tag.ToString() + "](Double)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
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
			Debug.Print("[" + tag.ToString() + "](String)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
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
			Debug.Print("[" + tag.ToString() + "](Map)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
			return bytes;
		}
		public static byte[] writeSimpleList(byte[] ArrayObject, int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_SIMPLE_LIST, tag);
			bytes = bytes.Concat(writeHead((byte)JceType.TYPE_BYTE, 0)).ToArray();
			bytes = bytes.Concat(writeInt(ArrayObject.Length + 2, 0)).ToArray();
			bytes = bytes.Concat(writeHead((byte)JceType.TYPE_STRUCT_BEGIN, 0)).ToArray();
			bytes = bytes.Concat(ArrayObject).ToArray();
			bytes = bytes.Concat(writeHead((byte)JceType.TYPE_STRUCT_END, 0)).ToArray();
			Debug.Print("[" + tag.ToString() + "](String)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
			return bytes;
		}
		public static byte[] writeList<T>(ICollection<T> Collection, int tag)
		{
			byte[] bytes = writeHead((byte)JceType.TYPE_LIST, tag);
			bytes = bytes.Concat(writeInt(Collection.Count, 0)).ToArray();
			for (var i = 0; i < Collection.Count; i++)
			{
				bytes = bytes.Concat(writeObject(Collection.ElementAtOrDefault(i), 0)).ToArray();
			}
			Debug.Print("[" + tag.ToString() + "](LIST)Length=" + bytes.Length.ToString() + "\r\n" + BitConverter.ToString(bytes).Replace("-", ""));
			return bytes;
		}
		public static byte[] writeObject(object o, int tag)
		{
			byte[] bytes = null;
			if (o is byte)
			{
				bytes = writeByte(Convert.ToByte(o), tag);
			}
			else if (o is bool)
			{
				bytes = writeBoolean((bool)o, tag);
			}
			else if (o is short)
			{
				bytes = writeShort(Convert.ToInt16(o), tag);
			}
			else if (o is int)
			{
				bytes = writeInt(Convert.ToInt32(o), tag);
			}
			else if (o is long)
			{
				bytes = writeLong(Convert.ToInt64(o), tag);
			}
			else if (o is float)
			{
				bytes = writeFloat(Convert.ToSingle(o), tag);
			}
			else if (o is double)
			{
				bytes = writeDouble(Convert.ToDouble(o), tag);
			}
			else if (o is string)
			{
				bytes = writeString(Convert.ToString(o), tag);
			}
			else if (o is IDictionary)
			{
				bytes = writeMap((Dictionary<object, object>)o, tag);
			}
			else if (o is IList)
			{
				bytes = writeSimpleList((byte[])o, tag);
			}
			else if (o is JceType)
			{
				bytes = writeFlag((JceType)o, tag);
			}
			else if (o.GetType().IsArray)
			{
				bytes = writeMap((Dictionary<object, object>)o, tag);
			}
			else if (o is ICollection)
			{
				bytes = writeList((ICollection<object>)o, tag);
			}
			return bytes;
		}
#endregion
	}

}