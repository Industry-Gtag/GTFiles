using System;
using System.Collections;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200004C RID: 76
[DefaultExecutionOrder(9999)]
public class CrittersActorGrabber : MonoBehaviour
{
	// Token: 0x0600016F RID: 367 RVA: 0x00009048 File Offset: 0x00007248
	private void Awake()
	{
		if (this.grabber == null)
		{
			this.grabber = base.GetComponent<CrittersGrabber>();
		}
		this.vibrationStartDistance *= this.vibrationStartDistance;
		this.vibrationEndDistance *= this.vibrationEndDistance;
		this.rb = base.GetComponent<Rigidbody>();
		CrittersGrabberSharedData.AddActorGrabber(this);
		this.actorsStillPresent = new List<CrittersActor>();
	}

	// Token: 0x06000170 RID: 368 RVA: 0x000090B4 File Offset: 0x000072B4
	private void LateUpdate()
	{
		if (CrittersManager.instance == null || !CrittersManager.instance.LocalInZone)
		{
			return;
		}
		if (this.isLeft)
		{
			this.NewJointMethod();
		}
		CrittersRigActorSetup crittersRigActorSetup;
		if ((this.grabber == null || !this.grabber.gameObject.activeSelf || this.grabber.rigPlayerId != PhotonNetwork.LocalPlayer.ActorNumber) && CrittersManager.instance.rigSetupByRig.TryGetValue(GorillaTagger.Instance.offlineVRRig, out crittersRigActorSetup))
		{
			int num;
			if (this.isLeft)
			{
				num = 1;
			}
			else
			{
				num = 3;
			}
			this.grabber = crittersRigActorSetup.rigActors[num].location.GetComponentInChildren<CrittersGrabber>();
			if (this.grabber != null)
			{
				this.grabber.isLeft = this.isLeft;
			}
		}
		if (this.grabber != null)
		{
			for (int i = 0; i < this.grabber.grabbedActors.Count; i++)
			{
				if (this.grabber.grabbedActors[i].localCanStore)
				{
					this.grabber.grabbedActors[i].CheckStorable();
				}
			}
		}
		if (this.transformToFollow != null)
		{
			base.transform.position = this.transformToFollow.position;
			base.transform.rotation = this.transformToFollow.rotation;
		}
		if (this.grabber == null)
		{
			return;
		}
		this.VerifyExistingGrab();
		this.validGrabTarget = this.FindGrabTargets();
		bool flag;
		if (this.isLeft)
		{
			flag = ControllerInputPoller.instance.leftGrab;
		}
		else
		{
			flag = ControllerInputPoller.instance.rightGrab;
		}
		bool flag2 = (this.isLeft ? (EquipmentInteractor.instance.leftHandHeldEquipment != null) : (EquipmentInteractor.instance.rightHandHeldEquipment != null));
		if (flag2)
		{
			flag = false;
		}
		if (!flag2)
		{
			if (this.validGrabTarget.IsNotNull())
			{
				if (this.validGrabTarget != this.lastHover)
				{
					this.lastHover = this.validGrabTarget;
					this.DoHover();
				}
			}
			else
			{
				this.lastHover = null;
			}
		}
		if (!this.isGrabbing && flag)
		{
			this.isGrabbing = true;
			this.remainingGrabDuration = this.grabDuration;
		}
		else if (this.isGrabbing)
		{
			if (!flag)
			{
				this.isGrabbing = false;
				this.DoRelease();
			}
			else if (this.queuedGrab != null)
			{
				this.CheckApplyQueuedGrab();
			}
		}
		if (this.isGrabbing && this.remainingGrabDuration > 0f)
		{
			this.remainingGrabDuration -= Time.deltaTime;
			this.DoGrab();
		}
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000934C File Offset: 0x0000754C
	private CrittersActor FindGrabTargets()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius, this.colliders, CrittersManager.instance.objectLayers | CrittersManager.instance.containerLayer);
		float num2 = 10000f;
		Collider collider = null;
		if (num <= 0)
		{
			return null;
		}
		for (int i = 0; i < num; i++)
		{
			Rigidbody attachedRigidbody = this.colliders[i].attachedRigidbody;
			if (!(attachedRigidbody == null))
			{
				CrittersActor component = attachedRigidbody.GetComponent<CrittersActor>();
				CrittersActor crittersActor;
				if (!(component == null) && (!(component is CrittersBag) || !CrittersManager.instance.actorById.TryGetValue(component.parentActorId, out crittersActor) || !(crittersActor is CrittersAttachPoint) || (crittersActor as CrittersAttachPoint).rigPlayerId != PhotonNetwork.LocalPlayer.ActorNumber || (crittersActor as CrittersAttachPoint).anchorLocation != CrittersAttachPoint.AnchoredLocationTypes.Arm || (crittersActor as CrittersAttachPoint).isLeft != this.isLeft) && component.usesRB && component.CanBeGrabbed(this.grabber))
				{
					float sqrMagnitude = (this.colliders[i].attachedRigidbody.position - base.transform.position).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
						collider = this.colliders[i];
					}
				}
			}
		}
		if (collider == null)
		{
			return null;
		}
		return collider.attachedRigidbody.GetComponent<CrittersActor>();
	}

	// Token: 0x06000172 RID: 370 RVA: 0x000094C5 File Offset: 0x000076C5
	private void DoHover()
	{
		this.validGrabTarget.OnHover(this.isLeft);
	}

	// Token: 0x06000173 RID: 371 RVA: 0x000094D8 File Offset: 0x000076D8
	private void DoGrab()
	{
		if (this.validGrabTarget.IsNull())
		{
			return;
		}
		this.grabber.grabbing = true;
		if (this.isLeft)
		{
			EquipmentInteractor.instance.disableLeftGrab = true;
		}
		else
		{
			EquipmentInteractor.instance.disableRightGrab = true;
		}
		this.isHandGrabbingDisabled = true;
		this.remainingGrabDuration = 0f;
		Vector3 vector = this.grabber.transform.InverseTransformPoint(this.validGrabTarget.transform.position);
		Quaternion quaternion = this.grabber.transform.InverseTransformRotation(this.validGrabTarget.transform.rotation);
		if (this.validGrabTarget.IsCurrentlyAttachedToBag())
		{
			this.queuedGrab = this.validGrabTarget;
			this.queuedRelativeGrabOffset = vector;
			this.queuedRelativeGrabRotation = quaternion;
			return;
		}
		if (this.validGrabTarget.AllowGrabbingActor(this.grabber))
		{
			this.ApplyGrab(this.validGrabTarget, quaternion, vector);
		}
	}

	// Token: 0x06000174 RID: 372 RVA: 0x000095C0 File Offset: 0x000077C0
	private void ApplyGrab(CrittersActor grabTarget, Quaternion localRotation, Vector3 localOffset)
	{
		if (grabTarget.AttemptSetEquipmentStorable())
		{
			this.RemoveGrabberPhysicsTrigger();
			this.AddGrabberPhysicsTrigger(grabTarget);
		}
		grabTarget.GrabbedBy(this.grabber, true, localRotation, localOffset, false);
		this.grabber.grabbedActors.Add(grabTarget);
		this.localGrabOffset = localOffset;
		CrittersPawn crittersPawn = grabTarget as CrittersPawn;
		if (crittersPawn.IsNotNull())
		{
			this.PlayHaptics(crittersPawn.grabbedHaptics, crittersPawn.grabbedHapticsStrength);
		}
	}

	// Token: 0x06000175 RID: 373 RVA: 0x0000962C File Offset: 0x0000782C
	private void DoRelease()
	{
		this.queuedGrab = null;
		this.grabber.grabbing = false;
		this.StopHaptics();
		for (int i = this.grabber.grabbedActors.Count - 1; i >= 0; i--)
		{
			CrittersActor crittersActor = this.grabber.grabbedActors[i];
			float magnitude = this.estimator.linearVelocity.magnitude;
			float num = magnitude + Mathf.Max(0f, magnitude - CrittersManager.instance.fastThrowThreshold) * CrittersManager.instance.fastThrowMultiplier;
			crittersActor.Released(true, crittersActor.transform.rotation, crittersActor.transform.position, this.estimator.linearVelocity.normalized * num, this.estimator.angularVelocity);
			if (i < this.grabber.grabbedActors.Count)
			{
				this.grabber.grabbedActors.RemoveAt(i);
			}
		}
		this.RemoveGrabberPhysicsTrigger();
		if (this.isHandGrabbingDisabled)
		{
			this.isHandGrabbingDisabled = false;
			if (this.isLeft)
			{
				EquipmentInteractor.instance.disableLeftGrab = false;
				return;
			}
			EquipmentInteractor.instance.disableRightGrab = false;
		}
	}

	// Token: 0x06000176 RID: 374 RVA: 0x00009764 File Offset: 0x00007964
	private void CheckApplyQueuedGrab()
	{
		if (Vector3.Magnitude(this.grabber.transform.InverseTransformPoint(this.queuedGrab.transform.position) - this.queuedRelativeGrabOffset) > this.grabDetachFromBagDist)
		{
			GorillaTagger.Instance.StartVibration(this.isLeft, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			if (this.queuedGrab.AllowGrabbingActor(this.grabber))
			{
				this.ApplyGrab(this.queuedGrab, this.queuedRelativeGrabRotation, this.queuedRelativeGrabOffset);
			}
			this.queuedGrab = null;
		}
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0000980C File Offset: 0x00007A0C
	private void VerifyExistingGrab()
	{
		for (int i = this.grabber.grabbedActors.Count - 1; i >= 0; i--)
		{
			CrittersActor crittersActor = this.grabber.grabbedActors[i];
			if (crittersActor.IsNull() || crittersActor.parentActorId != this.grabber.actorId)
			{
				if (this.grabber.IsNotNull())
				{
					this.grabber.grabbedActors.Remove(crittersActor);
				}
				this.RemoveGrabberPhysicsTrigger();
				this.StopHaptics();
			}
		}
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00009890 File Offset: 0x00007A90
	public void PlayHaptics(AudioClip clip, float strength)
	{
		if (clip == null)
		{
			return;
		}
		this.StopHaptics();
		this.playingHaptics = true;
		this.hapticsClip = clip;
		this.hapticsStrength = strength;
		this.hapticsLength = clip.length;
		this.haptics = base.StartCoroutine(this.PlayHapticsOnLoop());
	}

	// Token: 0x06000179 RID: 377 RVA: 0x000098E0 File Offset: 0x00007AE0
	public void StopHaptics()
	{
		if (this.playingHaptics)
		{
			this.playingHaptics = false;
			base.StopCoroutine(this.haptics);
			this.haptics = null;
			GorillaTagger.Instance.StopHapticClip(this.isLeft);
		}
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00009914 File Offset: 0x00007B14
	private IEnumerator PlayHapticsOnLoop()
	{
		for (;;)
		{
			GorillaTagger.Instance.PlayHapticClip(this.isLeft, this.hapticsClip, this.hapticsStrength);
			yield return new WaitForSeconds(this.hapticsLength);
		}
		yield break;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x00009924 File Offset: 0x00007B24
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
		CrittersActor crittersActor;
		if (!this.DoesActorActivateJoint(component, out crittersActor))
		{
			return;
		}
		this.ActivateJoints(component, crittersActor);
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00009960 File Offset: 0x00007B60
	private void ActivateJoints(CrittersActor rigidJoint, CrittersActor softJoint)
	{
		softJoint.SetJointSoft(this.grabber.rb);
		if (rigidJoint.parentActorId != -1)
		{
			rigidJoint.SetJointRigid(CrittersManager.instance.actorById[rigidJoint.parentActorId].rb);
		}
		CrittersGrabberSharedData.AddEnteredActor(rigidJoint);
	}

	// Token: 0x0600017D RID: 381 RVA: 0x000099B0 File Offset: 0x00007BB0
	private bool DoesActorActivateJoint(CrittersActor potentialBagActor, out CrittersActor heldStorableActor)
	{
		heldStorableActor = null;
		for (int i = 0; i < this.grabber.grabbedActors.Count; i++)
		{
			if (this.grabber.grabbedActors[i].localCanStore)
			{
				heldStorableActor = this.grabber.grabbedActors[i];
			}
		}
		CrittersActor crittersActor;
		return !(heldStorableActor == null) && potentialBagActor is CrittersBag && (!CrittersManager.instance.actorById.TryGetValue(potentialBagActor.parentActorId, out crittersActor) || !(crittersActor is CrittersAttachPoint) || (crittersActor as CrittersAttachPoint).rigPlayerId != PhotonNetwork.LocalPlayer.ActorNumber || (crittersActor as CrittersAttachPoint).anchorLocation != CrittersAttachPoint.AnchoredLocationTypes.Arm || (crittersActor as CrittersAttachPoint).isLeft != this.isLeft);
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00009A7C File Offset: 0x00007C7C
	private void AddGrabberPhysicsTrigger(CrittersActor actor)
	{
		CapsuleCollider capsuleCollider = CrittersManager.DuplicateCapsuleCollider(base.transform, actor.equipmentStoreTriggerCollider);
		capsuleCollider.isTrigger = true;
		this.triggerCollider = capsuleCollider;
		CrittersGrabberSharedData.AddTrigger(this.triggerCollider);
		this.rb.includeLayers = CrittersManager.instance.containerLayer;
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00009ACC File Offset: 0x00007CCC
	private void RemoveGrabberPhysicsTrigger()
	{
		if (this.triggerCollider != null)
		{
			CrittersGrabberSharedData.RemoveTrigger(this.triggerCollider);
			Object.Destroy(this.triggerCollider.gameObject);
		}
		this.triggerCollider = null;
		this.rb.includeLayers = 0;
	}

	// Token: 0x06000180 RID: 384 RVA: 0x00009B1C File Offset: 0x00007D1C
	private void NewJointMethod()
	{
		if (CrittersGrabberSharedData.triggerCollidersToCheck.Count == 0 && CrittersGrabberSharedData.enteredCritterActor.Count == 0)
		{
			return;
		}
		for (int i = 0; i < CrittersGrabberSharedData.actorGrabbers.Count; i++)
		{
			CrittersGrabberSharedData.actorGrabbers[i].actorsStillPresent.Clear();
			CapsuleCollider capsuleCollider = CrittersGrabberSharedData.actorGrabbers[i].triggerCollider;
			if (!(capsuleCollider == null))
			{
				Vector3 vector = capsuleCollider.transform.up * MathF.Max(0f, capsuleCollider.height / 2f - capsuleCollider.radius);
				int num = Physics.OverlapCapsuleNonAlloc(capsuleCollider.transform.position + vector, capsuleCollider.transform.position - vector, capsuleCollider.radius, this.colliders, CrittersManager.instance.containerLayer, QueryTriggerInteraction.Collide);
				if (num != 0)
				{
					for (int j = 0; j < num; j++)
					{
						Rigidbody attachedRigidbody = this.colliders[j].attachedRigidbody;
						if (!(attachedRigidbody == null))
						{
							CrittersActor component = attachedRigidbody.GetComponent<CrittersActor>();
							if (!(component == null) && !CrittersGrabberSharedData.actorGrabbers[i].actorsStillPresent.Contains(component))
							{
								CrittersGrabberSharedData.actorGrabbers[i].actorsStillPresent.Add(component);
							}
						}
					}
				}
			}
		}
		for (int k = 0; k < CrittersGrabberSharedData.actorGrabbers.Count; k++)
		{
			CrittersActorGrabber crittersActorGrabber = CrittersGrabberSharedData.actorGrabbers[k];
			for (int l = 0; l < CrittersGrabberSharedData.actorGrabbers[k].actorsStillPresent.Count; l++)
			{
				CrittersActor crittersActor = CrittersGrabberSharedData.actorGrabbers[k].actorsStillPresent[l];
				CrittersActor crittersActor2;
				if (crittersActorGrabber.DoesActorActivateJoint(crittersActor, out crittersActor2))
				{
					crittersActorGrabber.ActivateJoints(crittersActor, crittersActor2);
				}
			}
		}
		for (int m = CrittersGrabberSharedData.enteredCritterActor.Count - 1; m >= 0; m--)
		{
			CrittersActor crittersActor3 = CrittersGrabberSharedData.enteredCritterActor[m];
			bool flag = false;
			for (int n = 0; n < CrittersGrabberSharedData.actorGrabbers.Count; n++)
			{
				flag |= CrittersGrabberSharedData.actorGrabbers[n].actorsStillPresent.Contains(crittersActor3);
			}
			if (!flag)
			{
				CrittersGrabberSharedData.RemoveEnteredActor(crittersActor3);
				crittersActor3.DisconnectJoint();
			}
		}
		CrittersGrabberSharedData.DisableEmptyGrabberJoints();
	}

	// Token: 0x04000189 RID: 393
	public bool isGrabbing;

	// Token: 0x0400018A RID: 394
	public Collider[] colliders = new Collider[50];

	// Token: 0x0400018B RID: 395
	public bool isLeft;

	// Token: 0x0400018C RID: 396
	public float grabRadius = 0.05f;

	// Token: 0x0400018D RID: 397
	public float grabBreakRadius = 0.15f;

	// Token: 0x0400018E RID: 398
	private float grabDetachFromBagDist = 0.05f;

	// Token: 0x0400018F RID: 399
	public Transform transformToFollow;

	// Token: 0x04000190 RID: 400
	public GorillaVelocityEstimator estimator;

	// Token: 0x04000191 RID: 401
	public CrittersGrabber grabber;

	// Token: 0x04000192 RID: 402
	public float vibrationStartDistance;

	// Token: 0x04000193 RID: 403
	public float vibrationEndDistance;

	// Token: 0x04000194 RID: 404
	public CrittersActorGrabber otherHand;

	// Token: 0x04000195 RID: 405
	private bool isHandGrabbingDisabled;

	// Token: 0x04000196 RID: 406
	private float grabDuration = 0.3f;

	// Token: 0x04000197 RID: 407
	private float remainingGrabDuration;

	// Token: 0x04000198 RID: 408
	private bool playingHaptics;

	// Token: 0x04000199 RID: 409
	private AudioClip hapticsClip;

	// Token: 0x0400019A RID: 410
	private float hapticsStrength;

	// Token: 0x0400019B RID: 411
	private float hapticsLength;

	// Token: 0x0400019C RID: 412
	private Coroutine haptics;

	// Token: 0x0400019D RID: 413
	public CapsuleCollider triggerCollider;

	// Token: 0x0400019E RID: 414
	private Rigidbody rb;

	// Token: 0x0400019F RID: 415
	private CrittersActor validGrabTarget;

	// Token: 0x040001A0 RID: 416
	private CrittersActor lastHover;

	// Token: 0x040001A1 RID: 417
	private Vector3 localGrabOffset;

	// Token: 0x040001A2 RID: 418
	private CrittersActor queuedGrab;

	// Token: 0x040001A3 RID: 419
	private Vector3 queuedRelativeGrabOffset;

	// Token: 0x040001A4 RID: 420
	private Quaternion queuedRelativeGrabRotation;

	// Token: 0x040001A5 RID: 421
	public List<CrittersActor> actorsStillPresent;
}
