using System;
using System.Collections;
using System.Linq;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;

// Token: 0x02000897 RID: 2199
public class Gorillanalytics : MonoBehaviour
{
	// Token: 0x06003953 RID: 14675 RVA: 0x001387F2 File Offset: 0x001369F2
	private IEnumerator Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("GorillanalyticsChance", delegate(string s)
		{
			double num;
			if (double.TryParse(s, out num))
			{
				this.oneOverChance = num;
			}
		}, delegate(PlayFabError e)
		{
		}, false);
		for (;;)
		{
			yield return new WaitForSecondsRealtime(this.interval);
			if ((double)Random.Range(0f, 1f) < 1.0 / this.oneOverChance && PlayFabClientAPI.IsClientLoggedIn())
			{
				this.UploadGorillanalytics();
			}
		}
		yield break;
	}

	// Token: 0x06003954 RID: 14676 RVA: 0x00138804 File Offset: 0x00136A04
	private void UploadGorillanalytics()
	{
		try
		{
			string text;
			string text2;
			string text3;
			this.GetMapModeQueue(out text, out text2, out text3);
			Vector3 position = GTPlayer.Instance.headCollider.transform.position;
			Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
			this.uploadData.version = NetworkSystemConfig.AppVersion;
			this.uploadData.upload_chance = this.oneOverChance;
			this.uploadData.map = text;
			this.uploadData.mode = text2;
			this.uploadData.queue = text3;
			this.uploadData.player_count = (int)(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.PlayerCount : 0);
			this.uploadData.pos_x = position.x;
			this.uploadData.pos_y = position.y;
			this.uploadData.pos_z = position.z;
			this.uploadData.vel_x = averagedVelocity.x;
			this.uploadData.vel_y = averagedVelocity.y;
			this.uploadData.vel_z = averagedVelocity.z;
			this.uploadData.cosmetics_owned = string.Join(";", CosmeticsController.instance.unlockedCosmetics.Select((CosmeticsController.CosmeticItem c) => c.itemName));
			this.uploadData.cosmetics_worn = string.Join(";", CosmeticsController.instance.currentWornSet.items.Select((CosmeticsController.CosmeticItem c) => c.itemName));
			GorillaServer.Instance.UploadGorillanalytics(this.uploadData);
			GorillaTelemetry.EnqueueTelemetryEvent("periodic_player_state", this.uploadData, null);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x06003955 RID: 14677 RVA: 0x001389E4 File Offset: 0x00136BE4
	private void GetMapModeQueue(out string map, out string mode, out string queue)
	{
		if (!PhotonNetwork.InRoom)
		{
			map = "none";
			mode = "none";
			queue = "none";
			return;
		}
		object obj = null;
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null)
		{
			currentRoom.CustomProperties.TryGetValue("gameMode", out obj);
		}
		GameModeString gameModeString = GameModeString.FromString(((obj != null) ? obj.ToString() : null) ?? "");
		GTZone gtzone = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone;
		if (gtzone == GTZone.cityNoBuildings || gtzone == GTZone.cityWithSkyJungle || gtzone == GTZone.mall)
		{
			gtzone = GTZone.city;
		}
		if (gtzone == GTZone.tutorial)
		{
			gtzone = GTZone.forest;
		}
		if (gtzone == GTZone.ghostReactorTunnel)
		{
			gtzone = GTZone.ghostReactor;
		}
		map = gtzone.ToString().ToLower();
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			map += "private";
		}
		mode = ((gameModeString != null) ? gameModeString.gameType.ToUpper() : null);
		if (mode.IsNullOrEmpty())
		{
			mode = "none";
		}
		queue = ((gameModeString != null) ? gameModeString.queue.ToUpper() : null);
		if (queue.IsNullOrEmpty())
		{
			queue = "none";
		}
	}

	// Token: 0x04004973 RID: 18803
	public float interval = 60f;

	// Token: 0x04004974 RID: 18804
	public double oneOverChance = 4320.0;

	// Token: 0x04004975 RID: 18805
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04004976 RID: 18806
	public GameModeZoneMapping gameModeData;

	// Token: 0x04004977 RID: 18807
	private readonly Gorillanalytics.UploadData uploadData = new Gorillanalytics.UploadData();

	// Token: 0x04004978 RID: 18808
	public const string GORILLANALYTICS_EVENT_NAME = "periodic_player_state";

	// Token: 0x02000898 RID: 2200
	private class UploadData
	{
		// Token: 0x04004979 RID: 18809
		public string version;

		// Token: 0x0400497A RID: 18810
		public double upload_chance;

		// Token: 0x0400497B RID: 18811
		public string map;

		// Token: 0x0400497C RID: 18812
		public string mode;

		// Token: 0x0400497D RID: 18813
		public string queue;

		// Token: 0x0400497E RID: 18814
		public int player_count;

		// Token: 0x0400497F RID: 18815
		public float pos_x;

		// Token: 0x04004980 RID: 18816
		public float pos_y;

		// Token: 0x04004981 RID: 18817
		public float pos_z;

		// Token: 0x04004982 RID: 18818
		public float vel_x;

		// Token: 0x04004983 RID: 18819
		public float vel_y;

		// Token: 0x04004984 RID: 18820
		public float vel_z;

		// Token: 0x04004985 RID: 18821
		public string cosmetics_owned;

		// Token: 0x04004986 RID: 18822
		public string cosmetics_worn;
	}
}
