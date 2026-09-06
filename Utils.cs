using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000DFD RID: 3581
public static class Utils
{
	// Token: 0x060057AE RID: 22446 RVA: 0x001C95A0 File Offset: 0x001C77A0
	public static void Disable(this GameObject target)
	{
		if (!target.activeSelf)
		{
			return;
		}
		PooledList<IPreDisable> pooledList = Utils.g_listPool.Take();
		List<IPreDisable> list = pooledList.List;
		target.GetComponents<IPreDisable>(list);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			try
			{
				list[i].PreDisable();
			}
			catch (Exception)
			{
			}
		}
		target.SetActive(false);
		Utils.g_listPool.Return(pooledList);
	}

	// Token: 0x060057AF RID: 22447 RVA: 0x001C9618 File Offset: 0x001C7818
	public static void AddIfNew<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
		}
	}

	// Token: 0x060057B0 RID: 22448 RVA: 0x001C962A File Offset: 0x001C782A
	public static void RemoveIfContains<T>(this List<T> list, T item)
	{
		if (list.Contains(item))
		{
			list.Remove(item);
		}
	}

	// Token: 0x060057B1 RID: 22449 RVA: 0x001C963D File Offset: 0x001C783D
	public static bool InRoom(this NetPlayer player)
	{
		return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.AllNetPlayers.Contains(player);
	}

	// Token: 0x060057B2 RID: 22450 RVA: 0x001C9660 File Offset: 0x001C7860
	public static bool PlayerInRoom(int actorNumber)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			for (int i = 0; i < allNetPlayers.Length; i++)
			{
				if (allNetPlayers[i].ActorNumber == actorNumber)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060057B3 RID: 22451 RVA: 0x001C96A0 File Offset: 0x001C78A0
	public static bool PlayerInRoom(int actorNumer, out Player photonPlayer)
	{
		photonPlayer = null;
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumer, out photonPlayer);
	}

	// Token: 0x060057B4 RID: 22452 RVA: 0x001C96BF File Offset: 0x001C78BF
	public static bool PlayerInRoom(int actorNumber, out NetPlayer player)
	{
		if (NetworkSystem.Instance == null)
		{
			player = null;
			return false;
		}
		player = NetworkSystem.Instance.GetPlayer(actorNumber);
		return NetworkSystem.Instance.InRoom && player != null;
	}

	// Token: 0x060057B5 RID: 22453 RVA: 0x001C96F4 File Offset: 0x001C78F4
	public static long PackVector3ToLong(Vector3 vector)
	{
		long num = (long)Mathf.Clamp(Mathf.RoundToInt(vector.x * 1024f) + 1048576, 0, 2097151);
		long num2 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.y * 1024f) + 1048576, 0, 2097151);
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.z * 1024f) + 1048576, 0, 2097151);
		return num + (num2 << 21) + (num3 << 42);
	}

	// Token: 0x060057B6 RID: 22454 RVA: 0x001C9778 File Offset: 0x001C7978
	public static Vector3 UnpackVector3FromLong(long data)
	{
		float num = (float)(data & 2097151L);
		long num2 = (data >> 21) & 2097151L;
		long num3 = (data >> 42) & 2097151L;
		return new Vector3((float)((long)num - 1048576L) * 0.0009765625f, (float)(num2 - 1048576L) * 0.0009765625f, (float)(num3 - 1048576L) * 0.0009765625f);
	}

	// Token: 0x060057B7 RID: 22455 RVA: 0x001C97D6 File Offset: 0x001C79D6
	public static bool IsASCIILetterOrDigit(char c)
	{
		return (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z');
	}

	// Token: 0x060057B8 RID: 22456 RVA: 0x00002C2D File Offset: 0x00000E2D
	public static void Log(object message)
	{
	}

	// Token: 0x060057B9 RID: 22457 RVA: 0x00002C2D File Offset: 0x00000E2D
	public static void Log(object message, Object context)
	{
	}

	// Token: 0x060057BA RID: 22458 RVA: 0x001C9800 File Offset: 0x001C7A00
	public static bool ValidateServerTime(double time, double maximumLatency)
	{
		double currentTime = PhotonNetwork.CurrentTime;
		double num = 4294967.295 - maximumLatency;
		double num2;
		if (currentTime > maximumLatency || time < maximumLatency)
		{
			if (time > currentTime + 0.5)
			{
				return false;
			}
			num2 = currentTime - time;
		}
		else
		{
			double num3 = num + currentTime;
			if (time > currentTime + 0.5 && time < num3)
			{
				return false;
			}
			num2 = currentTime + (4294967.295 - time);
		}
		return num2 <= maximumLatency;
	}

	// Token: 0x060057BB RID: 22459 RVA: 0x001C9870 File Offset: 0x001C7A70
	public static double CalculateNetworkDeltaTime(double prevTime, double newTime)
	{
		if (newTime >= prevTime)
		{
			return newTime - prevTime;
		}
		double num = 4294967.295 - prevTime;
		return newTime + num;
	}

	// Token: 0x0400682E RID: 26670
	private static ObjectPool<PooledList<IPreDisable>> g_listPool = new ObjectPool<PooledList<IPreDisable>>(2, 10);

	// Token: 0x0400682F RID: 26671
	private static StringBuilder reusableSB = new StringBuilder();
}
