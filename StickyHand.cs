using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002FE RID: 766
public class StickyHand : MonoBehaviour, ISpawnable
{
	// Token: 0x170001ED RID: 493
	// (get) Token: 0x06001383 RID: 4995 RVA: 0x0006739F File Offset: 0x0006559F
	// (set) Token: 0x06001384 RID: 4996 RVA: 0x000673A7 File Offset: 0x000655A7
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06001385 RID: 4997 RVA: 0x000673B0 File Offset: 0x000655B0
	// (set) Token: 0x06001386 RID: 4998 RVA: 0x000673B8 File Offset: 0x000655B8
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001387 RID: 4999 RVA: 0x000673C4 File Offset: 0x000655C4
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.isLocal = rig.isLocal;
		this.flatHand.enabled = false;
		this.defaultLocalPosition = this.stringParent.transform.InverseTransformPoint(this.rb.transform.position);
		int num = ((this.CosmeticSelectedSide == ECosmeticSelectSide.Left) ? 1 : 2);
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x0006743C File Offset: 0x0006563C
	private void Update()
	{
		if (this.isLocal)
		{
			if (this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringDetachLength))
			{
				this.Unstick();
			}
			else if (!this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringTeleportLength))
			{
				this.rb.transform.position = this.stringParent.transform.TransformPoint(this.defaultLocalPosition);
			}
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.rb.isKinematic);
			return;
		}
		if (GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex) != this.rb.isKinematic)
		{
			if (this.rb.isKinematic)
			{
				this.Unstick();
				return;
			}
			this.Stick();
		}
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x0006756A File Offset: 0x0006576A
	private void Stick()
	{
		this.thwackSound.Play();
		this.flatHand.enabled = true;
		this.regularHand.enabled = false;
		this.rb.isKinematic = true;
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x0006759B File Offset: 0x0006579B
	private void Unstick()
	{
		this.schlupSound.Play();
		this.rb.isKinematic = false;
		this.flatHand.enabled = false;
		this.regularHand.enabled = true;
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x000675CC File Offset: 0x000657CC
	private void OnCollisionStay(Collision collision)
	{
		if (!this.isLocal || this.rb.isKinematic)
		{
			return;
		}
		if ((this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringMaxAttachLength))
		{
			return;
		}
		this.Stick();
		Vector3 point = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;
		this.rb.transform.rotation = Quaternion.LookRotation(normal, this.rb.transform.up);
		Vector3 vector = this.rb.transform.position - point;
		vector -= Vector3.Dot(vector, normal) * normal;
		this.rb.transform.position = point + vector + this.surfaceOffsetDistance * normal;
	}

	// Token: 0x040017EB RID: 6123
	[SerializeField]
	private MeshRenderer flatHand;

	// Token: 0x040017EC RID: 6124
	[SerializeField]
	private MeshRenderer regularHand;

	// Token: 0x040017ED RID: 6125
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x040017EE RID: 6126
	[SerializeField]
	private GameObject stringParent;

	// Token: 0x040017EF RID: 6127
	[SerializeField]
	private float surfaceOffsetDistance;

	// Token: 0x040017F0 RID: 6128
	[SerializeField]
	private float stringMaxAttachLength;

	// Token: 0x040017F1 RID: 6129
	[SerializeField]
	private float stringDetachLength;

	// Token: 0x040017F2 RID: 6130
	[SerializeField]
	private float stringTeleportLength;

	// Token: 0x040017F3 RID: 6131
	[SerializeField]
	private SoundBankPlayer thwackSound;

	// Token: 0x040017F4 RID: 6132
	[SerializeField]
	private SoundBankPlayer schlupSound;

	// Token: 0x040017F5 RID: 6133
	private VRRig myRig;

	// Token: 0x040017F6 RID: 6134
	private bool isLocal;

	// Token: 0x040017F7 RID: 6135
	private int stateBitIndex;

	// Token: 0x040017F8 RID: 6136
	private Vector3 defaultLocalPosition;
}
