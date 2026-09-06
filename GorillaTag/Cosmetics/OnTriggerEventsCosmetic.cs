using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200134F RID: 4943
	[RequireComponent(typeof(Collider))]
	public class OnTriggerEventsCosmetic : MonoBehaviour
	{
		// Token: 0x06007BE0 RID: 31712 RVA: 0x002877F8 File Offset: 0x002859F8
		private bool IsMyItem()
		{
			return this.rig != null && this.rig.isOfflineVRRig;
		}

		// Token: 0x06007BE1 RID: 31713 RVA: 0x00287818 File Offset: 0x00285A18
		private void Awake()
		{
			Collider[] components = base.GetComponents<Collider>();
			if (components == null || components.Length == 0)
			{
				Debug.LogError("OnTriggerEventsCosmetic requires at least one Collider on the same GameObject.");
				base.enabled = false;
				return;
			}
			bool flag = false;
			foreach (Collider collider in components)
			{
				if (collider != null && (collider.isTrigger || collider.attachedRigidbody != null))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.LogWarning("OnTriggerEventsCosmetic: Collider is not set to Trigger. OnTrigger will not fire. Path=" + base.transform.GetPathQ(), base.transform);
			}
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
			List<OnTriggerEventsCosmetic.Listener> list = new List<OnTriggerEventsCosmetic.Listener>();
			List<OnTriggerEventsCosmetic.Listener> list2 = new List<OnTriggerEventsCosmetic.Listener>();
			List<OnTriggerEventsCosmetic.Listener> list3 = new List<OnTriggerEventsCosmetic.Listener>();
			if (this.eventListeners != null)
			{
				for (int j = 0; j < this.eventListeners.Length; j++)
				{
					OnTriggerEventsCosmetic.Listener listener = this.eventListeners[j];
					if (listener.tagSet == null)
					{
						if (listener.triggerTagsList != null && listener.triggerTagsList.Count > 0)
						{
							listener.tagSet = new HashSet<string>(listener.triggerTagsList);
						}
						else
						{
							listener.tagSet = new HashSet<string>();
						}
					}
					if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerEnter)
					{
						list.Add(listener);
					}
					else if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerStay)
					{
						list2.Add(listener);
					}
					else if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerExit)
					{
						list3.Add(listener);
					}
				}
			}
			this.enterListeners = ((list.Count > 0) ? list.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>());
			this.stayListeners = ((list2.Count > 0) ? list2.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>());
			this.exitListeners = ((list3.Count > 0) ? list3.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>());
		}

		// Token: 0x06007BE2 RID: 31714 RVA: 0x00287A1A File Offset: 0x00285C1A
		private void OnTriggerEnter(Collider other)
		{
			if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
			{
				return;
			}
			this.Dispatch(this.enterListeners, other);
		}

		// Token: 0x06007BE3 RID: 31715 RVA: 0x00287A32 File Offset: 0x00285C32
		private void OnTriggerStay(Collider other)
		{
			if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
			{
				return;
			}
			this.Dispatch(this.stayListeners, other);
		}

		// Token: 0x06007BE4 RID: 31716 RVA: 0x00287A4A File Offset: 0x00285C4A
		private void OnTriggerExit(Collider other)
		{
			if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
			{
				return;
			}
			this.Dispatch(this.exitListeners, other);
		}

		// Token: 0x06007BE5 RID: 31717 RVA: 0x00287A64 File Offset: 0x00285C64
		private static bool IsOtherUsable(Collider other)
		{
			if (other == null)
			{
				return false;
			}
			GameObject gameObject = other.gameObject;
			return !(gameObject == null) && gameObject.activeInHierarchy;
		}

		// Token: 0x06007BE6 RID: 31718 RVA: 0x00287A98 File Offset: 0x00285C98
		private void Dispatch(OnTriggerEventsCosmetic.Listener[] listeners, Collider other)
		{
			if (listeners == null || listeners.Length == 0)
			{
				return;
			}
			int layer = other.gameObject.layer;
			GorillaGrabber gorillaGrabber = null;
			bool flag = other.TryGetComponent<GorillaGrabber>(out gorillaGrabber) && gorillaGrabber.enabled;
			bool flag2 = flag && gorillaGrabber.IsLeftHand;
			bool flag3;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem = this.myHeldItem;
				flag3 = heldItem != null && heldItem.InLeftHand();
			}
			else
			{
				flag3 = this.parentTransferable.InLeftHand();
			}
			bool flag4 = flag3;
			Vector3 vector = ((this.myCollider != null) ? this.myCollider.bounds.center : base.transform.position);
			foreach (OnTriggerEventsCosmetic.Listener listener in listeners)
			{
				bool flag5 = ((listener.handSource == OnTriggerEventsCosmetic.HandSource.HoldingHand) ? flag4 : (flag ? flag2 : flag4));
				if ((listener.syncForEveryoneInRoom || this.IsMyItem()) && (!listener.fireOnlyWhileHeld || !this.parentTransferable || this.parentTransferable.InHand()) && (listener.tagSet == null || listener.tagSet.Count <= 0 || OnTriggerEventsCosmetic.CompareTagAny(other.gameObject, listener.tagSet)) && ((1 << layer) & listener.triggerLayerMask.value) != 0)
				{
					UnityEvent<bool, Collider> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(flag5, other);
					}
					Vector3 vector2 = other.ClosestPoint(vector);
					UnityEvent<Vector3> listenerComponentContactPoint = listener.listenerComponentContactPoint;
					if (listenerComponentContactPoint != null)
					{
						listenerComponentContactPoint.Invoke(vector2);
					}
					VRRig componentInParent = other.GetComponentInParent<VRRig>();
					if (componentInParent != null)
					{
						UnityEvent<VRRig> onTriggeredVRRig = listener.onTriggeredVRRig;
						if (onTriggeredVRRig != null)
						{
							onTriggeredVRRig.Invoke(componentInParent);
						}
					}
				}
			}
		}

		// Token: 0x06007BE7 RID: 31719 RVA: 0x00287C48 File Offset: 0x00285E48
		private static bool CompareTagAny(GameObject go, HashSet<string> tagSet)
		{
			if (tagSet == null || tagSet.Count == 0)
			{
				return true;
			}
			foreach (string text in tagSet)
			{
				if (!string.IsNullOrEmpty(text) && go.CompareTag(text))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007BE8 RID: 31720 RVA: 0x00287CB4 File Offset: 0x00285EB4
		private bool IsTagValid(GameObject obj, OnTriggerEventsCosmetic.Listener listener)
		{
			return listener == null || (listener.tagSet == null || listener.tagSet.Count == 0) || OnTriggerEventsCosmetic.CompareTagAny(obj, listener.tagSet);
		}

		// Token: 0x04008DF8 RID: 36344
		[Tooltip("List of per-condition listeners. Each entry specifies when (Enter/Stay/Exit), what to trigger with (layers/tags), and which UnityEvents to fire.")]
		public OnTriggerEventsCosmetic.Listener[] eventListeners = new OnTriggerEventsCosmetic.Listener[0];

		// Token: 0x04008DF9 RID: 36345
		private OnTriggerEventsCosmetic.Listener[] enterListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();

		// Token: 0x04008DFA RID: 36346
		private OnTriggerEventsCosmetic.Listener[] stayListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();

		// Token: 0x04008DFB RID: 36347
		private OnTriggerEventsCosmetic.Listener[] exitListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();

		// Token: 0x04008DFC RID: 36348
		private Collider myCollider;

		// Token: 0x04008DFD RID: 36349
		private VRRig rig;

		// Token: 0x04008DFE RID: 36350
		private TransferrableObject parentTransferable;

		// Token: 0x04008DFF RID: 36351
		private IHeldItem myHeldItem;

		// Token: 0x02001350 RID: 4944
		[Serializable]
		public class Listener
		{
			// Token: 0x04008E00 RID: 36352
			[Tooltip("Only trigger interactions with objects on these layers.")]
			public LayerMask triggerLayerMask;

			// Token: 0x04008E01 RID: 36353
			[Tooltip("Optional tag whitelist. If non-empty, triggers must match at least one of these tags.")]
			public List<string> triggerTagsList = new List<string>();

			// Token: 0x04008E02 RID: 36354
			[Tooltip("Choose which trigger phase invokes this listener: Enter, Stay, or Exit.")]
			public OnTriggerEventsCosmetic.EventType eventType;

			// Token: 0x04008E03 RID: 36355
			public UnityEvent<bool, Collider> listenerComponent;

			// Token: 0x04008E04 RID: 36356
			public UnityEvent<Vector3> listenerComponentContactPoint;

			// Token: 0x04008E05 RID: 36357
			public UnityEvent<VRRig> onTriggeredVRRig;

			// Token: 0x04008E06 RID: 36358
			[Tooltip("If true, fire for everyone in the room. If false, only fire when this item is owned locally (offline rig).")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04008E07 RID: 36359
			[Tooltip("If true, only fire while this item is held. Requires a TransferrableObject on this object or a parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x04008E08 RID: 36360
			[Tooltip("Which hand determines the isLeftHand argument passed to the event.")]
			public OnTriggerEventsCosmetic.HandSource handSource;

			// Token: 0x04008E09 RID: 36361
			[NonSerialized]
			public HashSet<string> tagSet;
		}

		// Token: 0x02001351 RID: 4945
		public enum EventType
		{
			// Token: 0x04008E0B RID: 36363
			TriggerEnter,
			// Token: 0x04008E0C RID: 36364
			TriggerStay,
			// Token: 0x04008E0D RID: 36365
			TriggerExit
		}

		// Token: 0x02001352 RID: 4946
		public enum HandSource
		{
			// Token: 0x04008E0F RID: 36367
			[Tooltip("isLeftHand = which hand is physically touching this trigger (GorillaGrabber). Falls back to the holding hand if no hand collider is detected.")]
			TouchingHand,
			// Token: 0x04008E10 RID: 36368
			[Tooltip("isLeftHand = which hand this cosmetic is equipped in (TransferrableObject). Falls back to the touching hand if no TransferrableObject is found.")]
			HoldingHand
		}
	}
}
