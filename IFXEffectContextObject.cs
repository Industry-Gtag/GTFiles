using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000D23 RID: 3363
public interface IFXEffectContextObject
{
	// Token: 0x170007DC RID: 2012
	// (get) Token: 0x06005356 RID: 21334
	List<int> PrefabPoolIds { get; }

	// Token: 0x170007DD RID: 2013
	// (get) Token: 0x06005357 RID: 21335
	Vector3 Position { get; }

	// Token: 0x170007DE RID: 2014
	// (get) Token: 0x06005358 RID: 21336
	Quaternion Rotation { get; }

	// Token: 0x170007DF RID: 2015
	// (get) Token: 0x06005359 RID: 21337
	AudioSource SoundSource { get; }

	// Token: 0x170007E0 RID: 2016
	// (get) Token: 0x0600535A RID: 21338
	AudioClip Sound { get; }

	// Token: 0x170007E1 RID: 2017
	// (get) Token: 0x0600535B RID: 21339
	float Volume { get; }

	// Token: 0x170007E2 RID: 2018
	// (get) Token: 0x0600535C RID: 21340
	float Pitch { get; }

	// Token: 0x0600535D RID: 21341
	void OnTriggerActions();

	// Token: 0x0600535E RID: 21342
	void OnPlayVisualFX(int effectID, GameObject effect);

	// Token: 0x0600535F RID: 21343
	void OnPlaySoundFX(AudioSource audioSource);
}
