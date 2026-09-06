using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000674 RID: 1652
[Serializable]
internal class HandTransformFollowOffset
{
	// Token: 0x0600292C RID: 10540 RVA: 0x000DF6A4 File Offset: 0x000DD8A4
	internal void UpdatePositionRotation()
	{
		if (this.followTransform == null || this.targetTransforms == null)
		{
			return;
		}
		this.position = this.followTransform.position + this.followTransform.rotation * this.positionOffset * GTPlayer.Instance.scale;
		this.rotation = this.followTransform.rotation * this.rotationOffset;
		foreach (Transform transform in this.targetTransforms)
		{
			transform.position = this.position;
			transform.rotation = this.rotation;
		}
	}

	// Token: 0x040035B3 RID: 13747
	internal Transform followTransform;

	// Token: 0x040035B4 RID: 13748
	[SerializeField]
	private Transform[] targetTransforms;

	// Token: 0x040035B5 RID: 13749
	[SerializeField]
	internal Vector3 positionOffset;

	// Token: 0x040035B6 RID: 13750
	[SerializeField]
	internal Quaternion rotationOffset;

	// Token: 0x040035B7 RID: 13751
	private Vector3 position;

	// Token: 0x040035B8 RID: 13752
	private Quaternion rotation;
}
