using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class CrittersVoiceNoise : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000313 RID: 787 RVA: 0x0001211D File Offset: 0x0001031D
	private void Start()
	{
		this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0001212B File Offset: 0x0001032B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06000316 RID: 790 RVA: 0x00012140 File Offset: 0x00010340
	public void SliceUpdate()
	{
		float num = 0f;
		if (this.speaker.IsSpeaking)
		{
			num = this.speaker.Loudness;
		}
		if (num > this.minTriggerThreshold && CrittersManager.instance.IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.rig].rigActors[4].actorSet;
			if (crittersLoudNoise.IsNotNull() && !crittersLoudNoise.soundEnabled)
			{
				float num2 = Mathf.Lerp(this.noiseVolumeMin, this.noisVolumeMax, Mathf.Clamp01((num - this.minTriggerThreshold) / this.maxTriggerThreshold));
				crittersLoudNoise.PlayVoiceSpeechLocal(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time), 0.016666668f, num2);
			}
		}
	}

	// Token: 0x04000374 RID: 884
	[SerializeField]
	private GorillaSpeakerLoudness speaker;

	// Token: 0x04000375 RID: 885
	[SerializeField]
	private VRRig rig;

	// Token: 0x04000376 RID: 886
	[SerializeField]
	private float minTriggerThreshold = 0.01f;

	// Token: 0x04000377 RID: 887
	[SerializeField]
	private float maxTriggerThreshold = 0.3f;

	// Token: 0x04000378 RID: 888
	[SerializeField]
	private float noiseVolumeMin = 1f;

	// Token: 0x04000379 RID: 889
	[SerializeField]
	private float noisVolumeMax = 9f;
}
