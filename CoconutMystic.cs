using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x02000523 RID: 1315
public class CoconutMystic : MonoBehaviour
{
	// Token: 0x060020EE RID: 8430 RVA: 0x000B0978 File Offset: 0x000AEB78
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x000B0986 File Offset: 0x000AEB86
	private void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += this.OnPhotonEvent;
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x000B099E File Offset: 0x000AEB9E
	private void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= this.OnPhotonEvent;
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x000B09B8 File Offset: 0x000AEBB8
	private void OnPhotonEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		object[] array = (object[])evData.CustomData;
		object obj = array[0];
		if (!(obj is int))
		{
			return;
		}
		int num = (int)obj;
		if (num != CoconutMystic.kUpdateLabelEvent)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(evData.Sender);
		NetPlayer owningNetPlayer = this.rig.OwningNetPlayer;
		if (player != owningNetPlayer)
		{
			return;
		}
		int num2 = (int)array[1];
		this.label.text = this.answers.GetItem(num2).GetLocalizedString();
		this.soundPlayer.Play();
		this.breakEffect.Play();
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x000B0A5C File Offset: 0x000AEC5C
	public void UpdateLabel()
	{
		bool flag = this.geodeItem.currentState == TransferrableObject.PositionState.InLeftHand;
		this.label.rectTransform.localRotation = Quaternion.Euler(0f, flag ? 270f : 90f, 0f);
	}

	// Token: 0x060020F3 RID: 8435 RVA: 0x000B0AA8 File Offset: 0x000AECA8
	public void ShowAnswer()
	{
		this.answers.distinct = this.distinct;
		this.label.text = this.answers.NextItem().GetLocalizedString();
		this.soundPlayer.Play();
		this.breakEffect.Play();
		object obj = new object[]
		{
			CoconutMystic.kUpdateLabelEvent,
			this.answers.lastItemIndex
		};
		PhotonNetwork.RaiseEvent(176, obj, RaiseEventOptions.Default, SendOptions.SendReliable);
	}

	// Token: 0x04002BAA RID: 11178
	public VRRig rig;

	// Token: 0x04002BAB RID: 11179
	public GeodeItem geodeItem;

	// Token: 0x04002BAC RID: 11180
	public SoundBankPlayer soundPlayer;

	// Token: 0x04002BAD RID: 11181
	public ParticleSystem breakEffect;

	// Token: 0x04002BAE RID: 11182
	public RandomLocalizedStrings answers;

	// Token: 0x04002BAF RID: 11183
	public TMP_Text label;

	// Token: 0x04002BB0 RID: 11184
	public bool distinct;

	// Token: 0x04002BB1 RID: 11185
	private static readonly int kUpdateLabelEvent = "CoconutMystic.UpdateLabel".GetStaticHash();
}
