using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001405 RID: 5125
	[ExecuteInEditMode]
	public class DrawCircle : DrawBase
	{
		// Token: 0x0600811C RID: 33052 RVA: 0x0029EF98 File Offset: 0x0029D198
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x0600811D RID: 33053 RVA: 0x0029EFC2 File Offset: 0x0029D1C2
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawCircle(base.transform.position, base.transform.rotation * Vector3.back, this.Radius, this.NumSegments, color, depthTest, style);
		}

		// Token: 0x0400921D RID: 37405
		public float Radius = 1f;

		// Token: 0x0400921E RID: 37406
		public int NumSegments = 64;
	}
}
