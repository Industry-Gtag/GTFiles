using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x020012BE RID: 4798
	public sealed class ZoneLiquidEffectable : MonoBehaviour
	{
		// Token: 0x0600785D RID: 30813 RVA: 0x002705F3 File Offset: 0x0026E7F3
		private void Awake()
		{
			this.childRenderers = base.GetComponentsInChildren<Renderer>(false);
		}

		// Token: 0x0600785E RID: 30814 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnEnable()
		{
		}

		// Token: 0x0600785F RID: 30815 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnDisable()
		{
		}

		// Token: 0x0400889B RID: 34971
		public float radius = 1f;

		// Token: 0x0400889C RID: 34972
		[NonSerialized]
		public bool inLiquidVolume;

		// Token: 0x0400889D RID: 34973
		[NonSerialized]
		public bool wasInLiquidVolume;

		// Token: 0x0400889E RID: 34974
		[NonSerialized]
		public Renderer[] childRenderers;
	}
}
