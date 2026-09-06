using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x020013FD RID: 5117
	[CreateAssetMenu(fileName = "New Mesh Material Replacement", menuName = "Game Object Scheduling/New Mesh Material Replacement", order = 1)]
	public class MeshMaterialReplacement : ScriptableObject
	{
		// Token: 0x040091FF RID: 37375
		public Mesh mesh;

		// Token: 0x04009200 RID: 37376
		public Material[] materials;
	}
}
