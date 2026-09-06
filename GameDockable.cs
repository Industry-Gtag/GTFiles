using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006BD RID: 1725
public class GameDockable : MonoBehaviour
{
	// Token: 0x06002B12 RID: 11026 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x000E718C File Offset: 0x000E538C
	public GameEntityId BestDock()
	{
		int heldByHandIndex = this.gameEntity.heldByHandIndex;
		if (heldByHandIndex < 0)
		{
			return GameEntityId.Invalid;
		}
		SnapJointType snapJointType = (GamePlayer.IsLeftHand(heldByHandIndex) ? SnapJointType.HandL : SnapJointType.HandR);
		SnapJointType snapJointType2 = (GamePlayer.IsLeftHand(heldByHandIndex) ? SnapJointType.ForearmL : SnapJointType.ForearmR);
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		List<SuperInfectionSnapPoint> snapPoints = gamePlayer.snapPointManager.SnapPoints;
		float num = float.MaxValue;
		GameDock gameDock = null;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2)
			{
				GameEntity snappedEntity = snapPoints[i].GetSnappedEntity();
				if (!(snappedEntity == null))
				{
					GameDock component = snappedEntity.GetComponent<GameDock>();
					if (!(component == null) && component.CanDock(this))
					{
						Transform transform = component.dockMarker.transform;
						Vector3 zero = Vector3.zero;
						Quaternion identity = Quaternion.identity;
						float num2 = Vector3.Distance(transform.TransformPoint(identity * zero), base.transform.position);
						float num3 = this.dockableRadius + component.dockRadius;
						if (num2 < num && num2 < num3)
						{
							num = num2;
							gameDock = component;
						}
					}
				}
			}
		}
		for (int j = 0; j < 2; j++)
		{
			GameEntity grabbedGameEntity = gamePlayer.GetGrabbedGameEntity(j);
			if (!(grabbedGameEntity == null))
			{
				GameDock component2 = grabbedGameEntity.GetComponent<GameDock>();
				if (!(component2 == null) && component2.CanDock(this))
				{
					Transform transform2 = component2.dockMarker.transform;
					Vector3 zero2 = Vector3.zero;
					Quaternion identity2 = Quaternion.identity;
					float num2 = Vector3.Distance(transform2.TransformPoint(identity2 * zero2), base.transform.position);
					float num4 = this.dockableRadius + component2.dockRadius;
					if (num2 < num && num2 < num4)
					{
						num = num2;
						gameDock = component2;
					}
				}
			}
		}
		if (gameDock == null)
		{
			return GameEntityId.Invalid;
		}
		return gameDock.gameEntity.id;
	}

	// Token: 0x06002B14 RID: 11028 RVA: 0x000E738F File Offset: 0x000E558F
	public Transform GetDockablePoint()
	{
		if (!(this.dockablePoint == null))
		{
			return this.dockablePoint;
		}
		return base.transform;
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnUndock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
	}

	// Token: 0x040037E5 RID: 14309
	public GameEntity gameEntity;

	// Token: 0x040037E6 RID: 14310
	public float dockableRadius = 0.15f;

	// Token: 0x040037E7 RID: 14311
	public Transform dockablePoint;
}
