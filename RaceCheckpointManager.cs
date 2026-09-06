using System;
using UnityEngine;

// Token: 0x0200036B RID: 875
public class RaceCheckpointManager : MonoBehaviour
{
	// Token: 0x06001565 RID: 5477 RVA: 0x00071A1C File Offset: 0x0006FC1C
	private void Start()
	{
		this.visual = base.GetComponent<RaceVisual>();
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].Init(this, i);
		}
		this.OnRaceEnd();
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x00071A60 File Offset: 0x0006FC60
	public void OnRaceStart()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(i == 0);
		}
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x00071A94 File Offset: 0x0006FC94
	public void OnRaceEnd()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(false);
		}
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x00071AC2 File Offset: 0x0006FCC2
	public void OnCheckpointReached(int index, SoundBankPlayer checkpointSound)
	{
		this.checkpoints[index].SetIsCorrectCheckpoint(false);
		this.checkpoints[(index + 1) % this.checkpoints.Length].SetIsCorrectCheckpoint(true);
		this.visual.OnCheckpointPassed(index, checkpointSound);
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x00071AF8 File Offset: 0x0006FCF8
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpointIdx)
	{
		return checkpointIdx >= 0 && checkpointIdx < this.checkpoints.Length && player.IsPositionInRange(this.checkpoints[checkpointIdx].transform.position, 6f);
	}

	// Token: 0x04001A40 RID: 6720
	[SerializeField]
	private RaceCheckpoint[] checkpoints;

	// Token: 0x04001A41 RID: 6721
	private RaceVisual visual;
}
