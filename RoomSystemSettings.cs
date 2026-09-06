using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaTag;
using UnityEngine;

// Token: 0x02000D49 RID: 3401
[CreateAssetMenu(menuName = "ScriptableObjects/RoomSystemSettings", order = 2)]
internal class RoomSystemSettings : ScriptableObject
{
	// Token: 0x170007FE RID: 2046
	// (get) Token: 0x06005449 RID: 21577 RVA: 0x001BC17C File Offset: 0x001BA37C
	public ExpectedUsersDecayTimer ExpectedUsersTimer
	{
		get
		{
			return this.expectedUsersTimer;
		}
	}

	// Token: 0x170007FF RID: 2047
	// (get) Token: 0x0600544A RID: 21578 RVA: 0x001BC184 File Offset: 0x001BA384
	public TickSystemTimer ResyncNetworkTimeTimer
	{
		get
		{
			return this.resyncNetworkTimeTimer;
		}
	}

	// Token: 0x17000800 RID: 2048
	// (get) Token: 0x0600544B RID: 21579 RVA: 0x001BC18C File Offset: 0x001BA38C
	public CallLimiterWithCooldown StatusEffectLimiter
	{
		get
		{
			return this.statusEffectLimiter;
		}
	}

	// Token: 0x17000801 RID: 2049
	// (get) Token: 0x0600544C RID: 21580 RVA: 0x001BC194 File Offset: 0x001BA394
	public CallLimiterWithCooldown SoundEffectLimiter
	{
		get
		{
			return this.soundEffectLimiter;
		}
	}

	// Token: 0x17000802 RID: 2050
	// (get) Token: 0x0600544D RID: 21581 RVA: 0x001BC19C File Offset: 0x001BA39C
	public CallLimiterWithCooldown SoundEffectOtherLimiter
	{
		get
		{
			return this.soundEffectOtherLimiter;
		}
	}

	// Token: 0x17000803 RID: 2051
	// (get) Token: 0x0600544E RID: 21582 RVA: 0x001BC1A4 File Offset: 0x001BA3A4
	public CallLimiterWithCooldown PlayerEffectLimiter
	{
		get
		{
			return this.playerEffectLimiter;
		}
	}

	// Token: 0x17000804 RID: 2052
	// (get) Token: 0x0600544F RID: 21583 RVA: 0x001BC1AC File Offset: 0x001BA3AC
	public CallLimiterWithCooldown LavaSyncLimiter
	{
		get
		{
			return this.lavaSyncLimiter;
		}
	}

	// Token: 0x17000805 RID: 2053
	// (get) Token: 0x06005450 RID: 21584 RVA: 0x001BC1B4 File Offset: 0x001BA3B4
	public GameObject PlayerImpactEffect
	{
		get
		{
			return this.playerImpactEffect;
		}
	}

	// Token: 0x17000806 RID: 2054
	// (get) Token: 0x06005451 RID: 21585 RVA: 0x001BC1BC File Offset: 0x001BA3BC
	public List<RoomSystem.PlayerEffectConfig> PlayerEffects
	{
		get
		{
			return this.playerEffects;
		}
	}

	// Token: 0x17000807 RID: 2055
	// (get) Token: 0x06005452 RID: 21586 RVA: 0x001BC1C4 File Offset: 0x001BA3C4
	public int PausedDCTimer
	{
		get
		{
			return this.pausedDCTimer;
		}
	}

	// Token: 0x06005453 RID: 21587 RVA: 0x001BC1CC File Offset: 0x001BA3CC
	public int GetRoomCount(bool privateRoom, bool sub)
	{
		if (privateRoom)
		{
			if (!sub)
			{
				return this.privateRoomCountZoneModeMapping.GetRoomCount();
			}
			return this.subsPrivateRoomCountZoneModeMapping.GetRoomCount();
		}
		else
		{
			if (!sub)
			{
				return this.publicRoomCountZoneModeMapping.GetRoomCount();
			}
			return this.subsPublicRoomCountZoneModeMapping.GetRoomCount();
		}
	}

	// Token: 0x06005454 RID: 21588 RVA: 0x001BC208 File Offset: 0x001BA408
	public int GetRoomCount(GTZone zone, GameModeType mode, bool privateRoom, bool sub)
	{
		if (privateRoom)
		{
			if (!sub)
			{
				return this.privateRoomCountZoneModeMapping.GetRoomCount(zone, mode);
			}
			return this.subsPrivateRoomCountZoneModeMapping.GetRoomCount(zone, mode);
		}
		else
		{
			if (!sub)
			{
				return this.publicRoomCountZoneModeMapping.GetRoomCount(zone, mode);
			}
			return this.subsPublicRoomCountZoneModeMapping.GetRoomCount(zone, mode);
		}
	}

	// Token: 0x040065CC RID: 26060
	[SerializeField]
	private ExpectedUsersDecayTimer expectedUsersTimer;

	// Token: 0x040065CD RID: 26061
	[SerializeField]
	private TickSystemTimer resyncNetworkTimeTimer;

	// Token: 0x040065CE RID: 26062
	[SerializeField]
	private CallLimiterWithCooldown statusEffectLimiter;

	// Token: 0x040065CF RID: 26063
	[SerializeField]
	private CallLimiterWithCooldown soundEffectLimiter;

	// Token: 0x040065D0 RID: 26064
	[SerializeField]
	private CallLimiterWithCooldown soundEffectOtherLimiter;

	// Token: 0x040065D1 RID: 26065
	[SerializeField]
	private CallLimiterWithCooldown playerEffectLimiter;

	// Token: 0x040065D2 RID: 26066
	[SerializeField]
	private CallLimiterWithCooldown lavaSyncLimiter;

	// Token: 0x040065D3 RID: 26067
	[SerializeField]
	private GameObject playerImpactEffect;

	// Token: 0x040065D4 RID: 26068
	[SerializeField]
	private List<RoomSystem.PlayerEffectConfig> playerEffects = new List<RoomSystem.PlayerEffectConfig>();

	// Token: 0x040065D5 RID: 26069
	[SerializeField]
	private int pausedDCTimer;

	// Token: 0x040065D6 RID: 26070
	[SerializeField]
	private RoomCount publicRoomCountZoneModeMapping;

	// Token: 0x040065D7 RID: 26071
	[SerializeField]
	private PrivateRoomCount privateRoomCountZoneModeMapping;

	// Token: 0x040065D8 RID: 26072
	[SerializeField]
	private RoomCount subsPublicRoomCountZoneModeMapping;

	// Token: 0x040065D9 RID: 26073
	[SerializeField]
	private PrivateRoomCount subsPrivateRoomCountZoneModeMapping;
}
