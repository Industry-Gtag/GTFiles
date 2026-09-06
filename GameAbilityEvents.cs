using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000733 RID: 1843
[Serializable]
public class GameAbilityEvents
{
	// Token: 0x06002EC2 RID: 11970 RVA: 0x000FFC5C File Offset: 0x000FDE5C
	public void Reset()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].Reset();
		}
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000FFC90 File Offset: 0x000FDE90
	public void OnAbilityStart(float abilityTime, AudioSource audioSource)
	{
		this.startEvent.TryPlay(abilityTime, (this.startEvent.sound.audioSource == null) ? audioSource : this.startEvent.sound.audioSource);
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x000FFCC9 File Offset: 0x000FDEC9
	public void OnAbilityStop(float abilityTime, AudioSource audioSource)
	{
		this.stopEvent.TryPlay(abilityTime, (this.stopEvent.sound.audioSource == null) ? audioSource : this.stopEvent.sound.audioSource);
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x000FFD04 File Offset: 0x000FDF04
	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].TryPlay(abilityTime, (this.events[i].sound.audioSource == null) ? audioSource : this.events[i].sound.audioSource);
		}
	}

	// Token: 0x04003BF1 RID: 15345
	public GameAbilityEvent startEvent;

	// Token: 0x04003BF2 RID: 15346
	public GameAbilityEvent stopEvent;

	// Token: 0x04003BF3 RID: 15347
	public List<GameAbilityEvent> events;
}
