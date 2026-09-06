using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001403 RID: 5123
	public abstract class DrawBase : MonoBehaviour
	{
		// Token: 0x06008116 RID: 33046 RVA: 0x0029EE4C File Offset: 0x0029D04C
		private void Update()
		{
			if (this.Style != DebugUtil.Style.Wireframe)
			{
				this.Draw(this.ShadededColor, this.Style, this.DepthTest);
			}
			if (this.Style == DebugUtil.Style.Wireframe || this.Wireframe)
			{
				this.Draw(this.WireframeColor, DebugUtil.Style.Wireframe, this.DepthTest);
			}
		}

		// Token: 0x06008117 RID: 33047
		protected abstract void Draw(Color color, DebugUtil.Style style, bool depthTest);

		// Token: 0x04009214 RID: 37396
		public Color WireframeColor = Color.white;

		// Token: 0x04009215 RID: 37397
		public Color ShadededColor = Color.gray;

		// Token: 0x04009216 RID: 37398
		public bool Wireframe;

		// Token: 0x04009217 RID: 37399
		public DebugUtil.Style Style = DebugUtil.Style.FlatShaded;

		// Token: 0x04009218 RID: 37400
		public bool DepthTest = true;
	}
}
