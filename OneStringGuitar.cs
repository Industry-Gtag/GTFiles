using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200053B RID: 1339
public class OneStringGuitar : TransferrableObject
{
	// Token: 0x060021CE RID: 8654 RVA: 0x000B4184 File Offset: 0x000B2384
	public override Matrix4x4 GetDefaultTransformationMatrix()
	{
		return Matrix4x4.identity;
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x000B418C File Offset: 0x000B238C
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.chestColliderLeft = this._GetChestColliderByPath(rig, "rig/body_pivot/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformLeft");
		this.chestColliderRight = this._GetChestColliderByPath(rig, "rig/body_pivot/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformRight");
		this.currentChestCollider = this.chestColliderLeft;
		Transform[] array;
		string text;
		if (!GTHardCodedBones.TryGetBoneXforms(rig, out array, out text))
		{
			Debug.LogError("OneStringGuitar: Error getting bone Transforms: " + text, this);
			return;
		}
		this.parentHandLeft = array[9];
		this.parentHandRight = array[27];
		this.parentHand = this.parentHandRight;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.nullHit = default(RaycastHit);
		this.strumList.Add(this.strumCollider);
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.startingLeftChestOffset = this.chestOffsetLeft;
		this.startingRightChestOffset = this.chestOffsetRight;
		this.startingUnsnapDistance = this.unsnapDistance;
		this.selfInstrumentIndex = rig.AssignInstrumentToInstrumentSelfOnly(this);
		for (int i = 0; i < this.frets.Length; i++)
		{
			this.fretsList.Add(this.frets[i]);
		}
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x000B42D4 File Offset: 0x000B24D4
	private Collider _GetChestColliderByPath(VRRig vrRig, string chestColliderLeftPath)
	{
		Transform transform;
		if (!vrRig.transform.TryFindByExactPath(chestColliderLeftPath, out transform))
		{
			Debug.LogError("DEACTIVATING! do you move this without updating the script? could not find this transform: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		Collider component = transform.GetComponent<Collider>();
		if (!component)
		{
			Debug.LogError("DEACTIVATING! found transform but couldn't find collider at path: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		return component;
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x000B4344 File Offset: 0x000B2544
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this.currentState == TransferrableObject.PositionState.InLeftHand)
		{
			this.fretHandIndicator = this.leftHandIndicator;
			this.strumHandIndicator = this.rightHandIndicator;
		}
		else
		{
			this.fretHandIndicator = this.rightHandIndicator;
			this.strumHandIndicator = this.leftHandIndicator;
		}
		if (base.IsLocalObject())
		{
			this.parentHand = GTPlayer.Instance.GetHandFollower(this.currentState == TransferrableObject.PositionState.InLeftHand);
		}
		this.initOffset = Vector3.zero;
		this.initRotation = Quaternion.identity;
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x000B43C9 File Offset: 0x000B25C9
	internal override void OnDisable()
	{
		base.OnDisable();
		this.angleSnapped = false;
		this.positionSnapped = false;
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x000B43ED File Offset: 0x000B25ED
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
		return true;
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x000B4418 File Offset: 0x000B2618
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.lastState != (OneStringGuitar.GuitarStates)this.itemState)
		{
			this.angleSnapped = false;
			this.positionSnapped = false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			Vector3 vector = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startPositionLeft : this.startPositionRight);
			Quaternion quaternion = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startQuatLeft : this.startQuatRight);
			this.UpdateNonPlayingPosition(vector, quaternion);
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			Vector3 vector2 = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripPositionLeft : this.reverseGripPositionRight);
			Quaternion quaternion2 = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripQuatLeft : this.reverseGripQuatRight);
			this.UpdateNonPlayingPosition(vector2, quaternion2);
			if (this.IsMyItem() && (this.chestTouch.transform.position - this.currentChestCollider.transform.position).magnitude < this.snapDistance)
			{
				this.itemState = TransferrableObject.ItemStates.State2;
				this.angleSnapped = false;
				this.positionSnapped = false;
				this.currentChestCollider.gameObject.SetActive(true);
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			Quaternion quaternion3 = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.holdingOffsetRotationLeft : this.holdingOffsetRotationRight);
			Vector3 vector3 = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.chestOffsetLeft : this.chestOffsetRight);
			Quaternion quaternion4 = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * quaternion3;
			if (!this.angleSnapped && Quaternion.Angle(base.transform.rotation, quaternion4) > this.angleLerpSnap)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion4, this.lerpValue);
			}
			else
			{
				this.angleSnapped = true;
				base.transform.rotation = quaternion4;
			}
			Vector3 vector4 = this.currentChestCollider.transform.position + base.transform.rotation * vector3;
			if (!this.positionSnapped && (base.transform.position - vector4).magnitude > this.vectorLerpSnap)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.currentChestCollider.transform.position + base.transform.rotation * vector3, this.lerpValue);
			}
			else
			{
				this.positionSnapped = true;
				base.transform.position = vector4;
			}
			if (this.currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.parentHand = this.parentHandRight;
			}
			else
			{
				this.parentHand = this.parentHandLeft;
			}
			if (this.IsMyItem())
			{
				this.unsnapDistance = this.startingUnsnapDistance * base.myRig.transform.localScale.x;
				if (this.currentState == TransferrableObject.PositionState.InRightHand)
				{
					this.chestOffsetRight = Vector3.Scale(this.startingRightChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderRight;
					this.fretHandIndicator = this.rightHandIndicator;
					this.strumHandIndicator = this.leftHandIndicator;
				}
				else
				{
					this.chestOffsetLeft = Vector3.Scale(this.startingLeftChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderLeft;
					this.fretHandIndicator = this.leftHandIndicator;
					this.strumHandIndicator = this.rightHandIndicator;
				}
				if (this.Unsnap())
				{
					this.itemState = TransferrableObject.ItemStates.State1;
					this.angleSnapped = false;
					this.positionSnapped = false;
					if (this.currentState == TransferrableObject.PositionState.InLeftHand)
					{
						EquipmentInteractor.instance.wasLeftGrabPressed = true;
					}
					else
					{
						EquipmentInteractor.instance.wasRightGrabPressed = true;
					}
					this.currentChestCollider.gameObject.SetActive(false);
				}
				else
				{
					if (!this.handIn)
					{
						this.CheckFretFinger(this.fretHandIndicator.transform);
						HitChecker.CheckHandHit(ref this.collidersHitCount, this.interactableMask, this.sphereRadius, ref this.nullHit, ref this.raycastHits, ref this.raycastHitList, ref this.spherecastSweep, ref this.strumHandIndicator);
						if (this.collidersHitCount > 0)
						{
							int i = 0;
							while (i < this.collidersHitCount)
							{
								if (this.raycastHits[i].collider != null && this.strumCollider == this.raycastHits[i].collider)
								{
									GorillaTagger.Instance.StartVibration(this.strumHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
									this.PlayNote(this.currentFretIndex, Mathf.Max(Mathf.Min(1f, this.strumHandIndicator.currentVelocity.magnitude / this.maxVelocity) * this.maxVolume, this.minVolume));
									if (!NetworkSystem.Instance.InRoom || this.selfInstrumentIndex <= -1)
									{
										break;
									}
									NetworkView myVRRig = GorillaTagger.Instance.myVRRig;
									if (myVRRig == null)
									{
										break;
									}
									myVRRig.SendRPC("RPC_PlaySelfOnlyInstrument", RpcTarget.Others, new object[]
									{
										this.selfInstrumentIndex,
										this.currentFretIndex,
										this.audioSource.volume
									});
									break;
								}
								else
								{
									i++;
								}
							}
						}
					}
					this.handIn = HitChecker.CheckHandIn(ref this.anyHit, ref this.collidersHit, this.sphereRadius * base.transform.lossyScale.x, this.interactableMask, ref this.strumHandIndicator, ref this.strumList);
				}
			}
		}
		this.lastState = (OneStringGuitar.GuitarStates)this.itemState;
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x000B49D4 File Offset: 0x000B2BD4
	public override void PlayNote(int note, float volume)
	{
		this.audioSource.time = 0.005f;
		this.audioSource.clip = this.audioClips[note];
		this.audioSource.volume = volume;
		this.audioSource.GTPlay();
		base.PlayNote(note, volume);
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000B4A24 File Offset: 0x000B2C24
	private bool Unsnap()
	{
		return (this.parentHand.position - this.chestTouch.position).magnitude > this.unsnapDistance;
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x000B4A5C File Offset: 0x000B2C5C
	private void CheckFretFinger(Transform finger)
	{
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = null;
		}
		this.collidersHitCount = Physics.OverlapSphereNonAlloc(finger.position, this.sphereRadius, this.collidersHit, this.interactableMask, QueryTriggerInteraction.Collide);
		this.currentFretIndex = 5;
		if (this.collidersHitCount > 0)
		{
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.fretsList.Contains(this.collidersHit[j]))
				{
					this.currentFretIndex = this.fretsList.IndexOf(this.collidersHit[j]);
					if (this.currentFretIndex != this.lastFretIndex)
					{
						GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
					}
					this.lastFretIndex = this.currentFretIndex;
					return;
				}
			}
			return;
		}
		if (this.lastFretIndex != -1)
		{
			GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
		}
		this.lastFretIndex = -1;
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x000B4B90 File Offset: 0x000B2D90
	public void UpdateNonPlayingPosition(Vector3 positionTarget, Quaternion rotationTarget)
	{
		if (!this.angleSnapped)
		{
			if (Quaternion.Angle(rotationTarget, base.transform.localRotation) < this.angleLerpSnap)
			{
				this.angleSnapped = true;
				base.transform.localRotation = rotationTarget;
			}
			else
			{
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, rotationTarget, this.lerpValue);
			}
		}
		if (!this.positionSnapped)
		{
			if ((base.transform.localPosition - positionTarget).magnitude < this.vectorLerpSnap)
			{
				this.positionSnapped = true;
				base.transform.localPosition = positionTarget;
				return;
			}
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, positionTarget, this.lerpValue);
		}
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x000B4C54 File Offset: 0x000B2E54
	public override bool CanDeactivate()
	{
		return !base.gameObject.activeSelf || this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x000B4C77 File Offset: 0x000B2E77
	public override bool CanActivate()
	{
		return this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x000B4C8D File Offset: 0x000B2E8D
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.itemState = TransferrableObject.ItemStates.State1;
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x000B4CB0 File Offset: 0x000B2EB0
	public void GenerateVectorOffsetLeft()
	{
		this.chestOffsetLeft = base.transform.position - this.chestColliderLeft.transform.position;
		this.holdingOffsetRotationLeft = Quaternion.LookRotation(base.transform.position - this.chestColliderLeft.transform.position);
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x000B4D10 File Offset: 0x000B2F10
	public void GenerateVectorOffsetRight()
	{
		this.chestOffsetRight = base.transform.position - this.chestColliderRight.transform.position;
		this.holdingOffsetRotationRight = Quaternion.LookRotation(base.transform.position - this.chestColliderRight.transform.position);
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x000B4D6E File Offset: 0x000B2F6E
	public void GenerateReverseGripOffsetLeft()
	{
		this.reverseGripPositionLeft = base.transform.localPosition;
		this.reverseGripQuatLeft = base.transform.localRotation;
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000B4D92 File Offset: 0x000B2F92
	public void GenerateClubOffsetLeft()
	{
		this.startPositionLeft = base.transform.localPosition;
		this.startQuatLeft = base.transform.localRotation;
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000B4DB6 File Offset: 0x000B2FB6
	public void GenerateReverseGripOffsetRight()
	{
		this.reverseGripPositionRight = base.transform.localPosition;
		this.reverseGripQuatRight = base.transform.localRotation;
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000B4DDA File Offset: 0x000B2FDA
	public void GenerateClubOffsetRight()
	{
		this.startPositionRight = base.transform.localPosition;
		this.startQuatRight = base.transform.localRotation;
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000B4DFE File Offset: 0x000B2FFE
	public void TestClubPositionRight()
	{
		base.transform.localPosition = this.startPositionRight;
		base.transform.localRotation = this.startQuatRight;
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000B4E22 File Offset: 0x000B3022
	public void TestReverseGripPositionRight()
	{
		base.transform.localPosition = this.reverseGripPositionRight;
		base.transform.localRotation = this.reverseGripQuatRight;
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x000B4E48 File Offset: 0x000B3048
	public void TestPlayingPositionRight()
	{
		base.transform.rotation = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * this.holdingOffsetRotationRight;
		base.transform.position = this.chestColliderRight.transform.position + base.transform.rotation * this.chestOffsetRight;
	}

	// Token: 0x04002C84 RID: 11396
	public Vector3 chestOffsetLeft;

	// Token: 0x04002C85 RID: 11397
	public Vector3 chestOffsetRight;

	// Token: 0x04002C86 RID: 11398
	public Quaternion holdingOffsetRotationLeft;

	// Token: 0x04002C87 RID: 11399
	public Quaternion holdingOffsetRotationRight;

	// Token: 0x04002C88 RID: 11400
	public Quaternion chestRotationOffset;

	// Token: 0x04002C89 RID: 11401
	[NonSerialized]
	public Collider currentChestCollider;

	// Token: 0x04002C8A RID: 11402
	[NonSerialized]
	public Collider chestColliderLeft;

	// Token: 0x04002C8B RID: 11403
	[NonSerialized]
	public Collider chestColliderRight;

	// Token: 0x04002C8C RID: 11404
	public float lerpValue = 0.25f;

	// Token: 0x04002C8D RID: 11405
	public AudioSource audioSource;

	// Token: 0x04002C8E RID: 11406
	private Transform parentHand;

	// Token: 0x04002C8F RID: 11407
	private Transform parentHandLeft;

	// Token: 0x04002C90 RID: 11408
	private Transform parentHandRight;

	// Token: 0x04002C91 RID: 11409
	public float unsnapDistance;

	// Token: 0x04002C92 RID: 11410
	public float snapDistance;

	// Token: 0x04002C93 RID: 11411
	public Vector3 startPositionLeft;

	// Token: 0x04002C94 RID: 11412
	public Quaternion startQuatLeft;

	// Token: 0x04002C95 RID: 11413
	public Vector3 reverseGripPositionLeft;

	// Token: 0x04002C96 RID: 11414
	public Quaternion reverseGripQuatLeft;

	// Token: 0x04002C97 RID: 11415
	public Vector3 startPositionRight;

	// Token: 0x04002C98 RID: 11416
	public Quaternion startQuatRight;

	// Token: 0x04002C99 RID: 11417
	public Vector3 reverseGripPositionRight;

	// Token: 0x04002C9A RID: 11418
	public Quaternion reverseGripQuatRight;

	// Token: 0x04002C9B RID: 11419
	public float angleLerpSnap = 1f;

	// Token: 0x04002C9C RID: 11420
	public float vectorLerpSnap = 0.01f;

	// Token: 0x04002C9D RID: 11421
	private bool angleSnapped;

	// Token: 0x04002C9E RID: 11422
	private bool positionSnapped;

	// Token: 0x04002C9F RID: 11423
	public Transform chestTouch;

	// Token: 0x04002CA0 RID: 11424
	private int collidersHitCount;

	// Token: 0x04002CA1 RID: 11425
	private Collider[] collidersHit = new Collider[20];

	// Token: 0x04002CA2 RID: 11426
	private RaycastHit[] raycastHits = new RaycastHit[20];

	// Token: 0x04002CA3 RID: 11427
	private List<RaycastHit> raycastHitList = new List<RaycastHit>();

	// Token: 0x04002CA4 RID: 11428
	private RaycastHit nullHit;

	// Token: 0x04002CA5 RID: 11429
	public Collider[] collidersToBeIn;

	// Token: 0x04002CA6 RID: 11430
	public LayerMask interactableMask;

	// Token: 0x04002CA7 RID: 11431
	public int currentFretIndex;

	// Token: 0x04002CA8 RID: 11432
	public int lastFretIndex;

	// Token: 0x04002CA9 RID: 11433
	public Collider[] frets;

	// Token: 0x04002CAA RID: 11434
	private List<Collider> fretsList = new List<Collider>();

	// Token: 0x04002CAB RID: 11435
	public AudioClip[] audioClips;

	// Token: 0x04002CAC RID: 11436
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x04002CAD RID: 11437
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x04002CAE RID: 11438
	private GorillaTriggerColliderHandIndicator fretHandIndicator;

	// Token: 0x04002CAF RID: 11439
	private GorillaTriggerColliderHandIndicator strumHandIndicator;

	// Token: 0x04002CB0 RID: 11440
	private float sphereRadius;

	// Token: 0x04002CB1 RID: 11441
	private bool anyHit;

	// Token: 0x04002CB2 RID: 11442
	private bool handIn;

	// Token: 0x04002CB3 RID: 11443
	private Vector3 spherecastSweep;

	// Token: 0x04002CB4 RID: 11444
	public Collider strumCollider;

	// Token: 0x04002CB5 RID: 11445
	public float maxVolume = 1f;

	// Token: 0x04002CB6 RID: 11446
	public float minVolume = 0.05f;

	// Token: 0x04002CB7 RID: 11447
	public float maxVelocity = 2f;

	// Token: 0x04002CB8 RID: 11448
	private List<Collider> strumList = new List<Collider>();

	// Token: 0x04002CB9 RID: 11449
	private int selfInstrumentIndex = -1;

	// Token: 0x04002CBA RID: 11450
	private OneStringGuitar.GuitarStates lastState;

	// Token: 0x04002CBB RID: 11451
	private Vector3 startingLeftChestOffset;

	// Token: 0x04002CBC RID: 11452
	private Vector3 startingRightChestOffset;

	// Token: 0x04002CBD RID: 11453
	private float startingUnsnapDistance;

	// Token: 0x0200053C RID: 1340
	private enum GuitarStates
	{
		// Token: 0x04002CBF RID: 11455
		Club = 1,
		// Token: 0x04002CC0 RID: 11456
		HeldReverseGrip,
		// Token: 0x04002CC1 RID: 11457
		Playing = 4
	}
}
