using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000322 RID: 802
public class DevErrorSoundAnnoyer : MonoBehaviour
{
	// Token: 0x040018E9 RID: 6377
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x040018EA RID: 6378
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040018EB RID: 6379
	[SerializeField]
	private Text errorUIText;

	// Token: 0x040018EC RID: 6380
	[SerializeField]
	private Font errorFont;

	// Token: 0x040018ED RID: 6381
	public string displayedText;
}
