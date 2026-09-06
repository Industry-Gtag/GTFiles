using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B12 RID: 2834
public class ParticleCollisionListener : MonoBehaviour
{
	// Token: 0x0600487F RID: 18559 RVA: 0x00185ECC File Offset: 0x001840CC
	private void Awake()
	{
		this._events = new List<ParticleCollisionEvent>();
	}

	// Token: 0x06004880 RID: 18560 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnCollisionEvent(ParticleCollisionEvent ev)
	{
	}

	// Token: 0x06004881 RID: 18561 RVA: 0x00185EDC File Offset: 0x001840DC
	public void OnParticleCollision(GameObject other)
	{
		int collisionEvents = this.target.GetCollisionEvents(other, this._events);
		for (int i = 0; i < collisionEvents; i++)
		{
			this.OnCollisionEvent(this._events[i]);
		}
	}

	// Token: 0x04005B0D RID: 23309
	public ParticleSystem target;

	// Token: 0x04005B0E RID: 23310
	[SerializeReference]
	private List<ParticleCollisionEvent> _events = new List<ParticleCollisionEvent>();
}
