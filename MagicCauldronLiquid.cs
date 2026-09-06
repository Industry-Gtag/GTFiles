using System;
using UnityEngine;

// Token: 0x0200091A RID: 2330
public class MagicCauldronLiquid : MonoBehaviour
{
	// Token: 0x06003D06 RID: 15622 RVA: 0x0014C327 File Offset: 0x0014A527
	private void Test()
	{
		this._animProgress = 0f;
		this._animating = true;
		base.enabled = true;
	}

	// Token: 0x06003D07 RID: 15623 RVA: 0x0014C342 File Offset: 0x0014A542
	public void AnimateColorFromTo(Color a, Color b, float length = 1f)
	{
		this._colorStart = a;
		this._colorEnd = b;
		this._animProgress = 0f;
		this._animating = true;
		this.animLength = length;
		base.enabled = true;
	}

	// Token: 0x06003D08 RID: 15624 RVA: 0x0014C372 File Offset: 0x0014A572
	private void ApplyColor(Color color)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetColor(ShaderProps._BaseColor, color);
		this._applyMaterial.Apply();
	}

	// Token: 0x06003D09 RID: 15625 RVA: 0x0014C3A0 File Offset: 0x0014A5A0
	private void ApplyWaveParams(float amplitude, float frequency, float scale, float rotation)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetFloat(ShaderProps._WaveAmplitude, amplitude);
		this._applyMaterial.SetFloat(ShaderProps._WaveFrequency, frequency);
		this._applyMaterial.SetFloat(ShaderProps._WaveScale, scale);
		this._applyMaterial.Apply();
	}

	// Token: 0x06003D0A RID: 15626 RVA: 0x0014C3F9 File Offset: 0x0014A5F9
	private void OnEnable()
	{
		if (this._applyMaterial)
		{
			this._applyMaterial.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
	}

	// Token: 0x06003D0B RID: 15627 RVA: 0x0014C414 File Offset: 0x0014A614
	private void OnDisable()
	{
		this._animating = false;
		this._animProgress = 0f;
	}

	// Token: 0x06003D0C RID: 15628 RVA: 0x0014C428 File Offset: 0x0014A628
	private void Update()
	{
		if (!this._animating)
		{
			return;
		}
		float num = this._animationCurve.Evaluate(this._animProgress / this.animLength);
		float num2 = this._waveCurve.Evaluate(this._animProgress / this.animLength);
		if (num >= 1f)
		{
			this.ApplyColor(this._colorEnd);
			this._animating = false;
			base.enabled = false;
			return;
		}
		Color color = Color.Lerp(this._colorStart, this._colorEnd, num);
		Mathf.Lerp(this.waveNormal.frequency, this.waveAnimating.frequency, num2);
		Mathf.Lerp(this.waveNormal.amplitude, this.waveAnimating.amplitude, num2);
		Mathf.Lerp(this.waveNormal.scale, this.waveAnimating.scale, num2);
		Mathf.Lerp(this.waveNormal.rotation, this.waveAnimating.rotation, num2);
		this.ApplyColor(color);
		this._animProgress += Time.deltaTime;
	}

	// Token: 0x04004DB9 RID: 19897
	[SerializeField]
	private ApplyMaterialProperty _applyMaterial;

	// Token: 0x04004DBA RID: 19898
	[SerializeField]
	private Color _colorStart;

	// Token: 0x04004DBB RID: 19899
	[SerializeField]
	private Color _colorEnd;

	// Token: 0x04004DBC RID: 19900
	[SerializeField]
	private bool _animating;

	// Token: 0x04004DBD RID: 19901
	[SerializeField]
	private float _animProgress;

	// Token: 0x04004DBE RID: 19902
	[SerializeField]
	private AnimationCurve _animationCurve = AnimationCurves.EaseOutCubic;

	// Token: 0x04004DBF RID: 19903
	[SerializeField]
	private AnimationCurve _waveCurve = AnimationCurves.EaseInElastic;

	// Token: 0x04004DC0 RID: 19904
	public float animLength = 1f;

	// Token: 0x04004DC1 RID: 19905
	public MagicCauldronLiquid.WaveParams waveNormal;

	// Token: 0x04004DC2 RID: 19906
	public MagicCauldronLiquid.WaveParams waveAnimating;

	// Token: 0x0200091B RID: 2331
	[Serializable]
	public struct WaveParams
	{
		// Token: 0x04004DC3 RID: 19907
		public float amplitude;

		// Token: 0x04004DC4 RID: 19908
		public float frequency;

		// Token: 0x04004DC5 RID: 19909
		public float scale;

		// Token: 0x04004DC6 RID: 19910
		public float rotation;
	}
}
