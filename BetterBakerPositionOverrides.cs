using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000D88 RID: 3464
public class BetterBakerPositionOverrides : MonoBehaviour
{
	// Token: 0x040066DB RID: 26331
	public List<BetterBakerPositionOverrides.OverridePosition> overridePositions;

	// Token: 0x02000D89 RID: 3465
	[Serializable]
	public struct OverridePosition
	{
		// Token: 0x040066DC RID: 26332
		public GameObject go;

		// Token: 0x040066DD RID: 26333
		public Transform bakingTransform;

		// Token: 0x040066DE RID: 26334
		public Transform gameTransform;
	}
}
