using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001404 RID: 5124
	[ExecuteInEditMode]
	public class DrawBox : DrawBase
	{
		// Token: 0x06008119 RID: 33049 RVA: 0x0029EEC8 File Offset: 0x0029D0C8
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x0600811A RID: 33050 RVA: 0x0029EEF4 File Offset: 0x0029D0F4
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion quaternion = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * quaternion * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04009219 RID: 37401
		public float Radius = 1f;

		// Token: 0x0400921A RID: 37402
		public int NumSegments = 64;

		// Token: 0x0400921B RID: 37403
		public float StartAngle;

		// Token: 0x0400921C RID: 37404
		public float ArcAngle = 60f;
	}
}
