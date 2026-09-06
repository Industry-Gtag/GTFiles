using System;
using UnityEngine;

// Token: 0x02000E7D RID: 3709
public class MaterialMapping : ScriptableObject
{
	// Token: 0x06005A3C RID: 23100 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void CleanUpData()
	{
	}

	// Token: 0x04006B33 RID: 27443
	private static string path = "Assets/UberShaderConversion/MaterialMap.asset";

	// Token: 0x04006B34 RID: 27444
	public static string materialDirectory = "Assets/UberShaderConversion/Materials/";

	// Token: 0x04006B35 RID: 27445
	private static MaterialMapping instance;

	// Token: 0x04006B36 RID: 27446
	public ShaderGroup[] map;

	// Token: 0x04006B37 RID: 27447
	public Material mirrorMat;

	// Token: 0x04006B38 RID: 27448
	public RenderTexture mirrorTexture;
}
