using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020005AC RID: 1452
public class SoundEffects : MonoBehaviour
{
	// Token: 0x170003DA RID: 986
	// (get) Token: 0x060024C9 RID: 9417 RVA: 0x000C5C82 File Offset: 0x000C3E82
	public bool isPlaying
	{
		get
		{
			return this._lastClipIndex >= 0 && this._lastClipLength >= 0.0 && this._lastClipElapsedTime < this._lastClipLength;
		}
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x000C5CB5 File Offset: 0x000C3EB5
	public void Clear()
	{
		this.audioClips.Clear();
		this._lastClipIndex = -1;
		this._lastClipLength = -1.0;
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000C5CD8 File Offset: 0x000C3ED8
	public void Stop()
	{
		if (this.source)
		{
			this.source.GTStop();
		}
		this._lastClipLength = -1.0;
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x000C5D04 File Offset: 0x000C3F04
	public void PlayNext(float delayMin, float delayMax, float volMin, float volMax)
	{
		float num = this._rnd.NextFloat(delayMin, delayMax);
		float num2 = this._rnd.NextFloat(volMin, volMax);
		this.PlayNext(num, num2);
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x000C5D38 File Offset: 0x000C3F38
	public void PlayNext(float delay = 0f, float volume = 1f)
	{
		if (!this.source)
		{
			return;
		}
		if (this.audioClips == null || this.audioClips.Count == 0)
		{
			return;
		}
		if (this.source.isPlaying)
		{
			this.source.GTStop();
		}
		int num = this._rnd.NextInt(this.audioClips.Count);
		while (this.distinct && this._lastClipIndex == num)
		{
			num = this._rnd.NextInt(this.audioClips.Count);
		}
		AudioClip audioClip = this.audioClips[num];
		this._lastClipIndex = num;
		this._lastClipLength = (double)audioClip.length;
		float num2 = delay;
		if (num2 < this._minDelay)
		{
			num2 = this._minDelay;
		}
		if (num2 < 0.0001f)
		{
			this.source.GTPlayOneShot(audioClip, volume);
			this._lastClipElapsedTime = 0f;
			return;
		}
		this.source.clip = audioClip;
		this.source.volume = volume;
		this.source.GTPlayDelayed(num2);
		this._lastClipElapsedTime = -num2;
	}

	// Token: 0x060024CE RID: 9422 RVA: 0x000C5E4C File Offset: 0x000C404C
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(this.seed))
		{
			this.seed = "0x1337C0D3";
		}
		this._rnd = new SRand(this.seed);
		if (this.audioClips == null)
		{
			this.audioClips = new List<AudioClip>();
		}
	}

	// Token: 0x04003052 RID: 12370
	public AudioSource source;

	// Token: 0x04003053 RID: 12371
	[Space]
	public List<AudioClip> audioClips = new List<AudioClip>();

	// Token: 0x04003054 RID: 12372
	public string seed = "0x1337C0D3";

	// Token: 0x04003055 RID: 12373
	[Space]
	public bool distinct = true;

	// Token: 0x04003056 RID: 12374
	[SerializeField]
	private float _minDelay;

	// Token: 0x04003057 RID: 12375
	[Space]
	[SerializeField]
	private SRand _rnd;

	// Token: 0x04003058 RID: 12376
	[NonSerialized]
	private int _lastClipIndex = -1;

	// Token: 0x04003059 RID: 12377
	[NonSerialized]
	private double _lastClipLength = -1.0;

	// Token: 0x0400305A RID: 12378
	[NonSerialized]
	private TimeSince _lastClipElapsedTime;
}
