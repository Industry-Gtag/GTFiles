using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B0B RID: 2827
[HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/rendering/material-instance")]
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Scripts/MRTK/Core/MaterialInstance")]
public class MaterialInstance : MonoBehaviour
{
	// Token: 0x0600484F RID: 18511 RVA: 0x0018559D File Offset: 0x0018379D
	public Material AcquireMaterial(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		Material[] array = this.instanceMaterials;
		if (array != null && array.Length != 0)
		{
			return this.instanceMaterials[0];
		}
		return null;
	}

	// Token: 0x06004850 RID: 18512 RVA: 0x001855DB File Offset: 0x001837DB
	public Material[] AcquireMaterials(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		base.gameObject.GetComponent<Material>();
		return this.instanceMaterials;
	}

	// Token: 0x06004851 RID: 18513 RVA: 0x0018560E File Offset: 0x0018380E
	public void ReleaseMaterial(Object owner, bool autoDestroy = true)
	{
		this.materialOwners.Remove(owner);
		if (autoDestroy && this.materialOwners.Count == 0)
		{
			MaterialInstance.DestroySafe(this);
			if (!base.gameObject.activeInHierarchy)
			{
				this.RestoreRenderer();
			}
		}
	}

	// Token: 0x170006D5 RID: 1749
	// (get) Token: 0x06004852 RID: 18514 RVA: 0x00185646 File Offset: 0x00183846
	public Material Material
	{
		get
		{
			return this.AcquireMaterial(null, true);
		}
	}

	// Token: 0x170006D6 RID: 1750
	// (get) Token: 0x06004853 RID: 18515 RVA: 0x00185650 File Offset: 0x00183850
	public Material[] Materials
	{
		get
		{
			return this.AcquireMaterials(null, true);
		}
	}

	// Token: 0x170006D7 RID: 1751
	// (get) Token: 0x06004854 RID: 18516 RVA: 0x0018565A File Offset: 0x0018385A
	// (set) Token: 0x06004855 RID: 18517 RVA: 0x00185662 File Offset: 0x00183862
	public bool CacheSharedMaterialsFromRenderer
	{
		get
		{
			return this.cacheSharedMaterialsFromRenderer;
		}
		set
		{
			if (this.cacheSharedMaterialsFromRenderer != value)
			{
				if (value)
				{
					this.cachedSharedMaterials = this.CachedRenderer.sharedMaterials;
				}
				else
				{
					this.cachedSharedMaterials = null;
				}
				this.cacheSharedMaterialsFromRenderer = value;
			}
		}
	}

	// Token: 0x170006D8 RID: 1752
	// (get) Token: 0x06004856 RID: 18518 RVA: 0x00185691 File Offset: 0x00183891
	private Renderer CachedRenderer
	{
		get
		{
			if (this.cachedRenderer == null)
			{
				this.cachedRenderer = base.GetComponent<Renderer>();
				if (this.CacheSharedMaterialsFromRenderer)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
			}
			return this.cachedRenderer;
		}
	}

	// Token: 0x170006D9 RID: 1753
	// (get) Token: 0x06004857 RID: 18519 RVA: 0x001856CC File Offset: 0x001838CC
	// (set) Token: 0x06004858 RID: 18520 RVA: 0x00185701 File Offset: 0x00183901
	private Material[] CachedRendererSharedMaterials
	{
		get
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				if (this.cachedSharedMaterials == null)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
				return this.cachedSharedMaterials;
			}
			return this.cachedRenderer.sharedMaterials;
		}
		set
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				this.cachedSharedMaterials = value;
			}
			this.cachedRenderer.sharedMaterials = value;
		}
	}

	// Token: 0x06004859 RID: 18521 RVA: 0x0018571E File Offset: 0x0018391E
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x0600485A RID: 18522 RVA: 0x00185726 File Offset: 0x00183926
	private void OnDestroy()
	{
		this.RestoreRenderer();
	}

	// Token: 0x0600485B RID: 18523 RVA: 0x0018572E File Offset: 0x0018392E
	private void RestoreRenderer()
	{
		if (this.CachedRenderer != null && this.defaultMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.defaultMaterials;
		}
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = null;
	}

	// Token: 0x0600485C RID: 18524 RVA: 0x00185764 File Offset: 0x00183964
	private void Initialize()
	{
		if (!this.initialized && this.CachedRenderer != null)
		{
			if (!MaterialInstance.HasValidMaterial(this.defaultMaterials))
			{
				this.defaultMaterials = this.CachedRendererSharedMaterials;
			}
			else if (!this.materialsInstanced)
			{
				this.CachedRendererSharedMaterials = this.defaultMaterials;
			}
			this.initialized = true;
		}
	}

	// Token: 0x0600485D RID: 18525 RVA: 0x001857BD File Offset: 0x001839BD
	private void AcquireInstances()
	{
		if (this.CachedRenderer != null && !MaterialInstance.MaterialsMatch(this.CachedRendererSharedMaterials, this.instanceMaterials))
		{
			this.CreateInstances();
		}
	}

	// Token: 0x0600485E RID: 18526 RVA: 0x001857E8 File Offset: 0x001839E8
	private void CreateInstances()
	{
		this.Initialize();
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = MaterialInstance.InstanceMaterials(this.defaultMaterials);
		if (this.CachedRenderer != null && this.instanceMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.instanceMaterials;
		}
		this.materialsInstanced = true;
	}

	// Token: 0x0600485F RID: 18527 RVA: 0x00185840 File Offset: 0x00183A40
	private static bool MaterialsMatch(Material[] a, Material[] b)
	{
		int? num = ((a != null) ? new int?(a.Length) : null);
		int? num2 = ((b != null) ? new int?(b.Length) : null);
		if (!((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null))))
		{
			return false;
		}
		int num3 = 0;
		for (;;)
		{
			int num4 = num3;
			num2 = ((a != null) ? new int?(a.Length) : null);
			if (!((num4 < num2.GetValueOrDefault()) & (num2 != null)))
			{
				return true;
			}
			if (a[num3] != b[num3])
			{
				break;
			}
			num3++;
		}
		return false;
	}

	// Token: 0x06004860 RID: 18528 RVA: 0x001858E4 File Offset: 0x00183AE4
	private static Material[] InstanceMaterials(Material[] source)
	{
		if (source == null)
		{
			return null;
		}
		Material[] array = new Material[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] != null)
			{
				if (MaterialInstance.IsInstanceMaterial(source[i]))
				{
					Debug.LogWarning("A material (" + source[i].name + ") which is already instanced was instanced multiple times.");
				}
				array[i] = new Material(source[i]);
				Material material = array[i];
				material.name += " (Instance)";
			}
		}
		return array;
	}

	// Token: 0x06004861 RID: 18529 RVA: 0x00185964 File Offset: 0x00183B64
	private static void DestroyMaterials(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				MaterialInstance.DestroySafe(materials[i]);
			}
		}
	}

	// Token: 0x06004862 RID: 18530 RVA: 0x0018598A File Offset: 0x00183B8A
	private static bool IsInstanceMaterial(Material material)
	{
		return material != null && material.name.Contains(" (Instance)");
	}

	// Token: 0x06004863 RID: 18531 RVA: 0x001859A8 File Offset: 0x00183BA8
	private static bool HasValidMaterial(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004864 RID: 18532 RVA: 0x001859D6 File Offset: 0x00183BD6
	private static void DestroySafe(Object toDestroy)
	{
		if (toDestroy != null && Application.isPlaying)
		{
			Object.Destroy(toDestroy);
		}
	}

	// Token: 0x04005AF4 RID: 23284
	private Renderer cachedRenderer;

	// Token: 0x04005AF5 RID: 23285
	[SerializeField]
	[HideInInspector]
	private Material[] defaultMaterials;

	// Token: 0x04005AF6 RID: 23286
	private Material[] instanceMaterials;

	// Token: 0x04005AF7 RID: 23287
	private Material[] cachedSharedMaterials;

	// Token: 0x04005AF8 RID: 23288
	private bool initialized;

	// Token: 0x04005AF9 RID: 23289
	private bool materialsInstanced;

	// Token: 0x04005AFA RID: 23290
	[SerializeField]
	[Tooltip("Whether to use a cached copy of cachedRenderer.sharedMaterials or call sharedMaterials on the Renderer directly. Enabling the option will lead to better performance but you must turn it off before modifying sharedMaterials of the Renderer.")]
	private bool cacheSharedMaterialsFromRenderer;

	// Token: 0x04005AFB RID: 23291
	private readonly HashSet<Object> materialOwners = new HashSet<Object>();

	// Token: 0x04005AFC RID: 23292
	private const string instancePostfix = " (Instance)";
}
