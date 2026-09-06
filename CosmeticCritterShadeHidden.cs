using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class CosmeticCritterShadeHidden : CosmeticCritter
{
	// Token: 0x060004E6 RID: 1254 RVA: 0x0001B60E File Offset: 0x0001980E
	public void SetCenterAndRadius(Vector3 center, float radius)
	{
		this.orbitCenter = center;
		this.orbitRadius = radius;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001B61E File Offset: 0x0001981E
	public override void SetRandomVariables()
	{
		this.initialAngle = Random.Range(0f, 6.2831855f);
		this.orbitDirection = ((Random.value > 0.5f) ? 1f : (-1f));
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0001B654 File Offset: 0x00019854
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		float num2 = this.initialAngle + this.orbitDegreesPerSecond * num * this.orbitDirection;
		float num3 = this.verticalBobMagnitude * Mathf.Sin(num * this.verticalBobFrequency);
		base.transform.position = this.orbitCenter + new Vector3(this.orbitRadius * Mathf.Cos(num2), num3, this.orbitRadius * Mathf.Sin(num2));
	}

	// Token: 0x04000578 RID: 1400
	[Space]
	[Tooltip("How quickly the Shade orbits around the point where it spawned (the spawner's position).")]
	[SerializeField]
	private float orbitDegreesPerSecond;

	// Token: 0x04000579 RID: 1401
	[Tooltip("The strength of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobMagnitude;

	// Token: 0x0400057A RID: 1402
	[Tooltip("The frequency of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobFrequency;

	// Token: 0x0400057B RID: 1403
	private Vector3 orbitCenter;

	// Token: 0x0400057C RID: 1404
	private float initialAngle;

	// Token: 0x0400057D RID: 1405
	private float orbitRadius;

	// Token: 0x0400057E RID: 1406
	private float orbitDirection;
}
