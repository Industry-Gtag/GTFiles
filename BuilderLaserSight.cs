using System;
using UnityEngine;

// Token: 0x02000638 RID: 1592
public class BuilderLaserSight : MonoBehaviour
{
	// Token: 0x060027B7 RID: 10167 RVA: 0x000D2C2E File Offset: 0x000D0E2E
	public void Awake()
	{
		if (this.lineRenderer == null)
		{
			this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
		}
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = false;
		}
	}

	// Token: 0x060027B8 RID: 10168 RVA: 0x000D2C64 File Offset: 0x000D0E64
	public void SetPoints(Vector3 start, Vector3 end)
	{
		this.lineRenderer.positionCount = 2;
		this.lineRenderer.SetPosition(0, start);
		this.lineRenderer.SetPosition(1, end);
	}

	// Token: 0x060027B9 RID: 10169 RVA: 0x000D2C8C File Offset: 0x000D0E8C
	public void Show(bool show)
	{
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = show;
		}
	}

	// Token: 0x04003375 RID: 13173
	public LineRenderer lineRenderer;
}
