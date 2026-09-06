using System;
using System.Collections.Generic;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Audio
{
	// Token: 0x020012AA RID: 4778
	public class GTSpeaker : Speaker
	{
		// Token: 0x0600780E RID: 30734 RVA: 0x0026DD48 File Offset: 0x0026BF48
		public void Start()
		{
			LoudSpeakerNetwork componentInChildren = base.transform.root.GetComponentInChildren<LoudSpeakerNetwork>();
			if (componentInChildren != null)
			{
				this.AddExternalAudioSources(componentInChildren.SpeakerSources);
			}
		}

		// Token: 0x0600780F RID: 30735 RVA: 0x0026DD7B File Offset: 0x0026BF7B
		public void AddExternalAudioSources(AudioSource[] audioSources)
		{
			if (this._initializedExternalAudioSources)
			{
				return;
			}
			this._externalAudioSources = audioSources;
			this.InitializeExternalAudioSources();
			if (this._audioOutputStarted)
			{
				this.ExternalAudioOutputStart(this._frequency, this._channels, this._frameSamplesPerChannel);
			}
		}

		// Token: 0x06007810 RID: 30736 RVA: 0x0026DDB3 File Offset: 0x0026BFB3
		protected override void Initialize()
		{
			if (base.IsInitialized)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Already initialized.", Array.Empty<object>());
				}
				return;
			}
			base.Initialize();
		}

		// Token: 0x06007811 RID: 30737 RVA: 0x0026DDE8 File Offset: 0x0026BFE8
		private void InitializeExternalAudioSources()
		{
			this._initializedExternalAudioSources = true;
			this._externalAudioOutputs = new List<IAudioOut<float>>();
			AudioOutDelayControl.PlayDelayConfig playDelayConfig = new AudioOutDelayControl.PlayDelayConfig
			{
				Low = this.playbackDelaySettings.MinDelaySoft,
				High = this.playbackDelaySettings.MaxDelaySoft,
				Max = this.playbackDelaySettings.MaxDelayHard
			};
			foreach (AudioSource audioSource in this._externalAudioSources)
			{
				this._externalAudioOutputs.Add(this.GetAudioOutFactoryFromSource(audioSource, playDelayConfig)());
			}
		}

		// Token: 0x06007812 RID: 30738 RVA: 0x0026DE71 File Offset: 0x0026C071
		private Func<IAudioOut<float>> GetAudioOutFactoryFromSource(AudioSource source, AudioOutDelayControl.PlayDelayConfig pdc)
		{
			return () => new UnityAudioOut(source, pdc, this.Logger, string.Empty, this.Logger.IsDebugEnabled);
		}

		// Token: 0x06007813 RID: 30739 RVA: 0x0026DE98 File Offset: 0x0026C098
		protected override void OnAudioFrame(FrameOut<float> frame)
		{
			base.OnAudioFrame(frame);
			if (this.BroadcastExternal)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					audioOut.Push(frame.Buf);
					if (frame.EndOfStream)
					{
						audioOut.Flush();
					}
				}
			}
		}

		// Token: 0x06007814 RID: 30740 RVA: 0x0026DF10 File Offset: 0x0026C110
		protected override void AudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			this._audioOutputStarted = true;
			this._frequency = frequency;
			this._channels = channels;
			this._frameSamplesPerChannel = frameSamplesPerChannel;
			base.AudioOutputStart(frequency, channels, frameSamplesPerChannel);
			this.ExternalAudioOutputStart(frequency, channels, frameSamplesPerChannel);
		}

		// Token: 0x06007815 RID: 30741 RVA: 0x0026DF40 File Offset: 0x0026C140
		private void ExternalAudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					if (!audioOut.IsPlaying)
					{
						audioOut.Start(frequency, channels, frameSamplesPerChannel);
						audioOut.ToggleAudioSource(false);
					}
				}
			}
		}

		// Token: 0x06007816 RID: 30742 RVA: 0x0026DFAC File Offset: 0x0026C1AC
		protected override void AudioOutputStop()
		{
			this._audioOutputStarted = false;
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					audioOut.Stop();
				}
			}
			base.AudioOutputStop();
		}

		// Token: 0x06007817 RID: 30743 RVA: 0x0026E014 File Offset: 0x0026C214
		protected override void AudioOutputService()
		{
			base.AudioOutputService();
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					if (!audioOut.IsPlaying)
					{
						audioOut.Service();
					}
				}
			}
		}

		// Token: 0x06007818 RID: 30744 RVA: 0x0026E07C File Offset: 0x0026C27C
		public void ToggleAudioSource(bool toggle)
		{
			if (this._externalAudioOutputs == null)
			{
				return;
			}
			foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
			{
				audioOut.ToggleAudioSource(toggle);
			}
		}

		// Token: 0x04008843 RID: 34883
		[FormerlySerializedAs("UseExternalAudioSources")]
		public bool BroadcastExternal;

		// Token: 0x04008844 RID: 34884
		[SerializeField]
		private AudioSource[] _externalAudioSources;

		// Token: 0x04008845 RID: 34885
		private List<IAudioOut<float>> _externalAudioOutputs;

		// Token: 0x04008846 RID: 34886
		private int _frequency;

		// Token: 0x04008847 RID: 34887
		private int _channels;

		// Token: 0x04008848 RID: 34888
		private int _frameSamplesPerChannel;

		// Token: 0x04008849 RID: 34889
		private bool _initializedExternalAudioSources;

		// Token: 0x0400884A RID: 34890
		private bool _audioOutputStarted;
	}
}
