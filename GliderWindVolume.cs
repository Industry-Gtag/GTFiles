using System;
using UnityEngine;

// Token: 0x02000CE9 RID: 3305
public class GliderWindVolume : MonoBehaviour
{
	// Token: 0x060051D9 RID: 20953 RVA: 0x001B3447 File Offset: 0x001B1647
	public void SetProperties(float speed, float accel, AnimationCurve svaCurve, Vector3 windDirection)
	{
		this.maxSpeed = speed;
		this.maxAccel = accel;
		this.speedVsAccelCurve.CopyFrom(svaCurve);
		this.localWindDirection = windDirection;
	}

	// Token: 0x170007C2 RID: 1986
	// (get) Token: 0x060051DA RID: 20954 RVA: 0x001B346B File Offset: 0x001B166B
	public Vector3 WindDirection
	{
		get
		{
			return base.transform.TransformDirection(this.localWindDirection);
		}
	}

	// Token: 0x060051DB RID: 20955 RVA: 0x001B3480 File Offset: 0x001B1680
	public Vector3 GetAccelFromVelocity(Vector3 velocity)
	{
		Vector3 windDirection = this.WindDirection;
		float num = Mathf.Clamp(Vector3.Dot(velocity, windDirection), -this.maxSpeed, this.maxSpeed) / this.maxSpeed;
		float num2 = this.speedVsAccelCurve.Evaluate(num) * this.maxAccel;
		return windDirection * num2;
	}

	// Token: 0x0400643D RID: 25661
	[SerializeField]
	private float maxSpeed = 30f;

	// Token: 0x0400643E RID: 25662
	[SerializeField]
	private float maxAccel = 15f;

	// Token: 0x0400643F RID: 25663
	[SerializeField]
	private AnimationCurve speedVsAccelCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04006440 RID: 25664
	[SerializeField]
	private Vector3 localWindDirection = Vector3.up;
}
