using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F43 RID: 3907
	public class MoleTypes : MonoBehaviour
	{
		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x06005FE3 RID: 24547 RVA: 0x001E6BA5 File Offset: 0x001E4DA5
		// (set) Token: 0x06005FE4 RID: 24548 RVA: 0x001E6BAD File Offset: 0x001E4DAD
		public bool IsLeftSideMoleType { get; set; }

		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x06005FE5 RID: 24549 RVA: 0x001E6BB6 File Offset: 0x001E4DB6
		// (set) Token: 0x06005FE6 RID: 24550 RVA: 0x001E6BBE File Offset: 0x001E4DBE
		public Mole MoleContainerParent { get; set; }

		// Token: 0x06005FE7 RID: 24551 RVA: 0x001E6BC7 File Offset: 0x001E4DC7
		private void Start()
		{
			this.MoleContainerParent = base.GetComponentInParent<Mole>();
			if (this.MoleContainerParent)
			{
				this.IsLeftSideMoleType = this.MoleContainerParent.IsLeftSideMole;
			}
		}

		// Token: 0x04006E60 RID: 28256
		public bool isHazard;

		// Token: 0x04006E61 RID: 28257
		public int scorePoint = 1;

		// Token: 0x04006E62 RID: 28258
		public MeshRenderer MeshRenderer;

		// Token: 0x04006E63 RID: 28259
		public Material monkeMoleDefaultMaterial;

		// Token: 0x04006E64 RID: 28260
		public Material monkeMoleHitMaterial;
	}
}
