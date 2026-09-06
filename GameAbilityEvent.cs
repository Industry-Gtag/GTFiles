using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000732 RID: 1842
[Serializable]
public class GameAbilityEvent
{
	// Token: 0x06002EBF RID: 11967 RVA: 0x000FFBE8 File Offset: 0x000FDDE8
	public void Reset()
	{
		this.played = false;
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x000FFBF4 File Offset: 0x000FDDF4
	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		if (abilityTime < this.time || this.played)
		{
			return;
		}
		this.played = true;
		if (this.sound.IsValid())
		{
			this.sound.Play(audioSource);
		}
		for (int i = 0; i < this.triggerEvent.Count; i++)
		{
			this.triggerEvent[i].Invoke();
		}
	}

	// Token: 0x04003BED RID: 15341
	public float time;

	// Token: 0x04003BEE RID: 15342
	public AbilitySound sound;

	// Token: 0x04003BEF RID: 15343
	public List<UnityEvent> triggerEvent;

	// Token: 0x04003BF0 RID: 15344
	[NonSerialized]
	public bool played;
}
