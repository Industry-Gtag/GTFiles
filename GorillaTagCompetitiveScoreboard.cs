using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008B1 RID: 2225
public class GorillaTagCompetitiveScoreboard : MonoBehaviour
{
	// Token: 0x06003A6B RID: 14955 RVA: 0x0013D738 File Offset: 0x0013B938
	private void Awake()
	{
		GorillaTagCompetitiveManager.RegisterScoreboard(this);
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x0013D771 File Offset: 0x0013B971
	private void OnDestroy()
	{
		GorillaTagCompetitiveManager.DeregisterScoreboard(this);
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x0013D77C File Offset: 0x0013B97C
	public void UpdateScores(GorillaTagCompetitiveManager.GameState gameState, float activeRoundTime, List<RankedMultiplayerScore.PlayerScoreInRound> scores, Dictionary<int, int> PlayerRankedTiers, Dictionary<int, float> PlayerPredictedEloDeltas, List<NetPlayer> infectedPlayers, RankedProgressionManager progressionManager)
	{
		this.waitingForPlayers.SetActive(gameState == GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
		for (int i = 0; i < this.lines.Length; i++)
		{
			if (gameState != GorillaTagCompetitiveManager.GameState.WaitingForPlayers && scores != null && scores.Count > i)
			{
				RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = scores[i];
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerScoreInRound.PlayerId);
				if (netPlayerByID != null)
				{
					this.lines[i].gameObject.SetActive(true);
					if (PlayerRankedTiers == null || !PlayerRankedTiers.ContainsKey(playerScoreInRound.PlayerId))
					{
						this.lines[i].SetPlayer(netPlayerByID.SanitizedNickName, null);
					}
					else
					{
						this.lines[i].SetPlayer(netPlayerByID.SanitizedNickName, progressionManager.GetProgressionRankIcon(PlayerRankedTiers[playerScoreInRound.PlayerId]));
					}
					if (playerScoreInRound.TaggedTime.Approx(0f, 1E-06f))
					{
						this.lines[i].SetScore(Mathf.Max(activeRoundTime - playerScoreInRound.JoinTime, 0f), playerScoreInRound.NumTags);
					}
					else
					{
						this.lines[i].SetScore(Mathf.Max(playerScoreInRound.TaggedTime - playerScoreInRound.JoinTime, 0f), playerScoreInRound.NumTags);
					}
					if (PlayerPredictedEloDeltas.ContainsKey(playerScoreInRound.PlayerId))
					{
						float num = PlayerPredictedEloDeltas[playerScoreInRound.PlayerId];
						GorillaTagCompetitiveScoreboard.PredictedResult predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Even;
						if (num > this.largeEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Great;
						}
						else if (num > this.smallEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Good;
						}
						else if (num < -this.largeEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Poor;
						}
						else if (num < -this.smallEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Bad;
						}
						this.lines[i].SetPredictedResult(predictedResult);
					}
					this.lines[i].SetInfected(gameState == GorillaTagCompetitiveManager.GameState.Playing && infectedPlayers.Contains(netPlayerByID));
				}
			}
			else
			{
				this.lines[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x0013D94C File Offset: 0x0013BB4C
	public void DisplayPredictedResults(bool bShow)
	{
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].DisplayPredictedResults(bShow);
		}
	}

	// Token: 0x04004A5C RID: 19036
	public GorillaTagCompetitiveScoreboardLine[] lines;

	// Token: 0x04004A5D RID: 19037
	public GameObject waitingForPlayers;

	// Token: 0x04004A5E RID: 19038
	public float smallEloDelta = 10f;

	// Token: 0x04004A5F RID: 19039
	public float largeEloDelta = 25f;

	// Token: 0x020008B2 RID: 2226
	public enum PredictedResult
	{
		// Token: 0x04004A61 RID: 19041
		Great,
		// Token: 0x04004A62 RID: 19042
		Good,
		// Token: 0x04004A63 RID: 19043
		Even,
		// Token: 0x04004A64 RID: 19044
		Bad,
		// Token: 0x04004A65 RID: 19045
		Poor
	}
}
