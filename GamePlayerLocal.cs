using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020006F6 RID: 1782
public class GamePlayerLocal : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06002D0D RID: 11533 RVA: 0x000F2C98 File Offset: 0x000F0E98
	private void Awake()
	{
		GamePlayerLocal.instance = this;
		this.hands = new GamePlayerLocal.HandData[2];
		this.inputData = new GamePlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GamePlayerLocal.InputData(32);
		}
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinRoom);
		GamePlayerLocal._LoadSnappedPlayerPrefsToCache(this.gamePlayer);
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x000F2D0D File Offset: 0x000F0F0D
	private void OnJoinRoom()
	{
		this.gamePlayer.MigrateHeldActorNumbers();
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x000F2D1C File Offset: 0x000F0F1C
	public void OnUpdateInteract()
	{
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(this.currGameEntityManager, j);
		}
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x000F2D64 File Offset: 0x000F0F64
	public void DebugSlotsReport(string header)
	{
		try
		{
			string text = "[SlotsReport] {0} | currManager={1} localActor={2} | slots: ";
			GameEntityManager gameEntityManager = this.currGameEntityManager;
			object obj = ((gameEntityManager != null) ? new GTZone?(gameEntityManager.zone) : null);
			Player localPlayer = PhotonNetwork.LocalPlayer;
			string text2 = string.Format(text, header, obj, (localPlayer != null) ? localPlayer.ActorNumber : (-1));
			for (int i = 0; i < 4; i++)
			{
				GameEntity gameEntity;
				GamePlayer.SlotData slotData;
				if (this.gamePlayer.TryGetSlotEntity(i, out gameEntity))
				{
					if (gameEntity != null)
					{
						string text3 = text2;
						string text4 = "[{0}: id={1} '{2}' type={3} mgr={4}] ";
						object[] array = new object[5];
						array[0] = i;
						array[1] = gameEntity.id.index;
						array[2] = gameEntity.name;
						array[3] = gameEntity.typeId;
						int num = 4;
						GameEntityManager manager = gameEntity.manager;
						array[num] = ((manager != null) ? new GTZone?(manager.zone) : null);
						text2 = text3 + string.Format(text4, array);
					}
					else
					{
						text2 += string.Format("[{0}: STALE entity returned by TryGetSlotEntity!] ", i);
					}
				}
				else if (this.gamePlayer.TryGetSlotData(i, out slotData))
				{
					string text5 = text2;
					string text6 = "[{0}: rawId={1} mgr={2} ORPHANED_SLOT_DATA] ";
					object obj2 = i;
					object obj3 = slotData.entityId.index;
					GameEntityManager entityManager = slotData.entityManager;
					text2 = text5 + string.Format(text6, obj2, obj3, (entityManager != null) ? new GTZone?(entityManager.zone) : null);
				}
				else
				{
					text2 += string.Format("[{0}: empty] ", i);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x000F2F0C File Offset: 0x000F110C
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GamePlayerLocal.InputDataMotion inputDataMotion = default(GamePlayerLocal.InputDataMotion);
		inputDataMotion.position = ControllerInputPoller.DevicePosition(xrnode);
		inputDataMotion.rotation = ControllerInputPoller.DeviceRotation(xrnode);
		inputDataMotion.velocity = ControllerInputPoller.DeviceVelocity(xrnode);
		inputDataMotion.angVelocity = ControllerInputPoller.DeviceAngularVelocity(xrnode);
		inputDataMotion.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(inputDataMotion);
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x000F2F78 File Offset: 0x000F1178
	private void UpdateHand(GameEntityManager emptyHandManager, int handIndex)
	{
		GameEntityManager gameEntityManager;
		if (!this.gamePlayer.GetGrabbedGameEntityIdAndManager(handIndex, out gameEntityManager).IsValid())
		{
			this.UpdateHandEmpty(emptyHandManager, handIndex);
			return;
		}
		this.UpdateHandHolding(gameEntityManager, handIndex);
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x000F2FB0 File Offset: 0x000F11B0
	public void MigrateToEntityManager(GameEntityManager newEntityManager)
	{
		if (this.currGameEntityManager == newEntityManager && !this.pendingFullMigration)
		{
			return;
		}
		this.pendingFullMigration = false;
		this.DebugSlotsReport(string.Format("Pre-Migrate to zone={0}", (newEntityManager != null) ? new GTZone?(newEntityManager.zone) : null));
		for (int i = 0; i < 4; i++)
		{
			GamePlayer.SlotData slotData;
			if (this.gamePlayer.TryGetSlotData(i, out slotData) && !(slotData.entityManager == null) && !(slotData.entityManager == newEntityManager))
			{
				GameEntity gameEntity = slotData.entityManager.GetGameEntity(slotData.entityId);
				if (!(gameEntity == null) && gameEntity.IsScenePlaced)
				{
					slotData.entityManager.ReleaseScenePlacedHold(gameEntity);
					this.gamePlayer.ClearSlot(i);
					if (GamePlayer.IsGrabSlot(i))
					{
						this.ClearGrabbed(i);
					}
				}
			}
		}
		if (newEntityManager.IsAuthority())
		{
			this.gamePlayer.AuthorityMigrateToEntityManager(newEntityManager);
		}
		this.currGameEntityManager = newEntityManager;
		List<GameEntityCreateData> list;
		if (this.joinWithItemsSentForCurrentMigration)
		{
			this.joinWithItemsSentForCurrentMigration = false;
		}
		else if (GamePlayerLocal.TryGetMigrationRecoveryList(newEntityManager, out list))
		{
			this.currGameEntityManager.RequestMigrationRecovery(list);
		}
		this.DebugSlotsReport(string.Format("Post-Migrate to zone={0}", (newEntityManager != null) ? new GTZone?(newEntityManager.zone) : null));
	}

	// Token: 0x06002D14 RID: 11540 RVA: 0x000F3100 File Offset: 0x000F1300
	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = (gameBallId.IsValid() ? 0.0 : handData.gripPressedTime);
		this.hands[handIndex] = handData;
		if (handIndex == 0)
		{
			EquipmentInteractor.instance.disableLeftGrab = gameBallId.IsValid();
			return;
		}
		EquipmentInteractor.instance.disableRightGrab = gameBallId.IsValid();
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x000F3174 File Offset: 0x000F1374
	public void ClearGrabbedIfHeld(GameEntityId gameBallId, GameEntityManager manager)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.gamePlayer.IsInSlot(i, gameBallId.index, manager))
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002D16 RID: 11542 RVA: 0x000F31A9 File Offset: 0x000F13A9
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex);
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x000F31B8 File Offset: 0x000F13B8
	private void UpdateStuckState()
	{
		bool flag = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGrabbedGameEntityId(i).IsValid())
			{
				flag = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = flag;
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x000F3200 File Offset: 0x000F1400
	private void UpdateHandEmpty(GameEntityManager gameEntityManager, int handIndex)
	{
		if (gameEntityManager == null)
		{
			bool flag = (GamePlayer.IsLeftHand(handIndex) ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand)));
			if (flag)
			{
				bool gripWasHeld = this.hands[handIndex].gripWasHeld;
			}
			GamePlayerLocal.HandData handData = this.hands[handIndex];
			handData.gripWasHeld = flag;
			this.hands[handIndex] = handData;
			return;
		}
		if (this.gamePlayer.IsGrabbingDisabled())
		{
			return;
		}
		GamePlayerLocal.HandData handData2 = this.hands[handIndex];
		bool flag2 = GamePlayer.IsLeftHand(handIndex);
		bool flag3 = (flag2 ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand)));
		double timeAsDouble = Time.timeAsDouble;
		bool flag4 = flag3 && !handData2.gripWasHeld;
		if (flag4)
		{
			handData2.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData2.gripPressedTime;
		handData2.gripWasHeld = flag3;
		bool flag5 = (flag2 ? ControllerInputPoller.GetIndexPressed(XRNode.LeftHand) : ControllerInputPoller.GetIndexPressed(XRNode.RightHand));
		if (flag5 && !handData2.gripWasHeld)
		{
			handData2.triggerPressedTime = timeAsDouble;
		}
		double num2 = timeAsDouble - handData2.triggerPressedTime;
		handData2.triggerWasHeld = flag5;
		this.hands[handIndex] = handData2;
		if (flag3 && num < 0.15000000596046448)
		{
			Transform handTransform = this.gamePlayer.GetHandTransform(handIndex);
			Vector3 position = handTransform.position;
			Vector3 vector = Vector3.Lerp(position, this.GetFingerTransform(handIndex).position, 0.5f);
			Vector3 vector2 = position;
			Quaternion rotation = handTransform.rotation;
			bool flag6;
			GameEntityId gameEntityId = gameEntityManager.TryGrabLocal(position, vector, flag2, out vector2, out flag6);
			if (flag4)
			{
				if (gameEntityId.IsValid())
				{
					gameEntityManager.GetGameEntity(gameEntityId);
				}
				else
				{
					gameEntityManager.LogGrabDiagnostics(position, flag2, handIndex);
				}
			}
			if (gameEntityId.IsValid())
			{
				Vector3 vector3 = (flag6 ? vector : position);
				Transform transform = handTransform;
				GameEntity gameEntity = gameEntityManager.GetGameEntity(gameEntityId);
				Vector3 vector4 = gameEntity.transform.position + (vector3 - vector2);
				Quaternion quaternion = gameEntity.transform.rotation;
				GameGrabbable component = gameEntity.GetComponent<GameGrabbable>();
				GameGrab gameGrab;
				if (component && component.GetBestGrabPoint(position, rotation, handIndex, out gameGrab))
				{
					vector4 = gameGrab.position;
					quaternion = gameGrab.rotation;
				}
				Vector3 vector5 = transform.InverseTransformPoint(vector4);
				Quaternion quaternion2 = Quaternion.Inverse(transform.rotation) * quaternion;
				gameEntityManager.RequestGrabEntity(gameEntityId, flag2, vector5, quaternion2);
				if (gameEntity.GetComponent<GameEntityDelayedDestroy>() == null)
				{
					GamePlayerLocal.SetGrabSlotRecoveryData(handIndex, gameEntity.typeId, gameEntity.createData, vector5, quaternion2);
					int num3 = 1 - handIndex;
					if (GamePlayer.IsGrabSlot(num3) && GamePlayerLocal.slotsRecoveryData[num3].entityTypeId == gameEntity.typeId)
					{
						GamePlayerLocal.SetSlotRecoveryData(num3, -1, 0L);
					}
				}
			}
		}
		if (flag5 && num2 < 0.15000000596046448)
		{
			Vector3 position2 = this.gamePlayer.GetHandTransform(handIndex).position;
			GameTriggerInteractable gameTriggerInteractable = null;
			float num4 = float.MaxValue;
			int num5 = 0;
			while (num5 < GameTriggerInteractable.LocalInteractableTriggers.Count && !GameTriggerInteractable.LocalInteractableTriggers[num5].triggerInteractionActive)
			{
				if (GameTriggerInteractable.LocalInteractableTriggers[num5].PointWithinInteractableArea(position2))
				{
					float magnitude = (GameTriggerInteractable.LocalInteractableTriggers[num5].interactableCenter.position - position2).magnitude;
					if (magnitude <= num4)
					{
						num4 = magnitude;
						gameTriggerInteractable = GameTriggerInteractable.LocalInteractableTriggers[num5];
					}
				}
				num5++;
			}
			if (gameTriggerInteractable != null)
			{
				gameTriggerInteractable.BeginTriggerInteraction(handIndex);
			}
		}
		if (!flag5)
		{
			this.ClearTriggerInteractables(handIndex);
		}
	}

	// Token: 0x06002D19 RID: 11545 RVA: 0x000F35BC File Offset: 0x000F17BC
	private void UpdateHandHolding(GameEntityManager gameEntityManager, int handIndex)
	{
		if (gameEntityManager == null)
		{
			return;
		}
		XRNode xrnode = this.GetXRNode(handIndex);
		bool flag = GamePlayer.IsLeftHand(handIndex);
		if (!(flag ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand))))
		{
			GameEntityId grabbedGameEntityId = this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(grabbedGameEntityId);
			GamePlayerLocal.SetSlotRecoveryData(handIndex, -1, 0L);
			GameSnappable component = gameEntity.GetComponent<GameSnappable>();
			if (component != null)
			{
				SuperInfectionSnapPoint superInfectionSnapPoint = component.BestSnapPoint();
				if (superInfectionSnapPoint != null)
				{
					gameEntityManager.RequestSnapEntity(grabbedGameEntityId, flag, superInfectionSnapPoint.jointType);
					int num;
					if (gameEntity.GetComponent<GameEntityDelayedDestroy>() == null && GameSnappable.TryGetJointToSnapIndex(superInfectionSnapPoint.jointType, out num))
					{
						GamePlayerLocal.SetSlotRecoveryData(num, gameEntity.typeId, gameEntity.createData);
						GamePlayerLocal.SaveSnapSlotsRateLimited();
					}
					return;
				}
			}
			GameDockable component2 = gameEntity.GetComponent<GameDockable>();
			if (component2 != null)
			{
				GameEntityId gameEntityId = component2.BestDock();
				if (gameEntityId != GameEntityId.Invalid)
				{
					Transform dockablePoint = component2.GetDockablePoint();
					Quaternion quaternion = Quaternion.Inverse(Quaternion.Inverse(component2.transform.rotation) * dockablePoint.rotation);
					Vector3 vector = quaternion * -component2.transform.InverseTransformPoint(dockablePoint.position);
					GameEntity gameEntity2 = gameEntityManager.GetGameEntity(gameEntityId);
					if (gameEntity2 != null)
					{
						GameDock component3 = gameEntity2.GetComponent<GameDock>();
						if (component3 != null)
						{
							Transform dockMarker = component3.dockMarker;
							Vector3 vector2 = dockMarker.transform.TransformPoint(vector);
							vector = gameEntity2.transform.InverseTransformPoint(vector2);
							Quaternion quaternion2 = dockMarker.rotation * quaternion;
							quaternion = Quaternion.Inverse(gameEntity2.transform.rotation) * quaternion2;
						}
					}
					gameEntityManager.RequestAttachEntity(grabbedGameEntityId, gameEntityId, 0, vector, quaternion);
					return;
				}
			}
			Vector3 vector3 = ControllerInputPoller.DeviceAngularVelocity(xrnode);
			Quaternion quaternion3 = ControllerInputPoller.DeviceRotation(xrnode);
			Quaternion handRotOffset = GTPlayer.Instance.GetHandRotOffset(flag);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
			GamePlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector4 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector4 = rotation * vector4;
			vector4 *= transform.localScale.x;
			vector3 = rotation * quaternion3 * handRotOffset * vector3;
			this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector4 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			gameEntityManager.RequestThrowEntity(grabbedGameEntityId, flag, GTPlayer.Instance.HeadCenterPosition, vector4, vector3);
		}
		this.ClearTriggerInteractables(handIndex);
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x000CB5D4 File Offset: 0x000C97D4
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x000F38A8 File Offset: 0x000F1AA8
	private Transform GetFingerTransform(int handIndex)
	{
		GorillaTagger gorillaTagger = GorillaTagger.Instance;
		Transform transform;
		if (handIndex != 0)
		{
			if (handIndex != 1)
			{
				transform = null;
			}
			else
			{
				transform = gorillaTagger.rightHandTriggerCollider.transform;
			}
		}
		else
		{
			transform = gorillaTagger.leftHandTriggerCollider.transform;
		}
		return transform;
	}

	// Token: 0x06002D1C RID: 11548 RVA: 0x000F38E4 File Offset: 0x000F1AE4
	public Vector3 GetHandVelocity(int handIndex)
	{
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		GamePlayerLocal.InputData inputData = this.inputData[handIndex];
		Vector3 vector = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
		vector = rotation * vector;
		return vector * base.transform.localScale.x;
	}

	// Token: 0x06002D1D RID: 11549 RVA: 0x000F395C File Offset: 0x000F1B5C
	public Vector3 GetHandAngularVelocity(int handIndex)
	{
		object obj = ((handIndex == 0) ? 4 : 5);
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		object obj2 = obj;
		Quaternion quaternion = ControllerInputPoller.DeviceRotation(obj2);
		Vector3 vector = ControllerInputPoller.DeviceAngularVelocity(obj2);
		return rotation * -(Quaternion.Inverse(quaternion) * vector);
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x000F39AB File Offset: 0x000F1BAB
	public float GetHandSpeed(int handIndex)
	{
		return this.inputData[handIndex].GetMaxSpeed(0f, 0.05f);
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x000F39C4 File Offset: 0x000F1BC4
	public static bool IsHandHolding(XRNode xrNode)
	{
		return GamePlayerLocal.instance.gamePlayer.IsSlotOccupied((xrNode == XRNode.LeftHand) ? 0 : 1);
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x000CB60F File Offset: 0x000C980F
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x06002D21 RID: 11553 RVA: 0x000CB62B File Offset: 0x000C982B
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x06002D22 RID: 11554 RVA: 0x000F39E0 File Offset: 0x000F1BE0
	public void ClearTriggerInteractables(int handIndex)
	{
		for (int i = 0; i < GameTriggerInteractable.LocalInteractableTriggers.Count; i++)
		{
			if (GameTriggerInteractable.LocalInteractableTriggers[i].triggerInteractionActive && GameTriggerInteractable.LocalInteractableTriggers[i].handIndex == handIndex)
			{
				GameTriggerInteractable.LocalInteractableTriggers[i].EndTriggerInteraction();
			}
		}
	}

	// Token: 0x06002D23 RID: 11555 RVA: 0x000F3A38 File Offset: 0x000F1C38
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetSlotRecoveryData(int slot, int typeId, long createData)
	{
		if (!GamePlayer.IsSlot(slot))
		{
			return;
		}
		if (typeId == -2147483647)
		{
			return;
		}
		GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[slot];
		slotRecoveryData.entityTypeId = typeId;
		slotRecoveryData.createData = createData;
		GamePlayerLocal.slotsRecoveryData[slot] = slotRecoveryData;
	}

	// Token: 0x06002D24 RID: 11556 RVA: 0x000F3A80 File Offset: 0x000F1C80
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetGrabSlotRecoveryData(int slot, int typeId, long createData, Vector3 pos, Quaternion rot)
	{
		if (!GamePlayer.IsGrabSlot(slot))
		{
			return;
		}
		GamePlayerLocal.SetSlotRecoveryData(slot, typeId, createData);
		GamePlayerLocal.GrabSlotExtraRecoveryData grabSlotExtraRecoveryData = GamePlayerLocal.grabSlotsExtraRecoveryData[slot];
		grabSlotExtraRecoveryData.pos = pos;
		grabSlotExtraRecoveryData.rot = rot;
		GamePlayerLocal.grabSlotsExtraRecoveryData[slot] = grabSlotExtraRecoveryData;
	}

	// Token: 0x06002D25 RID: 11557 RVA: 0x000F3AC7 File Offset: 0x000F1CC7
	internal static void SaveSnapSlotsRateLimited()
	{
		if (GamePlayerLocal.snapSlotsSave_isQueued)
		{
			return;
		}
		if (GamePlayerLocal.snapSlotsSave_lastTime + 2f < Time.unscaledTime)
		{
			GamePlayerLocal._SaveSnapSlotsImmediately();
			return;
		}
		GamePlayerLocal.snapSlotsSave_isQueued = true;
		GTDelayedExec.Add(GamePlayerLocal.instance, 2f, 0);
	}

	// Token: 0x06002D26 RID: 11558 RVA: 0x000F3B01 File Offset: 0x000F1D01
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		GamePlayerLocal._SaveSnapSlotsImmediately();
	}

	// Token: 0x06002D27 RID: 11559 RVA: 0x000F3B08 File Offset: 0x000F1D08
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _SaveSnapSlotsImmediately()
	{
		GamePlayerLocal.snapSlotsSave_isQueued = false;
		GamePlayerLocal.snapSlotsSave_lastTime = Time.unscaledTime;
		int num = GamePlayerLocal._SnapSlotsSave_GetHash(GamePlayerLocal.slotsRecoveryData);
		if (num == GamePlayerLocal.snapSlotsSave_lastSavedHash)
		{
			return;
		}
		GamePlayerLocal.snapSlotsSave_lastSavedHash = num;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder(true))
		{
			for (int i = 2; i <= 3; i++)
			{
				GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[i];
				if (slotRecoveryData.entityTypeId != -1)
				{
					utf16ValueStringBuilder.Append(i);
					utf16ValueStringBuilder.Append(",");
					utf16ValueStringBuilder.Append(slotRecoveryData.entityTypeId);
					utf16ValueStringBuilder.Append(",");
					utf16ValueStringBuilder.Append(slotRecoveryData.createData);
					utf16ValueStringBuilder.Append("|");
				}
			}
			PlayerPrefs.SetString("GT_SnappedItems_V1", utf16ValueStringBuilder.ToString());
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06002D28 RID: 11560 RVA: 0x000F3BEC File Offset: 0x000F1DEC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _LoadSnappedPlayerPrefsToCache(GamePlayer gamePlayer)
	{
		for (int i = 0; i < 4; i++)
		{
			GamePlayerLocal.slotsRecoveryData[i] = new GamePlayerLocal.SlotRecoveryData
			{
				entityTypeId = -1,
				createData = 0L
			};
		}
		for (int j = 0; j < 2; j++)
		{
			GamePlayerLocal.grabSlotsExtraRecoveryData[j] = new GamePlayerLocal.GrabSlotExtraRecoveryData
			{
				pos = Vector3.zero,
				rot = Quaternion.identity
			};
		}
		string[] array = PlayerPrefs.GetString("GT_SnappedItems_V1").Split('|', StringSplitOptions.RemoveEmptyEntries);
		for (int k = 0; k < array.Length; k++)
		{
			string[] array2 = array[k].Split(',', StringSplitOptions.None);
			int num;
			int num2;
			long num3;
			if (array2.Length >= 3 && int.TryParse(array2[0], out num) && num < 4 && GamePlayer.IsSnapSlot(num) && int.TryParse(array2[1], out num2) && long.TryParse(array2[2], out num3))
			{
				GamePlayerLocal.slotsRecoveryData[num] = new GamePlayerLocal.SlotRecoveryData
				{
					entityTypeId = num2,
					createData = num3
				};
			}
		}
	}

	// Token: 0x06002D29 RID: 11561 RVA: 0x000F3CFC File Offset: 0x000F1EFC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int _SnapSlotsSave_GetHash(GamePlayerLocal.SlotRecoveryData[] slotsCache)
	{
		int num = 67466746;
		for (int i = 2; i <= 3; i++)
		{
			GamePlayerLocal.SlotRecoveryData slotRecoveryData = slotsCache[i];
			num = StaticHash.Compute(num, i.GetStaticHash(), slotRecoveryData.entityTypeId.GetStaticHash(), slotRecoveryData.createData.GetStaticHash());
		}
		return num;
	}

	// Token: 0x06002D2A RID: 11562 RVA: 0x000F3D48 File Offset: 0x000F1F48
	public static bool TryGetMigrationRecoveryList(GameEntityManager newEntityManager, out List<GameEntityCreateData> out_recoveryList)
	{
		out_recoveryList = GamePlayerLocal._migrationRecoveryList;
		GamePlayerLocal._migrationRecoveryList.Clear();
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		for (int i = 0; i < 4; i++)
		{
			GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[i];
			int entityTypeId = slotRecoveryData.entityTypeId;
			bool flag = entityTypeId != -1 && newEntityManager.FactoryPrefabById(entityTypeId) != null;
			GamePlayer.SlotData slotData;
			bool flag2 = gamePlayer.TryGetSlotData(i, out slotData);
			if (flag || flag2)
			{
				bool flag3 = newEntityManager != null && newEntityManager == slotData.entityManager;
				GameEntity gameEntity = (flag3 ? newEntityManager.GetGameEntity(slotData.entityId) : null);
				bool flag4 = gameEntity != null;
				int num = (flag4 ? gameEntity.typeId : (-1));
				bool flag5 = num != -1;
				bool flag6 = entityTypeId == num;
				if (!flag3 || !flag4 || !flag6)
				{
					string text = (flag ? "[GamePlayerLocal]  TryGetMigrationRecoveryList: Recovering from mismatch between migrated entities and recovery data." : "[GamePlayerLocal]  ERROR!!!  TryGetMigrationRecoveryList: UNRECOVERABLE mismatch between migrated entities and recovery data.");
					Debug.unityLogger.Log(flag ? LogType.Log : LogType.Error, text);
					if (flag)
					{
						long createData = slotRecoveryData.createData;
						if (newEntityManager.LocalValidateMigrationRecoveryItem(entityTypeId, ref createData))
						{
							bool flag7 = false;
							for (int j = 0; j < 4; j++)
							{
								GamePlayer.SlotData slotData2;
								if (j != i && gamePlayer.TryGetSlotData(j, out slotData2) && !(slotData2.entityManager == null))
								{
									GameEntity gameEntity2 = slotData2.entityManager.GetGameEntity(slotData2.entityId);
									if (gameEntity2 != null && gameEntity2.typeId == entityTypeId)
									{
										flag7 = true;
										break;
									}
								}
							}
							if (flag7)
							{
								GamePlayerLocal.SetSlotRecoveryData(i, -1, 0L);
							}
							else
							{
								GamePlayerLocal._migrationRecoveryList.Add(new GameEntityCreateData
								{
									entityTypeId = entityTypeId,
									position = (GamePlayer.IsGrabSlot(i) ? GamePlayerLocal.grabSlotsExtraRecoveryData[i].pos : Vector3.zero),
									rotation = (GamePlayer.IsGrabSlot(i) ? GamePlayerLocal.grabSlotsExtraRecoveryData[i].rot : Quaternion.identity),
									createData = createData,
									createdByEntityId = -1,
									slotIndex = i
								});
							}
						}
					}
				}
			}
		}
		return GamePlayerLocal._migrationRecoveryList.Count > 0;
	}

	// Token: 0x04003978 RID: 14712
	private const string preLog = "[GamePlayerLocal]  ";

	// Token: 0x04003979 RID: 14713
	private const string preErr = "[GamePlayerLocal]  ERROR!!!  ";

	// Token: 0x0400397A RID: 14714
	public GamePlayer gamePlayer;

	// Token: 0x0400397B RID: 14715
	private GamePlayerLocal.HandData[] hands;

	// Token: 0x0400397C RID: 14716
	public const int MAX_INPUT_HISTORY = 32;

	// Token: 0x0400397D RID: 14717
	private GamePlayerLocal.InputData[] inputData;

	// Token: 0x0400397E RID: 14718
	private const string SNAP_SLOTS_SAVE_KEY = "GT_SnappedItems_V1";

	// Token: 0x0400397F RID: 14719
	private const float SNAP_SLOTS_SAVE__INTERVAL = 2f;

	// Token: 0x04003980 RID: 14720
	[OnEnterPlay_Set(false)]
	private static bool snapSlotsSave_isQueued;

	// Token: 0x04003981 RID: 14721
	[OnEnterPlay_Set(0)]
	private static int snapSlotsSave_lastSavedHash;

	// Token: 0x04003982 RID: 14722
	[OnEnterPlay_Set(0)]
	private static int snapSlotsSave_frameWhenQueued;

	// Token: 0x04003983 RID: 14723
	[OnEnterPlay_Set(0f)]
	private static float snapSlotsSave_lastTime;

	// Token: 0x04003984 RID: 14724
	private static readonly GamePlayerLocal.SlotRecoveryData[] slotsRecoveryData = new GamePlayerLocal.SlotRecoveryData[4];

	// Token: 0x04003985 RID: 14725
	private static readonly GamePlayerLocal.GrabSlotExtraRecoveryData[] grabSlotsExtraRecoveryData = new GamePlayerLocal.GrabSlotExtraRecoveryData[2];

	// Token: 0x04003986 RID: 14726
	[OnEnterPlay_SetNull]
	public static volatile GamePlayerLocal instance;

	// Token: 0x04003987 RID: 14727
	[NonSerialized]
	public GameEntityManager currGameEntityManager;

	// Token: 0x04003988 RID: 14728
	[NonSerialized]
	internal bool joinWithItemsSentForCurrentMigration;

	// Token: 0x04003989 RID: 14729
	[NonSerialized]
	internal bool pendingFullMigration;

	// Token: 0x0400398A RID: 14730
	private static readonly List<GameEntityCreateData> _migrationRecoveryList = new List<GameEntityCreateData>(4);

	// Token: 0x020006F7 RID: 1783
	private enum HandGrabState
	{
		// Token: 0x0400398C RID: 14732
		Empty,
		// Token: 0x0400398D RID: 14733
		Holding
	}

	// Token: 0x020006F8 RID: 1784
	private struct HandData
	{
		// Token: 0x0400398E RID: 14734
		public GamePlayerLocal.HandGrabState grabState;

		// Token: 0x0400398F RID: 14735
		public bool gripWasHeld;

		// Token: 0x04003990 RID: 14736
		public bool triggerWasHeld;

		// Token: 0x04003991 RID: 14737
		public double gripPressedTime;

		// Token: 0x04003992 RID: 14738
		public double triggerPressedTime;
	}

	// Token: 0x020006F9 RID: 1785
	public struct InputDataMotion
	{
		// Token: 0x04003993 RID: 14739
		public double time;

		// Token: 0x04003994 RID: 14740
		public Vector3 position;

		// Token: 0x04003995 RID: 14741
		public Quaternion rotation;

		// Token: 0x04003996 RID: 14742
		public Vector3 velocity;

		// Token: 0x04003997 RID: 14743
		public Vector3 angVelocity;
	}

	// Token: 0x020006FA RID: 1786
	public class InputData
	{
		// Token: 0x06002D2D RID: 11565 RVA: 0x000F3F94 File Offset: 0x000F2194
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GamePlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x06002D2E RID: 11566 RVA: 0x000F3FAF File Offset: 0x000F21AF
		public void AddInput(GamePlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x06002D2F RID: 11567 RVA: 0x000F3FDC File Offset: 0x000F21DC
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					float sqrMagnitude = inputDataMotion.velocity.sqrMagnitude;
					if (sqrMagnitude > num3)
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return Mathf.Sqrt(num3);
		}

		// Token: 0x06002D30 RID: 11568 RVA: 0x000F4058 File Offset: 0x000F2258
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 vector = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					vector += inputDataMotion.velocity;
					num3++;
				}
			}
			if (num3 == 0)
			{
				return Vector3.zero;
			}
			return vector / (float)num3;
		}

		// Token: 0x04003998 RID: 14744
		public int maxInputs;

		// Token: 0x04003999 RID: 14745
		public List<GamePlayerLocal.InputDataMotion> inputMotionHistory;
	}

	// Token: 0x020006FB RID: 1787
	public struct SlotRecoveryData
	{
		// Token: 0x0400399A RID: 14746
		public int entityTypeId;

		// Token: 0x0400399B RID: 14747
		public long createData;
	}

	// Token: 0x020006FC RID: 1788
	public struct GrabSlotExtraRecoveryData
	{
		// Token: 0x0400399C RID: 14748
		public Vector3 pos;

		// Token: 0x0400399D RID: 14749
		public Quaternion rot;
	}
}
