using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001406 RID: 5126
	[ExecuteInEditMode]
	public class DrawLine : DrawBase
	{
		// Token: 0x0600811F RID: 33055 RVA: 0x0029F013 File Offset: 0x0029D213
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
		}

		// Token: 0x06008120 RID: 33056 RVA: 0x0029F023 File Offset: 0x0029D223
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawLine(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), color, depthTest);
		}

		// Token: 0x0400921F RID: 37407
		public Vector3 LocalEndVector = Vector3.right;
	}
}
