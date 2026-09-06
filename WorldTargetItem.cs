using System;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020004D4 RID: 1236
public class WorldTargetItem
{
	// Token: 0x06001E0B RID: 7691 RVA: 0x000A1EA7 File Offset: 0x000A00A7
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x000A1EC0 File Offset: 0x000A00C0
	[CanBeNull]
	public static WorldTargetItem GenerateTargetFromPlayerAndID(NetPlayer owner, int itemIdx)
	{
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(owner);
		if (vrrig == null)
		{
			Debug.LogError("Tried to setup a sharable object but the target rig is null...");
			return null;
		}
		Transform component = vrrig.myBodyDockPositions.TransferrableItem(itemIdx).gameObject.GetComponent<Transform>();
		return new WorldTargetItem(owner, itemIdx, component);
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x000A1F08 File Offset: 0x000A0108
	public static WorldTargetItem GenerateTargetFromWorldSharableItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		return new WorldTargetItem(owner, itemIdx, transform);
	}

	// Token: 0x06001E0E RID: 7694 RVA: 0x000A1F12 File Offset: 0x000A0112
	private WorldTargetItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		this.owner = owner;
		this.itemIdx = itemIdx;
		this.targetObject = transform;
		this.transferrableObject = transform.GetComponent<TransferrableObject>();
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x000A1F3B File Offset: 0x000A013B
	public override string ToString()
	{
		return string.Format("Id: {0} ({1})", this.itemIdx, this.owner);
	}

	// Token: 0x0400286F RID: 10351
	public readonly NetPlayer owner;

	// Token: 0x04002870 RID: 10352
	public readonly int itemIdx;

	// Token: 0x04002871 RID: 10353
	public readonly Transform targetObject;

	// Token: 0x04002872 RID: 10354
	public readonly TransferrableObject transferrableObject;
}
