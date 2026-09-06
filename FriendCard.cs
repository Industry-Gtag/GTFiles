using System;
using GorillaNetworking;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x02000CBB RID: 3259
public class FriendCard : MonoBehaviour
{
	// Token: 0x170007A5 RID: 1957
	// (get) Token: 0x06005092 RID: 20626 RVA: 0x001AAC10 File Offset: 0x001A8E10
	public TextMeshProUGUI NameText
	{
		get
		{
			return this.nameText;
		}
	}

	// Token: 0x170007A6 RID: 1958
	// (get) Token: 0x06005093 RID: 20627 RVA: 0x001AAC18 File Offset: 0x001A8E18
	public TextMeshProUGUI RoomText
	{
		get
		{
			return this.roomText;
		}
	}

	// Token: 0x170007A7 RID: 1959
	// (get) Token: 0x06005094 RID: 20628 RVA: 0x001AAC20 File Offset: 0x001A8E20
	public TextMeshProUGUI ZoneText
	{
		get
		{
			return this.zoneText;
		}
	}

	// Token: 0x170007A8 RID: 1960
	// (get) Token: 0x06005095 RID: 20629 RVA: 0x001AAC28 File Offset: 0x001A8E28
	public float Width
	{
		get
		{
			return this.width;
		}
	}

	// Token: 0x170007A9 RID: 1961
	// (get) Token: 0x06005096 RID: 20630 RVA: 0x001AAC30 File Offset: 0x001A8E30
	// (set) Token: 0x06005097 RID: 20631 RVA: 0x001AAC38 File Offset: 0x001A8E38
	public float Height { get; private set; } = 0.25f;

	// Token: 0x06005098 RID: 20632 RVA: 0x001AAC41 File Offset: 0x001A8E41
	private void Awake()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(false);
		}
	}

	// Token: 0x06005099 RID: 20633 RVA: 0x001AAC61 File Offset: 0x001A8E61
	private void OnDestroy()
	{
		if (this._button)
		{
			this._button.onPressed -= this.OnButtonPressed;
		}
	}

	// Token: 0x0600509A RID: 20634 RVA: 0x001AAC87 File Offset: 0x001A8E87
	public void Init(FriendDisplay owner)
	{
		this.friendDisplay = owner;
	}

	// Token: 0x0600509B RID: 20635 RVA: 0x001AAC90 File Offset: 0x001A8E90
	private void UpdateComponentStates()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(this.canRemove);
		}
		bool flag = this._isVimSlot && !SubscriptionManager.IsLocalSubscribed();
		if (this.canRemove)
		{
			this.SetButtonState((this.currentFriend != null) ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
			return;
		}
		if (this.joinable && !flag)
		{
			this.SetButtonState(FriendDisplay.ButtonState.Active);
			return;
		}
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x0600509C RID: 20636 RVA: 0x001AAD0C File Offset: 0x001A8F0C
	private void SetButtonState(FriendDisplay.ButtonState newState)
	{
		if (this._button == null)
		{
			return;
		}
		if (this._buttonState == newState)
		{
			return;
		}
		this._buttonState = newState;
		MeshRenderer buttonRenderer = this._button.buttonRenderer;
		FriendDisplay.ButtonState buttonState = this._buttonState;
		Material[] array;
		switch (buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			array = this._buttonDefaultMaterials;
			break;
		case FriendDisplay.ButtonState.Active:
			array = this._buttonActiveMaterials;
			break;
		case FriendDisplay.ButtonState.Alert:
			array = this._buttonAlertMaterials;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(buttonState);
			break;
		}
		buttonRenderer.sharedMaterials = array;
		this._button.delayTime = (float)((this._buttonState == FriendDisplay.ButtonState.Alert) ? 3 : 0);
	}

	// Token: 0x0600509D RID: 20637 RVA: 0x001AADA6 File Offset: 0x001A8FA6
	public void Populate(FriendBackendController.Friend friend)
	{
		this.Populate(friend, false);
	}

	// Token: 0x0600509E RID: 20638 RVA: 0x001AADB0 File Offset: 0x001A8FB0
	public void Populate(FriendBackendController.Friend friend, bool isVimSlot)
	{
		this.SetEmpty(isVimSlot);
		if (friend != null && friend.Presence != null)
		{
			if (friend.Presence.UserName != null)
			{
				this.SetName(friend.Presence.UserName.ToUpper());
			}
			if (!string.IsNullOrEmpty(friend.Presence.RoomId) && friend.Presence.RoomId.Length > 0)
			{
				bool? isPublic = friend.Presence.IsPublic;
				bool flag = true;
				bool flag2 = (isPublic.GetValueOrDefault() == flag) & (isPublic != null);
				bool flag3 = friend.Presence.RoomId[0] == '@';
				bool flag4 = friend.Presence.RoomId.Equals(NetworkSystem.Instance.RoomName);
				bool flag5 = false;
				if (!flag4 && flag2 && !friend.Presence.Zone.IsNullOrEmpty())
				{
					string text = friend.Presence.Zone.ToLower();
					foreach (GTZone gtzone in ZoneManagement.instance.activeZones)
					{
						if (text.Contains(gtzone.GetName<GTZone>().ToLower()))
						{
							flag5 = true;
						}
					}
				}
				this.joinable = !flag3 && !flag4 && (!flag2 || flag5) && this.HasKIDPermissionToJoinPrivateRooms();
				if (flag3)
				{
					this.SetRoom(friend.Presence.RoomId.Substring(1).ToUpper());
					this.SetZone("CUSTOM");
				}
				else if (!flag2)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone("PRIVATE");
				}
				else if (friend.Presence.Zone != null)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone(friend.Presence.Zone.ToUpper());
				}
			}
			else
			{
				this.joinable = false;
				this.SetRoom("OFFLINE");
			}
			this.currentFriend = friend;
		}
		this.UpdateComponentStates();
	}

	// Token: 0x0600509F RID: 20639 RVA: 0x001AAFCC File Offset: 0x001A91CC
	public void SetName(string friendName)
	{
		TMP_Text tmp_Text = this.nameText;
		this._friendName = friendName;
		tmp_Text.text = friendName;
	}

	// Token: 0x060050A0 RID: 20640 RVA: 0x001AAFF0 File Offset: 0x001A91F0
	public void SetRoom(string friendRoom)
	{
		TMP_Text tmp_Text = this.roomText;
		this._friendRoom = friendRoom;
		tmp_Text.text = friendRoom;
	}

	// Token: 0x060050A1 RID: 20641 RVA: 0x001AB014 File Offset: 0x001A9214
	public void SetZone(string friendZone)
	{
		TMP_Text tmp_Text = this.zoneText;
		this._friendZone = friendZone;
		tmp_Text.text = friendZone;
	}

	// Token: 0x060050A2 RID: 20642 RVA: 0x001AB038 File Offset: 0x001A9238
	public void Randomize()
	{
		this.SetEmpty();
		int num = Random.Range(0, this.randomNames.Length);
		this.SetName(this.randomNames[num].ToUpper());
		this.SetRoom(string.Format("{0}{1}{2}{3}", new object[]
		{
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91)
		}));
		bool flag = Random.Range(0f, 1f) > 0.5f;
		this.joinable = flag && Random.Range(0f, 1f) > 0.5f;
		if (flag)
		{
			int num2 = Random.Range(0, 17);
			GTZone gtzone = (GTZone)num2;
			this.SetZone(gtzone.ToString().ToUpper());
		}
		else
		{
			this.SetZone(this.privateString);
		}
		this.UpdateComponentStates();
	}

	// Token: 0x060050A3 RID: 20643 RVA: 0x001AB13E File Offset: 0x001A933E
	public void SetEmpty()
	{
		this.SetEmpty(this._isVimSlot);
	}

	// Token: 0x060050A4 RID: 20644 RVA: 0x001AB14C File Offset: 0x001A934C
	public void SetEmpty(bool isVimSlot)
	{
		base.CancelInvoke("RestoreAfterResubscribeMessage");
		this.SetName(this.emptyString);
		this.SetRoom(this.emptyString);
		this.SetZone(this.emptyString);
		this.joinable = false;
		this.currentFriend = null;
		this._isVimSlot = isVimSlot;
		this.UpdateComponentStates();
	}

	// Token: 0x060050A5 RID: 20645 RVA: 0x001AB1A3 File Offset: 0x001A93A3
	public void SetRemoveEnabled(bool enabled)
	{
		this.canRemove = enabled;
		this.UpdateComponentStates();
	}

	// Token: 0x060050A6 RID: 20646 RVA: 0x001AB1B4 File Offset: 0x001A93B4
	private void JoinButtonPressed()
	{
		if (this.joinable && this.currentFriend != null && this.currentFriend.Presence != null)
		{
			bool? isPublic = this.currentFriend.Presence.IsPublic;
			bool flag = true;
			JoinType joinType = (((isPublic.GetValueOrDefault() == flag) & (isPublic != null)) ? JoinType.FriendStationPublic : JoinType.FriendStationPrivate);
			GorillaComputer.instance.roomToJoin = this._friendRoom;
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(this._friendRoom, joinType);
			this.joinable = false;
			this.UpdateComponentStates();
		}
	}

	// Token: 0x060050A7 RID: 20647 RVA: 0x001AB23C File Offset: 0x001A943C
	private void RemoveFriendButtonPressed()
	{
		if (this.friendDisplay.InRemoveMode)
		{
			FriendSystem.Instance.RemoveFriend(this.currentFriend, null);
			this.SetEmpty();
		}
	}

	// Token: 0x060050A8 RID: 20648 RVA: 0x001AB264 File Offset: 0x001A9464
	private void OnDrawGizmosSelected()
	{
		float num = this.width * 0.5f * base.transform.lossyScale.x;
		float num2 = this.Height * 0.5f * base.transform.lossyScale.y;
		float num3 = num;
		float num4 = num2;
		Vector3 vector = base.transform.position + base.transform.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector2 = base.transform.position + base.transform.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector3 = base.transform.position + base.transform.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 vector4 = base.transform.position + base.transform.rotation * new Vector3(num3, -num4, 0f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector4, vector3);
		Gizmos.DrawLine(vector3, vector);
	}

	// Token: 0x060050A9 RID: 20649 RVA: 0x001AB398 File Offset: 0x001A9598
	public void SetButton(GorillaPressableDelayButton friendCardButton, Material[] normalMaterials, Material[] activeMaterials, Material[] alertMaterials, TextMeshProUGUI buttonText)
	{
		this._button = friendCardButton;
		this._button.SetFillBar(this.removeProgressBar);
		this._button.onPressBegin += this.OnButtonPressBegin;
		this._button.onPressAbort += this.OnButtonPressAbort;
		this._button.onPressed += this.OnButtonPressed;
		this._buttonDefaultMaterials = normalMaterials;
		this._buttonActiveMaterials = activeMaterials;
		this._buttonAlertMaterials = alertMaterials;
		this._buttonText = buttonText;
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060050AA RID: 20650 RVA: 0x001AB427 File Offset: 0x001A9627
	private void OnRemoveFriendBegin()
	{
		this.nameText.text = "REMOVING";
		this.roomText.text = "FRIEND";
		this.zoneText.text = this.emptyString;
	}

	// Token: 0x060050AB RID: 20651 RVA: 0x001AB45A File Offset: 0x001A965A
	private void OnRemoveFriendEnd()
	{
		this.nameText.text = this._friendName;
		this.roomText.text = this._friendRoom;
		this.zoneText.text = this._friendZone;
	}

	// Token: 0x060050AC RID: 20652 RVA: 0x001AB490 File Offset: 0x001A9690
	private void OnButtonPressBegin()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendBegin();
			break;
		default:
			return;
		}
	}

	// Token: 0x060050AD RID: 20653 RVA: 0x001AB4C0 File Offset: 0x001A96C0
	private void OnButtonPressAbort()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendEnd();
			break;
		default:
			return;
		}
	}

	// Token: 0x060050AE RID: 20654 RVA: 0x001AB4F0 File Offset: 0x001A96F0
	private void OnButtonPressed(GorillaPressableButton button, bool isLeftHand)
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			if (this._isVimSlot && this.currentFriend != null && !SubscriptionManager.IsLocalSubscribed())
			{
				this.ShowResubscribeMessage();
				return;
			}
			break;
		case FriendDisplay.ButtonState.Active:
			this.JoinButtonPressed();
			return;
		case FriendDisplay.ButtonState.Alert:
			this.RemoveFriendButtonPressed();
			break;
		default:
			return;
		}
	}

	// Token: 0x060050AF RID: 20655 RVA: 0x001AB544 File Offset: 0x001A9744
	private void ShowResubscribeMessage()
	{
		base.CancelInvoke("RestoreAfterResubscribeMessage");
		this.nameText.text = "RESUBSCRIBE TO UNLOCK!";
		this.roomText.text = this.emptyString;
		this.zoneText.text = this.emptyString;
		base.Invoke("RestoreAfterResubscribeMessage", 2.5f);
	}

	// Token: 0x060050B0 RID: 20656 RVA: 0x001AB45A File Offset: 0x001A965A
	private void RestoreAfterResubscribeMessage()
	{
		this.nameText.text = this._friendName;
		this.roomText.text = this._friendRoom;
		this.zoneText.text = this._friendZone;
	}

	// Token: 0x060050B1 RID: 20657 RVA: 0x001AB59E File Offset: 0x001A979E
	private bool HasKIDPermissionToJoinPrivateRooms()
	{
		return !KIDManager.KidEnabled || (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer));
	}

	// Token: 0x040062A9 RID: 25257
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x040062AA RID: 25258
	[SerializeField]
	private TextMeshProUGUI roomText;

	// Token: 0x040062AB RID: 25259
	[SerializeField]
	private TextMeshProUGUI zoneText;

	// Token: 0x040062AC RID: 25260
	[SerializeField]
	private Transform removeProgressBar;

	// Token: 0x040062AD RID: 25261
	[SerializeField]
	private float width = 0.25f;

	// Token: 0x040062AF RID: 25263
	private const string ResubscribeMessage = "RESUBSCRIBE TO UNLOCK!";

	// Token: 0x040062B0 RID: 25264
	private const float ResubscribeMessageDuration = 2.5f;

	// Token: 0x040062B1 RID: 25265
	private string emptyString = "";

	// Token: 0x040062B2 RID: 25266
	private string privateString = "PRIVATE";

	// Token: 0x040062B3 RID: 25267
	private bool joinable;

	// Token: 0x040062B4 RID: 25268
	private bool canRemove;

	// Token: 0x040062B5 RID: 25269
	private bool _isVimSlot;

	// Token: 0x040062B6 RID: 25270
	private GorillaPressableDelayButton _button;

	// Token: 0x040062B7 RID: 25271
	private TextMeshProUGUI _buttonText;

	// Token: 0x040062B8 RID: 25272
	private string _friendName;

	// Token: 0x040062B9 RID: 25273
	private string _friendRoom;

	// Token: 0x040062BA RID: 25274
	private string _friendZone;

	// Token: 0x040062BB RID: 25275
	private FriendBackendController.Friend currentFriend;

	// Token: 0x040062BC RID: 25276
	private FriendDisplay friendDisplay;

	// Token: 0x040062BD RID: 25277
	private string[] randomNames = new string[]
	{
		"Veronica", "Roman", "Janiyah", "Dalton", "Bellamy", "Eithan", "Celeste", "Isaac", "Astrid", "Azariah",
		"Keilani", "Zeke", "Jayleen", "Yosef", "Jaylee", "Bodie", "Greta", "Cain", "Ella", "Everly",
		"Finnley", "Paisley", "Kaison", "Luna", "Nina", "Maison", "Monroe", "Ricardo", "Zariyah", "Travis",
		"Lacey", "Elian", "Frankie", "Otis", "Adele", "Edison", "Amira", "Ivan", "Raelynn", "Eliel",
		"Aliana", "Beckett", "Mylah", "Melvin", "Magdalena", "Leroy", "Madeleine"
	};

	// Token: 0x040062BE RID: 25278
	private FriendDisplay.ButtonState _buttonState = (FriendDisplay.ButtonState)(-1);

	// Token: 0x040062BF RID: 25279
	private Material[] _buttonDefaultMaterials;

	// Token: 0x040062C0 RID: 25280
	private Material[] _buttonActiveMaterials;

	// Token: 0x040062C1 RID: 25281
	private Material[] _buttonAlertMaterials;
}
