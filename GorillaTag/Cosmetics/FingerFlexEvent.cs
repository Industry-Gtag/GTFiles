using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001336 RID: 4918
	public class FingerFlexEvent : MonoBehaviourTick
	{
		// Token: 0x06007B7F RID: 31615 RVA: 0x0028566F File Offset: 0x0028386F
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
		}

		// Token: 0x06007B80 RID: 31616 RVA: 0x00285695 File Offset: 0x00283895
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x06007B81 RID: 31617 RVA: 0x002856B4 File Offset: 0x002838B4
		public override void Tick()
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				FingerFlexEvent.Listener listener = this.eventListeners[i];
				this.FireEvents(listener);
			}
		}

		// Token: 0x06007B82 RID: 31618 RVA: 0x002856E4 File Offset: 0x002838E4
		private void FireEvents(FingerFlexEvent.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			bool flag = this.parentTransferable != null || this.myHeldItem != null;
			bool flag2;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem = this.myHeldItem;
				flag2 = heldItem == null || heldItem.InHand();
			}
			else
			{
				flag2 = this.parentTransferable.InHand();
			}
			bool flag3 = flag2;
			if (!this.ignoreTransferable && listener.fireOnlyWhileHeld && flag && !flag3 && listener.eventType == FingerFlexEvent.EventType.OnFingerReleased)
			{
				if (listener.fingerRightLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(false, 0f);
					}
					listener.fingerRightLastValue = 0f;
				}
				if (listener.fingerLeftLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(true, 0f);
					}
					listener.fingerLeftLastValue = 0f;
				}
			}
			if (!this.ignoreTransferable && flag && listener.fireOnlyWhileHeld && !flag3)
			{
				return;
			}
			switch (this.fingerType)
			{
			case FingerFlexEvent.FingerType.Thumb:
			{
				float calcT = this._rig.leftThumb.calcT;
				float calcT2 = this._rig.rightThumb.calcT;
				this.FireEvents(listener, calcT, calcT2);
				return;
			}
			case FingerFlexEvent.FingerType.Index:
			{
				float calcT3 = this._rig.leftIndex.calcT;
				float calcT4 = this._rig.rightIndex.calcT;
				this.FireEvents(listener, calcT3, calcT4);
				return;
			}
			case FingerFlexEvent.FingerType.Middle:
			{
				float calcT5 = this._rig.leftMiddle.calcT;
				float calcT6 = this._rig.rightMiddle.calcT;
				this.FireEvents(listener, calcT5, calcT6);
				return;
			}
			case FingerFlexEvent.FingerType.IndexAndMiddleMin:
			{
				float num = Mathf.Min(this._rig.leftIndex.calcT, this._rig.leftMiddle.calcT);
				float num2 = Mathf.Min(this._rig.rightIndex.calcT, this._rig.rightMiddle.calcT);
				this.FireEvents(listener, num, num2);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06007B83 RID: 31619 RVA: 0x002858EC File Offset: 0x00283AEC
		private void FireEvents(FingerFlexEvent.Listener listener, float leftFinger, float rightFinger)
		{
			bool flag = this.parentTransferable != null || this.myHeldItem != null;
			if ((this.ignoreTransferable && listener.checkLeftHand) || (!this.ignoreTransferable && flag && this.FingerFlexValidation(true)))
			{
				this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
				return;
			}
			if ((this.ignoreTransferable && !listener.checkLeftHand) || (!this.ignoreTransferable && flag && this.FingerFlexValidation(false)))
			{
				this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
				return;
			}
			this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
			this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
		}

		// Token: 0x06007B84 RID: 31620 RVA: 0x002859A0 File Offset: 0x00283BA0
		private void CheckFingerValue(FingerFlexEvent.Listener listener, float fingerValue, bool isLeft, ref float lastValue)
		{
			if (fingerValue > listener.fingerFlexValue)
			{
				listener.frameCounter++;
			}
			switch (listener.eventType)
			{
			case FingerFlexEvent.EventType.OnFingerFlexed:
				if (fingerValue > listener.fingerFlexValue && lastValue < listener.fingerFlexValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(isLeft, fingerValue);
					}
				}
				break;
			case FingerFlexEvent.EventType.OnFingerReleased:
				if (fingerValue <= listener.fingerReleaseValue && lastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			case FingerFlexEvent.EventType.OnFingerFlexStayed:
				if (fingerValue > listener.fingerFlexValue && lastValue >= listener.fingerFlexValue && listener.frameCounter % listener.frameInterval == 0)
				{
					UnityEvent<bool, float> listenerComponent3 = listener.listenerComponent;
					if (listenerComponent3 != null)
					{
						listenerComponent3.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			}
			lastValue = fingerValue;
		}

		// Token: 0x06007B85 RID: 31621 RVA: 0x00285A84 File Offset: 0x00283C84
		private bool FingerFlexValidation(bool isLeftHand)
		{
			bool flag;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem = this.myHeldItem;
				flag = heldItem != null && heldItem.InLeftHand();
			}
			else
			{
				flag = this.parentTransferable.InLeftHand();
			}
			bool flag2 = flag;
			return (!flag2 || isLeftHand) && (flag2 || !isLeftHand);
		}

		// Token: 0x04008D59 RID: 36185
		[SerializeField]
		public bool ignoreTransferable;

		// Token: 0x04008D5A RID: 36186
		[SerializeField]
		private FingerFlexEvent.FingerType fingerType = FingerFlexEvent.FingerType.Index;

		// Token: 0x04008D5B RID: 36187
		public FingerFlexEvent.Listener[] eventListeners = new FingerFlexEvent.Listener[0];

		// Token: 0x04008D5C RID: 36188
		private VRRig _rig;

		// Token: 0x04008D5D RID: 36189
		private TransferrableObject parentTransferable;

		// Token: 0x04008D5E RID: 36190
		private IHeldItem myHeldItem;

		// Token: 0x02001337 RID: 4919
		[Serializable]
		public class Listener
		{
			// Token: 0x04008D5F RID: 36191
			public FingerFlexEvent.EventType eventType;

			// Token: 0x04008D60 RID: 36192
			public UnityEvent<bool, float> listenerComponent;

			// Token: 0x04008D61 RID: 36193
			public float fingerFlexValue = 0.75f;

			// Token: 0x04008D62 RID: 36194
			public float fingerReleaseValue = 0.01f;

			// Token: 0x04008D63 RID: 36195
			[Tooltip("How many frames should pass to fire a finger flex stayed event")]
			public int frameInterval = 20;

			// Token: 0x04008D64 RID: 36196
			[Tooltip("This event will be fired for everyone in the room (synced) by default unless you uncheck this box so that it will be fired only for the local player.")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04008D65 RID: 36197
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x04008D66 RID: 36198
			[Tooltip("Whether to check the left hand or the right hand, only works if \"ignoreTransferable\" is true.")]
			public bool checkLeftHand;

			// Token: 0x04008D67 RID: 36199
			internal int frameCounter;

			// Token: 0x04008D68 RID: 36200
			internal float fingerRightLastValue;

			// Token: 0x04008D69 RID: 36201
			internal float fingerLeftLastValue;
		}

		// Token: 0x02001338 RID: 4920
		public enum EventType
		{
			// Token: 0x04008D6B RID: 36203
			OnFingerFlexed,
			// Token: 0x04008D6C RID: 36204
			OnFingerReleased,
			// Token: 0x04008D6D RID: 36205
			OnFingerFlexStayed
		}

		// Token: 0x02001339 RID: 4921
		private enum FingerType
		{
			// Token: 0x04008D6F RID: 36207
			Thumb,
			// Token: 0x04008D70 RID: 36208
			Index,
			// Token: 0x04008D71 RID: 36209
			Middle,
			// Token: 0x04008D72 RID: 36210
			IndexAndMiddleMin
		}
	}
}
