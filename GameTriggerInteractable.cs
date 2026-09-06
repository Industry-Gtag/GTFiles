using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000702 RID: 1794
[RequireComponent(typeof(GameEntity))]
public class GameTriggerInteractable : MonoBehaviour
{
	// Token: 0x06002D44 RID: 11588 RVA: 0x000F4820 File Offset: 0x000F2A20
	private void OnEnable()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		if (this.interactableWhileGrabbed)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartHolding));
			GameEntity gameEntity2 = this.gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.StopHolding));
		}
		if (this.interactableWhileSnapped)
		{
			GameEntity gameEntity3 = this.gameEntity;
			gameEntity3.OnSnapped = (Action)Delegate.Combine(gameEntity3.OnSnapped, new Action(this.StartHolding));
			GameEntity gameEntity4 = this.gameEntity;
			gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.StopHolding));
		}
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x000F48F3 File Offset: 0x000F2AF3
	public void StartHolding()
	{
		GameTriggerInteractable.LocalInteractableTriggers.AddIfNew(this);
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x000F4900 File Offset: 0x000F2B00
	public void StopHolding()
	{
		GameTriggerInteractable.LocalInteractableTriggers.RemoveIfContains(this);
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x000F4910 File Offset: 0x000F2B10
	public bool PointWithinInteractableArea(Vector3 point)
	{
		return (this.interactableCenter.position - point).magnitude < this.interactableRadius;
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x000F493E File Offset: 0x000F2B3E
	public void BeginTriggerInteraction(int _handIndex)
	{
		this.triggerInteractionActive = true;
		this.handIndex = _handIndex;
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x000F494E File Offset: 0x000F2B4E
	public void EndTriggerInteraction()
	{
		this.triggerInteractionActive = false;
		this.handIndex = -1;
	}

	// Token: 0x040039C2 RID: 14786
	public GameEntity gameEntity;

	// Token: 0x040039C3 RID: 14787
	public Transform interactableCenter;

	// Token: 0x040039C4 RID: 14788
	public float interactableRadius;

	// Token: 0x040039C5 RID: 14789
	public bool interactableWhileGrabbed;

	// Token: 0x040039C6 RID: 14790
	public bool interactableWhileSnapped;

	// Token: 0x040039C7 RID: 14791
	public bool interactablePermanently;

	// Token: 0x040039C8 RID: 14792
	public bool interactableOnOthers;

	// Token: 0x040039C9 RID: 14793
	public bool triggerInteractionActive;

	// Token: 0x040039CA RID: 14794
	public int handIndex = -1;

	// Token: 0x040039CB RID: 14795
	public static List<GameTriggerInteractable> LocalInteractableTriggers = new List<GameTriggerInteractable>();
}
