using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TagEffects;
using UnityEngine;

// Token: 0x020002FF RID: 767
public class TagEffectsPackToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x170001EF RID: 495
	// (get) Token: 0x0600138E RID: 5006 RVA: 0x000676C5 File Offset: 0x000658C5
	// (set) Token: 0x0600138F RID: 5007 RVA: 0x000676CD File Offset: 0x000658CD
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001390 RID: 5008 RVA: 0x000676D6 File Offset: 0x000658D6
	// (set) Token: 0x06001391 RID: 5009 RVA: 0x000676DE File Offset: 0x000658DE
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001392 RID: 5010 RVA: 0x000676E7 File Offset: 0x000658E7
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x000676F0 File Offset: 0x000658F0
	private void OnEnable()
	{
		this.Apply();
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x000676F8 File Offset: 0x000658F8
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x06001396 RID: 5014 RVA: 0x00067708 File Offset: 0x00065908
	public void Apply()
	{
		this._rig.CosmeticEffectPack = this.tagEffectPack;
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x0006771B File Offset: 0x0006591B
	public void Remove()
	{
		this._rig.CosmeticEffectPack = null;
	}

	// Token: 0x040017FB RID: 6139
	private VRRig _rig;

	// Token: 0x040017FC RID: 6140
	[SerializeField]
	private TagEffectPack tagEffectPack;
}
