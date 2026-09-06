using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002D5 RID: 725
public class GorillaMouthTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x06001298 RID: 4760 RVA: 0x0006368D File Offset: 0x0006188D
	// (set) Token: 0x06001299 RID: 4761 RVA: 0x00063695 File Offset: 0x00061895
	public bool IsSpawned { get; set; }

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x0600129A RID: 4762 RVA: 0x0006369E File Offset: 0x0006189E
	// (set) Token: 0x0600129B RID: 4763 RVA: 0x000636A6 File Offset: 0x000618A6
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x0600129C RID: 4764 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDespawn()
	{
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x000636AF File Offset: 0x000618AF
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x000636B8 File Offset: 0x000618B8
	private void OnEnable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().SetMouthTextureReplacement(this.newMouthAtlas);
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x000636D0 File Offset: 0x000618D0
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearMouthTextureReplacement();
	}

	// Token: 0x040016A8 RID: 5800
	[SerializeField]
	private Texture2D newMouthAtlas;

	// Token: 0x040016A9 RID: 5801
	private VRRig myRig;
}
