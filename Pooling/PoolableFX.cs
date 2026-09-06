using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Pooling
{
	// Token: 0x02000F1C RID: 3868
	public class PoolableFX : MonoBehaviour, IPoolable<PoolableFX>
	{
		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x06005EE2 RID: 24290 RVA: 0x001E34F8 File Offset: 0x001E16F8
		// (set) Token: 0x06005EE3 RID: 24291 RVA: 0x001E3500 File Offset: 0x001E1700
		public IObjectPool<PoolableFX> Pool { get; set; }

		// Token: 0x06005EE4 RID: 24292 RVA: 0x001E350C File Offset: 0x001E170C
		private void Reset()
		{
			this.particles = base.GetComponent<ParticleSystem>();
			if (this.particles)
			{
				ParticleSystem.MainModule main = this.particles.main;
				main.playOnAwake = false;
				main.stopAction = ParticleSystemStopAction.Callback;
			}
		}

		// Token: 0x06005EE5 RID: 24293 RVA: 0x001E354E File Offset: 0x001E174E
		public void Stop()
		{
			this.particles.Stop();
		}

		// Token: 0x06005EE6 RID: 24294 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnCreate()
		{
		}

		// Token: 0x06005EE7 RID: 24295 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPreGet()
		{
		}

		// Token: 0x06005EE8 RID: 24296 RVA: 0x001E355B File Offset: 0x001E175B
		public void OnPostGet()
		{
			if (this.particles)
			{
				this.particles.Play();
			}
		}

		// Token: 0x06005EE9 RID: 24297 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnRelease()
		{
		}

		// Token: 0x06005EEA RID: 24298 RVA: 0x001E3575 File Offset: 0x001E1775
		private void OnParticleSystemStopped()
		{
			this.Release<PoolableFX>();
		}

		// Token: 0x04006D91 RID: 28049
		[SerializeField]
		private ParticleSystem particles;
	}
}
