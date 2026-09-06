using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000675 RID: 1653
internal class ConnectedControllerHandler : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x0600292E RID: 10542 RVA: 0x000DF74E File Offset: 0x000DD94E
	// (set) Token: 0x0600292F RID: 10543 RVA: 0x000DF755 File Offset: 0x000DD955
	public static ConnectedControllerHandler Instance { get; private set; }

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x06002930 RID: 10544 RVA: 0x000DF75D File Offset: 0x000DD95D
	[SerializeField]
	private bool rightValid
	{
		get
		{
			return this.overrideRightEnable || (ControllerInputPoller.instance.RightHandValid && !this.overriddenControllers.HasFlag(OverrideControllers.RightController));
		}
	}

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x06002931 RID: 10545 RVA: 0x000DF792 File Offset: 0x000DD992
	[SerializeField]
	private bool leftValid
	{
		get
		{
			return this.overrideLeftEnable || (ControllerInputPoller.instance.LeftHandValid && !this.overriddenControllers.HasFlag(OverrideControllers.LeftController));
		}
	}

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x06002932 RID: 10546 RVA: 0x000DF7C7 File Offset: 0x000DD9C7
	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x06002933 RID: 10547 RVA: 0x000DF7CF File Offset: 0x000DD9CF
	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	// Token: 0x06002934 RID: 10548 RVA: 0x000DF7D8 File Offset: 0x000DD9D8
	private void Awake()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ConnectedControllerHandler.Instance = this;
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.rightXRController == null || this.leftXRController == null || this.snapTurnController == null)
		{
			base.enabled = false;
			return;
		}
		this.rightControllerList = new List<XRController>();
		this.leftcontrollerList = new List<XRController>();
		this.rightControllerList.Add(this.rightXRController);
		this.leftcontrollerList.Add(this.leftXRController);
		this.UpdateControllerStates();
	}

	// Token: 0x06002935 RID: 10549 RVA: 0x000DF88C File Offset: 0x000DDA8C
	private void Start()
	{
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.leftXRController == null || this.rightXRController == null || this.snapTurnController == null)
		{
			return;
		}
		this.playerHandler = GTPlayer.Instance;
		this.rightHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
		this.leftHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
	}

	// Token: 0x06002936 RID: 10550 RVA: 0x000DF913 File Offset: 0x000DDB13
	public void SetRightHandOffsets(Vector3 positionOffset, Quaternion rotationOffset)
	{
		this.rightHandFollower.positionOffset = positionOffset;
		this.rightHandFollower.rotationOffset = rotationOffset;
	}

	// Token: 0x06002937 RID: 10551 RVA: 0x000DF92D File Offset: 0x000DDB2D
	public void SetLeftHandOffsets(Vector3 positionOffset, Quaternion rotationOffset)
	{
		this.leftHandFollower.positionOffset = positionOffset;
		this.leftHandFollower.rotationOffset = rotationOffset;
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x000DF947 File Offset: 0x000DDB47
	public void SetOculusOffsets(bool rightHand = true, bool leftHand = true)
	{
		if (rightHand)
		{
			this.SetRightHandOffsets(this.oculusRightPosOffset, this.oculusRightRotOffset);
		}
		if (leftHand)
		{
			this.SetLeftHandOffsets(this.oculusLeftPosOffset, this.oculusLeftRotOffset);
		}
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x000DF973 File Offset: 0x000DDB73
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x000DF97B File Offset: 0x000DDB7B
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x000DF984 File Offset: 0x000DDB84
	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x000DF9A6 File Offset: 0x000DDBA6
	private void LateUpdate()
	{
		if (!this.rightValid)
		{
			this.rightHandFollower.UpdatePositionRotation();
		}
		if (!this.leftValid)
		{
			this.leftHandFollower.UpdatePositionRotation();
		}
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x000DF9D0 File Offset: 0x000DDBD0
	public void SliceUpdate()
	{
		if (this.playerHandler.inOverlay)
		{
			return;
		}
		this.updateControllers = false;
		if (ControllerInputPoller.instance.RightHandValid)
		{
			this.tempRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
			if (this.tempRightPos == this.lastRightPos)
			{
				if (Time.time > this.timeStoppedMovingRight + this.stoppedDurationMinimum && !this.overriddenControllers.HasFlag(OverrideControllers.RightController))
				{
					this.overriddenControllers |= OverrideControllers.RightController;
					this.updateControllers = true;
				}
			}
			else
			{
				this.timeStoppedMovingRight = Time.time;
				if (this.overriddenControllers.HasFlag(OverrideControllers.RightController))
				{
					this.overriddenControllers &= ~OverrideControllers.RightController;
					this.updateControllers = true;
				}
			}
			this.lastRightPos = this.tempRightPos;
		}
		if (ControllerInputPoller.instance.LeftHandValid)
		{
			this.tempLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
			if (this.tempLeftPos == this.lastLeftPos)
			{
				if (Time.time > this.timeStoppedMovingLeft + this.stoppedDurationMinimum && !this.overriddenControllers.HasFlag(OverrideControllers.LeftController))
				{
					this.overriddenControllers |= OverrideControllers.LeftController;
					this.updateControllers = true;
				}
			}
			else
			{
				this.timeStoppedMovingLeft = Time.time;
				if (this.overriddenControllers.HasFlag(OverrideControllers.LeftController))
				{
					this.overriddenControllers &= ~OverrideControllers.LeftController;
					this.updateControllers = true;
				}
			}
			this.lastLeftPos = this.tempLeftPos;
		}
		if ((!this.leftXRController.enabled && this.leftValid) || (!this.rightXRController.enabled && this.rightValid))
		{
			this.updateControllers = true;
		}
		if (this.updateControllers)
		{
			this.overrideEnabled = this.overriddenControllers > OverrideControllers.None;
			this.UpdateControllerStates();
		}
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x000DFBB0 File Offset: 0x000DDDB0
	private void UpdateControllerStates()
	{
		this.leftXRController.enabled = this.leftValid;
		this.rightXRController.enabled = this.rightValid;
		this.AssignSnapturnController();
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x000DFBDC File Offset: 0x000DDDDC
	private void AssignSnapturnController()
	{
		if (!this.leftValid && this.rightValid)
		{
			this.snapTurnController.controllers = this.rightControllerList;
			return;
		}
		if (!this.rightValid && this.leftValid)
		{
			this.snapTurnController.controllers = this.leftcontrollerList;
			return;
		}
		this.snapTurnController.controllers = this.rightControllerList;
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x000DFC40 File Offset: 0x000DDE40
	public bool GetValidForXRNode(XRNode controllerNode)
	{
		bool flag;
		if (controllerNode != XRNode.LeftHand)
		{
			flag = controllerNode != XRNode.RightHand || this.rightValid;
		}
		else
		{
			flag = this.leftValid;
		}
		return flag;
	}

	// Token: 0x040035BA RID: 13754
	[SerializeField]
	private HandTransformFollowOffset rightHandFollower;

	// Token: 0x040035BB RID: 13755
	[SerializeField]
	private HandTransformFollowOffset leftHandFollower;

	// Token: 0x040035BC RID: 13756
	[SerializeField]
	private XRController rightXRController;

	// Token: 0x040035BD RID: 13757
	[SerializeField]
	private XRController leftXRController;

	// Token: 0x040035BE RID: 13758
	[SerializeField]
	private GorillaSnapTurn snapTurnController;

	// Token: 0x040035BF RID: 13759
	private List<XRController> rightControllerList;

	// Token: 0x040035C0 RID: 13760
	private List<XRController> leftcontrollerList;

	// Token: 0x040035C1 RID: 13761
	[SerializeField]
	private bool overrideEnabled;

	// Token: 0x040035C2 RID: 13762
	private bool overrideLeftEnable;

	// Token: 0x040035C3 RID: 13763
	private bool overrideRightEnable;

	// Token: 0x040035C4 RID: 13764
	[SerializeField]
	private Vector3 lastRightPos;

	// Token: 0x040035C5 RID: 13765
	[SerializeField]
	private Vector3 lastLeftPos;

	// Token: 0x040035C6 RID: 13766
	private Vector3 tempRightPos;

	// Token: 0x040035C7 RID: 13767
	private Vector3 tempLeftPos;

	// Token: 0x040035C8 RID: 13768
	private bool updateControllers;

	// Token: 0x040035C9 RID: 13769
	private GTPlayer playerHandler;

	// Token: 0x040035CA RID: 13770
	[Tooltip("The rate at which controllers are checked to be moving, if they not moving, overrides and enables one hand mode")]
	[SerializeField]
	private float stoppedDurationMinimum = 5f;

	// Token: 0x040035CB RID: 13771
	[SerializeField]
	private OverrideControllers overriddenControllers;

	// Token: 0x040035CC RID: 13772
	private float timeStoppedMovingLeft;

	// Token: 0x040035CD RID: 13773
	private float timeStoppedMovingRight;

	// Token: 0x040035CE RID: 13774
	public Vector3 oculusRightPosOffset = new Vector3(0f, -0.27f, 0.09f);

	// Token: 0x040035CF RID: 13775
	public Quaternion oculusRightRotOffset = Quaternion.Euler(275f, 270f, -5f);

	// Token: 0x040035D0 RID: 13776
	public Vector3 oculusLeftPosOffset = new Vector3(-0f, -0.27f, 0.09f);

	// Token: 0x040035D1 RID: 13777
	public Quaternion oculusLeftRotOffset = Quaternion.Euler(275f, 90f, 5f);
}
