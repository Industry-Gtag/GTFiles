using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020001BD RID: 445
public class SkyboxController : MonoBehaviour
{
	// Token: 0x06000BEA RID: 3050 RVA: 0x000411C4 File Offset: 0x0003F3C4
	private void Start()
	{
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00041252 File Offset: 0x0003F452
	private void Update()
	{
		if (!this.lastUpdate.HasElapsed(1f, true))
		{
			return;
		}
		this.UpdateTime();
		this.UpdateSky();
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00041274 File Offset: 0x0003F474
	private void OnValidate()
	{
		this.UpdateSky();
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0004127C File Offset: 0x0003F47C
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x000412B4 File Offset: 0x0003F4B4
	private void UpdateSky()
	{
		if (this.skyMaterials == null || this.skyMaterials.Length == 0)
		{
			return;
		}
		int num = this.skyMaterials.Length;
		float num2 = Mathf.Clamp(this._currentTime, 0f, 1f);
		float num3 = 1f / (float)num;
		int num4 = (int)(num2 / num3);
		float num5 = (num2 - (float)num4 * num3) / num3;
		this._currentSky = this.skyMaterials[num4];
		this._nextSky = this.skyMaterials[(num4 + 1) % num];
		this.skyFront.sharedMaterial = this._currentSky;
		this.skyBack.sharedMaterial = this._nextSky;
		if (this._currentSky != null && this._currentSky.renderQueue != 3000)
		{
			this.SetFrontToTransparent();
			this._currentSky.SetFloat(ShaderProps._SkyAlpha, 1f - num5);
		}
		if (this._nextSky != null && this._nextSky.renderQueue == 3000)
		{
			this.SetBackToOpaque();
		}
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x000413AC File Offset: 0x0003F5AC
	private void SetFrontToTransparent()
	{
		bool flag = false;
		bool flag2 = false;
		string text = "Transparent";
		int num = 3000;
		BlendMode blendMode = BlendMode.SrcAlpha;
		BlendMode blendMode2 = BlendMode.OneMinusSrcAlpha;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.OneMinusSrcAlpha;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag2 ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = num;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x0004146C File Offset: 0x0003F66C
	private void SetFrontToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string text = "Opaque";
		int num = 2000;
		BlendMode blendMode = BlendMode.One;
		BlendMode blendMode2 = BlendMode.Zero;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.Zero;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = num;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0004152C File Offset: 0x0003F72C
	private void SetBackToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string text = "Opaque";
		int num = 2000;
		BlendMode blendMode = BlendMode.One;
		BlendMode blendMode2 = BlendMode.Zero;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.Zero;
		Material sharedMaterial = this.skyBack.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = num;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x04000E7B RID: 3707
	public MeshRenderer skyFront;

	// Token: 0x04000E7C RID: 3708
	public MeshRenderer skyBack;

	// Token: 0x04000E7D RID: 3709
	public Material[] skyMaterials = new Material[0];

	// Token: 0x04000E7E RID: 3710
	[Range(0f, 1f)]
	public float lerpValue;

	// Token: 0x04000E7F RID: 3711
	[NonSerialized]
	private Material _currentSky;

	// Token: 0x04000E80 RID: 3712
	[NonSerialized]
	private Material _nextSky;

	// Token: 0x04000E81 RID: 3713
	private TimeSince lastUpdate = TimeSince.Now();

	// Token: 0x04000E82 RID: 3714
	[Space]
	private BetterDayNightManager _dayNightManager;

	// Token: 0x04000E83 RID: 3715
	private double _currentSeconds = -1.0;

	// Token: 0x04000E84 RID: 3716
	private double _totalSecondsInRange = -1.0;

	// Token: 0x04000E85 RID: 3717
	private float _currentTime = -1f;
}
