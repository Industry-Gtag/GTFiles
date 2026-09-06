using System;
using UnityEngine;

// Token: 0x020006F1 RID: 1777
public class GameMirrorWhenEquipped : MonoBehaviour
{
	// Token: 0x06002CB9 RID: 11449 RVA: 0x000F173C File Offset: 0x000EF93C
	private void Awake()
	{
		if (this.m_gameEntity == null)
		{
			this.m_gameEntity = base.GetComponent<GameEntity>();
		}
		if (this.m_xformsToMirror == null)
		{
			this.m_xformsToMirror = Array.Empty<Transform>();
		}
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x000F176C File Offset: 0x000EF96C
	protected void OnEnable()
	{
		GameEntity gameEntity = this.m_gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity2 = this.m_gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity3 = this.m_gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity4 = this.m_gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleGameEntityOnEquipChanged));
	}

	// Token: 0x06002CBB RID: 11451 RVA: 0x000F1818 File Offset: 0x000EFA18
	protected void OnDisable()
	{
		GameEntity gameEntity = this.m_gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity2 = this.m_gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity3 = this.m_gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this._HandleGameEntityOnEquipChanged));
		GameEntity gameEntity4 = this.m_gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this._HandleGameEntityOnEquipChanged));
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x000F18C4 File Offset: 0x000EFAC4
	private void _HandleGameEntityOnEquipChanged()
	{
		if (this.m_shouldOnlyMirrorWhenSnapped && this.m_gameEntity.snappedJoint == SnapJointType.None)
		{
			return;
		}
		Vector3 vector = ((this.m_gameEntity.EquippedHandedness == this.m_handednessToMirror) ? new Vector3(-1f, 1f, 1f) : Vector3.one);
		for (int i = 0; i < this.m_xformsToMirror.Length; i++)
		{
			this.m_xformsToMirror[i].localScale = vector;
		}
	}

	// Token: 0x0400393F RID: 14655
	[SerializeField]
	private GameEntity m_gameEntity;

	// Token: 0x04003940 RID: 14656
	[SerializeField]
	private Transform[] m_xformsToMirror;

	// Token: 0x04003941 RID: 14657
	[SerializeField]
	private bool m_shouldOnlyMirrorWhenSnapped = true;

	// Token: 0x04003942 RID: 14658
	[Tooltip("Set the X axis scale to -1 if the gadget is attached (held or snapped) to the selected side.")]
	[SerializeField]
	private EHandedness m_handednessToMirror = EHandedness.Right;
}
