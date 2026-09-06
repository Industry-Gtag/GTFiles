using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012A6 RID: 4774
	public class DuplicateAudioSource : MonoBehaviour
	{
		// Token: 0x060077F2 RID: 30706 RVA: 0x0026D25E File Offset: 0x0026B45E
		public void SetTargetAudioSource(AudioSource target)
		{
			this.TargetAudioSource = target;
			this.StartDuplicating();
		}

		// Token: 0x060077F3 RID: 30707 RVA: 0x0026D270 File Offset: 0x0026B470
		[ContextMenu("Start Duplicating")]
		public void StartDuplicating()
		{
			this._isDuplicating = true;
			this._audioSource.loop = this.TargetAudioSource.loop;
			this._audioSource.clip = this.TargetAudioSource.clip;
			if (this.TargetAudioSource.isPlaying)
			{
				this._audioSource.Play();
			}
		}

		// Token: 0x060077F4 RID: 30708 RVA: 0x0026D2C8 File Offset: 0x0026B4C8
		[ContextMenu("Stop Duplicating")]
		public void StopDuplicating()
		{
			this._isDuplicating = false;
			this._audioSource.Stop();
		}

		// Token: 0x060077F5 RID: 30709 RVA: 0x0026D2DC File Offset: 0x0026B4DC
		public void LateUpdate()
		{
			if (this._isDuplicating)
			{
				if (this.TargetAudioSource.isPlaying && !this._audioSource.isPlaying)
				{
					this._audioSource.Play();
					return;
				}
				if (!this.TargetAudioSource.isPlaying && this._audioSource.isPlaying)
				{
					this._audioSource.Stop();
				}
			}
		}

		// Token: 0x04008825 RID: 34853
		public AudioSource TargetAudioSource;

		// Token: 0x04008826 RID: 34854
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04008827 RID: 34855
		[SerializeField]
		private bool _isDuplicating;
	}
}
