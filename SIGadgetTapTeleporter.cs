using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class SIGadgetTapTeleporter : SIGadget
{
	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060006B5 RID: 1717 RVA: 0x00025AE8 File Offset: 0x00023CE8
	// (set) Token: 0x060006B6 RID: 1718 RVA: 0x00025AF0 File Offset: 0x00023CF0
	public Color identifierColor { get; private set; }

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060006B7 RID: 1719 RVA: 0x00025AF9 File Offset: 0x00023CF9
	// (set) Token: 0x060006B8 RID: 1720 RVA: 0x00025B01 File Offset: 0x00023D01
	public bool useStealthTeleporters { get; private set; }

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060006B9 RID: 1721 RVA: 0x00025B0A File Offset: 0x00023D0A
	// (set) Token: 0x060006BA RID: 1722 RVA: 0x00025B12 File Offset: 0x00023D12
	public bool isVelocityPreserved { get; private set; }

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060006BB RID: 1723 RVA: 0x00025B1B File Offset: 0x00023D1B
	// (set) Token: 0x060006BC RID: 1724 RVA: 0x00025B23 File Offset: 0x00023D23
	public bool hasInfiniteDuration { get; private set; }

	// Token: 0x060006BD RID: 1725 RVA: 0x00025B2C File Offset: 0x00023D2C
	public override void OnEntityInit()
	{
		this.gameEntity.OnStateChanged += this.HandleStateChanged;
		this.gameEntity.onEntityDestroyed += this.HandleOnDestroyed;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.HandleHandAttached));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.HandleHandAttached));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.HandleHandDetach));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.HandleHandDetach));
		this.identifierColor = this.GenerateColor(this.gameEntity.GetNetId());
		this.ApplyIdentifierColor();
		this.UpdateNextSelectionDisplay();
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00025C28 File Offset: 0x00023E28
	private void HandleOnDestroyed(GameEntity entity)
	{
		if (this.gameEntity.IsAuthority())
		{
			if (this._selection1Teleport)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection1Teleport.gameEntity.id);
			}
			if (this._selection2Teleport)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection2Teleport.gameEntity.id);
			}
		}
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00025C9C File Offset: 0x00023E9C
	private new void OnDisable()
	{
		this.HandleHandDetach();
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00025CA4 File Offset: 0x00023EA4
	private void HandleHandAttached()
	{
		if (this.IsEquippedLocal())
		{
			this.isHandTapSetup = true;
			GorillaTagger.Instance.OnHandTap += this.HandleOnHandTap;
		}
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00025CCB File Offset: 0x00023ECB
	private void HandleHandDetach()
	{
		if (this.isHandTapSetup)
		{
			this.isHandTapSetup = false;
			GorillaTagger.Instance.OnHandTap -= this.HandleOnHandTap;
		}
		this.isActivated = false;
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00025CFC File Offset: 0x00023EFC
	private void HandleOnHandTap(bool isLeft, Vector3 position, Vector3 normal)
	{
		bool flag;
		if (base.FindAttachedHand(out flag) && isLeft == flag && this.isActivated)
		{
			this.PlaceTapTeleporter(position, normal);
		}
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00025D28 File Offset: 0x00023F28
	private Color GenerateColor(int seed)
	{
		Random.InitState(seed);
		float num = Mathf.Lerp(this.maxBrightness, this.minBrightness, Random.value);
		float num2 = Mathf.Lerp(this.maxBrightness, this.minBrightness, Random.value);
		Color black = Color.black;
		switch (Random.Range(0, 3))
		{
		case 0:
			black.r = num;
			black.g = num2;
			break;
		case 1:
			black.g = num;
			black.b = num2;
			break;
		case 2:
			black.b = num;
			black.r = num2;
			break;
		}
		return black;
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00025DC0 File Offset: 0x00023FC0
	protected override void OnUpdateAuthority(float dt)
	{
		this.isActivated = this.buttonActivatable.CheckInput(0.25f);
		if (this.nextPlacementDelay > 0f)
		{
			this.nextPlacementDelay -= dt;
		}
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x00025DF4 File Offset: 0x00023FF4
	private void PlaceTapTeleporter(Vector3 position, Vector3 normal)
	{
		if (this.nextPlacementDelay > 0f)
		{
			return;
		}
		if (!this.CheckValidTeleporterPlacement(position, normal))
		{
			return;
		}
		if (base.IsBlocked())
		{
			this.blockedSFX.Play();
			return;
		}
		base.SendClientToAuthorityRPC(0, new object[]
		{
			position,
			Quaternion.LookRotation(normal, base.transform.forward),
			this.nextSelectionId,
			this.hasInfiniteDuration ? (-1f) : this.portalDefaultDuration
		});
		this.CycleSelection();
		this.nextPlacementDelay = this.placementDelay;
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x00025E9C File Offset: 0x0002409C
	private bool CheckValidTeleporterPlacement(Vector3 position, Vector3 direction)
	{
		Vector3 vector = position + direction * this.nearOffset;
		Vector3 vector2 = position + direction * this.farOffset;
		return Physics.OverlapCapsuleNonAlloc(vector, vector2, this.overlapCheckRadius, this.overlapCheckResults, this.overlapCheckLayers) == 0;
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00025EEE File Offset: 0x000240EE
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.instanceUpgrades = withUpgrades;
		this.useStealthTeleporters = withUpgrades.Contains(SIUpgradeType.Tapteleport_Stealth);
		this.isVelocityPreserved = withUpgrades.Contains(SIUpgradeType.Tapteleport_Keep_Velocity);
		this.hasInfiniteDuration = withUpgrades.Contains(SIUpgradeType.Tapteleport_Infinite_Use);
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00025F30 File Offset: 0x00024130
	public override void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 4)
			{
				return;
			}
			Vector3 vector;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[0], out vector))
			{
				return;
			}
			Quaternion quaternion;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[1], out quaternion))
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[2], out num))
			{
				return;
			}
			if (num < 0 || num > 100)
			{
				return;
			}
			float num2;
			if (!GameEntityManager.ValidateDataType<float>(data[3], out num2))
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			if (Vector3.Distance(vector, base.transform.position) > this.placementCheckDistance)
			{
				return;
			}
			if (!this.CheckValidTeleporterPlacement(vector, quaternion * Vector3.forward))
			{
				return;
			}
			this.RemoveTeleporter(num);
			this.PlaceNewTapTeleporter(vector, quaternion, num, num2);
		}
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x00025FE4 File Offset: 0x000241E4
	public override void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 1)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			if (num < 0 || num > 1)
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			this.nextSelectionId = num;
			this.UpdateNextSelectionDisplay();
		}
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x0002603C File Offset: 0x0002423C
	private void RemoveTeleporter(int selectId)
	{
		if (selectId == 0)
		{
			if (this._selection1Teleport != null && this._selection1Teleport.gameObject.activeSelf)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection1Teleport.gameEntity.id);
				this._selection1Teleport = null;
				return;
			}
		}
		else if (selectId == 1 && this._selection2Teleport != null && this._selection2Teleport.gameObject.activeSelf)
		{
			this.gameEntity.manager.RequestDestroyItem(this._selection2Teleport.gameEntity.id);
			this._selection2Teleport = null;
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x000260E4 File Offset: 0x000242E4
	private void PlaceNewTapTeleporter(Vector3 position, Quaternion rotation, int selectionId, float duration)
	{
		GameEntityId gameEntityId = this.gameEntity.manager.RequestCreateItem(this.teleportPointPrefab.gameObject.name.GetStaticHash(), position, rotation, BitPackUtils.PackIntsIntoLong(selectionId, (int)duration));
		if (gameEntityId != GameEntityId.Invalid)
		{
			SIGadgetTapTeleporterDeployable component = this.gameEntity.manager.GetGameEntity(gameEntityId).GetComponent<SIGadgetTapTeleporterDeployable>();
			if (selectionId == 0)
			{
				if (this._selection2Teleport != null)
				{
					this._selection2Teleport.SetLink(this, component);
				}
				component.SetLink(this, this._selection2Teleport);
				this._selection1Teleport = component;
			}
			else if (selectionId == 1)
			{
				if (this._selection1Teleport != null)
				{
					this._selection1Teleport.SetLink(this, component);
				}
				component.SetLink(this, this._selection1Teleport);
				this._selection2Teleport = component;
			}
			this.UpdateNewTeleporters();
		}
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x000261B8 File Offset: 0x000243B8
	private void UpdateNewTeleporters()
	{
		int num;
		if (this._selection1Teleport)
		{
			num = this._selection1Teleport.gameEntity.GetNetId();
		}
		else
		{
			num = 0;
		}
		int num2;
		if (this._selection2Teleport)
		{
			num2 = this._selection2Teleport.gameEntity.GetNetId();
		}
		else
		{
			num2 = 0;
		}
		long num3 = BitPackUtils.PackIntsIntoLong(num, num2);
		this.gameEntity.RequestState(this.gameEntity.id, num3);
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x00026228 File Offset: 0x00024428
	private void HandleStateChanged(long oldState, long newState)
	{
		if (this.gameEntity.IsAuthority())
		{
			return;
		}
		int num;
		int num2;
		BitPackUtils.UnpackIntsFromLong(newState, out num, out num2);
		GameEntity gameEntityFromNetId = this.gameEntity.manager.GetGameEntityFromNetId(num);
		if (gameEntityFromNetId != null)
		{
			this._selection1Teleport = gameEntityFromNetId.GetComponent<SIGadgetTapTeleporterDeployable>();
		}
		else
		{
			this._selection1Teleport = null;
		}
		GameEntity gameEntityFromNetId2 = this.gameEntity.manager.GetGameEntityFromNetId(num2);
		if (gameEntityFromNetId2 != null)
		{
			this._selection2Teleport = gameEntityFromNetId2.GetComponent<SIGadgetTapTeleporterDeployable>();
			return;
		}
		this._selection2Teleport = null;
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x000262AC File Offset: 0x000244AC
	private void ApplyIdentifierColor()
	{
		this.identifierColorDisplay.material.color = this.identifierColor;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000262C4 File Offset: 0x000244C4
	private void UpdateNextSelectionDisplay()
	{
		if (this.nextSelectionId == 0)
		{
			this.selectionColorDisplay.material = this.selectionColor1;
			return;
		}
		if (this.nextSelectionId == 1)
		{
			this.selectionColorDisplay.material = this.selectionColor2;
		}
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x000262FA File Offset: 0x000244FA
	public void CycleSelection()
	{
		this.nextSelectionId = (this.nextSelectionId + 1) % 2;
		this.UpdateNextSelectionDisplay();
		base.SendClientToClientRPC(0, new object[] { this.nextSelectionId });
	}

	// Token: 0x0400083F RID: 2111
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x04000840 RID: 2112
	[SerializeField]
	private GameObject teleportPointPrefab;

	// Token: 0x04000841 RID: 2113
	[SerializeField]
	private SoundBankPlayer blockedSFX;

	// Token: 0x04000842 RID: 2114
	[SerializeField]
	private float placementDelay = 0.5f;

	// Token: 0x04000843 RID: 2115
	[SerializeField]
	private Renderer identifierColorDisplay;

	// Token: 0x04000844 RID: 2116
	[SerializeField]
	private Renderer selectionColorDisplay;

	// Token: 0x04000845 RID: 2117
	[SerializeField]
	private Material selectionColor1;

	// Token: 0x04000846 RID: 2118
	[SerializeField]
	private Material selectionColor2;

	// Token: 0x04000847 RID: 2119
	[SerializeField]
	private float portalDefaultDuration = 30f;

	// Token: 0x04000848 RID: 2120
	private float placementCheckDistance = 0.3f;

	// Token: 0x0400084D RID: 2125
	private SIGadgetTapTeleporterDeployable _selection1Teleport;

	// Token: 0x0400084E RID: 2126
	private SIGadgetTapTeleporterDeployable _selection2Teleport;

	// Token: 0x0400084F RID: 2127
	private bool isHandTapSetup;

	// Token: 0x04000850 RID: 2128
	private bool isActivated;

	// Token: 0x04000851 RID: 2129
	private float nextPlacementDelay;

	// Token: 0x04000852 RID: 2130
	private int nextSelectionId;

	// Token: 0x04000853 RID: 2131
	private SIUpgradeSet instanceUpgrades;

	// Token: 0x04000854 RID: 2132
	private float minBrightness = 0.3f;

	// Token: 0x04000855 RID: 2133
	private float maxBrightness = 1f;

	// Token: 0x04000856 RID: 2134
	[SerializeField]
	private LayerMask overlapCheckLayers;

	// Token: 0x04000857 RID: 2135
	[SerializeField]
	private float nearOffset = 0.11f;

	// Token: 0x04000858 RID: 2136
	[SerializeField]
	private float farOffset = 0.664f;

	// Token: 0x04000859 RID: 2137
	[SerializeField]
	private float overlapCheckRadius = 0.1f;

	// Token: 0x0400085A RID: 2138
	private Collider[] overlapCheckResults = new Collider[1];
}
