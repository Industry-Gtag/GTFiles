using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A44 RID: 2628
public class GorillaFireball : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x0600437C RID: 17276 RVA: 0x0016706F File Offset: 0x0016526F
	public override void Start()
	{
		base.Start();
		this.canExplode = false;
		this.explosionStartTime = 0f;
	}

	// Token: 0x0600437D RID: 17277 RVA: 0x0016708C File Offset: 0x0016528C
	private void Update()
	{
		if (this.explosionStartTime != 0f)
		{
			float num = (Time.time - this.explosionStartTime) / this.totalExplosionTime * (this.maxExplosionScale - 0.25f) + 0.25f;
			base.gameObject.transform.localScale = new Vector3(num, num, num);
			if (base.photonView.IsMine && Time.time > this.explosionStartTime + this.totalExplosionTime)
			{
				PhotonNetwork.Destroy(PhotonView.Get(this));
			}
		}
	}

	// Token: 0x0600437E RID: 17278 RVA: 0x00167114 File Offset: 0x00165314
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.rigidbody.useGravity)
		{
			this.rigidbody.AddForce(Physics.gravity * -this.gravityStrength * this.rigidbody.mass);
		}
	}

	// Token: 0x0600437F RID: 17279 RVA: 0x00167160 File Offset: 0x00165360
	public override void ThrowThisThingo()
	{
		base.ThrowThisThingo();
		this.canExplode = true;
	}

	// Token: 0x06004380 RID: 17280 RVA: 0x0016716F File Offset: 0x0016536F
	private new void OnCollisionEnter(Collision collision)
	{
		if (base.photonView.IsMine && this.canExplode)
		{
			base.photonView.RPC("Explode", RpcTarget.All, null);
		}
	}

	// Token: 0x06004381 RID: 17281 RVA: 0x00167198 File Offset: 0x00165398
	public void LocalExplode()
	{
		this.rigidbody.isKinematic = true;
		this.canExplode = false;
		this.explosionStartTime = Time.time;
	}

	// Token: 0x06004382 RID: 17282 RVA: 0x001671B8 File Offset: 0x001653B8
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (base.photonView.IsMine)
		{
			if ((bool)base.photonView.InstantiationData[0])
			{
				base.transform.parent = GorillaPlaySpace.Instance.myVRRig.leftHandTransform;
				return;
			}
			base.transform.parent = GorillaPlaySpace.Instance.myVRRig.rightHandTransform;
		}
	}

	// Token: 0x06004383 RID: 17283 RVA: 0x0016721B File Offset: 0x0016541B
	public void Explode()
	{
		this.LocalExplode();
	}

	// Token: 0x04005556 RID: 21846
	public float maxExplosionScale;

	// Token: 0x04005557 RID: 21847
	public float totalExplosionTime;

	// Token: 0x04005558 RID: 21848
	public float gravityStrength;

	// Token: 0x04005559 RID: 21849
	private bool canExplode;

	// Token: 0x0400555A RID: 21850
	private float explosionStartTime;
}
