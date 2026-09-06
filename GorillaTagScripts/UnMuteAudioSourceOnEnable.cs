using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000FAC RID: 4012
	public class UnMuteAudioSourceOnEnable : MonoBehaviour
	{
		// Token: 0x060063E5 RID: 25573 RVA: 0x00201DFA File Offset: 0x001FFFFA
		public void Awake()
		{
			this.originalVolume = this.audioSource.volume;
		}

		// Token: 0x060063E6 RID: 25574 RVA: 0x00201E0D File Offset: 0x0020000D
		public void OnEnable()
		{
			this.audioSource.volume = this.originalVolume;
		}

		// Token: 0x060063E7 RID: 25575 RVA: 0x00201E20 File Offset: 0x00200020
		public void OnDisable()
		{
			this.audioSource.volume = 0f;
		}

		// Token: 0x04007294 RID: 29332
		public AudioSource audioSource;

		// Token: 0x04007295 RID: 29333
		public float originalVolume;
	}
}
