using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001402 RID: 5122
	[ExecuteInEditMode]
	public class DrawArrow : DrawBase
	{
		// Token: 0x06008113 RID: 33043 RVA: 0x0029ED54 File Offset: 0x0029CF54
		private void OnValidate()
		{
			this.ConeRadius = Mathf.Max(0f, this.ConeRadius);
			this.ConeHeight = Mathf.Max(0f, this.ConeHeight);
			this.StemThickness = Mathf.Max(0f, this.StemThickness);
			this.NumSegments = Mathf.Max(4, this.NumSegments);
		}

		// Token: 0x06008114 RID: 33044 RVA: 0x0029EDB8 File Offset: 0x0029CFB8
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawArrow(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), this.ConeRadius, this.ConeHeight, this.NumSegments, this.StemThickness, color, depthTest, style);
		}

		// Token: 0x0400920F RID: 37391
		public Vector3 LocalEndVector = Vector3.right;

		// Token: 0x04009210 RID: 37392
		public float ConeRadius = 0.05f;

		// Token: 0x04009211 RID: 37393
		public float ConeHeight = 0.1f;

		// Token: 0x04009212 RID: 37394
		public float StemThickness = 0.05f;

		// Token: 0x04009213 RID: 37395
		public int NumSegments = 8;
	}
}
