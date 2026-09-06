using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000415 RID: 1045
public class LightArray : MonoBehaviour
{
	// Token: 0x060018EA RID: 6378 RVA: 0x0008CE4A File Offset: 0x0008B04A
	private void ToggleDynamicLighting()
	{
		GameLightingManager.instance.ToggleCustomDynamicLightingEnabled();
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x0008CE58 File Offset: 0x0008B058
	public void SetCascadeTime(int ct)
	{
		this.cascadeTime = ct;
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x0008CE64 File Offset: 0x0008B064
	public void SetSubArraysCascadeTime(int ct)
	{
		for (int i = 0; i < this.subArrays.Length; i++)
		{
			this.subArrays[i].cascadeTime = ct;
		}
	}

	// Token: 0x060018ED RID: 6381 RVA: 0x0008CE94 File Offset: 0x0008B094
	public void SetPreset(int i)
	{
		if (this.presets == null)
		{
			return;
		}
		LightArrayPresets.LightArrayPreset preset = this.presets.GetPreset(i);
		if (preset == null)
		{
			return;
		}
		this.SetColorAndIntensity(preset.color, preset.intensity);
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x0008CED4 File Offset: 0x0008B0D4
	public void SetPreset(string n)
	{
		if (this.presets == null)
		{
			return;
		}
		LightArrayPresets.LightArrayPreset preset = this.presets.GetPreset(n);
		if (preset == null)
		{
			return;
		}
		this.SetColorAndIntensity(preset.color, preset.intensity);
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x0008CF13 File Offset: 0x0008B113
	public void SetColorAndIntensity(string RRGGBBF)
	{
		this.SetColorAndIntensity(this.GetColor(RRGGBBF), float.Parse(RRGGBBF.Substring(6).ToString()));
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x0008CF34 File Offset: 0x0008B134
	private async void SetColorAndIntensity(Color c, float intensity)
	{
		if (this.cascadeTime < 0)
		{
			for (int i = this.subArrays.Length - 1; i >= 0; i--)
			{
				this.subArrays[i].SetColorAndIntensity(c, intensity);
				await Task.Delay(Mathf.Abs(this.cascadeTime));
			}
		}
		else
		{
			for (int i = 0; i < this.subArrays.Length; i++)
			{
				this.subArrays[i].SetColorAndIntensity(c, intensity);
				await Task.Delay(Mathf.Abs(this.cascadeTime));
			}
		}
		if (this.cascadeTime < 0)
		{
			for (int i = this.lights.Length - 1; i >= 0; i--)
			{
				await this.SetLightColorAndIntensity(c, intensity, i);
			}
		}
		else
		{
			for (int i = 0; i < this.lights.Length; i++)
			{
				await this.SetLightColorAndIntensity(c, intensity, i);
			}
		}
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x0008CF7B File Offset: 0x0008B17B
	public void SetColor(string RRGGBB)
	{
		this.SetColor(this.GetColor(RRGGBB));
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x0008CF8C File Offset: 0x0008B18C
	private async void SetColor(Color c)
	{
		if (this.cascadeTime < 0)
		{
			for (int i = this.subArrays.Length - 1; i >= 0; i--)
			{
				this.subArrays[i].SetColor(c);
				await Task.Delay(Mathf.Abs(this.cascadeTime));
			}
		}
		else
		{
			for (int i = 0; i < this.subArrays.Length; i++)
			{
				this.subArrays[i].SetColor(c);
				await Task.Delay(Mathf.Abs(this.cascadeTime));
			}
		}
		if (this.cascadeTime < 0)
		{
			for (int i = this.lights.Length - 1; i >= 0; i--)
			{
				await this.SetLightColor(c, i);
			}
		}
		else
		{
			for (int i = 0; i < this.lights.Length; i++)
			{
				await this.SetLightColor(c, i);
			}
		}
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x0008CFCC File Offset: 0x0008B1CC
	public async void SetIntensity(float intensity)
	{
		if (this.cascadeTime < 0)
		{
			for (int i = this.subArrays.Length - 1; i >= 0; i--)
			{
				this.subArrays[i].SetIntensity(intensity);
				await Task.Delay(Mathf.Abs(this.cascadeTime));
			}
		}
		else
		{
			for (int i = 0; i < this.subArrays.Length; i++)
			{
				this.subArrays[i].SetIntensity(intensity);
				await Task.Delay(Mathf.Abs(this.cascadeTime));
			}
		}
		if (this.cascadeTime < 0)
		{
			for (int i = this.lights.Length - 1; i >= 0; i--)
			{
				await this.SetLightIntensity(intensity, i);
			}
		}
		else
		{
			for (int i = 0; i < this.lights.Length; i++)
			{
				await this.SetLightIntensity(intensity, i);
			}
		}
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x0008D00C File Offset: 0x0008B20C
	private async Task SetLightColorAndIntensity(Color c, float intensity, int i)
	{
		this.lights[i].light.color = c;
		this.lights[i].light.intensity = intensity;
		this.lights[i].UpdateCachedLightColorAndIntensity();
		if (this.cascadeTime != 0)
		{
			await Task.Delay(Mathf.Abs(this.cascadeTime));
		}
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x0008D068 File Offset: 0x0008B268
	private async Task SetLightColor(Color c, int i)
	{
		this.lights[i].light.color = c;
		this.lights[i].UpdateCachedLightColorAndIntensity();
		if (this.cascadeTime != 0)
		{
			await Task.Delay(Mathf.Abs(this.cascadeTime));
		}
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x0008D0BC File Offset: 0x0008B2BC
	private async Task SetLightIntensity(float intensity, int i)
	{
		this.lights[i].light.intensity = intensity;
		this.lights[i].UpdateCachedLightColorAndIntensity();
		if (this.cascadeTime != 0)
		{
			await Task.Delay(Mathf.Abs(this.cascadeTime));
		}
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x0008D110 File Offset: 0x0008B310
	private Color GetColor(string RRGGBB)
	{
		return new Color((float)int.Parse(RRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
	}

	// Token: 0x060018F8 RID: 6392 RVA: 0x0008D170 File Offset: 0x0008B370
	private void LateUpdate()
	{
		bool flag = false;
		bool flag2 = false;
		if (this.preLightHue != this.setLightHue)
		{
			flag = true;
			this.preLightHue = this.setLightHue;
		}
		if (this.preLightSat != this.setLightSat)
		{
			flag = true;
			this.preLightSat = this.setLightSat;
		}
		if (this.preLightVal != this.setLightVal)
		{
			flag = true;
			this.preLightVal = this.setLightVal;
		}
		if (this.preLightIntensity != this.setLightIntensity)
		{
			flag2 = true;
			this.preLightIntensity = this.setLightIntensity;
		}
		if (flag && flag2)
		{
			this.SetColorAndIntensity(Color.HSVToRGB(this.setLightHue, this.setLightSat, this.setLightVal), this.setLightIntensity);
			return;
		}
		if (flag)
		{
			this.SetColor(Color.HSVToRGB(this.setLightHue, this.setLightSat, this.setLightVal));
			return;
		}
		if (flag2)
		{
			this.SetIntensity(this.setLightIntensity);
		}
	}

	// Token: 0x04002402 RID: 9218
	[SerializeField]
	private LightArrayPresets presets;

	// Token: 0x04002403 RID: 9219
	[SerializeField]
	private GameLight[] lights;

	// Token: 0x04002404 RID: 9220
	[SerializeField]
	private LightArray[] subArrays;

	// Token: 0x04002405 RID: 9221
	[SerializeField]
	private int cascadeTime;

	// Token: 0x04002406 RID: 9222
	[SerializeField]
	private float setLightHue = -1f;

	// Token: 0x04002407 RID: 9223
	[NonSerialized]
	private float preLightHue = -1f;

	// Token: 0x04002408 RID: 9224
	[SerializeField]
	private float setLightSat = -1f;

	// Token: 0x04002409 RID: 9225
	[NonSerialized]
	private float preLightSat = -1f;

	// Token: 0x0400240A RID: 9226
	[SerializeField]
	private float setLightVal = -1f;

	// Token: 0x0400240B RID: 9227
	[NonSerialized]
	private float preLightVal = -1f;

	// Token: 0x0400240C RID: 9228
	[SerializeField]
	private float setLightIntensity = -1f;

	// Token: 0x0400240D RID: 9229
	[NonSerialized]
	private float preLightIntensity = -1f;
}
