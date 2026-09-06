using System;
using System.Collections.Generic;
using System.IO;
using GorillaTag;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x02000140 RID: 320
public class SIGadgetDispenser : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060007EF RID: 2031 RVA: 0x0002B5CE File Offset: 0x000297CE
	internal bool isTryOn
	{
		get
		{
			return this.m_isTryOn;
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060007F0 RID: 2032 RVA: 0x0002B5D6 File Offset: 0x000297D6
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060007F1 RID: 2033 RVA: 0x0002B5DE File Offset: 0x000297DE
	public SIPlayer ActivePlayer
	{
		get
		{
			return this.parentTerminal.activePlayer;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060007F2 RID: 2034 RVA: 0x0002B5EB File Offset: 0x000297EB
	public string ActivePlayerName
	{
		get
		{
			return this.ActivePlayer.gamePlayer.rig.Creator.SanitizedNickName;
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060007F3 RID: 2035 RVA: 0x0002B607 File Offset: 0x00029807
	public bool IsAuthority
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060007F4 RID: 2036 RVA: 0x0002B623 File Offset: 0x00029823
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x060007F5 RID: 2037 RVA: 0x0002B635 File Offset: 0x00029835
	public GameEntityManager GameEntityManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x060007F6 RID: 2038 RVA: 0x0002B64C File Offset: 0x0002984C
	public SITechTreeNode CurrentNode
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO.GetTreeNode(this.parentTerminal.ActivePage, this._currentNode);
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x060007F7 RID: 2039 RVA: 0x0002B674 File Offset: 0x00029874
	public SITechTreePage CurrentPage
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO.GetTreePage((SITechTreePageId)this.parentTerminal.ActivePage);
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x060007F8 RID: 2040 RVA: 0x0002B696 File Offset: 0x00029896
	public SITechTreeSO TechTreeSO
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO;
		}
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002B6A8 File Offset: 0x000298A8
	protected void OnEnable()
	{
		if (this.m_isTryOn)
		{
			SIGadgetDispenser.g_tryOnOptions = this.m_tryOnOptions;
		}
		this._RefreshButtonsUsableState();
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0002B6C4 File Offset: 0x000298C4
	private void _RefreshButtonsUsableState()
	{
		foreach (SIGadgetListEntry sigadgetListEntry in this.gadgetPages)
		{
			SITechTreePageId id = (SITechTreePageId)sigadgetListEntry.Id;
			SITechTreePage sitechTreePage;
			if (this.TechTreeSO.TryGetTreePage(id, out sitechTreePage))
			{
				sigadgetListEntry.ButtonContainer.SetUsable(sitechTreePage.IsAllowed);
			}
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002B738 File Offset: 0x00029938
	private void SetNonPopupButtonsEnabled(bool enable)
	{
		foreach (Collider collider in this._nonPopupButtonColliders)
		{
			collider.enabled = enable;
		}
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0002B78C File Offset: 0x0002998C
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
		this.screenData = new Dictionary<SIGadgetDispenser.GadgetDispenserTerminalState, GameObject>();
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, this.waitingForScanScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetType, this.gadgetTypeScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList, this.gadgetListScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation, this.gadgetInformationScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed, this.gadgetDispensedScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen, this.gadgetsHelpScreen);
		this.parentTerminal.superInfection.techTreeSO.EnsureInitialized();
		int num = 0;
		int count = this.parentTerminal.superInfection.techTreeSO.TreePages.Count;
		for (int i = 0; i < count; i++)
		{
			SITechTreePage sitechTreePage = this.parentTerminal.superInfection.techTreeSO.TreePages[i];
			SIGadgetListEntry sigadgetListEntry = Object.Instantiate<SIGadgetListEntry>(this.pageListEntryPrefab, this.pageListParent);
			StaticLodManager.TryAddLateInstantiatedMembers(sigadgetListEntry.gameObject);
			sigadgetListEntry.Configure(this, sitechTreePage, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText, SITouchscreenButton.SITouchscreenButtonType.Select, i, -0.07f, count);
			this.gadgetPages.Add(sigadgetListEntry);
			num = Math.Max(num, sitechTreePage.DispensableGadgets.Count);
		}
		this.gadgetEntries = new List<SIDispenserGadgetListEntry>();
		for (int j = 0; j < num; j++)
		{
			SIDispenserGadgetListEntry sidispenserGadgetListEntry = Object.Instantiate<SIDispenserGadgetListEntry>(this.gadgetListEntryPrefab, this.gadgetListParent);
			sidispenserGadgetListEntry.transform.localPosition += new Vector3(0f, (float)j * -0.07f, 0f);
			sidispenserGadgetListEntry.SetStation(this, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText);
			this.gadgetEntries.Add(sidispenserGadgetListEntry);
		}
		if (this.m_isTryOn && base.isActiveAndEnabled)
		{
			SIGadgetDispenser.g_tryOnOptions = this.m_tryOnOptions;
		}
		this._RefreshButtonsUsableState();
		this.Reset();
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0002B9AF File Offset: 0x00029BAF
	public void Reset()
	{
		this.currentState = SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan;
		this.SetScreenVisibility(this.currentState, this.currentState);
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0002B9CC File Offset: 0x00029BCC
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy)
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
		}
		stream.SendNext(this.helpScreenIndex);
		stream.SendNext(this._currentNode);
		stream.SendNext((int)this.currentState);
		stream.SendNext((int)this.lastState);
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0002BA48 File Offset: 0x00029C48
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.helpScreenIndex = Mathf.Clamp((int)stream.ReceiveNext(), 0, this.helpPopupScreens.Length - 1);
		this._currentNode = (int)stream.ReceiveNext();
		if (this.CurrentNode == null && this.CurrentPage != null && this.CurrentPage.AllNodes.Count > 0 && this.CurrentPage.AllNodes[0].Value != null)
		{
			this._currentNode = (int)this.CurrentPage.AllNodes[0].Value.upgradeType;
		}
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState = (SIGadgetDispenser.GadgetDispenserTerminalState)stream.ReceiveNext();
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState2 = (SIGadgetDispenser.GadgetDispenserTerminalState)stream.ReceiveNext();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState) || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState2))
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(gadgetDispenserTerminalState, gadgetDispenserTerminalState2);
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0002BB5A File Offset: 0x00029D5A
	public void ZoneDataSerializeWrite(BinaryWriter writer)
	{
		writer.Write(this.helpScreenIndex);
		writer.Write(this._currentNode);
		writer.Write((int)this.currentState);
		writer.Write((int)this.lastState);
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0002BB8C File Offset: 0x00029D8C
	public void ZoneDataSerializeRead(BinaryReader reader)
	{
		this.helpScreenIndex = Mathf.Clamp(reader.ReadInt32(), 0, this.helpPopupScreens.Length - 1);
		int num = reader.ReadInt32();
		if (this.CurrentPage != null && this.CurrentPage.AllNodes != null)
		{
			this._currentNode = Mathf.Clamp(num, 0, this.CurrentPage.AllNodes.Count - 1);
		}
		else
		{
			this._currentNode = 0;
		}
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState = (SIGadgetDispenser.GadgetDispenserTerminalState)reader.ReadInt32();
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState2 = (SIGadgetDispenser.GadgetDispenserTerminalState)reader.ReadInt32();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState) || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState2))
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(gadgetDispenserTerminalState, gadgetDispenserTerminalState2);
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0002BC66 File Offset: 0x00029E66
	public void UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState newState, SIGadgetDispenser.GadgetDispenserTerminalState newLastState)
	{
		if (!this.IsPopupState(newLastState))
		{
			this.currentState = newLastState;
		}
		this.UpdateState(newState);
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0002BC80 File Offset: 0x00029E80
	public void UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState newState)
	{
		if (!this.IsPopupState(this.currentState))
		{
			this.lastState = this.currentState;
		}
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, this.lastState);
		switch (this.currentState)
		{
		case SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan:
			break;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetType:
			this.screenDescription.text = "GADGET TYPES";
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList:
			this.screenDescription.text = "UNLOCKED " + this.CurrentPage.nickName + " GADGETS";
			this.UpdateGadgetListVisibility();
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation:
			this.screenDescription.text = this.CurrentNode.nickName;
			this.gadgetDescriptionText.text = this.CurrentNode.description;
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed:
			this.gadgetDispensedText.text = this.ActivePlayerName + " HAS DISPENSED A " + this.CurrentNode.nickName + "!";
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen:
			this.UpdateHelpButtonPage(this.helpScreenIndex);
			break;
		default:
			return;
		}
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0002BD88 File Offset: 0x00029F88
	public void SetScreenVisibility(SIGadgetDispenser.GadgetDispenserTerminalState currentState, SIGadgetDispenser.GadgetDispenserTerminalState lastState)
	{
		bool flag = this.IsPopupState(currentState);
		this.background.color = ((currentState == SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan) ? Color.white : ((this.ActivePlayer != null && this.ActivePlayer.gamePlayer.IsLocal()) ? this.active : this.notActive));
		foreach (SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState in this.screenData.Keys)
		{
			bool flag2 = gadgetDispenserTerminalState == currentState || (flag && gadgetDispenserTerminalState == lastState);
			if (this.screenData[gadgetDispenserTerminalState].activeSelf != flag2)
			{
				this.screenData[gadgetDispenserTerminalState].SetActive(flag2);
			}
		}
		if (this.popupScreen.activeSelf != flag)
		{
			this.popupScreen.SetActive(flag);
		}
		this.screenDescription.gameObject.SetActive(currentState > SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
		this.SetNonPopupButtonsEnabled(!flag);
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0002BE94 File Offset: 0x0002A094
	public void UpdateGadgetListVisibility()
	{
		foreach (SIDispenserGadgetListEntry sidispenserGadgetListEntry in this.gadgetEntries)
		{
			sidispenserGadgetListEntry.gameObject.SetActive(false);
		}
		int num = 0;
		foreach (SITechTreeNode sitechTreeNode in this.CurrentPage.DispensableGadgets)
		{
			if (this.m_isTryOn || this.ActivePlayer.CurrentProgression.IsUnlocked(sitechTreeNode.upgradeType))
			{
				SIDispenserGadgetListEntry sidispenserGadgetListEntry2 = this.gadgetEntries[num++];
				sidispenserGadgetListEntry2.SetTechTreeNode(sitechTreeNode);
				sidispenserGadgetListEntry2.gameObject.SetActive(true);
				sidispenserGadgetListEntry2.DispenseButton.SetUsable(this.m_isTryOn || sitechTreeNode.IsAllowed);
			}
		}
		this.noDispensableGadgetsMessage.SetActive(num == 0);
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0002BFA0 File Offset: 0x0002A1A0
	public bool IsPopupState(SIGadgetDispenser.GadgetDispenserTerminalState state)
	{
		return state == SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed || state == SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x0002BFAC File Offset: 0x0002A1AC
	public void PlayerHandScanned(int actorNr)
	{
		this.UpdateState(this.handScannedState);
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0002BFBA File Offset: 0x0002A1BA
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
		if (!isPopupButton)
		{
			this._nonPopupButtonColliders.Add(button.GetComponent<Collider>());
		}
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0002BFD0 File Offset: 0x0002A1D0
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && (this.ActivePlayer == null || this.ActivePlayer != SIPlayer.LocalPlayer))
		{
			this.parentTerminal.PlayWrongPlayerBuzz(this.uiCenter);
		}
		else
		{
			this.touchSoundBankPlayer.Play();
		}
		if (!this.IsAuthority)
		{
			this.parentTerminal.TouchscreenButtonPressed(buttonType, data, actorNr, SICombinedTerminal.TerminalSubFunction.GadgetDispenser);
			return;
		}
		if (actorNr != this.ActivePlayer.ActorNr)
		{
			return;
		}
		this.touchSoundBankPlayer.Play();
		switch (this.currentState)
		{
		case SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetType:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				this.parentTerminal.SetActivePage(data);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetType);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				SITechTreeNode treeNode = this.TechTreeSO.GetTreeNode((int)this.CurrentPage.pageId, data);
				if (treeNode != null && treeNode.IsDispensableGadget)
				{
					this._currentNode = data;
					this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation);
				}
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Dispense)
			{
				SITechTreeNode treeNode2 = this.TechTreeSO.GetTreeNode((int)this.CurrentPage.pageId, data);
				if (treeNode2 != null && treeNode2.IsDispensableGadget)
				{
					this._currentNode = data;
					this.AuthorityDispenseGadgetForPlayer(this.ActivePlayer);
					this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed);
				}
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Dispense)
			{
				this.AuthorityDispenseGadgetForPlayer(this.ActivePlayer);
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.UpdateState(this.lastState);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen:
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

	// Token: 0x0600080A RID: 2058 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
	{
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0002C1E0 File Offset: 0x0002A3E0
	public void UpdateHelpButtonPage(int helpButtonPageIndex)
	{
		for (int i = 0; i < this.helpPopupScreens.Length; i++)
		{
			this.helpPopupScreens[i].SetActive(i == helpButtonPageIndex);
		}
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0002C214 File Offset: 0x0002A414
	public void AuthorityDispenseGadgetForPlayer(SIPlayer player)
	{
		if (!this.IsAuthority)
		{
			return;
		}
		int num = 0;
		int staticHash = this.CurrentNode.unlockedGadgetPrefab.name.GetStaticHash();
		for (int i = player.activePlayerGadgets.Count - 1; i >= 0; i--)
		{
			GameEntity gameEntityFromNetId = this.GameEntityManager.GetGameEntityFromNetId(player.activePlayerGadgets[i]);
			if (gameEntityFromNetId == null)
			{
				player.activePlayerGadgets.RemoveAt(i);
			}
			else
			{
				num++;
				if (num >= player.TotalGadgetLimit)
				{
					this.GameEntityManager.RequestDestroyItem(gameEntityFromNetId.id);
					break;
				}
			}
		}
		SIUpgradeSet upgrades = player.GetUpgrades(this.CurrentPage.pageId);
		int num2 = 0;
		foreach (GraphNode<SITechTreeNode> graphNode in this.CurrentPage.AllNodes)
		{
			num2 |= 1 << graphNode.Value.upgradeType.GetNodeId();
		}
		upgrades.SetBits(this.m_isTryOn ? num2 : (upgrades.GetBits() & num2));
		foreach (SITechTreeNode sitechTreeNode in this.CurrentPage.DispensableGadgets)
		{
			if (sitechTreeNode != this.CurrentNode)
			{
				upgrades.Remove(sitechTreeNode.upgradeType);
			}
		}
		long num3 = upgrades.GetCreateData(player);
		if (this.m_isTryOn)
		{
			num3 |= long.MinValue;
		}
		this.GameEntityManager.RequestCreateItem(staticHash, this.gadgetDispensePosition.position, this.gadgetDispensePosition.rotation, num3);
		this.dispenseSoundBankPlayer.Play();
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0002C3EC File Offset: 0x0002A5EC
	public void SetActivePage()
	{
		if (this.CurrentNode == null)
		{
			this._currentNode = this.CurrentPage.AllNodes[0].Value.upgradeType.GetNodeId();
		}
		if (this.ActivePlayer != null)
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList);
			return;
		}
		this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x0002C444 File Offset: 0x0002A644
	public bool IsValidPage(int pageId)
	{
		if (pageId < 0)
		{
			return false;
		}
		using (List<SIGadgetListEntry>.Enumerator enumerator = this.gadgetPages.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Id == pageId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x000066D3 File Offset: 0x000048D3
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04000A07 RID: 2567
	public SIGadgetDispenser.GadgetDispenserTerminalState handScannedState = SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList;

	// Token: 0x04000A08 RID: 2568
	public SIGadgetDispenser.GadgetDispenserTerminalState currentState;

	// Token: 0x04000A09 RID: 2569
	public SIGadgetDispenser.GadgetDispenserTerminalState lastState;

	// Token: 0x04000A0A RID: 2570
	public Transform gadgetDispensePosition;

	// Token: 0x04000A0B RID: 2571
	public int _currentNode;

	// Token: 0x04000A0C RID: 2572
	public SICombinedTerminal parentTerminal;

	// Token: 0x04000A0D RID: 2573
	[Header("TryOn")]
	[SerializeField]
	private bool m_isTryOn;

	// Token: 0x04000A0E RID: 2574
	[SerializeField]
	private GameEntityDelayedDestroy.Options m_tryOnOptions = new GameEntityDelayedDestroy.Options
	{
		delay = 30f,
		explosionVolume = 1f,
		beepVolume = 1f,
		beepPhases = new GameEntityDelayedDestroy.BeepPhase[]
		{
			new GameEntityDelayedDestroy.BeepPhase
			{
				timeRemaining = 10f,
				interval = 1f
			},
			new GameEntityDelayedDestroy.BeepPhase
			{
				timeRemaining = 5f,
				interval = 0.5f
			},
			new GameEntityDelayedDestroy.BeepPhase
			{
				timeRemaining = 2f,
				interval = 0.25f
			}
		}
	};

	// Token: 0x04000A0F RID: 2575
	internal static GameEntityDelayedDestroy.Options g_tryOnOptions = new GameEntityDelayedDestroy.Options
	{
		delay = 1f
	};

	// Token: 0x04000A10 RID: 2576
	public GameObject waitingForScanScreen;

	// Token: 0x04000A11 RID: 2577
	public GameObject gadgetTypeScreen;

	// Token: 0x04000A12 RID: 2578
	public GameObject gadgetListScreen;

	// Token: 0x04000A13 RID: 2579
	public GameObject gadgetInformationScreen;

	// Token: 0x04000A14 RID: 2580
	public GameObject gadgetDispensedScreen;

	// Token: 0x04000A15 RID: 2581
	public GameObject gadgetsHelpScreen;

	// Token: 0x04000A16 RID: 2582
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x04000A17 RID: 2583
	[Header("Main Screen Shared")]
	public TextMeshProUGUI screenDescription;

	// Token: 0x04000A18 RID: 2584
	public Image background;

	// Token: 0x04000A19 RID: 2585
	public Color active;

	// Token: 0x04000A1A RID: 2586
	public Color notActive;

	// Token: 0x04000A1B RID: 2587
	public Transform uiCenter;

	// Token: 0x04000A1C RID: 2588
	[Header("Popup Shared")]
	public GameObject popupScreen;

	// Token: 0x04000A1D RID: 2589
	[Header("Gadgets Type")]
	[SerializeField]
	private RectTransform pageListParent;

	// Token: 0x04000A1E RID: 2590
	[SerializeField]
	private SIGadgetListEntry pageListEntryPrefab;

	// Token: 0x04000A1F RID: 2591
	private List<SIGadgetListEntry> gadgetPages = new List<SIGadgetListEntry>();

	// Token: 0x04000A20 RID: 2592
	[FormerlySerializedAs("noDispensableGadgetsNotif")]
	[Header("Gadgets List")]
	[SerializeField]
	private GameObject noDispensableGadgetsMessage;

	// Token: 0x04000A21 RID: 2593
	[SerializeField]
	private RectTransform gadgetListParent;

	// Token: 0x04000A22 RID: 2594
	[SerializeField]
	private SIDispenserGadgetListEntry gadgetListEntryPrefab;

	// Token: 0x04000A23 RID: 2595
	private List<SIDispenserGadgetListEntry> gadgetEntries;

	// Token: 0x04000A24 RID: 2596
	[Header("Gadgets Description")]
	public TextMeshProUGUI gadgetDescriptionText;

	// Token: 0x04000A25 RID: 2597
	[Header("Gadget Dispensed")]
	public TextMeshProUGUI gadgetDispensedText;

	// Token: 0x04000A26 RID: 2598
	[Header("Help")]
	public int helpScreenIndex;

	// Token: 0x04000A27 RID: 2599
	public GameObject[] helpPopupScreens;

	// Token: 0x04000A28 RID: 2600
	[Header("Audio")]
	[SerializeField]
	private SoundBankPlayer touchSoundBankPlayer;

	// Token: 0x04000A29 RID: 2601
	[SerializeField]
	private SoundBankPlayer dispenseSoundBankPlayer;

	// Token: 0x04000A2A RID: 2602
	[Header("Main Screen Colliders")]
	[Tooltip("Button colliders to disable while popup screen is shown.  Gets updated live to include page and gadget buttons.")]
	[SerializeField]
	private List<Collider> _nonPopupButtonColliders;

	// Token: 0x04000A2B RID: 2603
	private Dictionary<SIGadgetDispenser.GadgetDispenserTerminalState, GameObject> screenData;

	// Token: 0x04000A2C RID: 2604
	private bool initialized;

	// Token: 0x02000141 RID: 321
	public enum GadgetDispenserTerminalState
	{
		// Token: 0x04000A2E RID: 2606
		WaitingForScan,
		// Token: 0x04000A2F RID: 2607
		GadgetType,
		// Token: 0x04000A30 RID: 2608
		GadgetList,
		// Token: 0x04000A31 RID: 2609
		GadgetInformation,
		// Token: 0x04000A32 RID: 2610
		GadgetDispensed,
		// Token: 0x04000A33 RID: 2611
		HelpScreen
	}
}
