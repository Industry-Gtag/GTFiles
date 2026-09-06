using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class VoxelMaterialManager : MonoBehaviour
{
	// Token: 0x06000D3A RID: 3386 RVA: 0x00048A07 File Offset: 0x00046C07
	private void OnEnable()
	{
		this.SetLightingProfile(this.startingIndex);
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00048A15 File Offset: 0x00046C15
	private void Update()
	{
		if (this._timeOfDayIndex != BetterDayNightManager.instance.currentTimeIndex)
		{
			this.UpdateMaterial();
		}
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00048A34 File Offset: 0x00046C34
	private void UpdateMaterial()
	{
		string currentTimeOfDay = BetterDayNightManager.instance.currentTimeOfDay;
		if (string.IsNullOrEmpty(currentTimeOfDay))
		{
			return;
		}
		int num = this.lightmapNames.IndexOf(currentTimeOfDay);
		if (num < 0 || num >= this.lightingProfiles.Count)
		{
			return;
		}
		this.SetLightingProfile(num);
		this._timeOfDayIndex = BetterDayNightManager.instance.currentTimeIndex;
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00048A90 File Offset: 0x00046C90
	private void SetLightingProfile(int index)
	{
		index = Mathf.Clamp(index, 0, this.lightingProfiles.Count - 1);
		VoxelMaterialManager.LightingProfile lightingProfile = this.lightingProfiles[index];
		Shader.SetGlobalVector("_Light_Direction", lightingProfile.direction);
		Shader.SetGlobalColor("_Light_Color", lightingProfile.color);
		Shader.SetGlobalColor("_Shadow_Color", lightingProfile.color * this.shadowBrightness);
		Shader.SetGlobalColor("_Backlight_Color", lightingProfile.color * this.backlightBrightness);
		this._timeOfDayIndex = index;
	}

	// Token: 0x04000FD4 RID: 4052
	public Material[] voxelMats;

	// Token: 0x04000FD5 RID: 4053
	public List<string> lightmapNames;

	// Token: 0x04000FD6 RID: 4054
	public List<VoxelMaterialManager.LightingProfile> lightingProfiles;

	// Token: 0x04000FD7 RID: 4055
	[Range(0f, 1f)]
	[SerializeField]
	private float shadowBrightness = 0.3f;

	// Token: 0x04000FD8 RID: 4056
	[Range(0f, 1f)]
	[SerializeField]
	private float backlightBrightness = 0.2f;

	// Token: 0x04000FD9 RID: 4057
	[SerializeField]
	private int startingIndex = 2;

	// Token: 0x04000FDA RID: 4058
	private int _timeOfDayIndex = -1;

	// Token: 0x020001F7 RID: 503
	[Serializable]
	public struct LightingProfile
	{
		// Token: 0x04000FDB RID: 4059
		public Color color;

		// Token: 0x04000FDC RID: 4060
		public Vector3 direction;
	}
}
