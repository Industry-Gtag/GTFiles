using System;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class GameModeSelectorJoinSubsButton : MonoBehaviour
{
	// Token: 0x06000412 RID: 1042 RVA: 0x000185B4 File Offset: 0x000167B4
	private void OnEnable()
	{
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscribed));
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeaveRoom);
		this.CheckSubscribed();
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00018620 File Offset: 0x00016820
	private void OnDisable()
	{
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscribed));
		RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeaveRoom);
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00018683 File Offset: 0x00016883
	[ContextMenu("Check Subscribed")]
	private void CheckSubscribed()
	{
		if (!SubscriptionManager.IsLocalSubscribed())
		{
			this.DisableButtonSubscribers();
			return;
		}
		if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.MaxPlayers <= 10)
		{
			this.ShowButton();
			return;
		}
		this.DisableButtonInPublicRoom();
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x000186B5 File Offset: 0x000168B5
	private void OnJoinRoom()
	{
		if (!RoomSystem.WasRoomPrivate)
		{
			this.CheckSubscribed();
			return;
		}
		this.DisableButtonPrivate();
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x000186CB File Offset: 0x000168CB
	private void OnLeaveRoom()
	{
		this.CheckSubscribed();
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x000186D3 File Offset: 0x000168D3
	private void ShowButton()
	{
		this.subsPublicButton.enabled = true;
		this.subsPublicButton.SetUnpressedMaterial();
		this.disabledObject.SetActive(false);
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x000186F8 File Offset: 0x000168F8
	private void DisableButtonSubscribers()
	{
		this.DisableButton("ONLY FOR SUBSCRIBERS");
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00018705 File Offset: 0x00016905
	private void DisableButtonPrivate()
	{
		this.DisableButton("IN PRIVATE ROOM");
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00018712 File Offset: 0x00016912
	private void DisableButtonInPublicRoom()
	{
		this.DisableButton("ALREADY IN PUBLIC ROOM");
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x0001871F File Offset: 0x0001691F
	private void DisableButton(string disabled)
	{
		this.subsPublicButton.enabled = false;
		this.subsPublicButton.SetRendererMaterial(this.DisabledButtonMaterial);
		this.disabledObject.SetActive(true);
		this.disabledText.text = disabled;
	}

	// Token: 0x04000482 RID: 1154
	public Material DisabledButtonMaterial;

	// Token: 0x04000483 RID: 1155
	[SerializeField]
	private GorillaPressableButton subsPublicButton;

	// Token: 0x04000484 RID: 1156
	[SerializeField]
	private GameObject disabledObject;

	// Token: 0x04000485 RID: 1157
	[SerializeField]
	private TextMeshPro disabledText;
}
