using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000860 RID: 2144
public class GorillaBodyRenderer : MonoBehaviour
{
	// Token: 0x170004F5 RID: 1269
	// (get) Token: 0x06003793 RID: 14227 RVA: 0x00130C8C File Offset: 0x0012EE8C
	// (set) Token: 0x06003794 RID: 14228 RVA: 0x00130C94 File Offset: 0x0012EE94
	public GorillaBodyType bodyType
	{
		get
		{
			return this._bodyType;
		}
		set
		{
			this.SetBodyType(value);
		}
	}

	// Token: 0x170004F6 RID: 1270
	// (get) Token: 0x06003795 RID: 14229 RVA: 0x00130C9D File Offset: 0x0012EE9D
	public bool renderFace
	{
		get
		{
			return this._renderFace;
		}
	}

	// Token: 0x170004F7 RID: 1271
	// (get) Token: 0x06003796 RID: 14230 RVA: 0x00130CA5 File Offset: 0x0012EEA5
	public static bool ForceSkeleton
	{
		get
		{
			return GorillaBodyRenderer.oopsAllSkeletons;
		}
	}

	// Token: 0x170004F8 RID: 1272
	// (get) Token: 0x06003797 RID: 14231 RVA: 0x00130CAC File Offset: 0x0012EEAC
	// (set) Token: 0x06003798 RID: 14232 RVA: 0x00130CB4 File Offset: 0x0012EEB4
	public GorillaBodyType gameModeBodyType { get; private set; }

	// Token: 0x170004F9 RID: 1273
	// (get) Token: 0x06003799 RID: 14233 RVA: 0x00130CBD File Offset: 0x0012EEBD
	// (set) Token: 0x0600379A RID: 14234 RVA: 0x00130CC5 File Offset: 0x0012EEC5
	public Material myDefaultSkinMaterialInstance { get; private set; }

	// Token: 0x0600379B RID: 14235 RVA: 0x00130CD0 File Offset: 0x0012EED0
	public SkinnedMeshRenderer GetBody(GorillaBodyType type)
	{
		if (type < GorillaBodyType.Default || type >= (GorillaBodyType)this._renderersCache.Length)
		{
			return null;
		}
		return this._renderersCache[(int)type];
	}

	// Token: 0x170004FA RID: 1274
	// (get) Token: 0x0600379C RID: 14236 RVA: 0x00130CF8 File Offset: 0x0012EEF8
	public SkinnedMeshRenderer ActiveBody
	{
		get
		{
			return this.GetBody(this._bodyType);
		}
	}

	// Token: 0x0600379D RID: 14237 RVA: 0x00130D08 File Offset: 0x0012EF08
	public static void SetAllSkeletons(bool allSkeletons)
	{
		GorillaBodyRenderer.oopsAllSkeletons = allSkeletons;
		GorillaTagger.Instance.offlineVRRig.bodyRenderer.Refresh();
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			rigContainer.Rig.bodyRenderer.Refresh();
		}
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x00130D78 File Offset: 0x0012EF78
	public void SetSkeletonBodyActive(bool active)
	{
		this.bodySkeleton.gameObject.SetActive(active);
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x00130D8B File Offset: 0x0012EF8B
	public static void EnableSkeletonOverlays(Material bodyMaterial, Material skeletonMaterial)
	{
		GorillaBodyRenderer.<>c__DisplayClass33_0 CS$<>8__locals1 = new GorillaBodyRenderer.<>c__DisplayClass33_0();
		CS$<>8__locals1.bodyMaterial = bodyMaterial;
		CS$<>8__locals1.skeletonMaterial = skeletonMaterial;
		CS$<>8__locals1.<EnableSkeletonOverlays>g__ShowSkeletonOverlay|0(GorillaTagger.Instance.offlineVRRig);
		VRRigCache.ApplyToAllRigs(new Action<VRRig>(CS$<>8__locals1.<EnableSkeletonOverlays>g__ShowSkeletonOverlay|0));
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x00130DC0 File Offset: 0x0012EFC0
	public static void DisableSkeletonOverlays()
	{
		GorillaBodyRenderer.HideSkeletonOverlay(GorillaTagger.Instance.offlineVRRig);
		VRRigCache.ApplyToAllRigs(new Action<VRRig>(GorillaBodyRenderer.HideSkeletonOverlay));
	}

	// Token: 0x060037A1 RID: 14241 RVA: 0x00130DE2 File Offset: 0x0012EFE2
	private static void HideSkeletonOverlay(VRRig rig)
	{
		rig.bodyRenderer.Refresh();
		rig.bodyRenderer.bodyDefault.sharedMaterial = rig.bodyRenderer.myDefaultSkinMaterialInstance;
	}

	// Token: 0x060037A2 RID: 14242 RVA: 0x00130E0A File Offset: 0x0012F00A
	public void SetGameModeBodyType(GorillaBodyType bodyType)
	{
		if (this.gameModeBodyType == bodyType)
		{
			return;
		}
		this.gameModeBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x060037A3 RID: 14243 RVA: 0x00130E23 File Offset: 0x0012F023
	public void SetCosmeticBodyType(GorillaBodyType bodyType)
	{
		if (this.cosmeticBodyType == bodyType)
		{
			return;
		}
		this.cosmeticBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x060037A4 RID: 14244 RVA: 0x00130E3C File Offset: 0x0012F03C
	public void SetDefaults()
	{
		this.gameModeBodyType = GorillaBodyType.Default;
		this.cosmeticBodyType = GorillaBodyType.Default;
		this.Refresh();
	}

	// Token: 0x060037A5 RID: 14245 RVA: 0x00130E52 File Offset: 0x0012F052
	private void Refresh()
	{
		this.SetBodyType(this.GetActiveBodyType());
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x00130E60 File Offset: 0x0012F060
	public void SetMaterialIndex(int materialIndex)
	{
		this._lastMatIndex = materialIndex;
		switch (this.bodyType)
		{
		case GorillaBodyType.Default:
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[materialIndex];
			return;
		case GorillaBodyType.NoHead:
			if (materialIndex == 0 && !this._applySkinToHeadlessMesh)
			{
				this.bodyNoHead.sharedMaterial = this.myDefaultSkinMaterialInstance;
				return;
			}
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[materialIndex];
			return;
		case GorillaBodyType.Skeleton:
			this.rig.skeleton.SetMaterialIndex(materialIndex);
			return;
		default:
			return;
		}
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x00130EF0 File Offset: 0x0012F0F0
	public void SetSkinMaterials(Material bodyMat, Material chestMat, bool allowHeadless)
	{
		this.EnsureInstantiatedMaterial();
		if (chestMat == null)
		{
			if (this._cachedSkinMaterials.Length != 1)
			{
				this._cachedSkinMaterials = new Material[1];
			}
			this._cachedSkinMaterials[0] = bodyMat;
		}
		else
		{
			if (this._cachedSkinMaterials.Length < 2)
			{
				this._cachedSkinMaterials = new Material[2];
			}
			this._cachedSkinMaterials[0] = bodyMat;
			this._cachedSkinMaterials[1] = chestMat;
		}
		this._applySkinToHeadlessMesh = allowHeadless;
		GorillaBodyType bodyType = this.bodyType;
		if (bodyType == GorillaBodyType.Default)
		{
			this.bodyDefault.sharedMaterials = this._cachedSkinMaterials;
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			return;
		}
		if (bodyType != GorillaBodyType.NoHead)
		{
			return;
		}
		if (this._applySkinToHeadlessMesh)
		{
			this.bodyNoHead.sharedMaterials = this._cachedSkinMaterials;
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			return;
		}
		this.bodyNoHead.sharedMaterials = this._defaultSkinMaterials;
		if (this._lastMatIndex != 0)
		{
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
		}
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x00131009 File Offset: 0x0012F209
	public void SetupAsLocalPlayerBody()
	{
		this.faceRenderer.gameObject.layer = 22;
	}

	// Token: 0x060037A9 RID: 14249 RVA: 0x0013101D File Offset: 0x0012F21D
	public GorillaBodyType GetActiveBodyType()
	{
		if (GorillaBodyRenderer.oopsAllSkeletons)
		{
			return GorillaBodyType.Skeleton;
		}
		if (this.gameModeBodyType == GorillaBodyType.Default)
		{
			return this.cosmeticBodyType;
		}
		return this.gameModeBodyType;
	}

	// Token: 0x060037AA RID: 14250 RVA: 0x00131040 File Offset: 0x0012F240
	private void SetBodyType(GorillaBodyType type)
	{
		if (this._bodyType == type)
		{
			return;
		}
		this.SetBodyEnabled(this._bodyType, false);
		this._bodyType = type;
		this.SetBodyEnabled(type, true);
		this._renderFace = this._bodyType != GorillaBodyType.NoHead && this._bodyType != GorillaBodyType.Skeleton && this._bodyType != GorillaBodyType.Invisible;
		if (this.faceRenderer != null)
		{
			this.faceRenderer.enabled = this._renderFace;
		}
		switch (type)
		{
		case GorillaBodyType.Default:
			this.bodyDefault.sharedMaterials = this._cachedSkinMaterials;
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			this.UpdateBodyMaterialColor(this.rig.playerColor);
			return;
		case GorillaBodyType.NoHead:
			if (this._applySkinToHeadlessMesh)
			{
				this.bodyNoHead.sharedMaterials = this._cachedSkinMaterials;
				this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			}
			else
			{
				this.bodyNoHead.sharedMaterials = this._defaultSkinMaterials;
				if (this._lastMatIndex != 0)
				{
					this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
				}
			}
			this.UpdateBodyMaterialColor(this.rig.playerColor);
			return;
		case GorillaBodyType.Skeleton:
			this.rig.skeleton.SetMaterialIndex(this._lastMatIndex);
			this.rig.skeleton.UpdateColor(this.rig.playerColor);
			return;
		default:
			return;
		}
	}

	// Token: 0x060037AB RID: 14251 RVA: 0x001311BD File Offset: 0x0012F3BD
	public void SetCosmeticBodyMesh(Mesh mesh)
	{
		if (this.defaultBodyMesh == null)
		{
			this.defaultBodyMesh = this.bodyDefault.sharedMesh;
		}
		this.bodyDefault.sharedMesh = mesh;
	}

	// Token: 0x060037AC RID: 14252 RVA: 0x001311EA File Offset: 0x0012F3EA
	public void ClearCosmeticBodyMesh()
	{
		if (this.defaultBodyMesh != null)
		{
			this.bodyDefault.sharedMesh = this.defaultBodyMesh;
		}
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x0013120C File Offset: 0x0012F40C
	private void SetBodyEnabled(GorillaBodyType bodyType, bool enabled)
	{
		SkinnedMeshRenderer body = this.GetBody(bodyType);
		if (body == null)
		{
			return;
		}
		body.enabled = enabled;
		Transform[] bones = body.bones;
		for (int i = 0; i < bones.Length; i++)
		{
			bones[i].gameObject.SetActive(enabled);
		}
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x00131255 File Offset: 0x0012F455
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060037AF RID: 14255 RVA: 0x0013125D File Offset: 0x0012F45D
	public void SharedStart()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		this.EnsureInstantiatedMaterial();
	}

	// Token: 0x060037B0 RID: 14256 RVA: 0x00131280 File Offset: 0x0012F480
	private void Setup()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		this._renderersCache = new SkinnedMeshRenderer[EnumData<GorillaBodyType>.Shared.Values.Length];
		this._renderersCache[0] = this.bodyDefault;
		this._renderersCache[1] = this.bodyNoHead;
		this._renderersCache[2] = this.bodySkeleton;
		this.SetBodyEnabled(GorillaBodyType.Default, true);
		this.SetBodyEnabled(GorillaBodyType.NoHead, false);
		this.SetBodyEnabled(GorillaBodyType.Skeleton, false);
		this._cachedSkinMaterials = this.bodyDefault.sharedMaterials;
		this._bodyType = GorillaBodyType.Default;
		this._bodyType = GorillaBodyType.Default;
		this.defaultBodyMesh = this.bodyDefault.sharedMesh;
		this.EnsureInstantiatedMaterial();
		this.UpdateColor(this.rig.playerColor);
		this.Refresh();
	}

	// Token: 0x060037B1 RID: 14257 RVA: 0x00131350 File Offset: 0x0012F550
	public void EnsureInstantiatedMaterial()
	{
		if (this.myDefaultSkinMaterialInstance == null)
		{
			this.myDefaultSkinMaterialInstance = Object.Instantiate<Material>(this.rig.materialsToChangeTo[0]);
			this.rig.materialsToChangeTo[0] = this.myDefaultSkinMaterialInstance;
		}
		if (this._defaultSkinMaterials.Length == 0)
		{
			this._defaultSkinMaterials = new Material[2];
			this._defaultSkinMaterials[0] = this.myDefaultSkinMaterialInstance;
			this._defaultSkinMaterials[1] = this.rig.defaultSkin.chestMaterial;
		}
	}

	// Token: 0x060037B2 RID: 14258 RVA: 0x001313D4 File Offset: 0x0012F5D4
	public void ResetBodyMaterial()
	{
		this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[0];
		this.bodyNoHead.sharedMaterial = (this._applySkinToHeadlessMesh ? this.rig.materialsToChangeTo[0] : this.myDefaultSkinMaterialInstance);
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x00131421 File Offset: 0x0012F621
	public void UpdateColor(Color color)
	{
		this.UpdateBodyMaterialColor(color);
		if (this.bodyType == GorillaBodyType.Skeleton)
		{
			this.rig.skeleton.UpdateColor(color);
		}
	}

	// Token: 0x060037B4 RID: 14260 RVA: 0x00131444 File Offset: 0x0012F644
	private void UpdateBodyMaterialColor(Color color)
	{
		this.EnsureInstantiatedMaterial();
		if (this.myDefaultSkinMaterialInstance != null)
		{
			this.myDefaultSkinMaterialInstance.color = color;
		}
	}

	// Token: 0x040047B8 RID: 18360
	[SerializeField]
	private GorillaBodyType _bodyType;

	// Token: 0x040047B9 RID: 18361
	[SerializeField]
	private bool _renderFace = true;

	// Token: 0x040047BA RID: 18362
	public MeshRenderer faceRenderer;

	// Token: 0x040047BB RID: 18363
	[SerializeField]
	private SkinnedMeshRenderer bodyDefault;

	// Token: 0x040047BC RID: 18364
	[SerializeField]
	private SkinnedMeshRenderer bodyNoHead;

	// Token: 0x040047BD RID: 18365
	[SerializeField]
	private SkinnedMeshRenderer bodySkeleton;

	// Token: 0x040047BE RID: 18366
	private int _lastMatIndex;

	// Token: 0x040047BF RID: 18367
	private Mesh defaultBodyMesh;

	// Token: 0x040047C0 RID: 18368
	private static bool oopsAllSkeletons;

	// Token: 0x040047C2 RID: 18370
	private GorillaBodyType cosmeticBodyType;

	// Token: 0x040047C4 RID: 18372
	[SerializeField]
	private Material[] _cachedSkinMaterials = new Material[0];

	// Token: 0x040047C5 RID: 18373
	[SerializeField]
	private Material[] _defaultSkinMaterials = new Material[0];

	// Token: 0x040047C6 RID: 18374
	private bool _applySkinToHeadlessMesh;

	// Token: 0x040047C7 RID: 18375
	[Space]
	[NonSerialized]
	private SkinnedMeshRenderer[] _renderersCache = new SkinnedMeshRenderer[0];

	// Token: 0x040047C8 RID: 18376
	private static readonly List<Material> gEmptyDefaultMats = new List<Material>();

	// Token: 0x040047C9 RID: 18377
	[Space]
	public VRRig rig;
}
