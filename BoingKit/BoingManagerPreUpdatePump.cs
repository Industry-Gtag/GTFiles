using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200143C RID: 5180
	public class BoingManagerPreUpdatePump : MonoBehaviour
	{
		// Token: 0x0600829E RID: 33438 RVA: 0x002A9EF3 File Offset: 0x002A80F3
		private void FixedUpdate()
		{
			this.TryPump();
		}

		// Token: 0x0600829F RID: 33439 RVA: 0x002A9EF3 File Offset: 0x002A80F3
		private void Update()
		{
			this.TryPump();
		}

		// Token: 0x060082A0 RID: 33440 RVA: 0x002A9EFB File Offset: 0x002A80FB
		private void TryPump()
		{
			if (this.m_lastPumpedFrame >= Time.frameCount)
			{
				return;
			}
			if (this.m_lastPumpedFrame >= 0)
			{
				this.DoPump();
			}
			this.m_lastPumpedFrame = Time.frameCount;
		}

		// Token: 0x060082A1 RID: 33441 RVA: 0x002A9F25 File Offset: 0x002A8125
		private void DoPump()
		{
			BoingManager.RestoreBehaviors();
			BoingManager.RestoreReactors();
			BoingManager.RestoreBones();
			BoingManager.DispatchReactorFieldCompute();
		}

		// Token: 0x0400937A RID: 37754
		private int m_lastPumpedFrame = -1;
	}
}
