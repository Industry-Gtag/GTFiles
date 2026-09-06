using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class CrittersBag : CrittersActor
{
	// Token: 0x060001A9 RID: 425 RVA: 0x0000A437 File Offset: 0x00008637
	protected override void Awake()
	{
		base.Awake();
		this.overlapColliders = new Collider[20];
		this.attachedColliders = new Dictionary<int, GameObject>();
		this.isAttachedToPlayer = false;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000A45E File Offset: 0x0000865E
	public override void OnHover(bool isLeft)
	{
		if (this.isAttachedToPlayer)
		{
			GorillaTagger.Instance.StartVibration(isLeft, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			return;
		}
		base.OnHover(isLeft);
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000A49C File Offset: 0x0000869C
	protected override void CleanupActor()
	{
		base.CleanupActor();
		for (int i = this.attachedColliders.Count - 1; i >= 0; i--)
		{
			this.attachedColliders[this.attachedColliders.ElementAt(i).Key].gameObject.Destroy();
		}
		this.attachedColliders.Clear();
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000A4FC File Offset: 0x000086FC
	protected override void GlobalGrabbedBy(CrittersActor grabbedBy)
	{
		base.GlobalGrabbedBy(grabbedBy);
		bool flag = this.attachedToLocalPlayer;
		if (grabbedBy.IsNotNull())
		{
			CrittersAttachPoint crittersAttachPoint = grabbedBy as CrittersAttachPoint;
			if (crittersAttachPoint != null)
			{
				this.isAttachedToPlayer = true;
				this.attachedToLocalPlayer = crittersAttachPoint.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber;
				goto IL_004F;
			}
		}
		this.isAttachedToPlayer = false;
		this.attachedToLocalPlayer = false;
		IL_004F:
		if (this.attachedToLocalPlayer != flag)
		{
			bool flag2 = this.attachedToLocalPlayer || flag;
			this.audioSrc.transform.localPosition = Vector3.zero;
			this.audioSrc.GTPlayOneShot(this.attachedToLocalPlayer ? this.equipSound : this.unequipSound, flag2 ? 1f : 0.5f);
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000A5AF File Offset: 0x000087AF
	public override void GrabbedBy(CrittersActor grabbedBy, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbedBy, positionOverride, localRotation, localOffset, disableGrabbing);
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000A5C0 File Offset: 0x000087C0
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulse = default(Vector3), Vector3 impulseRotation = default(Vector3))
	{
		if (this.parentActorId >= 0)
		{
			base.AttemptRemoveStoredObjectCollider(this.parentActorId, true);
		}
		int num = Physics.OverlapBoxNonAlloc(this.dropCube.transform.position, this.dropCube.size / 2f, this.overlapColliders, this.dropCube.transform.rotation, CrittersManager.instance.objectLayers, QueryTriggerInteraction.Collide);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				Rigidbody attachedRigidbody = this.overlapColliders[i].attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					CrittersAttachPoint component = attachedRigidbody.GetComponent<CrittersAttachPoint>();
					if (!(component == null) && component.anchorLocation == this.anchorLocation && !(component.GetComponentInChildren<CrittersBag>() != null))
					{
						CrittersActor crittersActor;
						if (this.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber && CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
						{
							CrittersGrabber crittersGrabber = crittersActor as CrittersGrabber;
							if (crittersGrabber != null)
							{
								GorillaTagger.Instance.StartVibration(crittersGrabber.isLeft, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
							}
						}
						this.GrabbedBy(component, true, default(Quaternion), default(Vector3), false);
						return;
					}
				}
			}
		}
		base.Released(keepWorldPosition, rotation, position, impulse, impulseRotation);
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000A730 File Offset: 0x00008930
	public void AddStoredObjectCollider(CrittersActor actor)
	{
		if (this.attachedColliders.ContainsKey(actor.actorId))
		{
			if (this.attachedColliders[actor.actorId].IsNull())
			{
				this.attachedColliders[actor.actorId] = CrittersManager.DuplicateCapsuleCollider(base.transform, actor.storeCollider).gameObject;
			}
		}
		else
		{
			this.attachedColliders.Add(actor.actorId, CrittersManager.DuplicateCapsuleCollider(base.transform, actor.storeCollider).gameObject);
		}
		this.audioSrc.transform.position = actor.transform.position;
		this.audioSrc.GTPlayOneShot(this.attachSound, 1f);
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000A7EC File Offset: 0x000089EC
	public void RemoveStoredObjectCollider(CrittersActor actor, bool playSound = true)
	{
		GameObject gameObject;
		if (this.attachedColliders.TryGetValue(actor.actorId, out gameObject))
		{
			Object.Destroy(gameObject);
			this.attachedColliders.Remove(actor.actorId);
		}
		if (playSound)
		{
			this.audioSrc.transform.position = actor.transform.position;
			this.audioSrc.GTPlayOneShot(this.detachSound, 1f);
		}
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000A85A File Offset: 0x00008A5A
	public bool IsActorValidStore(CrittersActor actor)
	{
		return this.blockAttachTypes == null || !this.blockAttachTypes.Contains(actor.crittersActorType);
	}

	// Token: 0x040001CC RID: 460
	public AudioSource audioSrc;

	// Token: 0x040001CD RID: 461
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x040001CE RID: 462
	public Collider attachableCollider;

	// Token: 0x040001CF RID: 463
	public BoxCollider dropCube;

	// Token: 0x040001D0 RID: 464
	private Collider[] overlapColliders;

	// Token: 0x040001D1 RID: 465
	public List<Collider> attachDisableColliders;

	// Token: 0x040001D2 RID: 466
	public Dictionary<int, GameObject> attachedColliders;

	// Token: 0x040001D3 RID: 467
	[Header("Child object attachment sounds")]
	public AudioClip attachSound;

	// Token: 0x040001D4 RID: 468
	public AudioClip detachSound;

	// Token: 0x040001D5 RID: 469
	[Header("Monke equip sounds")]
	public AudioClip equipSound;

	// Token: 0x040001D6 RID: 470
	public AudioClip unequipSound;

	// Token: 0x040001D7 RID: 471
	[Header("Attachment Blocking")]
	public List<CrittersActor.CrittersActorType> blockAttachTypes;

	// Token: 0x040001D8 RID: 472
	private bool isAttachedToPlayer;

	// Token: 0x040001D9 RID: 473
	private bool attachedToLocalPlayer;
}
