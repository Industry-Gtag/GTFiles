using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001407 RID: 5127
	[ExecuteInEditMode]
	public class DrawSphere : DrawBase
	{
		// Token: 0x06008122 RID: 33058 RVA: 0x0029F06B File Offset: 0x0029D26B
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.LatSegments = Mathf.Max(0, this.LatSegments);
		}

		// Token: 0x06008123 RID: 33059 RVA: 0x0029F098 File Offset: 0x0029D298
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawSphere(base.transform.position, base.transform.rotation, this.Radius * base.transform.lossyScale.x, this.LatSegments, this.LongSegments, color, depthTest, style);
		}

		// Token: 0x04009220 RID: 37408
		public float Radius = 1f;

		// Token: 0x04009221 RID: 37409
		public int LatSegments = 12;

		// Token: 0x04009222 RID: 37410
		public int LongSegments = 12;
	}
}
