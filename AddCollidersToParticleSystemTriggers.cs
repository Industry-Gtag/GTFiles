using System;
using UnityEngine;

// Token: 0x020005EF RID: 1519
public class AddCollidersToParticleSystemTriggers : MonoBehaviour
{
	// Token: 0x060025DA RID: 9690 RVA: 0x000C8DC0 File Offset: 0x000C6FC0
	private void Update()
	{
		this.count = 0;
		while (this.count < 6)
		{
			this.index++;
			if (this.index >= this.collidersToAdd.Length)
			{
				if (BetterDayNightManager.instance.collidersToAddToWeatherSystems.Count >= this.index - this.collidersToAdd.Length)
				{
					this.index = 0;
				}
				else
				{
					this.particleSystemToUpdate.trigger.SetCollider(this.count, BetterDayNightManager.instance.collidersToAddToWeatherSystems[this.index - this.collidersToAdd.Length]);
				}
			}
			if (this.index < this.collidersToAdd.Length)
			{
				this.particleSystemToUpdate.trigger.SetCollider(this.count, this.collidersToAdd[this.index]);
			}
			this.count++;
		}
	}

	// Token: 0x04003174 RID: 12660
	public Collider[] collidersToAdd;

	// Token: 0x04003175 RID: 12661
	public ParticleSystem particleSystemToUpdate;

	// Token: 0x04003176 RID: 12662
	private int count;

	// Token: 0x04003177 RID: 12663
	private int index;
}
