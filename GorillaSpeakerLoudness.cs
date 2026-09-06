using System;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using Oculus.VoiceSDK.Utilities;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020008A7 RID: 2215
public class GorillaSpeakerLoudness : MonoBehaviour, IGorillaSliceableSimple, IDynamicFloat, ISpeakerLoudness
{
	// Token: 0x1700052D RID: 1325
	// (get) Token: 0x060039F2 RID: 14834 RVA: 0x0013BC91 File Offset: 0x00139E91
	public bool IsSpeaking
	{
		get
		{
			return this.isSpeaking;
		}
	}

	// Token: 0x1700052E RID: 1326
	// (get) Token: 0x060039F3 RID: 14835 RVA: 0x0013BC99 File Offset: 0x00139E99
	public float Loudness
	{
		get
		{
			return this.loudness;
		}
	}

	// Token: 0x1700052F RID: 1327
	// (get) Token: 0x060039F4 RID: 14836 RVA: 0x0013BCA1 File Offset: 0x00139EA1
	public float LoudnessNormalized
	{
		get
		{
			return Mathf.Min(this.loudness / this.normalizedMax, 1f);
		}
	}

	// Token: 0x17000530 RID: 1328
	// (get) Token: 0x060039F5 RID: 14837 RVA: 0x0013BCBA File Offset: 0x00139EBA
	public float floatValue
	{
		get
		{
			return this.LoudnessNormalized;
		}
	}

	// Token: 0x17000531 RID: 1329
	// (get) Token: 0x060039F6 RID: 14838 RVA: 0x0013BCC2 File Offset: 0x00139EC2
	public bool IsMicEnabled
	{
		get
		{
			return this.isMicEnabled;
		}
	}

	// Token: 0x17000532 RID: 1330
	// (get) Token: 0x060039F7 RID: 14839 RVA: 0x0013BCCA File Offset: 0x00139ECA
	public float SmoothedLoudness
	{
		get
		{
			return this.smoothedLoudness;
		}
	}

	// Token: 0x060039F8 RID: 14840 RVA: 0x0013BCD2 File Offset: 0x00139ED2
	private void Start()
	{
		this.rigContainer = base.GetComponent<RigContainer>();
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x060039F9 RID: 14841 RVA: 0x0001212B File Offset: 0x0001032B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060039FA RID: 14842 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060039FB RID: 14843 RVA: 0x0013BCF6 File Offset: 0x00139EF6
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.UpdateMicEnabled();
		this.UpdateLoudness();
		this.UpdateSmoothedLoudness();
	}

	// Token: 0x060039FC RID: 14844 RVA: 0x0013BD28 File Offset: 0x00139F28
	private void UpdateMicEnabled()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig)
		{
			this.isMicEnabled = this.CheckMicConnection();
			rig.IsMicEnabled = this.isMicEnabled;
			return;
		}
		this.isMicEnabled = rig.IsMicEnabled;
	}

	// Token: 0x060039FD RID: 14845 RVA: 0x0013BD80 File Offset: 0x00139F80
	private bool CheckMicConnection()
	{
		this.permission = this.permission || MicPermissionsManager.HasMicPermission();
		if (this.permission && !this.micConnected && Microphone.devices != null)
		{
			this.micConnected = Microphone.devices.Length != 0;
		}
		return this.permission && this.micConnected;
	}

	// Token: 0x060039FE RID: 14846 RVA: 0x0013BDDC File Offset: 0x00139FDC
	private void UpdateLoudness()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		PhotonVoiceView voice = this.rigContainer.Voice;
		if (voice != null && this.speaker == null)
		{
			this.speaker = voice.SpeakerInUse;
		}
		if (this.recorder == null)
		{
			this.recorder = ((voice != null) ? voice.RecorderInUse : null);
		}
		if (this.recorder != null && this.offlineMic != null)
		{
			Microphone.End(UnityMicrophone.devices[0]);
			Object.Destroy(this.offlineMic);
			this.offlineMic = null;
			this.recorder.RestartRecording(true);
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig && this.recorder == null && this.isMicEnabled && !Microphone.IsRecording(UnityMicrophone.devices[0]))
		{
			this.offlineMic = Microphone.Start(UnityMicrophone.devices[0], true, 1, 16000);
		}
		if ((rig.remoteUseReplacementVoice || rig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && rig.SpeakingLoudness > 0f && !this.rigContainer.IsMutedFor(~RigContainer.MuteReason.Auto))
		{
			this.isSpeaking = true;
			this.loudness = rig.SpeakingLoudness;
			return;
		}
		if (voice != null && voice.IsSpeaking)
		{
			this.isSpeaking = true;
			if (!(this.speaker != null))
			{
				this.loudness = 0f;
				return;
			}
			if (this.speakerVoiceToLoudness == null)
			{
				this.speakerVoiceToLoudness = this.speaker.GetComponent<SpeakerVoiceToLoudness>();
			}
			if (this.speakerVoiceToLoudness != null)
			{
				this.loudness = this.speakerVoiceToLoudness.loudness;
				return;
			}
		}
		else if (voice != null && this.recorder != null && NetworkSystem.Instance.IsObjectLocallyOwned(voice.gameObject) && this.recorder.IsCurrentlyTransmitting)
		{
			if (this.voiceToLoudness == null)
			{
				this.voiceToLoudness = this.recorder.GetComponent<VoiceToLoudness>();
				if (this.voiceToLoudness == null)
				{
					this.recorder.AddComponent<VoiceToLoudness>();
				}
			}
			this.isSpeaking = true;
			if (this.voiceToLoudness != null)
			{
				this.loudness = this.voiceToLoudness.Loudness;
				return;
			}
			this.loudness = 0f;
			return;
		}
		else if (this.offlineMic != null && this.recorder == null && this.isMicEnabled && Microphone.IsRecording(UnityMicrophone.devices[0]))
		{
			this.isSpeaking = true;
			int num = Mathf.Min(Mathf.CeilToInt(this.deltaTime * 16000f), 16000);
			if (num > this.voiceSampleBuffer.Length)
			{
				Array.Resize<float>(ref this.voiceSampleBuffer, num);
			}
			if (this.offlineMic.samples >= num && this.offlineMic.GetData(this.voiceSampleBuffer, this.offlineMic.samples - num))
			{
				float num2 = 0f;
				for (int i = 0; i < this.voiceSampleBuffer.Length; i++)
				{
					num2 += Mathf.Abs(this.voiceSampleBuffer[i]);
				}
				this.loudness = num2 / (float)this.voiceSampleBuffer.Length;
				return;
			}
		}
		else
		{
			this.isSpeaking = false;
			this.loudness = 0f;
		}
	}

	// Token: 0x060039FF RID: 14847 RVA: 0x0013C14C File Offset: 0x0013A34C
	private void UpdateSmoothedLoudness()
	{
		if (!this.isSpeaking)
		{
			this.smoothedLoudness = 0f;
			return;
		}
		if (!Mathf.Approximately(this.loudness, this.lastLoudness))
		{
			this.timeSinceLoudnessChange = 0f;
			this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
			this.lastLoudness = this.loudness;
			return;
		}
		if (this.timeSinceLoudnessChange > this.loudnessUpdateCheckRate)
		{
			this.smoothedLoudness = 0.001f;
			return;
		}
		this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
		this.timeSinceLoudnessChange += this.deltaTime;
	}

	// Token: 0x04004A0C RID: 18956
	private bool isSpeaking;

	// Token: 0x04004A0D RID: 18957
	private float loudness;

	// Token: 0x04004A0E RID: 18958
	[SerializeField]
	private float normalizedMax = 0.175f;

	// Token: 0x04004A0F RID: 18959
	private bool isMicEnabled;

	// Token: 0x04004A10 RID: 18960
	private RigContainer rigContainer;

	// Token: 0x04004A11 RID: 18961
	private Speaker speaker;

	// Token: 0x04004A12 RID: 18962
	private SpeakerVoiceToLoudness speakerVoiceToLoudness;

	// Token: 0x04004A13 RID: 18963
	private Recorder recorder;

	// Token: 0x04004A14 RID: 18964
	private VoiceToLoudness voiceToLoudness;

	// Token: 0x04004A15 RID: 18965
	private float smoothedLoudness;

	// Token: 0x04004A16 RID: 18966
	private float lastLoudness;

	// Token: 0x04004A17 RID: 18967
	private float timeSinceLoudnessChange;

	// Token: 0x04004A18 RID: 18968
	private float loudnessUpdateCheckRate = 0.2f;

	// Token: 0x04004A19 RID: 18969
	private float loudnessBlendStrength = 2f;

	// Token: 0x04004A1A RID: 18970
	private bool permission;

	// Token: 0x04004A1B RID: 18971
	private bool micConnected;

	// Token: 0x04004A1C RID: 18972
	private float timeLastUpdated;

	// Token: 0x04004A1D RID: 18973
	private float deltaTime;

	// Token: 0x04004A1E RID: 18974
	private AudioClip offlineMic;

	// Token: 0x04004A1F RID: 18975
	private float[] voiceSampleBuffer = new float[128];
}
