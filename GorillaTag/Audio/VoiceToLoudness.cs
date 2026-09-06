using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012A4 RID: 4772
	[RequireComponent(typeof(Recorder))]
	public class VoiceToLoudness : MonoBehaviour
	{
		// Token: 0x060077EA RID: 30698 RVA: 0x0026D185 File Offset: 0x0026B385
		protected void Awake()
		{
			this._recorder = base.GetComponent<Recorder>();
		}

		// Token: 0x060077EB RID: 30699 RVA: 0x0026D193 File Offset: 0x0026B393
		protected void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
		{
			this.CreateProcessVoiceData(photonVoiceCreatedParams.Voice);
		}

		// Token: 0x060077EC RID: 30700 RVA: 0x0026D1A4 File Offset: 0x0026B3A4
		private void CreateProcessVoiceData(LocalVoice voice)
		{
			LocalVoiceAudioFloat localVoiceAudioFloat = voice as LocalVoiceAudioFloat;
			if (localVoiceAudioFloat != null)
			{
				this._photonVoiceCreated = true;
				localVoiceAudioFloat.AddPostProcessor(new IProcessor<float>[]
				{
					new ProcessVoiceDataToLoudness(this)
				});
			}
		}

		// Token: 0x060077ED RID: 30701 RVA: 0x0026D1D7 File Offset: 0x0026B3D7
		private void Update()
		{
			if (this._photonVoiceCreated)
			{
				return;
			}
			if (this._recorder != null && this._recorder.Voice != null)
			{
				this.CreateProcessVoiceData(this._recorder.Voice);
			}
		}

		// Token: 0x04008820 RID: 34848
		[NonSerialized]
		public float Loudness;

		// Token: 0x04008821 RID: 34849
		private Recorder _recorder;

		// Token: 0x04008822 RID: 34850
		private bool _photonVoiceCreated;

		// Token: 0x04008823 RID: 34851
		private float _checkVoice;
	}
}
