using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000539 RID: 1337
[RequireComponent(typeof(BezierSpline))]
public class ManipulatableSpinner : ManipulatableObject
{
	// Token: 0x1700039A RID: 922
	// (get) Token: 0x060021B3 RID: 8627 RVA: 0x000B3E6D File Offset: 0x000B206D
	// (set) Token: 0x060021B4 RID: 8628 RVA: 0x000B3E75 File Offset: 0x000B2075
	public float angle { get; private set; }

	// Token: 0x060021B5 RID: 8629 RVA: 0x000B3E7E File Offset: 0x000B207E
	private void Awake()
	{
		this.spline = base.GetComponent<BezierSpline>();
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000B3E8C File Offset: 0x000B208C
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
		Vector3 position = grabbingHand.transform.position;
		float num = this.FindPositionOnSpline(position);
		this.previousHandT = num;
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x000B3EB4 File Offset: 0x000B20B4
	protected override bool ShouldHandDetach(GameObject hand)
	{
		if (!this.spline.Loop && (this.currentHandT >= 0.99f || this.currentHandT <= 0.01f))
		{
			return true;
		}
		Vector3 position = hand.transform.position;
		Vector3 point = this.spline.GetPoint(this.currentHandT);
		return Vector3.SqrMagnitude(position - point) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x000B3F24 File Offset: 0x000B2124
	protected override void OnHeldUpdate(GameObject hand)
	{
		float angle = this.angle;
		Vector3 position = hand.transform.position;
		this.currentHandT = this.FindPositionOnSpline(position);
		float num = this.currentHandT - this.previousHandT;
		if (this.spline.Loop)
		{
			if (num > 0.5f)
			{
				num -= 1f;
			}
			else if (num < -0.5f)
			{
				num += 1f;
			}
		}
		this.angle += num;
		this.previousHandT = this.currentHandT;
		if (this.applyReleaseVelocity && this.currentHandT <= 0.99f && this.currentHandT >= 0.01f)
		{
			this.tVelocity = (this.angle - angle) / Time.deltaTime;
		}
	}

	// Token: 0x060021BA RID: 8634 RVA: 0x000B3FE0 File Offset: 0x000B21E0
	protected override void OnReleasedUpdate()
	{
		if (this.tVelocity != 0f)
		{
			this.angle += this.tVelocity * Time.deltaTime;
			if (Mathf.Abs(this.tVelocity) < this.lowSpeedThreshold)
			{
				this.tVelocity *= 1f - this.lowSpeedDrag * Time.deltaTime;
				return;
			}
			this.tVelocity *= 1f - this.releaseDrag * Time.deltaTime;
		}
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x000B4068 File Offset: 0x000B2268
	private float FindPositionOnSpline(Vector3 grabPoint)
	{
		int i = 0;
		int num = 200;
		float num2 = 0.001f;
		float num3 = 1f / (float)num;
		float3 @float = base.transform.InverseTransformPoint(grabPoint);
		float num4 = 0f;
		float num5 = float.PositiveInfinity;
		while (i < num)
		{
			float num6 = math.distancesq(this.spline.GetPointLocal(num2), @float);
			if (num6 < num5)
			{
				num5 = num6;
				num4 = num2;
			}
			num2 += num3;
			i++;
		}
		return num4;
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x000B40E4 File Offset: 0x000B22E4
	public void SetAngle(float newAngle)
	{
		this.angle = newAngle;
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000B40ED File Offset: 0x000B22ED
	public void SetVelocity(float newVelocity)
	{
		this.tVelocity = newVelocity;
	}

	// Token: 0x04002C7A RID: 11386
	public float breakDistance = 0.2f;

	// Token: 0x04002C7B RID: 11387
	public bool applyReleaseVelocity;

	// Token: 0x04002C7C RID: 11388
	public float releaseDrag = 1f;

	// Token: 0x04002C7D RID: 11389
	public float lowSpeedThreshold = 0.12f;

	// Token: 0x04002C7E RID: 11390
	public float lowSpeedDrag = 3f;

	// Token: 0x04002C7F RID: 11391
	private BezierSpline spline;

	// Token: 0x04002C80 RID: 11392
	private float previousHandT;

	// Token: 0x04002C81 RID: 11393
	private float currentHandT;

	// Token: 0x04002C82 RID: 11394
	private float tVelocity;
}
