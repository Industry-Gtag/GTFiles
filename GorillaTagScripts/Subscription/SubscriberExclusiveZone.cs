using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000FF6 RID: 4086
	public class SubscriberExclusiveZone : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006591 RID: 26001 RVA: 0x0020B36C File Offset: 0x0020956C
		private void Awake()
		{
			if (this.restrictedZone != null)
			{
				this.restrictedZoneCollider = this.restrictedZone.GetComponent<Collider>();
				if (this.restrictedZoneCollider != null && !this.restrictedZoneCollider.isTrigger)
				{
					Debug.LogError("restrictedZone must be a trigger collider!", this);
					base.enabled = false;
					return;
				}
				SubscriberZoneTrigger subscriberZoneTrigger = this.restrictedZone.GetComponent<SubscriberZoneTrigger>();
				if (subscriberZoneTrigger == null)
				{
					subscriberZoneTrigger = this.restrictedZone.AddComponent<SubscriberZoneTrigger>();
				}
				subscriberZoneTrigger.parentZone = this;
				subscriberZoneTrigger.isRestrictedZone = true;
			}
			if (this.warningZone != null)
			{
				this.influenceZoneCollider = this.warningZone.GetComponent<Collider>();
				if (this.influenceZoneCollider != null && !this.influenceZoneCollider.isTrigger)
				{
					Debug.LogError("influenceZone must be a trigger collider!", this);
					base.enabled = false;
					return;
				}
				SubscriberZoneTrigger subscriberZoneTrigger2 = this.warningZone.GetComponent<SubscriberZoneTrigger>();
				if (subscriberZoneTrigger2 == null)
				{
					subscriberZoneTrigger2 = this.warningZone.AddComponent<SubscriberZoneTrigger>();
				}
				subscriberZoneTrigger2.parentZone = this;
				subscriberZoneTrigger2.isRestrictedZone = false;
			}
			if (this.ejectionPoint == null)
			{
				Debug.LogError("Assign an ejectionPoint!", this);
				base.enabled = false;
				return;
			}
			this.UpdateDoor();
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x0020B498 File Offset: 0x00209698
		private void OnEnable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		// Token: 0x06006593 RID: 26003 RVA: 0x0020B4A8 File Offset: 0x002096A8
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
			this.ClearAllRigOverrides();
		}

		// Token: 0x06006594 RID: 26004 RVA: 0x0020B4BF File Offset: 0x002096BF
		private void Update()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			this.UpdateDoor();
			if (!SubscriptionManager.IsLocalSubscribed())
			{
				this.HandleZoneBehavior();
				return;
			}
			if (this.bodyColliderWasDisabled)
			{
				this.SetBodyCollider(GTPlayer.Instance, true);
				this.bodyColliderWasDisabled = false;
			}
		}

		// Token: 0x06006595 RID: 26005 RVA: 0x0020B4F8 File Offset: 0x002096F8
		public void OnZoneEnter(bool isRestricted)
		{
			if (isRestricted)
			{
				this.insideRestricted = true;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Entered restricted zone");
					return;
				}
			}
			else
			{
				this.insideInfluence = true;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Entered warning zone");
				}
			}
		}

		// Token: 0x06006596 RID: 26006 RVA: 0x0020B530 File Offset: 0x00209730
		public void OnZoneExit(bool isRestricted)
		{
			if (isRestricted)
			{
				this.insideRestricted = false;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Exited restricted zone");
					return;
				}
			}
			else
			{
				this.insideInfluence = false;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Exited warning zone");
				}
			}
		}

		// Token: 0x06006597 RID: 26007 RVA: 0x0020B568 File Offset: 0x00209768
		private void HandleZoneBehavior()
		{
			GTPlayer instance = GTPlayer.Instance;
			if (instance == null)
			{
				return;
			}
			if (this.insideRestricted)
			{
				if (Time.time - this.lastShoveTime >= this.shoveCooldown)
				{
					this.lastShoveTime = Time.time;
					instance.TeleportTo(this.ejectionPoint.transform, true, true);
					UnityEvent onEnterRestrictedZone = this.OnEnterRestrictedZone;
					if (onEnterRestrictedZone == null)
					{
						return;
					}
					onEnterRestrictedZone.Invoke();
				}
				return;
			}
			if (this.insideInfluence)
			{
				this.DisplaceToward(instance, this.ejectionPoint, this.driftSpeed);
				UnityEvent onWarning = this.OnWarning;
				if (onWarning == null)
				{
					return;
				}
				onWarning.Invoke();
			}
		}

		// Token: 0x06006598 RID: 26008 RVA: 0x0020B5FC File Offset: 0x002097FC
		private Vector3 FindSafeEjectionPosition(Vector3 playerPos)
		{
			if (this.restrictedZoneCollider == null)
			{
				return this.ejectionPoint.position;
			}
			Bounds bounds = this.restrictedZoneCollider.bounds;
			Vector3 vector = bounds.ClosestPoint(playerPos);
			Vector3 normalized = (vector - bounds.center).normalized;
			Vector3 vector2 = vector + normalized * (this.safetyCheckRadius + 0.5f);
			float num = Vector3.Distance(playerPos, vector2);
			RaycastHit raycastHit;
			if (Physics.SphereCast(playerPos + Vector3.up * this.safetyCheckRadius, this.safetyCheckRadius, normalized, out raycastHit, num, this.obstacleLayers))
			{
				vector2 = playerPos + normalized * Mathf.Max(0.1f, raycastHit.distance - this.safetyCheckRadius - 0.2f);
			}
			return vector2;
		}

		// Token: 0x06006599 RID: 26009 RVA: 0x0020B6CC File Offset: 0x002098CC
		private void DisplaceToward(GTPlayer player, Transform target, float speed)
		{
			Vector3 normalized = (target.position - player.transform.position).normalized;
			player.transform.position += normalized * speed * Time.deltaTime;
		}

		// Token: 0x0600659A RID: 26010 RVA: 0x0020B720 File Offset: 0x00209920
		private void SetBodyCollider(GTPlayer player, bool enabled)
		{
			if (player != null && player.bodyCollider != null)
			{
				player.bodyCollider.enabled = enabled;
				this.bodyColliderWasDisabled = !enabled;
				if (this.showDebugInfo && player.bodyCollider.enabled != enabled)
				{
					Debug.Log(string.Format("[Zone] Body collider: {0}", enabled));
				}
			}
		}

		// Token: 0x0600659B RID: 26011 RVA: 0x0020B788 File Offset: 0x00209988
		private void UpdateDoor()
		{
			bool flag = SubscriptionManager.IsLocalSubscribed();
			if (this.nonSubscribeDoorObject.activeSelf == flag)
			{
				this.nonSubscribeDoorObject.SetActive(!flag);
			}
			if (this.subscriberDoorObject.activeSelf != flag)
			{
				this.subscriberDoorObject.SetActive(SubscriptionManager.IsLocalSubscribed());
			}
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x0020B7D8 File Offset: 0x002099D8
		public void SliceUpdate()
		{
			IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
			if (this.restrictedZoneCollider == null)
			{
				return;
			}
			for (int i = 0; i < activeRigs.Count; i++)
			{
				if (!activeRigs[i].isOfflineVRRig && !SubscriptionManager.GetSubscriptionDetails(activeRigs[i]).active)
				{
					Vector3 vector = this.restrictedZoneCollider.transform.InverseTransformPoint(activeRigs[i].syncPos);
					Vector3 vector2 = ((BoxCollider)this.restrictedZoneCollider).size / 2f;
					Vector3 center = ((BoxCollider)this.restrictedZoneCollider).center;
					if (vector.x < vector2.x + center.x && vector.x > -vector2.x + center.x && vector.y < vector2.y + center.y && vector.y > -vector2.y + center.y && vector.z < vector2.z + center.z && vector.z > -vector2.z + center.z)
					{
						activeRigs[i].InOverrideSubscriptionZone = true;
						activeRigs[i].OverrideSubscriptionZoneLocation = this.ejectionPoint.position;
					}
					else
					{
						activeRigs[i].InOverrideSubscriptionZone = false;
						activeRigs[i].OverrideSubscriptionZoneLocation = Vector3.zero;
					}
				}
			}
		}

		// Token: 0x0600659D RID: 26013 RVA: 0x0020B954 File Offset: 0x00209B54
		public void ClearAllRigOverrides()
		{
			IReadOnlyList<VRRig> allRigs = VRRigCache.AllRigs;
			for (int i = 0; i < allRigs.Count; i++)
			{
				allRigs[i].InOverrideSubscriptionZone = false;
				allRigs[i].OverrideSubscriptionZoneLocation = Vector3.zero;
			}
		}

		// Token: 0x0600659E RID: 26014 RVA: 0x0020B996 File Offset: 0x00209B96
		private void OnDestroy()
		{
			if (this.tempEjectionObject != null)
			{
				Object.Destroy(this.tempEjectionObject);
			}
		}

		// Token: 0x0600659F RID: 26015 RVA: 0x0020B9B4 File Offset: 0x00209BB4
		private void OnDrawGizmos()
		{
			if (this.restrictedZoneCollider != null)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
				Gizmos.DrawCube(this.restrictedZoneCollider.bounds.center, this.restrictedZoneCollider.bounds.size);
			}
			if (this.influenceZoneCollider != null)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
				Gizmos.DrawCube(this.influenceZoneCollider.bounds.center, this.influenceZoneCollider.bounds.size);
			}
			if (this.ejectionPoint != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(this.ejectionPoint.position, 0.5f);
			}
		}

		// Token: 0x04007490 RID: 29840
		[Header("Zones")]
		[Tooltip("Inner restricted zone - hard pushback")]
		[SerializeField]
		private GameObject restrictedZone;

		// Token: 0x04007491 RID: 29841
		[Tooltip("Outer influence zone - gentle drift")]
		[SerializeField]
		private GameObject warningZone;

		// Token: 0x04007492 RID: 29842
		[Header("Safe Exit Point")]
		[SerializeField]
		private Transform ejectionPoint;

		// Token: 0x04007493 RID: 29843
		[Header("Tuning")]
		[SerializeField]
		private float driftSpeed = 3f;

		// Token: 0x04007494 RID: 29844
		[SerializeField]
		private float shoveCooldown = 0.5f;

		// Token: 0x04007495 RID: 29845
		[Header("Safety")]
		[SerializeField]
		private float safetyCheckRadius = 0.5f;

		// Token: 0x04007496 RID: 29846
		[SerializeField]
		private LayerMask obstacleLayers;

		// Token: 0x04007497 RID: 29847
		[Header("Door Visuals")]
		[SerializeField]
		private GameObject nonSubscribeDoorObject;

		// Token: 0x04007498 RID: 29848
		[SerializeField]
		private GameObject subscriberDoorObject;

		// Token: 0x04007499 RID: 29849
		public UnityEvent OnWarning;

		// Token: 0x0400749A RID: 29850
		public UnityEvent OnEnterRestrictedZone;

		// Token: 0x0400749B RID: 29851
		[Header("Debug")]
		[SerializeField]
		private bool showDebugInfo;

		// Token: 0x0400749C RID: 29852
		private bool insideRestricted;

		// Token: 0x0400749D RID: 29853
		private bool insideInfluence;

		// Token: 0x0400749E RID: 29854
		private float lastShoveTime;

		// Token: 0x0400749F RID: 29855
		private GameObject tempEjectionObject;

		// Token: 0x040074A0 RID: 29856
		private Collider restrictedZoneCollider;

		// Token: 0x040074A1 RID: 29857
		private Collider influenceZoneCollider;

		// Token: 0x040074A2 RID: 29858
		private bool bodyColliderWasDisabled;
	}
}
