using System;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class LineRendererUpdateTarget : MonoBehaviourPostTick
{
	// Token: 0x06001172 RID: 4466 RVA: 0x0005DCD8 File Offset: 0x0005BED8
	public override void PostTick()
	{
		if (this.lineRenderer == null || this.targetTransform == null || this.lineRenderer.positionCount != 2)
		{
			return;
		}
		if (!this.targetTransform.gameObject.activeSelf)
		{
			this.lineRenderer.enabled = false;
			return;
		}
		this.lineRenderer.enabled = true;
		this.lineRenderer.SetPosition(0, base.transform.position);
		this.lineRenderer.SetPosition(1, this.targetTransform.position);
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x0005DD69 File Offset: 0x0005BF69
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.useWorldSpace = true;
	}

	// Token: 0x040014CB RID: 5323
	private LineRenderer lineRenderer;

	// Token: 0x040014CC RID: 5324
	public Transform targetTransform;
}
