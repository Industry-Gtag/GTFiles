using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000CBC RID: 3260
public class FriendDisplay : MonoBehaviour
{
	// Token: 0x170007AA RID: 1962
	// (get) Token: 0x060050B3 RID: 20659 RVA: 0x001AB7AD File Offset: 0x001A99AD
	// (set) Token: 0x060050B4 RID: 20660 RVA: 0x001AB7B4 File Offset: 0x001A99B4
	public static int ConfiguredVimPageCount { get; private set; } = 0;

	// Token: 0x170007AB RID: 1963
	// (get) Token: 0x060050B5 RID: 20661 RVA: 0x001AB7BC File Offset: 0x001A99BC
	// (set) Token: 0x060050B6 RID: 20662 RVA: 0x001AB7C3 File Offset: 0x001A99C3
	public static int ConfiguredFreeExtraPageCount { get; private set; } = 1;

	// Token: 0x170007AC RID: 1964
	// (get) Token: 0x060050B7 RID: 20663 RVA: 0x001AB7CB File Offset: 0x001A99CB
	private int totalPages
	{
		get
		{
			return 1 + this.freeExtraPageCount + this.vimPageCount;
		}
	}

	// Token: 0x170007AD RID: 1965
	// (get) Token: 0x060050B8 RID: 20664 RVA: 0x001AB7DC File Offset: 0x001A99DC
	public int TotalCapacity
	{
		get
		{
			return 9 + (this.freeExtraPageCount + this.vimPageCount) * 9;
		}
	}

	// Token: 0x170007AE RID: 1966
	// (get) Token: 0x060050B9 RID: 20665 RVA: 0x001AB7F1 File Offset: 0x001A99F1
	public int VIMTotalCapacity
	{
		get
		{
			return this.vimPageCount * 9;
		}
	}

	// Token: 0x170007AF RID: 1967
	// (get) Token: 0x060050BA RID: 20666 RVA: 0x001AB7FC File Offset: 0x001A99FC
	public int FreeExtraTotalCapacity
	{
		get
		{
			return this.freeExtraPageCount * 9;
		}
	}

	// Token: 0x170007B0 RID: 1968
	// (get) Token: 0x060050BB RID: 20667 RVA: 0x001AB807 File Offset: 0x001A9A07
	public int VimPageCount
	{
		get
		{
			return this.vimPageCount;
		}
	}

	// Token: 0x170007B1 RID: 1969
	// (get) Token: 0x060050BC RID: 20668 RVA: 0x001AB80F File Offset: 0x001A9A0F
	public bool InRemoveMode
	{
		get
		{
			return this.inRemoveMode;
		}
	}

	// Token: 0x060050BD RID: 20669 RVA: 0x001AB817 File Offset: 0x001A9A17
	private void Awake()
	{
		FriendDisplay.ConfiguredVimPageCount = this.vimPageCount;
		FriendDisplay.ConfiguredFreeExtraPageCount = this.freeExtraPageCount;
	}

	// Token: 0x060050BE RID: 20670 RVA: 0x001AB830 File Offset: 0x001A9A30
	private void Start()
	{
		this.InitFriendCards();
		this.InitLocalPlayerCard();
		this.UpdateLocalPlayerPrivacyButtons();
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
		NetworkSystem.Instance.OnJoinedRoomEvent += this.OnJoinedRoom;
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.OnLocalSubscriptionChanged));
	}

	// Token: 0x060050BF RID: 20671 RVA: 0x001AB8C0 File Offset: 0x001A9AC0
	private void OnDestroy()
	{
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnJoinedRoomEvent -= this.OnJoinedRoom;
		}
		if (this.triggerNotifier != null)
		{
			this.triggerNotifier.TriggerEnterEvent -= this.TriggerEntered;
			this.triggerNotifier.TriggerExitEvent -= this.TriggerExited;
		}
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.OnLocalSubscriptionChanged));
	}

	// Token: 0x060050C0 RID: 20672 RVA: 0x001AB957 File Offset: 0x001A9B57
	private void OnLocalSubscriptionChanged()
	{
		if (!this.localPlayerAtDisplay)
		{
			return;
		}
		this.GoToFriendPage(this._currentPage);
	}

	// Token: 0x060050C1 RID: 20673 RVA: 0x001AB970 File Offset: 0x001A9B70
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh += this.OnGetFriendsReceived;
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
			this.localPlayerAtDisplay = true;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x060050C2 RID: 20674 RVA: 0x001AB9D0 File Offset: 0x001A9BD0
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh -= this.OnGetFriendsReceived;
			this.ClearFriendCards();
			this.ClearLocalPlayerCard();
			this.ClearPageButtons();
			this.localPlayerAtDisplay = false;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x060050C3 RID: 20675 RVA: 0x001ABA2E File Offset: 0x001A9C2E
	private void OnJoinedRoom()
	{
		this.Refresh();
	}

	// Token: 0x060050C4 RID: 20676 RVA: 0x001ABA36 File Offset: 0x001A9C36
	private void Refresh()
	{
		if (this.localPlayerAtDisplay)
		{
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
		}
	}

	// Token: 0x060050C5 RID: 20677 RVA: 0x001ABA52 File Offset: 0x001A9C52
	public void LocalPlayerFullyVisiblePress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Visible);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x060050C6 RID: 20678 RVA: 0x001ABA6D File Offset: 0x001A9C6D
	public void LocalPlayerPublicOnlyPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.PublicOnly);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x060050C7 RID: 20679 RVA: 0x001ABA88 File Offset: 0x001A9C88
	public void LocalPlayerFullyHiddenPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Hidden);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x060050C8 RID: 20680 RVA: 0x001ABAA4 File Offset: 0x001A9CA4
	private void UpdateLocalPlayerPrivacyButtons()
	{
		FriendSystem.PlayerPrivacy localPlayerPrivacy = FriendSystem.Instance.LocalPlayerPrivacy;
		this.SetButtonAppearance(this._localPlayerFullyVisibleButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Visible);
		this.SetButtonAppearance(this._localPlayerPublicOnlyButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly);
		this.SetButtonAppearance(this._localPlayerFullyHiddenButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden);
	}

	// Token: 0x060050C9 RID: 20681 RVA: 0x001ABAF0 File Offset: 0x001A9CF0
	private void UpdatePageButtons(int selectedPage)
	{
		int count = FriendBackendController.Instance.FriendsList.Count;
		bool flag = SubscriptionManager.IsLocalSubscribed();
		int num = 1 + this.freeExtraPageCount;
		int num2 = 9 + this.freeExtraPageCount * 9;
		bool flag2 = this.freeExtraPageCount > 0;
		int num3 = Mathf.Min(this.totalPages, this.PageButtons.Length);
		if (!flag2)
		{
			for (int i = num; i < num3; i++)
			{
				int num4 = i - num;
				bool flag3 = count > num2 + num4 * 9;
				if (flag || flag3)
				{
					flag2 = true;
					break;
				}
			}
		}
		for (int j = 0; j < this.PageButtons.Length; j++)
		{
			bool flag4;
			if (j >= num3)
			{
				flag4 = false;
			}
			else if (j == 0)
			{
				flag4 = flag2;
			}
			else if (j < num)
			{
				flag4 = true;
			}
			else
			{
				int num5 = j - num;
				bool flag5 = count > num2 + num5 * 9;
				flag4 = flag || flag5;
			}
			if (flag4)
			{
				this.SetPageButtonAppearance(this.PageButtons[j], (j == selectedPage) ? FriendDisplay.ButtonState.Active : FriendDisplay.ButtonState.Default);
			}
			else
			{
				this.HidePageButton(this.PageButtons[j]);
			}
		}
	}

	// Token: 0x060050CA RID: 20682 RVA: 0x001ABBF8 File Offset: 0x001A9DF8
	private void SetButtonAppearance(MeshRenderer buttonRenderer, bool active)
	{
		this.SetButtonAppearance(buttonRenderer, active ? FriendDisplay.ButtonState.Active : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060050CB RID: 20683 RVA: 0x001ABC08 File Offset: 0x001A9E08
	private void SetButtonAppearance(MeshRenderer buttonRenderer, FriendDisplay.ButtonState state)
	{
		Material[] array;
		switch (state)
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
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.sharedMaterials = array;
	}

	// Token: 0x060050CC RID: 20684 RVA: 0x001ABC60 File Offset: 0x001A9E60
	private void ClearPageButtons()
	{
		for (int i = 0; i < this.PageButtons.Length; i++)
		{
			this.HidePageButton(this.PageButtons[i]);
		}
	}

	// Token: 0x060050CD RID: 20685 RVA: 0x001ABC90 File Offset: 0x001A9E90
	private void HidePageButton(MeshRenderer buttonRenderer)
	{
		buttonRenderer.enabled = false;
		buttonRenderer.GetComponent<BoxCollider>().enabled = false;
		buttonRenderer.transform.localPosition = new Vector3(buttonRenderer.transform.localPosition.x, buttonRenderer.transform.localPosition.y, this.pageButtonInactiveZPos);
	}

	// Token: 0x060050CE RID: 20686 RVA: 0x001ABCE8 File Offset: 0x001A9EE8
	private void SetPageButtonAppearance(MeshRenderer buttonRenderer, FriendDisplay.ButtonState state)
	{
		buttonRenderer.enabled = true;
		buttonRenderer.GetComponent<BoxCollider>().enabled = true;
		Material[] array;
		switch (state)
		{
		case FriendDisplay.ButtonState.Default:
			array = this._pageButtonDefaultMaterials;
			break;
		case FriendDisplay.ButtonState.Active:
			array = this._pageButtonActiveMaterials;
			break;
		case FriendDisplay.ButtonState.Alert:
			array = this._pageButtonAlerttMaterials;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.sharedMaterials = array;
		Vector3 localPosition = buttonRenderer.transform.localPosition;
		buttonRenderer.transform.localPosition = new Vector3(localPosition.x, localPosition.y, this.pageButtonActiveZPos);
	}

	// Token: 0x060050CF RID: 20687 RVA: 0x001ABD80 File Offset: 0x001A9F80
	public void ToggleRemoveFriendMode()
	{
		this.inRemoveMode = !this.inRemoveMode;
		FriendCard[] array = this.friendCards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetRemoveEnabled(this.inRemoveMode);
		}
		this.SetButtonAppearance(this._removeFriendButton, this.inRemoveMode ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060050D0 RID: 20688 RVA: 0x001ABDD8 File Offset: 0x001A9FD8
	private void InitFriendCards()
	{
		float num = this.gridWidth / (float)this.gridDimension;
		float num2 = this.gridHeight / (float)this.gridDimension;
		Vector3 right = this.gridRoot.right;
		Vector3 vector = -this.gridRoot.up;
		Vector3 vector2 = this.gridRoot.position - right * (this.gridWidth * 0.5f - num * 0.5f) - vector * (this.gridHeight * 0.5f - num2 * 0.5f);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < this.gridDimension; i++)
		{
			for (int j = 0; j < this.gridDimension; j++)
			{
				FriendCard friendCard = this.friendCards[num4];
				friendCard.gameObject.SetActive(true);
				friendCard.transform.localScale = Vector3.one * (num / friendCard.Width);
				friendCard.transform.position = vector2 + right * num * (float)j + vector * num2 * (float)i;
				friendCard.transform.rotation = this.gridRoot.transform.rotation;
				friendCard.Init(this);
				friendCard.SetButton(this._friendCardButtons[num3++], this._buttonDefaultMaterials, this._buttonActiveMaterials, this._buttonAlertMaterials, this._friendCardButtonText[num4]);
				friendCard.SetEmpty();
				num4++;
			}
		}
	}

	// Token: 0x060050D1 RID: 20689 RVA: 0x001ABF78 File Offset: 0x001AA178
	public void RandomizeFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].Randomize();
		}
	}

	// Token: 0x060050D2 RID: 20690 RVA: 0x001ABFA8 File Offset: 0x001AA1A8
	private void ClearFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].SetEmpty();
		}
	}

	// Token: 0x060050D3 RID: 20691 RVA: 0x001ABFD5 File Offset: 0x001AA1D5
	public void OnGetFriendsReceived(List<FriendBackendController.Friend> friendsList)
	{
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
		this.GoToFriendPage(this._currentPage);
	}

	// Token: 0x060050D4 RID: 20692 RVA: 0x001ABFF0 File Offset: 0x001AA1F0
	public void GoToFriendPage(int currentPage)
	{
		int num = Mathf.Min(this.totalPages, this.PageButtons.Length);
		if (currentPage < 0 || currentPage >= num)
		{
			currentPage = 0;
		}
		this._currentPage = currentPage;
		this.UpdatePageButtons(currentPage);
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].SetEmpty();
		}
		List<FriendBackendController.Friend> friendsList = FriendBackendController.Instance.FriendsList;
		int num2 = currentPage * this.cardsPerPage;
		int num3 = 9 + this.freeExtraPageCount * 9;
		for (int j = 0; j < this.friendCards.Length; j++)
		{
			int num4 = num2 + j;
			bool flag = num4 >= num3;
			if (num4 < friendsList.Count)
			{
				this.friendCards[j].Populate(friendsList[num4], flag);
			}
			else
			{
				this.friendCards[j].SetEmpty(flag);
			}
		}
	}

	// Token: 0x060050D5 RID: 20693 RVA: 0x001AC0CD File Offset: 0x001AA2CD
	private void InitLocalPlayerCard()
	{
		this._localPlayerCard.Init(this);
		this.ClearLocalPlayerCard();
	}

	// Token: 0x060050D6 RID: 20694 RVA: 0x001AC0E4 File Offset: 0x001AA2E4
	private void PopulateLocalPlayerCard()
	{
		string text = PhotonNetworkController.Instance.CurrentRoomZone.GetName<GTZone>().ToUpper();
		this._localPlayerCard.SetName(NetworkSystem.Instance.LocalPlayer.NickName.ToUpper());
		if (!PhotonNetwork.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.RoomName) || NetworkSystem.Instance.RoomName.Length <= 0)
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		bool flag = GorillaComputer.instance != null && GorillaComputer.instance.IsVStumpRoomName(NetworkSystem.Instance.RoomName);
		bool flag2 = !NetworkSystem.Instance.SessionIsPrivate;
		if (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden || (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly && !flag2))
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		if (flag)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.Substring(1).ToUpper());
			this._localPlayerCard.SetZone("CUSTOM");
			return;
		}
		if (!flag2)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
			this._localPlayerCard.SetZone("PRIVATE");
			return;
		}
		this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
		this._localPlayerCard.SetZone(text);
	}

	// Token: 0x060050D7 RID: 20695 RVA: 0x001AC285 File Offset: 0x001AA485
	private void ClearLocalPlayerCard()
	{
		this._localPlayerCard.SetEmpty();
	}

	// Token: 0x060050D8 RID: 20696 RVA: 0x001AC294 File Offset: 0x001AA494
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		float num = this.gridWidth * 0.5f;
		float num2 = this.gridHeight * 0.5f;
		float num3 = num;
		float num4 = num2;
		Vector3 vector = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector2 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector3 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 vector4 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, -num4, 0f);
		for (int i = 0; i <= this.gridDimension; i++)
		{
			float num5 = (float)i / (float)this.gridDimension;
			Vector3 vector5 = Vector3.Lerp(vector, vector2, num5);
			Vector3 vector6 = Vector3.Lerp(vector3, vector4, num5);
			Gizmos.DrawLine(vector5, vector6);
			Vector3 vector7 = Vector3.Lerp(vector, vector3, num5);
			Vector3 vector8 = Vector3.Lerp(vector2, vector4, num5);
			Gizmos.DrawLine(vector7, vector8);
		}
	}

	// Token: 0x040062C2 RID: 25282
	[FormerlySerializedAs("gridCenter")]
	[SerializeField]
	private FriendCard[] friendCards = new FriendCard[9];

	// Token: 0x040062C3 RID: 25283
	[SerializeField]
	private Transform gridRoot;

	// Token: 0x040062C4 RID: 25284
	[SerializeField]
	private float gridWidth = 2f;

	// Token: 0x040062C5 RID: 25285
	[SerializeField]
	private float gridHeight = 1f;

	// Token: 0x040062C6 RID: 25286
	[SerializeField]
	private int gridDimension = 3;

	// Token: 0x040062C7 RID: 25287
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x040062C8 RID: 25288
	[FormerlySerializedAs("_joinButtons")]
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableDelayButton[] _friendCardButtons;

	// Token: 0x040062C9 RID: 25289
	[SerializeField]
	private TextMeshProUGUI[] _friendCardButtonText;

	// Token: 0x040062CA RID: 25290
	[SerializeField]
	private MeshRenderer _localPlayerFullyVisibleButton;

	// Token: 0x040062CB RID: 25291
	[SerializeField]
	private MeshRenderer _localPlayerPublicOnlyButton;

	// Token: 0x040062CC RID: 25292
	[SerializeField]
	private MeshRenderer _localPlayerFullyHiddenButton;

	// Token: 0x040062CD RID: 25293
	[SerializeField]
	private MeshRenderer _removeFriendButton;

	// Token: 0x040062CE RID: 25294
	[SerializeField]
	private FriendCard _localPlayerCard;

	// Token: 0x040062CF RID: 25295
	[SerializeField]
	private MeshRenderer[] PageButtons;

	// Token: 0x040062D0 RID: 25296
	[SerializeField]
	private Material[] _buttonDefaultMaterials;

	// Token: 0x040062D1 RID: 25297
	[SerializeField]
	private Material[] _buttonActiveMaterials;

	// Token: 0x040062D2 RID: 25298
	[SerializeField]
	private Material[] _buttonAlertMaterials;

	// Token: 0x040062D3 RID: 25299
	[SerializeField]
	private Material[] _pageButtonDefaultMaterials;

	// Token: 0x040062D4 RID: 25300
	[SerializeField]
	private Material[] _pageButtonActiveMaterials;

	// Token: 0x040062D5 RID: 25301
	[SerializeField]
	private Material[] _pageButtonAlerttMaterials;

	// Token: 0x040062D6 RID: 25302
	public const int PageCapacity = 9;

	// Token: 0x040062D7 RID: 25303
	public const int VIMPageCapacity = 9;

	// Token: 0x040062DA RID: 25306
	[SerializeField]
	private int freeExtraPageCount = 1;

	// Token: 0x040062DB RID: 25307
	[SerializeField]
	private int vimPageCount;

	// Token: 0x040062DC RID: 25308
	private int cardsPerPage = 9;

	// Token: 0x040062DD RID: 25309
	[SerializeField]
	private float pageButtonInactiveZPos;

	// Token: 0x040062DE RID: 25310
	[SerializeField]
	private float pageButtonActiveZPos;

	// Token: 0x040062DF RID: 25311
	private MeshRenderer[] _joinButtonRenderers;

	// Token: 0x040062E0 RID: 25312
	private bool inRemoveMode;

	// Token: 0x040062E1 RID: 25313
	private bool localPlayerAtDisplay;

	// Token: 0x040062E2 RID: 25314
	private int _currentPage;

	// Token: 0x02000CBD RID: 3261
	public enum ButtonState
	{
		// Token: 0x040062E4 RID: 25316
		Default,
		// Token: 0x040062E5 RID: 25317
		Active,
		// Token: 0x040062E6 RID: 25318
		Alert
	}
}
