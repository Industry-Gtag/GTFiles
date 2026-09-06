using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001316 RID: 4886
	public class ControllerButtonEvent : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000BFE RID: 3070
		// (get) Token: 0x06007A9C RID: 31388 RVA: 0x0027FBAA File Offset: 0x0027DDAA
		// (set) Token: 0x06007A9D RID: 31389 RVA: 0x0027FBB2 File Offset: 0x0027DDB2
		public bool IsSpawned { get; set; }

		// Token: 0x17000BFF RID: 3071
		// (get) Token: 0x06007A9E RID: 31390 RVA: 0x0027FBBB File Offset: 0x0027DDBB
		// (set) Token: 0x06007A9F RID: 31391 RVA: 0x0027FBC3 File Offset: 0x0027DDC3
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06007AA0 RID: 31392 RVA: 0x0027FBCC File Offset: 0x0027DDCC
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06007AA1 RID: 31393 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDespawn()
		{
		}

		// Token: 0x06007AA2 RID: 31394 RVA: 0x0027FBD5 File Offset: 0x0027DDD5
		private bool IsMyItem()
		{
			return this.myRig != null && this.myRig.isOfflineVRRig;
		}

		// Token: 0x06007AA3 RID: 31395 RVA: 0x0027FBF2 File Offset: 0x0027DDF2
		private void Awake()
		{
			this.triggerLastValue = 0f;
			this.gripLastValue = 0f;
			this.primaryLastValue = false;
			this.secondaryLastValue = false;
			this.frameCounter = 0;
		}

		// Token: 0x06007AA4 RID: 31396 RVA: 0x0027FC20 File Offset: 0x0027DE20
		public void LateUpdate()
		{
			if (!this.IsMyItem())
			{
				return;
			}
			XRNode xrnode = (this.inLeftHand ? XRNode.LeftHand : XRNode.RightHand);
			switch (this.buttonType)
			{
			case ControllerButtonEvent.ButtonType.trigger:
			{
				float num = ControllerInputPoller.TriggerFloat(xrnode);
				if (num > this.triggerValue)
				{
					this.frameCounter++;
				}
				if (num > this.triggerValue && this.triggerLastValue < this.triggerValue)
				{
					UnityEvent<bool, float> unityEvent = this.onButtonPressed;
					if (unityEvent != null)
					{
						unityEvent.Invoke(this.inLeftHand, num);
					}
				}
				else if (num <= this.triggerReleaseValue && this.triggerLastValue > this.triggerReleaseValue)
				{
					UnityEvent<bool, float> unityEvent2 = this.onButtonReleased;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				else if (num > this.triggerValue && this.triggerLastValue >= this.triggerValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent3 = this.onButtonPressStayed;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				this.triggerLastValue = num;
				return;
			}
			case ControllerButtonEvent.ButtonType.primary:
			{
				bool flag = ControllerInputPoller.PrimaryButtonPress(xrnode);
				if (flag)
				{
					this.frameCounter++;
				}
				if (flag && !this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent4 = this.onButtonPressed;
					if (unityEvent4 != null)
					{
						unityEvent4.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag && this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent5 = this.onButtonReleased;
					if (unityEvent5 != null)
					{
						unityEvent5.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag && this.primaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent6 = this.onButtonPressStayed;
					if (unityEvent6 != null)
					{
						unityEvent6.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.primaryLastValue = flag;
				return;
			}
			case ControllerButtonEvent.ButtonType.secondary:
			{
				bool flag2 = ControllerInputPoller.SecondaryButtonPress(xrnode);
				if (flag2)
				{
					this.frameCounter++;
				}
				if (flag2 && !this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent7 = this.onButtonPressed;
					if (unityEvent7 != null)
					{
						unityEvent7.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag2 && this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent8 = this.onButtonReleased;
					if (unityEvent8 != null)
					{
						unityEvent8.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag2 && this.secondaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent9 = this.onButtonPressStayed;
					if (unityEvent9 != null)
					{
						unityEvent9.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.secondaryLastValue = flag2;
				return;
			}
			case ControllerButtonEvent.ButtonType.grip:
			{
				float num2 = ControllerInputPoller.GripFloat(xrnode);
				if (num2 > this.gripValue)
				{
					this.frameCounter++;
				}
				if (num2 > this.gripValue && this.gripLastValue < this.gripValue)
				{
					UnityEvent<bool, float> unityEvent10 = this.onButtonPressed;
					if (unityEvent10 != null)
					{
						unityEvent10.Invoke(this.inLeftHand, num2);
					}
				}
				else if (num2 <= this.gripReleaseValue && this.gripLastValue > this.gripReleaseValue)
				{
					UnityEvent<bool, float> unityEvent11 = this.onButtonReleased;
					if (unityEvent11 != null)
					{
						unityEvent11.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				else if (num2 > this.gripValue && this.gripLastValue >= this.gripValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent12 = this.onButtonPressStayed;
					if (unityEvent12 != null)
					{
						unityEvent12.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				this.gripLastValue = num2;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x04008C29 RID: 35881
		[SerializeField]
		private float gripValue = 0.75f;

		// Token: 0x04008C2A RID: 35882
		[SerializeField]
		private float gripReleaseValue = 0.01f;

		// Token: 0x04008C2B RID: 35883
		[SerializeField]
		private float triggerValue = 0.75f;

		// Token: 0x04008C2C RID: 35884
		[SerializeField]
		private float triggerReleaseValue = 0.01f;

		// Token: 0x04008C2D RID: 35885
		[SerializeField]
		private ControllerButtonEvent.ButtonType buttonType;

		// Token: 0x04008C2E RID: 35886
		[Tooltip("How many frames should pass to trigger a press stayed button")]
		[SerializeField]
		private int frameInterval = 20;

		// Token: 0x04008C2F RID: 35887
		public UnityEvent<bool, float> onButtonPressed;

		// Token: 0x04008C30 RID: 35888
		public UnityEvent<bool, float> onButtonReleased;

		// Token: 0x04008C31 RID: 35889
		public UnityEvent<bool, float> onButtonPressStayed;

		// Token: 0x04008C32 RID: 35890
		private float triggerLastValue;

		// Token: 0x04008C33 RID: 35891
		private float gripLastValue;

		// Token: 0x04008C34 RID: 35892
		private bool primaryLastValue;

		// Token: 0x04008C35 RID: 35893
		private bool secondaryLastValue;

		// Token: 0x04008C36 RID: 35894
		private int frameCounter;

		// Token: 0x04008C37 RID: 35895
		private bool inLeftHand;

		// Token: 0x04008C38 RID: 35896
		private VRRig myRig;

		// Token: 0x02001317 RID: 4887
		private enum ButtonType
		{
			// Token: 0x04008C3C RID: 35900
			trigger,
			// Token: 0x04008C3D RID: 35901
			primary,
			// Token: 0x04008C3E RID: 35902
			secondary,
			// Token: 0x04008C3F RID: 35903
			grip
		}
	}
}
