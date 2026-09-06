using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x0200123B RID: 4667
	public class GravityZoneProxy : MonoBehaviour
	{
		// Token: 0x0600764D RID: 30285 RVA: 0x002664C7 File Offset: 0x002646C7
		private void OnTriggerEnter(Collider other)
		{
			this.DoBehaviour(this.OnEnter, other);
		}

		// Token: 0x0600764E RID: 30286 RVA: 0x002664D6 File Offset: 0x002646D6
		private void OnTriggerExit(Collider other)
		{
			this.DoBehaviour(this.OnExit, other);
		}

		// Token: 0x0600764F RID: 30287 RVA: 0x002664E8 File Offset: 0x002646E8
		private void DoBehaviour(GravityZoneProxy.ProxyBehaviour behaviour, Collider other)
		{
			if (this.zone == null)
			{
				return;
			}
			ValueTuple<bool, MonkeGravityController> monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
			if (!monkeGravityController.Item1)
			{
				return;
			}
			this.ApplyBehaviour(behaviour, monkeGravityController.Item2);
		}

		// Token: 0x06007650 RID: 30288 RVA: 0x00266524 File Offset: 0x00264724
		private void ApplyBehaviour(GravityZoneProxy.ProxyBehaviour behaviour, MonkeGravityController target)
		{
			if (this.zone == null)
			{
				return;
			}
			switch (behaviour)
			{
			case GravityZoneProxy.ProxyBehaviour.None:
				break;
			case GravityZoneProxy.ProxyBehaviour.EnterZone:
				this.zone.AddTarget(target, this.delay);
				return;
			case GravityZoneProxy.ProxyBehaviour.ExitZone:
				this.zone.RemoveTarget(target, this.delay);
				break;
			default:
				return;
			}
		}

		// Token: 0x040085DA RID: 34266
		[SerializeField]
		private BasicGravityZone zone;

		// Token: 0x040085DB RID: 34267
		[SerializeField]
		private GravityZoneProxy.ProxyBehaviour OnEnter = GravityZoneProxy.ProxyBehaviour.EnterZone;

		// Token: 0x040085DC RID: 34268
		[SerializeField]
		private GravityZoneProxy.ProxyBehaviour OnExit = GravityZoneProxy.ProxyBehaviour.ExitZone;

		// Token: 0x040085DD RID: 34269
		[SerializeField]
		private float delay;

		// Token: 0x0200123C RID: 4668
		public enum ProxyBehaviour
		{
			// Token: 0x040085DF RID: 34271
			None,
			// Token: 0x040085E0 RID: 34272
			EnterZone,
			// Token: 0x040085E1 RID: 34273
			ExitZone
		}
	}
}
