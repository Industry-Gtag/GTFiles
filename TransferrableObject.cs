using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using JetBrains.Annotations;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x0200054E RID: 1358
public class TransferrableObject : HoldableObject, ISelfValidator, IRequestableOwnershipGuardCallbacks, IPreDisable, ISpawnable, IBuildValidation
{
	// Token: 0x0600224E RID: 8782 RVA: 0x000B79A8 File Offset: 0x000B5BA8
	public void FixTransformOverride()
	{
		this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Validate(SelfValidationResult result)
	{
	}

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x06002250 RID: 8784 RVA: 0x000B79B6 File Offset: 0x000B5BB6
	// (set) Token: 0x06002251 RID: 8785 RVA: 0x000B79BE File Offset: 0x000B5BBE
	public VRRig myRig
	{
		get
		{
			return this._myRig;
		}
		private set
		{
			this._myRig = value;
		}
	}

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x06002252 RID: 8786 RVA: 0x000B79C7 File Offset: 0x000B5BC7
	// (set) Token: 0x06002253 RID: 8787 RVA: 0x000B79CF File Offset: 0x000B5BCF
	public bool isMyRigValid { get; private set; }

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x06002254 RID: 8788 RVA: 0x000B79D8 File Offset: 0x000B5BD8
	// (set) Token: 0x06002255 RID: 8789 RVA: 0x000B79E0 File Offset: 0x000B5BE0
	public VRRig myOnlineRig
	{
		get
		{
			return this._myOnlineRig;
		}
		private set
		{
			this._myOnlineRig = value;
			this.isMyOnlineRigValid = true;
		}
	}

	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x06002256 RID: 8790 RVA: 0x000B79F0 File Offset: 0x000B5BF0
	// (set) Token: 0x06002257 RID: 8791 RVA: 0x000B79F8 File Offset: 0x000B5BF8
	public bool isMyOnlineRigValid { get; private set; }

	// Token: 0x06002258 RID: 8792 RVA: 0x000B7A04 File Offset: 0x000B5C04
	public void SetTargetRig(VRRig rig)
	{
		if (rig == null)
		{
			this.targetRigSet = false;
			if (this.isSceneObject)
			{
				this.targetRig = rig;
				this.targetDockPositions = null;
				this.anchorOverrides = null;
				return;
			}
			if (this.myRig)
			{
				this.SetTargetRig(this.myRig);
			}
			if (this.myOnlineRig)
			{
				this.SetTargetRig(this.myOnlineRig);
			}
			return;
		}
		else
		{
			this.targetRigSet = true;
			this.targetRig = rig;
			BodyDockPositions component = rig.GetComponent<BodyDockPositions>();
			VRRigAnchorOverrides component2 = rig.GetComponent<VRRigAnchorOverrides>();
			if (!component)
			{
				Debug.LogError("There is no dock attached to this rig", this);
				return;
			}
			if (!component2)
			{
				Debug.LogError("There is no overrides attached to this rig", this);
				return;
			}
			this.anchorOverrides = component2;
			this.targetDockPositions = component;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			return;
		}
	}

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06002259 RID: 8793 RVA: 0x000B7AD4 File Offset: 0x000B5CD4
	public bool IsLocalOwnedWorldShareable
	{
		get
		{
			return this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine;
		}
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x000B7AF8 File Offset: 0x000B5CF8
	public void WorldShareableRequestOwnership()
	{
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
	}

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x0600225B RID: 8795 RVA: 0x000B7B54 File Offset: 0x000B5D54
	// (set) Token: 0x0600225C RID: 8796 RVA: 0x000B7B5C File Offset: 0x000B5D5C
	public bool isRigidbodySet { get; private set; }

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x0600225D RID: 8797 RVA: 0x000B7B65 File Offset: 0x000B5D65
	// (set) Token: 0x0600225E RID: 8798 RVA: 0x000B7B6D File Offset: 0x000B5D6D
	public bool shouldUseGravity { get; private set; }

	// Token: 0x0600225F RID: 8799 RVA: 0x000B7B76 File Offset: 0x000B5D76
	protected virtual void Awake()
	{
		if (this.isSceneObject)
		{
			this.IsSpawned = true;
			this.OnSpawn(null);
		}
	}

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x06002260 RID: 8800 RVA: 0x000B7B8E File Offset: 0x000B5D8E
	// (set) Token: 0x06002261 RID: 8801 RVA: 0x000B7B96 File Offset: 0x000B5D96
	public bool IsSpawned { get; set; }

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x06002262 RID: 8802 RVA: 0x000B7B9F File Offset: 0x000B5D9F
	// (set) Token: 0x06002263 RID: 8803 RVA: 0x000B7BA7 File Offset: 0x000B5DA7
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06002264 RID: 8804 RVA: 0x000B7BB0 File Offset: 0x000B5DB0
	public virtual void OnSpawn(VRRig rig)
	{
		try
		{
			if (!this.isSceneObject)
			{
				if (!rig)
				{
					Debug.LogError("Disabling TransferrableObject because could not find VRRig! \"" + base.transform.GetPath() + "\"", this);
					base.enabled = false;
					this.isMyRigValid = false;
					this.isMyOnlineRigValid = false;
					return;
				}
				this.myRig = (rig.isOfflineVRRig ? rig : null);
				this.myOnlineRig = (rig.isOfflineVRRig ? null : rig);
				this.targetDockPositions = rig.myBodyDockPositions;
			}
			else
			{
				this.myRig = null;
				this.myOnlineRig = null;
			}
			this.isMyRigValid = true;
			this.isMyOnlineRigValid = true;
			if (this.isSceneObject)
			{
				this.targetDockPositions = base.GetComponentInParent<BodyDockPositions>();
			}
			this.anchor = base.transform.parent;
			if (this.rigidbodyInstance == null)
			{
				this.rigidbodyInstance = base.GetComponent<Rigidbody>();
			}
			if (this.rigidbodyInstance != null)
			{
				this.isRigidbodySet = true;
				this.shouldUseGravity = this.rigidbodyInstance.useGravity;
			}
			this.audioSrc = base.GetComponent<AudioSource>();
			this.latched = false;
			if (!this.positionInitialized)
			{
				this.SetInitMatrix();
				this.positionInitialized = true;
			}
			if (this.anchor == null)
			{
				this.InitialDockObject = base.transform.parent;
			}
			else
			{
				this.InitialDockObject = this.anchor.parent;
			}
			this.isGrabAnchorSet = this.grabAnchor != null;
			if (this.isSceneObject)
			{
				foreach (ISpawnable spawnable in base.GetComponentsInChildren<ISpawnable>(true))
				{
					if (spawnable != this)
					{
						spawnable.IsSpawned = true;
						spawnable.CosmeticSelectedSide = this.CosmeticSelectedSide;
						spawnable.OnSpawn(this.myRig);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x000B7DC0 File Offset: 0x000B5FC0
	public virtual void OnDespawn()
	{
		try
		{
			if (!this.isSceneObject)
			{
				foreach (ISpawnable spawnable in base.GetComponentsInChildren<ISpawnable>(true))
				{
					if (spawnable != this)
					{
						spawnable.IsSpawned = false;
						spawnable.OnDespawn();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000B7E48 File Offset: 0x000B6048
	private void SetInitMatrix()
	{
		this.initMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		if (this.handPoseLeft != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseLeftReferenceRotation * Quaternion.Inverse(this.handPoseLeft.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseLeftReferencePoint) - this.handPoseLeft.transform.position;
			this.leftHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.leftHandMatrix = this.initMatrix;
		}
		if (this.handPoseRight != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseRightReferenceRotation * Quaternion.Inverse(this.handPoseRight.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseRightReferencePoint) - this.handPoseRight.transform.position;
			this.rightHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.rightHandMatrix = this.initMatrix;
		}
		base.transform.localPosition = this.initMatrix.Position();
		base.transform.localRotation = (in this.initMatrix).Rotation();
		this.positionInitialized = true;
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void Start()
	{
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x000B7FC0 File Offset: 0x000B61C0
	internal virtual void OnEnable()
	{
		try
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
			this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject();
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
		{
			this.previousItemState = (TransferrableObject.ItemStates)0;
			this.itemState = (TransferrableObject.ItemStates)0;
		}
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x000B8078 File Offset: 0x000B6278
	public virtual void OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (!base.enabled)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		try
		{
			TransferrableObjectManager.Register(this);
			this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
			if (!this.positionInitialized)
			{
				this.SetInitMatrix();
				this.positionInitialized = true;
			}
			if (this.isSceneObject)
			{
				if (!this.worldShareableInstance)
				{
					Debug.LogError("Missing Sharable Instance on Scene enabled object: " + base.gameObject.name);
				}
				else
				{
					this.worldShareableInstance.SyncToSceneObject(this);
					this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().AddCallbackTarget(this);
				}
			}
			else
			{
				if (!this.isSceneObject && !this.myRig && !this.myOnlineRig && !this.ownerRig)
				{
					this.ownerRig = base.GetComponentInParent<VRRig>(true);
					if (this.ownerRig.isOfflineVRRig)
					{
						this.myRig = this.ownerRig;
					}
					else
					{
						this.myOnlineRig = this.ownerRig;
					}
				}
				if (!this.myRig && this.myOnlineRig)
				{
					this.ownerRig = this.myOnlineRig;
					this.SetTargetRig(this.myOnlineRig);
				}
				if (!this.IsSpawned)
				{
					this.IsSpawned = true;
					this.OnSpawn((this.myRig != null) ? this.myRig : this.myOnlineRig);
				}
				if (this.myRig == null && this.myOnlineRig == null)
				{
					if (!this.isSceneObject)
					{
						base.gameObject.SetActive(false);
					}
				}
				else
				{
					this.objectIndex = this.targetDockPositions.ReturnTransferrableItemIndex(this.myIndex);
					if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
					{
						this.storedZone = BodyDockPositions.DropPositions.LeftArm;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
					{
						this.storedZone = BodyDockPositions.DropPositions.RightArm;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
					{
						this.storedZone = BodyDockPositions.DropPositions.LeftBack;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
					{
						this.storedZone = BodyDockPositions.DropPositions.RightBack;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnChest)
					{
						this.storedZone = BodyDockPositions.DropPositions.Chest;
					}
					if (this.IsLocalObject())
					{
						this.ownerRig = GorillaTagger.Instance.offlineVRRig;
						this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
					}
					if (this.objectIndex == -1)
					{
						base.gameObject.SetActive(false);
					}
					else
					{
						if (this.currentState == TransferrableObject.PositionState.OnLeftArm && this.flipOnXForLeftArm)
						{
							Transform transform = this.GetAnchor(this.currentState);
							transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
						}
						this.initState = this.currentState;
						this.enabledOnFrame = Time.frameCount;
						this.startInterpolation = true;
						if (NetworkSystem.Instance.InRoom)
						{
							if (this.canDrop || this.shareable)
							{
								this.SpawnTransferableObjectViews();
								if (this.myRig)
								{
									if (this.myRig != null && this.worldShareableInstance != null)
									{
										this.OnWorldShareableItemSpawn();
									}
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x000B83F0 File Offset: 0x000B65F0
	internal virtual void OnDisable()
	{
		TransferrableObjectManager.Unregister(this);
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
		this.enabledOnFrame = -1;
		base.transform.localScale = Vector3.one;
		try
		{
			if (!this.isSceneObject && this.IsLocalObject() && this.worldShareableInstance && !this.IsMyItem())
			{
				this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
				{
				});
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.Invalidate();
				this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
				if (this.targetDockPositions)
				{
					this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
				}
				if (!this.isSceneObject)
				{
					this.worldShareableInstance = null;
				}
			}
			this.PlayDestroyedOrDisabledEffect();
			if (this.isSceneObject)
			{
				this.IsSpawned = false;
				this.OnDespawn();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x000B8568 File Offset: 0x000B6768
	protected new virtual void OnDestroy()
	{
		TransferrableObjectManager.Unregister(this);
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x000B8570 File Offset: 0x000B6770
	public void CleanupDisable()
	{
		this.currentState = TransferrableObject.PositionState.None;
		this.enabledOnFrame = -1;
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			if (this.anchor != base.transform)
			{
				base.transform.parent = this.anchor;
			}
		}
		else
		{
			base.transform.parent = this.anchor;
		}
		this.interpState = TransferrableObject.InterpolateState.None;
		Transform transform = base.transform;
		Matrix4x4 defaultTransformationMatrix = this.GetDefaultTransformationMatrix();
		transform.SetLocalMatrixRelativeToParentWithXParity(in defaultTransformationMatrix);
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000B85FB File Offset: 0x000B67FB
	public virtual void PreDisable()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
		{
			this.previousItemState = (TransferrableObject.ItemStates)0;
			this.itemState = (TransferrableObject.ItemStates)0;
		}
		this.currentState = TransferrableObject.PositionState.None;
		this.interpState = TransferrableObject.InterpolateState.None;
		this.ResetToDefaultState();
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x000B8630 File Offset: 0x000B6830
	public virtual Matrix4x4 GetDefaultTransformationMatrix()
	{
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState == TransferrableObject.PositionState.InLeftHand)
		{
			return this.leftHandMatrix;
		}
		if (positionState != TransferrableObject.PositionState.InRightHand)
		{
			return this.initMatrix;
		}
		return this.rightHandMatrix;
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000B8662 File Offset: 0x000B6862
	public virtual bool ShouldBeKinematic()
	{
		if (this.detatchOnGrab)
		{
			return this.currentState != TransferrableObject.PositionState.Dropped && this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand;
		}
		return this.currentState != TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000B86A0 File Offset: 0x000B68A0
	private void SpawnShareableObject()
	{
		if (this.isSceneObject)
		{
			if (this.worldShareableInstance == null)
			{
				return;
			}
			this.worldShareableInstance.GetComponent<WorldShareableItem>().SetupSceneObjectOnNetwork(NetworkSystem.Instance.MasterClient);
			return;
		}
		else
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			this.SpawnTransferableObjectViews();
			if (!this.myRig)
			{
				return;
			}
			if (!this.canDrop && !this.shareable)
			{
				return;
			}
			if (this.myRig != null && this.worldShareableInstance != null)
			{
				this.OnWorldShareableItemSpawn();
			}
			return;
		}
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000B8734 File Offset: 0x000B6934
	public void SpawnTransferableObjectViews()
	{
		NetPlayer owner = NetworkSystem.Instance.LocalPlayer;
		if (!this.ownerRig.isOfflineVRRig)
		{
			owner = this.ownerRig.creator;
		}
		if (this.worldShareableInstance == null)
		{
			this.worldShareableInstance = this.targetDockPositions.AllocateSharableInstance(this.storedZone, owner);
		}
		GorillaTagger.OnPlayerSpawned(delegate
		{
			this.worldShareableInstance.SetupSharableObject(this.myIndex, owner, this.transform);
		});
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x000B87B8 File Offset: 0x000B69B8
	public virtual void OnJoinedRoom()
	{
		if (this.isSceneObject)
		{
			this.worldShareableInstance == null;
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		this.SpawnTransferableObjectViews();
		if (!this.myRig)
		{
			return;
		}
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x06002273 RID: 8819 RVA: 0x000B8830 File Offset: 0x000B6A30
	public virtual void OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.isSceneObject)
		{
			return;
		}
		if (!this.shareable && !this.allowWorldSharableInstance && !this.canDrop)
		{
			return;
		}
		if (base.gameObject.activeSelf && this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDockPositions)
			{
				this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
			}
			else
			{
				this.worldShareableInstance.ResetViews();
				ObjectPools.instance.Destroy(this.worldShareableInstance.gameObject);
			}
			this.worldShareableInstance = null;
		}
		if (!this.IsLocalObject())
		{
			this.OnItemDestroyedOrDisabled();
			base.gameObject.Disable();
			return;
		}
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x000B88FE File Offset: 0x000B6AFE
	public bool IsLocalObject()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x000B8915 File Offset: 0x000B6B15
	public void SetWorldShareableItem(WorldShareableItem item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x06002276 RID: 8822 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x06002277 RID: 8823 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void PlayDestroyedOrDisabledEffect()
	{
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x000B8924 File Offset: 0x000B6B24
	protected virtual void OnItemDestroyedOrDisabled()
	{
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDockPositions)
			{
				this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
			}
			Debug.LogError("Setting WSI to null in OnItemDestroyedOrDisabled", this);
			this.worldShareableInstance = null;
		}
		this.PlayDestroyedOrDisabledEffect();
		this.enabledOnFrame = -1;
		this.currentState = TransferrableObject.PositionState.None;
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x000B899E File Offset: 0x000B6B9E
	public virtual void TriggeredLateUpdate()
	{
		if (this.IsLocalObject() && this.canDrop)
		{
			this.LocalMyObjectValidation();
		}
		if (this.IsMyItem())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	// Token: 0x0600227A RID: 8826 RVA: 0x000B89D2 File Offset: 0x000B6BD2
	protected Transform DefaultAnchor()
	{
		if (this._isDefaultAnchorSet)
		{
			return this._defaultAnchor;
		}
		this._isDefaultAnchorSet = true;
		this._defaultAnchor = ((this.anchor == null) ? base.transform : this.anchor);
		return this._defaultAnchor;
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x000B8A12 File Offset: 0x000B6C12
	private Transform GetAnchor(TransferrableObject.PositionState pos)
	{
		if (this.grabAnchor == null)
		{
			return this.DefaultAnchor();
		}
		if (this.InHand())
		{
			return this.grabAnchor;
		}
		return this.DefaultAnchor();
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000B8A40 File Offset: 0x000B6C40
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x000B8A70 File Offset: 0x000B6C70
	private Transform GetTargetStorageZone(BodyDockPositions.DropPositions state)
	{
		switch (state)
		{
		case BodyDockPositions.DropPositions.None:
			return null;
		case BodyDockPositions.DropPositions.LeftArm:
			return this.targetDockPositions.leftArmTransform;
		case BodyDockPositions.DropPositions.RightArm:
			return this.targetDockPositions.rightArmTransform;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.Chest:
			return this.targetDockPositions.chestTransform;
		case BodyDockPositions.DropPositions.LeftBack:
			return this.targetDockPositions.leftBackTransform;
		default:
			if (state == BodyDockPositions.DropPositions.RightBack)
			{
				return this.targetDockPositions.rightBackTransform;
			}
			break;
		}
		throw new ArgumentOutOfRangeException();
	}

	// Token: 0x0600227E RID: 8830 RVA: 0x000B8AF1 File Offset: 0x000B6CF1
	public static Transform GetTargetDock(TransferrableObject.PositionState state, VRRig rig)
	{
		return TransferrableObject.GetTargetDock(state, rig.myBodyDockPositions, rig.GetComponent<VRRigAnchorOverrides>());
	}

	// Token: 0x0600227F RID: 8831 RVA: 0x000B8B08 File Offset: 0x000B6D08
	public static Transform GetTargetDock(TransferrableObject.PositionState state, BodyDockPositions dockPositions, VRRigAnchorOverrides anchorOverrides)
	{
		if (state <= TransferrableObject.PositionState.InRightHand)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				return anchorOverrides.AnchorOverride(state, dockPositions.leftArmTransform);
			case TransferrableObject.PositionState.OnRightArm:
				return anchorOverrides.AnchorOverride(state, dockPositions.rightArmTransform);
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				return anchorOverrides.AnchorOverride(state, dockPositions.leftHandTransform);
			default:
				if (state == TransferrableObject.PositionState.InRightHand)
				{
					return anchorOverrides.AnchorOverride(state, dockPositions.rightHandTransform);
				}
				break;
			}
		}
		else
		{
			if (state == TransferrableObject.PositionState.OnChest)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.chestTransform);
			}
			if (state == TransferrableObject.PositionState.OnLeftShoulder)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.leftBackTransform);
			}
			if (state == TransferrableObject.PositionState.OnRightShoulder)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.rightBackTransform);
			}
		}
		return null;
	}

	// Token: 0x06002280 RID: 8832 RVA: 0x000B8BAC File Offset: 0x000B6DAC
	private void UpdateFollowXform()
	{
		if (!this.targetRigSet)
		{
			return;
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		try
		{
			transform2 = TransferrableObject.GetTargetDock(this.currentState, this.targetDockPositions, this.anchorOverrides);
		}
		catch
		{
			Debug.LogError("anchorOverrides or targetDock has been destroyed", this);
			this.SetTargetRig(null);
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped && this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		if (this.detatchOnGrab && (this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand))
		{
			base.transform.parent = null;
		}
		if (this.interpState == TransferrableObject.InterpolateState.None)
		{
			try
			{
				if (transform == null)
				{
					return;
				}
				this.startInterpolation |= transform2 != transform.parent;
			}
			catch
			{
			}
			if (!this.startInterpolation && !this.isGrabAnchorSet && base.transform.parent != transform && transform != base.transform)
			{
				this.startInterpolation = true;
			}
			if (this.startInterpolation)
			{
				Vector3 position = base.transform.position;
				Quaternion rotation = base.transform.rotation;
				if (base.transform.parent != transform && transform != base.transform)
				{
					base.transform.parent = transform;
				}
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				if (this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					if (this.flipOnXForLeftHand)
					{
						transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					else if (this.flipOnYForLeftHand)
					{
						transform.localScale = new Vector3(1f, -1f, 1f);
					}
					else
					{
						transform.localScale = Vector3.one;
					}
				}
				else
				{
					transform.localScale = Vector3.one;
				}
				if (Time.frameCount == this.enabledOnFrame || Time.frameCount == this.enabledOnFrame + 1)
				{
					Matrix4x4 matrix4x = this.GetDefaultTransformationMatrix();
					if ((this.currentState != TransferrableObject.PositionState.InLeftHand || !(this.handPoseLeft != null)) && this.currentState == TransferrableObject.PositionState.InRightHand)
					{
						this.handPoseRight != null;
					}
					Matrix4x4 matrix4x2;
					if (this.transferrableItemSlotTransformOverride && this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x2))
					{
						matrix4x = matrix4x2;
					}
					Matrix4x4 matrix4x3 = transform.localToWorldMatrix * matrix4x;
					base.transform.SetLocalToWorldMatrixNoScale(matrix4x3);
					base.transform.localScale = matrix4x3.lossyScale;
				}
				else
				{
					this.interpState = TransferrableObject.InterpolateState.Interpolating;
					if (this.IsMyItem() && this.useGrabType == TransferrableObject.GrabType.Free)
					{
						bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
						if (!flag)
						{
							GameObject rightHand = EquipmentInteractor.instance.rightHand;
						}
						else
						{
							GameObject leftHand = EquipmentInteractor.instance.leftHand;
						}
						Transform targetDock = TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig);
						this.SetupMatrixForFreeGrab(position, rotation, targetDock, flag);
					}
					this.interpDt = this.interpTime;
					this.interpStartRot = rotation;
					this.interpStartPos = position;
					base.transform.position = position;
					base.transform.rotation = rotation;
				}
				this.startInterpolation = false;
			}
		}
		if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
		{
			Matrix4x4 matrix4x4 = this.GetDefaultTransformationMatrix();
			if (this.transferrableItemSlotTransformOverride != null)
			{
				if (this.transferrableItemSlotTransformOverrideCachedMatrix == null)
				{
					Matrix4x4 matrix4x5;
					this.transferrableItemSlotTransformOverrideApplicable = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x5);
					this.transferrableItemSlotTransformOverrideCachedMatrix = new Matrix4x4?(matrix4x5);
				}
				if (this.transferrableItemSlotTransformOverrideApplicable)
				{
					matrix4x4 = this.transferrableItemSlotTransformOverrideCachedMatrix.Value;
				}
			}
			float num = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			Mathf.SmoothStep(0f, 1f, num);
			Matrix4x4 matrix4x6 = transform.localToWorldMatrix * matrix4x4;
			Transform transform3 = base.transform;
			Vector3 vector = matrix4x6.Position();
			transform3.position = (in this.interpStartPos).LerpToUnclamped(in vector, num);
			base.transform.rotation = Quaternion.Slerp(this.interpStartRot, (in matrix4x6).Rotation(), num);
			base.transform.localScale = matrix4x4.lossyScale;
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = TransferrableObject.InterpolateState.None;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				if (this.flipOnXForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				if (this.flipOnYForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(1f, -1f, 1f);
				}
				matrix4x6 = transform.localToWorldMatrix * matrix4x4;
				base.transform.SetLocalToWorldMatrixNoScale(matrix4x6);
				base.transform.localScale = matrix4x4.lossyScale;
			}
		}
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x000B90EC File Offset: 0x000B72EC
	public virtual void DropItem()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
		this.currentState = TransferrableObject.PositionState.Dropped;
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.transferableObjectState = this.currentState;
		}
		if (this.canDrop)
		{
			base.transform.parent = null;
			if (this.anchor)
			{
				this.anchor.parent = this.InitialDockObject;
			}
			if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.isKinematic = true;
			}
		}
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x000B9214 File Offset: 0x000B7414
	protected virtual void OnStateChanged()
	{
		if (this.IsLocalObject() && this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.resetOnDocked)
		{
			int num = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
			if (!this.InHand() && num != 0)
			{
				TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
				if (syncOptions == TransferrableObject.SyncOptions.Bool)
				{
					this.ResetStateBools();
					return;
				}
				if (syncOptions != TransferrableObject.SyncOptions.Int)
				{
					return;
				}
				this.SetItemStateInt(0);
			}
		}
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x000B926C File Offset: 0x000B746C
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		if (this.isSceneObject)
		{
			this.disableItem = false;
		}
		else
		{
			for (int i = 0; i < this.ownerRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.ownerRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
				{
					this.disableItem = false;
					break;
				}
			}
			if (this.disableItem)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		if (this.previousState != this.currentState)
		{
			this.previousState = this.currentState;
			if (!this.Attached())
			{
				base.transform.parent = null;
				if (!this.ShouldBeKinematic() && this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = false;
				}
			}
			if (this.currentState == TransferrableObject.PositionState.None)
			{
				this.ResetToHome();
			}
			this.transferrableItemSlotTransformOverrideCachedMatrix = null;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			this.OnStateChanged();
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			if (!this.canDrop || this.allowReparenting)
			{
				goto IL_015A;
			}
			if (base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			try
			{
				if (this.anchor != null && this.anchor.parent != this.InitialDockObject)
				{
					this.anchor.parent = this.InitialDockObject;
				}
				goto IL_015A;
			}
			catch
			{
				goto IL_015A;
			}
		}
		if (this.currentState != TransferrableObject.PositionState.None)
		{
			this.UpdateFollowXform();
		}
		IL_015A:
		if (this.InHand() && !this.wasHeldShared)
		{
			UnityEvent onHeldShared = this.OnHeldShared;
			if (onHeldShared != null)
			{
				onHeldShared.Invoke();
			}
			this.wasHeldShared = true;
		}
		else if (!this.InHand() && !this.Dropped() && this.wasHeldShared)
		{
			UnityEvent onDockedShared = this.OnDockedShared;
			if (onDockedShared != null)
			{
				onDockedShared.Invoke();
			}
			this.wasHeldShared = false;
		}
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.rigidbodyInstance.isKinematic != this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
			if (this.worldShareableInstance)
			{
				if (this.currentState == TransferrableObject.PositionState.Dropped)
				{
					this.worldShareableInstance.EnableRemoteSync = true;
					return;
				}
				this.worldShareableInstance.EnableRemoteSync = !this.ShouldBeKinematic();
			}
		}
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x000B94A8 File Offset: 0x000B76A8
	public virtual void ResetToHome()
	{
		if (this.isSceneObject)
		{
			this.currentState = TransferrableObject.PositionState.None;
		}
		this.ResetXf();
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x000B94F4 File Offset: 0x000B76F4
	protected void ResetXf()
	{
		if (!this.positionInitialized)
		{
			this.initOffset = base.transform.localPosition;
			this.initRotation = base.transform.localRotation;
		}
		if (this.canDrop || this.allowWorldSharableInstance)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			if (this.ClearLocalPositionOnReset)
			{
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				base.transform.localScale = Vector3.one;
			}
			if (this.InitialDockObject)
			{
				this.anchor.localPosition = Vector3.zero;
				this.anchor.localRotation = Quaternion.identity;
				this.anchor.localScale = Vector3.one;
			}
			if (this.grabAnchor)
			{
				if (this.grabAnchor.parent != base.transform)
				{
					this.grabAnchor.parent = base.transform;
				}
				this.grabAnchor.localPosition = Vector3.zero;
				this.grabAnchor.localRotation = Quaternion.identity;
				this.grabAnchor.localScale = Vector3.one;
			}
			if (this.transferrableItemSlotTransformOverride)
			{
				Transform transformFromPositionState = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState);
				if (transformFromPositionState)
				{
					base.transform.position = transformFromPositionState.position;
					base.transform.rotation = transformFromPositionState.rotation;
					return;
				}
				if (this.anchorOverrides != null)
				{
					Transform transform2 = this.GetAnchor(this.currentState);
					Transform targetDock = TransferrableObject.GetTargetDock(this.currentState, this.targetDockPositions, this.anchorOverrides);
					Matrix4x4 matrix4x = this.GetDefaultTransformationMatrix();
					Matrix4x4 matrix4x2;
					if (this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, targetDock, out matrix4x2))
					{
						matrix4x = matrix4x2;
					}
					Matrix4x4 matrix4x3 = transform2.localToWorldMatrix * matrix4x;
					base.transform.SetLocalToWorldMatrixNoScale(matrix4x3);
					base.transform.localScale = matrix4x3.lossyScale;
					return;
				}
			}
			else
			{
				base.transform.SetLocalMatrixRelativeToParent(this.GetDefaultTransformationMatrix());
			}
		}
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000B9734 File Offset: 0x000B7934
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		this.ResetXf();
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x000B978C File Offset: 0x000B798C
	private void HandleLocalInput()
	{
		Behaviour[] array2;
		if (this.Dropped())
		{
			foreach (GameObject gameObject in this.gameObjectsActiveOnlyWhileHeld)
			{
				if (gameObject.activeSelf)
				{
					gameObject.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			foreach (GameObject gameObject2 in this.gameObjectsActiveOnlyWhileDocked)
			{
				if (gameObject2.activeSelf)
				{
					gameObject2.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			return;
		}
		if (!this.InHand())
		{
			foreach (GameObject gameObject3 in this.gameObjectsActiveOnlyWhileHeld)
			{
				if (gameObject3.activeSelf)
				{
					gameObject3.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			foreach (GameObject gameObject4 in this.gameObjectsActiveOnlyWhileDocked)
			{
				if (!gameObject4.activeSelf)
				{
					gameObject4.SetActive(true);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = true;
			}
			return;
		}
		foreach (GameObject gameObject5 in this.gameObjectsActiveOnlyWhileHeld)
		{
			if (!gameObject5.activeSelf)
			{
				gameObject5.SetActive(true);
			}
		}
		array2 = this.behavioursEnabledOnlyWhileHeld;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = true;
		}
		foreach (GameObject gameObject6 in this.gameObjectsActiveOnlyWhileDocked)
		{
			if (gameObject6.activeSelf)
			{
				gameObject6.SetActive(false);
			}
		}
		array2 = this.behavioursEnabledOnlyWhileDocked;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
		XRNode xrnode = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? XRNode.LeftHand : XRNode.RightHand);
		this.indexTrigger = ControllerInputPoller.TriggerFloat(xrnode);
		bool flag = !this.latched && this.indexTrigger >= this.myThreshold;
		bool flag2 = this.latched && this.indexTrigger < this.myThreshold - this.hysterisis;
		if (flag || this.testActivate)
		{
			this.testActivate = false;
			if (this.CanActivate())
			{
				this.OnActivate();
				return;
			}
		}
		else if (flag2 || this.testDeactivate)
		{
			this.testDeactivate = false;
			if (this.CanDeactivate())
			{
				this.OnDeactivate();
			}
		}
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void LocalMyObjectValidation()
	{
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x000B9A14 File Offset: 0x000B7C14
	protected virtual void LocalPersistanceValidation()
	{
		if (this.maxDistanceFromOriginBeforeRespawn != 0f && Vector3.Distance(base.transform.position, this.originPoint.position) > this.maxDistanceFromOriginBeforeRespawn)
		{
			if (this.audioSrc != null && this.resetPositionAudioClip != null)
			{
				this.audioSrc.GTPlayOneShot(this.resetPositionAudioClip, 1f);
			}
			if (this.currentState != TransferrableObject.PositionState.Dropped)
			{
				this.DropItem();
				this.currentState = TransferrableObject.PositionState.Dropped;
			}
			base.transform.position = this.originPoint.position;
			if (!this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.linearVelocity = Vector3.zero;
			}
		}
		if (this.rigidbodyInstance && this.rigidbodyInstance.linearVelocity.sqrMagnitude > 10000f)
		{
			Debug.Log("Moving too fast, Assuming ive fallen out of the map. Ressetting position", this);
			this.ResetToHome();
		}
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x000B9B14 File Offset: 0x000B7D14
	public void ObjectBeingTaken()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x000B9BB4 File Offset: 0x000B7DB4
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		this.LocalPersistanceValidation();
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.isSceneObject && this.IsLocalObject())
			{
				this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
				this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
				this.myRig.SetTransferrableDockPosition(this.objectIndex, this.storedZone);
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.transferableObjectState = this.currentState;
				this.worldShareableInstance.transferableObjectItemState = this.itemState;
			}
		}
		this.HandleLocalInput();
		if (this.InHand() && !this.wasHeldLocal)
		{
			UnityEvent onHeldLocal = this.OnHeldLocal;
			if (onHeldLocal != null)
			{
				onHeldLocal.Invoke();
			}
			this.wasHeldLocal = true;
			return;
		}
		if (!this.InHand() && !this.Dropped() && this.wasHeldLocal)
		{
			UnityEvent onDockedLocal = this.OnDockedLocal;
			if (onDockedLocal != null)
			{
				onDockedLocal.Invoke();
			}
			this.wasHeldLocal = false;
		}
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000B9CCC File Offset: 0x000B7ECC
	protected void LateUpdateReplicatedSceneObject()
	{
		if (this.myOnlineRig != null)
		{
			this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		}
		if (this.worldShareableInstance != null)
		{
			this.currentState = this.worldShareableInstance.transferableObjectState;
			this.itemState = this.worldShareableInstance.transferableObjectItemState;
			this.worldShareableInstance.EnableRemoteSync = !this.ShouldBeKinematic() || this.currentState == TransferrableObject.PositionState.Dropped;
		}
		if (this.isRigidbodySet && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000B9D70 File Offset: 0x000B7F70
	protected virtual void LateUpdateReplicated()
	{
		if (this.isSceneObject || this.shareable)
		{
			this.LateUpdateReplicatedSceneObject();
			return;
		}
		if (this.myOnlineRig == null)
		{
			return;
		}
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (!this.ValidateState(this.currentState))
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.Disable();
			}
			this.currentState = this.previousState;
		}
		if (this.isRigidbodySet)
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
		}
		bool flag = true;
		this.previousItemState = this.itemState;
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		int num = this.myOnlineRig.ActiveTransferrableObjectIndexLength();
		for (int i = 0; i < num; i++)
		{
			if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				flag = false;
				foreach (GameObject gameObject in this.gameObjectsActiveOnlyWhileHeld)
				{
					bool flag2 = this.InHand();
					if (gameObject.activeSelf != flag2)
					{
						gameObject.SetActive(flag2);
					}
				}
				Behaviour[] array2 = this.behavioursEnabledOnlyWhileHeld;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = this.InHand();
				}
				foreach (GameObject gameObject2 in this.gameObjectsActiveOnlyWhileDocked)
				{
					bool flag3 = this.InHand();
					if (gameObject2.activeSelf == flag3)
					{
						gameObject2.SetActive(!flag3);
					}
				}
				array2 = this.behavioursEnabledOnlyWhileDocked;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = !this.InHand();
				}
			}
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.previousItemState != this.itemState)
		{
			int num2 = (int)(this.previousItemState & (TransferrableObject.ItemStates)(-65));
			int num3 = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
			if (num2 != num3)
			{
				this.OnNetworkItemStateChanged(num3);
			}
		}
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000B9F84 File Offset: 0x000B8184
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		if (!this.IsLocalObject() && this.worldShareableInstance && !this.isSceneObject)
		{
			if (this.IsMyItem())
			{
				return;
			}
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
			{
			});
		}
		this.ResetXf();
		TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
		if (syncOptions == TransferrableObject.SyncOptions.Bool)
		{
			this.ResetStateBools();
			return;
		}
		if (syncOptions != TransferrableObject.SyncOptions.Int)
		{
			return;
		}
		this.SetItemStateInt(0);
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000BA028 File Offset: 0x000B8228
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.worldShareableInstance == null) && !this.worldShareableInstance.guard.isTrulyMine)
		{
			if (!this.IsGrabbable())
			{
				return;
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
		if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState != TransferrableObject.PositionState.OnLeftArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabLeft = false;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.startInterpolation = true;
			}
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InLeftHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InLeftHand, EquipmentInteractor.instance.leftHand.transform, TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, true);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabRight = false;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.startInterpolation = true;
			}
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InRightHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InRightHand, EquipmentInteractor.instance.rightHand.transform, TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, false);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		if (this.rigidbodyInstance && !this.rigidbodyInstance.isKinematic && this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		PlayerGameEvents.GrabbedObject(this.interactEventName);
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x000BA280 File Offset: 0x000B8480
	private void SetupMatrixForFreeGrab(Vector3 worldPosition, Quaternion worldRotation, Transform attachPoint, bool leftHand)
	{
		Quaternion rotation = attachPoint.transform.rotation;
		Vector3 position = attachPoint.transform.position;
		Quaternion quaternion = Quaternion.Inverse(rotation) * worldRotation;
		Vector3 vector = Quaternion.Inverse(rotation) * (worldPosition - position);
		this.OnHandMatrixUpdate(vector, quaternion, leftHand);
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x000BA2D3 File Offset: 0x000B84D3
	protected void SetupHandMatrix(Vector3 leftHandPos, Quaternion leftHandRot, Vector3 rightHandPos, Quaternion rightHandRot)
	{
		this.leftHandMatrix = Matrix4x4.TRS(leftHandPos, leftHandRot, Vector3.one);
		this.rightHandMatrix = Matrix4x4.TRS(rightHandPos, rightHandRot, Vector3.one);
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
	{
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x000BA2FC File Offset: 0x000B84FC
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.IsMyItem())
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (!this.IsHeld())
		{
			return false;
		}
		if (releasingHand == EquipmentInteractor.instance.leftHand)
		{
			this.canAutoGrabLeft = true;
		}
		else
		{
			this.canAutoGrabRight = true;
		}
		if (zoneReleased != null)
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.LeftArm;
			bool flag2 = this.currentState == TransferrableObject.PositionState.InRightHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.RightArm;
			if (flag || flag2)
			{
				return false;
			}
			if (this.targetDockPositions.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == this.targetDockPositions && (zoneReleased.dropPosition & this.dockPositions) != BodyDockPositions.DropPositions.None)
			{
				this.storedZone = zoneReleased.dropPosition;
			}
		}
		bool flag3 = false;
		this.interpState = TransferrableObject.InterpolateState.None;
		if (this.isSceneObject || this.canDrop || this.allowWorldSharableInstance)
		{
			if (!this.rigidbodyInstance)
			{
				return false;
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.EnableRemoteSync = true;
			}
			if (!flag3)
			{
				this.currentState = TransferrableObject.PositionState.Dropped;
			}
			if (this.rigidbodyInstance.isKinematic && !this.ShouldBeKinematic())
			{
				this.rigidbodyInstance.isKinematic = false;
			}
			GorillaVelocityEstimator component = base.GetComponent<GorillaVelocityEstimator>();
			if (component != null && this.rigidbodyInstance != null)
			{
				this.rigidbodyInstance.linearVelocity = component.linearVelocity;
				this.rigidbodyInstance.angularVelocity = component.angularVelocity;
			}
		}
		else
		{
			bool flag4 = this.allowWorldSharableInstance;
		}
		this.DropItemCleanup();
		EquipmentInteractor.instance.ForceDropEquipment(this);
		PlayerGameEvents.DroppedObject(this.interactEventName);
		return true;
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x000BA4B0 File Offset: 0x000B86B0
	public override void DropItemCleanup()
	{
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		BodyDockPositions.DropPositions dropPositions = this.storedZone;
		switch (dropPositions)
		{
		case BodyDockPositions.DropPositions.LeftArm:
			this.currentState = TransferrableObject.PositionState.OnLeftArm;
			return;
		case BodyDockPositions.DropPositions.RightArm:
			this.currentState = TransferrableObject.PositionState.OnRightArm;
			return;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
			break;
		case BodyDockPositions.DropPositions.Chest:
			this.currentState = TransferrableObject.PositionState.OnChest;
			return;
		default:
			if (dropPositions == BodyDockPositions.DropPositions.LeftBack)
			{
				this.currentState = TransferrableObject.PositionState.OnLeftShoulder;
				return;
			}
			if (dropPositions != BodyDockPositions.DropPositions.RightBack)
			{
				return;
			}
			this.currentState = TransferrableObject.PositionState.OnRightShoulder;
			break;
		}
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x000BA520 File Offset: 0x000B8720
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsGrabbable())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000BA584 File Offset: 0x000B8784
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		if (this.myRig.netView != null)
		{
			this.myRig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { soundIndex, flag, 0.1f });
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000BA605 File Offset: 0x000B8805
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x00023F0C File Offset: 0x0002210C
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x00023F0C File Offset: 0x0002210C
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x0600229B RID: 8859 RVA: 0x000BA617 File Offset: 0x000B8817
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x000BA620 File Offset: 0x000B8820
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000BA629 File Offset: 0x000B8829
	public virtual bool IsMyItem()
	{
		return GorillaTagger.Instance == null || (this.targetRig != null && this.targetRig == GorillaTagger.Instance.offlineVRRig);
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000BA653 File Offset: 0x000B8853
	protected virtual bool IsHeld()
	{
		return EquipmentInteractor.instance != null && (EquipmentInteractor.instance.leftHandHeldEquipment == this || EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000BA680 File Offset: 0x000B8880
	public virtual bool IsGrabbable()
	{
		return this.IsMyItem() || ((this.isSceneObject || this.shareable) && (this.isSceneObject || this.shareable) && (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None));
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000BA6DF File Offset: 0x000B88DF
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000BA6F5 File Offset: 0x000B88F5
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000BA704 File Offset: 0x000B8904
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x000BA70F File Offset: 0x000B890F
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000BA71A File Offset: 0x000B891A
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000BA726 File Offset: 0x000B8926
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000BA73E File Offset: 0x000B893E
	protected NetPlayer OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.netView.Owner;
		}
		return NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x000BA76C File Offset: 0x000B896C
	public bool ValidateState(TransferrableObject.PositionState state)
	{
		if (state <= TransferrableObject.PositionState.OnChest)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				if ((this.dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None)
				{
					return true;
				}
				return false;
			case TransferrableObject.PositionState.OnRightArm:
				if ((this.dockPositions & BodyDockPositions.DropPositions.RightArm) != BodyDockPositions.DropPositions.None)
				{
					return true;
				}
				return false;
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				return false;
			case TransferrableObject.PositionState.InLeftHand:
				break;
			default:
				if (state != TransferrableObject.PositionState.InRightHand)
				{
					if (state != TransferrableObject.PositionState.OnChest)
					{
						return false;
					}
					if ((this.dockPositions & BodyDockPositions.DropPositions.Chest) != BodyDockPositions.DropPositions.None)
					{
						return true;
					}
					return false;
				}
				break;
			}
			return true;
		}
		if (state != TransferrableObject.PositionState.OnLeftShoulder)
		{
			if (state != TransferrableObject.PositionState.OnRightShoulder)
			{
				if (state == TransferrableObject.PositionState.Dropped)
				{
					return this.canDrop || this.shareable;
				}
			}
			else if ((this.dockPositions & BodyDockPositions.DropPositions.RightBack) != BodyDockPositions.DropPositions.None)
			{
				return true;
			}
		}
		else if ((this.dockPositions & BodyDockPositions.DropPositions.LeftBack) != BodyDockPositions.DropPositions.None)
		{
			return true;
		}
		return false;
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x000BA80C File Offset: 0x000B8A0C
	private void OnNetworkItemStateChanged(int stateBits)
	{
		TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
		if (syncOptions != TransferrableObject.SyncOptions.Bool)
		{
			if (syncOptions != TransferrableObject.SyncOptions.Int)
			{
				return;
			}
			UnityEvent<int> onItemStateIntChanged = this.OnItemStateIntChanged;
			if (onItemStateIntChanged == null)
			{
				return;
			}
			onItemStateIntChanged.Invoke(stateBits);
		}
		else
		{
			int num = (int)(this.previousItemState & TransferrableObject.ItemStates.State0);
			int num2 = (int)(this.itemState & TransferrableObject.ItemStates.State0);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolFalse = this.OnItemStateBoolFalse;
				if (onItemStateBoolFalse != null)
				{
					onItemStateBoolFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolTrue = this.OnItemStateBoolTrue;
				if (onItemStateBoolTrue != null)
				{
					onItemStateBoolTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State1);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State1);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolBFalse = this.OnItemStateBoolBFalse;
				if (onItemStateBoolBFalse != null)
				{
					onItemStateBoolBFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolBTrue = this.OnItemStateBoolBTrue;
				if (onItemStateBoolBTrue != null)
				{
					onItemStateBoolBTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State2);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State2);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolCFalse = this.OnItemStateBoolCFalse;
				if (onItemStateBoolCFalse != null)
				{
					onItemStateBoolCFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolCTrue = this.OnItemStateBoolCTrue;
				if (onItemStateBoolCTrue != null)
				{
					onItemStateBoolCTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State3);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State3);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolDFalse = this.OnItemStateBoolDFalse;
				if (onItemStateBoolDFalse == null)
				{
					return;
				}
				onItemStateBoolDFalse.Invoke();
				return;
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolDTrue = this.OnItemStateBoolDTrue;
				if (onItemStateBoolDTrue == null)
				{
					return;
				}
				onItemStateBoolDTrue.Invoke();
				return;
			}
		}
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x000BA93F File Offset: 0x000B8B3F
	public void ToggleNetworkedItemStateBool()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(1);
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x000BA952 File Offset: 0x000B8B52
	public void ToggleNetworkedItemStateBoolB()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(2);
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x000BA965 File Offset: 0x000B8B65
	public void ToggleNetworkedItemStateBoolC()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(4);
	}

	// Token: 0x060022AC RID: 8876 RVA: 0x000BA978 File Offset: 0x000B8B78
	public void ToggleNetworkedItemStateBoolD()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(8);
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x000BA98C File Offset: 0x000B8B8C
	protected void ResetStateBools()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		if (!this.IsLocalObject())
		{
			return;
		}
		int num = 15;
		this.SetStateBit(false, num);
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000BA9B7 File Offset: 0x000B8BB7
	public void SetItemStateBool(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 1);
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000BA9CB File Offset: 0x000B8BCB
	public void SetItemStateBoolB(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 2);
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000BA9DF File Offset: 0x000B8BDF
	public void SetItemStateBoolC(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 4);
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000BA9F3 File Offset: 0x000B8BF3
	public void SetItemStateBoolD(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 8);
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000BAA08 File Offset: 0x000B8C08
	private void SetStateBit(bool value, int bitmask)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		int num = (int)this.itemState;
		if (value)
		{
			num |= bitmask;
		}
		else
		{
			num &= ~bitmask;
		}
		TransferrableObject.ItemStates itemStates = (TransferrableObject.ItemStates)num;
		if (this.itemState != itemStates)
		{
			this.previousItemState = this.itemState;
			this.itemState = itemStates;
			this.OnNetworkItemStateChanged(num);
		}
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x000BAA58 File Offset: 0x000B8C58
	private void ToggleStateBit(int bitmask)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		bool flag = (this.itemState & (TransferrableObject.ItemStates)bitmask) != (TransferrableObject.ItemStates)0;
		int num = (int)this.itemState;
		if (!flag)
		{
			num |= bitmask;
		}
		else
		{
			num &= ~bitmask;
		}
		this.previousItemState = this.itemState;
		this.itemState = (TransferrableObject.ItemStates)num;
		this.OnNetworkItemStateChanged(num);
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000BAAA4 File Offset: 0x000B8CA4
	public void SetItemStateInt(int newState)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Int)
		{
			return;
		}
		newState = Mathf.Clamp(newState, 0, 63);
		int num = newState & -65;
		int num2 = (int)(this.itemState & TransferrableObject.ItemStates.Part0Held);
		TransferrableObject.ItemStates itemStates = (TransferrableObject.ItemStates)(num | num2);
		if (this.itemState != itemStates)
		{
			this.previousItemState = this.itemState;
			this.itemState = itemStates;
			this.OnNetworkItemStateChanged(num);
		}
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x000BAB08 File Offset: 0x000B8D08
	public virtual void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer != null && toPlayer.Equals(fromPlayer))
		{
			return;
		}
		if (object.Equals(fromPlayer, NetworkSystem.Instance.LocalPlayer) && this.IsHeld())
		{
			this.DropItem();
		}
		if (toPlayer == null)
		{
			this.SetTargetRig(null);
			return;
		}
		this.rigidbodyInstance.useGravity = this.shouldUseGravity && object.Equals(toPlayer, NetworkSystem.Instance.LocalPlayer);
		if (!this.shareable && !this.isSceneObject)
		{
			return;
		}
		if (object.Equals(toPlayer, NetworkSystem.Instance.LocalPlayer))
		{
			if (GorillaTagger.Instance == null)
			{
				Debug.LogError("OnOwnershipTransferred has been initiated too quickly, The local player is not ready");
				return;
			}
			this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
			return;
		}
		else
		{
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(toPlayer);
			if (!vrrig)
			{
				Debug.LogError("failed to find target rig for ownershiptransfer");
				return;
			}
			this.SetTargetRig(vrrig);
			return;
		}
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x000BABE0 File Offset: 0x000B8DE0
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return false;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.isSceneObject)
		{
			return false;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000BAC98 File Offset: 0x000B8E98
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return true;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000BAD3C File Offset: 0x000B8F3C
	public void OnMyOwnerLeft()
	{
		if (this.currentState == TransferrableObject.PositionState.None || this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		this.DropItem();
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			this.anchor.localPosition = Vector3.zero;
			this.anchor.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000BADA3 File Offset: 0x000B8FA3
	public void OnMyCreatorLeft()
	{
		this.OnItemDestroyedOrDisabled();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000BADB8 File Offset: 0x000B8FB8
	public bool BuildValidationCheck()
	{
		int num = 0;
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.LeftArm))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.RightArm))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.Chest))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.LeftBack))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.RightBack))
		{
			num++;
		}
		if (num > 1)
		{
			Debug.LogError("transferrableitem is starting with multiple storedzones: " + base.transform.parent.name, base.gameObject);
			return false;
		}
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if ((GTPlayer.LocomotionEnabledLayers & (1 << componentsInChildren[i].gameObject.layer)) != 0)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Holdable cosmetic ",
					base.transform.name,
					" has a collider on a player movement layer! Players will fly around! Dear god, please fix! It's on the ",
					componentsInChildren[i].name,
					" collider"
				}), base.gameObject);
				return false;
			}
		}
		return true;
	}

	// Token: 0x04002D4D RID: 11597
	private VRRig _myRig;

	// Token: 0x04002D4F RID: 11599
	private VRRig _myOnlineRig;

	// Token: 0x04002D51 RID: 11601
	public bool latched;

	// Token: 0x04002D52 RID: 11602
	private float indexTrigger;

	// Token: 0x04002D53 RID: 11603
	public bool testActivate;

	// Token: 0x04002D54 RID: 11604
	public bool testDeactivate;

	// Token: 0x04002D55 RID: 11605
	[Tooltip("When the grip/trigger input is greater than this value the transferrable object is activated")]
	public float myThreshold = 0.8f;

	// Token: 0x04002D56 RID: 11606
	[Tooltip("When the grip/trigger input is less than (myThreshold - hysterisis) the transferrable object is deactivated")]
	public float hysterisis = 0.05f;

	// Token: 0x04002D57 RID: 11607
	[Tooltip("Set the x scale to -1 when held in left hand")]
	public bool flipOnXForLeftHand;

	// Token: 0x04002D58 RID: 11608
	[Tooltip("Set the y scale to -1 when held in left hand")]
	public bool flipOnYForLeftHand;

	// Token: 0x04002D59 RID: 11609
	[Tooltip("Set the x scale to -1 when docked on left arm")]
	public bool flipOnXForLeftArm;

	// Token: 0x04002D5A RID: 11610
	[Tooltip("disable grabbing the item from out of your other hand")]
	public bool disableStealing;

	// Token: 0x04002D5B RID: 11611
	[Tooltip("Allow other players to pick up this item")]
	public bool allowPlayerStealing;

	// Token: 0x04002D5C RID: 11612
	private TransferrableObject.PositionState initState;

	// Token: 0x04002D5D RID: 11613
	public TransferrableObject.ItemStates itemState;

	// Token: 0x04002D5E RID: 11614
	protected TransferrableObject.ItemStates previousItemState;

	// Token: 0x04002D5F RID: 11615
	protected const int HELD_BIT_MASK = 64;

	// Token: 0x04002D60 RID: 11616
	private const int BOOL_A_BITMASK = 1;

	// Token: 0x04002D61 RID: 11617
	private const int BOOL_B_BITMASK = 2;

	// Token: 0x04002D62 RID: 11618
	private const int BOOL_C_BITMASK = 4;

	// Token: 0x04002D63 RID: 11619
	private const int BOOL_D_BITMASK = 8;

	// Token: 0x04002D64 RID: 11620
	[DevInspectorShow]
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x04002D65 RID: 11621
	protected TransferrableObject.PositionState previousState;

	// Token: 0x04002D66 RID: 11622
	[DevInspectorYellow]
	[DevInspectorShow]
	public TransferrableObject.PositionState currentState;

	// Token: 0x04002D67 RID: 11623
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x04002D68 RID: 11624
	[DevInspectorCyan]
	[DevInspectorShow]
	public AdvancedItemState advancedGrabState;

	// Token: 0x04002D69 RID: 11625
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig targetRig;

	// Token: 0x04002D6A RID: 11626
	[HideInInspector]
	public bool targetRigSet;

	// Token: 0x04002D6B RID: 11627
	public TransferrableObject.GrabType useGrabType;

	// Token: 0x04002D6C RID: 11628
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig ownerRig;

	// Token: 0x04002D6D RID: 11629
	[DebugReadout]
	[NonSerialized]
	public BodyDockPositions targetDockPositions;

	// Token: 0x04002D6E RID: 11630
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04002D6F RID: 11631
	public bool canAutoGrabLeft;

	// Token: 0x04002D70 RID: 11632
	public bool canAutoGrabRight;

	// Token: 0x04002D71 RID: 11633
	[DevInspectorShow]
	public int objectIndex;

	// Token: 0x04002D72 RID: 11634
	[NonSerialized]
	public Transform anchor;

	// Token: 0x04002D73 RID: 11635
	[Tooltip("In Functional prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x04002D74 RID: 11636
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x04002D75 RID: 11637
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Left mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseLeft;

	// Token: 0x04002D76 RID: 11638
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Right mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseRight;

	// Token: 0x04002D77 RID: 11639
	[HideInInspector]
	public bool isGrabAnchorSet;

	// Token: 0x04002D78 RID: 11640
	private static Vector3 handPoseRightReferencePoint = new Vector3(-0.0141f, 0.0065f, -0.278f);

	// Token: 0x04002D79 RID: 11641
	private static Quaternion handPoseRightReferenceRotation = Quaternion.Euler(-2.058f, -17.2f, 65.05f);

	// Token: 0x04002D7A RID: 11642
	private static Vector3 handPoseLeftReferencePoint = new Vector3(0.0136f, 0.0045f, -0.2809f);

	// Token: 0x04002D7B RID: 11643
	private static Quaternion handPoseLeftReferenceRotation = Quaternion.Euler(-0.58f, 21.356f, -63.965f);

	// Token: 0x04002D7C RID: 11644
	public TransferrableItemSlotTransformOverride transferrableItemSlotTransformOverride;

	// Token: 0x04002D7D RID: 11645
	public int myIndex;

	// Token: 0x04002D7E RID: 11646
	[Tooltip("(Optional) objects to enable when held in hand and disable when not in hand")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x04002D7F RID: 11647
	[Tooltip("(Optional) objects to disable when held in hand and enable when not in hand")]
	public GameObject[] gameObjectsActiveOnlyWhileDocked;

	// Token: 0x04002D80 RID: 11648
	[Tooltip("(Optional) components to enable when held in hand and disable when not in hand")]
	public Behaviour[] behavioursEnabledOnlyWhileHeld;

	// Token: 0x04002D81 RID: 11649
	[Tooltip("(Optional) components to disable when held in hand and enable when not in hand")]
	public Behaviour[] behavioursEnabledOnlyWhileDocked;

	// Token: 0x04002D82 RID: 11650
	[SerializeField]
	protected internal WorldShareableItem worldShareableInstance;

	// Token: 0x04002D83 RID: 11651
	private float interpTime = 0.2f;

	// Token: 0x04002D84 RID: 11652
	private float interpDt;

	// Token: 0x04002D85 RID: 11653
	private Vector3 interpStartPos;

	// Token: 0x04002D86 RID: 11654
	private Quaternion interpStartRot;

	// Token: 0x04002D87 RID: 11655
	protected int enabledOnFrame = -1;

	// Token: 0x04002D88 RID: 11656
	protected Vector3 initOffset;

	// Token: 0x04002D89 RID: 11657
	protected Quaternion initRotation;

	// Token: 0x04002D8A RID: 11658
	private Matrix4x4 initMatrix = Matrix4x4.identity;

	// Token: 0x04002D8B RID: 11659
	private Matrix4x4 leftHandMatrix = Matrix4x4.identity;

	// Token: 0x04002D8C RID: 11660
	private Matrix4x4 rightHandMatrix = Matrix4x4.identity;

	// Token: 0x04002D8D RID: 11661
	private bool positionInitialized;

	// Token: 0x04002D8E RID: 11662
	public bool isSceneObject;

	// Token: 0x04002D8F RID: 11663
	public Rigidbody rigidbodyInstance;

	// Token: 0x04002D92 RID: 11666
	public bool canDrop;

	// Token: 0x04002D93 RID: 11667
	[Tooltip("completely drop the item instead of auto-returning to a stored zone")]
	public bool allowReparenting;

	// Token: 0x04002D94 RID: 11668
	[Tooltip("(Scene object) has a worldSharableInstance")]
	public bool shareable;

	// Token: 0x04002D95 RID: 11669
	[Tooltip("(Balloon) Unparent this object from the rig when grabbed")]
	public bool detatchOnGrab;

	// Token: 0x04002D96 RID: 11670
	[Tooltip("(Balloon) is this cosmetic droppable in the world")]
	public bool allowWorldSharableInstance;

	// Token: 0x04002D97 RID: 11671
	[ItemCanBeNull]
	public Transform originPoint;

	// Token: 0x04002D98 RID: 11672
	[ItemCanBeNull]
	public float maxDistanceFromOriginBeforeRespawn;

	// Token: 0x04002D99 RID: 11673
	public AudioClip resetPositionAudioClip;

	// Token: 0x04002D9A RID: 11674
	public float maxDistanceFromTargetPlayerBeforeRespawn;

	// Token: 0x04002D9B RID: 11675
	private bool wasHover;

	// Token: 0x04002D9C RID: 11676
	private bool isHover;

	// Token: 0x04002D9D RID: 11677
	private bool disableItem;

	// Token: 0x04002D9E RID: 11678
	protected bool loaded;

	// Token: 0x04002D9F RID: 11679
	public bool ClearLocalPositionOnReset;

	// Token: 0x04002DA0 RID: 11680
	[SerializeField]
	protected TransferrableObject.SyncOptions networkedStateEvents;

	// Token: 0x04002DA1 RID: 11681
	[SerializeField]
	protected bool resetOnDocked = true;

	// Token: 0x04002DA2 RID: 11682
	[SerializeField]
	protected string boolADebugName;

	// Token: 0x04002DA3 RID: 11683
	[SerializeField]
	protected UnityEvent OnItemStateBoolTrue;

	// Token: 0x04002DA4 RID: 11684
	[SerializeField]
	protected UnityEvent OnItemStateBoolFalse;

	// Token: 0x04002DA5 RID: 11685
	[SerializeField]
	protected string boolBDebugName;

	// Token: 0x04002DA6 RID: 11686
	[SerializeField]
	protected UnityEvent OnItemStateBoolBTrue;

	// Token: 0x04002DA7 RID: 11687
	[SerializeField]
	protected UnityEvent OnItemStateBoolBFalse;

	// Token: 0x04002DA8 RID: 11688
	[SerializeField]
	protected string boolCDebugName;

	// Token: 0x04002DA9 RID: 11689
	[SerializeField]
	protected UnityEvent OnItemStateBoolCTrue;

	// Token: 0x04002DAA RID: 11690
	[SerializeField]
	protected UnityEvent OnItemStateBoolCFalse;

	// Token: 0x04002DAB RID: 11691
	[SerializeField]
	protected string boolDDebugName;

	// Token: 0x04002DAC RID: 11692
	[SerializeField]
	protected UnityEvent OnItemStateBoolDTrue;

	// Token: 0x04002DAD RID: 11693
	[SerializeField]
	protected UnityEvent OnItemStateBoolDFalse;

	// Token: 0x04002DAE RID: 11694
	[SerializeField]
	protected UnityEvent<int> OnItemStateIntChanged;

	// Token: 0x04002DAF RID: 11695
	[FormerlySerializedAs("OnUndocked")]
	[SerializeField]
	private UnityEvent OnHeldLocal;

	// Token: 0x04002DB0 RID: 11696
	[SerializeField]
	private UnityEvent OnHeldShared;

	// Token: 0x04002DB1 RID: 11697
	[FormerlySerializedAs("OnDocked")]
	[SerializeField]
	private UnityEvent OnDockedLocal;

	// Token: 0x04002DB2 RID: 11698
	[FormerlySerializedAs("OnDockedLocal")]
	[SerializeField]
	private UnityEvent OnDockedShared;

	// Token: 0x04002DB3 RID: 11699
	private bool wasHeldLocal;

	// Token: 0x04002DB4 RID: 11700
	private bool wasHeldShared;

	// Token: 0x04002DB5 RID: 11701
	[Tooltip("(Optional) name broadcast by PlayerGameEvents")]
	public string interactEventName;

	// Token: 0x04002DB6 RID: 11702
	public const int kPositionStateCount = 8;

	// Token: 0x04002DB7 RID: 11703
	[DevInspectorShow]
	public TransferrableObject.InterpolateState interpState;

	// Token: 0x04002DB8 RID: 11704
	public bool startInterpolation;

	// Token: 0x04002DB9 RID: 11705
	public Transform InitialDockObject;

	// Token: 0x04002DBA RID: 11706
	private AudioSource audioSrc;

	// Token: 0x04002DBD RID: 11709
	protected Transform _defaultAnchor;

	// Token: 0x04002DBE RID: 11710
	protected bool _isDefaultAnchorSet;

	// Token: 0x04002DBF RID: 11711
	private Matrix4x4? transferrableItemSlotTransformOverrideCachedMatrix;

	// Token: 0x04002DC0 RID: 11712
	private bool transferrableItemSlotTransformOverrideApplicable;

	// Token: 0x0200054F RID: 1359
	public enum SyncOptions
	{
		// Token: 0x04002DC2 RID: 11714
		None,
		// Token: 0x04002DC3 RID: 11715
		Bool,
		// Token: 0x04002DC4 RID: 11716
		Int
	}

	// Token: 0x02000550 RID: 1360
	public enum ItemStates
	{
		// Token: 0x04002DC6 RID: 11718
		State0 = 1,
		// Token: 0x04002DC7 RID: 11719
		State1,
		// Token: 0x04002DC8 RID: 11720
		State2 = 4,
		// Token: 0x04002DC9 RID: 11721
		State3 = 8,
		// Token: 0x04002DCA RID: 11722
		State4 = 16,
		// Token: 0x04002DCB RID: 11723
		State5 = 32,
		// Token: 0x04002DCC RID: 11724
		Part0Held = 64,
		// Token: 0x04002DCD RID: 11725
		Part1Held = 128
	}

	// Token: 0x02000551 RID: 1361
	public enum GrabType
	{
		// Token: 0x04002DCF RID: 11727
		Default,
		// Token: 0x04002DD0 RID: 11728
		Free
	}

	// Token: 0x02000552 RID: 1362
	[Flags]
	public enum PositionState
	{
		// Token: 0x04002DD2 RID: 11730
		OnLeftArm = 1,
		// Token: 0x04002DD3 RID: 11731
		OnRightArm = 2,
		// Token: 0x04002DD4 RID: 11732
		InLeftHand = 4,
		// Token: 0x04002DD5 RID: 11733
		InRightHand = 8,
		// Token: 0x04002DD6 RID: 11734
		OnChest = 16,
		// Token: 0x04002DD7 RID: 11735
		OnLeftShoulder = 32,
		// Token: 0x04002DD8 RID: 11736
		OnRightShoulder = 64,
		// Token: 0x04002DD9 RID: 11737
		Dropped = 128,
		// Token: 0x04002DDA RID: 11738
		None = 0
	}

	// Token: 0x02000553 RID: 1363
	public enum InterpolateState
	{
		// Token: 0x04002DDC RID: 11740
		None,
		// Token: 0x04002DDD RID: 11741
		Interpolating
	}
}
