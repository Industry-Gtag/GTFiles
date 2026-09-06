using System;
using UnityEngine;

// Token: 0x02000499 RID: 1177
public class ForceRecalculateBounds : MonoBehaviourTick
{
	// Token: 0x06001C93 RID: 7315 RVA: 0x0009AD8D File Offset: 0x00098F8D
	private void Awake()
	{
		this.skinnedMesh = base.GetComponentInChildren<SkinnedMeshRenderer>();
		this.bounds = Vector3.one * 1000f;
		this.mainCamera = Camera.main.transform;
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x0009ADC0 File Offset: 0x00098FC0
	public override void Tick()
	{
		if (this.skinnedMesh == null)
		{
			return;
		}
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main.transform;
			return;
		}
		this.skinnedMesh.bounds = new Bounds(this.mainCamera.position, this.bounds);
	}

	// Token: 0x040026AE RID: 9902
	private SkinnedMeshRenderer skinnedMesh;

	// Token: 0x040026AF RID: 9903
	private Transform mainCamera;

	// Token: 0x040026B0 RID: 9904
	private Vector3 bounds;
}
