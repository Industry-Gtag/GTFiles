using System;
using TMPro;
using UnityEngine;

// Token: 0x02000612 RID: 1554
public class MonkeBallScoreboard : MonoBehaviour
{
	// Token: 0x060026D8 RID: 9944 RVA: 0x000CDD8E File Offset: 0x000CBF8E
	public void Setup(MonkeBallGame game)
	{
		this.game = game;
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x000CDD98 File Offset: 0x000CBF98
	public void RefreshScore()
	{
		for (int i = 0; i < this.game.team.Count; i++)
		{
			this.teamDisplays[i].scoreLabel.text = this.game.team[i].score.ToString();
		}
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000CDDED File Offset: 0x000CBFED
	public void RefreshTeamPlayers(int teamId, int numPlayers)
	{
		this.teamDisplays[teamId].playersLabel.text = string.Format("PLAYERS: {0}", Mathf.Clamp(numPlayers, 0, 99));
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000CDE19 File Offset: 0x000CC019
	public void PlayScoreFx()
	{
		this.PlayFX(this.scoreSound, this.scoreSoundVolume);
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000CDE2D File Offset: 0x000CC02D
	public void PlayPlayerJoinFx()
	{
		this.PlayFX(this.playerJoinSound, 0.5f);
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000CDE40 File Offset: 0x000CC040
	public void PlayPlayerLeaveFx()
	{
		this.PlayFX(this.playerLeaveSound, 0.5f);
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x000CDE53 File Offset: 0x000CC053
	public void PlayGameStartFx()
	{
		this.PlayFX(this.gameStartSound, this.gameStartVolume);
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x000CDE67 File Offset: 0x000CC067
	public void PlayGameEndFx()
	{
		this.PlayFX(this.gameEndSound, this.gameEndVolume);
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000CDE7B File Offset: 0x000CC07B
	private void PlayFX(AudioClip clip, float volume)
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = clip;
			this.audioSource.volume = volume;
			this.audioSource.Play();
		}
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000CDEAE File Offset: 0x000CC0AE
	public void RefreshTime(string timeString)
	{
		this.timeRemainingLabel.text = timeString;
	}

	// Token: 0x0400324B RID: 12875
	private MonkeBallGame game;

	// Token: 0x0400324C RID: 12876
	public MonkeBallScoreboard.TeamDisplay[] teamDisplays;

	// Token: 0x0400324D RID: 12877
	public TextMeshPro timeRemainingLabel;

	// Token: 0x0400324E RID: 12878
	public AudioSource audioSource;

	// Token: 0x0400324F RID: 12879
	public AudioClip scoreSound;

	// Token: 0x04003250 RID: 12880
	public float scoreSoundVolume;

	// Token: 0x04003251 RID: 12881
	public AudioClip playerJoinSound;

	// Token: 0x04003252 RID: 12882
	public AudioClip playerLeaveSound;

	// Token: 0x04003253 RID: 12883
	public AudioClip gameStartSound;

	// Token: 0x04003254 RID: 12884
	public float gameStartVolume;

	// Token: 0x04003255 RID: 12885
	public AudioClip gameEndSound;

	// Token: 0x04003256 RID: 12886
	public float gameEndVolume;

	// Token: 0x02000613 RID: 1555
	[Serializable]
	public class TeamDisplay
	{
		// Token: 0x04003257 RID: 12887
		public TextMeshPro nameLabel;

		// Token: 0x04003258 RID: 12888
		public TextMeshPro scoreLabel;

		// Token: 0x04003259 RID: 12889
		public TextMeshPro playersLabel;
	}
}
