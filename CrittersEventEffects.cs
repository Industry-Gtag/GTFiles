using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class CrittersEventEffects : MonoBehaviour
{
	// Token: 0x060001DB RID: 475 RVA: 0x0000B240 File Offset: 0x00009440
	private void Awake()
	{
		if (this.manager == null)
		{
			GTDev.LogError<string>("CrittersEventEffects missing reference to CrittersManager", null);
			return;
		}
		this.effectResponse = new Dictionary<CrittersManager.CritterEvent, GameObject>();
		for (int i = 0; i < this.eventEffects.Length; i++)
		{
			if (this.eventEffects[i].effect != null)
			{
				this.effectResponse.Add(this.eventEffects[i].eventType, this.eventEffects[i].effect);
			}
		}
		this.manager.OnCritterEventReceived += this.HandleReceivedEvent;
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000B2D8 File Offset: 0x000094D8
	private void HandleReceivedEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject;
		if (this.effectResponse.TryGetValue(eventType, out gameObject))
		{
			GameObject pooled = CrittersPool.GetPooled(gameObject);
			if (pooled.IsNotNull())
			{
				pooled.transform.position = position;
				pooled.transform.rotation = rotation;
			}
		}
	}

	// Token: 0x0400021E RID: 542
	public CrittersManager manager;

	// Token: 0x0400021F RID: 543
	public CrittersEventEffects.CrittersEventResponse[] eventEffects;

	// Token: 0x04000220 RID: 544
	private Dictionary<CrittersManager.CritterEvent, GameObject> effectResponse;

	// Token: 0x02000060 RID: 96
	[Serializable]
	public class CrittersEventResponse
	{
		// Token: 0x04000221 RID: 545
		public CrittersManager.CritterEvent eventType;

		// Token: 0x04000222 RID: 546
		public GameObject effect;
	}
}
