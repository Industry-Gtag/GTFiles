using System;
using UnityEngine;

// Token: 0x020008FF RID: 2303
public class HeightVolume : MonoBehaviour
{
	// Token: 0x06003C64 RID: 15460 RVA: 0x00149A15 File Offset: 0x00147C15
	private void Awake()
	{
		if (this.targetTransform == null)
		{
			this.targetTransform = Camera.main.transform;
		}
		this.musicSource = this.audioSource.gameObject.GetComponent<MusicSource>();
	}

	// Token: 0x06003C65 RID: 15461 RVA: 0x00149A4C File Offset: 0x00147C4C
	private void Update()
	{
		if (this.audioSource.gameObject.activeSelf && (!(this.musicSource != null) || !this.musicSource.VolumeOverridden))
		{
			if (this.targetTransform.position.y > this.heightTop.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.baseVolume : this.minVolume);
				return;
			}
			if (this.targetTransform.position.y < this.heightBottom.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.minVolume : this.baseVolume);
				return;
			}
			this.audioSource.volume = ((!this.invertHeightVol) ? ((this.targetTransform.position.y - this.heightBottom.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume) : ((this.heightTop.position.y - this.targetTransform.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume));
		}
	}

	// Token: 0x04004D0F RID: 19727
	public Transform heightTop;

	// Token: 0x04004D10 RID: 19728
	public Transform heightBottom;

	// Token: 0x04004D11 RID: 19729
	public AudioSource audioSource;

	// Token: 0x04004D12 RID: 19730
	public float baseVolume;

	// Token: 0x04004D13 RID: 19731
	public float minVolume;

	// Token: 0x04004D14 RID: 19732
	public Transform targetTransform;

	// Token: 0x04004D15 RID: 19733
	public bool invertHeightVol;

	// Token: 0x04004D16 RID: 19734
	private MusicSource musicSource;
}
