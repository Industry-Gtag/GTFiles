using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000100 RID: 256
[RequireComponent(typeof(GameEntity))]
public abstract class SIGadget : MonoBehaviour, IGameEntityComponent, IPrefabRequirements, IGameActivatable, IGameStateProvider
{
	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060005F8 RID: 1528 RVA: 0x0002270B File Offset: 0x0002090B
	// (set) Token: 0x060005F9 RID: 1529 RVA: 0x00022713 File Offset: 0x00020913
	public SITechTreePageId PageId
	{
		get
		{
			return this.pageId;
		}
		set
		{
			this.pageId = value;
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060005FA RID: 1530 RVA: 0x0002271C File Offset: 0x0002091C
	public IEnumerable<GameEntity> RequiredPrefabs
	{
		get
		{
			return this.additionalRequiredPrefabs;
		}
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00022724 File Offset: 0x00020924
	protected virtual void Update()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0002275E File Offset: 0x0002095E
	protected virtual void OnUpdateAuthority(float dt)
	{
		this.SleepAfterDelay();
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x0002275E File Offset: 0x0002095E
	protected virtual void OnUpdateRemote(float dt)
	{
		this.SleepAfterDelay();
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00022766 File Offset: 0x00020966
	protected virtual bool IsEquippedLocal()
	{
		return this.gameEntity.IsHeldByLocalPlayer() || this.gameEntity.IsSnappedByLocalPlayer();
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00022784 File Offset: 0x00020984
	protected Vector2 GetJoystickInput()
	{
		if (!this.ShouldProcessInput())
		{
			return default(Vector2);
		}
		return ControllerInputPoller.Primary2DAxis((this.gameEntity.heldByHandIndex == 0 || this.gameEntity.snappedJoint == SnapJointType.HandL) ? XRNode.LeftHand : XRNode.RightHand);
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x000227CC File Offset: 0x000209CC
	protected bool ShouldProcessInput()
	{
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			return true;
		}
		GamePlayer gamePlayer;
		if (this.gameEntity.IsSnappedByLocalPlayer() && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer))
		{
			SnapJointType snappedJoint = this.gameEntity.snappedJoint;
			GameEntity gameEntity;
			if (snappedJoint != SnapJointType.HandL)
			{
				if (snappedJoint != SnapJointType.HandR)
				{
					gameEntity = null;
				}
				else
				{
					gameEntity = gamePlayer.GetGrabbedGameEntity(1);
				}
			}
			else
			{
				gameEntity = gamePlayer.GetGrabbedGameEntity(0);
			}
			GameEntity gameEntity2 = gameEntity;
			return !gameEntity2 || gameEntity2.GetComponent<IGameActivatable>() == null;
		}
		return false;
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0002284C File Offset: 0x00020A4C
	public void SleepAfterDelay()
	{
		if (this.isSleeping || !this.shouldSleep)
		{
			return;
		}
		if (Time.time < this.timeReleased + this.sleepTime)
		{
			return;
		}
		base.GetComponent<Rigidbody>().isKinematic = true;
		this.isSleeping = true;
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00022887 File Offset: 0x00020A87
	public virtual SIUpgradeSet FilterUpgradeNodes(SIUpgradeSet upgrades)
	{
		return upgrades;
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x0002288C File Offset: 0x00020A8C
	public virtual void RefreshUpgradeVisuals(SIUpgradeSet withUpgrades)
	{
		foreach (SIGadget.UpgradeVisual upgradeVisual in this.UpgradeBasedVisuals)
		{
			upgradeVisual.Update(withUpgrades);
		}
		Action<SIUpgradeSet> onPostRefreshVisuals = this.OnPostRefreshVisuals;
		if (onPostRefreshVisuals == null)
		{
			return;
		}
		onPostRefreshVisuals(withUpgrades);
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x000228D0 File Offset: 0x00020AD0
	protected virtual void OnEnable()
	{
		if (!this.didApplyId)
		{
			GameObject gameObject = base.gameObject;
			gameObject.name = gameObject.name + "[" + SIGadget.uniqueId.ToString() + "]";
			this.didApplyId = true;
			SIGadget.uniqueId++;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Combine(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.timeReleased = Time.time;
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x000229CC File Offset: 0x00020BCC
	protected virtual void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Remove(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Remove(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.LeaveAllExclusionZones();
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00022A7C File Offset: 0x00020C7C
	public void GrabInitialization()
	{
		this.isSleeping = false;
		this.shouldSleep = false;
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		SuperInfectionManager component = this.gameEntity.manager.GetComponent<SuperInfectionManager>();
		if (((component != null) ? component.zoneSuperInfection : null) == null)
		{
			return;
		}
		bool flag = SIPlayer.LocalPlayer.activePlayerGadgets.Contains(this.gameEntity.GetNetId());
		SIProgression.Instance.UpdateHeldGadgetsTelemetry(this.PageId, flag, 1);
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x00022AF8 File Offset: 0x00020CF8
	public void ReleaseInitialization()
	{
		this.shouldSleep = true;
		this.isSleeping = false;
		this.timeReleased = Time.time;
		if (!this.gameEntity.WasLastHeldByLocalPlayer())
		{
			return;
		}
		SuperInfectionManager component = this.gameEntity.manager.GetComponent<SuperInfectionManager>();
		if (((component != null) ? component.zoneSuperInfection : null) == null)
		{
			return;
		}
		bool flag = SIPlayer.LocalPlayer.activePlayerGadgets.Contains(this.gameEntity.GetNetId());
		SIProgression.Instance.UpdateHeldGadgetsTelemetry(this.PageId, flag, -1);
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00022B80 File Offset: 0x00020D80
	public bool FindAttachedHand(out bool isLeft)
	{
		isLeft = false;
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.AttachedPlayerActorNr, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindSlotIndex(this.gameEntity.id);
		isLeft = num == 0 || num == 2;
		return isLeft || num == 1 || num == 3;
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x00022BD4 File Offset: 0x00020DD4
	public VRRig GetAttachedPlayerRig()
	{
		int attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
		GamePlayer gamePlayer;
		if (attachedPlayerActorNr < 1 || !GamePlayer.TryGetGamePlayer(attachedPlayerActorNr, out gamePlayer))
		{
			return null;
		}
		return gamePlayer.rig;
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnEntityInit()
	{
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnEntityDestroy()
	{
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00022C04 File Offset: 0x00020E04
	public virtual void OnEntityStateChange(long prevState, long newState)
	{
		foreach (IGameStateReceiver gameStateReceiver in this._gameStateReceivers)
		{
			gameStateReceiver.GameStateReceiverOnStateChanged(prevState, newState);
		}
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ProcessAuthorityToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00022C58 File Offset: 0x00020E58
	public void SendClientToAuthorityRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00022CB0 File Offset: 0x00020EB0
	public void SendClientToAuthorityRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00022D0C File Offset: 0x00020F0C
	public void SendAuthorityToClientRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.AuthorityToClientRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00022D64 File Offset: 0x00020F64
	public void SendAuthorityToClientRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.AuthorityToClientRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00022DC0 File Offset: 0x00020FC0
	public void SendClientToClientRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x00022E18 File Offset: 0x00021018
	public void SendClientToClientRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x00022E71 File Offset: 0x00021071
	public void ApplyExclusionZone(SIExclusionZone exclusionZone)
	{
		if (!this.appliedExclusionZones.Contains(exclusionZone))
		{
			bool activeExclusionFlags = this._activeExclusionFlags != (SIExclusionType)0;
			this.appliedExclusionZones.Add(exclusionZone);
			this._activeExclusionFlags |= exclusionZone.exclusionType;
			if (!activeExclusionFlags)
			{
				this.HandleBlockedActionChanged(true);
			}
		}
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x00022EAF File Offset: 0x000210AF
	public void LeaveExclusionZone(SIExclusionZone exclusionZone)
	{
		if (this.appliedExclusionZones.Contains(exclusionZone))
		{
			this.appliedExclusionZones.Remove(exclusionZone);
			this.RecalcExclusionFlags();
			if (this._activeExclusionFlags == (SIExclusionType)0)
			{
				this.HandleBlockedActionChanged(false);
			}
		}
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00022EE4 File Offset: 0x000210E4
	private void LeaveAllExclusionZones()
	{
		foreach (SIExclusionZone siexclusionZone in this.appliedExclusionZones)
		{
			if (siexclusionZone != null)
			{
				siexclusionZone.ClearGadget(this);
			}
		}
		this.appliedExclusionZones.Clear();
		this._activeExclusionFlags = (SIExclusionType)0;
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x00022F54 File Offset: 0x00021154
	private void RecalcExclusionFlags()
	{
		SIExclusionType siexclusionType = (SIExclusionType)0;
		for (int i = 0; i < this.appliedExclusionZones.Count; i++)
		{
			siexclusionType |= this.appliedExclusionZones[i].exclusionType;
		}
		this._activeExclusionFlags = siexclusionType;
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x00022F94 File Offset: 0x00021194
	protected bool IsBlocked()
	{
		return (this._activeExclusionFlags & SIExclusionType.AffectsOthers) > (SIExclusionType)0;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x00022FA1 File Offset: 0x000211A1
	protected bool IsBlocked(SIExclusionType flag)
	{
		return (this._activeExclusionFlags & flag) > (SIExclusionType)0;
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void HandleBlockedActionChanged(bool isBlocked)
	{
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x00022FAE File Offset: 0x000211AE
	void IGameStateProvider.GameStateReceiverRegister(IGameStateReceiver receiver)
	{
		this._gameStateReceivers.Add(receiver);
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x00022FBC File Offset: 0x000211BC
	void IGameStateProvider.GameStateReceiverUnregister(IGameStateReceiver receiver)
	{
		this._gameStateReceivers.Remove(receiver);
	}

	// Token: 0x04000782 RID: 1922
	public GameEntity gameEntity;

	// Token: 0x04000783 RID: 1923
	[Tooltip("Add additional required prefabs here.  These will be automatically added to the GameEntityManager factory.")]
	public GameEntity[] additionalRequiredPrefabs;

	// Token: 0x04000784 RID: 1924
	public float sleepTime = 10f;

	// Token: 0x04000785 RID: 1925
	private bool shouldSleep = true;

	// Token: 0x04000786 RID: 1926
	private bool isSleeping;

	// Token: 0x04000787 RID: 1927
	private float timeReleased;

	// Token: 0x04000788 RID: 1928
	protected bool activatedLocally;

	// Token: 0x04000789 RID: 1929
	[SerializeField]
	private SITechTreePageId pageId;

	// Token: 0x0400078A RID: 1930
	public Action<SIUpgradeSet> OnPostRefreshVisuals;

	// Token: 0x0400078B RID: 1931
	private static int uniqueId = 101;

	// Token: 0x0400078C RID: 1932
	private bool didApplyId;

	// Token: 0x0400078D RID: 1933
	[SerializeField]
	private SIGadget.UpgradeVisual[] UpgradeBasedVisuals;

	// Token: 0x0400078E RID: 1934
	private readonly List<SIExclusionZone> appliedExclusionZones = new List<SIExclusionZone>();

	// Token: 0x0400078F RID: 1935
	private SIExclusionType _activeExclusionFlags;

	// Token: 0x04000790 RID: 1936
	private List<IGameStateReceiver> _gameStateReceivers = new List<IGameStateReceiver>();

	// Token: 0x02000101 RID: 257
	[Serializable]
	private struct UpgradeVisual
	{
		// Token: 0x06000622 RID: 1570 RVA: 0x00023004 File Offset: 0x00021204
		public void Update(SIUpgradeSet withUpgrades)
		{
			bool flag = true;
			if (this.appearRequirements.Length != 0)
			{
				flag = false;
				foreach (SIUpgradeType siupgradeType in this.appearRequirements)
				{
					if (withUpgrades.Contains(siupgradeType))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				foreach (SIUpgradeType siupgradeType2 in this.disappearRequirements)
				{
					if (withUpgrades.Contains(siupgradeType2))
					{
						flag = false;
						break;
					}
				}
			}
			GameObject[] array2 = this.objects;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].SetActive(flag);
			}
		}

		// Token: 0x04000791 RID: 1937
		public GameObject[] objects;

		// Token: 0x04000792 RID: 1938
		[Tooltip("For the objects to become activated, you must match AT LEAST ONE appearRequirement (if there are any), and not match any disappearRequirements.")]
		public SIUpgradeType[] appearRequirements;

		// Token: 0x04000793 RID: 1939
		[Tooltip("For the objects to become deactivated, you must match AT LEAST ONE disappearRequirement (if there are any).")]
		public SIUpgradeType[] disappearRequirements;
	}
}
