using System;
using UnityEngine;

// Token: 0x02000538 RID: 1336
public class ManipulatableSlider : ManipulatableObject
{
	// Token: 0x060021A8 RID: 8616 RVA: 0x000B3A3E File Offset: 0x000B1C3E
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x000B3A62 File Offset: 0x000B1C62
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x000B3A80 File Offset: 0x000B1C80
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000B3AC0 File Offset: 0x000B1CC0
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x000B3B58 File Offset: 0x000B1D58
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

	// Token: 0x060021AE RID: 8622 RVA: 0x000B3D0C File Offset: 0x000B1F0C
	public void SetProgress(float x, float y, float z)
	{
		x = Mathf.Clamp(x, 0f, 1f);
		y = Mathf.Clamp(y, 0f, 1f);
		z = Mathf.Clamp(z, 0f, 1f);
		Vector3 vector = this.startingPos;
		vector.x += Mathf.Lerp(this.minXOffset, this.maxXOffset, x);
		vector.y += Mathf.Lerp(this.minYOffset, this.maxYOffset, y);
		vector.z += Mathf.Lerp(this.minZOffset, this.maxZOffset, z);
		base.transform.localPosition = vector;
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x000B3DB9 File Offset: 0x000B1FB9
	public float GetProgressX()
	{
		return ((base.transform.localPosition - this.startingPos).x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x000B3DEB File Offset: 0x000B1FEB
	public float GetProgressY()
	{
		return ((base.transform.localPosition - this.startingPos).y - this.minYOffset) / (this.maxYOffset - this.minYOffset);
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000B3E1D File Offset: 0x000B201D
	public float GetProgressZ()
	{
		return ((base.transform.localPosition - this.startingPos).z - this.minZOffset) / (this.maxZOffset - this.minZOffset);
	}

	// Token: 0x04002C6E RID: 11374
	public float breakDistance = 0.2f;

	// Token: 0x04002C6F RID: 11375
	public float maxXOffset;

	// Token: 0x04002C70 RID: 11376
	public float minXOffset;

	// Token: 0x04002C71 RID: 11377
	public float maxYOffset;

	// Token: 0x04002C72 RID: 11378
	public float minYOffset;

	// Token: 0x04002C73 RID: 11379
	public float maxZOffset;

	// Token: 0x04002C74 RID: 11380
	public float minZOffset;

	// Token: 0x04002C75 RID: 11381
	public bool applyReleaseVelocity;

	// Token: 0x04002C76 RID: 11382
	public float releaseDrag = 1f;

	// Token: 0x04002C77 RID: 11383
	private Matrix4x4 localSpace;

	// Token: 0x04002C78 RID: 11384
	private Vector3 startingPos;

	// Token: 0x04002C79 RID: 11385
	private Vector3 velocity;
}
