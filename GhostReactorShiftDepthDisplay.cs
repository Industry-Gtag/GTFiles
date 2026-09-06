using System;
using System.Collections.Generic;
using System.Text;
using GorillaTagScripts.GhostReactor;
using TMPro;
using UnityEngine;

// Token: 0x02000722 RID: 1826
[Serializable]
public class GhostReactorShiftDepthDisplay
{
	// Token: 0x06002E68 RID: 11880 RVA: 0x000FCDF1 File Offset: 0x000FAFF1
	public void Setup()
	{
		this.StopDelveDeeperFX();
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x000FCDF9 File Offset: 0x000FAFF9
	public int GetRewardXP()
	{
		return this.reactor.GetDepthLevel() * 10 + 10;
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x000FCE0C File Offset: 0x000FB00C
	public void RefreshDisplay()
	{
		int depthLevel = this.reactor.GetDepthLevel();
		this.reactor.GetDepthLevelConfig(depthLevel);
		this.reactor.GetDepthLevelConfig(depthLevel + 1);
		switch (this.shiftManager.GetState())
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
		case GhostReactorShiftManager.State.ShiftActive:
		{
			foreach (TMP_Text tmp_Text in this.logoFrames)
			{
				tmp_Text.gameObject.SetActive(false);
			}
			this.cachedStringBuilder.Clear();
			this.cachedStringBuilder.Append("<color=grey>Team Goals:</color>\n");
			int num = 0;
			if (this.shiftManager.coresRequiredToDelveDeeper > 0)
			{
				int num2 = Math.Min(this.shiftManager.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected), this.shiftManager.coresRequiredToDelveDeeper);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format("Deposit {0} Cores ", this.shiftManager.coresRequiredToDelveDeeper));
				stringBuilder.Append(string.Format("({0}/{1})", num2, this.shiftManager.coresRequiredToDelveDeeper));
				stringBuilder.Append("\n");
				this.cachedStringBuilder.Append(stringBuilder);
				num++;
			}
			if (this.shiftManager.sentientCoresRequiredToDelveDeeper > 0)
			{
				int num3 = Math.Min(this.shiftManager.shiftStats.GetShiftStat(GRShiftStatType.SentientCoresCollected), this.shiftManager.sentientCoresRequiredToDelveDeeper);
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(string.Format("Collect {0} Seeds ", this.shiftManager.sentientCoresRequiredToDelveDeeper));
				stringBuilder2.Append(string.Format("({0}/{1})", num3, this.shiftManager.sentientCoresRequiredToDelveDeeper));
				stringBuilder2.Append("\n");
				this.cachedStringBuilder.Append(stringBuilder2);
				num++;
			}
			foreach (GREnemyCount grenemyCount in this.shiftManager.killsRequiredToDelveDeeper)
			{
				if (grenemyCount.Count > 0)
				{
					int num4 = (this.shiftManager.shiftStats.EnemyKills.ContainsKey(grenemyCount.GetEnemyType()) ? Math.Min(this.shiftManager.shiftStats.EnemyKills[grenemyCount.GetEnemyType()], grenemyCount.Count) : 0);
					StringBuilder stringBuilder3 = new StringBuilder();
					string text = "Kill";
					if (grenemyCount.EnemyType == GREnemyType.MoonBoss_Phase1 || grenemyCount.EnemyType == GREnemyType.MoonBoss_Phase2)
					{
						text = "Repel";
					}
					stringBuilder3.Append((grenemyCount.Count == 1) ? (text + " 1 " + grenemyCount.GetEnemyName() + " ") : string.Format("{0} {1} {2} ", text, grenemyCount.Count, grenemyCount.GetEnemyType().Pluralize()));
					stringBuilder3.Append(string.Format("({0}/{1})", num4, grenemyCount.Count));
					stringBuilder3.Append("\n");
					this.cachedStringBuilder.Append(stringBuilder3);
				}
			}
			if (this.shiftManager.maxPlayerDeaths >= 0)
			{
				StringBuilder stringBuilder4 = new StringBuilder();
				stringBuilder4.Append(string.Format("Limit Incidents to {0} ", this.shiftManager.maxPlayerDeaths));
				stringBuilder4.Append(string.Format("({0} so far)", this.shiftManager.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths)));
				stringBuilder4.Append("\n");
				this.cachedStringBuilder.Append(stringBuilder4);
				num++;
			}
			this.jumbotronRequirements.text = this.cachedStringBuilder.ToString();
			int num5 = this.reactor.GetCurrLevelGenConfig().coresRequired * 5;
			int rewardXP = this.GetRewardXP();
			this.cachedStringBuilder.Clear();
			this.cachedStringBuilder.Append("<color=grey>Rewards:</color>\n");
			this.cachedStringBuilder.Append(string.Format("+⑭{0}\n", num5));
			this.cachedStringBuilder.Append(string.Format("+{0} XP\n", rewardXP));
			this.jumbotronRewards.text = this.cachedStringBuilder.ToString();
			break;
		}
		case GhostReactorShiftManager.State.PreparingToDrill:
			this.jumbotronRequirements.text = "";
			this.jumbotronRewards.text = "";
			break;
		case GhostReactorShiftManager.State.Drilling:
			this.jumbotronRequirements.text = "";
			this.jumbotronRewards.text = "";
			break;
		}
		if (this.jumbotronState != null)
		{
			int state = (int)this.shiftManager.GetState();
			if (state >= 0 && state < GhostReactorShiftDepthDisplay.STATE_NAMES.Length)
			{
				this.jumbotronState.text = GhostReactorShiftDepthDisplay.STATE_NAMES[state];
			}
			else
			{
				this.jumbotronState.text = null;
			}
		}
		this.RefreshObjectives();
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x000FD350 File Offset: 0x000FB550
	public void RefreshObjectives()
	{
		GRShiftStat shiftStats = this.shiftManager.shiftStats;
		bool flag = shiftStats.GetShiftStat(GRShiftStatType.CoresCollected) >= this.shiftManager.coresRequiredToDelveDeeper;
		bool flag2 = shiftStats.GetShiftStat(GRShiftStatType.SentientCoresCollected) >= this.shiftManager.sentientCoresRequiredToDelveDeeper;
		bool flag3 = this.shiftManager.maxPlayerDeaths < 0 || shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths) <= this.shiftManager.maxPlayerDeaths;
		bool flag4 = true;
		foreach (GREnemyCount grenemyCount in this.shiftManager.killsRequiredToDelveDeeper)
		{
			if (shiftStats.EnemyKills.GetValueOrDefault(grenemyCount.GetEnemyType()) < grenemyCount.Count)
			{
				flag4 = false;
				break;
			}
		}
		if (this.shiftManager.ShiftActive && flag && flag2 && flag3 && flag4)
		{
			this.shiftManager.authorizedToDelveDeeper = true;
		}
		if (this.shiftManager.IsSoaking())
		{
			this.shiftManager.authorizedToDelveDeeper = true;
		}
		if (this.shiftManager.authorizedToDelveDeeper && this.jumbotronRequirements != null)
		{
			this.jumbotronRequirements.text = "<color=green>AUTHORIZED TO\nDELVE DEEPER</color>";
		}
		bool authorizedToDelveDeeper = this.shiftManager.authorizedToDelveDeeper;
		if (this.delveDeeperButton != null)
		{
			this.delveDeeperButton.SetActive(authorizedToDelveDeeper && !this.shiftManager.ShiftActive);
		}
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x000FD4CC File Offset: 0x000FB6CC
	public void StartDelveDeeperFX()
	{
		this.delveDeeperAudio.Play();
		this.delveDeeperNonspatializedAudio.Play();
		for (int i = 0; i < this.delveDeeperAnims.Count; i++)
		{
			this.delveDeeperAnims[i].Play();
		}
		for (int j = 0; j < this.delveDeeperAnimators.Count; j++)
		{
			this.delveDeeperAnimators[j].enabled = true;
		}
		for (int k = 0; k < this.delveDeeperParticles.Count; k++)
		{
			this.delveDeeperParticles[k].emission.enabled = true;
		}
		GorillaTagger.Instance.StartVibration(false, 0.1f, (float)this.shiftManager.GetDrillingDuration());
		GorillaTagger.Instance.StartVibration(true, 0.1f, (float)this.shiftManager.GetDrillingDuration());
	}

	// Token: 0x06002E6D RID: 11885 RVA: 0x000FD5A8 File Offset: 0x000FB7A8
	public void StopDelveDeeperFX()
	{
		this.delveDeeperAudio.Stop();
		this.delveDeeperNonspatializedAudio.Stop();
		for (int i = 0; i < this.delveDeeperAnimators.Count; i++)
		{
			this.delveDeeperAnimators[i].enabled = false;
		}
		for (int j = 0; j < this.delveDeeperParticles.Count; j++)
		{
			this.delveDeeperParticles[j].emission.enabled = false;
		}
	}

	// Token: 0x04003AFC RID: 15100
	public GhostReactorShiftManager shiftManager;

	// Token: 0x04003AFD RID: 15101
	public GhostReactor reactor;

	// Token: 0x04003AFE RID: 15102
	[SerializeField]
	public TMP_Text jumbotronTitle;

	// Token: 0x04003AFF RID: 15103
	[SerializeField]
	public TMP_Text jumbotronState;

	// Token: 0x04003B00 RID: 15104
	[SerializeField]
	public TMP_Text jumbotronTime;

	// Token: 0x04003B01 RID: 15105
	[SerializeField]
	public TMP_Text jumbotronRequirements;

	// Token: 0x04003B02 RID: 15106
	[SerializeField]
	public TMP_Text jumbotronRewards;

	// Token: 0x04003B03 RID: 15107
	[SerializeField]
	public List<TMP_Text> logoFrames;

	// Token: 0x04003B04 RID: 15108
	[SerializeField]
	private GameObject delveDeeperButton;

	// Token: 0x04003B05 RID: 15109
	[SerializeField]
	private AudioSource delveDeeperAudio;

	// Token: 0x04003B06 RID: 15110
	[SerializeField]
	private AudioSource delveDeeperNonspatializedAudio;

	// Token: 0x04003B07 RID: 15111
	[SerializeField]
	private List<Animation> delveDeeperAnims;

	// Token: 0x04003B08 RID: 15112
	[SerializeField]
	private List<Animator> delveDeeperAnimators;

	// Token: 0x04003B09 RID: 15113
	[SerializeField]
	private List<ParticleSystem> delveDeeperParticles;

	// Token: 0x04003B0A RID: 15114
	private static readonly string[] STATE_NAMES = new string[] { "--", "PREPARING ENTRY", "PREPARING ENTRY", "READY", "ACTIVE", "EVALUATING SHIFT", "PREPARE TO DIVE", "DIVING" };

	// Token: 0x04003B0B RID: 15115
	private StringBuilder cachedStringBuilder = new StringBuilder(256);
}
