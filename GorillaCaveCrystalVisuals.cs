using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000866 RID: 2150
public class GorillaCaveCrystalVisuals : MonoBehaviour
{
	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x060037BD RID: 14269 RVA: 0x001315A2 File Offset: 0x0012F7A2
	// (set) Token: 0x060037BE RID: 14270 RVA: 0x001315AA File Offset: 0x0012F7AA
	public float lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			this._lerp = value;
		}
	}

	// Token: 0x060037BF RID: 14271 RVA: 0x001315B4 File Offset: 0x0012F7B4
	public void Setup()
	{
		base.TryGetComponent<MeshRenderer>(out this._renderer);
		if (this._renderer == null)
		{
			return;
		}
		this._setup = GorillaCaveCrystalSetup.Instance;
		this._sharedMaterial = this._renderer.sharedMaterial;
		this._initialized = this.crysalPreset != null && this._renderer != null && this._sharedMaterial != null;
		this.Update();
	}

	// Token: 0x060037C0 RID: 14272 RVA: 0x00131630 File Offset: 0x0012F830
	private void Start()
	{
		this.UpdateAlbedo();
		this.ForceUpdate();
	}

	// Token: 0x060037C1 RID: 14273 RVA: 0x00131640 File Offset: 0x0012F840
	public void UpdateAlbedo()
	{
		if (!this._initialized)
		{
			return;
		}
		if (this.instanceAlbedo == null)
		{
			return;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetTexture(GorillaCaveCrystalVisuals._MainTex, this.instanceAlbedo);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060037C2 RID: 14274 RVA: 0x001316B5 File Offset: 0x0012F8B5
	private void Awake()
	{
		this.UpdateAlbedo();
		this.Update();
	}

	// Token: 0x060037C3 RID: 14275 RVA: 0x001316C4 File Offset: 0x0012F8C4
	private void Update()
	{
		if (!this._initialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			int hashCode = new ValueTuple<CrystalVisualsPreset, float>(this.crysalPreset, this._lerp).GetHashCode();
			if (this._lastState == hashCode)
			{
				return;
			}
			this._lastState = hashCode;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		CrystalVisualsPreset.VisualState stateA = this.crysalPreset.stateA;
		CrystalVisualsPreset.VisualState stateB = this.crysalPreset.stateB;
		Color color = Color.Lerp(stateA.albedo, stateB.albedo, this._lerp);
		Color color2 = Color.Lerp(stateA.emission, stateB.emission, this._lerp);
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetColor(GorillaCaveCrystalVisuals._Color, color);
		this._block.SetColor(GorillaCaveCrystalVisuals._EmissionColor, color2);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060037C4 RID: 14276 RVA: 0x001317BA File Offset: 0x0012F9BA
	public void ForceUpdate()
	{
		this._lastState = 0;
		this.Update();
	}

	// Token: 0x060037C5 RID: 14277 RVA: 0x001317CC File Offset: 0x0012F9CC
	private static void InitializeCrystals()
	{
		foreach (GorillaCaveCrystalVisuals gorillaCaveCrystalVisuals in Object.FindObjectsByType<GorillaCaveCrystalVisuals>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
		{
			gorillaCaveCrystalVisuals.UpdateAlbedo();
			gorillaCaveCrystalVisuals.ForceUpdate();
			gorillaCaveCrystalVisuals._lastState = -1;
		}
	}

	// Token: 0x040047E8 RID: 18408
	public CrystalVisualsPreset crysalPreset;

	// Token: 0x040047E9 RID: 18409
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerp;

	// Token: 0x040047EA RID: 18410
	[Space]
	public MeshRenderer _renderer;

	// Token: 0x040047EB RID: 18411
	public Material _sharedMaterial;

	// Token: 0x040047EC RID: 18412
	[SerializeField]
	public Texture2D instanceAlbedo;

	// Token: 0x040047ED RID: 18413
	[SerializeField]
	private bool _initialized;

	// Token: 0x040047EE RID: 18414
	[SerializeField]
	private int _lastState;

	// Token: 0x040047EF RID: 18415
	[SerializeField]
	public GorillaCaveCrystalSetup _setup;

	// Token: 0x040047F0 RID: 18416
	private MaterialPropertyBlock _block;

	// Token: 0x040047F1 RID: 18417
	[NonSerialized]
	private bool _ranSetupOnce;

	// Token: 0x040047F2 RID: 18418
	private static readonly ShaderHashId _Color = "_Color";

	// Token: 0x040047F3 RID: 18419
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";

	// Token: 0x040047F4 RID: 18420
	private static readonly ShaderHashId _MainTex = "_MainTex";
}
