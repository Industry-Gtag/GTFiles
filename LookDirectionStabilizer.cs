using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020002E9 RID: 745
public class LookDirectionStabilizer : MonoBehaviour, ISpawnable
{
	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060012F5 RID: 4853 RVA: 0x00064FDE File Offset: 0x000631DE
	// (set) Token: 0x060012F6 RID: 4854 RVA: 0x00064FE6 File Offset: 0x000631E6
	public bool IsSpawned { get; set; }

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x060012F7 RID: 4855 RVA: 0x00064FEF File Offset: 0x000631EF
	// (set) Token: 0x060012F8 RID: 4856 RVA: 0x00064FF7 File Offset: 0x000631F7
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x060012F9 RID: 4857 RVA: 0x00065000 File Offset: 0x00063200
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x0006500C File Offset: 0x0006320C
	private void Update()
	{
		Transform rigTarget = this.myRig.head.rigTarget;
		Vector3 up = this.myRig.transform.up;
		if (Vector3.Dot(rigTarget.forward, up) < 0f)
		{
			Quaternion quaternion = Quaternion.LookRotation(rigTarget.up.ProjectOntoPlane(up), up);
			Quaternion rotation = base.transform.parent.rotation;
			float num = Vector3.Dot(rigTarget.up, up);
			base.transform.rotation = Quaternion.Lerp(rotation, quaternion, Mathf.InverseLerp(1f, 0.7f, num));
			return;
		}
		base.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x04001733 RID: 5939
	private VRRig myRig;
}
