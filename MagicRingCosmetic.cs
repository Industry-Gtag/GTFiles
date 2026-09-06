using System;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class MagicRingCosmetic : MonoBehaviour
{
	// Token: 0x06000DF3 RID: 3571 RVA: 0x0004C74A File Offset: 0x0004A94A
	protected void Awake()
	{
		this.materialPropertyBlock = new MaterialPropertyBlock();
		this.defaultEmissiveColor = this.ringRenderer.sharedMaterial.GetColor(ShaderProps._EmissionColor);
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x0004C774 File Offset: 0x0004A974
	protected void LateUpdate()
	{
		float celsius = this.thermalReceiver.celsius;
		if (celsius >= this.fadeInTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedIn)
		{
			this.fadeInSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedIn;
		}
		else if (celsius <= this.fadeOutTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedOut)
		{
			this.fadeOutSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedOut;
		}
		this.emissiveAmount = Mathf.MoveTowards(this.emissiveAmount, (this.fadeState == MagicRingCosmetic.FadeState.FadedIn) ? 1f : 0f, Time.deltaTime / this.fadeTime);
		this.ringRenderer.GetPropertyBlock(this.materialPropertyBlock);
		this.materialPropertyBlock.SetColor(ShaderProps._EmissionColor, new Color(this.defaultEmissiveColor.r, this.defaultEmissiveColor.g, this.defaultEmissiveColor.b, this.emissiveAmount));
		this.materialPropertyBlock.SetFloat(ShaderProps._EmissiveAmount, this.emissiveAmount);
		this.ringRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x0400109F RID: 4255
	[Tooltip("The ring will fade in the emissive texture based on temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x040010A0 RID: 4256
	public Renderer ringRenderer;

	// Token: 0x040010A1 RID: 4257
	public float fadeInTemperatureThreshold = 200f;

	// Token: 0x040010A2 RID: 4258
	public float fadeOutTemperatureThreshold = 190f;

	// Token: 0x040010A3 RID: 4259
	public float fadeTime = 1.5f;

	// Token: 0x040010A4 RID: 4260
	public SoundBankPlayer fadeInSounds;

	// Token: 0x040010A5 RID: 4261
	public SoundBankPlayer fadeOutSounds;

	// Token: 0x040010A6 RID: 4262
	private MagicRingCosmetic.FadeState fadeState;

	// Token: 0x040010A7 RID: 4263
	private Color defaultEmissiveColor;

	// Token: 0x040010A8 RID: 4264
	private float emissiveAmount;

	// Token: 0x040010A9 RID: 4265
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x02000212 RID: 530
	private enum FadeState
	{
		// Token: 0x040010AB RID: 4267
		FadedOut,
		// Token: 0x040010AC RID: 4268
		FadedIn
	}
}
