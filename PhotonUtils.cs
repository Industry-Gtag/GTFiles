using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

// Token: 0x02000D09 RID: 3337
public static class PhotonUtils
{
	// Token: 0x060052C7 RID: 21191 RVA: 0x001B5404 File Offset: 0x001B3604
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11, out T12 arg12)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
		arg11 = (T11)((object)args[startIndex + 10]);
		arg12 = (T12)((object)args[startIndex + 11]);
	}

	// Token: 0x060052C8 RID: 21192 RVA: 0x001B54DC File Offset: 0x001B36DC
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
		arg11 = (T11)((object)args[startIndex + 10]);
	}

	// Token: 0x060052C9 RID: 21193 RVA: 0x001B55A4 File Offset: 0x001B37A4
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
	}

	// Token: 0x060052CA RID: 21194 RVA: 0x001B5658 File Offset: 0x001B3858
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
	}

	// Token: 0x060052CB RID: 21195 RVA: 0x001B56FC File Offset: 0x001B38FC
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
	}

	// Token: 0x060052CC RID: 21196 RVA: 0x001B5790 File Offset: 0x001B3990
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
	}

	// Token: 0x060052CD RID: 21197 RVA: 0x001B5810 File Offset: 0x001B3A10
	public static void ParseArgs<T1, T2, T3, T4, T5, T6>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
	}

	// Token: 0x060052CE RID: 21198 RVA: 0x001B5880 File Offset: 0x001B3A80
	public static void ParseArgs<T1, T2, T3, T4, T5>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
	}

	// Token: 0x060052CF RID: 21199 RVA: 0x001B58E0 File Offset: 0x001B3AE0
	public static void ParseArgs<T1, T2, T3, T4>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
	}

	// Token: 0x060052D0 RID: 21200 RVA: 0x001B592D File Offset: 0x001B3B2D
	public static void ParseArgs<T1, T2, T3>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
	}

	// Token: 0x060052D1 RID: 21201 RVA: 0x001B595E File Offset: 0x001B3B5E
	public static void ParseArgs<T1, T2>(this object[] args, int startIndex, out T1 arg1, out T2 arg2)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
	}

	// Token: 0x060052D2 RID: 21202 RVA: 0x001B597E File Offset: 0x001B3B7E
	public static void ParseArgs<T1>(this object[] args, int startIndex, out T1 arg1)
	{
		arg1 = (T1)((object)args[startIndex]);
	}

	// Token: 0x060052D3 RID: 21203 RVA: 0x001B5990 File Offset: 0x001B3B90
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11, out T12 arg12)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		arg11 = default(T11);
		arg12 = default(T12);
		if (args == null || args.Length < startIndex + 12)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10, out arg11, out arg12);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052D4 RID: 21204 RVA: 0x001B5A44 File Offset: 0x001B3C44
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		arg11 = default(T11);
		if (args == null || args.Length < startIndex + 11)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10, out arg11);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052D5 RID: 21205 RVA: 0x001B5AEC File Offset: 0x001B3CEC
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		if (args == null || args.Length < startIndex + 10)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052D6 RID: 21206 RVA: 0x001B5B8C File Offset: 0x001B3D8C
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		if (args == null || args.Length < startIndex + 9)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052D7 RID: 21207 RVA: 0x001B5C20 File Offset: 0x001B3E20
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		if (args == null || args.Length < startIndex + 8)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052D8 RID: 21208 RVA: 0x001B5CA8 File Offset: 0x001B3EA8
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		if (args == null || args.Length < startIndex + 7)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052D9 RID: 21209 RVA: 0x001B5D28 File Offset: 0x001B3F28
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		if (args == null || args.Length < startIndex + 6)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052DA RID: 21210 RVA: 0x001B5D9C File Offset: 0x001B3F9C
	public static bool TryParseArgs<T1, T2, T3, T4, T5>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		if (args == null || args.Length < startIndex + 5)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052DB RID: 21211 RVA: 0x001B5E08 File Offset: 0x001B4008
	public static bool TryParseArgs<T1, T2, T3, T4>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		if (args == null || args.Length < startIndex + 4)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052DC RID: 21212 RVA: 0x001B5E68 File Offset: 0x001B4068
	public static bool TryParseArgs<T1, T2, T3>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		if (args == null || args.Length < startIndex + 3)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052DD RID: 21213 RVA: 0x001B5EC0 File Offset: 0x001B40C0
	public static bool TryParseArgs<T1, T2>(this object[] args, int startIndex, out T1 arg1, out T2 arg2)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		if (args == null || args.Length < startIndex + 2)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052DE RID: 21214 RVA: 0x001B5F0C File Offset: 0x001B410C
	public static bool TryParseArgs<T1>(this object[] args, int startIndex, out T1 arg1)
	{
		arg1 = default(T1);
		if (args == null || args.Length < startIndex + 1)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x060052DF RID: 21215 RVA: 0x001B5F50 File Offset: 0x001B4150
	public static readonly ref T[] FetchDelegatesNonAlloc<T>(T @delegate) where T : MulticastDelegate
	{
		if (@delegate == null)
		{
			return PhotonUtils.EmptyArray<T>.Ref();
		}
		return @delegate.GetInvocationListUnsafe<T>();
	}

	// Token: 0x060052E0 RID: 21216 RVA: 0x001B5F6C File Offset: 0x001B416C
	public static object[] FetchScratchArray(int size)
	{
		if (size < 0)
		{
			throw new Exception("Size cannot be less than 0.");
		}
		object[] array;
		if (!PhotonUtils.gLengthToArgsArray.TryGetValue(size, out array))
		{
			array = new object[size];
			PhotonUtils.gLengthToArgsArray.Add(size, array);
		}
		return array;
	}

	// Token: 0x060052E1 RID: 21217 RVA: 0x001B5FAC File Offset: 0x001B41AC
	public static NetPlayer GetNetPlayer(int actorNumber)
	{
		NetworkSystem networkSystem;
		if (!PhotonUtils.TryGetNetSystem(out networkSystem))
		{
			return null;
		}
		return networkSystem.GetPlayer(actorNumber);
	}

	// Token: 0x170007D0 RID: 2000
	// (get) Token: 0x060052E2 RID: 21218 RVA: 0x001B5FCC File Offset: 0x001B41CC
	public static int LocalActorNumber
	{
		get
		{
			NetPlayer localNetPlayer = PhotonUtils.LocalNetPlayer;
			if (localNetPlayer == null)
			{
				return -1;
			}
			return localNetPlayer.ActorNumber;
		}
	}

	// Token: 0x170007D1 RID: 2001
	// (get) Token: 0x060052E3 RID: 21219 RVA: 0x001B5FEC File Offset: 0x001B41EC
	public static NetPlayer LocalNetPlayer
	{
		get
		{
			if (PhotonUtils.gLocalNetPlayer != null)
			{
				return PhotonUtils.gLocalNetPlayer;
			}
			NetworkSystem networkSystem;
			if (PhotonUtils.TryGetNetSystem(out networkSystem))
			{
				PhotonUtils.gLocalNetPlayer = networkSystem.GetLocalPlayer();
			}
			return PhotonUtils.gLocalNetPlayer;
		}
	}

	// Token: 0x060052E4 RID: 21220 RVA: 0x001B601F File Offset: 0x001B421F
	private static bool TryGetNetSystem(out NetworkSystem ns)
	{
		if (!PhotonUtils.gNetSystem)
		{
			PhotonUtils.gNetSystem = NetworkSystem.Instance;
		}
		if (!PhotonUtils.gNetSystem)
		{
			ns = null;
			return false;
		}
		ns = PhotonUtils.gNetSystem;
		return true;
	}

	// Token: 0x060052E5 RID: 21221 RVA: 0x001B6050 File Offset: 0x001B4250
	static PhotonUtils()
	{
		for (int i = 0; i <= 16; i++)
		{
			PhotonUtils.gLengthToArgsArray.Add(i, new object[i]);
		}
	}

	// Token: 0x04006487 RID: 25735
	private static NetworkSystem gNetSystem;

	// Token: 0x04006488 RID: 25736
	private static NetPlayer gLocalNetPlayer;

	// Token: 0x04006489 RID: 25737
	private static readonly Dictionary<int, object[]> gLengthToArgsArray = new Dictionary<int, object[]>(16);

	// Token: 0x0400648A RID: 25738
	private const int ARG_ARRAYS = 16;

	// Token: 0x02000D0A RID: 3338
	private static class EmptyArray<T>
	{
		// Token: 0x060052E6 RID: 21222 RVA: 0x001B6087 File Offset: 0x001B4287
		public static readonly ref T[] Ref()
		{
			return ref PhotonUtils.EmptyArray<T>.gEmpty;
		}

		// Token: 0x0400648B RID: 25739
		private static readonly T[] gEmpty = Array.Empty<T>();
	}

	// Token: 0x02000D0B RID: 3339
	public static class CustomTypes
	{
		// Token: 0x060052E8 RID: 21224 RVA: 0x001B609C File Offset: 0x001B429C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitOnLoad()
		{
			PhotonPeer.RegisterType(typeof(Color32), 67, new SerializeMethod(PhotonUtils.CustomTypes.SerializeColor32), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeColor32));
			PhotonPeer.RegisterType(typeof(global::UnityEngine.BoundsInt), 73, new SerializeMethod(PhotonUtils.CustomTypes.SerializeBoundsInt), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeBoundsInt));
			PhotonPeer.RegisterType(typeof(int3), 74, new SerializeMethod(PhotonUtils.CustomTypes.SerializeInt3), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeInt3));
			PhotonPeer.RegisterType(typeof(Voxel), 88, new SerializeStreamMethod(PhotonUtils.CustomTypes.SerializeVoxel), new DeserializeStreamMethod(PhotonUtils.CustomTypes.DeserializeVoxel));
			PhotonPeer.RegisterType(typeof(VoxelAction), 89, new SerializeMethod(PhotonUtils.CustomTypes.SerializeVoxelAction), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeVoxelAction));
			PhotonPeer.RegisterType(typeof(VoxelOperation), 90, new SerializeMethod(PhotonUtils.CustomTypes.SerializeVoxelOperation), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeVoxelOperation));
			PhotonPeer.RegisterType(typeof(VoxelManager.VoxelMineOperation), 77, new SerializeMethod(PhotonUtils.CustomTypes.SerializeVoxelMineOperation), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeVoxelMineOperation));
		}

		// Token: 0x060052E9 RID: 21225 RVA: 0x001B61CF File Offset: 0x001B43CF
		public static byte[] SerializeColor32(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<Color32>((Color32)value);
		}

		// Token: 0x060052EA RID: 21226 RVA: 0x001B61DC File Offset: 0x001B43DC
		public static object DeserializeColor32(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<Color32>(data);
		}

		// Token: 0x060052EB RID: 21227 RVA: 0x001B61E9 File Offset: 0x001B43E9
		public static byte[] SerializeBoundsInt(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<global::UnityEngine.BoundsInt>((global::UnityEngine.BoundsInt)value);
		}

		// Token: 0x060052EC RID: 21228 RVA: 0x001B61F6 File Offset: 0x001B43F6
		public static object DeserializeBoundsInt(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<global::UnityEngine.BoundsInt>(data);
		}

		// Token: 0x060052ED RID: 21229 RVA: 0x001B6203 File Offset: 0x001B4403
		public static byte[] SerializeInt3(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<int3>((int3)value);
		}

		// Token: 0x060052EE RID: 21230 RVA: 0x001B6210 File Offset: 0x001B4410
		public static object DeserializeInt3(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<int3>(data);
		}

		// Token: 0x060052EF RID: 21231 RVA: 0x001B621D File Offset: 0x001B441D
		public static byte[] SerializeVoxelAction(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<VoxelAction>((VoxelAction)value);
		}

		// Token: 0x060052F0 RID: 21232 RVA: 0x001B622A File Offset: 0x001B442A
		public static object DeserializeVoxelAction(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<VoxelAction>(data);
		}

		// Token: 0x060052F1 RID: 21233 RVA: 0x001B6237 File Offset: 0x001B4437
		public static byte[] SerializeVoxelOperation(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<VoxelOperation>((VoxelOperation)value);
		}

		// Token: 0x060052F2 RID: 21234 RVA: 0x001B6244 File Offset: 0x001B4444
		public static object DeserializeVoxelOperation(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<VoxelOperation>(data);
		}

		// Token: 0x060052F3 RID: 21235 RVA: 0x001B6251 File Offset: 0x001B4451
		public static byte[] SerializeVoxelMineOperation(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<VoxelManager.VoxelMineOperation>((VoxelManager.VoxelMineOperation)value);
		}

		// Token: 0x060052F4 RID: 21236 RVA: 0x001B625E File Offset: 0x001B445E
		public static object DeserializeVoxelMineOperation(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<VoxelManager.VoxelMineOperation>(data);
		}

		// Token: 0x060052F5 RID: 21237 RVA: 0x001B626C File Offset: 0x001B446C
		private static short SerializeVoxel(StreamBuffer stream, object value)
		{
			Voxel voxel = (Voxel)value;
			byte[] array = PhotonUtils.CustomTypes.memVox;
			lock (array)
			{
				byte[] array2 = PhotonUtils.CustomTypes.memVox;
				array2[0] = voxel.Material;
				array2[1] = voxel.Density;
				stream.Write(array2, 0, 2);
			}
			return 2;
		}

		// Token: 0x060052F6 RID: 21238 RVA: 0x001B62D0 File Offset: 0x001B44D0
		private static object DeserializeVoxel(StreamBuffer stream, short length)
		{
			Voxel voxel = default(Voxel);
			if (length == 2)
			{
				return voxel;
			}
			byte[] array = PhotonUtils.CustomTypes.memVox;
			lock (array)
			{
				stream.Read(PhotonUtils.CustomTypes.memVox, 0, 2);
				voxel.Material = PhotonUtils.CustomTypes.memVox[0];
				voxel.Density = PhotonUtils.CustomTypes.memVox[1];
			}
			return voxel;
		}

		// Token: 0x060052F7 RID: 21239 RVA: 0x001B634C File Offset: 0x001B454C
		private static T CastToStruct<T>(byte[] bytes) where T : struct
		{
			return MemoryMarshal.Read<T>(bytes);
		}

		// Token: 0x060052F8 RID: 21240 RVA: 0x001B6359 File Offset: 0x001B4559
		private static byte[] CastToBytes<T>(T data) where T : struct
		{
			byte[] staticArray = PhotonUtils.CustomTypes._arrayBag.GetStaticArray(Marshal.SizeOf<T>());
			MemoryMarshal.Write<T>(staticArray, ref data);
			return staticArray;
		}

		// Token: 0x0400648C RID: 25740
		private static StaticArrayBag<byte> _arrayBag = new StaticArrayBag<byte>();

		// Token: 0x0400648D RID: 25741
		private const short LEN_C32 = 4;

		// Token: 0x0400648E RID: 25742
		private const int SizeVox = 2;

		// Token: 0x0400648F RID: 25743
		private static readonly byte[] memVox = new byte[2];
	}
}
