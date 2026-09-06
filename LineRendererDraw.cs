using System;
using UnityEngine;

// Token: 0x0200041E RID: 1054
public class LineRendererDraw : MonoBehaviour
{
	// Token: 0x0600190B RID: 6411 RVA: 0x0008E01A File Offset: 0x0008C21A
	public void SetUpLine(Transform[] points)
	{
		this.lr.positionCount = points.Length;
		this.points = points;
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x0008E034 File Offset: 0x0008C234
	private void LateUpdate()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.lr.SetPosition(i, this.points[i].position);
		}
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x0008E06D File Offset: 0x0008C26D
	public void Enable(bool enable)
	{
		this.lr.enabled = enable;
	}

	// Token: 0x04002439 RID: 9273
	public LineRenderer lr;

	// Token: 0x0400243A RID: 9274
	public Transform[] points;
}
