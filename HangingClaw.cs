using System;
using UnityEngine;

// Token: 0x020007BA RID: 1978
[RequireComponent(typeof(LineRenderer))]
public class HangingClaw : MonoBehaviourPostTick
{
	// Token: 0x06003292 RID: 12946 RVA: 0x001154D0 File Offset: 0x001136D0
	protected void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Vector3 position = base.transform.position;
		this.segmentCount = 4;
		float magnitude = (this.endTransform.position - position).magnitude;
		this.segmentCount = Mathf.Max(2, this.segmentCount);
		this.baseSegLen = magnitude / (float)this.segmentCount;
		this.ropeSegs = new HangingClaw.RopeSegment[this.segmentCount];
		this.invMass = new float[this.segmentCount];
		for (int i = 0; i < this.segmentCount; i++)
		{
			Vector3 vector = Vector3.Lerp(position, this.endTransform.position, (float)i / (float)(this.segmentCount - 1));
			this.ropeSegs[i] = new HangingClaw.RopeSegment(vector);
		}
		this.invMass[0] = 0f;
		for (int j = 1; j < this.segmentCount - 1; j++)
		{
			this.invMass[j] = 1f / Mathf.Max(0.0001f, this.segmentMassKg);
		}
		this.invMass[this.segmentCount - 1] = 1f / Mathf.Max(0.0001f, this.endMassKg);
	}

	// Token: 0x06003293 RID: 12947 RVA: 0x00115604 File Offset: 0x00113804
	public override void PostTick()
	{
		this.Simulate();
		this.DrawRope();
		int num = this.segmentCount - 1;
		int num2 = this.segmentCount;
		this.endTransform.position = this.ropeSegs[num].pos;
	}

	// Token: 0x06003294 RID: 12948 RVA: 0x0011564C File Offset: 0x0011384C
	private void Simulate()
	{
		float num = this.baseSegLen;
		this.targetSegLenScaled = num * (1f + this.slackFraction);
		float num2 = 0.01111f;
		float num3 = Time.time * 0.5f;
		Vector3 vector = this.gravity * num2 * num2;
		Vector3 vector2 = base.transform.position + new Vector3(0f, 0.012f * Mathf.Sin(num3), 0.02f * Mathf.Cos(num3));
		for (int i = 1; i < this.segmentCount; i++)
		{
			Vector3 vector3 = this.ropeSegs[i].pos - this.ropeSegs[i].posOld;
			this.ropeSegs[i].posOld = this.ropeSegs[i].pos;
			HangingClaw.RopeSegment[] array = this.ropeSegs;
			int num4 = i;
			array[num4].pos = array[num4].pos + (vector3 * this.velocityDamping + vector);
		}
		int num5 = 3;
		for (int j = 0; j < num5; j++)
		{
			this.ApplyConstraints(vector2);
		}
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x00115788 File Offset: 0x00113988
	private void ApplyConstraints(Vector3 topPos)
	{
		this.ropeSegs[0].pos = topPos;
		this.ropeSegs[0].posOld = topPos;
		float num = Mathf.Clamp01(this.ropeStiffness);
		for (int i = 0; i < this.segmentCount - 1; i++)
		{
			this.ApplyConstraintSegment(ref this.ropeSegs[i], ref this.ropeSegs[i + 1], this.invMass[i], this.invMass[i + 1], num);
		}
	}

	// Token: 0x06003296 RID: 12950 RVA: 0x0011580C File Offset: 0x00113A0C
	private void ApplyConstraintSegment(ref HangingClaw.RopeSegment a, ref HangingClaw.RopeSegment b, float wA, float wB, float stiffness)
	{
		Vector3 vector = b.pos - a.pos;
		float magnitude = vector.magnitude;
		if (magnitude < 1E-06f)
		{
			return;
		}
		float num = magnitude - this.targetSegLenScaled;
		if (Mathf.Abs(num) < 1E-06f)
		{
			return;
		}
		Vector3 vector2 = vector / magnitude;
		float num2 = wA + wB;
		if (num2 <= 0f)
		{
			return;
		}
		Vector3 vector3 = vector2 * (num * stiffness);
		a.pos += vector3 * (wA / num2);
		b.pos += -vector3 * (wB / num2);
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x001158C0 File Offset: 0x00113AC0
	private void DrawRope()
	{
		if (this.lineRenderer == null)
		{
			return;
		}
		this.lineRenderer.positionCount = this.segmentCount;
		for (int i = 0; i < this.segmentCount; i++)
		{
			Vector3 pos = this.ropeSegs[i].pos;
			if (this.heightCap && pos.y > this.heightCap.position.y)
			{
				pos.y = this.heightCap.position.y;
			}
			this.lineRenderer.SetPosition(i, this.ropeSegs[i].pos);
		}
	}

	// Token: 0x0400418F RID: 16783
	public Transform endTransform;

	// Token: 0x04004190 RID: 16784
	public Transform heightCap;

	// Token: 0x04004191 RID: 16785
	private int segmentCount = 6;

	// Token: 0x04004192 RID: 16786
	public float segmentMassKg = 1f;

	// Token: 0x04004193 RID: 16787
	public float endMassKg = 5f;

	// Token: 0x04004194 RID: 16788
	public float ropeStiffness = 0.9f;

	// Token: 0x04004195 RID: 16789
	public float slackFraction = 0.02f;

	// Token: 0x04004196 RID: 16790
	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	// Token: 0x04004197 RID: 16791
	public float velocityDamping = 0.98f;

	// Token: 0x04004198 RID: 16792
	private float maxY;

	// Token: 0x04004199 RID: 16793
	private LineRenderer lineRenderer;

	// Token: 0x0400419A RID: 16794
	private HangingClaw.RopeSegment[] ropeSegs;

	// Token: 0x0400419B RID: 16795
	private float baseSegLen;

	// Token: 0x0400419C RID: 16796
	private float targetSegLenScaled;

	// Token: 0x0400419D RID: 16797
	private float[] invMass;

	// Token: 0x020007BB RID: 1979
	public struct RopeSegment
	{
		// Token: 0x06003299 RID: 12953 RVA: 0x001159D7 File Offset: 0x00113BD7
		public RopeSegment(Vector3 p)
		{
			this.pos = p;
			this.posOld = p;
		}

		// Token: 0x0400419E RID: 16798
		public Vector3 pos;

		// Token: 0x0400419F RID: 16799
		public Vector3 posOld;
	}
}
