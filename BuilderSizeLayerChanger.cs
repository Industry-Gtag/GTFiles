using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000623 RID: 1571
public class BuilderSizeLayerChanger : MonoBehaviour
{
	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x06002724 RID: 10020 RVA: 0x000CF0B8 File Offset: 0x000CD2B8
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

	// Token: 0x06002725 RID: 10021 RVA: 0x000CF0F8 File Offset: 0x000CD2F8
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x000CF110 File Offset: 0x000CD310
	public void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x000CF19C File Offset: 0x000CD39C
	public void OnTriggerExit(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x040032AA RID: 12970
	public float maxScale;

	// Token: 0x040032AB RID: 12971
	public float minScale;

	// Token: 0x040032AC RID: 12972
	public bool isAssurance;

	// Token: 0x040032AD RID: 12973
	public bool affectLayerA = true;

	// Token: 0x040032AE RID: 12974
	public bool affectLayerB = true;

	// Token: 0x040032AF RID: 12975
	public bool affectLayerC = true;

	// Token: 0x040032B0 RID: 12976
	public bool affectLayerD = true;

	// Token: 0x040032B1 RID: 12977
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x040032B2 RID: 12978
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x040032B3 RID: 12979
	[SerializeField]
	private GameObject fxForLayerChange;
}
