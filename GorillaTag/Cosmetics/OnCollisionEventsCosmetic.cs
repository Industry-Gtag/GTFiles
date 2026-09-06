using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200134B RID: 4939
	[RequireComponent(typeof(Collider))]
	public class OnCollisionEventsCosmetic : MonoBehaviour
	{
		// Token: 0x06007BD5 RID: 31701 RVA: 0x002872E5 File Offset: 0x002854E5
		private bool IsMyItem()
		{
			return this.rig != null && this.rig.isOfflineVRRig;
		}

		// Token: 0x06007BD6 RID: 31702 RVA: 0x00287304 File Offset: 0x00285504
		private void Awake()
		{
			this.myCollider = base.GetComponent<Collider>();
			if (this.myCollider == null)
			{
				Debug.LogError("OnCollisionEventsCosmetic requires a Collider on the same GameObject.");
				base.enabled = false;
				return;
			}
			if (this.myCollider.isTrigger)
			{
				Debug.LogWarning("OnCollisionEventsCosmetic: Collider is set to Trigger. OnCollision will not fire. Set it to non-trigger for collisions.");
			}
			this.rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
			List<OnCollisionEventsCosmetic.Listener> list = new List<OnCollisionEventsCosmetic.Listener>();
			List<OnCollisionEventsCosmetic.Listener> list2 = new List<OnCollisionEventsCosmetic.Listener>();
			List<OnCollisionEventsCosmetic.Listener> list3 = new List<OnCollisionEventsCosmetic.Listener>();
			if (this.eventListeners != null)
			{
				for (int i = 0; i < this.eventListeners.Length; i++)
				{
					OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
					if (listener.tagSet == null)
					{
						if (listener.collisionTagsList != null && listener.collisionTagsList.Count > 0)
						{
							listener.tagSet = new HashSet<string>(listener.collisionTagsList);
						}
						else
						{
							listener.tagSet = new HashSet<string>();
						}
					}
					if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionEnter)
					{
						list.Add(listener);
					}
					else if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionStay)
					{
						list2.Add(listener);
					}
					else if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionExit)
					{
						list3.Add(listener);
					}
				}
			}
			this.enterListeners = ((list.Count > 0) ? list.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
			this.stayListeners = ((list2.Count > 0) ? list2.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
			this.exitListeners = ((list3.Count > 0) ? list3.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
		}

		// Token: 0x06007BD7 RID: 31703 RVA: 0x00287487 File Offset: 0x00285687
		private void OnCollisionEnter(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.enterListeners, collision);
		}

		// Token: 0x06007BD8 RID: 31704 RVA: 0x0028749F File Offset: 0x0028569F
		private void OnCollisionStay(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.stayListeners, collision);
		}

		// Token: 0x06007BD9 RID: 31705 RVA: 0x002874B7 File Offset: 0x002856B7
		private void OnCollisionExit(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.exitListeners, collision);
		}

		// Token: 0x06007BDA RID: 31706 RVA: 0x002874D0 File Offset: 0x002856D0
		private static bool IsCollisionUsable(Collision collision)
		{
			if (collision == null)
			{
				return false;
			}
			Collider collider = collision.collider;
			if (collider == null)
			{
				return false;
			}
			GameObject gameObject = collider.gameObject;
			return !(gameObject == null) && gameObject.activeInHierarchy;
		}

		// Token: 0x06007BDB RID: 31707 RVA: 0x00287510 File Offset: 0x00285710
		private void Dispatch(OnCollisionEventsCosmetic.Listener[] listeners, Collision collision)
		{
			if (listeners == null || listeners.Length == 0)
			{
				return;
			}
			Collider collider = collision.collider;
			GameObject gameObject = ((collider != null) ? collider.gameObject : null);
			if (gameObject == null)
			{
				return;
			}
			int layer = gameObject.layer;
			GorillaGrabber gorillaGrabber = null;
			bool flag = collider != null && collider.TryGetComponent<GorillaGrabber>(out gorillaGrabber) && gorillaGrabber.enabled;
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
			Vector3 vector2;
			if (collision.contactCount > 0)
			{
				vector2 = collision.GetContact(0).point;
			}
			else
			{
				vector2 = collider.ClosestPoint(vector);
			}
			foreach (OnCollisionEventsCosmetic.Listener listener in listeners)
			{
				bool flag5 = ((listener.handSource == OnCollisionEventsCosmetic.HandSource.HoldingHand) ? flag4 : (flag ? flag2 : flag4));
				if ((listener.syncForEveryoneInRoom || this.IsMyItem()) && (!listener.fireOnlyWhileHeld || !this.parentTransferable || this.parentTransferable.InHand()) && (listener.tagSet == null || listener.tagSet.Count <= 0 || OnCollisionEventsCosmetic.CompareTagAny(gameObject, listener.tagSet)) && ((1 << layer) & listener.collisionLayerMask.value) != 0)
				{
					if (listener.listenerComponent != null)
					{
						listener.listenerComponent.Invoke(flag5, collision);
					}
					if (listener.listenerComponentContactPoint != null)
					{
						listener.listenerComponentContactPoint.Invoke(vector2);
					}
					VRRig componentInParent = gameObject.GetComponentInParent<VRRig>();
					if (componentInParent != null && listener.onCollidedVRRig != null)
					{
						listener.onCollidedVRRig.Invoke(componentInParent);
					}
				}
			}
		}

		// Token: 0x06007BDC RID: 31708 RVA: 0x0028770C File Offset: 0x0028590C
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

		// Token: 0x06007BDD RID: 31709 RVA: 0x00287778 File Offset: 0x00285978
		private bool IsTagValid(GameObject obj, OnCollisionEventsCosmetic.Listener listener)
		{
			return listener == null || (listener.tagSet == null || listener.tagSet.Count == 0) || OnCollisionEventsCosmetic.CompareTagAny(obj, listener.tagSet);
		}

		// Token: 0x04008DDF RID: 36319
		[Tooltip("List of per-condition listeners. Each entry specifies when (Enter/Stay/Exit), what to collide with (layers/tags), and which UnityEvents to fire.")]
		public OnCollisionEventsCosmetic.Listener[] eventListeners = new OnCollisionEventsCosmetic.Listener[0];

		// Token: 0x04008DE0 RID: 36320
		private OnCollisionEventsCosmetic.Listener[] enterListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04008DE1 RID: 36321
		private OnCollisionEventsCosmetic.Listener[] stayListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04008DE2 RID: 36322
		private OnCollisionEventsCosmetic.Listener[] exitListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04008DE3 RID: 36323
		private Collider myCollider;

		// Token: 0x04008DE4 RID: 36324
		private VRRig rig;

		// Token: 0x04008DE5 RID: 36325
		private TransferrableObject parentTransferable;

		// Token: 0x04008DE6 RID: 36326
		private IHeldItem myHeldItem;

		// Token: 0x0200134C RID: 4940
		[Serializable]
		public class Listener
		{
			// Token: 0x04008DE7 RID: 36327
			[Tooltip("Only collisions with objects on these layers will be considered.")]
			public LayerMask collisionLayerMask;

			// Token: 0x04008DE8 RID: 36328
			[Tooltip("Optional tag whitelist. If non-empty, collisions must match at least one of these tags.")]
			public List<string> collisionTagsList = new List<string>();

			// Token: 0x04008DE9 RID: 36329
			[Tooltip("Choose which collision phase triggers this listener: Enter, Stay, or Exit.")]
			public OnCollisionEventsCosmetic.EventType eventType;

			// Token: 0x04008DEA RID: 36330
			public UnityEvent<bool, Collision> listenerComponent;

			// Token: 0x04008DEB RID: 36331
			public UnityEvent<Vector3> listenerComponentContactPoint;

			// Token: 0x04008DEC RID: 36332
			public UnityEvent<VRRig> onCollidedVRRig;

			// Token: 0x04008DED RID: 36333
			[Tooltip("If true, fire for everyone in the room. If false, only fire when this item is owned locally (offline rig).")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04008DEE RID: 36334
			[Tooltip("If true, only fire while this item is held. Requires a TransferrableObject on this object or a parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x04008DEF RID: 36335
			[Tooltip("Which hand determines the isLeftHand argument passed to the event.")]
			public OnCollisionEventsCosmetic.HandSource handSource;

			// Token: 0x04008DF0 RID: 36336
			[NonSerialized]
			public HashSet<string> tagSet;
		}

		// Token: 0x0200134D RID: 4941
		public enum EventType
		{
			// Token: 0x04008DF2 RID: 36338
			CollisionEnter,
			// Token: 0x04008DF3 RID: 36339
			CollisionStay,
			// Token: 0x04008DF4 RID: 36340
			CollisionExit
		}

		// Token: 0x0200134E RID: 4942
		public enum HandSource
		{
			// Token: 0x04008DF6 RID: 36342
			[Tooltip("isLeftHand = which hand is physically colliding with this object (GorillaGrabber). Falls back to the holding hand if no hand collider is detected.")]
			TouchingHand,
			// Token: 0x04008DF7 RID: 36343
			[Tooltip("isLeftHand = which hand this cosmetic is equipped in (TransferrableObject). Falls back to the touching hand if no TransferrableObject is found.")]
			HoldingHand
		}
	}
}
