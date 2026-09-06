using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020006FF RID: 1791
public class GameSnappable : MonoBehaviour
{
	// Token: 0x06002D36 RID: 11574 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x000F42D0 File Offset: 0x000F24D0
	public void GetSnapOffset(SnapJointType jointType, out Vector3 positionOffset, out Quaternion rotationOffset)
	{
		foreach (GameSnappable.SnapJointOffset snapJointOffset in this.snapOffsets)
		{
			if ((snapJointOffset.jointType & jointType) != SnapJointType.None)
			{
				positionOffset = snapJointOffset.positionOffset;
				rotationOffset = Quaternion.Euler(snapJointOffset.rotationOffset);
				return;
			}
		}
		positionOffset = Vector3.zero;
		rotationOffset = Quaternion.identity;
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x000F435C File Offset: 0x000F255C
	public SuperInfectionSnapPoint BestSnapPoint()
	{
		int heldByHandIndex = this.gameEntity.heldByHandIndex;
		if (heldByHandIndex < 0)
		{
			return null;
		}
		bool flag = GamePlayer.IsLeftHand(heldByHandIndex);
		SnapJointType snapJointType = (flag ? SnapJointType.HandL : SnapJointType.HandR);
		SnapJointType snapJointType2 = (flag ? SnapJointType.ForearmL : SnapJointType.ForearmR);
		List<SuperInfectionSnapPoint> snapPoints = GamePlayerLocal.instance.gamePlayer.snapPointManager.SnapPoints;
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2 && (snapPoints[i].jointType & this.snapLocationTypes) != SnapJointType.None && !snapPoints[i].HasSnapped())
			{
				Vector3 vector;
				Quaternion quaternion;
				this.GetSnapOffset(snapPoints[i].jointType, out vector, out quaternion);
				float num3 = Vector3.Distance(snapPoints[i].transform.TransformPoint(quaternion * vector), base.transform.position);
				float num4 = this.snapRadius + snapPoints[i].snapPointRadius;
				if (num3 < num && num3 < num4)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		if (num2 >= 0)
		{
			return snapPoints[num2];
		}
		if ((this.snapLocationTypes & SnapJointType.Holster) != SnapJointType.None)
		{
			GameEntityManager currGameEntityManager = GamePlayerLocal.instance.currGameEntityManager;
			IEnumerable<SuperInfectionSnapPoint> points = ((currGameEntityManager != null) ? currGameEntityManager.superInfectionManager : null).GetPoints(SnapJointType.Holster);
			SuperInfectionSnapPoint superInfectionSnapPoint = null;
			float num5 = this.snapRadius;
			foreach (SuperInfectionSnapPoint superInfectionSnapPoint2 in points)
			{
				if (!superInfectionSnapPoint2.HasSnapped())
				{
					Vector3 vector2;
					Quaternion quaternion2;
					this.GetSnapOffset(superInfectionSnapPoint2.jointType, out vector2, out quaternion2);
					float num6 = Vector3.Distance(superInfectionSnapPoint2.transform.TransformPoint(quaternion2 * vector2), base.transform.position);
					if (num6 < num5)
					{
						superInfectionSnapPoint = superInfectionSnapPoint2;
						num5 = num6;
					}
				}
			}
			if (superInfectionSnapPoint != null)
			{
				return superInfectionSnapPoint;
			}
		}
		return null;
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x000F456C File Offset: 0x000F276C
	public GameEntityId BestSnapPointDock()
	{
		int heldByHandIndex = this.gameEntity.heldByHandIndex;
		if (heldByHandIndex < 0)
		{
			return GameEntityId.Invalid;
		}
		SnapJointType snapJointType = (GamePlayer.IsLeftHand(heldByHandIndex) ? SnapJointType.HandL : SnapJointType.HandR);
		SnapJointType snapJointType2 = (GamePlayer.IsLeftHand(heldByHandIndex) ? SnapJointType.ForearmL : SnapJointType.ForearmR);
		List<SuperInfectionSnapPoint> snapPoints = GamePlayerLocal.instance.gamePlayer.snapPointManager.SnapPoints;
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2 && (snapPoints[i].jointType & this.snapLocationTypes) != SnapJointType.None && snapPoints[i].HasSnapped())
			{
				Vector3 vector;
				Quaternion quaternion;
				this.GetSnapOffset(snapPoints[i].jointType, out vector, out quaternion);
				float num3 = Vector3.Distance(snapPoints[i].transform.TransformPoint(quaternion * vector), base.transform.position);
				float num4 = this.snapRadius + snapPoints[i].snapPointRadius;
				if (num3 < num && num3 < num4)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		if (num2 < 0)
		{
			return GameEntityId.Invalid;
		}
		return snapPoints[num2].GetSnappedEntity().id;
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x000F46C4 File Offset: 0x000F28C4
	public bool CanGrabWithHand(bool leftHand)
	{
		if (this.snappedToJoint == null)
		{
			return true;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return (leftHand && jointType != SnapJointType.HandL && jointType != SnapJointType.ForearmL) || (!leftHand && jointType != SnapJointType.HandR && jointType != SnapJointType.ForearmR);
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x000F4712 File Offset: 0x000F2912
	public void OnSnap()
	{
		this.snapSound.Play(null);
		this.snapHaptic.PlayIfSnappedLocal(this.gameEntity);
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x000F4734 File Offset: 0x000F2934
	public bool IsSnappedToLeftArm()
	{
		if (this.snappedToJoint == null)
		{
			return false;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return jointType == SnapJointType.HandL || jointType == SnapJointType.ForearmL;
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x000F476C File Offset: 0x000F296C
	public bool IsSnappedToRightArm()
	{
		if (this.snappedToJoint == null)
		{
			return false;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return jointType == SnapJointType.HandR || jointType == SnapJointType.ForearmR;
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x000F47A3 File Offset: 0x000F29A3
	public void OnUnsnap()
	{
		this.unsnapSound.Play(null);
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x000F47B1 File Offset: 0x000F29B1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryGetJointToSnapIndex(SnapJointType jointType, out int out_slot)
	{
		out_slot = GameSnappable.GetJointToSnapIndex(jointType);
		return out_slot != -1;
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x000F47C4 File Offset: 0x000F29C4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetJointToSnapIndex(SnapJointType jointType)
	{
		int num;
		if (jointType != SnapJointType.HandL)
		{
			if (jointType != SnapJointType.HandR)
			{
				num = -1;
			}
			else
			{
				num = 3;
			}
		}
		else
		{
			num = 2;
		}
		return num;
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x000F47E8 File Offset: 0x000F29E8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SnapJointType GetSnapIndexToJoint(int snapIndex)
	{
		SnapJointType snapJointType;
		if (snapIndex != 2)
		{
			if (snapIndex != 3)
			{
				snapJointType = SnapJointType.None;
			}
			else
			{
				snapJointType = SnapJointType.HandR;
			}
		}
		else
		{
			snapJointType = SnapJointType.HandL;
		}
		return snapJointType;
	}

	// Token: 0x040039B6 RID: 14774
	public GameEntity gameEntity;

	// Token: 0x040039B7 RID: 14775
	public float snapRadius = 0.15f;

	// Token: 0x040039B8 RID: 14776
	public SuperInfectionSnapPoint snappedToJoint;

	// Token: 0x040039B9 RID: 14777
	public AbilitySound snapSound;

	// Token: 0x040039BA RID: 14778
	public AbilitySound unsnapSound;

	// Token: 0x040039BB RID: 14779
	public AbilityHaptic snapHaptic;

	// Token: 0x040039BC RID: 14780
	public SnapJointType snapLocationTypes;

	// Token: 0x040039BD RID: 14781
	public List<GameSnappable.SnapJointOffset> snapOffsets;

	// Token: 0x02000700 RID: 1792
	[Serializable]
	public struct SnapJointOffset
	{
		// Token: 0x040039BE RID: 14782
		public SnapJointType jointType;

		// Token: 0x040039BF RID: 14783
		public Vector3 positionOffset;

		// Token: 0x040039C0 RID: 14784
		public Vector3 rotationOffset;
	}
}
