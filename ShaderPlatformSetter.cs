using System;
using UnityEngine;

// Token: 0x0200038F RID: 911
public static class ShaderPlatformSetter
{
	// Token: 0x06001614 RID: 5652 RVA: 0x0007D62D File Offset: 0x0007B82D
	[RuntimeInitializeOnLoadMethod]
	public static void HandleRuntimeInitializeOnLoad()
	{
		Shader.DisableKeyword("PLATFORM_IS_ANDROID");
		Shader.DisableKeyword("QATESTING");
	}
}
