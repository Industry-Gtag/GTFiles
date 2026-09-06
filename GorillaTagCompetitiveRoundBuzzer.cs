using System;
using UnityEngine;

// Token: 0x020008B0 RID: 2224
public class GorillaTagCompetitiveRoundBuzzer : MonoBehaviour
{
	// Token: 0x06003A64 RID: 14948 RVA: 0x0013D5F7 File Offset: 0x0013B7F7
	private void OnEnable()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.OnStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime += this.OnUpdateRemainingTime;
	}

	// Token: 0x06003A65 RID: 14949 RVA: 0x0013D61B File Offset: 0x0013B81B
	private void OnDisable()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.OnStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime -= this.OnUpdateRemainingTime;
	}

	// Token: 0x06003A66 RID: 14950 RVA: 0x0013D640 File Offset: 0x0013B840
	private void OnStateChanged(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			this.PlaySFX(this.needMorePlayerClip);
			break;
		case GorillaTagCompetitiveManager.GameState.Playing:
			this.PlaySFX(this.roundStartClip);
			break;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			this.PlaySFX(this.roundEndClip);
			break;
		}
		this.lastState = newState;
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x0013D698 File Offset: 0x0013B898
	private void OnUpdateRemainingTime(float remainingTime)
	{
		int num = Mathf.CeilToInt(remainingTime);
		int num2 = Mathf.CeilToInt(this.lastStateRemainingTime);
		if (num != num2)
		{
			GorillaTagCompetitiveManager.GameState gameState = this.lastState;
			if (gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown)
			{
				if (gameState == GorillaTagCompetitiveManager.GameState.Playing)
				{
					if (num > 0 && num <= this.roundEndCountdownDuration)
					{
						this.PlaySFX(this.roundEndingCountdownClip);
					}
				}
			}
			else if (num > 0)
			{
				this.PlaySFX(this.roundCountdownClip);
			}
		}
		this.lastStateRemainingTime = remainingTime;
	}

	// Token: 0x06003A68 RID: 14952 RVA: 0x0013D6FF File Offset: 0x0013B8FF
	private void PlaySFX(AudioClip clip)
	{
		this.PlaySFX(clip, 1f);
	}

	// Token: 0x06003A69 RID: 14953 RVA: 0x0013D70D File Offset: 0x0013B90D
	private void PlaySFX(AudioClip clip, float volume)
	{
		this.audioSource.PlayOneShot(clip, volume);
	}

	// Token: 0x04004A53 RID: 19027
	public AudioSource audioSource;

	// Token: 0x04004A54 RID: 19028
	public AudioClip roundCountdownClip;

	// Token: 0x04004A55 RID: 19029
	public AudioClip roundStartClip;

	// Token: 0x04004A56 RID: 19030
	public AudioClip roundEndingCountdownClip;

	// Token: 0x04004A57 RID: 19031
	public int roundEndCountdownDuration = 5;

	// Token: 0x04004A58 RID: 19032
	public AudioClip roundEndClip;

	// Token: 0x04004A59 RID: 19033
	public AudioClip needMorePlayerClip;

	// Token: 0x04004A5A RID: 19034
	private GorillaTagCompetitiveManager.GameState lastState;

	// Token: 0x04004A5B RID: 19035
	private float lastStateRemainingTime = -1f;
}
