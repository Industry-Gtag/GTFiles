using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000704 RID: 1796
public class SuperInfectionSnapPointManager : MonoBehaviour
{
	// Token: 0x06002D53 RID: 11603 RVA: 0x000F4B2C File Offset: 0x000F2D2C
	public void Awake()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		ISpawnable[] componentsInChildren = base.GetComponentsInChildren<ISpawnable>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].OnSpawn(componentInParent);
		}
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x000F4B60 File Offset: 0x000F2D60
	public void Start()
	{
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.SnapPoints)
		{
			superInfectionSnapPoint.Initialize();
			this.snapPointDict[superInfectionSnapPoint.jointType] = superInfectionSnapPoint;
		}
	}

	// Token: 0x06002D55 RID: 11605 RVA: 0x000F4BC4 File Offset: 0x000F2DC4
	public void Clear()
	{
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.SnapPoints)
		{
			superInfectionSnapPoint.Clear();
		}
		this.snapPointDict.Clear();
	}

	// Token: 0x06002D56 RID: 11606 RVA: 0x000F4C20 File Offset: 0x000F2E20
	public SuperInfectionSnapPoint FindSnapPoint(SnapJointType jointType)
	{
		if (jointType == SnapJointType.None)
		{
			return null;
		}
		if (this.snapPointDict.ContainsKey(jointType))
		{
			return this.snapPointDict[jointType];
		}
		return null;
	}

	// Token: 0x06002D57 RID: 11607 RVA: 0x000F4C43 File Offset: 0x000F2E43
	public static SuperInfectionSnapPoint FindSnapPoint(GamePlayer player, SnapJointType jointType)
	{
		if (player == null)
		{
			return null;
		}
		return player.snapPointManager.FindSnapPoint(jointType);
	}

	// Token: 0x06002D58 RID: 11608 RVA: 0x000F4C5C File Offset: 0x000F2E5C
	public void DropAllSnappedAuthority()
	{
		for (int i = 0; i < this.SnapPoints.Count; i++)
		{
			GameEntity snappedEntity = this.SnapPoints[i].GetSnappedEntity();
			if (!(snappedEntity == null))
			{
				Vector3 position = snappedEntity.transform.position;
				snappedEntity.manager.RequestGrabEntity(snappedEntity.id, false, Vector3.zero, Quaternion.identity);
				snappedEntity.manager.RequestThrowEntity(snappedEntity.id, false, position, Vector3.zero, Vector3.zero);
			}
		}
	}

	// Token: 0x040039D6 RID: 14806
	public List<SuperInfectionSnapPoint> SnapPoints;

	// Token: 0x040039D7 RID: 14807
	private Dictionary<SnapJointType, SuperInfectionSnapPoint> snapPointDict = new Dictionary<SnapJointType, SuperInfectionSnapPoint>();
}
