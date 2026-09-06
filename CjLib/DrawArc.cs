using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001401 RID: 5121
	[ExecuteInEditMode]
	public class DrawArc : DrawBase
	{
		// Token: 0x06008110 RID: 33040 RVA: 0x0029EC77 File Offset: 0x0029CE77
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06008111 RID: 33041 RVA: 0x0029ECB0 File Offset: 0x0029CEB0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion quaternion = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * quaternion * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x0400920B RID: 37387
		public float Radius = 1f;

		// Token: 0x0400920C RID: 37388
		public int NumSegments = 64;

		// Token: 0x0400920D RID: 37389
		public float StartAngle;

		// Token: 0x0400920E RID: 37390
		public float ArcAngle = 60f;
	}
}
