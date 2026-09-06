using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaGameModes;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FD8 RID: 4056
	public class CustomMapModeSelector : GameModeSelectorButtonLayout
	{
		// Token: 0x060064F5 RID: 25845 RVA: 0x00207E26 File Offset: 0x00206026
		private void Awake()
		{
			CustomMapModeSelector.instances.AddIfNew(this);
		}

		// Token: 0x060064F6 RID: 25846 RVA: 0x00207E34 File Offset: 0x00206034
		public void OnEnable()
		{
			if (GorillaComputer.instance != null)
			{
				this.SetupButtons();
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
			}
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnRoomHostSwitched;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
			this.roomHostDescriptionText.SetActive(false);
			this.roomHostText.gameObject.SetActive(false);
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
			{
				this.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
			}
		}

		// Token: 0x060064F7 RID: 25847 RVA: 0x00207F14 File Offset: 0x00206114
		public void OnDisable()
		{
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnRoomHostSwitched;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x060064F8 RID: 25848 RVA: 0x00207F7E File Offset: 0x0020617E
		private void OnJoinedRoom()
		{
			this.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
		}

		// Token: 0x060064F9 RID: 25849 RVA: 0x00207F90 File Offset: 0x00206190
		private void OnRoomHostSwitched(NetPlayer newRoomHost)
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate)
			{
				return;
			}
			CustomMapModeSelector.reusableString = this.notInRoomHostString;
			if (!newRoomHost.IsNull)
			{
				this.roomHostDescriptionText.SetActive(true);
				CustomMapModeSelector.reusableString = newRoomHost.DefaultName;
				if (GorillaComputer.instance.NametagsEnabled && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
				{
					RigContainer rigContainer;
					if (newRoomHost.IsLocal)
					{
						CustomMapModeSelector.reusableString = newRoomHost.NickName;
					}
					else if (VRRigCache.Instance.TryGetVrrig(newRoomHost, out rigContainer))
					{
						CustomMapModeSelector.reusableString = rigContainer.Rig.playerNameVisible;
					}
				}
			}
			this.roomHostText.text = this.roomHostLabel + CustomMapModeSelector.reusableString;
			this.roomHostText.gameObject.SetActive(true);
		}

		// Token: 0x060064FA RID: 25850 RVA: 0x00208056 File Offset: 0x00206256
		private void OnDisconnected()
		{
			this.roomHostText.gameObject.SetActive(false);
			this.roomHostDescriptionText.SetActive(false);
		}

		// Token: 0x060064FB RID: 25851 RVA: 0x00208078 File Offset: 0x00206278
		public static void ResetButtons()
		{
			CustomMapModeSelector.gamemodes = new List<GameModeType> { GameModeType.Casual };
			CustomMapModeSelector.defaultGamemodeForLoadedMap = GameModeType.Casual;
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.SetupButtons();
			}
			GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
		}

		// Token: 0x060064FC RID: 25852 RVA: 0x002080FC File Offset: 0x002062FC
		public static void SetAvailableGameModes(int[] availableModes, int defaultMode)
		{
			CustomMapModeSelector.gamemodes.Clear();
			CustomMapModeSelector.gamemodes.Add(GameModeType.Casual);
			if (availableModes != null)
			{
				foreach (int num in availableModes)
				{
					CustomMapModeSelector.gamemodes.Add((GameModeType)num);
				}
			}
			CustomMapModeSelector.defaultGamemodeForLoadedMap = (GameModeType)defaultMode;
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.SetupButtons();
			}
			GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
		}

		// Token: 0x060064FD RID: 25853 RVA: 0x002081A4 File Offset: 0x002063A4
		protected override async void SetupButtons()
		{
			if (this.superToggleButton != null)
			{
				this.superToggleButton.transform.parent.gameObject.SetActive(false);
			}
			int count = 0;
			while (GorillaComputer.instance == null)
			{
				await Task.Delay(100);
			}
			bool flag = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone != this.zone;
			foreach (GameModeType gameModeType in CustomMapModeSelector.gamemodes)
			{
				if (count == this.currentButtons.Count)
				{
					this.currentButtons.Add(Object.Instantiate<ModeSelectButton>(this.pf_button, base.transform));
				}
				ModeSelectButton modeSelectButton = this.currentButtons[count];
				modeSelectButton.transform.localPosition = new Vector3((float)count * -0.15f, 0f, 0f);
				modeSelectButton.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
				modeSelectButton.WarningScreen = this.warningScreen;
				modeSelectButton.SetInfo(gameModeType.ToString(), GameMode.GameModeZoneMapping.GetModeName(gameModeType), GameMode.GameModeZoneMapping.IsNew(gameModeType), GameMode.GameModeZoneMapping.GetCountdown(gameModeType));
				modeSelectButton.gameObject.SetActive(true);
				count++;
				flag |= GorillaComputer.instance.currentGameMode.Value.ToUpper() == gameModeType.ToString().ToUpper();
			}
			for (int i = count; i < this.currentButtons.Count; i++)
			{
				this.currentButtons[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x060064FE RID: 25854 RVA: 0x002081DC File Offset: 0x002063DC
		public static void RefreshHostName()
		{
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
			}
		}

		// Token: 0x040073AA RID: 29610
		[SerializeField]
		private TMP_Text roomHostText;

		// Token: 0x040073AB RID: 29611
		[SerializeField]
		private GameObject roomHostDescriptionText;

		// Token: 0x040073AC RID: 29612
		[SerializeField]
		private string notInRoomHostString = "-NOT IN ROOM-";

		// Token: 0x040073AD RID: 29613
		[SerializeField]
		private string roomHostLabel = "ROOM HOST: ";

		// Token: 0x040073AE RID: 29614
		private static List<GameModeType> gamemodes = new List<GameModeType> { GameModeType.Casual };

		// Token: 0x040073AF RID: 29615
		private static GameModeType defaultGamemodeForLoadedMap = GameModeType.Casual;

		// Token: 0x040073B0 RID: 29616
		private static List<CustomMapModeSelector> instances = new List<CustomMapModeSelector>();

		// Token: 0x040073B1 RID: 29617
		private static string reusableString = "";
	}
}
