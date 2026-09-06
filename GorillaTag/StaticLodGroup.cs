using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011FB RID: 4603
	[DefaultExecutionOrder(2000)]
	public class StaticLodGroup : MonoBehaviour, IGorillaSimpleBackgroundWorker
	{
		// Token: 0x060074C9 RID: 29897 RVA: 0x0025EC61 File Offset: 0x0025CE61
		protected void OnEnable()
		{
			if (this.initialized)
			{
				StaticLodManager.SetEnabled(this.index, true);
				return;
			}
			GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
		}

		// Token: 0x060074CA RID: 29898 RVA: 0x0025EC7E File Offset: 0x0025CE7E
		protected void OnDisable()
		{
			if (this.initialized)
			{
				StaticLodManager.SetEnabled(this.index, false);
			}
		}

		// Token: 0x060074CB RID: 29899 RVA: 0x0025EC94 File Offset: 0x0025CE94
		private void OnDestroy()
		{
			if (this.initialized)
			{
				StaticLodManager.Unregister(this.index);
			}
		}

		// Token: 0x060074CC RID: 29900 RVA: 0x0025ECA9 File Offset: 0x0025CEA9
		public void SimpleWork()
		{
			if (this.initialized)
			{
				return;
			}
			this.index = StaticLodManager.Register(this);
			StaticLodManager.SetEnabled(this.index, true);
			this.initialized = true;
		}

		// Token: 0x04008474 RID: 33908
		public const int k_monoDefaultExecutionOrder = 2000;

		// Token: 0x04008475 RID: 33909
		private int index;

		// Token: 0x04008476 RID: 33910
		public float collisionEnableDistance = 3f;

		// Token: 0x04008477 RID: 33911
		public float uiFadeDistanceMax = 10f;

		// Token: 0x04008478 RID: 33912
		private bool initialized;
	}
}
