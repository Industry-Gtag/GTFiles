using System;
using System.Threading.Tasks;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio;
using Modio.Mods;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000FEE RID: 4078
	public class CustomMapsRoomMapDisplay : MonoBehaviour
	{
		// Token: 0x06006562 RID: 25954 RVA: 0x0020A630 File Offset: 0x00208830
		public void Start()
		{
			this.roomMapNameText.text = this.noRoomMapString;
			this.roomMapStatusText.text = this.notLoadedStatusString;
			this.roomMapLabelText.gameObject.SetActive(true);
			this.roomMapNameText.gameObject.SetActive(true);
			this.roomMapStatusLabelText.gameObject.SetActive(false);
			this.roomMapStatusText.gameObject.SetActive(false);
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnectedFromRoom;
			CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
			CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		}

		// Token: 0x06006563 RID: 25955 RVA: 0x0020A728 File Offset: 0x00208928
		public void OnDestroy()
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnectedFromRoom;
			CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
			CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
			CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		}

		// Token: 0x06006564 RID: 25956 RVA: 0x0020A7B9 File Offset: 0x002089B9
		private void OnJoinedRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06006565 RID: 25957 RVA: 0x0020A7B9 File Offset: 0x002089B9
		private void OnDisconnectedFromRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06006566 RID: 25958 RVA: 0x0020A7B9 File Offset: 0x002089B9
		private void OnRoomMapChanged(ModId roomMapModId)
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06006567 RID: 25959 RVA: 0x0020A7C4 File Offset: 0x002089C4
		private async Task UpdateRoomMap()
		{
			ModId currentRoomMap = CustomMapManager.GetRoomMapId();
			if (currentRoomMap == ModId.Null)
			{
				this.roomMapNameText.text = this.noRoomMapString;
				this.roomMapStatusLabelText.gameObject.SetActive(false);
				this.roomMapStatusText.gameObject.SetActive(false);
			}
			else
			{
				ValueTuple<Error, Mod> valueTuple = await ModIOManager.GetMod(currentRoomMap, false, null);
				Error item = valueTuple.Item1;
				Mod item2 = valueTuple.Item2;
				if (item)
				{
					this.roomMapNameText.text = string.Format("FAILED TO GET MOD INFO.\n({0})", item.Code);
				}
				else
				{
					this.roomMapNameText.text = item2.Name;
					this.roomMapStatusLabelText.gameObject.SetActive(true);
					if (CustomMapLoader.IsMapLoaded(currentRoomMap))
					{
						this.roomMapStatusText.text = this.readyToPlayStatusString;
						this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
					}
					else if (CustomMapManager.IsLoading(currentRoomMap._id))
					{
						this.roomMapStatusText.text = this.loadingStatusString;
						this.roomMapStatusText.color = this.loadingStatusStringColor;
					}
					else
					{
						this.roomMapStatusText.text = this.notLoadedStatusString;
						this.roomMapStatusText.color = this.notLoadedStatusStringColor;
					}
					this.roomMapStatusText.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06006568 RID: 25960 RVA: 0x0020A808 File Offset: 0x00208A08
		private void OnMapLoadComplete(bool success)
		{
			if (success)
			{
				this.roomMapStatusText.text = this.readyToPlayStatusString;
				this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
				return;
			}
			this.roomMapStatusText.text = this.loadFailedStatusString;
			this.roomMapStatusText.color = this.loadFailedStatusStringColor;
		}

		// Token: 0x06006569 RID: 25961 RVA: 0x0020A860 File Offset: 0x00208A60
		private void OnMapLoadProgress(MapLoadStatus status, int progress, string message)
		{
			switch (status)
			{
			case MapLoadStatus.Downloading:
				this.roomMapStatusText.text = ((progress > 0) ? (this.downloadingStatusString + " " + progress.ToString() + "%") : this.downloadingStatusString);
				this.roomMapStatusText.color = this.loadingStatusStringColor;
				return;
			case MapLoadStatus.Loading:
				this.roomMapStatusText.text = this.loadingStatusString;
				this.roomMapStatusText.color = this.loadingStatusStringColor;
				break;
			case MapLoadStatus.Unloading:
			case MapLoadStatus.Error:
				break;
			case MapLoadStatus.Installing:
				this.roomMapStatusText.text = ((progress > 0) ? (this.installingStatusString + " " + progress.ToString() + "%") : this.installingStatusString);
				this.roomMapStatusText.color = this.loadingStatusStringColor;
				return;
			default:
				return;
			}
		}

		// Token: 0x04007456 RID: 29782
		[SerializeField]
		private TMP_Text roomMapLabelText;

		// Token: 0x04007457 RID: 29783
		[SerializeField]
		private TMP_Text roomMapNameText;

		// Token: 0x04007458 RID: 29784
		[SerializeField]
		private TMP_Text roomMapStatusLabelText;

		// Token: 0x04007459 RID: 29785
		[SerializeField]
		private TMP_Text roomMapStatusText;

		// Token: 0x0400745A RID: 29786
		[SerializeField]
		private string noRoomMapString = "NONE";

		// Token: 0x0400745B RID: 29787
		[SerializeField]
		private string notLoadedStatusString = "NOT LOADED";

		// Token: 0x0400745C RID: 29788
		[SerializeField]
		private string loadingStatusString = "LOADING...";

		// Token: 0x0400745D RID: 29789
		[SerializeField]
		private string downloadingStatusString = "DOWNLOADING";

		// Token: 0x0400745E RID: 29790
		[SerializeField]
		private string installingStatusString = "INSTALLING";

		// Token: 0x0400745F RID: 29791
		[SerializeField]
		private string readyToPlayStatusString = "READY!";

		// Token: 0x04007460 RID: 29792
		[SerializeField]
		private string loadFailedStatusString = "LOAD FAILED";

		// Token: 0x04007461 RID: 29793
		[SerializeField]
		private Color notLoadedStatusStringColor = Color.red;

		// Token: 0x04007462 RID: 29794
		[SerializeField]
		private Color loadingStatusStringColor = Color.yellow;

		// Token: 0x04007463 RID: 29795
		[SerializeField]
		private Color readyToPlayStatusStringColor = Color.green;

		// Token: 0x04007464 RID: 29796
		[SerializeField]
		private Color loadFailedStatusStringColor = Color.red;
	}
}
