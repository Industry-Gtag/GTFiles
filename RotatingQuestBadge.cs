using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class RotatingQuestBadge : MonoBehaviour, ISpawnable
{
	// Token: 0x1700019B RID: 411
	// (get) Token: 0x06001062 RID: 4194 RVA: 0x00057EEF File Offset: 0x000560EF
	// (set) Token: 0x06001063 RID: 4195 RVA: 0x00057EF7 File Offset: 0x000560F7
	public bool IsSpawned { get; set; }

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06001064 RID: 4196 RVA: 0x00057F00 File Offset: 0x00056100
	// (set) Token: 0x06001065 RID: 4197 RVA: 0x00057F08 File Offset: 0x00056108
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001066 RID: 4198 RVA: 0x00057F14 File Offset: 0x00056114
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnQuestScoreChanged += this.OnProgressScoreChanged;
		this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDespawn()
	{
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x00057F6D File Offset: 0x0005616D
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.SetBadgeLevel(-1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x00057F93 File Offset: 0x00056193
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnQuestScoreChanged -= this.OnProgressScoreChanged;
			this.myRig = null;
		}
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x00057FC8 File Offset: 0x000561C8
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x00057FD8 File Offset: 0x000561D8
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnQuestScoreChanged += this.OnProgressScoreChanged;
			this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
			return true;
		}
		return false;
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x00058034 File Offset: 0x00056234
	private void OnProgressScoreChanged(int score)
	{
		score = Mathf.Clamp(score, 0, 99999);
		this.displayField.text = score.ToString();
		this.UpdateBadge(score);
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x00058060 File Offset: 0x00056260
	private void UpdateBadge(int score)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			if (this.badgeLevels[i].requiredPoints <= score && this.badgeLevels[i].requiredPoints > num)
			{
				num = this.badgeLevels[i].requiredPoints;
				num2 = i;
			}
		}
		this.SetBadgeLevel(num2);
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x000580C8 File Offset: 0x000562C8
	private void SetBadgeLevel(int level)
	{
		level = Mathf.Clamp(level, 0, this.badgeLevels.Length - 1);
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			this.badgeLevels[i].badge.SetActive(i == level);
		}
	}

	// Token: 0x04001394 RID: 5012
	[SerializeField]
	private TextMeshPro displayField;

	// Token: 0x04001395 RID: 5013
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x04001396 RID: 5014
	[SerializeField]
	private VRRig myRig;

	// Token: 0x04001397 RID: 5015
	[SerializeField]
	private RotatingQuestBadge.BadgeLevel[] badgeLevels;

	// Token: 0x02000264 RID: 612
	[Serializable]
	public struct BadgeLevel
	{
		// Token: 0x0400139A RID: 5018
		public GameObject badge;

		// Token: 0x0400139B RID: 5019
		public int requiredPoints;
	}
}
