using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200065E RID: 1630
public class BuilderScanKiosk : MonoBehaviourTick
{
	// Token: 0x0600289F RID: 10399 RVA: 0x000DBB46 File Offset: 0x000D9D46
	public static bool IsSaveSlotValid(int slot)
	{
		return slot >= 0 && slot < BuilderScanKiosk.NUM_SAVE_SLOTS;
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x000DBB58 File Offset: 0x000D9D58
	private void Start()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.AddListener(new UnityAction(this.OnSavePressed));
		}
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.AddListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveSuccess.AddListener(new UnityAction(this.OnSaveSuccess));
			this.targetTable.OnSaveFailure.AddListener(new UnityAction<string>(this.OnSaveFail));
			SharedBlocksManager.OnSaveTimeUpdated += this.OnSaveTimeUpdated;
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.AddListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			gorillaPressableButton.onPressed += this.OnScanButtonPressed;
		}
		this.scanTriangle = this.scanAnimation.GetComponent<MeshRenderer>();
		this.scanTriangle.enabled = false;
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.LoadPlayerPrefs();
		this.UpdateUI();
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000DBCAC File Offset: 0x000D9EAC
	private new void OnEnable()
	{
		base.OnEnable();
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.UpdateUI));
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000DBCC5 File Offset: 0x000D9EC5
	private new void OnDisable()
	{
		base.OnDisable();
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.UpdateUI));
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000DBCE0 File Offset: 0x000D9EE0
	private void OnDestroy()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.RemoveListener(new UnityAction(this.OnSavePressed));
		}
		SharedBlocksManager.OnSaveTimeUpdated -= this.OnSaveTimeUpdated;
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.RemoveListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveFailure.RemoveListener(new UnityAction<string>(this.OnSaveFail));
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.RemoveListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= this.OnScanButtonPressed;
			}
		}
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000DBDF4 File Offset: 0x000D9FF4
	private void OnNoneButtonPressed()
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		if (this.targetTable.CurrentSaveSlot != -1)
		{
			this.targetTable.CurrentSaveSlot = -1;
			this.SavePlayerPrefs();
			this.UpdateUI();
		}
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000DBE48 File Offset: 0x000DA048
	private void OnScanButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		int i = 0;
		while (i < this.scanButtons.Count)
		{
			if (button.Equals(this.scanButtons[i]))
			{
				if (i != this.targetTable.CurrentSaveSlot)
				{
					this.targetTable.CurrentSaveSlot = i;
					this.SavePlayerPrefs();
					this.UpdateUI();
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnDevScanPressed()
	{
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000DBEC8 File Offset: 0x000DA0C8
	private void LoadPlayerPrefs()
	{
		int @int = PlayerPrefs.GetInt(BuilderScanKiosk.playerPrefKey, -1);
		this.targetTable.CurrentSaveSlot = @int;
		this.UpdateUI();
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x000DBEF3 File Offset: 0x000DA0F3
	private void SavePlayerPrefs()
	{
		PlayerPrefs.SetInt(BuilderScanKiosk.playerPrefKey, this.targetTable.CurrentSaveSlot);
		PlayerPrefs.Save();
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x000DBF10 File Offset: 0x000DA110
	private void ToggleSaveButton(bool enabled)
	{
		if (enabled)
		{
			this.saveButton.enabled = true;
			this.saveButton.buttonRenderer.material = this.saveButton.unpressedMaterial;
			return;
		}
		this.saveButton.enabled = false;
		this.saveButton.buttonRenderer.material = this.saveButton.pressedMaterial;
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000DBF70 File Offset: 0x000DA170
	public override void Tick()
	{
		if (this.isAnimating)
		{
			if (this.scanAnimation == null)
			{
				this.isAnimating = false;
			}
			else if ((double)Time.time > this.scanCompleteTime)
			{
				this.scanTriangle.enabled = false;
				this.isAnimating = false;
			}
		}
		if (this.coolingDown && (double)Time.time > this.coolDownCompleteTime)
		{
			this.coolingDown = false;
			this.UpdateUI();
		}
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x000DBFE0 File Offset: 0x000DA1E0
	private void OnSavePressed()
	{
		if (this.targetTable == null || !this.isDirty || this.coolingDown)
		{
			return;
		}
		BuilderScanKiosk.ScannerState scannerState = this.scannerState;
		if (scannerState == BuilderScanKiosk.ScannerState.IDLE)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.CONFIRMATION;
			this.UpdateUI();
			return;
		}
		if (scannerState != BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			return;
		}
		this.scannerState = BuilderScanKiosk.ScannerState.SAVING;
		if (this.scanAnimation != null)
		{
			this.scanCompleteTime = (double)(Time.time + this.scanAnimation.clip.length);
			this.scanTriangle.enabled = true;
			this.scanAnimation.Rewind();
			this.scanAnimation.Play();
		}
		if (this.soundBank != null)
		{
			this.soundBank.Play();
		}
		this.isAnimating = true;
		this.saveError = false;
		this.errorMsg = string.Empty;
		this.coolDownCompleteTime = (double)(Time.time + this.saveCooldownSeconds);
		this.coolingDown = true;
		this.UpdateUI();
		string text;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY", out text, "BUSY");
		string text2;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS", out text2, "PLEASE REMOVE BLOCKS CONNECTED OUTSIDE OF TABLE PLATFORM");
		this.targetTable.SaveTableForPlayer(text, text2);
	}

	// Token: 0x060028AC RID: 10412 RVA: 0x000DC104 File Offset: 0x000DA304
	private string GetSavePath()
	{
		return string.Concat(new string[]
		{
			this.GetSaveFolder(),
			Path.DirectorySeparatorChar.ToString(),
			BuilderScanKiosk.SAVE_FILE,
			"_",
			this.targetTable.CurrentSaveSlot.ToString(),
			".png"
		});
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x000DC160 File Offset: 0x000DA360
	private string GetSaveFolder()
	{
		return Application.persistentDataPath + Path.DirectorySeparatorChar.ToString() + BuilderScanKiosk.SAVE_FOLDER;
	}

	// Token: 0x060028AE RID: 10414 RVA: 0x000DC17B File Offset: 0x000DA37B
	private void OnSaveDirtyChanged(bool dirty)
	{
		this.isDirty = dirty;
		this.UpdateUI();
	}

	// Token: 0x060028AF RID: 10415 RVA: 0x000DC18A File Offset: 0x000DA38A
	private void OnSaveTimeUpdated()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x000DC18A File Offset: 0x000DA38A
	private void OnSaveSuccess()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x000DC1A0 File Offset: 0x000DA3A0
	private void OnSaveFail(string errorMsg)
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = true;
		this.errorMsg = errorMsg;
		this.UpdateUI();
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x000DC1C0 File Offset: 0x000DA3C0
	private void UpdateUI()
	{
		this.screenText.text = this.GetTextForScreen();
		this.ToggleSaveButton(BuilderScanKiosk.IsSaveSlotValid(this.targetTable.CurrentSaveSlot) && !this.coolingDown);
		this.noneButton.buttonRenderer.material = ((!BuilderScanKiosk.IsSaveSlotValid(this.targetTable.CurrentSaveSlot)) ? this.noneButton.pressedMaterial : this.noneButton.unpressedMaterial);
		bool flag = SubscriptionManager.IsLocalSubscribed();
		for (int i = 0; i < this.scanButtons.Count; i++)
		{
			GorillaPressableButton gorillaPressableButton = this.scanButtons[i];
			if (gorillaPressableButton.isSubscriberOnlyButton && !flag)
			{
				gorillaPressableButton.buttonRenderer.material = ((gorillaPressableButton.nonSubscriberMaterial != null) ? gorillaPressableButton.nonSubscriberMaterial : gorillaPressableButton.unpressedMaterial);
			}
			else
			{
				gorillaPressableButton.buttonRenderer.material = ((this.targetTable.CurrentSaveSlot == i) ? gorillaPressableButton.pressedMaterial : gorillaPressableButton.unpressedMaterial);
			}
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON", out text, "YES UPDATE SCAN");
			this.saveButton.myTmpText.text = text;
			return;
		}
		string text2;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON", out text2, "UPDATE SCAN");
		this.saveButton.myTmpText.text = text2;
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000DC314 File Offset: 0x000DA514
	private string GetTextForScreen()
	{
		if (this.targetTable == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		string text = "";
		int currentSaveSlot = this.targetTable.CurrentSaveSlot;
		if (!BuilderScanKiosk.IsSaveSlotValid(currentSaveSlot))
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT", out text, "<b><color=red>NONE</color></b>");
			stringBuilder.Append(text);
		}
		else if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
		{
			stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
		}
		else
		{
			stringBuilder.Append("<b><color=red>");
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out text, "SCAN ");
			stringBuilder.Append(text);
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
			SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(currentSaveSlot);
			DateTime dateTime = DateTime.FromBinary(publishInfoForSlot.publishTime);
			if (dateTime > DateTime.MinValue)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL", out text, "UPDATED ");
				stringBuilder.Append(": ");
				stringBuilder.Append(text);
				stringBuilder.Append(dateTime.ToString());
				stringBuilder.Append("\n");
			}
			if (SharedBlocksManager.IsMapIDValid(publishInfoForSlot.mapID))
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL", out text, "MAP ID: ");
				stringBuilder.Append(text);
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(0, 4));
				stringBuilder.Append("-");
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(4));
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS", out text, "\nUSE THIS CODE IN THE SHARE MY BLOCKS ROOM");
				stringBuilder.Append(text);
			}
		}
		stringBuilder.Append("\n");
		switch (this.scannerState)
		{
		case BuilderScanKiosk.ScannerState.IDLE:
			if (this.saveError)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR", out text, "ERROR WHILE SCANNING: ");
				stringBuilder.Append(text);
				stringBuilder.Append(this.errorMsg);
			}
			else if (this.coolingDown)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN", out text, "COOLING DOWN...");
				stringBuilder.Append(text);
			}
			else if (!this.isDirty)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES", out text, "NO UNSAVED CHANGES");
				stringBuilder.Append(text);
			}
			break;
		case BuilderScanKiosk.ScannerState.CONFIRMATION:
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE", out text, "YOU ARE ABOUT TO REPLACE ");
			if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
			{
				stringBuilder.Append(text);
				stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
			}
			else
			{
				stringBuilder.Append(text);
				stringBuilder.Append("<b><color=red>");
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out text, "SCAN ");
				stringBuilder.Append(text);
				stringBuilder.Append(currentSaveSlot + 1);
				stringBuilder.Append("</color></b>");
			}
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION", out text, " ARE YOU SURE YOU WANT TO SCAN?");
			stringBuilder.Append(text);
			break;
		case BuilderScanKiosk.ScannerState.SAVING:
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING", out text, "SCANNING BUILD...");
			stringBuilder.Append(text);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		stringBuilder.Append("\n\n\n");
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS", out text, "CREATE A <b><color=red>NEW</color></b> PRIVATE ROOM TO LOAD ");
		stringBuilder.Append(text);
		if (!BuilderScanKiosk.IsSaveSlotValid(currentSaveSlot))
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE", out text, "<b><color=red>AN EMPTY TABLE</color></b>");
			stringBuilder.Append(text);
		}
		else if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
		{
			stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
		}
		else
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out text, "SCAN ");
			stringBuilder.Append("<b><color=red>");
			stringBuilder.Append(text);
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x040034EA RID: 13546
	private const string MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT_KEY = "MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT";

	// Token: 0x040034EB RID: 13547
	private const string MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL";

	// Token: 0x040034EC RID: 13548
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL";

	// Token: 0x040034ED RID: 13549
	private const string MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL";

	// Token: 0x040034EE RID: 13550
	private const string MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS";

	// Token: 0x040034EF RID: 13551
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR";

	// Token: 0x040034F0 RID: 13552
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY";

	// Token: 0x040034F1 RID: 13553
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS";

	// Token: 0x040034F2 RID: 13554
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN";

	// Token: 0x040034F3 RID: 13555
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES";

	// Token: 0x040034F4 RID: 13556
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE";

	// Token: 0x040034F5 RID: 13557
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION";

	// Token: 0x040034F6 RID: 13558
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING";

	// Token: 0x040034F7 RID: 13559
	private const string MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS";

	// Token: 0x040034F8 RID: 13560
	private const string MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE";

	// Token: 0x040034F9 RID: 13561
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_NONE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_NONE";

	// Token: 0x040034FA RID: 13562
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_ONE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_ONE";

	// Token: 0x040034FB RID: 13563
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_TWO_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_TWO";

	// Token: 0x040034FC RID: 13564
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_THREE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_THREE";

	// Token: 0x040034FD RID: 13565
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON";

	// Token: 0x040034FE RID: 13566
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON";

	// Token: 0x040034FF RID: 13567
	[SerializeField]
	private GorillaPressableButton saveButton;

	// Token: 0x04003500 RID: 13568
	[SerializeField]
	private GorillaPressableButton noneButton;

	// Token: 0x04003501 RID: 13569
	[SerializeField]
	private List<GorillaPressableButton> scanButtons;

	// Token: 0x04003502 RID: 13570
	[SerializeField]
	private BuilderTable targetTable;

	// Token: 0x04003503 RID: 13571
	[SerializeField]
	private float saveCooldownSeconds = 5f;

	// Token: 0x04003504 RID: 13572
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x04003505 RID: 13573
	[SerializeField]
	private SoundBankPlayer soundBank;

	// Token: 0x04003506 RID: 13574
	[SerializeField]
	private Animation scanAnimation;

	// Token: 0x04003507 RID: 13575
	private MeshRenderer scanTriangle;

	// Token: 0x04003508 RID: 13576
	private bool isAnimating;

	// Token: 0x04003509 RID: 13577
	private static string playerPrefKey = "BuilderSaveSlot";

	// Token: 0x0400350A RID: 13578
	private static string SAVE_FOLDER = "MonkeBlocks";

	// Token: 0x0400350B RID: 13579
	private static string SAVE_FILE = "MyBuild";

	// Token: 0x0400350C RID: 13580
	public static int NUM_SAVE_SLOTS = 5;

	// Token: 0x0400350D RID: 13581
	public static int DEV_SAVE_SLOT = -2;

	// Token: 0x0400350E RID: 13582
	private Texture2D buildCaptureTexture;

	// Token: 0x0400350F RID: 13583
	private bool isDirty;

	// Token: 0x04003510 RID: 13584
	private bool saveError;

	// Token: 0x04003511 RID: 13585
	private string errorMsg = string.Empty;

	// Token: 0x04003512 RID: 13586
	private bool coolingDown;

	// Token: 0x04003513 RID: 13587
	private double coolDownCompleteTime;

	// Token: 0x04003514 RID: 13588
	private double scanCompleteTime;

	// Token: 0x04003515 RID: 13589
	private BuilderScanKiosk.ScannerState scannerState;

	// Token: 0x0200065F RID: 1631
	private enum ScannerState
	{
		// Token: 0x04003517 RID: 13591
		IDLE,
		// Token: 0x04003518 RID: 13592
		CONFIRMATION,
		// Token: 0x04003519 RID: 13593
		SAVING
	}
}
