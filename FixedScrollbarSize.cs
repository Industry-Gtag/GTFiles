using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000A2 RID: 162
public class FixedScrollbarSize : MonoBehaviour
{
	// Token: 0x06000401 RID: 1025 RVA: 0x00017E07 File Offset: 0x00016007
	private void OnEnable()
	{
		this.EnforceScrollbarSize();
		CanvasUpdateRegistry.instance.Equals(null);
		Canvas.willRenderCanvases += this.EnforceScrollbarSize;
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00017E2C File Offset: 0x0001602C
	private void OnDisable()
	{
		Canvas.willRenderCanvases -= this.EnforceScrollbarSize;
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00017E40 File Offset: 0x00016040
	private void EnforceScrollbarSize()
	{
		if (this.ScrollRect.horizontalScrollbar && this.ScrollRect.horizontalScrollbar.size != this.HorizontalBarSize)
		{
			this.ScrollRect.horizontalScrollbar.size = this.HorizontalBarSize;
		}
		if (this.ScrollRect.verticalScrollbar && this.ScrollRect.verticalScrollbar.size != this.VerticalBarSize)
		{
			this.ScrollRect.verticalScrollbar.size = this.VerticalBarSize;
		}
	}

	// Token: 0x04000471 RID: 1137
	public ScrollRect ScrollRect;

	// Token: 0x04000472 RID: 1138
	public float HorizontalBarSize = 0.2f;

	// Token: 0x04000473 RID: 1139
	public float VerticalBarSize = 0.2f;
}
