using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020009DE RID: 2526
public class ReplacementVoice : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060040CD RID: 16589 RVA: 0x0001212B File Offset: 0x0001032B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060040CE RID: 16590 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060040CF RID: 16591 RVA: 0x00158EEC File Offset: 0x001570EC
	public void SliceUpdate()
	{
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.ShouldPlayReplacementVoice())
		{
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < this.loudReplacementVoiceThreshold)
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClips[Random.Range(0, this.replacementVoiceClips.Length - 1)];
				this.replacementVoiceSource.volume = this.normalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClipsLoud[Random.Range(0, this.replacementVoiceClipsLoud.Length - 1)];
				this.replacementVoiceSource.volume = this.loudVolume;
			}
			this.replacementVoiceSource.GTPlay();
			return;
		}
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride, out cosmeticEffect))
		{
			if (this.myVRRig.SpeakingLoudness < this.myVRRig.replacementVoiceLoudnessThreshold)
			{
				return;
			}
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < cosmeticEffect.voiceOverrideLoudThreshold)
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideNormalClips[Random.Range(0, cosmeticEffect.voiceOverrideNormalClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideNormalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideLoudClips[Random.Range(0, cosmeticEffect.voiceOverrideLoudClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideLoudVolume;
			}
			this.replacementVoiceSource.GTPlay();
		}
	}

	// Token: 0x0400514D RID: 20813
	public AudioSource replacementVoiceSource;

	// Token: 0x0400514E RID: 20814
	public AudioClip[] replacementVoiceClips;

	// Token: 0x0400514F RID: 20815
	public AudioClip[] replacementVoiceClipsLoud;

	// Token: 0x04005150 RID: 20816
	public float loudReplacementVoiceThreshold = 0.1f;

	// Token: 0x04005151 RID: 20817
	public VRRig myVRRig;

	// Token: 0x04005152 RID: 20818
	public float normalVolume = 0.5f;

	// Token: 0x04005153 RID: 20819
	public float loudVolume = 0.8f;
}
