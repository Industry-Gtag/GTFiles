using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006FD RID: 1789
public class GamePressableButton : MonoBehaviour, IClickable
{
	// Token: 0x06002D31 RID: 11569 RVA: 0x000F40E0 File Offset: 0x000F22E0
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x000F40EC File Offset: 0x000F22EC
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = collider.gameObject.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (!component)
		{
			return;
		}
		if (!this.CheckValidEquippedState(component.isLeftHand))
		{
			return;
		}
		this.PressButton(component.isLeftHand);
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x000F4148 File Offset: 0x000F2348
	private bool CheckValidEquippedState(bool pressedHandLeft)
	{
		if (!this.requireEquipped)
		{
			return true;
		}
		int num = -1;
		GamePlayer gamePlayer;
		if (this.gameEntity.IsHeldByLocalPlayer() && this.activeWhileGrabbed && GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		GamePlayer gamePlayer2;
		if (num == -1 && this.gameEntity.IsSnappedByLocalPlayer() && this.activeWhileSnapped && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			num = gamePlayer2.FindSnapIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return false;
		}
		bool flag = GamePlayer.IsLeftHand(num);
		return pressedHandLeft != flag;
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x000F41F0 File Offset: 0x000F23F0
	private void PressButton(bool isLeftHand)
	{
		this.touchTime = Time.time;
		UnityEvent unityEvent = this.onPressButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, isLeftHand, 0.05f });
		}
	}

	// Token: 0x0400399E RID: 14750
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x0400399F RID: 14751
	[SerializeField]
	private bool requireEquipped;

	// Token: 0x040039A0 RID: 14752
	[SerializeField]
	private bool activeWhileGrabbed;

	// Token: 0x040039A1 RID: 14753
	[SerializeField]
	private bool activeWhileSnapped;

	// Token: 0x040039A2 RID: 14754
	public UnityEvent onPressButton;

	// Token: 0x040039A3 RID: 14755
	[Header("Button Press")]
	public float debounceTime = 0.25f;

	// Token: 0x040039A4 RID: 14756
	public int pressButtonSoundIndex = 67;

	// Token: 0x040039A5 RID: 14757
	private float touchTime;
}
