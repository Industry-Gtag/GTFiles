using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020008AD RID: 2221
public class GorillaTagCompetitiveRankCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x17000534 RID: 1332
	// (get) Token: 0x06003A4B RID: 14923 RVA: 0x0013D1E2 File Offset: 0x0013B3E2
	// (set) Token: 0x06003A4C RID: 14924 RVA: 0x0013D1EA File Offset: 0x0013B3EA
	public bool IsSpawned { get; set; }

	// Token: 0x17000535 RID: 1333
	// (get) Token: 0x06003A4D RID: 14925 RVA: 0x0013D1F3 File Offset: 0x0013B3F3
	// (set) Token: 0x06003A4E RID: 14926 RVA: 0x0013D1FB File Offset: 0x0013B3FB
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06003A4F RID: 14927 RVA: 0x0013D204 File Offset: 0x0013B404
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnRankedSubtierChanged += this.OnRankedScoreChanged;
		this.OnRankedScoreChanged(this.myRig.GetCurrentRankedSubTier(false), this.myRig.GetCurrentRankedSubTier(true));
	}

	// Token: 0x06003A50 RID: 14928 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDespawn()
	{
	}

	// Token: 0x06003A51 RID: 14929 RVA: 0x0013D26A File Offset: 0x0013B46A
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.UpdateDisplayedCosmetic(-1, -1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x06003A52 RID: 14930 RVA: 0x0013D291 File Offset: 0x0013B491
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnRankedSubtierChanged -= this.OnRankedScoreChanged;
			this.myRig = null;
		}
	}

	// Token: 0x06003A53 RID: 14931 RVA: 0x0013D2C6 File Offset: 0x0013B4C6
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x06003A54 RID: 14932 RVA: 0x0013D2D8 File Offset: 0x0013B4D8
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnRankedSubtierChanged += this.OnRankedScoreChanged;
			this.OnRankedScoreChanged(this.myRig.GetCurrentRankedSubTier(false), this.myRig.GetCurrentRankedSubTier(true));
			return true;
		}
		return false;
	}

	// Token: 0x06003A55 RID: 14933 RVA: 0x0013D341 File Offset: 0x0013B541
	private void OnRankedScoreChanged(int questRank, int pcRank)
	{
		this.UpdateDisplayedCosmetic(questRank, pcRank);
	}

	// Token: 0x06003A56 RID: 14934 RVA: 0x0013D34C File Offset: 0x0013B54C
	private void UpdateDisplayedCosmetic(int questRank, int pcRank)
	{
		if (this.rankCosmetics == null)
		{
			return;
		}
		int num = (this.usePCELO ? pcRank : questRank);
		if (num <= 0)
		{
			num = 0;
		}
		for (int i = 0; i < this.rankCosmetics.Length; i++)
		{
			this.rankCosmetics[i].SetActive(i == num);
		}
	}

	// Token: 0x04004A3F RID: 19007
	[Tooltip("If enabled, display PC rank. Otherwise, display Quest rank")]
	[SerializeField]
	private bool usePCELO;

	// Token: 0x04004A40 RID: 19008
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x04004A41 RID: 19009
	[SerializeField]
	private VRRig myRig;

	// Token: 0x04004A42 RID: 19010
	[SerializeField]
	private GameObject[] rankCosmetics;
}
