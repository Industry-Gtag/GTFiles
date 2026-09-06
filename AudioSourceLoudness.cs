using System;
using UnityEngine;

// Token: 0x020005F8 RID: 1528
public class AudioSourceLoudness : MonoBehaviour, ISpeakerLoudness
{
	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x06002603 RID: 9731 RVA: 0x000C93C5 File Offset: 0x000C75C5
	public bool IsSpeaking
	{
		get
		{
			return this.audioSource != null && this.audioSource.isPlaying;
		}
	}

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x06002604 RID: 9732 RVA: 0x000C93E2 File Offset: 0x000C75E2
	public float Loudness
	{
		get
		{
			return this.loudness;
		}
	}

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x06002605 RID: 9733 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsMicEnabled
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000C93EA File Offset: 0x000C75EA
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		this.sampleBuffer = new float[Mathf.Max(1, this.sampleWindow)];
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000C9420 File Offset: 0x000C7620
	private void Update()
	{
		if (this.audioSource == null || !this.audioSource.isPlaying)
		{
			this.loudness = 0f;
			return;
		}
		this.audioSource.GetOutputData(this.sampleBuffer, 0);
		float num = 0f;
		for (int i = 0; i < this.sampleBuffer.Length; i++)
		{
			num += Mathf.Abs(this.sampleBuffer[i]);
		}
		this.loudness = num / (float)this.sampleBuffer.Length;
	}

	// Token: 0x04003191 RID: 12689
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003192 RID: 12690
	[Tooltip("Number of output samples averaged per frame to compute loudness.")]
	[SerializeField]
	private int sampleWindow = 256;

	// Token: 0x04003193 RID: 12691
	private float loudness;

	// Token: 0x04003194 RID: 12692
	private float[] sampleBuffer;
}
