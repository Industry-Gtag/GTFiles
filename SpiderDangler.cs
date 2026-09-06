using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class SpiderDangler : MonoBehaviour
{
	// Token: 0x06000D66 RID: 3430 RVA: 0x000495F0 File Offset: 0x000477F0
	protected void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Vector3 position = base.transform.position;
		float magnitude = (this.endTransform.position - position).magnitude;
		this.ropeSegLen = magnitude / 6f;
		this.ropeSegs = new SpiderDangler.RopeSegment[6];
		for (int i = 0; i < 6; i++)
		{
			this.ropeSegs[i] = new SpiderDangler.RopeSegment(position);
			position.y -= this.ropeSegLen;
		}
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x00049677 File Offset: 0x00047877
	protected void FixedUpdate()
	{
		this.Simulate();
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00049680 File Offset: 0x00047880
	protected void LateUpdate()
	{
		this.DrawRope();
		Vector3 normalized = (this.ropeSegs[this.ropeSegs.Length - 2].pos - this.ropeSegs[this.ropeSegs.Length - 1].pos).normalized;
		this.endTransform.position = this.ropeSegs[this.ropeSegs.Length - 1].pos;
		this.endTransform.up = normalized;
		Vector4 vector = this.spinSpeeds * Time.time;
		vector = new Vector4(Mathf.Sin(vector.x), Mathf.Sin(vector.y), Mathf.Sin(vector.z), Mathf.Sin(vector.w));
		vector.Scale(this.spinScales);
		this.endTransform.Rotate(Vector3.up, vector.x + vector.y + vector.z + vector.w);
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x00049784 File Offset: 0x00047984
	private void Simulate()
	{
		this.ropeSegLenScaled = this.ropeSegLen * base.transform.lossyScale.x;
		Vector3 vector = new Vector3(0f, -0.5f, 0f) * Time.fixedDeltaTime;
		for (int i = 1; i < 6; i++)
		{
			Vector3 vector2 = this.ropeSegs[i].pos - this.ropeSegs[i].posOld;
			this.ropeSegs[i].posOld = this.ropeSegs[i].pos;
			SpiderDangler.RopeSegment[] array = this.ropeSegs;
			int num = i;
			array[num].pos = array[num].pos + vector2 * 0.95f;
			SpiderDangler.RopeSegment[] array2 = this.ropeSegs;
			int num2 = i;
			array2[num2].pos = array2[num2].pos + vector;
		}
		for (int j = 0; j < 8; j++)
		{
			this.ApplyConstraint();
		}
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x0004988C File Offset: 0x00047A8C
	private void ApplyConstraint()
	{
		this.ropeSegs[0].pos = base.transform.position;
		this.ApplyConstraintSegment(ref this.ropeSegs[0], ref this.ropeSegs[1], 0f, 1f);
		for (int i = 1; i < 5; i++)
		{
			this.ApplyConstraintSegment(ref this.ropeSegs[i], ref this.ropeSegs[i + 1], 0.5f, 0.5f);
		}
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00049914 File Offset: 0x00047B14
	private void ApplyConstraintSegment(ref SpiderDangler.RopeSegment segA, ref SpiderDangler.RopeSegment segB, float dampenA, float dampenB)
	{
		float num = (segA.pos - segB.pos).magnitude - this.ropeSegLenScaled;
		Vector3 vector = (segA.pos - segB.pos).normalized * num;
		segA.pos -= vector * dampenA;
		segB.pos += vector * dampenB;
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x000499A0 File Offset: 0x00047BA0
	private void DrawRope()
	{
		Vector3[] array = new Vector3[6];
		for (int i = 0; i < 6; i++)
		{
			array[i] = this.ropeSegs[i].pos;
		}
		this.lineRenderer.positionCount = array.Length;
		this.lineRenderer.SetPositions(array);
	}

	// Token: 0x0400100E RID: 4110
	public Transform endTransform;

	// Token: 0x0400100F RID: 4111
	public Vector4 spinSpeeds = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);

	// Token: 0x04001010 RID: 4112
	public Vector4 spinScales = new Vector4(180f, 90f, 120f, 180f);

	// Token: 0x04001011 RID: 4113
	private LineRenderer lineRenderer;

	// Token: 0x04001012 RID: 4114
	private SpiderDangler.RopeSegment[] ropeSegs;

	// Token: 0x04001013 RID: 4115
	private float ropeSegLen;

	// Token: 0x04001014 RID: 4116
	private float ropeSegLenScaled;

	// Token: 0x04001015 RID: 4117
	private const int kSegmentCount = 6;

	// Token: 0x04001016 RID: 4118
	private const float kVelocityDamper = 0.95f;

	// Token: 0x04001017 RID: 4119
	private const int kConstraintCalculationIterations = 8;

	// Token: 0x020001FE RID: 510
	public struct RopeSegment
	{
		// Token: 0x06000D6E RID: 3438 RVA: 0x00049A45 File Offset: 0x00047C45
		public RopeSegment(Vector3 pos)
		{
			this.pos = pos;
			this.posOld = pos;
		}

		// Token: 0x04001018 RID: 4120
		public Vector3 pos;

		// Token: 0x04001019 RID: 4121
		public Vector3 posOld;
	}
}
