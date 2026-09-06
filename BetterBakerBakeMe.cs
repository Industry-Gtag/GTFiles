using System;
using System.Collections.Generic;
using GorillaTag.Rendering.Shaders;
using UnityEngine;

// Token: 0x02000D87 RID: 3463
public class BetterBakerBakeMe : FlagForBaking
{
	// Token: 0x040066D8 RID: 26328
	public GameObject[] stuffIncludingParentsToBake;

	// Token: 0x040066D9 RID: 26329
	public GameObject getMatStuffFromHere;

	// Token: 0x040066DA RID: 26330
	public List<ShaderConfigData.ShaderConfig> allConfigs = new List<ShaderConfigData.ShaderConfig>();
}
