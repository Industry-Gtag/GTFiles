using System;
using UnityEngine;

// Token: 0x02000937 RID: 2359
public class XfToXfLine : MonoBehaviour
{
	// Token: 0x06003DC9 RID: 15817 RVA: 0x0014F133 File Offset: 0x0014D333
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06003DCA RID: 15818 RVA: 0x0014F141 File Offset: 0x0014D341
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.pt0.transform.position);
		this.lineRenderer.SetPosition(1, this.pt1.transform.position);
	}

	// Token: 0x04004E7B RID: 20091
	public Transform pt0;

	// Token: 0x04004E7C RID: 20092
	public Transform pt1;

	// Token: 0x04004E7D RID: 20093
	private LineRenderer lineRenderer;
}
