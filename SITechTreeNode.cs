using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000167 RID: 359
[Serializable]
public class SITechTreeNode
{
	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000978 RID: 2424 RVA: 0x00032C19 File Offset: 0x00030E19
	// (set) Token: 0x06000979 RID: 2425 RVA: 0x00032C21 File Offset: 0x00030E21
	public EAssetReleaseTier EdReleaseTier
	{
		get
		{
			return this.m_edReleaseTier;
		}
		set
		{
			this.m_edReleaseTier = value;
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x0600097A RID: 2426 RVA: 0x00032C2C File Offset: 0x00030E2C
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier edReleaseTier = this.m_edReleaseTier;
			return edReleaseTier != EAssetReleaseTier.Disabled && edReleaseTier <= EAssetReleaseTier.PublicRC && (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x0600097B RID: 2427 RVA: 0x00032C58 File Offset: 0x00030E58
	public bool IsAllowed
	{
		get
		{
			return (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x0600097C RID: 2428 RVA: 0x00032C69 File Offset: 0x00030E69
	public bool IsDispensableGadget
	{
		get
		{
			return this.IsValid && this.unlockedGadgetPrefab && this.IsAllowed;
		}
	}

	// Token: 0x04000B7C RID: 2940
	[SerializeField]
	private EAssetReleaseTier m_edReleaseTier = (EAssetReleaseTier)(-1);

	// Token: 0x04000B7D RID: 2941
	public SIUpgradeType upgradeType;

	// Token: 0x04000B7E RID: 2942
	public string nickName;

	// Token: 0x04000B7F RID: 2943
	public string description;

	// Token: 0x04000B80 RID: 2944
	public ESuperGameModes excludedGameModes;

	// Token: 0x04000B81 RID: 2945
	public SIUpgradeType[] parentUpgrades;

	// Token: 0x04000B82 RID: 2946
	public GameEntity unlockedGadgetPrefab;

	// Token: 0x04000B83 RID: 2947
	public SIResource.ResourceCost[] nodeCost;

	// Token: 0x04000B84 RID: 2948
	public bool costOverride;
}
