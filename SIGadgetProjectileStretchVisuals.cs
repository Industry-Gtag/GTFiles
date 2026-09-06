using System;
using UnityEngine;

// Token: 0x020000E7 RID: 231
[RequireComponent(typeof(SIGadgetBlasterProjectile))]
public class SIGadgetProjectileStretchVisuals : MonoBehaviourTick
{
	// Token: 0x06000569 RID: 1385 RVA: 0x0001E854 File Offset: 0x0001CA54
	public new void OnEnable()
	{
		base.OnEnable();
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
		this.totalLength = (this.frontStretch.position - this.rearStretch.position).magnitude;
		this.distancePerFrame = this.projectile.startingVelocity * Time.fixedDeltaTime;
		this.maxStretchRatio = this.distancePerFrame / this.totalLength * this.framesPerPosition;
		this.timeSpawned = Time.time;
		this.maxSizeReached = false;
		this.baseVisuals.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.baseVisuals.transform.localScale = new Vector3(1f, 1f, 1f);
		this.frontDistance = (this.frontStretch.position - base.transform.position).magnitude;
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0001E950 File Offset: 0x0001CB50
	public override void Tick()
	{
		if (this.maxSizeReached)
		{
			return;
		}
		float num = (Time.time - this.timeSpawned) * this.projectile.startingVelocity / this.totalLength / 2f + 1f;
		if (num >= this.maxStretchRatio)
		{
			num = this.maxStretchRatio;
			this.maxSizeReached = true;
		}
		this.baseVisuals.transform.localPosition = new Vector3(0f, 0f, -(num - 1f) * this.frontDistance);
		this.baseVisuals.transform.localScale = new Vector3(1f, 1f, num);
	}

	// Token: 0x04000645 RID: 1605
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x04000646 RID: 1606
	public GameObject baseVisuals;

	// Token: 0x04000647 RID: 1607
	public Transform frontStretch;

	// Token: 0x04000648 RID: 1608
	public Transform rearStretch;

	// Token: 0x04000649 RID: 1609
	public float framesPerPosition;

	// Token: 0x0400064A RID: 1610
	private float totalLength;

	// Token: 0x0400064B RID: 1611
	private float distancePerFrame;

	// Token: 0x0400064C RID: 1612
	private float maxStretchRatio;

	// Token: 0x0400064D RID: 1613
	private bool maxSizeReached;

	// Token: 0x0400064E RID: 1614
	private float frontDistance;

	// Token: 0x0400064F RID: 1615
	private float timeSpawned;
}
