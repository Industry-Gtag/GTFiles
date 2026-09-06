using System;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public class TestManipulatableCube : ManipulatableObject
{
	// Token: 0x06002195 RID: 8597 RVA: 0x000B32FA File Offset: 0x000B14FA
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x000B331E File Offset: 0x000B151E
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000B333C File Offset: 0x000B153C
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000B337C File Offset: 0x000B157C
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000B3414 File Offset: 0x000B1614
	protected override void OnReleasedUpdate()
	{
		if (this.velocity != Vector3.zero)
		{
			Vector3 vector = this.localSpace.MultiplyPoint(base.transform.position);
			vector += this.velocity * Time.deltaTime;
			if (vector.x < this.minXOffset)
			{
				vector.x = this.minXOffset;
				this.velocity.x = 0f;
			}
			else if (vector.x > this.maxXOffset)
			{
				vector.x = this.maxXOffset;
				this.velocity.x = 0f;
			}
			if (vector.y < this.minYOffset)
			{
				vector.y = this.minYOffset;
				this.velocity.y = 0f;
			}
			else if (vector.y > this.maxYOffset)
			{
				vector.y = this.maxYOffset;
				this.velocity.y = 0f;
			}
			if (vector.z < this.minZOffset)
			{
				vector.z = this.minZOffset;
				this.velocity.z = 0f;
			}
			else if (vector.z > this.maxZOffset)
			{
				vector.z = this.maxZOffset;
				this.velocity.z = 0f;
			}
			vector += this.startingPos;
			base.transform.localPosition = vector;
			this.velocity *= 1f - this.releaseDrag * Time.deltaTime;
			if (this.velocity.sqrMagnitude < 0.001f)
			{
				this.velocity = Vector3.zero;
			}
		}
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000B35C5 File Offset: 0x000B17C5
	public Matrix4x4 GetLocalSpace()
	{
		return this.localSpace;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000B35D0 File Offset: 0x000B17D0
	public void SetCubeToSpecificPosition(Vector3 pos)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(pos);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x000B3660 File Offset: 0x000B1860
	public void SetCubeToSpecificPosition(float x, float y, float z)
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		vector.x = Mathf.Clamp(x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x04002C54 RID: 11348
	public float breakDistance = 0.2f;

	// Token: 0x04002C55 RID: 11349
	public float maxXOffset;

	// Token: 0x04002C56 RID: 11350
	public float minXOffset;

	// Token: 0x04002C57 RID: 11351
	public float maxYOffset;

	// Token: 0x04002C58 RID: 11352
	public float minYOffset;

	// Token: 0x04002C59 RID: 11353
	public float maxZOffset;

	// Token: 0x04002C5A RID: 11354
	public float minZOffset;

	// Token: 0x04002C5B RID: 11355
	public bool applyReleaseVelocity;

	// Token: 0x04002C5C RID: 11356
	public float releaseDrag = 1f;

	// Token: 0x04002C5D RID: 11357
	private Matrix4x4 localSpace;

	// Token: 0x04002C5E RID: 11358
	private Vector3 startingPos;

	// Token: 0x04002C5F RID: 11359
	private Vector3 velocity;
}
