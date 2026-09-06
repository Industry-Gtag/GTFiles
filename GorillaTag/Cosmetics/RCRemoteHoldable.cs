using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012D9 RID: 4825
	public class RCRemoteHoldable : TransferrableObject, ISnapTurnOverride
	{
		// Token: 0x17000BBD RID: 3005
		// (get) Token: 0x060078DF RID: 30943 RVA: 0x002762BC File Offset: 0x002744BC
		public XRNode XRNode
		{
			get
			{
				return this.xrNode;
			}
		}

		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x060078E0 RID: 30944 RVA: 0x002762C4 File Offset: 0x002744C4
		public RCVehicle Vehicle
		{
			get
			{
				return this.targetVehicle;
			}
		}

		// Token: 0x060078E1 RID: 30945 RVA: 0x002762CC File Offset: 0x002744CC
		public bool TurnOverrideActive()
		{
			return base.gameObject.activeSelf && this.currentlyHeld && this.xrNode == XRNode.RightHand;
		}

		// Token: 0x060078E2 RID: 30946 RVA: 0x002762F0 File Offset: 0x002744F0
		protected override void Awake()
		{
			base.Awake();
			this.initialJoystickRotation = this.joystickTransform.localRotation;
			this.initialTriggerRotation = this.triggerTransform.localRotation;
			if (this.buttonTransform != null)
			{
				this.initialButtonRotation = this.buttonTransform.localRotation;
				this.initialButtonPosition = this.buttonTransform.localPosition;
			}
		}

		// Token: 0x060078E3 RID: 30947 RVA: 0x00276358 File Offset: 0x00274558
		internal override void OnEnable()
		{
			base.OnEnable();
			if (!this._TryFindRemoteVehicle())
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (this._events.IsNotNull() || base.gameObject.TryGetComponent<RubberDuckEvents>(out this._events))
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
				this._events.Activate += this.OnStartConnectionEvent;
			}
			this.WakeUpRemoteVehicle();
		}

		// Token: 0x060078E4 RID: 30948 RVA: 0x00276448 File Offset: 0x00274648
		internal override void OnDisable()
		{
			base.OnDisable();
			GorillaSnapTurn gorillaSnapTurn = ((GorillaTagger.Instance != null) ? GorillaTagger.Instance.GetComponent<GorillaSnapTurn>() : null);
			if (gorillaSnapTurn != null)
			{
				gorillaSnapTurn.UnsetTurningOverride(this);
			}
			if (this.networkSync != null && this.networkSync.photonView.IsMine)
			{
				PhotonNetwork.Destroy(this.networkSync.gameObject);
				this.networkSync = null;
			}
			if (this._events.IsNotNull())
			{
				this._events.Activate -= this.OnStartConnectionEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x060078E5 RID: 30949 RVA: 0x00276500 File Offset: 0x00274700
		protected override void OnDestroy()
		{
			base.OnDestroy();
			GorillaSnapTurn gorillaSnapTurn = ((GorillaTagger.Instance != null) ? GorillaTagger.Instance.GetComponent<GorillaSnapTurn>() : null);
			if (gorillaSnapTurn != null)
			{
				gorillaSnapTurn.UnsetTurningOverride(this);
			}
		}

		// Token: 0x060078E6 RID: 30950 RVA: 0x00276540 File Offset: 0x00274740
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			if (PhotonNetwork.InRoom && this.networkSync != null && this.networkSync.photonView.Owner == null)
			{
				PhotonNetwork.Destroy(this.networkSync.gameObject);
				this.networkSync = null;
			}
			if (this.networkSync == null && PhotonNetwork.InRoom)
			{
				object[] array = new object[] { this.myIndex };
				GameObject gameObject = PhotonNetwork.Instantiate(this.networkSyncPrefabName, Vector3.zero, Quaternion.identity, 0, array);
				this.networkSync = ((gameObject != null) ? gameObject.GetComponent<RCCosmeticNetworkSync>() : null);
			}
			this.currentlyHeld = true;
			bool flag = grabbingHand == EquipmentInteractor.instance.rightHand;
			this.xrNode = (flag ? XRNode.RightHand : XRNode.LeftHand);
			GorillaSnapTurn component = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			if (flag)
			{
				component.SetTurningOverride(this);
			}
			else
			{
				component.UnsetTurningOverride(this);
			}
			if (this.targetVehicle != null)
			{
				this.targetVehicle.StartConnection(this, this.networkSync);
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(this.emptyArgs);
			}
		}

		// Token: 0x060078E7 RID: 30951 RVA: 0x00276690 File Offset: 0x00274890
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.currentlyHeld = false;
			this.currentInput = default(RCRemoteHoldable.RCInput);
			if (this.targetVehicle != null)
			{
				this.targetVehicle.EndConnection();
			}
			this.joystickTransform.localRotation = this.initialJoystickRotation;
			this.triggerTransform.localRotation = this.initialTriggerRotation;
			GorillaTagger.Instance.GetComponent<GorillaSnapTurn>().UnsetTurningOverride(this);
			return true;
		}

		// Token: 0x060078E8 RID: 30952 RVA: 0x00276708 File Offset: 0x00274908
		private void Update()
		{
			if (this.currentlyHeld)
			{
				this.currentInput.joystick = ControllerInputPoller.Primary2DAxis(this.xrNode);
				this.currentInput.trigger = ControllerInputPoller.TriggerFloat(this.xrNode);
				this.currentInput.buttons = (ControllerInputPoller.PrimaryButtonPress(this.xrNode) ? 1 : 0);
				if (this.targetVehicle != null)
				{
					this.targetVehicle.ApplyRemoteControlInput(this.currentInput);
				}
				this.joystickTransform.localRotation = this.initialJoystickRotation * Quaternion.Euler(this.joystickLeanDegrees * this.currentInput.joystick.y, 0f, -this.joystickLeanDegrees * this.currentInput.joystick.x);
				this.triggerTransform.localRotation = this.initialTriggerRotation * Quaternion.Euler(this.triggerPullDegrees * this.currentInput.trigger, 0f, 0f);
				if (this.buttonTransform != null)
				{
					this.buttonTransform.localPosition = this.initialButtonPosition + this.initialButtonRotation * new Vector3(0f, 0f, -this.buttonPressDepth * (float)((this.currentInput.buttons > 0) ? 1 : 0));
				}
			}
		}

		// Token: 0x060078E9 RID: 30953 RVA: 0x00276867 File Offset: 0x00274A67
		public void OnStartConnectionEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			this.WakeUpRemoteVehicle();
		}

		// Token: 0x060078EA RID: 30954 RVA: 0x0027688E File Offset: 0x00274A8E
		public void WakeUpRemoteVehicle()
		{
			if (this.networkSync != null && this.targetVehicle.IsNotNull() && !this.targetVehicle.HasLocalAuthority)
			{
				this.targetVehicle.WakeUpRemote(this.networkSync);
			}
		}

		// Token: 0x060078EB RID: 30955 RVA: 0x002768CC File Offset: 0x00274ACC
		private bool _TryFindRemoteVehicle()
		{
			if (this.targetVehicle != null)
			{
				return true;
			}
			VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
			if (componentInParent.IsNull())
			{
				Debug.LogError("RCRemoteHoldable: unable to find parent vrrig");
				return false;
			}
			CosmeticItemInstance cosmeticItemInstance = componentInParent.cosmeticsObjectRegistry.Cosmetic(base.name);
			if (cosmeticItemInstance == null)
			{
				return false;
			}
			int instanceID = base.gameObject.GetInstanceID();
			return this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.objects) || this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.leftObjects) || this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.rightObjects);
		}

		// Token: 0x060078EC RID: 30956 RVA: 0x0027695C File Offset: 0x00274B5C
		private bool _TryFindRemoteVehicle_InCosmeticInstanceArray(int thisGobjInstId, List<GameObject> gameObjects)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				if (gameObject.GetInstanceID() != thisGobjInstId)
				{
					this.targetVehicle = gameObject.GetComponentInChildren<RCVehicle>(true);
					if (this.targetVehicle != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x040089CE RID: 35278
		[SerializeField]
		private Transform joystickTransform;

		// Token: 0x040089CF RID: 35279
		[SerializeField]
		private Transform triggerTransform;

		// Token: 0x040089D0 RID: 35280
		[SerializeField]
		private Transform buttonTransform;

		// Token: 0x040089D1 RID: 35281
		private RCVehicle targetVehicle;

		// Token: 0x040089D2 RID: 35282
		private float joystickLeanDegrees = 30f;

		// Token: 0x040089D3 RID: 35283
		private float triggerPullDegrees = 40f;

		// Token: 0x040089D4 RID: 35284
		private float buttonPressDepth = 0.005f;

		// Token: 0x040089D5 RID: 35285
		private Quaternion initialJoystickRotation;

		// Token: 0x040089D6 RID: 35286
		private Quaternion initialTriggerRotation;

		// Token: 0x040089D7 RID: 35287
		private Quaternion initialButtonRotation;

		// Token: 0x040089D8 RID: 35288
		private Vector3 initialButtonPosition;

		// Token: 0x040089D9 RID: 35289
		private bool currentlyHeld;

		// Token: 0x040089DA RID: 35290
		private XRNode xrNode;

		// Token: 0x040089DB RID: 35291
		private RCRemoteHoldable.RCInput currentInput;

		// Token: 0x040089DC RID: 35292
		[HideInInspector]
		public RCCosmeticNetworkSync networkSync;

		// Token: 0x040089DD RID: 35293
		private string networkSyncPrefabName = "RCCosmeticNetworkSync";

		// Token: 0x040089DE RID: 35294
		private RubberDuckEvents _events;

		// Token: 0x040089DF RID: 35295
		private object[] emptyArgs = new object[0];

		// Token: 0x020012DA RID: 4826
		public struct RCInput
		{
			// Token: 0x040089E0 RID: 35296
			public Vector2 joystick;

			// Token: 0x040089E1 RID: 35297
			public float trigger;

			// Token: 0x040089E2 RID: 35298
			public byte buttons;
		}
	}
}
