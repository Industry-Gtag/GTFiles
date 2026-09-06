using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005D3 RID: 1491
public class GameModePages : BasePageHandler
{
	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x06002580 RID: 9600 RVA: 0x000C8015 File Offset: 0x000C6215
	protected override int pageSize
	{
		get
		{
			return this.buttons.Length;
		}
	}

	// Token: 0x170003F8 RID: 1016
	// (get) Token: 0x06002581 RID: 9601 RVA: 0x000C801F File Offset: 0x000C621F
	protected override int entriesCount
	{
		get
		{
			return GameMode.gameModeNames.Count;
		}
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x000C802C File Offset: 0x000C622C
	private void Awake()
	{
		GameModePages.gameModeSelectorInstances.Add(this);
		this.buttons = base.GetComponentsInChildren<GameModeSelectButton>();
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].buttonIndex = i;
			this.buttons[i].selector = this;
		}
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x000C807F File Offset: 0x000C627F
	protected override void Start()
	{
		base.Start();
		base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		this.initialized = true;
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x000C8099 File Offset: 0x000C6299
	private void OnEnable()
	{
		if (this.initialized)
		{
			base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x000C80AE File Offset: 0x000C62AE
	private void OnDestroy()
	{
		GameModePages.gameModeSelectorInstances.Remove(this);
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x000C80BC File Offset: 0x000C62BC
	protected override void ShowPage(int selectedPage, int startIndex, int endIndex)
	{
		GameModePages.textBuilder.Clear();
		for (int i = startIndex; i < endIndex; i++)
		{
			GameModePages.textBuilder.AppendLine(GameMode.gameModeNames[i]);
		}
		this.gameModeText.text = GameModePages.textBuilder.ToString();
		if (base.selectedIndex >= startIndex && base.selectedIndex <= endIndex)
		{
			this.UpdateAllButtons(this.currentButtonIndex);
		}
		else
		{
			this.UpdateAllButtons(-1);
		}
		int num = ((selectedPage == base.pages - 1 && base.maxEntires > endIndex) ? (base.maxEntires - endIndex) : 0);
		this.EnableEntryButtons(num);
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x000C8159 File Offset: 0x000C6359
	protected override void PageEntrySelected(int pageEntry, int selectionIndex)
	{
		if (selectionIndex >= this.entriesCount)
		{
			return;
		}
		GameModePages.sharedSelectedIndex = selectionIndex;
		this.UpdateAllButtons(pageEntry);
		this.currentButtonIndex = pageEntry;
		GorillaComputer.instance.OnModeSelectButtonPress(GameMode.gameModeNames[selectionIndex], false);
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x000C8194 File Offset: 0x000C6394
	private void UpdateAllButtons(int onButton)
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			if (i == onButton)
			{
				this.buttons[onButton].isOn = true;
				this.buttons[onButton].UpdateColor();
			}
			else if (this.buttons[i].isOn)
			{
				this.buttons[i].isOn = false;
				this.buttons[i].UpdateColor();
			}
		}
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x000C8200 File Offset: 0x000C6400
	private void EnableEntryButtons(int buttonsMissing)
	{
		int num = this.buttons.Length - buttonsMissing;
		int i;
		for (i = 0; i < num; i++)
		{
			this.buttons[i].gameObject.SetActive(true);
		}
		while (i < this.buttons.Length)
		{
			this.buttons[i].gameObject.SetActive(false);
			i++;
		}
	}

	// Token: 0x0600258A RID: 9610 RVA: 0x000C825C File Offset: 0x000C645C
	public static void SetSelectedGameModeShared(string gameMode)
	{
		GameModePages.sharedSelectedIndex = GameMode.gameModeNames.IndexOf(gameMode);
		if (GameModePages.sharedSelectedIndex < 0)
		{
			return;
		}
		for (int i = 0; i < GameModePages.gameModeSelectorInstances.Count; i++)
		{
			GameModePages.gameModeSelectorInstances[i].SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x040030F1 RID: 12529
	private int currentButtonIndex;

	// Token: 0x040030F2 RID: 12530
	[SerializeField]
	private Text gameModeText;

	// Token: 0x040030F3 RID: 12531
	[SerializeField]
	private GameModeSelectButton[] buttons;

	// Token: 0x040030F4 RID: 12532
	private bool initialized;

	// Token: 0x040030F5 RID: 12533
	private static int sharedSelectedIndex = 0;

	// Token: 0x040030F6 RID: 12534
	private static StringBuilder textBuilder = new StringBuilder(50);

	// Token: 0x040030F7 RID: 12535
	[OnEnterPlay_Clear]
	private static List<GameModePages> gameModeSelectorInstances = new List<GameModePages>(7);
}
