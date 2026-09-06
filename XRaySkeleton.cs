using System;
using UnityEngine;

// Token: 0x020005B2 RID: 1458
public class XRaySkeleton : SyncToPlayerColor, IGorillaSimpleBackgroundWorker
{
	// Token: 0x060024F6 RID: 9462 RVA: 0x000C6425 File Offset: 0x000C4625
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x000C6430 File Offset: 0x000C4630
	internal void OnBuildInitialize()
	{
		this.target = this.renderer.material;
		this.mats = this.rig.materialsToChangeTo;
		this.tagMaterials = new Material[this.mats.Length];
		this.tagMaterials[0] = new Material(this.target);
		while (this.currentIndex < this.mats.Length)
		{
			Material material = new Material(this.mats[this.currentIndex]);
			this.tagMaterials[this.currentIndex] = material;
			this.currentIndex++;
		}
	}

	// Token: 0x060024F8 RID: 9464 RVA: 0x000C64C8 File Offset: 0x000C46C8
	public void SimpleWork()
	{
		if (this.currentIndex >= 0 && this.currentIndex < this.mats.Length)
		{
			Material material = new Material(this.mats[this.currentIndex]);
			this.tagMaterials[this.currentIndex] = material;
			this.currentIndex++;
			GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
		}
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x000C6523 File Offset: 0x000C4723
	public void SetMaterialIndex(int index)
	{
		this.renderer.sharedMaterial = this.tagMaterials[index];
		this._lastMatIndex = index;
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x000C653F File Offset: 0x000C473F
	private void Setup()
	{
		this.colorPropertiesToSync = new ShaderHashId[]
		{
			XRaySkeleton._BaseColor,
			XRaySkeleton._EmissionColor
		};
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x000C6568 File Offset: 0x000C4768
	public override void UpdateColor(Color color)
	{
		if (this._lastMatIndex != 0)
		{
			return;
		}
		Material material = this.tagMaterials[0];
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(color, out num, out num2, out num3);
		Color color2 = Color.HSVToRGB(num, num2, Mathf.Clamp(num3, this.baseValueMinMax.x, this.baseValueMinMax.y));
		material.SetColor(XRaySkeleton._BaseColor, color2);
		float num4;
		float num5;
		float num6;
		Color.RGBToHSV(color, out num4, out num5, out num6);
		Color color3 = Color.HSVToRGB(num4, 0.82f, 0.9f, true);
		color3 = new Color(color3.r * 1.4f, color3.g * 1.4f, color3.b * 1.4f);
		material.SetColor(XRaySkeleton._EmissionColor, ColorUtils.ComposeHDR(new Color32(36, 191, 136, byte.MaxValue), 2f));
		this.renderer.sharedMaterial = material;
	}

	// Token: 0x04003076 RID: 12406
	public SkinnedMeshRenderer renderer;

	// Token: 0x04003077 RID: 12407
	public Vector2 baseValueMinMax = new Vector2(0.69f, 1f);

	// Token: 0x04003078 RID: 12408
	public Material[] tagMaterials = new Material[0];

	// Token: 0x04003079 RID: 12409
	private int _lastMatIndex;

	// Token: 0x0400307A RID: 12410
	private Material[] mats;

	// Token: 0x0400307B RID: 12411
	private int currentIndex = 1;

	// Token: 0x0400307C RID: 12412
	private static readonly ShaderHashId _BaseColor = "_BaseColor";

	// Token: 0x0400307D RID: 12413
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";
}
