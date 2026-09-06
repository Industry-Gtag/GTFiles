using System;
using UnityEngine;
using UnityEngine.Events;

namespace PerformanceSystems
{
	// Token: 0x02000F27 RID: 3879
	public interface ILod
	{
		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x06005F14 RID: 24340
		int CurrentLod { get; }

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x06005F15 RID: 24341
		Vector3 Position { get; }

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06005F16 RID: 24342
		float[] LodRanges { get; }

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x06005F17 RID: 24343
		UnityEvent[] OnLodRangeEvents { get; }

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06005F18 RID: 24344
		UnityEvent OnCulledEvent { get; }

		// Token: 0x06005F19 RID: 24345
		void UpdateLod(Vector3 refPos);
	}
}
