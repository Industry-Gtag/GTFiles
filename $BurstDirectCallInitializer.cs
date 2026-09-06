using System;
using Unity.Burst;
using UnityEngine;

// Token: 0x020014AD RID: 5293
internal static class $BurstDirectCallInitializer
{
	// Token: 0x06008424 RID: 33828 RVA: 0x002B22B4 File Offset: 0x002B04B4
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Initialize()
	{
		BurstCompilerOptions options = BurstCompiler.Options;
	}
}
