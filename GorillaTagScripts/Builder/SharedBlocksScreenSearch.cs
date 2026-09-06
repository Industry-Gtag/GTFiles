using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001072 RID: 4210
	public class SharedBlocksScreenSearch : SharedBlocksScreen, IGorillaSliceableSimple
	{
		// Token: 0x060068F7 RID: 26871 RVA: 0x0021C5D8 File Offset: 0x0021A7D8
		public override void OnSelectPressed()
		{
			if (SharedBlocksManager.IsMapIDValid(this.currentMapCode))
			{
				this.savedMapCode = this.currentMapCode;
				this.terminal.SelectMapIDAndOpenInfo(this.savedMapCode);
				return;
			}
			if (this.currentMapCode.Length < 8)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH", out text, "INVALID MAP ID LENGTH"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH]");
				}
				this.terminal.SetStatusText(text);
				return;
			}
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID", out text2, "INVALID MAP ID"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID]");
			}
			this.terminal.SetStatusText(text2);
		}

		// Token: 0x060068F8 RID: 26872 RVA: 0x0021C671 File Offset: 0x0021A871
		public override void OnDeletePressed()
		{
			if (this.currentMapCode.Length > 0)
			{
				this.currentMapCode = this.currentMapCode.Substring(0, this.currentMapCode.Length - 1);
				this.UpdateInput();
			}
		}

		// Token: 0x060068F9 RID: 26873 RVA: 0x0021C6A6 File Offset: 0x0021A8A6
		public override void OnNumberPressed(int number)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += number.ToString();
				this.UpdateInput();
			}
		}

		// Token: 0x060068FA RID: 26874 RVA: 0x0021C6D4 File Offset: 0x0021A8D4
		public override void OnLetterPressed(string letter)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += letter;
				this.UpdateInput();
			}
		}

		// Token: 0x060068FB RID: 26875 RVA: 0x0021C6FC File Offset: 0x0021A8FC
		public override void Show()
		{
			SharedBlocksManager.OnRecentMapIdsUpdated += this.DrawScreen;
			this.currentMapCode = string.Empty;
			this.DrawScreen();
			base.Show();
			this.RefreshPlayerCounter();
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnMapLoaded(table.GetCurrentMapID());
			}
		}

		// Token: 0x060068FC RID: 26876 RVA: 0x0021C788 File Offset: 0x0021A988
		public override void Hide()
		{
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			SharedBlocksManager.OnRecentMapIdsUpdated -= this.DrawScreen;
			base.Hide();
		}

		// Token: 0x060068FD RID: 26877 RVA: 0x0021C810 File Offset: 0x0021AA10
		private void OnMapLoaded(string mapID)
		{
			string text = "LOADED MAP : " + (SharedBlocksManager.IsMapIDValid(mapID) ? SharedBlocksTerminal.MapIDToDisplayedString(mapID) : "NONE");
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale(SharedBlocksManager.IsMapIDValid(mapID) ? "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL" : "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE", out text2, text))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL]");
			}
			text2 = text2.Replace("{mapDisplayName}", SharedBlocksTerminal.MapIDToDisplayedString(mapID));
			this.loadedMap.text = text2;
		}

		// Token: 0x060068FE RID: 26878 RVA: 0x0021C884 File Offset: 0x0021AA84
		private void OnMapCleared()
		{
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE", out text, "LOADED MAP : NONE"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE]");
			}
			this.loadedMap.text = text;
		}

		// Token: 0x060068FF RID: 26879 RVA: 0x0021C8BC File Offset: 0x0021AABC
		private void UpdateInput()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			string text = "MAP SEARCH : ";
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH", out text2, text))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH]");
			}
			text2 += SharedBlocksTerminal.MapIDToDisplayedString(this.currentMapCode);
			this.inputText.text = text2;
		}

		// Token: 0x06006900 RID: 26880 RVA: 0x0021C90E File Offset: 0x0021AB0E
		public void SetMapCode(string mapCode)
		{
			if (mapCode == null)
			{
				this.currentMapCode = string.Empty;
			}
			else
			{
				this.currentMapCode = mapCode;
			}
			this.UpdateInput();
		}

		// Token: 0x06006901 RID: 26881 RVA: 0x0021C92D File Offset: 0x0021AB2D
		public void SetInputTextEnabled(bool enabled)
		{
			if (enabled)
			{
				this.inputText.color = Color.white;
				return;
			}
			this.inputText.color = Color.gray;
		}

		// Token: 0x06006902 RID: 26882 RVA: 0x0021C954 File Offset: 0x0021AB54
		private void DrawScreen()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			this.UpdateInput();
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_VOTES", out text, "RECENT VOTES"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_VOTES]");
			}
			this.sb.Clear();
			this.sb.Append(text + "\n");
			foreach (string text2 in SharedBlocksManager.GetRecentUpVotes())
			{
				if (SharedBlocksManager.IsMapIDValid(text2))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(text2));
					this.sb.Append("\n");
				}
			}
			this.recentList.text = this.sb.ToString();
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL", out text, "MY MAPS"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL]");
			}
			this.sb.Clear();
			this.sb.Append(text + "\n");
			foreach (string text3 in SharedBlocksManager.GetLocalMapIDs())
			{
				if (SharedBlocksManager.IsMapIDValid(text3))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(text3));
					this.sb.Append("\n");
				}
			}
			this.myScanList.text = this.sb.ToString();
		}

		// Token: 0x06006903 RID: 26883 RVA: 0x0021CAEC File Offset: 0x0021ACEC
		private void RefreshPlayerCounter()
		{
			this.terminal.RefreshLobbyCount();
			this.playerCountText.text = this.terminal.GetLobbyText();
			this.playersInLobbyWarning.gameObject.SetActive(!this.terminal.AreAllPlayersInLobby());
		}

		// Token: 0x06006904 RID: 26884 RVA: 0x0021CB38 File Offset: 0x0021AD38
		public void SliceUpdate()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x06006905 RID: 26885 RVA: 0x0021CB40 File Offset: 0x0021AD40
		public void OnEnable()
		{
			if (!this.updating)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = true;
			}
			this.RefreshPlayerCounter();
			RoomSystem.PlayersChangedEvent += new Action(this.PlayersChangedEvent);
		}

		// Token: 0x06006906 RID: 26886 RVA: 0x0021CB38 File Offset: 0x0021AD38
		private void PlayersChangedEvent()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x06006907 RID: 26887 RVA: 0x0021CB79 File Offset: 0x0021AD79
		public void OnDisable()
		{
			if (this.updating)
			{
				GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = false;
			}
			RoomSystem.PlayersChangedEvent -= new Action(this.PlayersChangedEvent);
		}

		// Token: 0x04007865 RID: 30821
		[SerializeField]
		private TMP_Text loadedMap;

		// Token: 0x04007866 RID: 30822
		[SerializeField]
		private TMP_Text inputText;

		// Token: 0x04007867 RID: 30823
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04007868 RID: 30824
		[SerializeField]
		private TMP_Text recentList;

		// Token: 0x04007869 RID: 30825
		[SerializeField]
		private TMP_Text myScanList;

		// Token: 0x0400786A RID: 30826
		[SerializeField]
		private TMP_Text playerCountText;

		// Token: 0x0400786B RID: 30827
		[SerializeField]
		private TMP_Text playersInLobbyWarning;

		// Token: 0x0400786C RID: 30828
		private string currentMapCode;

		// Token: 0x0400786D RID: 30829
		private string savedMapCode;

		// Token: 0x0400786E RID: 30830
		private StringBuilder sb = new StringBuilder();

		// Token: 0x0400786F RID: 30831
		private bool updating;
	}
}
