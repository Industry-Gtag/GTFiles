using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A77 RID: 2679
public class CustomMapsGrabbablesController : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06004512 RID: 17682 RVA: 0x00170AC8 File Offset: 0x0016ECC8
	private void Awake()
	{
		this.isGrabbed = false;
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x06004513 RID: 17683 RVA: 0x00170B2C File Offset: 0x0016ED2C
	private void OnDestroy()
	{
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x06004514 RID: 17684 RVA: 0x00170B88 File Offset: 0x0016ED88
	public void OnEntityInit()
	{
		GTDev.Log<string>("CustomMapsGrabbablesController::OnEntityInit", null);
		if (MapSpawnManager.instance == null)
		{
			return;
		}
		base.transform.parent = MapSpawnManager.instance.transform;
		byte b;
		GrabbableEntity.UnpackCreateData(this.entity.createData, out b, out this.luaAgentID);
		MapEntity mapEntity;
		if (!MapSpawnManager.instance.SpawnEntity((int)b, out mapEntity))
		{
			GTDev.LogError<string>("CustomMapsGrabbablesController::OnEntityInit could not spawn grabbable", null);
			Object.Destroy(base.gameObject);
			return;
		}
		GrabbableEntity grabbableEntity = (GrabbableEntity)mapEntity;
		if (grabbableEntity == null)
		{
			return;
		}
		grabbableEntity.gameObject.SetActive(true);
		grabbableEntity.transform.parent = this.entity.transform;
		grabbableEntity.transform.localPosition = Vector3.zero;
		grabbableEntity.transform.localRotation = Quaternion.identity;
		this.returnParent = this.entity.transform.parent;
		this.entity.audioSource = grabbableEntity.audioSource;
		this.entity.catchSound = grabbableEntity.catchSound;
		this.entity.catchSoundVolume = grabbableEntity.catchSoundVolume;
		this.entity.throwSound = grabbableEntity.throwSound;
		this.entity.throwSoundVolume = grabbableEntity.throwSoundVolume;
		Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer("Prop");
		}
	}

	// Token: 0x06004515 RID: 17685 RVA: 0x00170CF5 File Offset: 0x0016EEF5
	public int GetGrabbingActor()
	{
		if (!this.isGrabbed)
		{
			return -1;
		}
		return this.entity.heldByActorNumber;
	}

	// Token: 0x06004516 RID: 17686 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06004517 RID: 17687 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x06004518 RID: 17688 RVA: 0x00170D0C File Offset: 0x0016EF0C
	private void OnGrabbed()
	{
		this.isGrabbed = true;
	}

	// Token: 0x06004519 RID: 17689 RVA: 0x00170D15 File Offset: 0x0016EF15
	private void OnReleased()
	{
		this.isGrabbed = false;
		if (this.returnParent.IsNotNull())
		{
			this.entity.transform.parent = this.returnParent;
		}
	}

	// Token: 0x040056F8 RID: 22264
	public GameEntity entity;

	// Token: 0x040056F9 RID: 22265
	public short luaAgentID;

	// Token: 0x040056FA RID: 22266
	private bool isGrabbed;

	// Token: 0x040056FB RID: 22267
	private Transform returnParent;
}
