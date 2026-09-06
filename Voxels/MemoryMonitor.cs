using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Voxels
{
	// Token: 0x020013B9 RID: 5049
	public class MemoryMonitor : MonoBehaviour
	{
		// Token: 0x06007E2B RID: 32299 RVA: 0x00295C06 File Offset: 0x00293E06
		private void Update()
		{
			if (Time.time < this.nextLog)
			{
				return;
			}
			this.nextLog = Time.time + this.logInterval;
		}

		// Token: 0x06007E2C RID: 32300 RVA: 0x00295C28 File Offset: 0x00293E28
		private void Collect()
		{
			Profiler.GetMonoUsedSizeLong();
			GC.GetTotalMemory(false);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.GetTotalMemory(false);
			Profiler.GetMonoUsedSizeLong();
			GC.GetTotalMemory(false);
		}

		// Token: 0x040090DE RID: 37086
		public float logInterval = 1f;

		// Token: 0x040090DF RID: 37087
		private float nextLog;
	}
}
