using System;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001322 RID: 4898
	public class DistanceCheckerCosmetic : MonoBehaviour, ISpawnable, IGorillaSliceableSimple
	{
		// Token: 0x17000C09 RID: 3081
		// (get) Token: 0x06007AFA RID: 31482 RVA: 0x00281C2C File Offset: 0x0027FE2C
		// (set) Token: 0x06007AFB RID: 31483 RVA: 0x00281C34 File Offset: 0x0027FE34
		public bool IsSpawned { get; set; }

		// Token: 0x17000C0A RID: 3082
		// (get) Token: 0x06007AFC RID: 31484 RVA: 0x00281C3D File Offset: 0x0027FE3D
		// (set) Token: 0x06007AFD RID: 31485 RVA: 0x00281C45 File Offset: 0x0027FE45
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06007AFE RID: 31486 RVA: 0x00281C4E File Offset: 0x0027FE4E
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06007AFF RID: 31487 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDespawn()
		{
		}

		// Token: 0x06007B00 RID: 31488 RVA: 0x00281C58 File Offset: 0x0027FE58
		private void OnEnable()
		{
			this.currentState = DistanceCheckerCosmetic.State.None;
			this.transferableObject = base.GetComponentInParent<TransferrableObject>();
			if (this.transferableObject != null)
			{
				this.ownerRig = this.transferableObject.ownerRig;
			}
			else
			{
				this.ownerRig = base.GetComponentInParent<VRRig>();
			}
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.ResetClosestPlayer();
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06007B01 RID: 31489 RVA: 0x00019269 File Offset: 0x00017469
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06007B02 RID: 31490 RVA: 0x00281CD0 File Offset: 0x0027FED0
		public void SliceUpdate()
		{
			this.UpdateDistance();
		}

		// Token: 0x06007B03 RID: 31491 RVA: 0x00281CD8 File Offset: 0x0027FED8
		private bool IsBelowThreshold(Vector3 distance)
		{
			return distance.IsShorterThan(this.distanceThreshold);
		}

		// Token: 0x06007B04 RID: 31492 RVA: 0x00281CEB File Offset: 0x0027FEEB
		private bool IsAboveThreshold(Vector3 distance)
		{
			return distance.IsLongerThan(this.distanceThreshold);
		}

		// Token: 0x06007B05 RID: 31493 RVA: 0x00281D00 File Offset: 0x0027FF00
		private void UpdateClosestPlayer(bool others = false)
		{
			if (!PhotonNetwork.InRoom)
			{
				this.ResetClosestPlayer();
				return;
			}
			VRRig vrrig = this.currentClosestPlayer;
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
			foreach (VRRig vrrig2 in VRRigCache.ActiveRigs)
			{
				if (!others || !(this.ownerRig != null) || !(vrrig2 == this.ownerRig))
				{
					Vector3 vector = vrrig2.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(vector) && vector.sqrMagnitude < this.closestDistance.sqrMagnitude)
					{
						this.closestDistance = vector;
						this.currentClosestPlayer = vrrig2;
					}
				}
			}
			if (this.currentClosestPlayer != null && this.currentClosestPlayer != vrrig)
			{
				UnityEvent<VRRig, float> unityEvent = this.onClosestPlayerBelowThresholdChanged;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.currentClosestPlayer, this.closestDistance.magnitude);
			}
		}

		// Token: 0x06007B06 RID: 31494 RVA: 0x00281E10 File Offset: 0x00280010
		private void ResetClosestPlayer()
		{
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
		}

		// Token: 0x06007B07 RID: 31495 RVA: 0x00281E24 File Offset: 0x00280024
		private void UpdateDistance()
		{
			bool flag = true;
			switch (this.distanceTo)
			{
			case DistanceCheckerCosmetic.DistanceCondition.Owner:
			{
				Vector3 vector = this.myRig.transform.position - this.distanceFrom.position;
				if (this.IsBelowThreshold(vector))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
					return;
				}
				if (this.IsAboveThreshold(vector))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
				}
				break;
			}
			case DistanceCheckerCosmetic.DistanceCondition.Others:
				this.UpdateClosestPlayer(true);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig in VRRigCache.ActiveRigs)
				{
					if (!(this.ownerRig != null) || !(vrrig == this.ownerRig))
					{
						Vector3 vector2 = vrrig.transform.position - this.distanceFrom.position;
						if (this.IsBelowThreshold(vector2))
						{
							this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
							flag = false;
						}
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			case DistanceCheckerCosmetic.DistanceCondition.Everyone:
				this.UpdateClosestPlayer(false);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig2 in VRRigCache.ActiveRigs)
				{
					Vector3 vector3 = vrrig2.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(vector3))
					{
						this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
						flag = false;
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06007B08 RID: 31496 RVA: 0x00281FB4 File Offset: 0x002801B4
		private void UpdateState(DistanceCheckerCosmetic.State newState)
		{
			if (this.currentState == newState)
			{
				return;
			}
			this.currentState = newState;
			if (this.currentState != DistanceCheckerCosmetic.State.AboveThreshold)
			{
				if (this.currentState == DistanceCheckerCosmetic.State.BelowThreshold)
				{
					UnityEvent unityEvent = this.onOneIsBelowThreshold;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
				return;
			}
			UnityEvent unityEvent2 = this.onAllAreAboveThreshold;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
		}

		// Token: 0x04008C8A RID: 35978
		[SerializeField]
		private Transform distanceFrom;

		// Token: 0x04008C8B RID: 35979
		[SerializeField]
		private DistanceCheckerCosmetic.DistanceCondition distanceTo;

		// Token: 0x04008C8C RID: 35980
		[Tooltip("Receive events when above or below this distance")]
		public float distanceThreshold;

		// Token: 0x04008C8D RID: 35981
		public UnityEvent onOneIsBelowThreshold;

		// Token: 0x04008C8E RID: 35982
		public UnityEvent onAllAreAboveThreshold;

		// Token: 0x04008C8F RID: 35983
		public UnityEvent<VRRig, float> onClosestPlayerBelowThresholdChanged;

		// Token: 0x04008C90 RID: 35984
		private VRRig myRig;

		// Token: 0x04008C91 RID: 35985
		private DistanceCheckerCosmetic.State currentState;

		// Token: 0x04008C92 RID: 35986
		private Vector3 closestDistance;

		// Token: 0x04008C93 RID: 35987
		private VRRig currentClosestPlayer;

		// Token: 0x04008C94 RID: 35988
		private VRRig ownerRig;

		// Token: 0x04008C95 RID: 35989
		private TransferrableObject transferableObject;

		// Token: 0x02001323 RID: 4899
		private enum State
		{
			// Token: 0x04008C99 RID: 35993
			AboveThreshold,
			// Token: 0x04008C9A RID: 35994
			BelowThreshold,
			// Token: 0x04008C9B RID: 35995
			None
		}

		// Token: 0x02001324 RID: 4900
		private enum DistanceCondition
		{
			// Token: 0x04008C9D RID: 35997
			None,
			// Token: 0x04008C9E RID: 35998
			Owner,
			// Token: 0x04008C9F RID: 35999
			Others,
			// Token: 0x04008CA0 RID: 36000
			Everyone
		}
	}
}
