using System;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x0200042F RID: 1071
public static class CustomSerializer
{
	// Token: 0x06001966 RID: 6502 RVA: 0x0008EB4C File Offset: 0x0008CD4C
	public static byte[] ByteSerialize(this object obj)
	{
		return CustomSerializer.Serialize(obj);
	}

	// Token: 0x06001967 RID: 6503 RVA: 0x0008EB54 File Offset: 0x0008CD54
	public static object ByteDeserialize(this byte[] bytes)
	{
		return CustomSerializer.Deserialize(bytes);
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x0008EB5C File Offset: 0x0008CD5C
	public static byte[] Serialize(object obj)
	{
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8))
			{
				CustomSerializer.SerializeObject(binaryWriter, obj);
				array = memoryStream.ToArray();
			}
		}
		return array;
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x0008EBBC File Offset: 0x0008CDBC
	public static object Deserialize(byte[] data)
	{
		object obj;
		using (MemoryStream memoryStream = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
			{
				obj = CustomSerializer.DeserializeObject(binaryReader);
			}
		}
		return obj;
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x0008EC18 File Offset: 0x0008CE18
	private static void SerializeObject(BinaryWriter writer, object obj)
	{
		string text = obj as string;
		if (text != null)
		{
			writer.Write(1);
			writer.Write(text);
			return;
		}
		if (obj is bool)
		{
			bool flag = (bool)obj;
			writer.Write(2);
			writer.Write(flag);
			return;
		}
		if (obj is int)
		{
			int num = (int)obj;
			writer.Write(3);
			writer.Write(num);
			return;
		}
		if (obj is float)
		{
			float num2 = (float)obj;
			writer.Write(4);
			writer.Write(num2);
			return;
		}
		if (obj is double)
		{
			double num3 = (double)obj;
			writer.Write(5);
			writer.Write(num3);
			return;
		}
		if (obj is Vector2)
		{
			Vector2 vector = (Vector2)obj;
			writer.Write(6);
			writer.Write(vector.x);
			writer.Write(vector.y);
			return;
		}
		if (obj is Vector3)
		{
			Vector3 vector2 = (Vector3)obj;
			writer.Write(7);
			writer.Write(vector2.x);
			writer.Write(vector2.y);
			writer.Write(vector2.z);
			return;
		}
		object[] array = obj as object[];
		if (array != null)
		{
			writer.Write(8);
			CustomSerializer.SerializeObjectArray(writer, array);
			return;
		}
		if (obj is byte)
		{
			byte b = (byte)obj;
			writer.Write(9);
			writer.Write(b);
			return;
		}
		Enum @enum = obj as Enum;
		if (@enum != null)
		{
			writer.Write(10);
			writer.Write(Convert.ToInt32(@enum));
			writer.Write(@enum.GetType().AssemblyQualifiedName);
			return;
		}
		NetEventOptions netEventOptions = obj as NetEventOptions;
		if (netEventOptions != null)
		{
			writer.Write(11);
			CustomSerializer.SerializeNetEventOptions(writer, netEventOptions);
			return;
		}
		if (obj is Quaternion)
		{
			Quaternion quaternion = (Quaternion)obj;
			writer.Write(12);
			writer.Write(quaternion.x);
			writer.Write(quaternion.y);
			writer.Write(quaternion.z);
			writer.Write(quaternion.w);
			return;
		}
		Debug.LogWarning("<color=blue>type not supported " + obj.GetType().ToString() + "</color>");
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x0008EE5C File Offset: 0x0008D05C
	private static void SerializeObjectArray(BinaryWriter writer, object[] objects)
	{
		writer.Write(objects.Length);
		foreach (object obj in objects)
		{
			CustomSerializer.SerializeObject(writer, obj);
		}
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x0008EE90 File Offset: 0x0008D090
	private static void SerializeNetEventOptions(BinaryWriter writer, NetEventOptions options)
	{
		writer.Write((int)options.Reciever);
		if (options.TargetActors == null)
		{
			writer.Write(0);
		}
		else
		{
			writer.Write(options.TargetActors.Length);
			foreach (int num in options.TargetActors)
			{
				writer.Write(num);
			}
		}
		writer.Write(options.Flags.WebhookFlags);
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x0008EEFC File Offset: 0x0008D0FC
	private static object DeserializeObject(BinaryReader reader)
	{
		switch (reader.ReadByte())
		{
		case 0:
			return null;
		case 1:
			return reader.ReadString();
		case 2:
			return reader.ReadBoolean();
		case 3:
			return reader.ReadInt32();
		case 4:
			return reader.ReadSingle();
		case 5:
			return reader.ReadDouble();
		case 6:
		{
			float num = reader.ReadSingle();
			float num2 = reader.ReadSingle();
			return new Vector2(num, num2);
		}
		case 7:
		{
			float num3 = reader.ReadSingle();
			float num4 = reader.ReadSingle();
			float num5 = reader.ReadSingle();
			return new Vector3(num3, num4, num5);
		}
		case 8:
			return CustomSerializer.DeserializeObjectArray(reader);
		case 9:
			return reader.ReadByte();
		case 10:
		{
			int num6 = reader.ReadInt32();
			return Enum.ToObject(Type.GetType(reader.ReadString()), num6);
		}
		case 11:
			return CustomSerializer.DeserializeNetEventOptions(reader);
		case 12:
		{
			float num7 = reader.ReadSingle();
			float num8 = reader.ReadSingle();
			float num9 = reader.ReadSingle();
			float num10 = reader.ReadSingle();
			return new Quaternion(num7, num8, num9, num10);
		}
		default:
			throw new InvalidOperationException("Unsupported type");
		}
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x0008F030 File Offset: 0x0008D230
	private static object[] DeserializeObjectArray(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = CustomSerializer.DeserializeObject(reader);
		}
		return array;
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x0008F064 File Offset: 0x0008D264
	private static NetEventOptions DeserializeNetEventOptions(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		int[] array = null;
		if (num2 > 0)
		{
			array = new int[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = reader.ReadInt32();
			}
		}
		byte b = reader.ReadByte();
		return new NetEventOptions(num, array, b);
	}
}
