using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007FA RID: 2042
public class GRShuttle : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003413 RID: 13331 RVA: 0x0011E458 File Offset: 0x0011C658
	public void Awake()
	{
		this.shuttleUI.Setup(null, null);
		if (this.entryCardScanner != null)
		{
			this.entryCardScanner.requireSpecificPlayer = true;
			this.entryCardScanner.restrictToPlayer = null;
		}
		if (this.departCardScanner != null)
		{
			this.departCardScanner.requireSpecificPlayer = true;
			this.departCardScanner.restrictToPlayer = null;
		}
		this.state = GRShuttleState.Docked;
	}

	// Token: 0x06003414 RID: 13332 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003415 RID: 13333 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003416 RID: 13334 RVA: 0x0011E4C5 File Offset: 0x0011C6C5
	public void Init(int shuttleId)
	{
		this.shuttleId = shuttleId;
		this.StopMoveFx();
	}

	// Token: 0x06003417 RID: 13335 RVA: 0x0011E4D4 File Offset: 0x0011C6D4
	public void SetBay(GRBay bay)
	{
		this.shuttleBay = bay;
	}

	// Token: 0x06003418 RID: 13336 RVA: 0x0011E4DD File Offset: 0x0011C6DD
	public void SetReactor(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06003419 RID: 13337 RVA: 0x0011E4E6 File Offset: 0x0011C6E6
	public void SetLocation(GRShuttleGroupLoc location)
	{
		this.location = location;
		this.targetSection = this.ClampTargetSection(this.targetSection);
	}

	// Token: 0x0600341A RID: 13338 RVA: 0x0011E501 File Offset: 0x0011C701
	public void Setup(GhostReactor reactor, GRShuttleGroupLoc location, int employeeIndex)
	{
		this.reactor = reactor;
		this.location = location;
		this.employeeIndex = employeeIndex;
		this.SetOwner(null);
		this.targetSection = this.ClampTargetSection(this.targetSection);
	}

	// Token: 0x0600341B RID: 13339 RVA: 0x0011E534 File Offset: 0x0011C734
	public int GetTargetFloor()
	{
		if (this.specificDestinationShuttle != null)
		{
			return this.specificDestinationShuttle.specificFloor;
		}
		if (this.targetSection < 0 || this.targetSection >= GRShuttle.sectionFloors.Length)
		{
			return 0;
		}
		return GRShuttle.sectionFloors[this.targetSection];
	}

	// Token: 0x0600341C RID: 13340 RVA: 0x0011E581 File Offset: 0x0011C781
	public GRShuttleState GetState()
	{
		return this.state;
	}

	// Token: 0x0600341D RID: 13341 RVA: 0x0011E589 File Offset: 0x0011C789
	public NetPlayer GetOwner()
	{
		return this.shuttleOwner;
	}

	// Token: 0x0600341E RID: 13342 RVA: 0x0011E594 File Offset: 0x0011C794
	public void SetOwner(NetPlayer player)
	{
		this.shuttleOwner = player;
		this.shuttleUI.Setup(this.reactor, player);
		this.entryCardScanner.restrictToPlayer = player;
		this.departCardScanner.restrictToPlayer = player;
		if (this.shuttleBay != null)
		{
			this.shuttleBay.Refresh();
		}
	}

	// Token: 0x0600341F RID: 13343 RVA: 0x0011E5EB File Offset: 0x0011C7EB
	public void SliceUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06003420 RID: 13344 RVA: 0x0011E5F3 File Offset: 0x0011C7F3
	public void Refresh()
	{
		this.shuttleUI.RefreshUI();
	}

	// Token: 0x06003421 RID: 13345 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void JoinShuttleRoomLocalPlayer(GRShuttle sourceShuttle, GRShuttle destShuttle)
	{
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x0011E600 File Offset: 0x0011C800
	public static void TeleportLocalPlayer(GRShuttle sourceShuttle, GRShuttle destShuttle)
	{
		sourceShuttle.friendCollider.RefreshPlayersWithinBounds();
		if (!sourceShuttle.friendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		VRRig localRig = VRRig.LocalRig;
		float num = destShuttle.transform.rotation.eulerAngles.y - sourceShuttle.transform.rotation.eulerAngles.y;
		Vector3 vector = localRig.transform.position - instance.transform.position;
		Vector3 vector2 = sourceShuttle.transform.InverseTransformPoint(instance.transform.position);
		vector2.x *= 0.8f;
		vector2.z *= 0.8f;
		Vector3 vector3 = destShuttle.transform.TransformPoint(vector2);
		instance.TeleportTo(vector3, instance.transform.rotation, false, false);
		instance.turnParent.transform.RotateAround(instance.headCollider.transform.position, sourceShuttle.transform.up, num);
		localRig.transform.position = instance.transform.position + vector;
		instance.InitializeValues();
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x0011E73C File Offset: 0x0011C93C
	public void SetState(GRShuttleState newState, bool force = false)
	{
		if (this.state == newState && !force)
		{
			return;
		}
		switch (this.state)
		{
		case GRShuttleState.Docked:
			if (this.shuttleBay != null)
			{
				this.shuttleBay.Refresh();
			}
			break;
		case GRShuttleState.PostMove:
			if (this.specificDestinationShuttle != null)
			{
				this.OpenDoorLocal();
			}
			else
			{
				this.CloseDoorLocal();
			}
			break;
		case GRShuttleState.PostArrive:
			this.OpenDoorLocal();
			break;
		}
		this.state = newState;
		this.stateStartTime = Time.timeAsDouble;
		switch (this.state)
		{
		case GRShuttleState.Docked:
			if (this.shuttleBay != null)
			{
				this.shuttleBay.Refresh();
			}
			this.StopMoveFx();
			break;
		case GRShuttleState.PreMove:
			this.CloseDoorLocal();
			this.takeOffSound.Play(null);
			if (this.specificDestinationShuttle != null)
			{
				GRPlayer grplayer = GRPlayer.Get(GRElevatorManager.LowestActorNumberInElevator(this.friendCollider, this.specificDestinationShuttle.friendCollider));
				this.shuttleOwner = grplayer.gamePlayer.rig.OwningNetPlayer;
			}
			GRShuttle.TryStartLocalPlayerShuttleMove(this.shuttleId, this.shuttleOwner);
			this.StartMoveFx();
			return;
		case GRShuttleState.Moving:
			this.moveSound.Play(null);
			return;
		case GRShuttleState.PostMove:
			break;
		case GRShuttleState.Arriving:
			this.CloseDoorLocal();
			this.moveSound.Play(null);
			return;
		case GRShuttleState.PostArrive:
			this.landSound.Play(null);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003424 RID: 13348 RVA: 0x0011E8B0 File Offset: 0x0011CAB0
	private void UpdateState()
	{
		double timeAsDouble = Time.timeAsDouble;
		switch (this.state)
		{
		case GRShuttleState.PreMove:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Moving, false);
				return;
			}
			break;
		case GRShuttleState.Moving:
			if (timeAsDouble > this.stateStartTime + 5.0)
			{
				this.SetState(GRShuttleState.PostMove, false);
				return;
			}
			break;
		case GRShuttleState.PostMove:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Docked, false);
				return;
			}
			break;
		case GRShuttleState.Arriving:
			if (timeAsDouble > this.stateStartTime + 2.0)
			{
				this.SetState(GRShuttleState.PostArrive, false);
				return;
			}
			break;
		case GRShuttleState.PostArrive:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Docked, false);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003425 RID: 13349 RVA: 0x0011E972 File Offset: 0x0011CB72
	public void RequestArrival()
	{
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleArrive, this.shuttleId);
	}

	// Token: 0x06003426 RID: 13350 RVA: 0x0011E98C File Offset: 0x0011CB8C
	private void StartMoveFx()
	{
		if (this.windowFx != null)
		{
			this.windowFx.Play();
		}
		for (int i = 0; i < this.hideOnMove.Count; i++)
		{
			this.hideOnMove[i].SetActive(false);
		}
		for (int j = 0; j < this.showOnMove.Count; j++)
		{
			this.showOnMove[j].SetActive(true);
		}
	}

	// Token: 0x06003427 RID: 13351 RVA: 0x0011EA04 File Offset: 0x0011CC04
	private void StopMoveFx()
	{
		if (this.windowFx != null)
		{
			this.windowFx.Stop();
		}
		for (int i = 0; i < this.hideOnMove.Count; i++)
		{
			this.hideOnMove[i].SetActive(true);
		}
		for (int j = 0; j < this.showOnMove.Count; j++)
		{
			this.showOnMove[j].SetActive(false);
		}
	}

	// Token: 0x06003428 RID: 13352 RVA: 0x0011EA7C File Offset: 0x0011CC7C
	public bool IsPodUnlocked()
	{
		if (this.specificDestinationShuttle != null)
		{
			return true;
		}
		if (this.shuttleOwner == null)
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(this.shuttleOwner);
		return !(grplayer == null) && grplayer.IsDropPodUnlocked();
	}

	// Token: 0x06003429 RID: 13353 RVA: 0x0011EAC0 File Offset: 0x0011CCC0
	public int GetMaxDropFloor()
	{
		if (this.shuttleOwner == null)
		{
			return 0;
		}
		GRPlayer grplayer = GRPlayer.Get(this.shuttleOwner);
		if (grplayer == null)
		{
			return 0;
		}
		return grplayer.GetMaxDropFloor();
	}

	// Token: 0x0600342A RID: 13354 RVA: 0x0011EAF4 File Offset: 0x0011CCF4
	public void OnShuttleMove()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleLaunch, this.shuttleId);
	}

	// Token: 0x0600342B RID: 13355 RVA: 0x0011EB18 File Offset: 0x0011CD18
	public void OnShuttleMoveActorNr(int actorNr)
	{
		if (this.state != GRShuttleState.Docked || actorNr != this.shuttleOwner.ActorNumber || this.GetTargetFloor() > this.GetMaxDropFloor())
		{
			this.departCardScanner.onFailed.Invoke();
			return;
		}
		this.departCardScanner.onSucceeded.Invoke();
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleLaunch, this.shuttleId);
	}

	// Token: 0x0600342C RID: 13356 RVA: 0x0011EB82 File Offset: 0x0011CD82
	public void TargetLevelUp()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleTargetLevelUp, this.shuttleId);
	}

	// Token: 0x0600342D RID: 13357 RVA: 0x0011EBA5 File Offset: 0x0011CDA5
	public void TargetLevelDown()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleTargetLevelDown, this.shuttleId);
	}

	// Token: 0x0600342E RID: 13358 RVA: 0x0011EBC8 File Offset: 0x0011CDC8
	private GRShuttle GetTargetShuttle()
	{
		if (this.specificDestinationShuttle != null)
		{
			return this.specificDestinationShuttle;
		}
		if (this.shuttleOwner == null)
		{
			return null;
		}
		GRShuttle drillShuttleForPlayer = GRElevatorManager._instance.GetDrillShuttleForPlayer(this.shuttleOwner.ActorNumber);
		GRShuttle stagingShuttleForPlayer = GRElevatorManager._instance.GetStagingShuttleForPlayer(this.shuttleOwner.ActorNumber);
		if (this.location != GRShuttleGroupLoc.Drill)
		{
			return drillShuttleForPlayer;
		}
		return stagingShuttleForPlayer;
	}

	// Token: 0x0600342F RID: 13359 RVA: 0x0011EC2C File Offset: 0x0011CE2C
	public bool IsPlayerOwner(GRPlayer player)
	{
		return GRPlayer.Get(this.GetOwner()) == player;
	}

	// Token: 0x06003430 RID: 13360 RVA: 0x0011EC40 File Offset: 0x0011CE40
	public bool IsShuttleInteractableByPlayer(GRPlayer player, bool ignoreOwnership)
	{
		if (!ignoreOwnership && !this.IsPlayerOwner(player) && this.specificDestinationShuttle == null)
		{
			return false;
		}
		if (this.entryCardScanner == null)
		{
			return true;
		}
		if (this.departCardScanner == null)
		{
			return true;
		}
		bool flag = GameEntityManager.IsPlayerHandNearPosition(player.gamePlayer, this.entryCardScanner.transform.position, false, true, 16f);
		bool flag2 = GameEntityManager.IsPlayerHandNearPosition(player.gamePlayer, this.departCardScanner.transform.position, false, true, 16f);
		return flag || flag2;
	}

	// Token: 0x06003431 RID: 13361 RVA: 0x0011ECD0 File Offset: 0x0011CED0
	public bool IsPlayerOwner(NetPlayer player)
	{
		return this.GetOwner() == player;
	}

	// Token: 0x06003432 RID: 13362 RVA: 0x0011ECDC File Offset: 0x0011CEDC
	public void ToggleDoor()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		if (this.entryDoor.doorState == GRDoor.DoorState.Closed)
		{
			this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleOpen, this.shuttleId);
			return;
		}
		if (this.entryDoor.doorState == GRDoor.DoorState.Open)
		{
			double timeAsDouble = Time.timeAsDouble;
			if (timeAsDouble > this.lastCloseTime + 5.0)
			{
				this.lastCloseTime = timeAsDouble;
				this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleClose, this.shuttleId);
			}
		}
	}

	// Token: 0x06003433 RID: 13363 RVA: 0x0011ED60 File Offset: 0x0011CF60
	public void ToggleDoorActorNr(int actorNr)
	{
		if (this.state == GRShuttleState.Docked && this.GetOwner() != null && this.GetOwner().ActorNumber == actorNr && GRPlayer.Get(this.shuttleOwner).IsDropPodUnlocked())
		{
			IDCardScanner idcardScanner = this.entryCardScanner;
			if (idcardScanner != null)
			{
				idcardScanner.onSucceeded.Invoke();
			}
			this.ToggleDoor();
			return;
		}
		IDCardScanner idcardScanner2 = this.entryCardScanner;
		if (idcardScanner2 == null)
		{
			return;
		}
		idcardScanner2.onFailed.Invoke();
	}

	// Token: 0x06003434 RID: 13364 RVA: 0x0011EDD0 File Offset: 0x0011CFD0
	public void EmergencyOpenDoor()
	{
		if (this.state == GRShuttleState.Docked)
		{
			if (PhotonNetwork.InRoom)
			{
				this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleOpen, this.shuttleId);
				return;
			}
			this.OpenDoorLocal();
		}
	}

	// Token: 0x06003435 RID: 13365 RVA: 0x0011EE00 File Offset: 0x0011D000
	public void OnOpenDoor()
	{
		if (this.entryDoor.doorState == GRDoor.DoorState.Closed && this.entryCardScanner != null)
		{
			this.entryCardScanner.onSucceeded.Invoke();
		}
		this.OpenDoorLocal();
	}

	// Token: 0x06003436 RID: 13366 RVA: 0x0011EE33 File Offset: 0x0011D033
	public void OpenDoorLocal()
	{
		if (this.entryDoor != null && this.entryDoor.doorState == GRDoor.DoorState.Closed)
		{
			this.entryDoor.SetDoorState(GRDoor.DoorState.Open);
		}
		if (this.shuttleBay != null)
		{
			this.shuttleBay.SetOpen(true);
		}
	}

	// Token: 0x06003437 RID: 13367 RVA: 0x0011EE70 File Offset: 0x0011D070
	public void CloseDoorLocal()
	{
		if (this.entryDoor != null && this.entryDoor.doorState == GRDoor.DoorState.Open)
		{
			this.entryDoor.SetDoorState(GRDoor.DoorState.Closed);
		}
	}

	// Token: 0x06003438 RID: 13368 RVA: 0x0011EE94 File Offset: 0x0011D094
	public void OnCloseDoor()
	{
		if (this.entryDoor.doorState == GRDoor.DoorState.Open && this.entryCardScanner != null)
		{
			this.entryCardScanner.onSucceeded.Invoke();
		}
		this.CloseDoorLocal();
	}

	// Token: 0x06003439 RID: 13369 RVA: 0x0011EEC8 File Offset: 0x0011D0C8
	public void OnLaunch()
	{
		if (this.GetTargetFloor() > this.GetMaxDropFloor())
		{
			return;
		}
		this.SetState(GRShuttleState.PreMove, false);
		if (this.departCardScanner != null)
		{
			this.departCardScanner.onSucceeded.Invoke();
		}
	}

	// Token: 0x0600343A RID: 13370 RVA: 0x0011EF04 File Offset: 0x0011D104
	public void OnArrive()
	{
		this.SetState(GRShuttleState.Arriving, false);
	}

	// Token: 0x0600343B RID: 13371 RVA: 0x0011EF0E File Offset: 0x0011D10E
	public void OnTargetLevelUp()
	{
		this.targetSection = this.ClampTargetSection(this.targetSection - 1);
		if (this.shuttleUI != null)
		{
			this.shuttleUI.RefreshUI();
		}
	}

	// Token: 0x0600343C RID: 13372 RVA: 0x0011EF37 File Offset: 0x0011D137
	public void OnTargetLevelDown()
	{
		this.targetSection = this.ClampTargetSection(this.targetSection + 1);
		if (this.shuttleUI != null)
		{
			this.shuttleUI.RefreshUI();
		}
	}

	// Token: 0x0600343D RID: 13373 RVA: 0x0011EF60 File Offset: 0x0011D160
	private int ClampTargetSection(int newTargetSection)
	{
		if (this.location == GRShuttleGroupLoc.Staging)
		{
			newTargetSection = Mathf.Clamp(newTargetSection, 1, GRShuttle.sectionFloors.Length - 1);
		}
		else
		{
			newTargetSection = 0;
		}
		return newTargetSection;
	}

	// Token: 0x0600343E RID: 13374 RVA: 0x0011EF84 File Offset: 0x0011D184
	public static void TryStartLocalPlayerShuttleMove(int currShuttleId, NetPlayer shuttleOwner)
	{
		GRPlayer local = GRPlayer.GetLocal();
		if (local == null)
		{
			return;
		}
		GRShuttle shuttle = GRElevatorManager.GetShuttle(currShuttleId);
		if (shuttle == null)
		{
			return;
		}
		if (!GRElevatorManager.IsPlayerInShuttle(local.gamePlayer.rig.OwningNetPlayer.ActorNumber, shuttle, null))
		{
			return;
		}
		if (shuttleOwner != null && shuttleOwner.GetPlayerRef() != null)
		{
			local.shuttleData.ownerUserId = shuttleOwner.UserId;
		}
		else
		{
			local.shuttleData.ownerUserId = VRRig.LocalRig.OwningNetPlayer.UserId;
		}
		local.shuttleData.currShuttleId = currShuttleId;
		local.shuttleData.targetShuttleId = -1;
		local.shuttleData.targetLevel = shuttle.GetTargetFloor();
		GRShuttle.SetPlayerShuttleState(local, GRPlayer.ShuttleState.Moving);
	}

	// Token: 0x0600343F RID: 13375 RVA: 0x0011F03C File Offset: 0x0011D23C
	public static void UpdateGRPlayerShuttle(GRPlayer player)
	{
		if (player == null)
		{
			return;
		}
		GRPlayer.ShuttleData shuttleData = player.shuttleData;
		if (shuttleData == null || shuttleData.state == GRPlayer.ShuttleState.Idle)
		{
			return;
		}
		if (!player.gamePlayer.IsLocal())
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		double num = shuttleData.stateStartTime;
		if (shuttleData.state != GRPlayer.ShuttleState.Idle && timeAsDouble > num + 10.0)
		{
			GRShuttle.CancelPlayerShuttle(player);
			return;
		}
		switch (shuttleData.state)
		{
		case GRPlayer.ShuttleState.Moving:
			if (timeAsDouble > num + 3.0)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.JoinRoom);
				return;
			}
			break;
		case GRPlayer.ShuttleState.WaitForLeaveRoom:
			if (!PhotonNetwork.InRoom)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeadPlayer);
				return;
			}
			break;
		case GRPlayer.ShuttleState.JoinRoom:
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeadPlayer);
				return;
			}
			GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeaveRoom);
			return;
		case GRPlayer.ShuttleState.WaitForLeadPlayer:
			player.shuttleData.targetShuttleId = -1;
			if (PhotonNetwork.InRoom)
			{
				player.shuttleData.targetShuttleId = GRShuttle.CalcTargetShuttleId(player.shuttleData.currShuttleId, player.shuttleData.ownerUserId);
			}
			if (player.shuttleData.targetShuttleId != -1)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Teleport);
				return;
			}
			break;
		case GRPlayer.ShuttleState.Teleport:
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId).zone);
			if (timeAsDouble > num + 1.0 && (managerForZone == null || managerForZone.IsZoneActive()))
			{
				int num2 = GRShuttle.CalcTargetShuttleId(player.shuttleData.currShuttleId, player.shuttleData.ownerUserId);
				if (num2 == player.shuttleData.targetShuttleId)
				{
					GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.PostTeleport);
					return;
				}
				if (num2 != -1)
				{
					player.shuttleData.currShuttleId = player.shuttleData.targetShuttleId;
					player.shuttleData.targetShuttleId = num2;
					GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.TeleportToMyShuttleSafety);
					return;
				}
			}
			break;
		}
		case GRPlayer.ShuttleState.TeleportToMyShuttleSafety:
			GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.PostTeleport);
			return;
		case GRPlayer.ShuttleState.PostTeleport:
			if (timeAsDouble > num + 1.0)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003440 RID: 13376 RVA: 0x0011F22C File Offset: 0x0011D42C
	public static int CalcTargetShuttleId(int currShuttleId, string ownerUserId)
	{
		GRShuttle shuttle = GRElevatorManager.GetShuttle(currShuttleId);
		if (shuttle.specificDestinationShuttle != null)
		{
			return shuttle.specificDestinationShuttle.shuttleId;
		}
		GRPlayer fromUserId = GRPlayer.GetFromUserId(ownerUserId);
		if (fromUserId != null)
		{
			bool flag = shuttle.GetTargetFloor() >= 0;
			GRShuttle assignedShuttle = fromUserId.GetAssignedShuttle(flag);
			if (assignedShuttle != null)
			{
				return assignedShuttle.shuttleId;
			}
		}
		return -1;
	}

	// Token: 0x06003441 RID: 13377 RVA: 0x0011F290 File Offset: 0x0011D490
	public static void CancelPlayerShuttle(GRPlayer player)
	{
		GRPlayer.ShuttleState shuttleState = player.shuttleData.state;
		if (shuttleState - GRPlayer.ShuttleState.Moving > 3)
		{
			if (shuttleState - GRPlayer.ShuttleState.Teleport <= 2)
			{
				GRShuttle shuttle = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
				if (shuttle != null)
				{
					shuttle.OpenDoorLocal();
				}
			}
		}
		else
		{
			GRShuttle shuttle2 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			if (shuttle2 != null)
			{
				shuttle2.OpenDoorLocal();
			}
		}
		GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Idle);
	}

	// Token: 0x06003442 RID: 13378 RVA: 0x0011F300 File Offset: 0x0011D500
	public static void SetPlayerShuttleState(GRPlayer player, GRPlayer.ShuttleState newState)
	{
		GRPlayer.ShuttleData shuttleData = player.shuttleData;
		shuttleData.state = newState;
		shuttleData.stateStartTime = Time.timeAsDouble;
		switch (shuttleData.state)
		{
		case GRPlayer.ShuttleState.Moving:
		case GRPlayer.ShuttleState.WaitForLeaveRoom:
		case GRPlayer.ShuttleState.WaitForLeadPlayer:
			break;
		case GRPlayer.ShuttleState.JoinRoom:
		{
			GRShuttle shuttle = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle targetShuttle = shuttle.GetTargetShuttle();
			if (targetShuttle != null && !NetworkSystem.Instance.SessionIsPrivate && shuttle.shuttleOwner.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				GRElevatorManager.LeadShuttleJoin(shuttle.friendCollider, targetShuttle.friendCollider, targetShuttle.joinTrigger, shuttle.GetTargetFloor());
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.Teleport:
		{
			GRShuttle shuttle2 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle shuttle3 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle3 != null)
			{
				GRShuttle.TeleportLocalPlayer(shuttle2, shuttle3);
				shuttle3.CloseDoorLocal();
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.TeleportToMyShuttleSafety:
		{
			GRShuttle shuttle4 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle shuttle5 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle5 != null)
			{
				GRShuttle.TeleportLocalPlayer(shuttle4, shuttle5);
				shuttle5.CloseDoorLocal();
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.PostTeleport:
		{
			GRShuttle shuttle6 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle6 != null)
			{
				shuttle6.RequestArrival();
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x040043EE RID: 17390
	public const int InvalidId = -1;

	// Token: 0x040043EF RID: 17391
	private const int MAX_DEPTH = 29;

	// Token: 0x040043F0 RID: 17392
	public GTZone zone;

	// Token: 0x040043F1 RID: 17393
	public GRShuttleUI shuttleUI;

	// Token: 0x040043F2 RID: 17394
	public GRDoor entryDoor;

	// Token: 0x040043F3 RID: 17395
	private GRShuttleGroupLoc location;

	// Token: 0x040043F4 RID: 17396
	private int employeeIndex;

	// Token: 0x040043F5 RID: 17397
	public AbilitySound takeOffSound;

	// Token: 0x040043F6 RID: 17398
	public AbilitySound moveSound;

	// Token: 0x040043F7 RID: 17399
	public AbilitySound landSound;

	// Token: 0x040043F8 RID: 17400
	public GorillaFriendCollider friendCollider;

	// Token: 0x040043F9 RID: 17401
	public GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x040043FA RID: 17402
	public GRShuttle specificDestinationShuttle;

	// Token: 0x040043FB RID: 17403
	public int specificFloor = -1;

	// Token: 0x040043FC RID: 17404
	public ParticleSystem windowFx;

	// Token: 0x040043FD RID: 17405
	public List<GameObject> hideOnMove;

	// Token: 0x040043FE RID: 17406
	public List<GameObject> showOnMove;

	// Token: 0x040043FF RID: 17407
	public BoxCollider inShuttleVolume;

	// Token: 0x04004400 RID: 17408
	public IDCardScanner entryCardScanner;

	// Token: 0x04004401 RID: 17409
	public IDCardScanner departCardScanner;

	// Token: 0x04004402 RID: 17410
	[NonSerialized]
	public int shuttleId;

	// Token: 0x04004403 RID: 17411
	private GhostReactor reactor;

	// Token: 0x04004404 RID: 17412
	private int targetSection;

	// Token: 0x04004405 RID: 17413
	private GRShuttleState state;

	// Token: 0x04004406 RID: 17414
	private double stateStartTime;

	// Token: 0x04004407 RID: 17415
	private GRBay shuttleBay;

	// Token: 0x04004408 RID: 17416
	private NetPlayer shuttleOwner;

	// Token: 0x04004409 RID: 17417
	private double lastCloseTime;

	// Token: 0x0400440A RID: 17418
	private static int[] sectionFloors = new int[] { -1, 0, 4, 9, 14, 19, 24, 29 };
}
