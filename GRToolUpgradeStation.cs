using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200082C RID: 2092
public class GRToolUpgradeStation : MonoBehaviour
{
	// Token: 0x060035BE RID: 13758 RVA: 0x00128CEF File Offset: 0x00126EEF
	public void Init(GRToolProgressionManager tree, GhostReactor reactor)
	{
		this._reactor = reactor;
		this.defaultCostText = this.CostText.text;
		this.toolProgressionManager = tree;
		this.toolProgressionManager.OnProgressionUpdated += this.ResearchTreeUpdated;
		this.ResetScreen();
	}

	// Token: 0x170004BF RID: 1215
	// (get) Token: 0x060035BF RID: 13759 RVA: 0x00128D2D File Offset: 0x00126F2D
	public bool canInsertTool
	{
		get
		{
			return this.currentState == GRToolUpgradeStation.UpgradeStationState.Idle && !this.bIsToolInserted;
		}
	}

	// Token: 0x060035C0 RID: 13760 RVA: 0x00128D42 File Offset: 0x00126F42
	public void ResearchTreeUpdated()
	{
		this.UpdateUI();
	}

	// Token: 0x060035C1 RID: 13761 RVA: 0x00128D4A File Offset: 0x00126F4A
	public void Update()
	{
		if (this.currentState == GRToolUpgradeStation.UpgradeStationState.Upgrading)
		{
			this.UpgradingUpdate(PhotonNetwork.Time);
		}
	}

	// Token: 0x060035C2 RID: 13762 RVA: 0x00128D60 File Offset: 0x00126F60
	public void ToolInserted(GRTool tool)
	{
		if (!this.canInsertTool)
		{
			return;
		}
		this.bIsToolInserted = true;
		this.insertedTool = tool;
		this.insertedToolType = this.insertedTool.toolType;
		this.selectedToolUpgrades = this.toolProgressionManager.GetToolUpgrades(this.insertedToolType);
		this.ResetScreen();
		this.UpdateUI();
		this.SelectUpgrade(0);
		this.LocalPlacedToolInUpgradeStation(tool.gameEntity.id);
	}

	// Token: 0x060035C3 RID: 13763 RVA: 0x00128DD0 File Offset: 0x00126FD0
	public void UpdateUI()
	{
		this.UpdateUpgradeTexts();
		this.UpdateSelectedUpgrade();
	}

	// Token: 0x060035C4 RID: 13764 RVA: 0x00128DE0 File Offset: 0x00126FE0
	public void UpdateUpgradeTexts()
	{
		this.ToolNameText.text = GRUtils.GetToolName(this.insertedToolType);
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.selectedToolUpgrades.Count > i)
			{
				this.UpgradeTitlesText[i].text = this.selectedToolUpgrades[i].partMetadata.name;
			}
			else
			{
				this.UpgradeTitlesText[i].text = null;
			}
		}
	}

	// Token: 0x060035C5 RID: 13765 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void UnlockAllUpgrades()
	{
	}

	// Token: 0x060035C6 RID: 13766 RVA: 0x00128E58 File Offset: 0x00127058
	public void UpdateSelectedUpgrade()
	{
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex && this.selectedToolUpgrades[this.selectedUpgradeIndex] != null)
		{
			if (this.selectedToolUpgrades[this.selectedUpgradeIndex].unlocked)
			{
				this.DescriptionText.text = this.selectedToolUpgrades[this.selectedUpgradeIndex].partMetadata.description;
				int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
				this.CostText.text = string.Format(this.defaultCostText, researchCost.ToString());
				GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
				this.CostText.color = ((researchCost > grplayer.ShiftCredits) ? this.lockedColor : this.unlockedColor);
				return;
			}
			this.CostText.text = "NEEDS RESEARCH";
			this.CostText.color = this.lockedColor;
		}
	}

	// Token: 0x060035C7 RID: 13767 RVA: 0x00128F60 File Offset: 0x00127160
	public void ResetScreen()
	{
		this.DescriptionText.text = "PLEASE INSERT A TOOL";
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			this.UpgradeTitlesText[i].text = "----";
			this.UpgradeTitlesText[i].color = this.lockedColor;
			this.MFD_ButtonTexts[i].color = this.unSelectedColor;
		}
		this.ToolNameText.text = "----";
		this.CostText.text = "-";
		this.ToolNameText.color = this.unSelectedColor;
		this.DescriptionText.color = this.unSelectedColor;
		this.CostText.color = this.unSelectedColor;
	}

	// Token: 0x060035C8 RID: 13768 RVA: 0x0012901C File Offset: 0x0012721C
	public void SelectUpgrade(int index)
	{
		if (index >= this.selectedToolUpgrades.Count)
		{
			return;
		}
		this.selectedUpgradeIndex = index;
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (i < this.selectedToolUpgrades.Count)
			{
				bool unlocked = this.selectedToolUpgrades[i].unlocked;
				this.UpgradeTitlesText[i].color = (unlocked ? this.unlockedColor : this.lockedColor);
				this.UpgradeLockedImage[i].gameObject.SetActive(!unlocked);
			}
			else
			{
				this.UpgradeLockedImage[i].gameObject.SetActive(true);
				this.UpgradeTitlesText[i].color = this.lockedColor;
			}
			this.UpgradeButtons[i].isOn = false;
			this.UpgradeButtons[i].UpdateColor();
		}
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex && this.selectedToolUpgrades[this.selectedUpgradeIndex] != null)
		{
			this.UpgradeButtons[this.selectedUpgradeIndex].isOn = true;
			this.UpgradeButtons[this.selectedUpgradeIndex].UpdateColor();
			this.DescriptionText.color = this.UpgradeTitlesText[this.selectedUpgradeIndex].color;
			this.CostText.color = this.UpgradeTitlesText[this.selectedUpgradeIndex].color;
		}
		this.UpdateUI();
	}

	// Token: 0x060035C9 RID: 13769 RVA: 0x00129185 File Offset: 0x00127385
	public void UpgradeTool()
	{
		this._reactor.grManager.ToolUpgradeStationRequestUpgrade(this.selectedToolUpgrades[this.selectedUpgradeIndex].type, this.insertedToolEntity.GetNetId());
	}

	// Token: 0x060035CA RID: 13770 RVA: 0x001291B8 File Offset: 0x001273B8
	public void LocalPlacedToolInUpgradeStation(GameEntityId entityId)
	{
		GameEntity gameEntity = this._reactor.grManager.gameEntityManager.GetGameEntity(entityId);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.ItemInserted;
		if (gameEntity.heldByActorNumber >= 0)
		{
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
			int num = gamePlayer.FindHandIndex(entityId);
			gamePlayer.ClearGrabbedIfHeld(entityId, gameEntity.manager);
			if (gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.gamePlayer.ClearGrabbed(num);
				GamePlayerLocal.instance.ClearGrabbed(num);
			}
			gameEntity.heldByActorNumber = -1;
			gameEntity.heldByHandIndex = -1;
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
			this.PositionInsertedTool(gameEntity);
			this.SelectUpgrade(0);
		}
	}

	// Token: 0x060035CB RID: 13771 RVA: 0x0012926C File Offset: 0x0012746C
	public void PositionInsertedTool(GameEntity entity)
	{
		this.insertedToolEntity = entity;
		entity.transform.SetParent(this.startingLocation);
		entity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		entity.pickupable = false;
	}

	// Token: 0x060035CC RID: 13772 RVA: 0x001292FC File Offset: 0x001274FC
	public void PayForUpgrade(int Player)
	{
		if (Player == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			bool flag = researchCost <= grplayer.ShiftCredits;
			bool unlocked = this.selectedToolUpgrades[this.selectedUpgradeIndex].unlocked;
			if (flag && unlocked)
			{
				UnityEvent onSucceeded = this.IDCardScanner.onSucceeded;
				if (onSucceeded != null)
				{
					onSucceeded.Invoke();
				}
				this.StartUpgrade(PhotonNetwork.Time);
			}
		}
	}

	// Token: 0x060035CD RID: 13773 RVA: 0x00129380 File Offset: 0x00127580
	public void StartUpgrade(double startTime)
	{
		if (this.currentState != GRToolUpgradeStation.UpgradeStationState.ItemInserted)
		{
			return;
		}
		this.upgradeStartTime = startTime;
		this.insertedToolEntity.transform.SetParent(this.startingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Upgrading;
	}

	// Token: 0x060035CE RID: 13774 RVA: 0x001293D5 File Offset: 0x001275D5
	public void UpgradingUpdate(double currentTime)
	{
		if (currentTime >= this.upgradeStartTime + this.upgradeAnimationLength)
		{
			this.CompleteUpgrade();
		}
	}

	// Token: 0x060035CF RID: 13775 RVA: 0x001293ED File Offset: 0x001275ED
	public void CompleteUpgrade()
	{
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Complete;
		this.ResetScreen();
		this.MoveToolToFinished();
	}

	// Token: 0x060035D0 RID: 13776 RVA: 0x00129404 File Offset: 0x00127604
	public void MoveItemToUpgradeSlot()
	{
		this.insertedToolEntity.transform.SetParent(this.upgradingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.upgradingLocation.position;
			component.rotation = this.upgradingLocation.rotation;
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = false;
	}

	// Token: 0x060035D1 RID: 13777 RVA: 0x001294A4 File Offset: 0x001276A4
	public void MoveToolToFinished()
	{
		this.insertedToolEntity.transform.SetParent(this.depositedLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Complete;
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.UpgradeTool();
		this.EjectToolFromEnd();
		this.ResetScreen();
	}

	// Token: 0x060035D2 RID: 13778 RVA: 0x0012956C File Offset: 0x0012776C
	public void EjectToolFromStart()
	{
		this.insertedToolEntity.transform.SetParent(this.startingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.insertedToolEntity.transform.SetParent(null, true);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.insertedToolEntity = null;
		this.insertedTool = null;
		this.insertedToolType = GRTool.GRToolType.None;
		this.bIsToolInserted = false;
		this.ResetScreen();
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Idle;
	}

	// Token: 0x060035D3 RID: 13779 RVA: 0x00129658 File Offset: 0x00127858
	public void EjectToolFromEnd()
	{
		this.insertedToolEntity.transform.SetParent(this.depositedLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.insertedToolEntity.transform.SetParent(null, true);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.depositedLocation.position;
			component.rotation = this.depositedLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.insertedToolEntity = null;
		this.insertedTool = null;
		this.insertedToolType = GRTool.GRToolType.None;
		this.bIsToolInserted = false;
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Idle;
	}

	// Token: 0x0400464B RID: 17995
	private GRTool insertedTool;

	// Token: 0x0400464C RID: 17996
	private GRTool.GRToolType insertedToolType;

	// Token: 0x0400464D RID: 17997
	private GameEntity insertedToolEntity;

	// Token: 0x0400464E RID: 17998
	[NonSerialized]
	private GhostReactor _reactor;

	// Token: 0x0400464F RID: 17999
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x04004650 RID: 18000
	[NonSerialized]
	private List<GRToolProgressionTree.GRToolProgressionNode> selectedToolUpgrades = new List<GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x04004651 RID: 18001
	[NonSerialized]
	public bool bIsToolInserted;

	// Token: 0x04004652 RID: 18002
	public Transform startingLocation;

	// Token: 0x04004653 RID: 18003
	public Transform upgradingLocation;

	// Token: 0x04004654 RID: 18004
	public Transform depositedLocation;

	// Token: 0x04004655 RID: 18005
	public Transform ejectionTransform;

	// Token: 0x04004656 RID: 18006
	public float ejectionVelocity;

	// Token: 0x04004657 RID: 18007
	public Color selectedColor;

	// Token: 0x04004658 RID: 18008
	public Color unSelectedColor;

	// Token: 0x04004659 RID: 18009
	public Color lockedColor;

	// Token: 0x0400465A RID: 18010
	public Color unlockedColor;

	// Token: 0x0400465B RID: 18011
	public TMP_Text[] UpgradeTitlesText;

	// Token: 0x0400465C RID: 18012
	public TMP_Text[] MFD_ButtonTexts;

	// Token: 0x0400465D RID: 18013
	public GorillaPressableButton[] UpgradeButtons;

	// Token: 0x0400465E RID: 18014
	public Image[] UpgradeLockedImage;

	// Token: 0x0400465F RID: 18015
	public TMP_Text ToolNameText;

	// Token: 0x04004660 RID: 18016
	public TMP_Text DescriptionText;

	// Token: 0x04004661 RID: 18017
	public TMP_Text CostText;

	// Token: 0x04004662 RID: 18018
	private string defaultCostText;

	// Token: 0x04004663 RID: 18019
	public IDCardScanner IDCardScanner;

	// Token: 0x04004664 RID: 18020
	private int selectedUpgradeIndex;

	// Token: 0x04004665 RID: 18021
	private double upgradeStartTime;

	// Token: 0x04004666 RID: 18022
	public double upgradeAnimationLength;

	// Token: 0x04004667 RID: 18023
	public Vector3 rotationAnimation;

	// Token: 0x04004668 RID: 18024
	private GRToolUpgradeStation.UpgradeStationState currentState;

	// Token: 0x04004669 RID: 18025
	public GameEntity attachedItem;

	// Token: 0x0200082D RID: 2093
	private enum UpgradeStationState
	{
		// Token: 0x0400466B RID: 18027
		Idle,
		// Token: 0x0400466C RID: 18028
		ItemInserted,
		// Token: 0x0400466D RID: 18029
		Upgrading,
		// Token: 0x0400466E RID: 18030
		Complete
	}
}
