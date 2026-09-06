using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200036A RID: 874
public class RaceCheckpoint : MonoBehaviour
{
	// Token: 0x06001561 RID: 5473 RVA: 0x0007199B File Offset: 0x0006FB9B
	public void Init(RaceCheckpointManager manager, int index)
	{
		this.manager = manager;
		this.checkpointIndex = index;
		this.SetIsCorrectCheckpoint(index == 0);
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x000719B5 File Offset: 0x0006FBB5
	public void SetIsCorrectCheckpoint(bool isCorrect)
	{
		this.isCorrect = isCorrect;
		this.banner.sharedMaterial = (isCorrect ? this.activeCheckpointMat : this.wrongCheckpointMat);
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x000719DA File Offset: 0x0006FBDA
	private void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.headCollider)
		{
			return;
		}
		if (this.isCorrect)
		{
			this.manager.OnCheckpointReached(this.checkpointIndex, this.checkpointSound);
			return;
		}
		this.wrongCheckpointSound.Play();
	}

	// Token: 0x04001A38 RID: 6712
	[SerializeField]
	private MeshRenderer banner;

	// Token: 0x04001A39 RID: 6713
	[SerializeField]
	private Material activeCheckpointMat;

	// Token: 0x04001A3A RID: 6714
	[SerializeField]
	private Material wrongCheckpointMat;

	// Token: 0x04001A3B RID: 6715
	[SerializeField]
	private SoundBankPlayer checkpointSound;

	// Token: 0x04001A3C RID: 6716
	[SerializeField]
	private SoundBankPlayer wrongCheckpointSound;

	// Token: 0x04001A3D RID: 6717
	private RaceCheckpointManager manager;

	// Token: 0x04001A3E RID: 6718
	private int checkpointIndex;

	// Token: 0x04001A3F RID: 6719
	private bool isCorrect;
}
