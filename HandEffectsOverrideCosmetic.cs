using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020003D9 RID: 985
public class HandEffectsOverrideCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06001772 RID: 6002 RVA: 0x00087400 File Offset: 0x00085600
	// (set) Token: 0x06001773 RID: 6003 RVA: 0x00087408 File Offset: 0x00085608
	public bool IsSpawned { get; set; }

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06001774 RID: 6004 RVA: 0x00087411 File Offset: 0x00085611
	// (set) Token: 0x06001775 RID: 6005 RVA: 0x00087419 File Offset: 0x00085619
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001776 RID: 6006 RVA: 0x00087422 File Offset: 0x00085622
	public void OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06001777 RID: 6007 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDespawn()
	{
	}

	// Token: 0x06001778 RID: 6008 RVA: 0x0008742B File Offset: 0x0008562B
	public void OnEnable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Add(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Add(this);
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x00087458 File Offset: 0x00085658
	public void OnDisable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Remove(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Remove(this);
	}

	// Token: 0x040022B4 RID: 8884
	public HandEffectsOverrideCosmetic.HandEffectType handEffectType;

	// Token: 0x040022B5 RID: 8885
	public bool isLeftHand;

	// Token: 0x040022B6 RID: 8886
	public HandEffectsOverrideCosmetic.EffectsOverride firstPerson;

	// Token: 0x040022B7 RID: 8887
	public HandEffectsOverrideCosmetic.EffectsOverride thirdPerson;

	// Token: 0x040022B8 RID: 8888
	private VRRig _rig;

	// Token: 0x020003DA RID: 986
	[Serializable]
	public class EffectsOverride
	{
		// Token: 0x040022BB RID: 8891
		public GameObject effectVFX;

		// Token: 0x040022BC RID: 8892
		public bool playHaptics;

		// Token: 0x040022BD RID: 8893
		public float hapticStrength = 0.5f;

		// Token: 0x040022BE RID: 8894
		public float hapticDuration = 0.5f;

		// Token: 0x040022BF RID: 8895
		public bool parentEffect;
	}

	// Token: 0x020003DB RID: 987
	public enum HandEffectType
	{
		// Token: 0x040022C1 RID: 8897
		None,
		// Token: 0x040022C2 RID: 8898
		FistBump,
		// Token: 0x040022C3 RID: 8899
		HighFive
	}
}
