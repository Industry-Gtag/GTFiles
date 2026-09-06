using System;
using UnityEngine;

// Token: 0x0200045F RID: 1119
[Serializable]
public struct NetworkSystemConfig
{
	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06001ACE RID: 6862 RVA: 0x00094B7D File Offset: 0x00092D7D
	public static string AppVersion
	{
		get
		{
			return NetworkSystemConfig.prependCode + "." + NetworkSystemConfig.AppVersionStripped;
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x06001ACF RID: 6863 RVA: 0x00094B94 File Offset: 0x00092D94
	public static string AppVersionStripped
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.gameVersionType,
				".",
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x06001AD0 RID: 6864 RVA: 0x00094BF4 File Offset: 0x00092DF4
	public static string BundleVersion
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x06001AD1 RID: 6865 RVA: 0x00094C43 File Offset: 0x00092E43
	public static string GameVersionType
	{
		get
		{
			return NetworkSystemConfig.gameVersionType;
		}
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x06001AD2 RID: 6866 RVA: 0x00094C4A File Offset: 0x00092E4A
	public static int GameMajorVersion
	{
		get
		{
			return NetworkSystemConfig.majorVersion;
		}
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x06001AD3 RID: 6867 RVA: 0x00094C51 File Offset: 0x00092E51
	public static int GameMinorVersion
	{
		get
		{
			return NetworkSystemConfig.minorVersion;
		}
	}

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x06001AD4 RID: 6868 RVA: 0x00094C58 File Offset: 0x00092E58
	public static int GameMinorVersion2
	{
		get
		{
			return NetworkSystemConfig.minorVersion2;
		}
	}

	// Token: 0x04002576 RID: 9590
	[HideInInspector]
	public int MaxPlayerCount;

	// Token: 0x04002577 RID: 9591
	private static string gameVersionType = "live1";

	// Token: 0x04002578 RID: 9592
	public static string prependCode = "GoodMonkeMallPrepend824";

	// Token: 0x04002579 RID: 9593
	public static int majorVersion = 1;

	// Token: 0x0400257A RID: 9594
	public static int minorVersion = 1;

	// Token: 0x0400257B RID: 9595
	public static int minorVersion2 = 145;
}
