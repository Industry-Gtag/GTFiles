using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000597 RID: 1431
public class HypnoRing : MonoBehaviour, ISpawnable
{
	// Token: 0x170003CE RID: 974
	// (get) Token: 0x06002437 RID: 9271 RVA: 0x000C28B9 File Offset: 0x000C0AB9
	// (set) Token: 0x06002438 RID: 9272 RVA: 0x000C28C1 File Offset: 0x000C0AC1
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x06002439 RID: 9273 RVA: 0x000C28CA File Offset: 0x000C0ACA
	// (set) Token: 0x0600243A RID: 9274 RVA: 0x000C28D2 File Offset: 0x000C0AD2
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x0600243B RID: 9275 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x000C28DB File Offset: 0x000C0ADB
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x000C28E4 File Offset: 0x000C0AE4
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			base.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * this.rotationSpeed, Vector3.up);
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.maxVolume, Time.deltaTime / this.fadeInDuration);
			this.audioSource.volume = this.currentVolume;
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
				return;
			}
		}
		else
		{
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, 0f, Time.deltaTime / this.fadeOutDuration);
			if (this.audioSource.isPlaying)
			{
				if (this.currentVolume == 0f)
				{
					this.audioSource.GTStop();
					return;
				}
				this.audioSource.volume = this.currentVolume;
			}
		}
	}

	// Token: 0x04002F7A RID: 12154
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04002F7B RID: 12155
	private VRRig myRig;

	// Token: 0x04002F7C RID: 12156
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04002F7D RID: 12157
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002F7E RID: 12158
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04002F7F RID: 12159
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x04002F80 RID: 12160
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x04002F83 RID: 12163
	private float currentVolume;
}
