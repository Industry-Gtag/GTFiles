using System;
using UnityEngine;

// Token: 0x0200049D RID: 1181
public class PlayerPrefFlags
{
	// Token: 0x06001C9F RID: 7327 RVA: 0x0009B17B File Offset: 0x0009937B
	internal static bool Check(PlayerPrefFlags.Flag flag)
	{
		return (PlayerPrefs.GetInt("PlayerPrefFlags0", 5) & (int)flag) == (int)flag;
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x0009B190 File Offset: 0x00099390
	internal static void Touch(PlayerPrefFlags.Flag flag)
	{
		bool flag2 = (PlayerPrefs.GetInt("PlayerPrefFlags0", 5) & (int)flag) == (int)flag;
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, flag2);
		}
	}

	// Token: 0x06001CA1 RID: 7329 RVA: 0x0009B1C4 File Offset: 0x000993C4
	internal static void TouchIf(PlayerPrefFlags.Flag flag, bool value)
	{
		int @int = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		if (value == ((@int & (int)flag) == (int)flag) && PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, value);
		}
	}

	// Token: 0x06001CA2 RID: 7330 RVA: 0x0009B1FC File Offset: 0x000993FC
	internal static void Set(PlayerPrefFlags.Flag flag, bool value)
	{
		int num = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		if (value)
		{
			num |= (int)flag;
		}
		else
		{
			num &= (int)(~(int)flag);
		}
		PlayerPrefs.SetInt("PlayerPrefFlags0", num);
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, value);
		}
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x0009B244 File Offset: 0x00099444
	internal static bool Flip(PlayerPrefFlags.Flag flag)
	{
		int num = PlayerPrefs.GetInt("PlayerPrefFlags0", 5);
		bool flag2 = (num & (int)flag) != (int)flag;
		if (flag2)
		{
			num |= (int)flag;
		}
		else
		{
			num &= (int)(~(int)flag);
		}
		PlayerPrefs.SetInt("PlayerPrefFlags0", num);
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange(flag, flag2);
		}
		return flag2;
	}

	// Token: 0x040026BC RID: 9916
	public static Action<PlayerPrefFlags.Flag, bool> OnFlagChange;

	// Token: 0x040026BD RID: 9917
	private const int defaultValue = 5;

	// Token: 0x0200049E RID: 1182
	public enum Flag
	{
		// Token: 0x040026BF RID: 9919
		SHOW_1P_COSMETICS = 1,
		// Token: 0x040026C0 RID: 9920
		SWAP_HELD_COSMETICS,
		// Token: 0x040026C1 RID: 9921
		GAME_MODE_SELECTOR_IS_SUPER = 4,
		// Token: 0x040026C2 RID: 9922
		GTV_MUTED = 8,
		// Token: 0x040026C3 RID: 9923
		ANTI_NAUSEA_ON = 16,
		// Token: 0x040026C4 RID: 9924
		GRAVDASH_FLIP_X = 32,
		// Token: 0x040026C5 RID: 9925
		GRAVDASH_FLIP_Y = 64
	}
}
