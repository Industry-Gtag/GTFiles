using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001077 RID: 4215
	public class SharedBlocksVotingStation : MonoBehaviour
	{
		// Token: 0x0600693C RID: 26940 RVA: 0x0021E00C File Offset: 0x0021C20C
		private void Start()
		{
			this.SetupLocalization();
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.tableZone, out builderTable))
			{
				this.table = builderTable;
				this.table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnLoadedMapChanged(this.table.GetCurrentMapID());
			}
			else
			{
				GTDev.LogWarning<string>("No Builder Table found for Voting Station", null);
			}
			base.GetComponentsInChildren<MeshRenderer>(false, this.meshes);
			this.upVoteButton.onPressButton.AddListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.AddListener(new UnityAction(this.OnDownVotePressed));
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x0600693D RID: 26941 RVA: 0x0021E0FC File Offset: 0x0021C2FC
		private void OnDestroy()
		{
			this.upVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnDownVotePressed));
			if (this.table != null)
			{
				this.table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x0600693E RID: 26942 RVA: 0x0021E1BC File Offset: 0x0021C3BC
		private void SetupLocalization()
		{
			if (this._statusLocText == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Trying to set up Localization, but [_statusLocText] is NULL");
				return;
			}
			if (this._screenLocText == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Trying to set up Localization, but [_screenLocText] is NULL");
				return;
			}
			string text = "voting-status-index";
			string text2 = "map-name-index";
			string text3 = "map-name";
			this._statusIndexVar = this._statusLocText.StringReference[text] as IntVariable;
			this._mapDisplayIndexVar = this._screenLocText.StringReference[text2] as IntVariable;
			this._mapNameVar = this._screenLocText.StringReference[text3] as StringVariable;
			if (this._statusIndexVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [IntVariable] with var-name [" + text + "]");
			}
			if (this._mapDisplayIndexVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [IntVariable] with var-name [" + text2 + "]");
			}
			if (this._mapNameVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [StringVariable] with var-name [" + text3 + "]");
			}
		}

		// Token: 0x0600693F RID: 26943 RVA: 0x0021E2B8 File Offset: 0x0021C4B8
		private void OnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(this.tableZone);
			foreach (MeshRenderer meshRenderer in this.meshes)
			{
				meshRenderer.enabled = flag;
			}
		}

		// Token: 0x06006940 RID: 26944 RVA: 0x0021E31C File Offset: 0x0021C51C
		private void OnUpVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this._statusIndexVar.Value = 2;
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.upVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, true, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.upVoteButton.pressedMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.upVoteButton.enabled = false;
				this.downVoteButton.enabled = true;
			}
		}

		// Token: 0x06006941 RID: 26945 RVA: 0x0021E3D8 File Offset: 0x0021C5D8
		private void OnDownVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this._statusIndexVar.Value = 2;
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.downVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, false, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.downVoteButton.pressedMaterial;
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = false;
			}
		}

		// Token: 0x06006942 RID: 26946 RVA: 0x0021E494 File Offset: 0x0021C694
		private void OnVoteResponse(bool success, string message)
		{
			this.voteInProgress = false;
			if (success)
			{
				this._statusIndexVar.Value = 0;
				this.statusText.gameObject.SetActive(true);
			}
			else
			{
				int num;
				if (int.TryParse(message, out num))
				{
					this._statusIndexVar.Value = num;
				}
				else
				{
					this.statusText.text = message;
					Debug.Log("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] WARNING: Passing in a non-int value for the [message]. This will not be localized!");
				}
				this.statusText.gameObject.SetActive(true);
				if (!this.loadedMapID.IsNullOrEmpty())
				{
					this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.upVoteButton.enabled = true;
					this.downVoteButton.enabled = true;
				}
			}
			this.clearStatusTime = Time.time + this.clearStatusDelay;
			this.waitingToClearStatus = true;
		}

		// Token: 0x06006943 RID: 26947 RVA: 0x0021E576 File Offset: 0x0021C776
		private void LateUpdate()
		{
			if (this.waitingToClearStatus && Time.time > this.clearStatusTime)
			{
				this.waitingToClearStatus = false;
				this._statusIndexVar.Value = 2;
				this.statusText.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006944 RID: 26948 RVA: 0x0021E5B1 File Offset: 0x0021C7B1
		private void OnLoadedMapChanged(string mapID)
		{
			this.loadedMapID = mapID;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x06006945 RID: 26949 RVA: 0x0021E5D1 File Offset: 0x0021C7D1
		private void OnMapCleared()
		{
			this.loadedMapID = null;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x06006946 RID: 26950 RVA: 0x0021E5F4 File Offset: 0x0021C7F4
		private void UpdateScreen()
		{
			if (!this.loadedMapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(this.loadedMapID))
			{
				this._mapDisplayIndexVar.Value = 1;
				this._mapNameVar.Value = SharedBlocksTerminal.MapIDToDisplayedString(this.loadedMapID);
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = true;
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				return;
			}
			this._mapDisplayIndexVar.Value = 0;
			this.upVoteButton.enabled = false;
			this.downVoteButton.enabled = false;
			this.upVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
			this.downVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
		}

		// Token: 0x040078C7 RID: 30919
		public const int VOTING_STATUS_INDEX_SUCCESS = 0;

		// Token: 0x040078C8 RID: 30920
		public const int VOTING_STATUS_INDEX_NOT_LOGGED_IN = 1;

		// Token: 0x040078C9 RID: 30921
		public const int VOTING_STATUS_INDEX_EMPTY = 2;

		// Token: 0x040078CA RID: 30922
		private const int MAP_DISPLAY_INDEX_NONE = 0;

		// Token: 0x040078CB RID: 30923
		private const int MAP_DISPLAY_INDEX_NAMED_MAP = 1;

		// Token: 0x040078CC RID: 30924
		[SerializeField]
		private TMP_Text screenText;

		// Token: 0x040078CD RID: 30925
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x040078CE RID: 30926
		[SerializeField]
		private GorillaPressableButton upVoteButton;

		// Token: 0x040078CF RID: 30927
		[SerializeField]
		private GorillaPressableButton downVoteButton;

		// Token: 0x040078D0 RID: 30928
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x040078D1 RID: 30929
		[SerializeField]
		private Material buttonDefaultMaterial;

		// Token: 0x040078D2 RID: 30930
		[SerializeField]
		private Material buttonDisabledMaterial;

		// Token: 0x040078D3 RID: 30931
		[Header("Localization Setup")]
		[SerializeField]
		private LocalizedText _statusLocText;

		// Token: 0x040078D4 RID: 30932
		[SerializeField]
		private LocalizedText _screenLocText;

		// Token: 0x040078D5 RID: 30933
		private BuilderTable table;

		// Token: 0x040078D6 RID: 30934
		private string loadedMapID = string.Empty;

		// Token: 0x040078D7 RID: 30935
		private bool voteInProgress;

		// Token: 0x040078D8 RID: 30936
		private bool waitingToClearStatus;

		// Token: 0x040078D9 RID: 30937
		private float clearStatusTime;

		// Token: 0x040078DA RID: 30938
		private float clearStatusDelay = 2f;

		// Token: 0x040078DB RID: 30939
		private IntVariable _statusIndexVar;

		// Token: 0x040078DC RID: 30940
		private IntVariable _mapDisplayIndexVar;

		// Token: 0x040078DD RID: 30941
		private StringVariable _mapNameVar;

		// Token: 0x040078DE RID: 30942
		private List<MeshRenderer> meshes = new List<MeshRenderer>(12);
	}
}
