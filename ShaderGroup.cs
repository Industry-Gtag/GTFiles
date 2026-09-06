using System;
using UnityEngine;

// Token: 0x02000E7E RID: 3710
[Serializable]
public struct ShaderGroup
{
	// Token: 0x06005A3F RID: 23103 RVA: 0x001D4A76 File Offset: 0x001D2C76
	public ShaderGroup(Material material, Shader original, Shader gameplay, Shader baking)
	{
		this.material = material;
		this.originalShader = original;
		this.gameplayShader = gameplay;
		this.bakingShader = baking;
	}

	// Token: 0x04006B39 RID: 27449
	public Material material;

	// Token: 0x04006B3A RID: 27450
	public Shader originalShader;

	// Token: 0x04006B3B RID: 27451
	public Shader gameplayShader;

	// Token: 0x04006B3C RID: 27452
	public Shader bakingShader;
}
