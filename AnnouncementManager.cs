using System;
using GorillaNetworking;
using LitJson;
using PlayFab;
using UnityEngine;

// Token: 0x02000B27 RID: 2855
public class AnnouncementManager : MonoBehaviour
{
	// Token: 0x06004951 RID: 18769 RVA: 0x001881FA File Offset: 0x001863FA
	public bool ShowAnnouncement()
	{
		return this._showAnnouncement;
	}

	// Token: 0x170006F6 RID: 1782
	// (get) Token: 0x06004952 RID: 18770 RVA: 0x00188202 File Offset: 0x00186402
	// (set) Token: 0x06004953 RID: 18771 RVA: 0x0018820A File Offset: 0x0018640A
	public bool _completedSetup { get; private set; }

	// Token: 0x170006F7 RID: 1783
	// (get) Token: 0x06004954 RID: 18772 RVA: 0x00188213 File Offset: 0x00186413
	// (set) Token: 0x06004955 RID: 18773 RVA: 0x0018821B File Offset: 0x0018641B
	public bool _announcementActive { get; private set; }

	// Token: 0x170006F8 RID: 1784
	// (get) Token: 0x06004956 RID: 18774 RVA: 0x00188224 File Offset: 0x00186424
	public static AnnouncementManager Instance
	{
		get
		{
			if (AnnouncementManager._instance == null)
			{
				Debug.LogError("[KID::ANNOUNCEMENT] [_instance] is NULL, does it exist in the scene?");
			}
			return AnnouncementManager._instance;
		}
	}

	// Token: 0x170006F9 RID: 1785
	// (get) Token: 0x06004957 RID: 18775 RVA: 0x00188242 File Offset: 0x00186442
	private static string AnnouncementDPlayerPref
	{
		get
		{
			if (string.IsNullOrEmpty(AnnouncementManager._announcementIDPref))
			{
				AnnouncementManager._announcementIDPref = "announcement-id-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return AnnouncementManager._announcementIDPref;
		}
	}

	// Token: 0x06004958 RID: 18776 RVA: 0x00188270 File Offset: 0x00186470
	private void Awake()
	{
		if (AnnouncementManager._instance != null)
		{
			Debug.LogError("[KID::ANNOUNCEMENT] [AnnouncementManager] has already been setup, does another already exist in the scene?");
			return;
		}
		AnnouncementManager._instance = this;
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Announcement Message Box has not been set. Announcement system will not work without it");
		}
	}

	// Token: 0x06004959 RID: 18777 RVA: 0x001882A8 File Offset: 0x001864A8
	private void Start()
	{
		if (this._announcementMessageBox == null)
		{
			return;
		}
		this._announcementMessageBox.RightButton = "";
		this._announcementMessageBox.LeftButton = "Continue";
		PlayFabTitleDataCache.Instance.GetTitleData("AnnouncementData", new Action<string>(this.ConfigureAnnouncement), new Action<PlayFabError>(this.OnError), false);
	}

	// Token: 0x0600495A RID: 18778 RVA: 0x0018830C File Offset: 0x0018650C
	public void OnContinuePressed()
	{
		HandRayController.Instance.DisableHandRays();
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Message Box is null, Continue Button cannot work");
			return;
		}
		PrivateUIRoom.RemoveUI(this._announcementMessageBox.transform);
		this._announcementActive = false;
		PlayerPrefs.SetString(AnnouncementManager.AnnouncementDPlayerPref, this._announcementData.AnnouncementID);
		PlayerPrefs.Save();
	}

	// Token: 0x0600495B RID: 18779 RVA: 0x0018836D File Offset: 0x0018656D
	private void OnError(PlayFabError error)
	{
		Debug.LogError("[ANNOUNCEMENT] Failed to Get Title Data for key [AnnouncementData]. Error:\n[" + error.ErrorMessage);
		this._completedSetup = true;
	}

	// Token: 0x0600495C RID: 18780 RVA: 0x0018838C File Offset: 0x0018658C
	private void ConfigureAnnouncement(string data)
	{
		this._announcementString = data;
		this._announcementData = JsonMapper.ToObject<SAnnouncementData>(this._announcementString);
		if (!bool.TryParse(this._announcementData.ShowAnnouncement, out this._showAnnouncement))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Failed to parse [ShowAnnouncement] with value [" + this._announcementData.ShowAnnouncement + "] to a bool, assuming false");
			return;
		}
		if (!this.ShowAnnouncement())
		{
			this._completedSetup = true;
			return;
		}
		if (string.IsNullOrEmpty(this._announcementData.AnnouncementID))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Announcement Version is empty or null. Will not show announcement");
			return;
		}
		string @string = PlayerPrefs.GetString(AnnouncementManager.AnnouncementDPlayerPref, "");
		if (this._announcementData.AnnouncementID == @string)
		{
			this._completedSetup = true;
			return;
		}
		PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
		HandRayController.Instance.EnableHandRays();
		this._announcementMessageBox.Header = this._announcementData.AnnouncementTitle;
		this._announcementMessageBox.Body = this._announcementData.Message;
		this._announcementActive = true;
		PrivateUIRoom.AddUI(this._announcementMessageBox.transform);
		this._completedSetup = true;
	}

	// Token: 0x04005B88 RID: 23432
	private const string ANNOUNCEMENT_ID_PLAYERPREF_PREFIX = "announcement-id-";

	// Token: 0x04005B89 RID: 23433
	private const string ANNOUNCEMENT_TITLE_DATA_KEY = "AnnouncementData";

	// Token: 0x04005B8A RID: 23434
	private const string ANNOUNCEMENT_HEADING = "Announcement!";

	// Token: 0x04005B8B RID: 23435
	private const string ANNOUNCEMENT_BUTTON_TEXT = "Continue";

	// Token: 0x04005B8C RID: 23436
	[SerializeField]
	private MessageBox _announcementMessageBox;

	// Token: 0x04005B8D RID: 23437
	private string _announcementString = string.Empty;

	// Token: 0x04005B8E RID: 23438
	private SAnnouncementData _announcementData;

	// Token: 0x04005B8F RID: 23439
	private bool _showAnnouncement;

	// Token: 0x04005B92 RID: 23442
	private static AnnouncementManager _instance;

	// Token: 0x04005B93 RID: 23443
	private static string _announcementIDPref = "";
}
