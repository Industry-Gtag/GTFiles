using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaTag;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200016B RID: 363
[DefaultExecutionOrder(100)]
public class SITechTreeStation : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x170000CE RID: 206
	// (get) Token: 0x06000991 RID: 2449 RVA: 0x00033024 File Offset: 0x00031224
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x06000992 RID: 2450 RVA: 0x0003302C File Offset: 0x0003122C
	public SITechTreeNode CurrentNode
	{
		get
		{
			return this.techTreeSO.GetTreeNode(this.parentTerminal.ActivePage, this.currentNodeId);
		}
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x06000993 RID: 2451 RVA: 0x0003304A File Offset: 0x0003124A
	public SITechTreePage CurrentPage
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO.GetTreePage((SITechTreePageId)this.parentTerminal.ActivePage);
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x06000994 RID: 2452 RVA: 0x0003306C File Offset: 0x0003126C
	public SIPlayer ActivePlayer
	{
		get
		{
			return this.parentTerminal.activePlayer;
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x06000995 RID: 2453 RVA: 0x00033079 File Offset: 0x00031279
	public string ActivePlayerName
	{
		get
		{
			NetPlayer creator = this.ActivePlayer.gamePlayer.rig.Creator;
			if (creator == null)
			{
				return null;
			}
			return creator.SanitizedNickName;
		}
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x06000996 RID: 2454 RVA: 0x0003309B File Offset: 0x0003129B
	public bool IsAuthority
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x06000997 RID: 2455 RVA: 0x000330B7 File Offset: 0x000312B7
	public GameEntityManager GameEntityManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager;
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x06000998 RID: 2456 RVA: 0x000330CE File Offset: 0x000312CE
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager;
		}
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x000330E0 File Offset: 0x000312E0
	private void CollectButtonColliders()
	{
		SITechTreeStation.<>c__DisplayClass75_0 CS$<>8__locals1;
		CS$<>8__locals1.buttons = base.GetComponentsInChildren<SITouchscreenButton>(true).ToList<SITouchscreenButton>();
		SITechTreeStation.<CollectButtonColliders>g__RemoveButtonsInside|75_2((from d in base.GetComponentsInChildren<DestroyIfNotBeta>()
			select d.gameObject).ToArray<GameObject>(), ref CS$<>8__locals1);
		SITechTreeStation.<CollectButtonColliders>g__RemoveButtonsInside|75_2(new GameObject[] { this.techTreeHelpScreen, this.nodePopupScreen }, ref CS$<>8__locals1);
		this._nonPopupButtonColliders = CS$<>8__locals1.buttons.Select((SITouchscreenButton b) => b.GetComponent<Collider>()).ToList<Collider>();
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0003318C File Offset: 0x0003138C
	private void SetNonPopupButtonsEnabled(bool enable)
	{
		foreach (Collider collider in this._nonPopupButtonColliders)
		{
			collider.enabled = enable;
		}
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x000331E0 File Offset: 0x000313E0
	private void OnEnable()
	{
		SIProgression instance = SIProgression.Instance;
		instance.OnTreeReady = (Action)Delegate.Combine(instance.OnTreeReady, new Action(this.OnProgressionUpdate));
		SIProgression instance2 = SIProgression.Instance;
		instance2.OnInventoryReady = (Action)Delegate.Combine(instance2.OnInventoryReady, new Action(this.OnProgressionUpdate));
		SIProgression instance3 = SIProgression.Instance;
		instance3.OnNodeUnlocked = (Action<SIUpgradeType>)Delegate.Combine(instance3.OnNodeUnlocked, new Action<SIUpgradeType>(this.OnProgressionUpdateNode));
		this._RefreshButtonsUsableState();
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x00033268 File Offset: 0x00031468
	private void OnDisable()
	{
		SIProgression instance = SIProgression.Instance;
		instance.OnTreeReady = (Action)Delegate.Remove(instance.OnTreeReady, new Action(this.OnProgressionUpdate));
		SIProgression instance2 = SIProgression.Instance;
		instance2.OnInventoryReady = (Action)Delegate.Remove(instance2.OnInventoryReady, new Action(this.OnProgressionUpdate));
		SIProgression instance3 = SIProgression.Instance;
		instance3.OnNodeUnlocked = (Action<SIUpgradeType>)Delegate.Remove(instance3.OnNodeUnlocked, new Action<SIUpgradeType>(this.OnProgressionUpdateNode));
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x000332E8 File Offset: 0x000314E8
	public void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		if (this.parentTerminal == null)
		{
			this.parentTerminal = base.GetComponentInParent<SICombinedTerminal>();
		}
		this.screenData = new Dictionary<SITechTreeStation.TechTreeStationTerminalState, GameObject>();
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan, this.waitingForScanScreen);
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList, this.pagesListScreen);
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.TechTreePage, this.pageScreen);
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup, this.nodePopupScreen);
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.HelpScreen, this.techTreeHelpScreen);
		this.techTreeSO.EnsureInitialized();
		this.pageButtons = new List<SIGadgetListEntry>();
		this.techTreePages = new List<SITechTreeUIPage>();
		this.spriteByType.Add(SIResource.ResourceType.TechPoint, this.techPointSprite);
		this.spriteByType.Add(SIResource.ResourceType.StrangeWood, this.strangeWoodSprite);
		this.spriteByType.Add(SIResource.ResourceType.WeirdGear, this.weirdGearSprite);
		this.spriteByType.Add(SIResource.ResourceType.VibratingSpring, this.vibratingSpringSprite);
		this.spriteByType.Add(SIResource.ResourceType.BouncySand, this.bouncySandSprite);
		this.spriteByType.Add(SIResource.ResourceType.FloppyMetal, this.floppyMetalSprite);
		int count = this.techTreeSO.TreePages.Count;
		for (int i = 0; i < count; i++)
		{
			SITechTreePage sitechTreePage = this.techTreeSO.TreePages[i];
			if (sitechTreePage.IsValid)
			{
				this.techTreeIconById.Add(sitechTreePage.pageId, sitechTreePage.icon);
				SIGadgetListEntry sigadgetListEntry = Object.Instantiate<SIGadgetListEntry>(this.pageListEntryPrefab, this.pageListParent);
				StaticLodManager.TryAddLateInstantiatedMembers(sigadgetListEntry.gameObject);
				sigadgetListEntry.Configure(this, sitechTreePage, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText, SITouchscreenButton.SITouchscreenButtonType.PageSelect, i, -0.07f, count);
				this.pageButtons.Add(sigadgetListEntry);
				SITechTreeUIPage sitechTreeUIPage = Object.Instantiate<SITechTreeUIPage>(this.pagePrefab, this.pageParent);
				StaticLodManager.TryAddLateInstantiatedMembers(sitechTreeUIPage.gameObject);
				sitechTreeUIPage.Configure(this, sitechTreePage, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText);
				this.techTreePages.Add(sitechTreeUIPage);
			}
		}
		this.Reset();
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0003350C File Offset: 0x0003170C
	private void _RefreshButtonsUsableState()
	{
		foreach (SIGadgetListEntry sigadgetListEntry in this.pageButtons)
		{
			SITechTreePageId id = (SITechTreePageId)sigadgetListEntry.Id;
			SITechTreePage sitechTreePage;
			if (this.techTreeSO.TryGetTreePage(id, out sitechTreePage))
			{
				sigadgetListEntry.ButtonContainer.SetUsable(sitechTreePage.IsAllowed);
			}
		}
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00033580 File Offset: 0x00031780
	public void Reset()
	{
		this.currentState = SITechTreeStation.TechTreeStationTerminalState.WaitingForScan;
		this.nodePopupState = SITechTreeStation.NodePopupState.Description;
		this.SetScreenVisibility(this.currentState, this.currentState);
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x000335A4 File Offset: 0x000317A4
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy)
		{
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan, SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
		}
		stream.SendNext(this.currentNodeId);
		stream.SendNext(this.helpScreenIndex);
		stream.SendNext((int)this.nodePopupState);
		stream.SendNext((int)this.currentState);
		stream.SendNext((int)this.lastState);
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00033630 File Offset: 0x00031830
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.currentNodeId = (int)stream.ReceiveNext();
		if (this.CurrentNode == null)
		{
			this.currentNodeId = (int)this.CurrentPage.AllNodes[0].Value.upgradeType;
		}
		this.helpScreenIndex = Mathf.Clamp((int)stream.ReceiveNext(), 0, this.helpPopupScreens.Length - 1);
		this.nodePopupState = (SITechTreeStation.NodePopupState)stream.ReceiveNext();
		if (!Enum.IsDefined(typeof(SITechTreeStation.NodePopupState), this.nodePopupState))
		{
			this.nodePopupState = SITechTreeStation.NodePopupState.Description;
		}
		SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState = (SITechTreeStation.TechTreeStationTerminalState)stream.ReceiveNext();
		SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState2 = (SITechTreeStation.TechTreeStationTerminalState)stream.ReceiveNext();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SITechTreeStation.TechTreeStationTerminalState), techTreeStationTerminalState) || !Enum.IsDefined(typeof(SITechTreeStation.TechTreeStationTerminalState), techTreeStationTerminalState2))
		{
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan, SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(techTreeStationTerminalState, techTreeStationTerminalState2);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00033743 File Offset: 0x00031943
	public void ZoneDataSerializeWrite(BinaryWriter writer)
	{
		writer.Write(this.currentNodeId);
		writer.Write(this.helpScreenIndex);
		writer.Write((int)this.nodePopupState);
		writer.Write((int)this.currentState);
		writer.Write((int)this.lastState);
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00033784 File Offset: 0x00031984
	public void ZoneDataSerializeRead(BinaryReader reader)
	{
		this.currentNodeId = reader.ReadInt32();
		if (this.CurrentNode == null || !Enum.IsDefined(typeof(SIUpgradeType), this.CurrentNode.upgradeType))
		{
			GTDev.LogError<string>(string.Format("SITechTreeStation.ZoneDataSerializeRead: Invalid currentNodeId {0} for page {1}. Falling back to first node.", this.currentNodeId, this.parentTerminal.ActivePage), null);
			this.currentNodeId = (int)this.CurrentPage.AllNodes[0].Value.upgradeType;
		}
		this.helpScreenIndex = Mathf.Clamp(reader.ReadInt32(), 0, this.helpPopupScreens.Length - 1);
		this.nodePopupState = (SITechTreeStation.NodePopupState)reader.ReadInt32();
		if (!Enum.IsDefined(typeof(SITechTreeStation.NodePopupState), this.nodePopupState))
		{
			this.nodePopupState = SITechTreeStation.NodePopupState.Description;
		}
		SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState = (SITechTreeStation.TechTreeStationTerminalState)reader.ReadInt32();
		SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState2 = (SITechTreeStation.TechTreeStationTerminalState)reader.ReadInt32();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SITechTreeStation.TechTreeStationTerminalState), techTreeStationTerminalState) || !Enum.IsDefined(typeof(SITechTreeStation.TechTreeStationTerminalState), techTreeStationTerminalState2))
		{
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan, SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(techTreeStationTerminalState, techTreeStationTerminalState2);
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x000338CA File Offset: 0x00031ACA
	public void UpdateState(SITechTreeStation.TechTreeStationTerminalState newState, SITechTreeStation.TechTreeStationTerminalState newLastState)
	{
		if (!this.IsPopupState(newLastState))
		{
			this.currentState = newLastState;
		}
		this.UpdateState(newState);
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x000338E4 File Offset: 0x00031AE4
	public void UpdateState(SITechTreeStation.TechTreeStationTerminalState newState)
	{
		if (!this.IsPopupState(this.currentState))
		{
			this.lastState = this.currentState;
		}
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, this.lastState);
		switch (this.currentState)
		{
		case SITechTreeStation.TechTreeStationTerminalState.WaitingForScan:
			break;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList:
			this.playerNameText.text = this.ActivePlayerName;
			this.screenDescriptionText.text = "TECH TREE PAGES";
			return;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreePage:
		{
			this.playerNameText.text = this.ActivePlayerName;
			this.UpdateNodeData(this.ActivePlayer);
			TMP_Text tmp_Text = this.screenDescriptionText;
			SITechTreePage treePage = this.techTreeSO.GetTreePage((SITechTreePageId)this.parentTerminal.ActivePage);
			tmp_Text.text = ((treePage != null) ? treePage.nickName : null);
			foreach (SIGadgetListEntry sigadgetListEntry in this.pageButtons)
			{
				sigadgetListEntry.selectionIndicator.SetActive(sigadgetListEntry.Id == this.parentTerminal.ActivePage);
			}
			foreach (SITechTreeUIPage sitechTreeUIPage in this.techTreePages)
			{
				sitechTreeUIPage.gameObject.SetActive(sitechTreeUIPage.id == (SITechTreePageId)this.parentTerminal.ActivePage);
			}
			Sprite sprite;
			this.techTreeIconById.TryGetValue((SITechTreePageId)this.parentTerminal.ActivePage, out sprite);
			this.techTreeIcon.sprite = sprite;
			return;
		}
		case SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup:
			switch (this.nodePopupState)
			{
			case SITechTreeStation.NodePopupState.Description:
				this.nodeNameText.text = this.CurrentNode.nickName;
				this.nodeDescriptionText.text = this.CurrentNode.description;
				if (this.ActivePlayer.NodeResearched(this.CurrentNode.upgradeType))
				{
					this.nodeResearched.SetActive(true);
					this.nodeLocked.SetActive(false);
					this.nodeAvailable.SetActive(false);
					this.nodeResearchButton.SetActive(false);
					this.canAffordNode.SetActive(false);
					this.cantAffordNode.SetActive(false);
				}
				else if (this.ActivePlayer.NodeParentsUnlocked(this.CurrentNode.upgradeType))
				{
					this.nodeResearched.SetActive(false);
					this.nodeLocked.SetActive(false);
					this.nodeAvailable.SetActive(true);
					this.nodeResearchButton.SetActive(true);
					bool flag = this.ActivePlayer.PlayerCanAffordNode(this.CurrentNode);
					this.canAffordNode.SetActive(flag);
					this.cantAffordNode.SetActive(!flag);
				}
				else
				{
					this.nodeResearched.SetActive(false);
					this.nodeAvailable.SetActive(false);
					this.nodeLocked.SetActive(true);
					this.nodeResearchButton.SetActive(false);
					this.canAffordNode.SetActive(false);
					this.cantAffordNode.SetActive(false);
				}
				this.nodeResourceTypeText.text = this.FormattedCurrentResourceTypesForNode(this.CurrentNode);
				this.nodeResourceCostText.text = this.FormattedResearchCost(this.CurrentNode);
				this.playerCurrentResourceAmountsText.text = this.FormattedCurrentResourceAmountsForNode(this.CurrentNode);
				break;
			case SITechTreeStation.NodePopupState.NotEnoughResources:
				this.nodeNameResearchMessageText.text = "NOT ENOUGH RESOURCES TO UNLOCK NODE! GATHER MORE AND TRY AGAIN!";
				break;
			case SITechTreeStation.NodePopupState.Success:
				this.nodeNameResearchMessageText.text = "SUCCESSFULLY UNLOCKED TECH NODE!";
				break;
			case SITechTreeStation.NodePopupState.Loading:
				if (this.ActivePlayer.NodeResearched(this.CurrentNode.upgradeType))
				{
					this.nodePopupState = SITechTreeStation.NodePopupState.Success;
					this.nodeNameResearchMessageText.text = "SUCCESSFULLY UNLOCKED TECH NODE!";
				}
				else
				{
					this.nodeNameResearchMessageText.text = "ATTEMPTING TO UNLOCK NODE\n\nLOADING . . .";
				}
				break;
			}
			this.UpdateNodePopupPage();
			return;
		case SITechTreeStation.TechTreeStationTerminalState.HelpScreen:
			this.UpdateHelpButtonPage(this.helpScreenIndex);
			break;
		default:
			return;
		}
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00033CCC File Offset: 0x00031ECC
	public void SetScreenVisibility(SITechTreeStation.TechTreeStationTerminalState currentState, SITechTreeStation.TechTreeStationTerminalState lastState)
	{
		bool flag = this.IsPopupState(currentState);
		this.background.color = ((currentState == SITechTreeStation.TechTreeStationTerminalState.WaitingForScan) ? Color.white : ((this.ActivePlayer != null && this.ActivePlayer.gamePlayer.IsLocal()) ? this.active : this.notActive));
		foreach (SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState in this.screenData.Keys)
		{
			if (techTreeStationTerminalState == SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList)
			{
				this.screenData[techTreeStationTerminalState].SetActive(currentState > SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
			}
			else
			{
				bool flag2 = techTreeStationTerminalState == currentState || (flag && techTreeStationTerminalState == lastState);
				if (this.screenData[techTreeStationTerminalState].activeSelf != flag2)
				{
					this.screenData[techTreeStationTerminalState].SetActive(flag2);
				}
			}
		}
		if (this.popupScreen.activeSelf != flag)
		{
			this.popupScreen.SetActive(flag);
		}
		bool flag3 = currentState > SITechTreeStation.TechTreeStationTerminalState.WaitingForScan;
		this.screenDescriptionText.gameObject.SetActive(flag3);
		this.playerNameText.gameObject.SetActive(flag3);
		this.SetNonPopupButtonsEnabled(!flag);
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00033E0C File Offset: 0x0003200C
	public bool IsPopupState(SITechTreeStation.TechTreeStationTerminalState state)
	{
		return state == SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup || state == SITechTreeStation.TechTreeStationTerminalState.HelpScreen;
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x00033E18 File Offset: 0x00032018
	public void PlayerHandScanned(int actorNr)
	{
		if (!this.IsAuthority)
		{
			this.parentTerminal.PlayerHandScanned(actorNr);
			return;
		}
		this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePage);
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00033E36 File Offset: 0x00032036
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
		if (!isPopupButton)
		{
			this._nonPopupButtonColliders.Add(button.GetComponent<Collider>());
		}
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00033E4C File Offset: 0x0003204C
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && (this.ActivePlayer == null || this.ActivePlayer != SIPlayer.LocalPlayer))
		{
			this.parentTerminal.PlayWrongPlayerBuzz(this.uiCenter);
		}
		else
		{
			this.soundBankPlayer.Play();
		}
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && this.ActivePlayer == SIPlayer.LocalPlayer && this.currentState == SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup && this.nodePopupState == SITechTreeStation.NodePopupState.Description && buttonType == SITouchscreenButton.SITouchscreenButtonType.Research && !SIPlayer.LocalPlayer.NodeResearched(this.CurrentNode.upgradeType) && SIPlayer.LocalPlayer.NodeParentsUnlocked(this.CurrentNode.upgradeType))
		{
			SIProgression.Instance.TryUnlock(this.CurrentNode.upgradeType);
		}
		if (!this.IsAuthority)
		{
			this.parentTerminal.TouchscreenButtonPressed(buttonType, data, actorNr, SICombinedTerminal.TerminalSubFunction.TechTree);
			return;
		}
		if (this.ActivePlayer == null || actorNr != this.ActivePlayer.ActorNr)
		{
			return;
		}
		this.soundBankPlayer.Play();
		if (buttonType == SITouchscreenButton.SITouchscreenButtonType.PageSelect)
		{
			this.parentTerminal.SetActivePage(data);
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePage);
			return;
		}
		switch (this.currentState)
		{
		case SITechTreeStation.TechTreeStationTerminalState.WaitingForScan:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.HelpScreen);
			}
			return;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.HelpScreen);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				this.parentTerminal.SetActivePage(data);
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePage);
			}
			return;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreePage:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				this.currentNodeId = data;
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.HelpScreen);
				return;
			}
			return;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup:
			if (this.nodePopupState == SITechTreeStation.NodePopupState.Description)
			{
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
				{
					this.UpdateState(this.lastState);
				}
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Research)
				{
					if (this.ActivePlayer.PlayerCanAffordNode(this.CurrentNode))
					{
						this.nodePopupState = SITechTreeStation.NodePopupState.Loading;
					}
					else
					{
						this.nodePopupState = SITechTreeStation.NodePopupState.NotEnoughResources;
					}
					this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup);
					return;
				}
			}
			else if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.nodePopupState = SITechTreeStation.NodePopupState.Description;
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup);
			}
			return;
		case SITechTreeStation.TechTreeStationTerminalState.HelpScreen:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.helpScreenIndex = 0;
				this.UpdateState(this.lastState);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Next)
			{
				this.helpScreenIndex = Mathf.Clamp(this.helpScreenIndex + 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.helpScreenIndex);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.helpScreenIndex = Mathf.Clamp(this.helpScreenIndex - 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.helpScreenIndex);
			}
			return;
		default:
			return;
		}
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
	{
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x000340C4 File Offset: 0x000322C4
	public void UpdateHelpButtonPage(int helpButtonPageIndex)
	{
		for (int i = 0; i < this.helpPopupScreens.Length; i++)
		{
			this.helpPopupScreens[i].SetActive(i == helpButtonPageIndex);
		}
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x000340F8 File Offset: 0x000322F8
	public void UpdateNodePopupPage()
	{
		int num = ((this.nodePopupState == SITechTreeStation.NodePopupState.Description) ? 0 : 1);
		if (this.nodePopupScreens[0].activeSelf != (num == 0))
		{
			this.nodePopupScreens[0].SetActive(num == 0);
		}
		if (this.nodePopupScreens[1].activeSelf != (num == 1))
		{
			this.nodePopupScreens[1].SetActive(num == 1);
		}
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0003415C File Offset: 0x0003235C
	public void UpdateNodeData(SIPlayer player)
	{
		if (player == null)
		{
			for (int i = 0; i < this.techTreePages.Count; i++)
			{
				this.techTreePages[i].PopulateDefaultNodeData();
			}
			return;
		}
		for (int j = 0; j < this.techTreePages.Count; j++)
		{
			this.techTreePages[j].PopulatePlayerNodeData(player);
		}
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x000341C4 File Offset: 0x000323C4
	public string FormattedResearchCost(SITechTreeNode node)
	{
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			string text = "";
			text = text + sinode.costs[SIResource.ResourceType.TechPoint].ToString() + "\n";
			foreach (KeyValuePair<SIResource.ResourceType, int> keyValuePair in sinode.costs)
			{
				if (keyValuePair.Key != SIResource.ResourceType.TechPoint)
				{
					text += keyValuePair.Value.ToString();
					return text;
				}
			}
			return text;
		}
		return string.Join<int>("\n", node.nodeCost.Select((SIResource.ResourceCost c) => c.amount));
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x000342A8 File Offset: 0x000324A8
	public string FormattedCurrentResourceAmountsForNode(SITechTreeNode node)
	{
		string text = "";
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			text = text + this.ActivePlayer.CurrentProgression.resourceArray[0].ToString() + "\n";
			using (Dictionary<SIResource.ResourceType, int>.Enumerator enumerator = sinode.costs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SIResource.ResourceType, int> keyValuePair = enumerator.Current;
					if (keyValuePair.Key != SIResource.ResourceType.TechPoint)
					{
						text = text + this.ActivePlayer.CurrentProgression.resourceArray[(int)keyValuePair.Key].ToString() + "\n";
					}
				}
				return text;
			}
		}
		for (int i = 0; i < node.nodeCost.Length; i++)
		{
			text = text + this.ActivePlayer.CurrentProgression.resourceArray[(int)node.nodeCost[i].type].ToString() + "\n";
		}
		return text;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x000343C0 File Offset: 0x000325C0
	public string FormattedCurrentResourceTypesForNode(SITechTreeNode node)
	{
		string text = "";
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			text = text + SIResource.ResourceType.TechPoint.ToString().ToUpperInvariant() + "\n";
			using (Dictionary<SIResource.ResourceType, int>.Enumerator enumerator = sinode.costs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SIResource.ResourceType, int> keyValuePair = enumerator.Current;
					if (keyValuePair.Key != SIResource.ResourceType.TechPoint)
					{
						text = text + keyValuePair.Key.ToString().ToUpperInvariant() + "\n";
						this.resourceCost.sprite = this.spriteByType[keyValuePair.Key];
					}
				}
				return text;
			}
		}
		for (int i = 0; i < node.nodeCost.Length; i++)
		{
			text = text + node.nodeCost[i].type.ToString().ToUpperInvariant() + "\n";
		}
		return text;
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x000344DC File Offset: 0x000326DC
	private void OnProgressionUpdate()
	{
		this.UpdateNodeData(this.ActivePlayer);
		this.UpdateState(this.currentState);
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x000344F6 File Offset: 0x000326F6
	private void OnProgressionUpdateNode(SIUpgradeType type)
	{
		this.OnProgressionUpdate();
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00034500 File Offset: 0x00032700
	public void SetActivePage()
	{
		if (this.CurrentNode == null)
		{
			this.currentNodeId = this.CurrentPage.AllNodes[0].Value.upgradeType.GetNodeId();
		}
		if (this.ActivePlayer != null)
		{
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePage);
			return;
		}
		this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00034558 File Offset: 0x00032758
	public bool IsValidPage(int pageId)
	{
		if (pageId < 0)
		{
			return false;
		}
		using (List<SITechTreeUIPage>.Enumerator enumerator = this.techTreePages.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.id == (SITechTreePageId)pageId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x000066D3 File Offset: 0x000048D3
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x000345F0 File Offset: 0x000327F0
	[CompilerGenerated]
	internal static void <CollectButtonColliders>g__RemoveButtonsInside|75_2(GameObject[] roots, ref SITechTreeStation.<>c__DisplayClass75_0 A_1)
	{
		for (int i = 0; i < roots.Length; i++)
		{
			foreach (SITouchscreenButton sitouchscreenButton in roots[i].GetComponentsInChildren<SITouchscreenButton>(true))
			{
				A_1.buttons.Remove(sitouchscreenButton);
			}
		}
	}

	// Token: 0x04000B93 RID: 2963
	private const string preLog = "[GT/SITechTreeStation]  ";

	// Token: 0x04000B94 RID: 2964
	private const string preErr = "ERROR!!!  ";

	// Token: 0x04000B95 RID: 2965
	private Dictionary<SITechTreeStation.TechTreeStationTerminalState, GameObject> screenData;

	// Token: 0x04000B96 RID: 2966
	public SITechTreeStation.TechTreeStationTerminalState currentState;

	// Token: 0x04000B97 RID: 2967
	public SITechTreeStation.TechTreeStationTerminalState lastState;

	// Token: 0x04000B98 RID: 2968
	public SICombinedTerminal parentTerminal;

	// Token: 0x04000B99 RID: 2969
	public Sprite techPointSprite;

	// Token: 0x04000B9A RID: 2970
	public Sprite strangeWoodSprite;

	// Token: 0x04000B9B RID: 2971
	public Sprite weirdGearSprite;

	// Token: 0x04000B9C RID: 2972
	public Sprite vibratingSpringSprite;

	// Token: 0x04000B9D RID: 2973
	public Sprite bouncySandSprite;

	// Token: 0x04000B9E RID: 2974
	public Sprite floppyMetalSprite;

	// Token: 0x04000B9F RID: 2975
	public int currentNodeId;

	// Token: 0x04000BA0 RID: 2976
	public SITechTreeSO techTreeSO;

	// Token: 0x04000BA1 RID: 2977
	public GameObject waitingForScanScreen;

	// Token: 0x04000BA2 RID: 2978
	public GameObject pagesListScreen;

	// Token: 0x04000BA3 RID: 2979
	public GameObject pageScreen;

	// Token: 0x04000BA4 RID: 2980
	public GameObject nodePopupScreen;

	// Token: 0x04000BA5 RID: 2981
	public GameObject techTreeHelpScreen;

	// Token: 0x04000BA6 RID: 2982
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x04000BA7 RID: 2983
	public Color active;

	// Token: 0x04000BA8 RID: 2984
	public Color notActive;

	// Token: 0x04000BA9 RID: 2985
	[Header("Main Screen Shared")]
	public TextMeshProUGUI screenDescriptionText;

	// Token: 0x04000BAA RID: 2986
	public TextMeshProUGUI playerNameText;

	// Token: 0x04000BAB RID: 2987
	public Image background;

	// Token: 0x04000BAC RID: 2988
	public Transform uiCenter;

	// Token: 0x04000BAD RID: 2989
	[Header("Popup Shared")]
	public GameObject popupScreen;

	// Token: 0x04000BAE RID: 2990
	[Header("Pages List")]
	[SerializeField]
	private Transform pageListParent;

	// Token: 0x04000BAF RID: 2991
	[SerializeField]
	private SIGadgetListEntry pageListEntryPrefab;

	// Token: 0x04000BB0 RID: 2992
	private List<SIGadgetListEntry> pageButtons = new List<SIGadgetListEntry>(11);

	// Token: 0x04000BB1 RID: 2993
	[Header("Tree Page")]
	[SerializeField]
	private Transform pageParent;

	// Token: 0x04000BB2 RID: 2994
	[SerializeField]
	private SITechTreeUIPage pagePrefab;

	// Token: 0x04000BB3 RID: 2995
	private List<SITechTreeUIPage> techTreePages = new List<SITechTreeUIPage>(11);

	// Token: 0x04000BB4 RID: 2996
	[SerializeField]
	private SpriteRenderer techTreeIcon;

	// Token: 0x04000BB5 RID: 2997
	[Header("Node Popup")]
	public GameObject[] nodePopupScreens;

	// Token: 0x04000BB6 RID: 2998
	[Header("Research Node Description")]
	public TextMeshProUGUI nodeNameText;

	// Token: 0x04000BB7 RID: 2999
	public TextMeshProUGUI nodeDescriptionText;

	// Token: 0x04000BB8 RID: 3000
	public TextMeshProUGUI nodeResourceTypeText;

	// Token: 0x04000BB9 RID: 3001
	public TextMeshProUGUI nodeResourceCostText;

	// Token: 0x04000BBA RID: 3002
	public TextMeshProUGUI playerCurrentResourceAmountsText;

	// Token: 0x04000BBB RID: 3003
	public GameObject nodeAvailable;

	// Token: 0x04000BBC RID: 3004
	public GameObject nodeLocked;

	// Token: 0x04000BBD RID: 3005
	public GameObject nodeResearched;

	// Token: 0x04000BBE RID: 3006
	public GameObject canAffordNode;

	// Token: 0x04000BBF RID: 3007
	public GameObject cantAffordNode;

	// Token: 0x04000BC0 RID: 3008
	public GameObject nodeResearchButton;

	// Token: 0x04000BC1 RID: 3009
	public SpriteRenderer techPointCost;

	// Token: 0x04000BC2 RID: 3010
	public SpriteRenderer resourceCost;

	// Token: 0x04000BC3 RID: 3011
	[Header("Research Attempt")]
	public TextMeshProUGUI nodeNameResearchMessageText;

	// Token: 0x04000BC4 RID: 3012
	public SITechTreeStation.NodePopupState nodePopupState;

	// Token: 0x04000BC5 RID: 3013
	[Header("Help")]
	public int helpScreenIndex;

	// Token: 0x04000BC6 RID: 3014
	public GameObject[] helpPopupScreens;

	// Token: 0x04000BC7 RID: 3015
	[Header("Audio")]
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04000BC8 RID: 3016
	[Header("Main Screen Colliders")]
	[Tooltip("Button colliders to disable while popup screen is shown.  Gets updated live to include page and gadget node buttons.")]
	[SerializeField]
	private List<Collider> _nonPopupButtonColliders;

	// Token: 0x04000BC9 RID: 3017
	private Dictionary<SIResource.ResourceType, Sprite> spriteByType = new Dictionary<SIResource.ResourceType, Sprite>();

	// Token: 0x04000BCA RID: 3018
	private Dictionary<SITechTreePageId, Sprite> techTreeIconById = new Dictionary<SITechTreePageId, Sprite>();

	// Token: 0x04000BCB RID: 3019
	private bool initialized;

	// Token: 0x0200016C RID: 364
	public enum NodePopupState
	{
		// Token: 0x04000BCD RID: 3021
		Description,
		// Token: 0x04000BCE RID: 3022
		NotEnoughResources,
		// Token: 0x04000BCF RID: 3023
		Success,
		// Token: 0x04000BD0 RID: 3024
		PurchaseInitiation,
		// Token: 0x04000BD1 RID: 3025
		Loading
	}

	// Token: 0x0200016D RID: 365
	public enum TechTreeStationTerminalState
	{
		// Token: 0x04000BD3 RID: 3027
		WaitingForScan,
		// Token: 0x04000BD4 RID: 3028
		TechTreePagesList,
		// Token: 0x04000BD5 RID: 3029
		TechTreePage,
		// Token: 0x04000BD6 RID: 3030
		TechTreeNodePopup,
		// Token: 0x04000BD7 RID: 3031
		HelpScreen
	}
}
