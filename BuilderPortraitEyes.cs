using System;
using UnityEngine;

// Token: 0x0200061F RID: 1567
public class BuilderPortraitEyes : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600271B RID: 10011 RVA: 0x000CEF4A File Offset: 0x000CD14A
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.scale = base.transform.lossyScale.x;
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x000CEF69 File Offset: 0x000CD169
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.eyes.transform.position = this.eyeCenter.transform.position;
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x000CEF94 File Offset: 0x000CD194
	public void SliceUpdate()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		Vector3 vector = Vector3.ClampMagnitude(Vector3.ProjectOnPlane(GorillaTagger.Instance.headCollider.transform.position - this.eyeCenter.position, this.eyeCenter.forward), this.moveRadius * this.scale);
		this.eyes.transform.position = this.eyeCenter.position + vector;
	}

	// Token: 0x040032A2 RID: 12962
	[SerializeField]
	private Transform eyeCenter;

	// Token: 0x040032A3 RID: 12963
	[SerializeField]
	private GameObject eyes;

	// Token: 0x040032A4 RID: 12964
	[SerializeField]
	private float moveRadius = 0.5f;

	// Token: 0x040032A5 RID: 12965
	private float scale = 1f;
}
