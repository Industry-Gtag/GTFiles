using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x02000E18 RID: 3608
public static class UnityYaml
{
	// Token: 0x04006887 RID: 26759
	private static readonly Assembly EngineAssembly = Assembly.GetAssembly(typeof(MonoBehaviour));

	// Token: 0x04006888 RID: 26760
	private static readonly Assembly TerrainAssembly = Assembly.GetAssembly(typeof(Tree));

	// Token: 0x04006889 RID: 26761
	public static Dictionary<int, Type> ClassIDToType = new Dictionary<int, Type>();
}
