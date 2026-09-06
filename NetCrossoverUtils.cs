using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000453 RID: 1107
public static class NetCrossoverUtils
{
	// Token: 0x06001A73 RID: 6771 RVA: 0x0009431D File Offset: 0x0009251D
	public static void Prewarm()
	{
		NetCrossoverUtils.FixedBuffer = new byte[2048];
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x00094330 File Offset: 0x00092530
	public static void WriteNetDataToBuffer<T>(this T data, PhotonStream stream) where T : struct, INetworkStruct
	{
		if (stream.IsReading)
		{
			Debug.LogError("Attempted to write data to a reading stream!");
			return;
		}
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<T>(data, intPtr, true);
			Marshal.Copy(intPtr, NetCrossoverUtils.FixedBuffer, 0, num);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				stream.SendNext(NetCrossoverUtils.FixedBuffer[i]);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x000943C8 File Offset: 0x000925C8
	public static object ReadNetDataFromBuffer<T>(PhotonStream stream) where T : struct, INetworkStruct
	{
		if (stream.IsWriting)
		{
			Debug.LogError("Attmpted to read data from a writing stream!");
			return null;
		}
		IntPtr intPtr = 0;
		object obj;
		try
		{
			Type typeFromHandle = typeof(T);
			int num = (int)stream.ReceiveNext();
			int num2 = Marshal.SizeOf(typeFromHandle);
			if (num != num2)
			{
				Debug.LogError(string.Format("Expected datasize {0} when reading data for type '{1}',", num2, typeFromHandle.Name) + string.Format("but {0} data is available!", num));
				obj = null;
			}
			else
			{
				intPtr = Marshal.AllocHGlobal(num2);
				for (int i = 0; i < num2; i++)
				{
					NetCrossoverUtils.FixedBuffer[i] = (byte)stream.ReceiveNext();
				}
				Marshal.Copy(NetCrossoverUtils.FixedBuffer, 0, intPtr, num2);
				obj = (T)((object)Marshal.PtrToStructure(intPtr, typeFromHandle));
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return obj;
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x000944B0 File Offset: 0x000926B0
	public static void WriteNetDataToBuffer(this object data, PhotonStream stream)
	{
		if (stream.IsReading)
		{
			Debug.LogError("Attempted to write data to a reading stream!");
			return;
		}
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(data.GetType());
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(data, intPtr, true);
			Marshal.Copy(intPtr, NetCrossoverUtils.FixedBuffer, 0, num);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				stream.SendNext(NetCrossoverUtils.FixedBuffer[i]);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x00094544 File Offset: 0x00092744
	public static void SerializeToRPCData<T>(this RPCArgBuffer<T> argBuffer) where T : struct
	{
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<T>(argBuffer.Args, intPtr, true);
			Marshal.Copy(intPtr, argBuffer.Data, 0, num);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x000945A4 File Offset: 0x000927A4
	public static void PopulateWithRPCData<T>(this RPCArgBuffer<T> argBuffer, byte[] data) where T : struct
	{
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(data, 0, intPtr, num);
			argBuffer.Args = Marshal.PtrToStructure<T>(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x00094600 File Offset: 0x00092800
	public static Dictionary<string, SessionProperty> ToPropDict(this Hashtable hash)
	{
		Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty>();
		foreach (DictionaryEntry dictionaryEntry in hash)
		{
			dictionary.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
		}
		return dictionary;
	}

	// Token: 0x04002530 RID: 9520
	private const int MaxParameterByteLength = 2048;

	// Token: 0x04002531 RID: 9521
	private static byte[] FixedBuffer;
}
