using System;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x0200125D RID: 4701
	[CreateAssetMenu(fileName = "MeshGenerator", menuName = "ScriptableObjects/MeshGenerator", order = 1)]
	public class MonkeFXSettingsSO : ScriptableObject
	{
		// Token: 0x06007705 RID: 30469 RVA: 0x002699BE File Offset: 0x00267BBE
		protected void Awake()
		{
			MonkeFX.Register(this);
		}

		// Token: 0x040086B1 RID: 34481
		public GTDirectAssetRef<Mesh>[] sourceMeshes;

		// Token: 0x040086B2 RID: 34482
		[HideInInspector]
		public Mesh combinedMesh;
	}
}
