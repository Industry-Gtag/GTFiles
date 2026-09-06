using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000929 RID: 2345
public class SizeLayerChanger : MonoBehaviour
{
	// Token: 0x170005A8 RID: 1448
	// (get) Token: 0x06003D73 RID: 15731 RVA: 0x0014D814 File Offset: 0x0014BA14
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06003D74 RID: 15732 RVA: 0x0014D854 File Offset: 0x0014BA54
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06003D75 RID: 15733 RVA: 0x0014D86C File Offset: 0x0014BA6C
	public void OnTriggerEnter(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x06003D76 RID: 15734 RVA: 0x0014D8EC File Offset: 0x0014BAEC
	public void OnTriggerExit(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x04004E33 RID: 20019
	public float maxScale;

	// Token: 0x04004E34 RID: 20020
	public float minScale;

	// Token: 0x04004E35 RID: 20021
	public bool isAssurance;

	// Token: 0x04004E36 RID: 20022
	public bool affectLayerA = true;

	// Token: 0x04004E37 RID: 20023
	public bool affectLayerB = true;

	// Token: 0x04004E38 RID: 20024
	public bool affectLayerC = true;

	// Token: 0x04004E39 RID: 20025
	public bool affectLayerD = true;

	// Token: 0x04004E3A RID: 20026
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04004E3B RID: 20027
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04004E3C RID: 20028
	[SerializeField]
	private bool triggerWithBodyCollider;
}
