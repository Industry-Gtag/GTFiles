using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004AB RID: 1195
public class BalloonString : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001D12 RID: 7442 RVA: 0x0009DBC4 File Offset: 0x0009BDC4
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.vertices = new List<Vector3>(this.numSegments + 1);
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.vertices.Add(this.startPositionXf.position);
			int num = this.vertices.Count - 2;
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)((i + 1) / (this.vertices.Count - 1));
				Vector3 vector = Vector3.Lerp(this.startPositionXf.position, this.endPositionXf.position, num2);
				this.vertices.Add(vector);
			}
			this.vertices.Add(this.endPositionXf.position);
		}
	}

	// Token: 0x06001D13 RID: 7443 RVA: 0x0009DC94 File Offset: 0x0009BE94
	private void UpdateDynamics()
	{
		this.vertices[0] = this.startPositionXf.position;
		this.vertices[this.vertices.Count - 1] = this.endPositionXf.position;
	}

	// Token: 0x06001D14 RID: 7444 RVA: 0x0009DCD0 File Offset: 0x0009BED0
	private void UpdateRenderPositions()
	{
		this.lineRenderer.SetPosition(0, this.startPositionXf.transform.position);
		this.lineRenderer.SetPosition(1, this.endPositionXf.transform.position);
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x0001212B File Offset: 0x0001032B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x0009DD0A File Offset: 0x0009BF0A
	public void SliceUpdate()
	{
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.UpdateDynamics();
			this.UpdateRenderPositions();
		}
	}

	// Token: 0x0400274C RID: 10060
	public Transform startPositionXf;

	// Token: 0x0400274D RID: 10061
	public Transform endPositionXf;

	// Token: 0x0400274E RID: 10062
	private List<Vector3> vertices;

	// Token: 0x0400274F RID: 10063
	public int numSegments = 1;

	// Token: 0x04002750 RID: 10064
	private LineRenderer lineRenderer;
}
