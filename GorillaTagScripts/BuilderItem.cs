using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F58 RID: 3928
	public class BuilderItem : TransferrableObject
	{
		// Token: 0x06006097 RID: 24727 RVA: 0x001EA564 File Offset: 0x001E8764
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06006098 RID: 24728 RVA: 0x001EA584 File Offset: 0x001E8784
		protected override void Awake()
		{
			base.Awake();
			this.parent = base.transform.parent;
			this.currTable = null;
			this.initialPosition = base.transform.position;
			this.initialRotation = base.transform.rotation;
			this.initialGrabInteractorScale = this.gripInteractor.transform.localScale;
		}

		// Token: 0x06006099 RID: 24729 RVA: 0x000B1488 File Offset: 0x000AF688
		internal override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x0600609A RID: 24730 RVA: 0x0004C208 File Offset: 0x0004A408
		internal override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x0600609B RID: 24731 RVA: 0x001EA5E7 File Offset: 0x001E87E7
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x0600609C RID: 24732 RVA: 0x001EA604 File Offset: 0x001E8804
		public void AttachPiece(BuilderPiece piece)
		{
			base.transform.SetPositionAndRotation(piece.transform.position, piece.transform.rotation);
			piece.transform.localScale = Vector3.one;
			piece.transform.SetParent(this.itemRoot.transform);
			Debug.LogFormat(piece.gameObject, "Attach Piece {0} to container {1}", new object[]
			{
				piece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = piece;
		}

		// Token: 0x0600609D RID: 24733 RVA: 0x001EA69C File Offset: 0x001E889C
		public void DetachPiece(BuilderPiece piece)
		{
			if (piece != this.attachedPiece)
			{
				Debug.LogErrorFormat("Trying to detach piece {0} from a container containing {1}", new object[]
				{
					piece.pieceId,
					this.attachedPiece.pieceId
				});
				return;
			}
			piece.transform.SetParent(null);
			Debug.LogFormat(this.attachedPiece.gameObject, "Detach Piece {0} from container {1}", new object[]
			{
				this.attachedPiece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = null;
		}

		// Token: 0x0600609E RID: 24734 RVA: 0x001EA744 File Offset: 0x001E8944
		private new void OnStateChanged()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				this.enableCollidersWhenReady = true;
				this.gripInteractor.transform.localScale = this.initialGrabInteractorScale * 2f;
				this.handsFreeOfCollidersTime = 0f;
				return;
			}
			this.enableCollidersWhenReady = false;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			this.handsFreeOfCollidersTime = 0f;
		}

		// Token: 0x0600609F RID: 24735 RVA: 0x001EA7B8 File Offset: 0x001E89B8
		public override Matrix4x4 GetDefaultTransformationMatrix()
		{
			if (this.reliableState.dirty)
			{
				base.SetupHandMatrix(this.reliableState.leftHandAttachPos, this.reliableState.leftHandAttachRot, this.reliableState.rightHandAttachPos, this.reliableState.rightHandAttachRot);
				this.reliableState.dirty = false;
			}
			return base.GetDefaultTransformationMatrix();
		}

		// Token: 0x060060A0 RID: 24736 RVA: 0x001EA818 File Offset: 0x001E8A18
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			BuilderItem.BuilderItemState itemState = (BuilderItem.BuilderItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
			if (this.enableCollidersWhenReady)
			{
				bool flag = this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsRight) || this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsLeft);
				this.handsFreeOfCollidersTime += (flag ? 0f : Time.deltaTime);
				if (this.handsFreeOfCollidersTime > 0.1f)
				{
					this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
					this.enableCollidersWhenReady = false;
				}
			}
		}

		// Token: 0x060060A1 RID: 24737 RVA: 0x001EA8D0 File Offset: 0x001E8AD0
		private bool IsOverlapping(List<InteractionPoint> interactionPoints)
		{
			if (interactionPoints == null)
			{
				return false;
			}
			for (int i = 0; i < interactionPoints.Count; i++)
			{
				if (interactionPoints[i] == this.gripInteractor)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060060A2 RID: 24738 RVA: 0x001EA90A File Offset: 0x001E8B0A
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
		}

		// Token: 0x060060A3 RID: 24739 RVA: 0x001EA912 File Offset: 0x001E8B12
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (GorillaTagger.Instance.offlineVRRig.scaleFactor < 1f)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x060060A4 RID: 24740 RVA: 0x001EA93A File Offset: 0x001E8B3A
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			this.parentItem = null;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			return true;
		}

		// Token: 0x060060A5 RID: 24741 RVA: 0x001EA975 File Offset: 0x001E8B75
		public void OnHoverOverTableStart(BuilderTable table)
		{
			this.currTable = table;
		}

		// Token: 0x060060A6 RID: 24742 RVA: 0x001EA97E File Offset: 0x001E8B7E
		public void OnHoverOverTableEnd(BuilderTable table)
		{
			this.currTable = null;
		}

		// Token: 0x060060A7 RID: 24743 RVA: 0x001EA987 File Offset: 0x001E8B87
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
		}

		// Token: 0x060060A8 RID: 24744 RVA: 0x001EA990 File Offset: 0x001E8B90
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			base.transform.position = this.initialPosition;
			base.transform.rotation = this.initialRotation;
			if (this.worldShareableInstance != null)
			{
				this.worldShareableInstance.transform.position = this.initialPosition;
				this.worldShareableInstance.transform.rotation = this.initialRotation;
			}
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x060060A9 RID: 24745 RVA: 0x000D4992 File Offset: 0x000D2B92
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x060060AA RID: 24746 RVA: 0x001EAA12 File Offset: 0x001E8C12
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x060060AB RID: 24747 RVA: 0x001EAA4B File Offset: 0x001E8C4B
		private bool ShouldPlayFX()
		{
			return this.previousItemState == BuilderItem.BuilderItemState.isHeld || this.previousItemState == BuilderItem.BuilderItemState.dropped;
		}

		// Token: 0x060060AC RID: 24748 RVA: 0x001EAA62 File Offset: 0x001E8C62
		public static GameObject BuildEnvItem(int prefabHash, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(prefabHash, true);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			return gameObject;
		}

		// Token: 0x060060AD RID: 24749 RVA: 0x001EAA80 File Offset: 0x001E8C80
		protected override void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
		{
			if (leftHand)
			{
				this.reliableState.leftHandAttachPos = localPosition;
				this.reliableState.leftHandAttachRot = localRotation;
			}
			else
			{
				this.reliableState.rightHandAttachPos = localPosition;
				this.reliableState.rightHandAttachRot = localRotation;
			}
			this.reliableState.dirty = true;
		}

		// Token: 0x060060AE RID: 24750 RVA: 0x001EAACE File Offset: 0x001E8CCE
		public int GetPhotonViewId()
		{
			if (this.worldShareableInstance == null)
			{
				return -1;
			}
			return this.worldShareableInstance.ViewID;
		}

		// Token: 0x04006F34 RID: 28468
		public BuilderItemReliableState reliableState;

		// Token: 0x04006F35 RID: 28469
		public string builtItemPath;

		// Token: 0x04006F36 RID: 28470
		public GameObject itemRoot;

		// Token: 0x04006F37 RID: 28471
		private bool enableCollidersWhenReady;

		// Token: 0x04006F38 RID: 28472
		private float handsFreeOfCollidersTime;

		// Token: 0x04006F39 RID: 28473
		[NonSerialized]
		public BuilderPiece attachedPiece;

		// Token: 0x04006F3A RID: 28474
		public List<Behaviour> onlyWhenPlacedBehaviours;

		// Token: 0x04006F3B RID: 28475
		[NonSerialized]
		public BuilderItem parentItem;

		// Token: 0x04006F3C RID: 28476
		public List<BuilderAttachGridPlane> gridPlanes;

		// Token: 0x04006F3D RID: 28477
		public List<BuilderAttachEdge> edges;

		// Token: 0x04006F3E RID: 28478
		private List<Collider> colliders;

		// Token: 0x04006F3F RID: 28479
		private Transform parent;

		// Token: 0x04006F40 RID: 28480
		private Vector3 initialPosition;

		// Token: 0x04006F41 RID: 28481
		private Quaternion initialRotation;

		// Token: 0x04006F42 RID: 28482
		private Vector3 initialGrabInteractorScale;

		// Token: 0x04006F43 RID: 28483
		private BuilderTable currTable;

		// Token: 0x04006F44 RID: 28484
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006F45 RID: 28485
		public AudioClip snapAudio;

		// Token: 0x04006F46 RID: 28486
		public AudioClip placeAudio;

		// Token: 0x04006F47 RID: 28487
		public GameObject placeVFX;

		// Token: 0x04006F48 RID: 28488
		private new BuilderItem.BuilderItemState previousItemState = BuilderItem.BuilderItemState.dropped;

		// Token: 0x02000F59 RID: 3929
		private enum BuilderItemState
		{
			// Token: 0x04006F4A RID: 28490
			isHeld = 1,
			// Token: 0x04006F4B RID: 28491
			dropped,
			// Token: 0x04006F4C RID: 28492
			placed = 4,
			// Token: 0x04006F4D RID: 28493
			unused0 = 8,
			// Token: 0x04006F4E RID: 28494
			none = 16
		}
	}
}
