using System;
using UnityEngine;

// Token: 0x02000DD8 RID: 3544
[CreateAssetMenu(menuName = "Gorilla Tag/SoundBankSO")]
public class SoundBankSO : ScriptableObject
{
	// Token: 0x040067BC RID: 26556
	public AudioClip[] sounds;

	// Token: 0x040067BD RID: 26557
	public Vector2 volumeRange = new Vector2(0.5f, 0.5f);

	// Token: 0x040067BE RID: 26558
	public Vector2 pitchRange = new Vector2(1f, 1f);
}
