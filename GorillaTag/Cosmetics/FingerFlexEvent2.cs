using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200133A RID: 4922
	public class FingerFlexEvent2 : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06007B88 RID: 31624 RVA: 0x00285B24 File Offset: 0x00283D24
		private bool TryLinkToNextEvent(int index)
		{
			if (index < this.list.Length - 1)
			{
				if (this.list[index].IsFlexTrigger && this.list[index + 1].IsReleaseTrigger)
				{
					this.list[index].linkIndex = index + 1;
					this.list[index + 1].linkIndex = index;
					return true;
				}
				this.list[index + 1].linkIndex = -1;
			}
			this.list[index].linkIndex = -1;
			return false;
		}

		// Token: 0x06007B89 RID: 31625 RVA: 0x00285BA0 File Offset: 0x00283DA0
		private void Awake()
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			this.myTransferrable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
			for (int i = 0; i < this.list.Length; i++)
			{
				FingerFlexEvent2.FlexEvent flexEvent = this.list[i];
				if (this.myTransferrable.IsNull() && flexEvent.RequiresHeldItem)
				{
					this.myTransferrable = base.GetComponentInParent<TransferrableObject>();
				}
				if (flexEvent.tryLink && this.TryLinkToNextEvent(i))
				{
					FingerFlexEvent2.FlexEvent flexEvent2 = this.list[i + 1];
					flexEvent.releaseThreshold = flexEvent2.releaseThreshold;
					flexEvent2.flexThreshold = flexEvent.flexThreshold;
					flexEvent2.fingerType = flexEvent.fingerType;
					flexEvent2.handType = flexEvent.handType;
					flexEvent2.networked = flexEvent.networked;
					i++;
				}
			}
		}

		// Token: 0x06007B8A RID: 31626 RVA: 0x00285C70 File Offset: 0x00283E70
		private void CalcFlex(bool disable)
		{
			for (int i = 0; i < this.list.Length; i++)
			{
				FingerFlexEvent2.FlexEvent flexEvent = this.list[i];
				if ((flexEvent.networked || this.myRig.isOfflineVRRig) && (!flexEvent.RequiresHeldItem || !this.myTransferrable.IsNull() || this.myHeldItem != null) && (flexEvent.handType != FingerFlexEvent2.FlexEvent.HandType.EquippedSide || !this.myTransferrable.IsNull()))
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					switch (flexEvent.handType)
					{
					case FingerFlexEvent2.FlexEvent.HandType.HeldItemHand:
						if (!this.myTransferrable.IsNull())
						{
							flag = this.myTransferrable.currentState == TransferrableObject.PositionState.InLeftHand;
							flag2 = this.myTransferrable.currentState == TransferrableObject.PositionState.InRightHand;
							flag3 = flag || flag2;
						}
						else if (this.myHeldItem != null)
						{
							flag = this.myHeldItem.InLeftHand();
							flag2 = !flag && this.myHeldItem.InHand();
							flag3 = flag || flag2;
						}
						break;
					case FingerFlexEvent2.FlexEvent.HandType.EquippedSide:
						flag = (this.myTransferrable.storedZone & (BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.LeftBack)) > BodyDockPositions.DropPositions.None;
						flag2 = (this.myTransferrable.storedZone & (BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.RightBack)) > BodyDockPositions.DropPositions.None;
						break;
					case FingerFlexEvent2.FlexEvent.HandType.LeftHand:
						flag = true;
						break;
					case FingerFlexEvent2.FlexEvent.HandType.RightHand:
						flag2 = true;
						break;
					}
					if ((!flag || !flag2) && (flag || flag2 || flexEvent.wasHeld))
					{
						float num;
						if (disable || (flexEvent.wasHeld && !flag3))
						{
							num = 0f;
						}
						else
						{
							FingerFlexEvent2.FlexEvent.FingerType fingerType = flexEvent.fingerType;
							float num2;
							switch (fingerType)
							{
							case FingerFlexEvent2.FlexEvent.FingerType.Thumb:
								num2 = (flag ? this.myRig.leftThumb.calcT : this.myRig.rightThumb.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.Index:
								num2 = (flag ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.Middle:
								num2 = (flag ? this.myRig.leftMiddle.calcT : this.myRig.rightMiddle.calcT);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.IndexAndMiddle:
								num2 = (flag ? Mathf.Min(this.myRig.leftIndex.calcT, this.myRig.leftMiddle.calcT) : Mathf.Min(this.myRig.rightIndex.calcT, this.myRig.rightMiddle.calcT));
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.IndexOrMiddle:
								num2 = (flag ? Mathf.Max(this.myRig.leftIndex.calcT, this.myRig.leftMiddle.calcT) : Mathf.Max(this.myRig.rightIndex.calcT, this.myRig.rightMiddle.calcT));
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.StickLeft:
								num2 = Mathf.Max(0f, -ControllerInputPoller.Primary2DAxis(flag ? XRNode.LeftHand : XRNode.RightHand).x);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.StickRight:
								num2 = Mathf.Max(0f, ControllerInputPoller.Primary2DAxis(flag ? XRNode.LeftHand : XRNode.RightHand).x);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.StickUp:
								num2 = Mathf.Max(0f, ControllerInputPoller.Primary2DAxis(flag ? XRNode.LeftHand : XRNode.RightHand).y);
								break;
							case FingerFlexEvent2.FlexEvent.FingerType.StickDown:
								num2 = Mathf.Max(0f, -ControllerInputPoller.Primary2DAxis(flag ? XRNode.LeftHand : XRNode.RightHand).y);
								break;
							default:
								<PrivateImplementationDetails>.ThrowSwitchExpressionException(fingerType);
								break;
							}
							num = num2;
						}
						float num3 = num;
						flexEvent.ProcessState(flag, num3);
						flexEvent.wasHeld = flag3 && !disable;
						if (flexEvent.IsLinked)
						{
							FingerFlexEvent2.FlexEvent flexEvent2 = this.list[i + 1];
							flexEvent2.ProcessState(flag, num3);
							flexEvent2.wasHeld = flag3;
							i++;
						}
					}
				}
			}
		}

		// Token: 0x06007B8B RID: 31627 RVA: 0x0001A297 File Offset: 0x00018497
		public void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06007B8C RID: 31628 RVA: 0x0028601A File Offset: 0x0028421A
		public void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
			this.CalcFlex(true);
		}

		// Token: 0x17000C16 RID: 3094
		// (get) Token: 0x06007B8D RID: 31629 RVA: 0x00286029 File Offset: 0x00284229
		// (set) Token: 0x06007B8E RID: 31630 RVA: 0x00286031 File Offset: 0x00284231
		public bool TickRunning { get; set; }

		// Token: 0x06007B8F RID: 31631 RVA: 0x0028603A File Offset: 0x0028423A
		public void Tick()
		{
			this.CalcFlex(false);
		}

		// Token: 0x04008D73 RID: 36211
		public FingerFlexEvent2.FlexEvent[] list;

		// Token: 0x04008D74 RID: 36212
		private VRRig myRig;

		// Token: 0x04008D75 RID: 36213
		private TransferrableObject myTransferrable;

		// Token: 0x04008D76 RID: 36214
		private IHeldItem myHeldItem;

		// Token: 0x0200133B RID: 4923
		[Serializable]
		public class FlexEvent
		{
			// Token: 0x17000C17 RID: 3095
			// (get) Token: 0x06007B91 RID: 31633 RVA: 0x00286043 File Offset: 0x00284243
			public bool IsFlexTrigger
			{
				get
				{
					return this.triggerType == FingerFlexEvent2.FlexEvent.TriggerType.OnFlex;
				}
			}

			// Token: 0x17000C18 RID: 3096
			// (get) Token: 0x06007B92 RID: 31634 RVA: 0x0028604E File Offset: 0x0028424E
			public bool IsReleaseTrigger
			{
				get
				{
					return this.triggerType == FingerFlexEvent2.FlexEvent.TriggerType.OnRelease;
				}
			}

			// Token: 0x17000C19 RID: 3097
			// (get) Token: 0x06007B93 RID: 31635 RVA: 0x0028605C File Offset: 0x0028425C
			public bool RequiresHeldItem
			{
				get
				{
					FingerFlexEvent2.FlexEvent.HandType handType = this.handType;
					return handType == FingerFlexEvent2.FlexEvent.HandType.HeldItemHand || handType == FingerFlexEvent2.FlexEvent.HandType.EquippedSide;
				}
			}

			// Token: 0x17000C1A RID: 3098
			// (get) Token: 0x06007B94 RID: 31636 RVA: 0x00286080 File Offset: 0x00284280
			public bool HasValidLink
			{
				get
				{
					return this.linkIndex >= 0;
				}
			}

			// Token: 0x17000C1B RID: 3099
			// (get) Token: 0x06007B95 RID: 31637 RVA: 0x0028608E File Offset: 0x0028428E
			public bool IsLinked
			{
				get
				{
					return this.tryLink && this.linkIndex >= 0;
				}
			}

			// Token: 0x17000C1C RID: 3100
			// (get) Token: 0x06007B96 RID: 31638 RVA: 0x002860A6 File Offset: 0x002842A6
			private bool ShowMainProperties
			{
				get
				{
					return !this.IsLinked || this.IsFlexTrigger;
				}
			}

			// Token: 0x17000C1D RID: 3101
			// (get) Token: 0x06007B97 RID: 31639 RVA: 0x002860B8 File Offset: 0x002842B8
			private bool ShowFlexThreshold
			{
				get
				{
					return this.ShowMainProperties;
				}
			}

			// Token: 0x17000C1E RID: 3102
			// (get) Token: 0x06007B98 RID: 31640 RVA: 0x002860C0 File Offset: 0x002842C0
			private bool ShowReleaseThreshold
			{
				get
				{
					return (!this.IsLinked || this.IsReleaseTrigger) && !this.IsFlexTrigger;
				}
			}

			// Token: 0x06007B99 RID: 31641 RVA: 0x002860E0 File Offset: 0x002842E0
			public void ProcessState(bool leftHand, float flexValue)
			{
				this.currentState = ((flexValue < this.releaseThreshold) ? FingerFlexEvent2.FlexEvent.RangeState.Below : ((flexValue >= this.flexThreshold) ? FingerFlexEvent2.FlexEvent.RangeState.Above : FingerFlexEvent2.FlexEvent.RangeState.Within));
				if (this.ShowMainProperties && this.currentState != this.lastState && this.continuousProperties != null && this.continuousProperties.Count > 0)
				{
					float num = Mathf.InverseLerp(this.releaseThreshold, this.flexThreshold, flexValue);
					this.continuousProperties.ApplyAll(num);
				}
				if (this.currentState == FingerFlexEvent2.FlexEvent.RangeState.Above && this.lastState == FingerFlexEvent2.FlexEvent.RangeState.Below)
				{
					this.lastThresholdTime = Time.time;
					this.lastState = FingerFlexEvent2.FlexEvent.RangeState.Above;
					if (this.IsFlexTrigger)
					{
						UnityEvent<bool, float> unityEvent = this.unityEvent;
						if (unityEvent == null)
						{
							return;
						}
						unityEvent.Invoke(leftHand, flexValue);
						return;
					}
				}
				else if (this.currentState == FingerFlexEvent2.FlexEvent.RangeState.Below && this.lastState == FingerFlexEvent2.FlexEvent.RangeState.Above)
				{
					this.lastThresholdTime = Time.time;
					this.lastState = FingerFlexEvent2.FlexEvent.RangeState.Below;
					if (this.IsReleaseTrigger)
					{
						UnityEvent<bool, float> unityEvent2 = this.unityEvent;
						if (unityEvent2 == null)
						{
							return;
						}
						unityEvent2.Invoke(leftHand, flexValue);
					}
				}
			}

			// Token: 0x04008D78 RID: 36216
			public FingerFlexEvent2.FlexEvent.TriggerType triggerType;

			// Token: 0x04008D79 RID: 36217
			public bool tryLink = true;

			// Token: 0x04008D7A RID: 36218
			[HideInInspector]
			public int linkIndex = -1;

			// Token: 0x04008D7B RID: 36219
			[Space]
			public FingerFlexEvent2.FlexEvent.FingerType fingerType = FingerFlexEvent2.FlexEvent.FingerType.Index;

			// Token: 0x04008D7C RID: 36220
			[Space]
			public FingerFlexEvent2.FlexEvent.HandType handType;

			// Token: 0x04008D7D RID: 36221
			private const string ADVANCED = "Advanced Properties";

			// Token: 0x04008D7E RID: 36222
			[Tooltip("When this is checked, all players in the room will fire the event. Otherwise, only the local player will fire it. You should usually leave this on, unless you're using it for something local like controller haptics.")]
			public bool networked = true;

			// Token: 0x04008D7F RID: 36223
			[Range(0.01f, 0.75f)]
			public float flexThreshold = 0.75f;

			// Token: 0x04008D80 RID: 36224
			[Range(0.01f, 1f)]
			public float releaseThreshold = 0.01f;

			// Token: 0x04008D81 RID: 36225
			public ContinuousPropertyArray continuousProperties;

			// Token: 0x04008D82 RID: 36226
			public UnityEvent<bool, float> unityEvent;

			// Token: 0x04008D83 RID: 36227
			[NonSerialized]
			public bool wasHeld;

			// Token: 0x04008D84 RID: 36228
			[NonSerialized]
			public bool marginError;

			// Token: 0x04008D85 RID: 36229
			private FingerFlexEvent2.FlexEvent.RangeState currentState;

			// Token: 0x04008D86 RID: 36230
			private FingerFlexEvent2.FlexEvent.RangeState lastState;

			// Token: 0x04008D87 RID: 36231
			private float lastThresholdTime = -100000f;

			// Token: 0x0200133C RID: 4924
			public enum TriggerType
			{
				// Token: 0x04008D89 RID: 36233
				OnFlex,
				// Token: 0x04008D8A RID: 36234
				OnRelease = 2
			}

			// Token: 0x0200133D RID: 4925
			public enum FingerType
			{
				// Token: 0x04008D8C RID: 36236
				Thumb,
				// Token: 0x04008D8D RID: 36237
				Index,
				// Token: 0x04008D8E RID: 36238
				Middle,
				// Token: 0x04008D8F RID: 36239
				IndexAndMiddle,
				// Token: 0x04008D90 RID: 36240
				IndexOrMiddle,
				// Token: 0x04008D91 RID: 36241
				StickLeft,
				// Token: 0x04008D92 RID: 36242
				StickRight,
				// Token: 0x04008D93 RID: 36243
				StickUp,
				// Token: 0x04008D94 RID: 36244
				StickDown
			}

			// Token: 0x0200133E RID: 4926
			public enum HandType
			{
				// Token: 0x04008D96 RID: 36246
				HeldItemHand,
				// Token: 0x04008D97 RID: 36247
				EquippedSide,
				// Token: 0x04008D98 RID: 36248
				LeftHand,
				// Token: 0x04008D99 RID: 36249
				RightHand
			}

			// Token: 0x0200133F RID: 4927
			private enum RangeState
			{
				// Token: 0x04008D9B RID: 36251
				Below,
				// Token: 0x04008D9C RID: 36252
				Within,
				// Token: 0x04008D9D RID: 36253
				Above
			}
		}
	}
}
