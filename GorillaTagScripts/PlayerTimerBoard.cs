using System;
using System.Collections.Generic;
using System.Text;
using KID.Model;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000FA1 RID: 4001
	public class PlayerTimerBoard : MonoBehaviour
	{
		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x0600639C RID: 25500 RVA: 0x00200745 File Offset: 0x001FE945
		// (set) Token: 0x0600639D RID: 25501 RVA: 0x0020074D File Offset: 0x001FE94D
		public bool IsDirty { get; set; } = true;

		// Token: 0x0600639E RID: 25502 RVA: 0x00200756 File Offset: 0x001FE956
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x0600639F RID: 25503 RVA: 0x0020075E File Offset: 0x001FE95E
		private void OnEnable()
		{
			this.TryInit();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.RedrawPlayerLines));
		}

		// Token: 0x060063A0 RID: 25504 RVA: 0x00200777 File Offset: 0x001FE977
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.RegisterTimerBoard(this);
			this.isInitialized = true;
		}

		// Token: 0x060063A1 RID: 25505 RVA: 0x002007A2 File Offset: 0x001FE9A2
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.UnregisterTimerBoard(this);
			}
			this.isInitialized = false;
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.RedrawPlayerLines));
		}

		// Token: 0x060063A2 RID: 25506 RVA: 0x002007D4 File Offset: 0x001FE9D4
		public void SetSleepState(bool awake)
		{
			this.playerColumn.enabled = awake;
			this.timeColumn.enabled = awake;
			if (this.linesParent != null)
			{
				this.linesParent.SetActive(awake);
			}
		}

		// Token: 0x060063A3 RID: 25507 RVA: 0x00200808 File Offset: 0x001FEA08
		public void SortLines()
		{
			this.lines.Sort(new Comparison<PlayerTimerBoardLine>(PlayerTimerBoardLine.CompareByTotalTime));
		}

		// Token: 0x060063A4 RID: 25508 RVA: 0x00200824 File Offset: 0x001FEA24
		public void RedrawPlayerLines()
		{
			this.stringBuilder.Clear();
			this.stringBuilderTime.Clear();
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER", out text, "<b><color=yellow>PLAYER</color></b>"))
			{
				Debug.LogError("[LOCALIZATION::MONKE_BLOCKS::TIMER] Failed to get key for Game Mode [MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER]");
			}
			this.stringBuilder.Append("<b><color=yellow>");
			this.stringBuilder.Append(text);
			this.stringBuilder.Append("</color></b>");
			if (!LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES", out text, "<b><color=yellow>LATEST TIME</color></b>"))
			{
				Debug.LogError("[LOCALIZATION::MONKE_BLOCKS::TIMER] Failed to get key for Game Mode [MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES]");
			}
			this.stringBuilderTime.Append("<b><color=yellow>");
			this.stringBuilderTime.Append(text);
			this.stringBuilderTime.Append("</color></b>");
			this.SortLines();
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			bool flag = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
			for (int i = 0; i < this.lines.Count; i++)
			{
				try
				{
					if (this.lines[i].gameObject.activeInHierarchy)
					{
						this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
						if (this.lines[i].linePlayer != null && this.lines[i].linePlayer.InRoom)
						{
							this.stringBuilder.Append("\n ");
							this.stringBuilder.Append(flag ? this.lines[i].playerNameVisible : this.lines[i].linePlayer.DefaultName);
							this.stringBuilderTime.Append("\n ");
							this.stringBuilderTime.Append(this.lines[i].playerTimeStr);
						}
					}
				}
				catch
				{
				}
			}
			this.playerColumn.text = this.stringBuilder.ToString();
			this.timeColumn.text = this.stringBuilderTime.ToString();
			this.IsDirty = false;
		}

		// Token: 0x0400724D RID: 29261
		[SerializeField]
		private GameObject linesParent;

		// Token: 0x0400724E RID: 29262
		public List<PlayerTimerBoardLine> lines;

		// Token: 0x0400724F RID: 29263
		public TextMeshPro notInRoomText;

		// Token: 0x04007250 RID: 29264
		public TextMeshPro playerColumn;

		// Token: 0x04007251 RID: 29265
		public TextMeshPro timeColumn;

		// Token: 0x04007252 RID: 29266
		[SerializeField]
		private int startingYValue;

		// Token: 0x04007253 RID: 29267
		[SerializeField]
		private int lineHeight;

		// Token: 0x04007254 RID: 29268
		private StringBuilder stringBuilder = new StringBuilder(220);

		// Token: 0x04007255 RID: 29269
		private StringBuilder stringBuilderTime = new StringBuilder(220);

		// Token: 0x04007256 RID: 29270
		private const string MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER_KEY = "MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER";

		// Token: 0x04007257 RID: 29271
		private const string MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES_KEY = "MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES";

		// Token: 0x04007258 RID: 29272
		private bool isInitialized;
	}
}
