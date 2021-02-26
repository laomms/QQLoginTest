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

using System.IO;
using System.Text;
using ProtoBuf;

namespace QQ_Login
{
	public class ProtoBuff
	{
		[Obsolete]
		public void DecordProto(byte[] bytesIn)
		{
			ProtoReader reader = null;
			using (var ms = new MemoryStream(bytesIn))
			{
				reader = ProtoReader.Create(ms, null, null);
			}
			long start = reader.Position;
			int field = reader.ReadFieldHeader();
			while (field > 0)
			{
				long payloadStart = reader.Position;
				switch (reader.WireType)
				{
					case WireType.Varint:
					case WireType.Fixed32:
					case WireType.Fixed64:
						var val = reader.ReadInt64();
						break;
					case WireType.String:
						var payloadBytes = ProtoReader.AppendBytes(null, reader);
						try
						{
							var utf8 = Encoding.UTF8.GetString(payloadBytes);
						}
						catch
						{

						}
						using (var subReader = ReadProto(payloadBytes))
						{
							if (subReader != null)
							{

							}
						}
						break;
					case WireType.StartGroup:
						var tok = ProtoReader.StartSubItem(reader);
						ProtoReader.EndSubItem(tok, reader);
						break;
					default:
						throw new InvalidOperationException("oops we missed this!");
				}
				start = reader.Position;
				field = reader.ReadFieldHeader();
			}
		}

		[Obsolete]
		public static ProtoReader ReadProto(byte[] payload)
		{
			if (payload == null || payload.Length == 0)
			{
				return null;
			}
			try
			{
				var ms = new MemoryStream(payload);
				using (var reader = ProtoReader.Create(ms, null, null))
				{
					int field = reader.ReadFieldHeader();
					while (field > 0)
					{
						reader.SkipField();
						field = reader.ReadFieldHeader();
					}
				}
				ms.Position = 0;
				return ProtoReader.Create(ms, null, null);
			}
			catch
			{
				return null;
			}
		}
	}

}