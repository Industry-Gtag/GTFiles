using System;
using BoingKit;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

// Token: 0x02000349 RID: 841
public class GTDoor : NetworkSceneObject
{
	// Token: 0x060014BA RID: 5306 RVA: 0x0006E874 File Offset: 0x0006CA74
	protected override void Start()
	{
		base.Start();
		Collider[] array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		this.tLastOpened = 0f;
		GTDoorTrigger[] array2 = this.doorButtonTriggers;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].TriggeredEvent.AddListener(new UnityAction(this.DoorButtonTriggered));
		}
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x0006E8E0 File Offset: 0x0006CAE0
	private void Update()
	{
		if (this.currentState == GTDoor.DoorState.Open || this.currentState == GTDoor.DoorState.Closed)
		{
			if (Time.time < this.lastChecked + this.secondsCheck)
			{
				return;
			}
			this.lastChecked = Time.time;
		}
		this.UpdateDoorState();
		this.UpdateDoorAnimation();
		Collider[] array;
		if (this.currentState == GTDoor.DoorState.Closed)
		{
			array = this.doorColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			return;
		}
		array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x0006E970 File Offset: 0x0006CB70
	private void UpdateDoorState()
	{
		this.peopleInHoldOpenVolume = false;
		foreach (GTDoorTrigger gtdoorTrigger in this.doorHoldOpenTriggers)
		{
			gtdoorTrigger.ValidateOverlappingColliders();
			if (gtdoorTrigger.overlapCount > 0)
			{
				this.peopleInHoldOpenVolume = true;
				break;
			}
		}
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
			if (this.buttonTriggeredThisFrame)
			{
				this.buttonTriggeredThisFrame = false;
				if (!NetworkSystem.Instance.InRoom)
				{
					this.OpenDoor();
				}
				else
				{
					this.currentState = GTDoor.DoorState.OpeningWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.Opening });
				}
			}
			break;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			break;
		case GTDoor.DoorState.Closing:
			if (this.doorSpring.Value < 1f)
			{
				this.currentState = GTDoor.DoorState.Closed;
			}
			if (this.peopleInHoldOpenVolume)
			{
				this.currentState = GTDoor.DoorState.HeldOpenLocally;
				if (NetworkSystem.Instance.InRoom && base.IsMine)
				{
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.HeldOpen });
				}
				this.audioSource.GTPlayOneShot(this.openSound, 1f);
			}
			break;
		case GTDoor.DoorState.Open:
			if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
			{
				if (this.peopleInHoldOpenVolume)
				{
					this.currentState = GTDoor.DoorState.HeldOpenLocally;
					if (NetworkSystem.Instance.InRoom && base.IsMine)
					{
						this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.HeldOpen });
					}
				}
				else if (!NetworkSystem.Instance.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.IsMine)
				{
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.Closing });
				}
			}
			break;
		case GTDoor.DoorState.Opening:
			if (this.doorSpring.Value > 89f)
			{
				this.currentState = GTDoor.DoorState.Open;
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			if (!this.peopleInHoldOpenVolume)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.IsMine)
				{
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
					this.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[] { GTDoor.DoorState.Closing });
				}
			}
			break;
		case GTDoor.DoorState.HeldOpenLocally:
			if (!this.peopleInHoldOpenVolume)
			{
				this.CloseDoor();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			GTDoor.DoorState doorState = this.currentState;
			if (doorState == GTDoor.DoorState.ClosingWaitingOnRPC)
			{
				this.CloseDoor();
				return;
			}
			if (doorState != GTDoor.DoorState.OpeningWaitingOnRPC)
			{
				return;
			}
			this.OpenDoor();
		}
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x0006EC14 File Offset: 0x0006CE14
	private void DoorButtonTriggered()
	{
		GTDoor.DoorState doorState = this.currentState;
		if (doorState - GTDoor.DoorState.Open <= 4)
		{
			return;
		}
		this.buttonTriggeredThisFrame = true;
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x0006EC38 File Offset: 0x0006CE38
	private void OpenDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			this.ResetDoorOpenedTime();
			this.audioSource.GTPlayOneShot(this.openSound, 1f);
			this.currentState = GTDoor.DoorState.Opening;
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x0006ECA0 File Offset: 0x0006CEA0
	private void CloseDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.Opening:
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.audioSource.GTPlayOneShot(this.closeSound, 1f);
			this.currentState = GTDoor.DoorState.Closing;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x0006ED00 File Offset: 0x0006CF00
	private void UpdateDoorAnimation()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
			return;
		}
		this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, 1f, Time.deltaTime);
		this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x0006EDDF File Offset: 0x0006CFDF
	public void ResetDoorOpenedTime()
	{
		this.tLastOpened = Time.time;
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x0006EDEC File Offset: 0x0006CFEC
	[PunRPC]
	public void ChangeDoorState(GTDoor.DoorState shouldOpenState, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "ChangeDoorState");
		this.ChangeDoorStateShared(shouldOpenState);
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x0006EE00 File Offset: 0x0006D000
	[Rpc]
	public unsafe static void RPC_ChangeDoorState(NetworkRunner runner, GTDoor.DoorState shouldOpenState, int doorId)
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage != SimulationStages.Resimulate)
			{
				int num = 8;
				num += 4;
				num += 4;
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)"));
						int num2 = 8;
						*(GTDoor.DoorState*)(ptr2 + num2) = shouldOpenState;
						num2 += 4;
						*(int*)(ptr2 + num2) = doorId;
						num2 += 4;
						ptr->Offset = num2 * 8;
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
					goto IL_0010;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)", num);
			}
			return;
		}
		IL_0010:
		GTDoor[] array = global::UnityEngine.Object.FindObjectsByType<GTDoor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		if (array == null || array.Length == 0)
		{
			return;
		}
		foreach (GTDoor gtdoor in array)
		{
			if (gtdoor.GTDoorID == doorId)
			{
				gtdoor.ChangeDoorStateShared(shouldOpenState);
			}
		}
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x0006EF58 File Offset: 0x0006D158
	private void ChangeDoorStateShared(GTDoor.DoorState shouldOpenState)
	{
		switch (shouldOpenState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.HeldOpenLocally:
			break;
		case GTDoor.DoorState.Closing:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpen:
				this.CloseDoor();
				return;
			default:
				return;
			}
			break;
		case GTDoor.DoorState.Opening:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
				this.OpenDoor();
				return;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			default:
				return;
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
				break;
			case GTDoor.DoorState.Closing:
				this.audioSource.GTPlayOneShot(this.openSound, 1f);
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpenLocally:
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			default:
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x0006F04C File Offset: 0x0006D24C
	public void SetupDoorIDs()
	{
		GTDoor[] array = global::UnityEngine.Object.FindObjectsByType<GTDoor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GTDoorID = i + 1;
		}
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x0006F0B0 File Offset: 0x0006D2B0
	[NetworkRpcStaticWeavedInvoker("System.Void GTDoor::RPC_ChangeDoorState(Fusion.NetworkRunner,GTDoor/DoorState,System.Int32)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ChangeDoorState@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		GTDoor.DoorState doorState = *(GTDoor.DoorState*)(ptr + num);
		num += 4;
		GTDoor.DoorState doorState2 = doorState;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int num3 = num2;
		NetworkBehaviourUtils.InvokeRpc = true;
		GTDoor.RPC_ChangeDoorState(runner, doorState2, num3);
	}

	// Token: 0x04001968 RID: 6504
	[SerializeField]
	private Transform doorTransform;

	// Token: 0x04001969 RID: 6505
	[SerializeField]
	private Collider[] doorColliders;

	// Token: 0x0400196A RID: 6506
	[SerializeField]
	private GTDoorTrigger[] doorButtonTriggers;

	// Token: 0x0400196B RID: 6507
	[SerializeField]
	private GTDoorTrigger[] doorHoldOpenTriggers;

	// Token: 0x0400196C RID: 6508
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400196D RID: 6509
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x0400196E RID: 6510
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x0400196F RID: 6511
	[SerializeField]
	private float doorOpenSpeed = 1f;

	// Token: 0x04001970 RID: 6512
	[SerializeField]
	private float doorCloseSpeed = 1f;

	// Token: 0x04001971 RID: 6513
	[SerializeField]
	[Range(1.5f, 10f)]
	private float timeUntilDoorCloses = 3f;

	// Token: 0x04001972 RID: 6514
	private int GTDoorID;

	// Token: 0x04001973 RID: 6515
	[DebugOption]
	private GTDoor.DoorState currentState;

	// Token: 0x04001974 RID: 6516
	private float tLastOpened;

	// Token: 0x04001975 RID: 6517
	private FloatSpring doorSpring;

	// Token: 0x04001976 RID: 6518
	[DebugOption]
	private bool peopleInHoldOpenVolume;

	// Token: 0x04001977 RID: 6519
	[DebugOption]
	private bool buttonTriggeredThisFrame;

	// Token: 0x04001978 RID: 6520
	private float lastChecked;

	// Token: 0x04001979 RID: 6521
	private float secondsCheck = 1f;

	// Token: 0x0200034A RID: 842
	public enum DoorState
	{
		// Token: 0x0400197B RID: 6523
		Closed,
		// Token: 0x0400197C RID: 6524
		ClosingWaitingOnRPC,
		// Token: 0x0400197D RID: 6525
		Closing,
		// Token: 0x0400197E RID: 6526
		Open,
		// Token: 0x0400197F RID: 6527
		OpeningWaitingOnRPC,
		// Token: 0x04001980 RID: 6528
		Opening,
		// Token: 0x04001981 RID: 6529
		HeldOpen,
		// Token: 0x04001982 RID: 6530
		HeldOpenLocally
	}
}
