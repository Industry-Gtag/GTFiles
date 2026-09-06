using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000350 RID: 848
[Serializable]
public struct GorillaPosRotConstraint
{
	// Token: 0x0400199C RID: 6556
	[Tooltip("Transform that should be moved, rotated, and scaled to match the `source` Transform in world space.")]
	public Transform follower;

	// Token: 0x0400199D RID: 6557
	[Tooltip("Bone that `follower` should match. Set to `None` to assign a specific Transform within the same prefab.")]
	public GTHardCodedBones.SturdyEBone sourceGorillaBone;

	// Token: 0x0400199E RID: 6558
	[Tooltip("Transform that `follower` should match. This is overridden at runtime if `sourceGorillaBone` is not `None`. If set in inspector, then it should be only set to a child of the the prefab this component belongs to.")]
	public Transform source;

	// Token: 0x0400199F RID: 6559
	public string sourceRelativePath;

	// Token: 0x040019A0 RID: 6560
	[Tooltip("Offset to be applied to the follower's position.")]
	public Vector3 positionOffset;

	// Token: 0x040019A1 RID: 6561
	[Tooltip("Offset to be applied to the follower's rotation.")]
	public Quaternion rotationOffset;
}
