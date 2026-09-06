using System;
using UnityEngine;

// Token: 0x020005E2 RID: 1506
public class GorillaSceneCamera : MonoBehaviour
{
	// Token: 0x060025B6 RID: 9654 RVA: 0x000C87CE File Offset: 0x000C69CE
	public void SetSceneCamera(int sceneIndex)
	{
		base.transform.position = this.sceneTransforms[sceneIndex].scenePosition;
		base.transform.eulerAngles = this.sceneTransforms[sceneIndex].sceneRotation;
	}

	// Token: 0x04003151 RID: 12625
	public GorillaSceneTransform[] sceneTransforms;
}
