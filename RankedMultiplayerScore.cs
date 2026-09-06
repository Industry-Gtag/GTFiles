using System;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000938 RID: 2360
public class RankedMultiplayerScore : MonoBehaviourTick
{
	// Token: 0x170005AE RID: 1454
	// (get) Token: 0x06003DCC RID: 15820 RVA: 0x0014F17B File Offset: 0x0014D37B
	// (set) Token: 0x06003DCD RID: 15821 RVA: 0x0014F183 File Offset: 0x0014D383
	public RankedProgressionManager Progression { get; private set; }

	// Token: 0x06003DCE RID: 15822 RVA: 0x0014F18C File Offset: 0x0014D38C
	public void Initialize()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.OnStateChanged;
		GorillaTagCompetitiveManager.onRoundStart += this.OnGameStarted;
		GorillaTagCompetitiveManager.onRoundEnd += this.OnGameEnded;
		GorillaTagCompetitiveManager.onPlayerJoined += this.OnPlayerJoined;
		GorillaTagCompetitiveManager.onPlayerLeft += this.OnPlayerLeft;
		GorillaTagCompetitiveManager.onTagOccurred += this.OnTagReported;
		GorillaGameManager instance = GorillaGameManager.instance;
		if (instance != null)
		{
			this.CompetitiveManager = instance as GorillaTagCompetitiveManager;
		}
		this.Progression = RankedProgressionManager.Instance;
		RankedProgressionManager progression = this.Progression;
		progression.OnPlayerEloAcquired = (Action<int, float, int>)Delegate.Combine(progression.OnPlayerEloAcquired, new Action<int, float, int>(this.HandlePlayerEloAcquired));
	}

	// Token: 0x06003DCF RID: 15823 RVA: 0x0014F24C File Offset: 0x0014D44C
	private void HandlePlayerEloAcquired(int playerId, float elo, int tier)
	{
		this.CachePlayerRankedProgressionData(playerId, tier, elo);
	}

	// Token: 0x06003DD0 RID: 15824 RVA: 0x0014F257 File Offset: 0x0014D457
	private void OnDestroy()
	{
		this.Unsubscribe();
	}

	// Token: 0x06003DD1 RID: 15825 RVA: 0x0014F260 File Offset: 0x0014D460
	public void Unsubscribe()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.OnStateChanged;
		GorillaTagCompetitiveManager.onRoundStart -= this.OnGameStarted;
		GorillaTagCompetitiveManager.onRoundEnd -= this.OnGameEnded;
		GorillaTagCompetitiveManager.onPlayerJoined -= this.OnPlayerJoined;
		GorillaTagCompetitiveManager.onPlayerLeft -= this.OnPlayerLeft;
		GorillaTagCompetitiveManager.onTagOccurred -= this.OnTagReported;
		if (this.Progression != null)
		{
			RankedProgressionManager progression = this.Progression;
			progression.OnPlayerEloAcquired = (Action<int, float, int>)Delegate.Remove(progression.OnPlayerEloAcquired, new Action<int, float, int>(this.HandlePlayerEloAcquired));
		}
	}

	// Token: 0x06003DD2 RID: 15826 RVA: 0x0014F308 File Offset: 0x0014D508
	public override void Tick()
	{
		if (this.PerSecondTimer > 0f && Time.time >= this.PerSecondTimer + 1f)
		{
			if (this.CompetitiveManager == null)
			{
				return;
			}
			this.OnPerSecondTimerElapsed(NetworkSystem.Instance.AllNetPlayers.Length, this.CompetitiveManager.currentInfected.Count);
			this.PerSecondTimer = Time.time;
		}
	}

	// Token: 0x06003DD3 RID: 15827 RVA: 0x0014F374 File Offset: 0x0014D574
	private void OnPerSecondTimerElapsed(int playersInGame, int infectedPlayers)
	{
		foreach (int num in this.AllPlayerInRoundScores.Keys.ToList<int>())
		{
			RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = this.AllPlayerInRoundScores[num];
			playerScoreInRound.Infected = this.CompetitiveManager.IsInfected(NetworkSystem.Instance.GetPlayer(num));
			if (!playerScoreInRound.Infected)
			{
				float num2 = (float)infectedPlayers / (float)playersInGame;
				playerScoreInRound.PointsOnDefense += Mathf.Lerp(this.PointsPerUninfectedSecMin, this.PointsPerUninfectedSecMax, num2);
			}
			this.AllPlayerInRoundScores[num] = playerScoreInRound;
		}
	}

	// Token: 0x06003DD4 RID: 15828 RVA: 0x0014F42C File Offset: 0x0014D62C
	public void ResetMatch()
	{
		this.AllFinalPlayerScores.Clear();
		this.AllPlayerInRoundScores.Clear();
	}

	// Token: 0x06003DD5 RID: 15829 RVA: 0x0014F444 File Offset: 0x0014D644
	private void OnStateChanged(GorillaTagCompetitiveManager.GameState state)
	{
		if (state == GorillaTagCompetitiveManager.GameState.StartingCountdown)
		{
			this.OnGameStarted();
			this.Progression.AcquireRoomRankInformation(true);
		}
	}

	// Token: 0x06003DD6 RID: 15830 RVA: 0x0014F45C File Offset: 0x0014D65C
	public void OnGameStarted()
	{
		this.PerSecondTimer = Time.time;
		if (!this.IsLateJoiner)
		{
			this.ResetMatch();
			for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
			{
				this.StartTrackingPlayer(NetworkSystem.Instance.AllNetPlayers[i], false);
			}
		}
	}

	// Token: 0x06003DD7 RID: 15831 RVA: 0x0014F4AC File Offset: 0x0014D6AC
	public void OnGameEnded()
	{
		foreach (int num in this.AllPlayerInRoundScores.Keys.ToList<int>())
		{
			RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = this.AllPlayerInRoundScores[num];
			if (!playerScoreInRound.Infected)
			{
				playerScoreInRound.TaggedTime = Time.time;
			}
			this.AllPlayerInRoundScores[num] = playerScoreInRound;
		}
		this.PerSecondTimer = -1f;
		this.ReportScore();
		this.WasInfectedInitially = false;
		this.IsLateJoiner = false;
	}

	// Token: 0x06003DD8 RID: 15832 RVA: 0x0014F550 File Offset: 0x0014D750
	private void OnPlayerJoined(NetPlayer player)
	{
		if (NetworkSystem.Instance.IsMasterClient && this.CompetitiveManager.IsMatchActive())
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<float> list3 = new List<float>();
			List<float> list4 = new List<float>();
			List<bool> list5 = new List<bool>();
			List<float> list6 = new List<float>();
			foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
			{
				list.Add(keyValuePair.Value.PlayerId);
				list2.Add(keyValuePair.Value.NumTags);
				list3.Add(keyValuePair.Value.PointsOnDefense);
				list4.Add(Time.time - keyValuePair.Value.JoinTime);
				list5.Add(keyValuePair.Value.Infected);
				if (!keyValuePair.Value.Infected)
				{
					list6.Add(0f);
				}
				else
				{
					list6.Add(Time.time - keyValuePair.Value.TaggedTime);
				}
			}
			GameMode.ActiveNetworkHandler.SendRPC("SendScoresToLateJoinerRPC", player, new object[]
			{
				list.ToArray(),
				list2.ToArray(),
				list3.ToArray(),
				list4.ToArray(),
				list5.ToArray(),
				list6.ToArray()
			});
		}
		this.StartTrackingPlayer(player, true);
	}

	// Token: 0x06003DD9 RID: 15833 RVA: 0x0014F6D8 File Offset: 0x0014D8D8
	public void ReceivedScoresForLateJoiner(int[] playerIds, int[] numTags, float[] pointsOnDefense, float[] joinTime, bool[] infected, float[] taggedTime)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			this.IsLateJoiner = true;
			for (int i = 0; i < playerIds.Length; i++)
			{
				int num = playerIds[i];
				RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = new RankedMultiplayerScore.PlayerScoreInRound(num, infected[i]);
				playerScoreInRound.NumTags = numTags[i];
				playerScoreInRound.PointsOnDefense = pointsOnDefense[i];
				playerScoreInRound.JoinTime = Time.time - joinTime[i];
				if (!infected[i])
				{
					playerScoreInRound.TaggedTime = 0f;
				}
				else
				{
					playerScoreInRound.TaggedTime = Time.time - taggedTime[i];
				}
				this.AllPlayerInRoundScores.TryAdd(num, playerScoreInRound);
			}
		}
	}

	// Token: 0x06003DDA RID: 15834 RVA: 0x0014F76E File Offset: 0x0014D96E
	private void OnPlayerLeft(NetPlayer player)
	{
		this.AllPlayerInRoundScores.Remove(player.ActorNumber);
	}

	// Token: 0x06003DDB RID: 15835 RVA: 0x0014F784 File Offset: 0x0014D984
	private void StartTrackingPlayer(NetPlayer player, bool lateJoin)
	{
		bool flag = lateJoin;
		if (!lateJoin && this.CompetitiveManager != null)
		{
			flag = this.CompetitiveManager.IsInfected(player);
			if (player.ActorNumber == NetworkSystem.Instance.LocalPlayerID)
			{
				this.WasInfectedInitially = true;
			}
		}
		if (player == NetworkSystem.Instance.LocalPlayer)
		{
			this.CachePlayerRankedProgressionData(player.ActorNumber, this.Progression.GetProgressionRankIndex(), this.Progression.GetEloScore());
		}
		this.AllPlayerInRoundScores.TryAdd(player.ActorNumber, new RankedMultiplayerScore.PlayerScoreInRound(player.ActorNumber, flag));
	}

	// Token: 0x06003DDC RID: 15836 RVA: 0x0014F818 File Offset: 0x0014DA18
	public RankedMultiplayerScore.PlayerScoreInRound GetInGameScoreForSelf()
	{
		RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound;
		if (this.AllPlayerInRoundScores.TryGetValue(NetworkSystem.Instance.LocalPlayerID, out playerScoreInRound))
		{
			return playerScoreInRound;
		}
		return default(RankedMultiplayerScore.PlayerScoreInRound);
	}

	// Token: 0x06003DDD RID: 15837 RVA: 0x0014F84C File Offset: 0x0014DA4C
	public void OnTagReported(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound;
		if (this.AllPlayerInRoundScores.TryGetValue(taggingPlayer.ActorNumber, out playerScoreInRound))
		{
			playerScoreInRound.NumTags++;
			this.AllPlayerInRoundScores[taggingPlayer.ActorNumber] = playerScoreInRound;
		}
		RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound2;
		if (this.AllPlayerInRoundScores.TryGetValue(taggedPlayer.ActorNumber, out playerScoreInRound2))
		{
			playerScoreInRound2.Infected = true;
			playerScoreInRound2.TaggedTime = Time.time;
			this.AllPlayerInRoundScores[taggedPlayer.ActorNumber] = playerScoreInRound2;
		}
	}

	// Token: 0x06003DDE RID: 15838 RVA: 0x0014F8C8 File Offset: 0x0014DAC8
	private void ReportScore()
	{
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
		{
			foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
			{
				this.AllFinalPlayerScores.Add(new RankedMultiplayerScore.PlayerScore
				{
					PlayerId = keyValuePair.Key,
					GameScore = this.ComputeGameScore(keyValuePair.Value.NumTags, keyValuePair.Value.PointsOnDefense),
					EloScore = (this.PlayerRankedElos.ContainsKey(keyValuePair.Key) ? this.PlayerRankedElos[keyValuePair.Key] : 0f),
					NumTags = keyValuePair.Value.NumTags,
					TimeUntagged = keyValuePair.Value.TaggedTime - keyValuePair.Value.JoinTime,
					PointsOnDefense = keyValuePair.Value.PointsOnDefense
				});
			}
			GorillaTagCompetitiveServerApi.Instance.RequestSubmitMatchScores((string)obj, this.AllFinalPlayerScores);
		}
		this.PredictPlayerEloChanges();
	}

	// Token: 0x06003DDF RID: 15839 RVA: 0x0014FA14 File Offset: 0x0014DC14
	public float ComputeGameScore(int tags, float pointsOnDefense)
	{
		return (float)(tags * this.PointsPerTag) + pointsOnDefense;
	}

	// Token: 0x06003DE0 RID: 15840 RVA: 0x0014FA24 File Offset: 0x0014DC24
	private void PredictPlayerEloChanges()
	{
		this.VisitedScoreCombintations.Clear();
		this.AllFinalPlayerScores = this.AllFinalPlayerScores.OrderByDescending((RankedMultiplayerScore.PlayerScore s) => s.GameScore).ToList<RankedMultiplayerScore.PlayerScore>();
		float num = this.Progression.MaxEloConstant / (float)(this.AllFinalPlayerScores.Count - 1);
		this.InProgressEloDeltaPerPlayer.Clear();
		for (int i = 0; i < this.AllFinalPlayerScores.Count; i++)
		{
			this.InProgressEloDeltaPerPlayer.Add(this.AllFinalPlayerScores[i].PlayerId, 0f);
		}
		for (int j = 0; j < this.AllFinalPlayerScores.Count; j++)
		{
			for (int k = 0; k < this.AllFinalPlayerScores.Count; k++)
			{
				if (j != k)
				{
					bool flag = this.AllFinalPlayerScores[j].GameScore.Approx(this.AllFinalPlayerScores[k].GameScore, 1E-06f);
					float eloWinProbability = RankedProgressionManager.GetEloWinProbability(this.AllFinalPlayerScores[k].EloScore, this.AllFinalPlayerScores[j].EloScore);
					float eloWinProbability2 = RankedProgressionManager.GetEloWinProbability(this.AllFinalPlayerScores[j].EloScore, this.AllFinalPlayerScores[k].EloScore);
					int num2 = j * this.AllFinalPlayerScores.Count + k;
					if (!this.VisitedScoreCombintations.ContainsKey(num2))
					{
						RankedMultiplayerScore.PlayerScore playerScore = this.AllFinalPlayerScores[j];
						float num3;
						if (flag)
						{
							num3 = 0.5f;
						}
						else
						{
							num3 = (float)((j < k) ? 1 : 0);
						}
						float eloScore = playerScore.EloScore;
						float num4 = RankedProgressionManager.UpdateEloScore(eloScore, eloWinProbability, num3, num);
						Dictionary<int, float> dictionary = this.InProgressEloDeltaPerPlayer;
						int num5 = playerScore.PlayerId;
						dictionary[num5] += num4 - eloScore;
						this.VisitedScoreCombintations.Add(num2, true);
					}
					int num6 = k * this.AllFinalPlayerScores.Count + j;
					if (!this.VisitedScoreCombintations.ContainsKey(num6))
					{
						RankedMultiplayerScore.PlayerScore playerScore2 = this.AllFinalPlayerScores[k];
						float num3;
						if (flag)
						{
							num3 = 0.5f;
						}
						else
						{
							num3 = (float)((k < j) ? 1 : 0);
						}
						float eloScore2 = playerScore2.EloScore;
						float num7 = RankedProgressionManager.UpdateEloScore(eloScore2, eloWinProbability2, num3, num);
						Dictionary<int, float> dictionary = this.InProgressEloDeltaPerPlayer;
						int num5 = playerScore2.PlayerId;
						dictionary[num5] += num7 - eloScore2;
						this.VisitedScoreCombintations.Add(num6, true);
					}
				}
			}
		}
	}

	// Token: 0x06003DE1 RID: 15841 RVA: 0x0014FCB4 File Offset: 0x0014DEB4
	public void CachePlayerRankedProgressionData(int playerId, int tierIdx, float elo)
	{
		if (this.PlayerRankedTierIndices.ContainsKey(playerId))
		{
			this.PlayerRankedTierIndices[playerId] = tierIdx;
		}
		else
		{
			this.PlayerRankedTierIndices.Add(playerId, tierIdx);
		}
		if (this.PlayerRankedElos.ContainsKey(playerId))
		{
			this.PlayerRankedElos[playerId] = elo;
			return;
		}
		this.PlayerRankedElos.Add(playerId, elo);
	}

	// Token: 0x170005AF RID: 1455
	// (get) Token: 0x06003DE2 RID: 15842 RVA: 0x0014FD14 File Offset: 0x0014DF14
	// (set) Token: 0x06003DE3 RID: 15843 RVA: 0x0014FD1C File Offset: 0x0014DF1C
	public Dictionary<int, int> PlayerRankedTiers
	{
		get
		{
			return this.PlayerRankedTierIndices;
		}
		set
		{
			this.PlayerRankedTierIndices = value;
		}
	}

	// Token: 0x170005B0 RID: 1456
	// (get) Token: 0x06003DE4 RID: 15844 RVA: 0x0014FD25 File Offset: 0x0014DF25
	// (set) Token: 0x06003DE5 RID: 15845 RVA: 0x0014FD2D File Offset: 0x0014DF2D
	public Dictionary<int, float> PlayerRankedEloScores
	{
		get
		{
			return this.PlayerRankedElos;
		}
		set
		{
			this.PlayerRankedElos = value;
		}
	}

	// Token: 0x170005B1 RID: 1457
	// (get) Token: 0x06003DE6 RID: 15846 RVA: 0x0014FD36 File Offset: 0x0014DF36
	// (set) Token: 0x06003DE7 RID: 15847 RVA: 0x0014FD3E File Offset: 0x0014DF3E
	public Dictionary<int, float> ProjectedEloDeltas
	{
		get
		{
			return this.InProgressEloDeltaPerPlayer;
		}
		set
		{
			this.InProgressEloDeltaPerPlayer = value;
		}
	}

	// Token: 0x06003DE8 RID: 15848 RVA: 0x0014FD48 File Offset: 0x0014DF48
	public List<RankedMultiplayerScore.PlayerScoreInRound> GetSortedScores()
	{
		List<RankedMultiplayerScore.PlayerScoreInRound> list = new List<RankedMultiplayerScore.PlayerScoreInRound>();
		foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
		{
			list.Add(keyValuePair.Value);
		}
		list.Sort((RankedMultiplayerScore.PlayerScoreInRound s1, RankedMultiplayerScore.PlayerScoreInRound s2) => this.ComputeGameScore(s2.NumTags, s2.PointsOnDefense).CompareTo(this.ComputeGameScore(s1.NumTags, s1.PointsOnDefense)));
		return list;
	}

	// Token: 0x04004E7E RID: 20094
	public static float LongestUntaggedTieEpsilon = 0.2f;

	// Token: 0x04004E7F RID: 20095
	public static int RESULT_TIE = -1;

	// Token: 0x04004E80 RID: 20096
	[SerializeField]
	private int PointsPerTag = 30;

	// Token: 0x04004E81 RID: 20097
	[SerializeField]
	private float PointsPerUninfectedSecMin = 0.5f;

	// Token: 0x04004E82 RID: 20098
	[SerializeField]
	private float PointsPerUninfectedSecMax = 2f;

	// Token: 0x04004E83 RID: 20099
	private float PerSecondTimer = -1f;

	// Token: 0x04004E84 RID: 20100
	private bool WasInfectedInitially;

	// Token: 0x04004E85 RID: 20101
	private GorillaTagCompetitiveManager CompetitiveManager;

	// Token: 0x04004E86 RID: 20102
	protected Dictionary<int, RankedMultiplayerScore.PlayerScoreInRound> AllPlayerInRoundScores = new Dictionary<int, RankedMultiplayerScore.PlayerScoreInRound>();

	// Token: 0x04004E87 RID: 20103
	protected List<RankedMultiplayerScore.PlayerScore> AllFinalPlayerScores = new List<RankedMultiplayerScore.PlayerScore>();

	// Token: 0x04004E88 RID: 20104
	protected Dictionary<int, bool> VisitedScoreCombintations = new Dictionary<int, bool>();

	// Token: 0x04004E89 RID: 20105
	protected Dictionary<int, float> InProgressEloDeltaPerPlayer = new Dictionary<int, float>();

	// Token: 0x04004E8A RID: 20106
	protected Dictionary<int, int> PlayerRankedTierIndices = new Dictionary<int, int>();

	// Token: 0x04004E8B RID: 20107
	protected Dictionary<int, float> PlayerRankedElos = new Dictionary<int, float>();

	// Token: 0x04004E8C RID: 20108
	private RankedMultiplayerScore.ResultData PendingResults;

	// Token: 0x04004E8D RID: 20109
	private RankedMultiplayerScore.RecordHolder<int> ResultsMostTags;

	// Token: 0x04004E8E RID: 20110
	private RankedMultiplayerScore.RecordHolder<float> ResultsLongestUntagged;

	// Token: 0x04004E8F RID: 20111
	private bool IsLateJoiner;

	// Token: 0x02000939 RID: 2361
	public struct PlayerScore
	{
		// Token: 0x04004E91 RID: 20113
		public int PlayerId;

		// Token: 0x04004E92 RID: 20114
		public float GameScore;

		// Token: 0x04004E93 RID: 20115
		public float EloScore;

		// Token: 0x04004E94 RID: 20116
		public int NumTags;

		// Token: 0x04004E95 RID: 20117
		public float TimeUntagged;

		// Token: 0x04004E96 RID: 20118
		public float PointsOnDefense;
	}

	// Token: 0x0200093A RID: 2362
	public struct PlayerScoreInRound
	{
		// Token: 0x06003DEC RID: 15852 RVA: 0x0014FE88 File Offset: 0x0014E088
		public PlayerScoreInRound(int id, bool initInfected = false)
		{
			this.PlayerId = id;
			this.NumTags = 0;
			this.PointsOnDefense = 0f;
			this.JoinTime = Time.time;
			this.Infected = initInfected;
			this.TaggedTime = (initInfected ? Time.time : 0f);
		}

		// Token: 0x04004E97 RID: 20119
		public int PlayerId;

		// Token: 0x04004E98 RID: 20120
		public int NumTags;

		// Token: 0x04004E99 RID: 20121
		public float PointsOnDefense;

		// Token: 0x04004E9A RID: 20122
		public float JoinTime;

		// Token: 0x04004E9B RID: 20123
		public float TaggedTime;

		// Token: 0x04004E9C RID: 20124
		public bool Infected;
	}

	// Token: 0x0200093B RID: 2363
	public struct ResultData
	{
		// Token: 0x06003DED RID: 15853 RVA: 0x0014FED5 File Offset: 0x0014E0D5
		public bool IsMostTagsTied()
		{
			return this.MostTagsPlayerId == RankedMultiplayerScore.RESULT_TIE;
		}

		// Token: 0x06003DEE RID: 15854 RVA: 0x0014FEE4 File Offset: 0x0014E0E4
		public bool IsLongestUntaggedTied()
		{
			return this.LongestUntaggedPlayerId == RankedMultiplayerScore.RESULT_TIE;
		}

		// Token: 0x04004E9D RID: 20125
		public float Elo;

		// Token: 0x04004E9E RID: 20126
		public int Rank;

		// Token: 0x04004E9F RID: 20127
		public int MostTags;

		// Token: 0x04004EA0 RID: 20128
		public float LongestUntagged;

		// Token: 0x04004EA1 RID: 20129
		public int MostTagsPlayerId;

		// Token: 0x04004EA2 RID: 20130
		public int LongestUntaggedPlayerId;
	}

	// Token: 0x0200093C RID: 2364
	public struct RecordHolder<T>
	{
		// Token: 0x04004EA3 RID: 20131
		public int PlayerId;

		// Token: 0x04004EA4 RID: 20132
		public T Value;
	}
}
