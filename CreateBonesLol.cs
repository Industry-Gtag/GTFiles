using System;
using UnityEngine;

// Token: 0x020005B3 RID: 1459
public class CreateBonesLol : MonoBehaviour
{
	// Token: 0x060024FE RID: 9470 RVA: 0x000C66AC File Offset: 0x000C48AC
	private void Update()
	{
		if (this.skeleton.Bones.Count <= 0)
		{
			return;
		}
		foreach (OVRBone ovrbone in this.skeleton.Bones)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.cube);
			gameObject.transform.parent = ovrbone.Transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
		base.enabled = false;
	}

	// Token: 0x0400307E RID: 12414
	public GameObject cube;

	// Token: 0x0400307F RID: 12415
	public OVRSkeleton skeleton;
}
