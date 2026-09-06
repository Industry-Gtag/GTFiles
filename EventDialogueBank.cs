using System;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class EventDialogueBank : MonoBehaviour
{
	// Token: 0x060003E6 RID: 998 RVA: 0x00017658 File Offset: 0x00015858
	private void Awake()
	{
		for (int i = 0; i < this.bank.Length; i++)
		{
			if (this.bank[i].audioSource == null)
			{
				this.bank[i].audioSource = this.defaultAudioSource;
			}
			this.bank[i].audioSource.playOnAwake = false;
			this.bank[i].audioSource.gameObject.SetActive(true);
		}
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x000176DC File Offset: 0x000158DC
	private void LateUpdate()
	{
		if (this._index == Mathf.FloorToInt(this.index) - 1)
		{
			return;
		}
		this._index = Mathf.FloorToInt(this.index) - 1;
		if (this._index < 0 || this._index >= this.bank.Length || this.bank[this._index].audioClip == null || this.bank[this._index].audioSource == null)
		{
			return;
		}
		if (this.bank[this._index].audioSource.isPlaying)
		{
			this.bank[this._index].audioSource.Stop();
		}
		this.bank[this._index].audioSource.clip = this.bank[this._index].audioClip;
		this.bank[this._index].audioSource.Play();
	}

	// Token: 0x04000454 RID: 1108
	[SerializeField]
	private EventDialogueBank.EventDialogueBankEntry[] bank;

	// Token: 0x04000455 RID: 1109
	[SerializeField]
	private AudioSource defaultAudioSource;

	// Token: 0x04000456 RID: 1110
	[SerializeField]
	private float index;

	// Token: 0x04000457 RID: 1111
	private int _index = -1;

	// Token: 0x0200009D RID: 157
	[Serializable]
	public struct EventDialogueBankEntry
	{
		// Token: 0x04000458 RID: 1112
		public AudioClip audioClip;

		// Token: 0x04000459 RID: 1113
		public AudioSource audioSource;
	}
}
