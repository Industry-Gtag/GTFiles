using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000803 RID: 2051
public class GRTool : MonoBehaviour, IGameEntitySerialize, IGameEntityComponent, IGameEntityDebugComponent
{
	// Token: 0x1400005B RID: 91
	// (add) Token: 0x06003460 RID: 13408 RVA: 0x0011FB60 File Offset: 0x0011DD60
	// (remove) Token: 0x06003461 RID: 13409 RVA: 0x0011FB98 File Offset: 0x0011DD98
	public event GRTool.EnergyChangeEvent OnEnergyChange;

	// Token: 0x1400005C RID: 92
	// (add) Token: 0x06003462 RID: 13410 RVA: 0x0011FBD0 File Offset: 0x0011DDD0
	// (remove) Token: 0x06003463 RID: 13411 RVA: 0x0011FC08 File Offset: 0x0011DE08
	public event GRTool.ToolUpgradedEvent onToolUpgraded;

	// Token: 0x06003464 RID: 13412 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06003465 RID: 13413 RVA: 0x0011FC3D File Offset: 0x0011DE3D
	private void Start()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		this.RefreshMeters();
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x0011FC60 File Offset: 0x0011DE60
	public void OnEntityInit()
	{
		this.energy = this.GetEnergyStart();
		GhostReactor.ToolEntityCreateData toolEntityCreateData = GhostReactor.ToolEntityCreateData.Unpack(this.gameEntity.createData);
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null)
		{
			GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = ghostReactorManager.GetToolUpgradeStationFullForIndex(toolEntityCreateData.stationIndex);
			if (toolUpgradeStationFullForIndex != null)
			{
				toolUpgradeStationFullForIndex.InitLinkedEntity(this.gameEntity);
			}
		}
	}

	// Token: 0x06003467 RID: 13415 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003468 RID: 13416 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003469 RID: 13417 RVA: 0x0011FCC1 File Offset: 0x0011DEC1
	public int GetEnergyMax()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyMax);
	}

	// Token: 0x0600346A RID: 13418 RVA: 0x0011FCCF File Offset: 0x0011DECF
	public int GetEnergyUseCost()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyUseCost);
	}

	// Token: 0x0600346B RID: 13419 RVA: 0x0011FCDD File Offset: 0x0011DEDD
	public int GetEnergyStart()
	{
		if (!this.attributes.HasValueForAttribute(GRAttributeType.EnergyStart))
		{
			return 0;
		}
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyStart);
	}

	// Token: 0x0600346C RID: 13420 RVA: 0x0011FCFB File Offset: 0x0011DEFB
	private void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x0011FD24 File Offset: 0x0011DF24
	private void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
	}

	// Token: 0x0600346E RID: 13422 RVA: 0x0011FD4D File Offset: 0x0011DF4D
	public void RefillEnergy(int count, GameEntityId chargingEntityId)
	{
		this.SetEnergyInternal(this.energy + count, chargingEntityId);
	}

	// Token: 0x0600346F RID: 13423 RVA: 0x0011FD5E File Offset: 0x0011DF5E
	public void RefillEnergy()
	{
		this.SetEnergyInternal(this.GetEnergyMax(), GameEntityId.Invalid);
	}

	// Token: 0x06003470 RID: 13424 RVA: 0x0011FD71 File Offset: 0x0011DF71
	public void UseEnergy()
	{
		this.SetEnergyInternal(this.energy - this.GetEnergyUseCost(), GameEntityId.Invalid);
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x0011FD8B File Offset: 0x0011DF8B
	public bool HasEnoughEnergy()
	{
		return this.energy >= this.GetEnergyUseCost();
	}

	// Token: 0x06003472 RID: 13426 RVA: 0x0011FD9E File Offset: 0x0011DF9E
	public void SetEnergy(int newEnergy)
	{
		this.SetEnergyInternal(newEnergy, GameEntityId.Invalid);
	}

	// Token: 0x06003473 RID: 13427 RVA: 0x0011FDAC File Offset: 0x0011DFAC
	public bool IsEnergyFull()
	{
		return this.energy >= this.GetEnergyMax();
	}

	// Token: 0x06003474 RID: 13428 RVA: 0x0011FDC0 File Offset: 0x0011DFC0
	private void SetEnergyInternal(int value, GameEntityId chargingEntityId)
	{
		int num = this.energy;
		this.energy = Mathf.Clamp(value, 0, this.GetEnergyMax());
		int num2 = this.energy - num;
		GRTool.EnergyChangeEvent onEnergyChange = this.OnEnergyChange;
		if (onEnergyChange != null)
		{
			onEnergyChange(this, num2, chargingEntityId);
		}
		this.RefreshMeters();
	}

	// Token: 0x06003475 RID: 13429 RVA: 0x0011FE0C File Offset: 0x0011E00C
	public void RefreshMeters()
	{
		for (int i = 0; i < this.energyMeters.Count; i++)
		{
			this.energyMeters[i].Refresh();
		}
	}

	// Token: 0x06003476 RID: 13430 RVA: 0x0011FE40 File Offset: 0x0011E040
	public bool HasUpgradeInstalled(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgradeSlots.Count; i++)
		{
			if (this.upgradeSlots[i].installedItem != null && this.upgradeSlots[i].installedItem.UpgradeType == upgradeID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003477 RID: 13431 RVA: 0x0011FE94 File Offset: 0x0011E094
	public GRTool.Upgrade FindMatchingUpgrade(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgrades.Count; i++)
		{
			if (this.upgrades[i].UpgradeType == upgradeID)
			{
				return this.upgrades[i];
			}
		}
		return null;
	}

	// Token: 0x06003478 RID: 13432 RVA: 0x0011FEDC File Offset: 0x0011E0DC
	public float GetPointDistanceToUpgrade(Vector3 point, GRTool.Upgrade upgrade)
	{
		if (upgrade.VisibleItem.Count < 1)
		{
			return -1f;
		}
		if (this.upgradeListsAreValidFor != upgrade)
		{
			this.reservedMeshFilterSearchList.Clear();
			upgrade.VisibleItem[0].GetComponentsInChildren<MeshFilter>(this.reservedMeshFilterSearchList);
			this.reservedMeshFilterSearchListSkinned.Clear();
			upgrade.VisibleItem[0].GetComponentsInChildren<SkinnedMeshRenderer>(false, this.reservedMeshFilterSearchListSkinned);
			this.upgradeListsAreValidFor = upgrade;
		}
		float num = float.MaxValue;
		foreach (MeshFilter meshFilter in this.reservedMeshFilterSearchList)
		{
			Vector3 vector = meshFilter.transform.InverseTransformPoint(point);
			Bounds bounds = meshFilter.sharedMesh.bounds;
			Vector3 vector2 = new Vector3(Mathf.Clamp(vector.x, bounds.min.x, bounds.max.x), Mathf.Clamp(vector.y, bounds.min.y, bounds.max.y), Mathf.Clamp(vector.z, bounds.min.z, bounds.max.z));
			Vector3 vector3 = vector - vector2;
			float sqrMagnitude = meshFilter.transform.TransformVector(vector3).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
			}
		}
		if (this.reservedMeshFilterSearchListSkinned != null)
		{
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.reservedMeshFilterSearchListSkinned)
			{
				Vector3 vector4 = skinnedMeshRenderer.transform.InverseTransformPoint(point);
				Bounds localBounds = skinnedMeshRenderer.localBounds;
				Vector3 vector5 = new Vector3(Mathf.Clamp(vector4.x, localBounds.min.x, localBounds.max.x), Mathf.Clamp(vector4.y, localBounds.min.y, localBounds.max.y), Mathf.Clamp(vector4.z, localBounds.min.z, localBounds.max.z));
				Vector3 vector6 = vector4 - vector5;
				float sqrMagnitude2 = skinnedMeshRenderer.transform.TransformVector(vector6).sqrMagnitude;
				if (sqrMagnitude2 < num)
				{
					num = sqrMagnitude2;
				}
			}
		}
		if (num == 3.4028235E+38f)
		{
			return Vector3.Distance(point, upgrade.VisibleItem[0].transform.position);
		}
		return Mathf.Sqrt(num);
	}

	// Token: 0x06003479 RID: 13433 RVA: 0x00120178 File Offset: 0x0011E378
	public Transform GetUpgradeAttachTransform(GRTool.Upgrade upgrade)
	{
		if (upgrade.VisibleItem.Count < 1)
		{
			return null;
		}
		return upgrade.VisibleItem[0].transform;
	}

	// Token: 0x0600347A RID: 13434 RVA: 0x0012019C File Offset: 0x0011E39C
	public void UpgradeTool(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgrades.Count; i++)
		{
			if (this.upgrades[i].UpgradeType == upgradeID)
			{
				this.ClearUpgradeSlot(this.upgrades[i].Slot);
				for (int j = 0; j < this.upgrades[i].VisibleItem.Count; j++)
				{
					this.upgrades[i].VisibleItem[j].SetActive(true);
				}
				for (int k = 0; k < this.upgradeSlots[this.upgrades[i].Slot].DefaultVisibleItems.Count; k++)
				{
					this.upgradeSlots[this.upgrades[i].Slot].DefaultVisibleItems[k].SetActive(false);
				}
				foreach (GRBonusEntry grbonusEntry in this.upgrades[i].bonusEffects)
				{
					this.attributes.AddBonus(grbonusEntry);
				}
				this.upgradeSlots[this.upgrades[i].Slot].installedItem = this.upgrades[i];
				if (this.UpgradeFXNode != null && this.upgrades[i].VisibleItem.Count > 0)
				{
					this.UpgradeFXNode.transform.position = this.upgrades[i].VisibleItem[0].transform.position;
					this.UpgradeFXNode.transform.rotation = this.upgrades[i].VisibleItem[0].transform.rotation;
					ParticleSystem componentInChildren = this.UpgradeFXNode.GetComponentInChildren<ParticleSystem>();
					AudioSource componentInChildren2 = this.UpgradeFXNode.GetComponentInChildren<AudioSource>();
					if (componentInChildren != null)
					{
						componentInChildren.Play();
					}
					if (componentInChildren2 != null)
					{
						componentInChildren2.Play();
					}
				}
			}
		}
		GRTool.ToolUpgradedEvent toolUpgradedEvent = this.onToolUpgraded;
		if (toolUpgradedEvent == null)
		{
			return;
		}
		toolUpgradedEvent(this);
	}

	// Token: 0x0600347B RID: 13435 RVA: 0x001203F0 File Offset: 0x0011E5F0
	public void ClearUpgradeSlot(int slot)
	{
		if (this.upgradeSlots[slot].installedItem != null)
		{
			for (int i = 0; i < this.upgradeSlots[slot].installedItem.VisibleItem.Count; i++)
			{
				this.upgradeSlots[slot].installedItem.VisibleItem[i].SetActive(false);
			}
			foreach (GRBonusEntry grbonusEntry in this.upgradeSlots[slot].installedItem.bonusEffects)
			{
				this.attributes.RemoveBonus(grbonusEntry);
			}
			for (int j = 0; j < this.upgradeSlots[slot].DefaultVisibleItems.Count; j++)
			{
				this.upgradeSlots[slot].DefaultVisibleItems[j].SetActive(true);
			}
		}
	}

	// Token: 0x0600347C RID: 13436 RVA: 0x001204F4 File Offset: 0x0011E6F4
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		writer.Write(this.upgradeSlots.Count);
		for (int i = 0; i < this.upgradeSlots.Count; i++)
		{
			if (this.upgradeSlots[i] != null)
			{
				if (this.upgradeSlots[i].installedItem != null)
				{
					writer.Write(this.upgradeSlots[i].installedItem.UpgradeType.ToString());
				}
				else
				{
					writer.Write("");
				}
			}
			else
			{
				writer.Write("");
			}
		}
		writer.Write(this.energy);
	}

	// Token: 0x0600347D RID: 13437 RVA: 0x00120598 File Offset: 0x0011E798
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			GRToolProgressionManager.ToolParts toolParts = GRToolProgressionManager.ToolParts.None;
			if (Enum.TryParse<GRToolProgressionManager.ToolParts>(reader.ReadString(), out toolParts))
			{
				this.UpgradeTool(toolParts);
			}
		}
		int num2 = reader.ReadInt32();
		this.SetEnergy(num2);
	}

	// Token: 0x0600347E RID: 13438 RVA: 0x001205E0 File Offset: 0x0011E7E0
	public void GrabbedByPlayer()
	{
		if (this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer)
			{
				grplayer.GrabbedItem(this.gameEntity.id, base.gameObject.name);
			}
		}
	}

	// Token: 0x0600347F RID: 13439 RVA: 0x0012063B File Offset: 0x0011E83B
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Tool Energy: <color=\"yellow\">{0}<color=\"white\"> ", this.energy));
	}

	// Token: 0x04004429 RID: 17449
	public GRAttributes attributes;

	// Token: 0x0400442A RID: 17450
	public List<GRTool.Upgrade> upgrades;

	// Token: 0x0400442B RID: 17451
	public List<GRTool.UpgradeSlot> upgradeSlots = new List<GRTool.UpgradeSlot>();

	// Token: 0x0400442C RID: 17452
	public List<GRMeterEnergy> energyMeters;

	// Token: 0x0400442D RID: 17453
	public GameEntity gameEntity;

	// Token: 0x0400442E RID: 17454
	public GRTool.GRToolType toolType;

	// Token: 0x0400442F RID: 17455
	[ReadOnly]
	public int energy;

	// Token: 0x04004431 RID: 17457
	public GameObject UpgradeFXNode;

	// Token: 0x04004433 RID: 17459
	private List<MeshFilter> reservedMeshFilterSearchList = new List<MeshFilter>(32);

	// Token: 0x04004434 RID: 17460
	private List<SkinnedMeshRenderer> reservedMeshFilterSearchListSkinned = new List<SkinnedMeshRenderer>(32);

	// Token: 0x04004435 RID: 17461
	private GRTool.Upgrade upgradeListsAreValidFor;

	// Token: 0x02000804 RID: 2052
	public enum GRToolType
	{
		// Token: 0x04004437 RID: 17463
		None,
		// Token: 0x04004438 RID: 17464
		Club,
		// Token: 0x04004439 RID: 17465
		Collector,
		// Token: 0x0400443A RID: 17466
		Flash,
		// Token: 0x0400443B RID: 17467
		Lantern,
		// Token: 0x0400443C RID: 17468
		Revive,
		// Token: 0x0400443D RID: 17469
		ShieldGun,
		// Token: 0x0400443E RID: 17470
		DirectionalShield,
		// Token: 0x0400443F RID: 17471
		DockWrist,
		// Token: 0x04004440 RID: 17472
		EnergyEfficiency,
		// Token: 0x04004441 RID: 17473
		DropPod,
		// Token: 0x04004442 RID: 17474
		HockeyStick,
		// Token: 0x04004443 RID: 17475
		StatusWatch,
		// Token: 0x04004444 RID: 17476
		RattyBackpack
	}

	// Token: 0x02000805 RID: 2053
	[Serializable]
	public class Upgrade
	{
		// Token: 0x04004445 RID: 17477
		public GRToolProgressionManager.ToolParts UpgradeType;

		// Token: 0x04004446 RID: 17478
		public int Slot;

		// Token: 0x04004447 RID: 17479
		public List<GameObject> VisibleItem;

		// Token: 0x04004448 RID: 17480
		public List<GRBonusEntry> bonusEffects;
	}

	// Token: 0x02000806 RID: 2054
	[Serializable]
	public class UpgradeSlot
	{
		// Token: 0x04004449 RID: 17481
		public List<GameObject> DefaultVisibleItems;

		// Token: 0x0400444A RID: 17482
		[NonSerialized]
		public GRTool.Upgrade installedItem;
	}

	// Token: 0x02000807 RID: 2055
	// (Invoke) Token: 0x06003484 RID: 13444
	public delegate void EnergyChangeEvent(GRTool tool, int energyChange, GameEntityId chargingEntityId);

	// Token: 0x02000808 RID: 2056
	// (Invoke) Token: 0x06003488 RID: 13448
	public delegate void ToolUpgradedEvent(GRTool tool);
}
