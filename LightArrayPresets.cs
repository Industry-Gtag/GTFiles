using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200041C RID: 1052
[CreateAssetMenu(fileName = "LightArrayPresets", menuName = "Scriptable Objects/LightArrayPresets")]
public class LightArrayPresets : ScriptableObject
{
	// Token: 0x06001906 RID: 6406 RVA: 0x0008DF80 File Offset: 0x0008C180
	private void initLookup()
	{
		this.lookup = new Dictionary<string, LightArrayPresets.LightArrayPreset>();
		for (int i = 0; i < this.presets.Length; i++)
		{
			this.lookup.Add(this.presets[i].name, this.presets[i]);
		}
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x0008DFCB File Offset: 0x0008C1CB
	public LightArrayPresets.LightArrayPreset GetPreset(int i)
	{
		return this.presets[i];
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x0008DFD5 File Offset: 0x0008C1D5
	public LightArrayPresets.LightArrayPreset GetPreset(string n)
	{
		if (this.lookup == null)
		{
			this.initLookup();
		}
		return this.lookup[n];
	}

	// Token: 0x04002434 RID: 9268
	private Dictionary<string, LightArrayPresets.LightArrayPreset> lookup;

	// Token: 0x04002435 RID: 9269
	[SerializeField]
	private LightArrayPresets.LightArrayPreset[] presets;

	// Token: 0x0200041D RID: 1053
	[Serializable]
	public class LightArrayPreset
	{
		// Token: 0x04002436 RID: 9270
		public string name = "Color";

		// Token: 0x04002437 RID: 9271
		public Color color = Color.white;

		// Token: 0x04002438 RID: 9272
		public float intensity = 1f;
	}
}
