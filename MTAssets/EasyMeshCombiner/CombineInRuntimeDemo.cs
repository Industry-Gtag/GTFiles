using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x0200115E RID: 4446
	public class CombineInRuntimeDemo : MonoBehaviour
	{
		// Token: 0x06006FA6 RID: 28582 RVA: 0x0023F93C File Offset: 0x0023DB3C
		private void Update()
		{
			if (!this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(true);
				this.undoButton.SetActive(false);
			}
			if (this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(false);
				this.undoButton.SetActive(true);
			}
		}

		// Token: 0x06006FA7 RID: 28583 RVA: 0x0023F993 File Offset: 0x0023DB93
		public void CombineMeshes()
		{
			this.runtimeCombiner.CombineMeshes();
		}

		// Token: 0x06006FA8 RID: 28584 RVA: 0x0023F9A1 File Offset: 0x0023DBA1
		public void UndoMerge()
		{
			this.runtimeCombiner.UndoMerge();
		}

		// Token: 0x04007F89 RID: 32649
		public GameObject combineButton;

		// Token: 0x04007F8A RID: 32650
		public GameObject undoButton;

		// Token: 0x04007F8B RID: 32651
		public RuntimeMeshCombiner runtimeCombiner;
	}
}
