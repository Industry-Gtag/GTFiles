using System;
using UnityEngine;

// Token: 0x02000D1B RID: 3355
public class CopyMaterialScript : MonoBehaviour
{
	// Token: 0x06005333 RID: 21299 RVA: 0x001B71E2 File Offset: 0x001B53E2
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06005334 RID: 21300 RVA: 0x001B71F9 File Offset: 0x001B53F9
	private void Update()
	{
		if (this.sourceToCopyMaterialFrom.material != this.mySkinnedMeshRenderer.material)
		{
			this.mySkinnedMeshRenderer.material = this.sourceToCopyMaterialFrom.material;
		}
	}

	// Token: 0x040064D7 RID: 25815
	public SkinnedMeshRenderer sourceToCopyMaterialFrom;

	// Token: 0x040064D8 RID: 25816
	public SkinnedMeshRenderer mySkinnedMeshRenderer;
}
