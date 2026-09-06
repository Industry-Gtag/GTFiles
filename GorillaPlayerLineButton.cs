using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000A2E RID: 2606
public class GorillaPlayerLineButton : MonoBehaviour, IClickable
{
	// Token: 0x060042C8 RID: 17096 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x060042C9 RID: 17097 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnDisable()
	{
	}

	// Token: 0x060042CA RID: 17098 RVA: 0x00163A94 File Offset: 0x00161C94
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
				{
					this.isOn = !this.isOn;
				}
				this.parentLine.PressButton(this.isOn, this.buttonType);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x060042CB RID: 17099 RVA: 0x00163AA4 File Offset: 0x00161CA4
	private void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled || this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component == null)
		{
			return;
		}
		this.SetTouchTime(0f);
		this.Click(component.isLeftHand);
	}

	// Token: 0x060042CC RID: 17100 RVA: 0x00163AF6 File Offset: 0x00161CF6
	public void SetTouchTime(float add = 0f)
	{
		this.touchTime = Time.time + add;
	}

	// Token: 0x060042CD RID: 17101 RVA: 0x00163B08 File Offset: 0x00161D08
	public void Click(bool leftHand = false)
	{
		if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
		{
			if (this.isAutoOn)
			{
				this.isOn = false;
			}
			else
			{
				this.isOn = !this.isOn;
			}
		}
		if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute || this.buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech || this.buttonType == GorillaPlayerLineButton.ButtonType.Cheating || this.buttonType == GorillaPlayerLineButton.ButtonType.Cancel || this.parentLine.canPressNextReportButton)
		{
			this.parentLine.PressButton(this.isOn, this.buttonType);
			GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, leftHand, 0.05f);
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, leftHand, 0.05f });
			}
		}
	}

	// Token: 0x060042CE RID: 17102 RVA: 0x00163C18 File Offset: 0x00161E18
	private void OnTriggerExit(Collider other)
	{
		if (this.buttonType != GorillaPlayerLineButton.ButtonType.Mute && other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.parentLine.canPressNextReportButton = true;
		}
	}

	// Token: 0x060042CF RID: 17103 RVA: 0x00163C40 File Offset: 0x00161E40
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		if (this.isAutoOn)
		{
			base.GetComponent<MeshRenderer>().material = this.autoOnMaterial;
			this.myText.text = this.autoOnText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x0400548A RID: 21642
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x0400548B RID: 21643
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x0400548C RID: 21644
	public bool isOn;

	// Token: 0x0400548D RID: 21645
	public bool isAutoOn;

	// Token: 0x0400548E RID: 21646
	public Material offMaterial;

	// Token: 0x0400548F RID: 21647
	public Material onMaterial;

	// Token: 0x04005490 RID: 21648
	public Material autoOnMaterial;

	// Token: 0x04005491 RID: 21649
	public string offText;

	// Token: 0x04005492 RID: 21650
	public string onText;

	// Token: 0x04005493 RID: 21651
	public string autoOnText;

	// Token: 0x04005494 RID: 21652
	public Text myText;

	// Token: 0x04005495 RID: 21653
	public float debounceTime = 0.25f;

	// Token: 0x04005496 RID: 21654
	public float touchTime;

	// Token: 0x04005497 RID: 21655
	public bool testPress;

	// Token: 0x02000A2F RID: 2607
	public enum ButtonType
	{
		// Token: 0x04005499 RID: 21657
		HateSpeech,
		// Token: 0x0400549A RID: 21658
		Cheating,
		// Token: 0x0400549B RID: 21659
		Toxicity,
		// Token: 0x0400549C RID: 21660
		Mute,
		// Token: 0x0400549D RID: 21661
		Report,
		// Token: 0x0400549E RID: 21662
		Cancel,
		// Token: 0x0400549F RID: 21663
		MuteAllRoom,
		// Token: 0x040054A0 RID: 21664
		KickRoom,
		// Token: 0x040054A1 RID: 21665
		BanRoom,
		// Token: 0x040054A2 RID: 21666
		Confirm
	}
}
