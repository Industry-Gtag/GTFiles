using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000FC1 RID: 4033
	public class CMSTryOnArea : MonoBehaviour
	{
		// Token: 0x0600644D RID: 25677 RVA: 0x002043CA File Offset: 0x002025CA
		public void InitializeForCustomMap(CompositeTriggerEvents customMapTryOnArea, Scene customMapScene)
		{
			this.originalScene = customMapScene;
			if (this.tryOnAreaCollider.IsNull())
			{
				return;
			}
			customMapTryOnArea.AddCollider(this.tryOnAreaCollider);
		}

		// Token: 0x0600644E RID: 25678 RVA: 0x002043ED File Offset: 0x002025ED
		public void RemoveFromCustomMap(CompositeTriggerEvents customMapTryOnArea)
		{
			if (this.tryOnAreaCollider.IsNull())
			{
				return;
			}
			customMapTryOnArea.RemoveCollider(this.tryOnAreaCollider);
		}

		// Token: 0x0600644F RID: 25679 RVA: 0x00204409 File Offset: 0x00202609
		public bool IsFromScene(Scene unloadingScene)
		{
			return unloadingScene == this.originalScene;
		}

		// Token: 0x04007311 RID: 29457
		private Scene originalScene;

		// Token: 0x04007312 RID: 29458
		public BoxCollider tryOnAreaCollider;
	}
}
