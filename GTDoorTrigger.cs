using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x0200034B RID: 843
public class GTDoorTrigger : MonoBehaviour
{
	// Token: 0x17000210 RID: 528
	// (get) Token: 0x060014C8 RID: 5320 RVA: 0x0006F11D File Offset: 0x0006D31D
	public int overlapCount
	{
		get
		{
			return this.overlappingColliders.Count;
		}
	}

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x060014C9 RID: 5321 RVA: 0x0006F12A File Offset: 0x0006D32A
	public bool TriggeredThisFrame
	{
		get
		{
			return this.lastTriggeredFrame == Time.frameCount;
		}
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x0006F13C File Offset: 0x0006D33C
	public void ValidateOverlappingColliders()
	{
		for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
		{
			if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
			{
				this.overlappingColliders.RemoveAt(i);
			}
		}
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x0006F1AC File Offset: 0x0006D3AC
	private void OnTriggerEnter(Collider other)
	{
		if (!this.overlappingColliders.Contains(other))
		{
			this.overlappingColliders.Add(other);
		}
		this.lastTriggeredFrame = Time.frameCount;
		this.TriggeredEvent.Invoke();
		if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
		{
			this.timeline.Play();
		}
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x0006F230 File Offset: 0x0006D430
	private void OnTriggerExit(Collider other)
	{
		this.overlappingColliders.Remove(other);
	}

	// Token: 0x04001983 RID: 6531
	[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
	public PlayableDirector timeline;

	// Token: 0x04001984 RID: 6532
	private int lastTriggeredFrame = -1;

	// Token: 0x04001985 RID: 6533
	private List<Collider> overlappingColliders = new List<Collider>(20);

	// Token: 0x04001986 RID: 6534
	internal UnityEvent TriggeredEvent = new UnityEvent();
}
