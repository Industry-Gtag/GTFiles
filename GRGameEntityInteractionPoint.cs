using System;
using UnityEngine;

// Token: 0x020007B8 RID: 1976
public class GRGameEntityInteractionPoint : MonoBehaviour
{
	// Token: 0x06003286 RID: 12934 RVA: 0x00114E8B File Offset: 0x0011308B
	public void Start()
	{
		base.transform.parent = this.targetParent;
	}

	// Token: 0x06003287 RID: 12935 RVA: 0x00114EA0 File Offset: 0x001130A0
	public void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x00114EFC File Offset: 0x001130FC
	public void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x00114F57 File Offset: 0x00113157
	public void OnGrabbed()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.TickWhileHeld));
		Action onGrabStart = this.OnGrabStart;
		if (onGrabStart == null)
		{
			return;
		}
		onGrabStart();
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x00114F90 File Offset: 0x00113190
	public void OnReleased()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.TickWhileHeld));
		this.gameEntity.transform.parent = this.targetParent;
		this.gameEntity.transform.localRotation = Quaternion.identity;
		this.gameEntity.transform.localPosition = Vector3.zero;
		this.OnGrabEnd();
	}

	// Token: 0x0600328B RID: 12939 RVA: 0x00115010 File Offset: 0x00113210
	public void TickWhileHeld()
	{
		if (this.targetParent != null)
		{
			Vector3 position = this.targetParent.transform.position;
			Vector3 position2 = base.transform.position;
			if (Vector3.Magnitude(position - position2) > this.autoReleaseDistance)
			{
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
				if (gamePlayer != null)
				{
					gamePlayer.ClearGrabbedIfHeld(this.gameEntity.id, this.gameEntity.manager);
				}
				if (gamePlayer != null && GamePlayerLocal.instance.gamePlayer == gamePlayer)
				{
					GamePlayerLocal.instance.ClearGrabbedIfHeld(this.gameEntity.id, this.gameEntity.manager);
				}
				this.OnReleased();
				return;
			}
		}
		Action onGrabContinue = this.OnGrabContinue;
		if (onGrabContinue == null)
		{
			return;
		}
		onGrabContinue();
	}

	// Token: 0x04004180 RID: 16768
	public GameEntity gameEntity;

	// Token: 0x04004181 RID: 16769
	public float autoReleaseDistance = 0.1f;

	// Token: 0x04004182 RID: 16770
	public Action OnGrabStart;

	// Token: 0x04004183 RID: 16771
	public Action OnGrabContinue;

	// Token: 0x04004184 RID: 16772
	public Action OnGrabEnd;

	// Token: 0x04004185 RID: 16773
	public Transform targetParent;
}
