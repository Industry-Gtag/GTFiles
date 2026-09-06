using System;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000703 RID: 1795
public class SuperInfectionSnapPoint : MonoBehaviour
{
	// Token: 0x06002D4C RID: 11596 RVA: 0x000F497C File Offset: 0x000F2B7C
	public void Initialize()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent == null)
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  Expected a VRRig to be in parent hierarchy. Path=\"" + base.transform.GetPathQ() + "\"");
		}
		Transform[] array;
		string text;
		if (!GTHardCodedBones.TryGetBoneXforms(componentInParent, out array, out text))
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  Could not get bone transforms: " + text);
		}
		if (this.overrideParentTransform != null)
		{
			this.parentTransform = this.overrideParentTransform;
		}
		else if (!GTHardCodedBones.TryGetBoneXform(array, this.parentBone.Bone, out this.parentTransform))
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  " + string.Format("Could not find bone Transform `{0}`.", this.parentBone));
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (this.parentTransform != null)
		{
			base.transform.SetParent(this.parentTransform, false);
		}
		base.transform.localPosition = localPosition;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x06002D4D RID: 11597 RVA: 0x000F4A7F File Offset: 0x000F2C7F
	public void Clear()
	{
		this.Unsnapped();
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x000F4A88 File Offset: 0x000F2C88
	public void Snapped(GameEntity entity)
	{
		this.snappedEntity = entity;
		GameSnappable gameSnappable;
		if (this.snappedEntity.TryGetComponent<GameSnappable>(out gameSnappable))
		{
			gameSnappable.snappedToJoint = this;
			return;
		}
		Debug.LogError(string.Format("Snapped: entity {0} has no GameSnappable!?", this.snappedEntity));
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x000F4AC8 File Offset: 0x000F2CC8
	public void Unsnapped()
	{
		GameSnappable gameSnappable;
		if (this.snappedEntity && this.snappedEntity.TryGetComponent<GameSnappable>(out gameSnappable))
		{
			gameSnappable.snappedToJoint = null;
		}
		else
		{
			Debug.LogError(string.Format("Unsnapped: entity {0} has no GameSnappable!?", this.snappedEntity));
		}
		this.snappedEntity = null;
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x000F4B16 File Offset: 0x000F2D16
	public bool HasSnapped()
	{
		return this.snappedEntity != null;
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x000F4B24 File Offset: 0x000F2D24
	public GameEntity GetSnappedEntity()
	{
		return this.snappedEntity;
	}

	// Token: 0x040039CC RID: 14796
	private const string preLog = "[SuperInfectionSnapPoint]  ";

	// Token: 0x040039CD RID: 14797
	private const string preErr = "[SuperInfectionSnapPoint]  ERROR!!!  ";

	// Token: 0x040039CE RID: 14798
	public GamePlayer playerForPoint;

	// Token: 0x040039CF RID: 14799
	public SnapJointType jointType;

	// Token: 0x040039D0 RID: 14800
	public GTHardCodedBones.SturdyEBone parentBone;

	// Token: 0x040039D1 RID: 14801
	public Transform overrideParentTransform;

	// Token: 0x040039D2 RID: 14802
	private Transform parentTransform;

	// Token: 0x040039D3 RID: 14803
	public bool canSnapOverride;

	// Token: 0x040039D4 RID: 14804
	public float snapPointRadius;

	// Token: 0x040039D5 RID: 14805
	private GameEntity snappedEntity;
}
