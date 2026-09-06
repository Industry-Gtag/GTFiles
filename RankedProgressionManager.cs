using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000943 RID: 2371
public class RankedProgressionManager : MonoBehaviour
{
	// Token: 0x170005B3 RID: 1459
	// (get) Token: 0x06003E1A RID: 15898 RVA: 0x00150376 File Offset: 0x0014E576
	// (set) Token: 0x06003E1B RID: 15899 RVA: 0x0015037E File Offset: 0x0014E57E
	public int MaxRank { get; private set; }

	// Token: 0x170005B4 RID: 1460
	// (get) Token: 0x06003E1C RID: 15900 RVA: 0x00150387 File Offset: 0x0014E587
	// (set) Token: 0x06003E1D RID: 15901 RVA: 0x0015038F File Offset: 0x0014E58F
	public float LowTierThreshold { get; set; }

	// Token: 0x170005B5 RID: 1461
	// (get) Token: 0x06003E1E RID: 15902 RVA: 0x00150398 File Offset: 0x0014E598
	// (set) Token: 0x06003E1F RID: 15903 RVA: 0x001503A0 File Offset: 0x0014E5A0
	public float HighTierThreshold { get; set; }

	// Token: 0x170005B6 RID: 1462
	// (get) Token: 0x06003E20 RID: 15904 RVA: 0x001503A9 File Offset: 0x0014E5A9
	// (set) Token: 0x06003E21 RID: 15905 RVA: 0x00002C2D File Offset: 0x00000E2D
	public List<RankedProgressionManager.RankedProgressionTier> MajorTiers
	{
		get
		{
			return this.majorTiers;
		}
		private set
		{
		}
	}

	// Token: 0x06003E22 RID: 15906 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void DebugSetELO()
	{
	}

	// Token: 0x06003E23 RID: 15907 RVA: 0x00002C2D File Offset: 0x00000E2D
	[ContextMenu("Reset ELO")]
	private void DebugResetELO()
	{
	}

	// Token: 0x06003E24 RID: 15908 RVA: 0x001503B1 File Offset: 0x0014E5B1
	private void Awake()
	{
		if (RankedProgressionManager.Instance)
		{
			GTDev.LogError<string>("Duplicate RankedProgressionManager detected. Destroying self.", base.gameObject, null);
			Object.Destroy(this);
			return;
		}
		RankedProgressionManager.Instance = this;
	}

	// Token: 0x06003E25 RID: 15909 RVA: 0x001503E0 File Offset: 0x0014E5E0
	private void Start()
	{
		if (this.majorTiers.Count < 3)
		{
			GTDev.LogWarning<string>("At least 3 MMR tiers must be defined.", null);
			return;
		}
		GameMode.OnStartGameMode += this.OnJoinedRoom;
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
		float num = 100f;
		int num2 = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			this.majorTiers[i].SetMinThreshold((i == 0) ? 100f : this.majorTiers[i - 1].thresholdMax);
			for (int j = 0; j < this.majorTiers[i].subTiers.Count; j++)
			{
				num2++;
				this.majorTiers[i].subTiers[j].SetMinThreshold(num);
				num = this.majorTiers[i].subTiers[j].thresholdMax;
			}
		}
		this.MaxRank = num2 - 1;
		this.LowTierThreshold = this.majorTiers[0].thresholdMax;
		List<RankedProgressionManager.RankedProgressionTier> list = this.majorTiers;
		this.HighTierThreshold = list[list.Count - 1].GetMinThreshold();
		this.EloScorePC = new RankedMultiplayerStatisticFloat(RankedProgressionManager.RANKED_ELO_PC_KEY, 100f, 100f, 4000f, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.EloScoreQuest = new RankedMultiplayerStatisticFloat(RankedProgressionManager.RANKED_ELO_KEY, 100f, 100f, 4000f, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.NewTierGracePeriodIdxPC = new RankedMultiplayerStatisticInt(RankedProgressionManager.RANKED_PROGRESSION_GRACE_PERIOD_KEY, 0, -1, int.MaxValue, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.NewTierGracePeriodIdxQuest = new RankedMultiplayerStatisticInt(RankedProgressionManager.RANKED_PROGRESSION_GRACE_PERIOD_PC_KEY, 0, -1, int.MaxValue, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
	}

	// Token: 0x06003E26 RID: 15910 RVA: 0x00150592 File Offset: 0x0014E792
	private void OnDestroy()
	{
		GameMode.OnStartGameMode += this.OnJoinedRoom;
		RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoined);
	}

	// Token: 0x06003E27 RID: 15911 RVA: 0x001505C0 File Offset: 0x0014E7C0
	public void RequestUnlockCompetitiveQueue(bool unlock)
	{
		GorillaTagCompetitiveServerApi.Instance.RequestUnlockCompetitiveQueue(unlock, delegate
		{
			this.AcquireLocalPlayerRankInformation();
		});
	}

	// Token: 0x06003E28 RID: 15912 RVA: 0x001505D9 File Offset: 0x0014E7D9
	public IEnumerator LoadStatsWhenReady()
	{
		yield return new WaitUntil(() => NetworkSystem.Instance.LocalPlayer.UserId != null);
		if (this.HasUnlockedCompetitiveQueue())
		{
			this.RequestUnlockCompetitiveQueue(true);
		}
		else
		{
			this.AcquireLocalPlayerRankInformation();
		}
		yield break;
	}

	// Token: 0x06003E29 RID: 15913 RVA: 0x001505E8 File Offset: 0x0014E7E8
	private void OnJoinedRoom(GameModeType newGameModeType)
	{
		if (newGameModeType == GameModeType.InfectionCompetitive)
		{
			this.AcquireRoomRankInformation(false);
		}
	}

	// Token: 0x06003E2A RID: 15914 RVA: 0x001505F6 File Offset: 0x0014E7F6
	private void OnPlayerJoined(NetPlayer player)
	{
		if (GorillaGameManager.instance != null && GorillaGameManager.instance.GameType() == GameModeType.InfectionCompetitive)
		{
			this.AcquireSinglePlayerRankInformation(player);
		}
	}

	// Token: 0x06003E2B RID: 15915 RVA: 0x0015061C File Offset: 0x0014E81C
	private void AcquireLocalPlayerRankInformation()
	{
		List<string> list = new List<string>();
		list.Add(NetworkSystem.Instance.LocalPlayer.UserId);
		GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnLocalPlayerRankedInformationAcquired));
	}

	// Token: 0x06003E2C RID: 15916 RVA: 0x0015065C File Offset: 0x0014E85C
	private void AcquireSinglePlayerRankInformation(NetPlayer player)
	{
		if (player == null)
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add(player.UserId);
		GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnPlayersRankedInformationAcquired));
	}

	// Token: 0x06003E2D RID: 15917 RVA: 0x00150698 File Offset: 0x0014E898
	public void AcquireRoomRankInformation(bool includeLocalPlayer = true)
	{
		List<string> list = new List<string>();
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (includeLocalPlayer || !netPlayer.IsLocal)
			{
				list.Add(netPlayer.UserId);
			}
		}
		if (list.Count > 0)
		{
			GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnPlayersRankedInformationAcquired));
		}
	}

	// Token: 0x06003E2E RID: 15918 RVA: 0x00150720 File Offset: 0x0014E920
	private void OnPlayersRankedInformationAcquired(GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData)
	{
		foreach (GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData rankedModePlayerProgressionData in rankedModeProgressionData.playerData)
		{
			if (rankedModePlayerProgressionData != null && rankedModePlayerProgressionData.platformData != null && rankedModePlayerProgressionData.platformData.Length >= 2)
			{
				int num = -1;
				foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
				{
					if (netPlayer.UserId == rankedModePlayerProgressionData.playfabID)
					{
						num = netPlayer.ActorNumber;
						break;
					}
				}
				if (num >= 0)
				{
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = rankedModePlayerProgressionData.platformData[1];
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData2 = rankedModePlayerProgressionData.platformData[0];
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData3 = rankedModeProgressionPlatformData2;
					int rankFromTiers = RankedProgressionManager.Instance.GetRankFromTiers(rankedModeProgressionPlatformData3.majorTier, rankedModeProgressionPlatformData3.minorTier);
					Action<int, float, int> onPlayerEloAcquired = this.OnPlayerEloAcquired;
					if (onPlayerEloAcquired != null)
					{
						onPlayerEloAcquired(num, rankedModeProgressionPlatformData3.elo, rankFromTiers);
					}
					if (num == NetworkSystem.Instance.LocalPlayerID)
					{
						this.SetLocalProgressionData(rankedModePlayerProgressionData);
					}
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(num, out rigContainer))
					{
						VRRig rig = rigContainer.Rig;
						if (rig != null)
						{
							int rankFromTiers2 = this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
							int rankFromTiers3 = RankedProgressionManager.Instance.GetRankFromTiers(rankedModeProgressionPlatformData2.majorTier, rankedModeProgressionPlatformData2.minorTier);
							rig.SetRankedInfo(rankedModeProgressionPlatformData3.elo, rankFromTiers2, rankFromTiers3, false);
						}
					}
				}
			}
		}
	}

	// Token: 0x06003E2F RID: 15919 RVA: 0x001508B0 File Offset: 0x0014EAB0
	private void OnLocalPlayerRankedInformationAcquired(GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData)
	{
		if (rankedModeProgressionData.playerData.Count > 0)
		{
			this.SetLocalProgressionData(rankedModeProgressionData.playerData[0]);
			float eloScore = this.GetEloScore();
			int progressionRankIndexQuest = this.GetProgressionRankIndexQuest();
			int progressionRankIndexPC = this.GetProgressionRankIndexPC();
			int num = progressionRankIndexPC;
			this.HandlePlayerRankedInfoReceived(NetworkSystem.Instance.LocalPlayer.ActorNumber, eloScore, num);
			VRRig.LocalRig.SetRankedInfo(eloScore, progressionRankIndexQuest, progressionRankIndexPC, true);
		}
	}

	// Token: 0x06003E30 RID: 15920 RVA: 0x0015091B File Offset: 0x0014EB1B
	public bool AreValuesValid(float elo, int questTier, int pcTier)
	{
		return elo >= 100f && elo <= 4000f && questTier >= 0 && questTier <= this.MaxRank && pcTier >= 0 && pcTier <= this.MaxRank;
	}

	// Token: 0x06003E31 RID: 15921 RVA: 0x0015094A File Offset: 0x0014EB4A
	public void HandlePlayerRankedInfoReceived(int actorNum, float elo, int tier)
	{
		Action<int, float, int> onPlayerEloAcquired = this.OnPlayerEloAcquired;
		if (onPlayerEloAcquired == null)
		{
			return;
		}
		onPlayerEloAcquired(actorNum, elo, tier);
	}

	// Token: 0x06003E32 RID: 15922 RVA: 0x0015095F File Offset: 0x0014EB5F
	public void SetLocalProgressionData(GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData data)
	{
		this.ProgressionData = data;
	}

	// Token: 0x06003E33 RID: 15923 RVA: 0x00150968 File Offset: 0x0014EB68
	public void LoadStats()
	{
		base.StartCoroutine(this.LoadStatsWhenReady());
	}

	// Token: 0x06003E34 RID: 15924 RVA: 0x00150977 File Offset: 0x0014EB77
	public float GetEloScore()
	{
		return this.GetEloScorePC();
	}

	// Token: 0x06003E35 RID: 15925 RVA: 0x0015097F File Offset: 0x0014EB7F
	public void SetEloScore(float val)
	{
		GorillaTagCompetitiveServerApi.Instance.RequestSetEloValue(val, delegate
		{
			this.AcquireLocalPlayerRankInformation();
		});
	}

	// Token: 0x06003E36 RID: 15926 RVA: 0x00150998 File Offset: 0x0014EB98
	public float GetEloScorePC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 100f;
		}
		return this.ProgressionData.platformData[0].elo;
	}

	// Token: 0x06003E37 RID: 15927 RVA: 0x001509D7 File Offset: 0x0014EBD7
	public float GetEloScoreQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 100f;
		}
		return this.ProgressionData.platformData[1].elo;
	}

	// Token: 0x06003E38 RID: 15928 RVA: 0x00150A16 File Offset: 0x0014EC16
	private int GetNewTierGracePeriodIdx()
	{
		return this.NewTierGracePeriodIdxPC;
	}

	// Token: 0x06003E39 RID: 15929 RVA: 0x00150A23 File Offset: 0x0014EC23
	private void SetNewTierGracePeriodIdx(int val)
	{
		this.NewTierGracePeriodIdxPC.Set(val);
	}

	// Token: 0x06003E3A RID: 15930 RVA: 0x00150A31 File Offset: 0x0014EC31
	private void IncrementNewTierGracePeriodIdx()
	{
		this.NewTierGracePeriodIdxPC.Increment();
	}

	// Token: 0x06003E3B RID: 15931 RVA: 0x00150A3E File Offset: 0x0014EC3E
	public bool TryGetProgressionSubTier(out RankedProgressionManager.RankedProgressionSubTier subTier, out int index)
	{
		subTier = null;
		index = -1;
		return this.TryGetProgressionSubTier(this.GetEloScore(), out subTier, out index);
	}

	// Token: 0x06003E3C RID: 15932 RVA: 0x00150A54 File Offset: 0x0014EC54
	public bool TryGetProgressionSubTier(float elo, out RankedProgressionManager.RankedProgressionSubTier subTier, out int index)
	{
		int num = 0;
		subTier = null;
		index = -1;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			float num2 = ((i < this.majorTiers.Count - 1) ? this.majorTiers[i].thresholdMax : 4000.1f);
			if (elo < num2)
			{
				int j = 0;
				while (j < this.majorTiers[i].subTiers.Count)
				{
					float num3 = ((j < this.majorTiers[i].subTiers.Count - 1) ? this.majorTiers[i].subTiers[j].thresholdMax : num2);
					if (elo < num3)
					{
						subTier = this.majorTiers[i].subTiers[j];
						index = num;
						return true;
					}
					j++;
					num++;
				}
			}
			else
			{
				num += this.majorTiers[i].subTiers.Count;
			}
		}
		return false;
	}

	// Token: 0x06003E3D RID: 15933 RVA: 0x00150B58 File Offset: 0x0014ED58
	private RankedProgressionManager.RankedProgressionTier GetProgressionMajorTierBySubTierIndex(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == idx)
				{
					return this.majorTiers[i];
				}
				j++;
				num++;
			}
		}
		return null;
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x00150BB4 File Offset: 0x0014EDB4
	private RankedProgressionManager.RankedProgressionSubTier GetProgressionSubTierByIndex(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == idx)
				{
					return this.majorTiers[i].subTiers[j];
				}
				j++;
				num++;
			}
		}
		return null;
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x00150C1C File Offset: 0x0014EE1C
	private RankedProgressionManager.RankedProgressionSubTier GetNextProgressionSubTierByIndex(int idx)
	{
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(idx + 1);
		if (progressionSubTierByIndex != null)
		{
			return progressionSubTierByIndex;
		}
		return this.GetProgressionSubTierByIndex(idx);
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x00150C40 File Offset: 0x0014EE40
	private RankedProgressionManager.RankedProgressionSubTier GetPrevProgressionSubTierByIndex(int idx)
	{
		if (idx > 0)
		{
			RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(idx - 1);
			if (progressionSubTierByIndex != null)
			{
				return progressionSubTierByIndex;
			}
		}
		return this.GetProgressionSubTierByIndex(idx);
	}

	// Token: 0x06003E41 RID: 15937 RVA: 0x00150C67 File Offset: 0x0014EE67
	public string GetProgressionRankName()
	{
		return this.GetProgressionRankName(this.GetEloScore());
	}

	// Token: 0x06003E42 RID: 15938 RVA: 0x00150C78 File Offset: 0x0014EE78
	public string GetProgressionRankName(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return rankedProgressionSubTier.name;
		}
		return string.Empty;
	}

	// Token: 0x06003E43 RID: 15939 RVA: 0x00150CA0 File Offset: 0x0014EEA0
	public string GetNextProgressionRankName(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier nextProgressionSubTierByIndex = this.GetNextProgressionSubTierByIndex(subTierIdx);
		if (nextProgressionSubTierByIndex != null)
		{
			return nextProgressionSubTierByIndex.name;
		}
		return null;
	}

	// Token: 0x06003E44 RID: 15940 RVA: 0x00150CC0 File Offset: 0x0014EEC0
	public string GetPrevProgressionRankName(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier prevProgressionSubTierByIndex = this.GetPrevProgressionSubTierByIndex(subTierIdx);
		if (prevProgressionSubTierByIndex != null)
		{
			return prevProgressionSubTierByIndex.name;
		}
		return null;
	}

	// Token: 0x06003E45 RID: 15941 RVA: 0x00150CE0 File Offset: 0x0014EEE0
	public int GetProgressionRankIndex()
	{
		return this.GetProgressionRankIndexPC();
	}

	// Token: 0x06003E46 RID: 15942 RVA: 0x00150CE8 File Offset: 0x0014EEE8
	public RankedProgressionManager.RankedProgressionSubTier GetProgressionSubTier()
	{
		return this.GetProgressionSubTierByIndex(this.GetProgressionRankIndex());
	}

	// Token: 0x06003E47 RID: 15943 RVA: 0x00150CF8 File Offset: 0x0014EEF8
	public int GetProgressionRankIndexQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0;
		}
		GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = this.ProgressionData.platformData[1];
		return this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
	}

	// Token: 0x06003E48 RID: 15944 RVA: 0x00150D4C File Offset: 0x0014EF4C
	public int GetProgressionRankIndexPC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0;
		}
		GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = this.ProgressionData.platformData[0];
		return this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
	}

	// Token: 0x06003E49 RID: 15945 RVA: 0x00150DA0 File Offset: 0x0014EFA0
	public int GetRankFromTiers(int majorTier, int minorTier)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			for (int j = 0; j < this.majorTiers[i].subTiers.Count; j++)
			{
				if (i == majorTier && j == minorTier)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x06003E4A RID: 15946 RVA: 0x00150DF8 File Offset: 0x0014EFF8
	public int GetProgressionRankIndex(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return num;
		}
		return -1;
	}

	// Token: 0x06003E4B RID: 15947 RVA: 0x00150E15 File Offset: 0x0014F015
	public float GetProgressionRankProgress()
	{
		return this.GetProgressionRankProgressPC();
	}

	// Token: 0x06003E4C RID: 15948 RVA: 0x00150E1D File Offset: 0x0014F01D
	public float GetProgressionRankProgressQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0f;
		}
		return this.ProgressionData.platformData[1].rankProgress;
	}

	// Token: 0x06003E4D RID: 15949 RVA: 0x00150E5C File Offset: 0x0014F05C
	public float GetProgressionRankProgressPC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0f;
		}
		return this.ProgressionData.platformData[0].rankProgress;
	}

	// Token: 0x06003E4E RID: 15950 RVA: 0x00150E9C File Offset: 0x0014F09C
	public int ClampProgressionRankIndex(int subTierIdx)
	{
		if (subTierIdx < 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == subTierIdx)
				{
					return subTierIdx;
				}
				j++;
				num++;
			}
		}
		return num - 1;
	}

	// Token: 0x06003E4F RID: 15951 RVA: 0x00150EF8 File Offset: 0x0014F0F8
	public Sprite GetProgressionRankIcon()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return null;
		}
		int num = ((this.ProgressionData == null) ? 0 : this.ProgressionData.platformData[0].minorTier);
		int num2 = ((this.ProgressionData == null) ? 0 : this.ProgressionData.platformData[0].majorTier);
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = this.majorTiers[num2].subTiers[num];
		if (rankedProgressionSubTier == null)
		{
			return null;
		}
		return rankedProgressionSubTier.icon;
	}

	// Token: 0x06003E50 RID: 15952 RVA: 0x00150F90 File Offset: 0x0014F190
	public string GetRankedProgressionTierName()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return "None";
		}
		int minorTier = this.ProgressionData.platformData[0].minorTier;
		int majorTier = this.ProgressionData.platformData[0].majorTier;
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = this.majorTiers[majorTier].subTiers[minorTier];
		if (rankedProgressionSubTier != null)
		{
			return rankedProgressionSubTier.name;
		}
		return "None";
	}

	// Token: 0x06003E51 RID: 15953 RVA: 0x0015101C File Offset: 0x0014F21C
	public Sprite GetProgressionRankIcon(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return rankedProgressionSubTier.icon;
		}
		return null;
	}

	// Token: 0x06003E52 RID: 15954 RVA: 0x00151040 File Offset: 0x0014F240
	public Sprite GetProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(subTierIdx);
		if (progressionSubTierByIndex != null)
		{
			return progressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x06003E53 RID: 15955 RVA: 0x00151060 File Offset: 0x0014F260
	public Sprite GetNextProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier nextProgressionSubTierByIndex = this.GetNextProgressionSubTierByIndex(subTierIdx);
		if (nextProgressionSubTierByIndex != null)
		{
			return nextProgressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x06003E54 RID: 15956 RVA: 0x00151080 File Offset: 0x0014F280
	public Sprite GetPrevProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier prevProgressionSubTierByIndex = this.GetPrevProgressionSubTierByIndex(subTierIdx);
		if (prevProgressionSubTierByIndex != null)
		{
			return prevProgressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x06003E55 RID: 15957 RVA: 0x001510A0 File Offset: 0x0014F2A0
	public float GetCurrentELO()
	{
		return this.GetEloScore();
	}

	// Token: 0x06003E56 RID: 15958 RVA: 0x001510A8 File Offset: 0x0014F2A8
	public void GetSubtierRankThresholds(int subTierIdx, out float minThreshold, out float maxThreshold)
	{
		minThreshold = 0f;
		maxThreshold = 1f;
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(subTierIdx);
		if (progressionSubTierByIndex != null)
		{
			maxThreshold = progressionSubTierByIndex.thresholdMax;
			if (maxThreshold <= 0f)
			{
				RankedProgressionManager.RankedProgressionTier progressionMajorTierBySubTierIndex = this.GetProgressionMajorTierBySubTierIndex(subTierIdx);
				if (progressionMajorTierBySubTierIndex != null)
				{
					maxThreshold = progressionMajorTierBySubTierIndex.thresholdMax;
					if (maxThreshold <= 0f)
					{
						maxThreshold = 4000f;
					}
				}
			}
			minThreshold = progressionSubTierByIndex.GetMinThreshold();
			if (minThreshold <= 0f)
			{
				RankedProgressionManager.RankedProgressionTier progressionMajorTierBySubTierIndex2 = this.GetProgressionMajorTierBySubTierIndex(subTierIdx);
				if (progressionMajorTierBySubTierIndex2 != null)
				{
					minThreshold = progressionMajorTierBySubTierIndex2.GetMinThreshold();
					if (minThreshold <= 0f)
					{
						minThreshold = 100f;
					}
				}
			}
		}
	}

	// Token: 0x06003E57 RID: 15959 RVA: 0x00151136 File Offset: 0x0014F336
	public static float GetEloWinProbability(float ratingPlayer1, float ratingPlayer2)
	{
		return 1f / (1f + Mathf.Pow(10f, (ratingPlayer1 - ratingPlayer2) / 400f));
	}

	// Token: 0x06003E58 RID: 15960 RVA: 0x00151157 File Offset: 0x0014F357
	public static float UpdateEloScore(float eloScore, float expectedResult, float actualResult, float k)
	{
		return Mathf.Clamp(eloScore + k * (actualResult - expectedResult), 100f, 4000f);
	}

	// Token: 0x06003E59 RID: 15961 RVA: 0x0015116F File Offset: 0x0014F36F
	public RankedProgressionManager.ERankedMatchmakingTier GetRankedMatchmakingTier()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return RankedProgressionManager.ERankedMatchmakingTier.Low;
		}
		return (RankedProgressionManager.ERankedMatchmakingTier)this.ProgressionData.platformData[0].majorTier;
	}

	// Token: 0x170005B7 RID: 1463
	// (get) Token: 0x06003E5A RID: 15962 RVA: 0x001511AA File Offset: 0x0014F3AA
	public float CompetitiveQueueEloFloor
	{
		get
		{
			return this.LowTierThreshold;
		}
	}

	// Token: 0x06003E5B RID: 15963 RVA: 0x001511B2 File Offset: 0x0014F3B2
	private bool HasUnlockedCompetitiveQueue()
	{
		return GorillaComputer.instance.allowedInCompetitive;
	}

	// Token: 0x04004EB5 RID: 20149
	public static RankedProgressionManager Instance;

	// Token: 0x04004EB6 RID: 20150
	public const float DEFAULT_ELO = 100f;

	// Token: 0x04004EB7 RID: 20151
	public const float MIN_ELO = 100f;

	// Token: 0x04004EB8 RID: 20152
	public const float MAX_ELO = 4000f;

	// Token: 0x04004EB9 RID: 20153
	public const float MAJOR_TIER_MIN_RANGE = 200f;

	// Token: 0x04004EBA RID: 20154
	public const float SUB_TIER_MIN_RANGE = 20f;

	// Token: 0x04004EBB RID: 20155
	public static string RANKED_ELO_KEY = "RankedElo";

	// Token: 0x04004EBC RID: 20156
	public static string RANKED_PROGRESSION_GRACE_PERIOD_KEY = "RankedProgGracePeriod";

	// Token: 0x04004EBD RID: 20157
	public static string RANKED_ELO_PC_KEY = "RankedEloPC";

	// Token: 0x04004EBE RID: 20158
	public static string RANKED_PROGRESSION_GRACE_PERIOD_PC_KEY = "RankedProgGracePeriodPC";

	// Token: 0x04004EBF RID: 20159
	private RankedMultiplayerStatisticFloat EloScorePC;

	// Token: 0x04004EC0 RID: 20160
	private RankedMultiplayerStatisticFloat EloScoreQuest;

	// Token: 0x04004EC1 RID: 20161
	private RankedMultiplayerStatisticInt NewTierGracePeriodIdxPC;

	// Token: 0x04004EC2 RID: 20162
	private RankedMultiplayerStatisticInt NewTierGracePeriodIdxQuest;

	// Token: 0x04004EC3 RID: 20163
	private GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData ProgressionData;

	// Token: 0x04004EC4 RID: 20164
	[SerializeField]
	private List<RankedProgressionManager.RankedProgressionTier> majorTiers = new List<RankedProgressionManager.RankedProgressionTier>();

	// Token: 0x04004EC5 RID: 20165
	[SerializeField]
	private int newTierGracePeriod = 3;

	// Token: 0x04004EC6 RID: 20166
	public float MaxEloConstant = 90f;

	// Token: 0x04004EC8 RID: 20168
	private RankedProgressionManager.RankedProgressionEvent ProgressionEvent;

	// Token: 0x04004EC9 RID: 20169
	public Action<int, float, int> OnPlayerEloAcquired;

	// Token: 0x04004ECC RID: 20172
	[Space]
	[ContextMenuItem("Set ELO", "DebugSetELO")]
	public int debugEloPoints = 100;

	// Token: 0x02000944 RID: 2372
	public enum ERankedMatchmakingTier
	{
		// Token: 0x04004ECE RID: 20174
		Low,
		// Token: 0x04004ECF RID: 20175
		Medium,
		// Token: 0x04004ED0 RID: 20176
		High
	}

	// Token: 0x02000945 RID: 2373
	public enum ERankedProgressionEventType
	{
		// Token: 0x04004ED2 RID: 20178
		None,
		// Token: 0x04004ED3 RID: 20179
		Progress,
		// Token: 0x04004ED4 RID: 20180
		Promotion,
		// Token: 0x04004ED5 RID: 20181
		Relegation
	}

	// Token: 0x02000946 RID: 2374
	public class RankedProgressionEvent
	{
		// Token: 0x06003E60 RID: 15968 RVA: 0x00151220 File Offset: 0x0014F420
		public override string ToString()
		{
			string text = "Progression Info\n";
			text += string.Format("Event Type: {0}\n", this.evtType.ToString());
			text += string.Format("Left Tier: {0}\n", this.leftName);
			text += string.Format("Right Tier: {0}\n", this.rightName);
			text += string.Format("Left Value: {0}\n", this.minVal.ToString("N0"));
			text += string.Format("Right Value: {0}\n", this.maxVal.ToString("N0"));
			text += string.Format("Elo Delta: {0}\n", this.delta.ToString("N0"));
			if (this.evtType == RankedProgressionManager.ERankedProgressionEventType.Promotion || this.evtType == RankedProgressionManager.ERankedProgressionEventType.Relegation)
			{
				text += string.Format("Fanfare Tier: {0}\n", this.newTierName);
			}
			return text;
		}

		// Token: 0x04004ED6 RID: 20182
		public RankedProgressionManager.ERankedProgressionEventType evtType;

		// Token: 0x04004ED7 RID: 20183
		public Sprite progressIconLeft;

		// Token: 0x04004ED8 RID: 20184
		public Sprite progressIconRight;

		// Token: 0x04004ED9 RID: 20185
		public Sprite newTierIcon;

		// Token: 0x04004EDA RID: 20186
		public string leftName;

		// Token: 0x04004EDB RID: 20187
		public string rightName;

		// Token: 0x04004EDC RID: 20188
		public string newTierName;

		// Token: 0x04004EDD RID: 20189
		public float minVal;

		// Token: 0x04004EDE RID: 20190
		public float maxVal;

		// Token: 0x04004EDF RID: 20191
		public float delta;
	}

	// Token: 0x02000947 RID: 2375
	public abstract class RankedProgressionTierBase
	{
		// Token: 0x06003E62 RID: 15970 RVA: 0x00151310 File Offset: 0x0014F510
		public void SetMinThreshold(float val)
		{
			this.thresholdMin = val;
		}

		// Token: 0x06003E63 RID: 15971 RVA: 0x00151319 File Offset: 0x0014F519
		public float GetMinThreshold()
		{
			if (this.thresholdMin < 0f)
			{
				GTDev.LogError<string>("Tier min threshold not initialized. Can only be used at runtime.", null);
			}
			return this.thresholdMin;
		}

		// Token: 0x04004EE0 RID: 20192
		public string name;

		// Token: 0x04004EE1 RID: 20193
		public Color color = Color.white;

		// Token: 0x04004EE2 RID: 20194
		public float thresholdMax;

		// Token: 0x04004EE3 RID: 20195
		private float thresholdMin = -1f;
	}

	// Token: 0x02000948 RID: 2376
	[Serializable]
	public class RankedProgressionSubTier : RankedProgressionManager.RankedProgressionTierBase
	{
		// Token: 0x04004EE4 RID: 20196
		public Sprite icon;
	}

	// Token: 0x02000949 RID: 2377
	[Serializable]
	public class RankedProgressionTier : RankedProgressionManager.RankedProgressionTierBase
	{
		// Token: 0x06003E66 RID: 15974 RVA: 0x00151360 File Offset: 0x0014F560
		public void InsertSubTierAt(int idx, float tierMin)
		{
			RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = new RankedProgressionManager.RankedProgressionSubTier
			{
				name = "NewTier"
			};
			this.subTiers.Insert(idx, rankedProgressionSubTier);
			this.EnforceSubTierValidity(tierMin);
		}

		// Token: 0x06003E67 RID: 15975 RVA: 0x00151394 File Offset: 0x0014F594
		public void EnforceSubTierValidity(float thresholdMin)
		{
			float num = (((this.thresholdMax == 0f) ? 4000f : this.thresholdMax) - thresholdMin) / (float)this.subTiers.Count;
			for (int i = 0; i < this.subTiers.Count - 1; i++)
			{
				float num2 = thresholdMin + (float)(i + 1) * num;
				num2 = Mathf.Round(num2 / 10f);
				this.subTiers[i].thresholdMax = num2 * 10f;
			}
		}

		// Token: 0x04004EE5 RID: 20197
		public List<RankedProgressionManager.RankedProgressionSubTier> subTiers = new List<RankedProgressionManager.RankedProgressionSubTier>();
	}
}
