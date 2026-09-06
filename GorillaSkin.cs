using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020002D6 RID: 726
public class GorillaSkin : ScriptableObject
{
	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x060012A1 RID: 4769 RVA: 0x000636E2 File Offset: 0x000618E2
	public Mesh bodyMesh
	{
		get
		{
			return this._bodyMesh;
		}
	}

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x060012A2 RID: 4770 RVA: 0x000636EA File Offset: 0x000618EA
	public bool allowHeadless
	{
		get
		{
			return !this._disableHeadless;
		}
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x000636F8 File Offset: 0x000618F8
	public static GorillaSkin CopyWithInstancedMaterials(GorillaSkin basis)
	{
		GorillaSkin gorillaSkin = ScriptableObject.CreateInstance<GorillaSkin>();
		gorillaSkin._chestMaterial = ((basis._chestMaterial != null) ? new Material(basis._chestMaterial) : null);
		gorillaSkin._bodyMaterial = ((basis._bodyMaterial != null) ? new Material(basis._bodyMaterial) : null);
		gorillaSkin._scoreboardMaterial = ((basis._scoreboardMaterial != null) ? new Material(basis._scoreboardMaterial) : null);
		gorillaSkin._bodyMesh = basis.bodyMesh;
		return gorillaSkin;
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x060012A4 RID: 4772 RVA: 0x0006377C File Offset: 0x0006197C
	public Material bodyMaterial
	{
		get
		{
			return this._bodyMaterial;
		}
	}

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x060012A5 RID: 4773 RVA: 0x00063784 File Offset: 0x00061984
	public Material chestMaterial
	{
		get
		{
			return this._chestMaterial;
		}
	}

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x060012A6 RID: 4774 RVA: 0x0006378C File Offset: 0x0006198C
	public Material scoreboardMaterial
	{
		get
		{
			return this._scoreboardMaterial;
		}
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x00063794 File Offset: 0x00061994
	public static void ShowActiveSkin(VRRig rig)
	{
		bool flag;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out flag);
		GorillaSkin.ShowSkin(rig, activeSkin, flag);
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x000637B4 File Offset: 0x000619B4
	public void ApplySkinToMannequin(GameObject mannequin, bool swapMesh = false)
	{
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (!mannequin.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
		{
			MeshRenderer meshRenderer;
			if (mannequin.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				meshRenderer.GetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
				GorillaSkin._g_sharedMaterialsCache[0] = this.bodyMaterial;
				GorillaSkin._g_sharedMaterialsCache[1] = this.chestMaterial;
				meshRenderer.SetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
			}
			return;
		}
		int subMeshCount = skinnedMeshRenderer.sharedMesh.subMeshCount;
		if (swapMesh && this.bodyMesh != null)
		{
			skinnedMeshRenderer.sharedMesh = this.bodyMesh;
		}
		int subMeshCount2 = skinnedMeshRenderer.sharedMesh.subMeshCount;
		skinnedMeshRenderer.GetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
		if (subMeshCount == subMeshCount2)
		{
			GorillaSkin._g_sharedMaterialsCache[0] = this.bodyMaterial;
			if (subMeshCount > 2)
			{
				GorillaSkin._g_sharedMaterialsCache[1] = this.chestMaterial;
			}
			skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
			return;
		}
		if (GorillaSkin._g_sharedMaterialsCache.Count == subMeshCount)
		{
			if (subMeshCount2 == 2 && subMeshCount > subMeshCount2)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(GorillaSkin._g_sharedMaterialsCache[2]);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			if (subMeshCount2 == 3 && subMeshCount < subMeshCount2 && GorillaSkin._g_sharedMaterialsCache.Count > 1)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(this.chestMaterial);
				GorillaSkin._g_materialsWriteCache.Add(GorillaSkin._g_sharedMaterialsCache[1]);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			Debug.LogError(string.Format("Unexpected Submesh count {0} {1}", subMeshCount, subMeshCount2));
			return;
		}
		else
		{
			if (subMeshCount2 == 2)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			if (subMeshCount2 == 3)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(this.chestMaterial);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			Debug.LogError(string.Format("Unexpected Submesh count {0}", subMeshCount2));
			return;
		}
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x000639D0 File Offset: 0x00061BD0
	public static GorillaSkin GetActiveSkin(VRRig rig, out bool useDefaultBodySkin)
	{
		if (rig.CurrentModeSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentModeSkin;
		}
		if (rig.TemporaryEffectSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.TemporaryEffectSkin;
		}
		if (rig.CurrentCosmeticSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentCosmeticSkin;
		}
		useDefaultBodySkin = true;
		return rig.defaultSkin;
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x00063A2C File Offset: 0x00061C2C
	public static void ShowSkin(VRRig rig, GorillaSkin skin, bool useDefaultBodySkin = false)
	{
		if (skin.bodyMesh != null)
		{
			rig.bodyRenderer.SetCosmeticBodyMesh(skin.bodyMesh);
		}
		else
		{
			rig.bodyRenderer.ClearCosmeticBodyMesh();
		}
		if (useDefaultBodySkin)
		{
			rig.materialsToChangeTo[0] = rig.myDefaultSkinMaterialInstance;
		}
		else
		{
			rig.materialsToChangeTo[0] = skin.bodyMaterial;
		}
		rig.bodyRenderer.SetSkinMaterials(rig.materialsToChangeTo[rig.setMatIndex], skin.chestMaterial, skin.allowHeadless);
		rig.scoreboardMaterial = skin.scoreboardMaterial;
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x00063AB8 File Offset: 0x00061CB8
	public static void ApplyToRig(VRRig rig, GorillaSkin skin, GorillaSkin.SkinType type)
	{
		bool flag;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out flag);
		switch (type)
		{
		case GorillaSkin.SkinType.cosmetic:
			rig.CurrentCosmeticSkin = skin;
			break;
		case GorillaSkin.SkinType.gameMode:
			rig.CurrentModeSkin = skin;
			break;
		case GorillaSkin.SkinType.temporaryEffect:
			rig.TemporaryEffectSkin = skin;
			break;
		default:
			Debug.LogError("Unknown skin slot");
			break;
		}
		bool flag2;
		GorillaSkin activeSkin2 = GorillaSkin.GetActiveSkin(rig, out flag2);
		if (activeSkin != activeSkin2)
		{
			GorillaSkin.ShowSkin(rig, activeSkin2, flag2);
		}
	}

	// Token: 0x040016AC RID: 5804
	[FormerlySerializedAs("chestMaterial")]
	[FormerlySerializedAs("chestEarsMaterial")]
	[SerializeField]
	private Material _chestMaterial;

	// Token: 0x040016AD RID: 5805
	[FormerlySerializedAs("bodyMaterial")]
	[SerializeField]
	private Material _bodyMaterial;

	// Token: 0x040016AE RID: 5806
	[SerializeField]
	private Material _scoreboardMaterial;

	// Token: 0x040016AF RID: 5807
	[Tooltip("Check this if skin materials are incompatible with HeadlessMonkeRig mesh")]
	[SerializeField]
	private bool _disableHeadless;

	// Token: 0x040016B0 RID: 5808
	[Space]
	[SerializeField]
	private Mesh _bodyMesh;

	// Token: 0x040016B1 RID: 5809
	[Space]
	[NonSerialized]
	private Material _bodyRuntime;

	// Token: 0x040016B2 RID: 5810
	[NonSerialized]
	private Material _chestRuntime;

	// Token: 0x040016B3 RID: 5811
	[NonSerialized]
	private Material _scoreRuntime;

	// Token: 0x040016B4 RID: 5812
	private static List<Material> _g_sharedMaterialsCache = new List<Material>(2);

	// Token: 0x040016B5 RID: 5813
	private static List<Material> _g_materialsWriteCache = new List<Material>(3);

	// Token: 0x020002D7 RID: 727
	public enum SkinType
	{
		// Token: 0x040016B7 RID: 5815
		cosmetic,
		// Token: 0x040016B8 RID: 5816
		gameMode,
		// Token: 0x040016B9 RID: 5817
		temporaryEffect
	}
}
