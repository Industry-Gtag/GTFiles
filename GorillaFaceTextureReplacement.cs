using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002CA RID: 714
public class GorillaFaceTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x0600126C RID: 4716 RVA: 0x00062A93 File Offset: 0x00060C93
	// (set) Token: 0x0600126D RID: 4717 RVA: 0x00062A9B File Offset: 0x00060C9B
	public bool IsSpawned { get; set; }

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x0600126E RID: 4718 RVA: 0x00062AA4 File Offset: 0x00060CA4
	// (set) Token: 0x0600126F RID: 4719 RVA: 0x00062AAC File Offset: 0x00060CAC
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001270 RID: 4720 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDespawn()
	{
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x00062AB5 File Offset: 0x00060CB5
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x00062AC0 File Offset: 0x00060CC0
	private void OnEnable()
	{
		Material material = this.myRig.GetComponent<GorillaMouthFlap>().SetFaceMaterialReplacement(this.newFaceMaterial);
		MeshRenderer[] array = this.alsoApplyFaceTo;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial = material;
		}
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x00062B02 File Offset: 0x00060D02
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearFaceMaterialReplacement();
	}

	// Token: 0x04001653 RID: 5715
	[SerializeField]
	private Material newFaceMaterial;

	// Token: 0x04001654 RID: 5716
	private VRRig myRig;

	// Token: 0x04001655 RID: 5717
	[SerializeField]
	private MeshRenderer[] alsoApplyFaceTo;
}
