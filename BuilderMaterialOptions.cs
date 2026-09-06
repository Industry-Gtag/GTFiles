using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000639 RID: 1593
[CreateAssetMenu(fileName = "BuilderMaterialOptions01a", menuName = "Gorilla Tag/Builder/Options", order = 0)]
public class BuilderMaterialOptions : ScriptableObject
{
	// Token: 0x060027BB RID: 10171 RVA: 0x000D2CA8 File Offset: 0x000D0EA8
	public void GetMaterialFromType(int materialType, out Material material, out int soundIndex)
	{
		if (this.options == null)
		{
			material = null;
			soundIndex = -1;
			return;
		}
		foreach (BuilderMaterialOptions.Options options in this.options)
		{
			if (options.materialId.GetHashCode() == materialType)
			{
				material = options.material;
				soundIndex = options.soundIndex;
				return;
			}
		}
		material = null;
		soundIndex = -1;
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x000D2D2C File Offset: 0x000D0F2C
	public void GetDefaultMaterial(out int materialType, out Material material, out int soundIndex)
	{
		if (this.options.Count > 0)
		{
			materialType = this.options[0].materialId.GetHashCode();
			material = this.options[0].material;
			soundIndex = this.options[0].soundIndex;
			return;
		}
		materialType = -1;
		material = null;
		soundIndex = -1;
	}

	// Token: 0x04003376 RID: 13174
	public List<BuilderMaterialOptions.Options> options;

	// Token: 0x0200063A RID: 1594
	[Serializable]
	public class Options
	{
		// Token: 0x04003377 RID: 13175
		public string materialId;

		// Token: 0x04003378 RID: 13176
		public Material material;

		// Token: 0x04003379 RID: 13177
		[GorillaSoundLookup]
		public int soundIndex;

		// Token: 0x0400337A RID: 13178
		[NonSerialized]
		public int materialType;
	}
}
