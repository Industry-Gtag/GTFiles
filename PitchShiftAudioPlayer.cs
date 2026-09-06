using System;
using UnityEngine;

// Token: 0x0200095C RID: 2396
public class PitchShiftAudioPlayer : MonoBehaviour
{
	// Token: 0x06003EF8 RID: 16120 RVA: 0x00152DDE File Offset: 0x00150FDE
	private void Awake()
	{
		if (this._source == null)
		{
			this._source = base.GetComponent<AudioSource>();
		}
		if (this._pitch == null)
		{
			this._pitch = base.GetComponent<RangedFloat>();
		}
	}

	// Token: 0x06003EF9 RID: 16121 RVA: 0x00152E14 File Offset: 0x00151014
	private void OnEnable()
	{
		this._pitchMixVars.Rent(out this._pitchMix);
		this._source.outputAudioMixerGroup = this._pitchMix.group;
	}

	// Token: 0x06003EFA RID: 16122 RVA: 0x00152E3E File Offset: 0x0015103E
	private void OnDisable()
	{
		this._source.Stop();
		this._source.outputAudioMixerGroup = null;
		AudioMixVar pitchMix = this._pitchMix;
		if (pitchMix == null)
		{
			return;
		}
		pitchMix.ReturnToPool();
	}

	// Token: 0x06003EFB RID: 16123 RVA: 0x00152E67 File Offset: 0x00151067
	private void Update()
	{
		if (this.apply)
		{
			this.ApplyPitch();
		}
	}

	// Token: 0x06003EFC RID: 16124 RVA: 0x00152E77 File Offset: 0x00151077
	private void ApplyPitch()
	{
		this._pitchMix.value = this._pitch.curved;
	}

	// Token: 0x04004F44 RID: 20292
	public bool apply = true;

	// Token: 0x04004F45 RID: 20293
	[SerializeField]
	private AudioSource _source;

	// Token: 0x04004F46 RID: 20294
	[SerializeField]
	private AudioMixVarPool _pitchMixVars;

	// Token: 0x04004F47 RID: 20295
	[SerializeReference]
	private AudioMixVar _pitchMix;

	// Token: 0x04004F48 RID: 20296
	[SerializeField]
	private RangedFloat _pitch;
}
