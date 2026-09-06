using System;
using UnityEngine;

// Token: 0x0200087F RID: 2175
public class GorillaHatButtonParent : MonoBehaviour
{
	// Token: 0x060038A0 RID: 14496 RVA: 0x0013463C File Offset: 0x0013283C
	public void Start()
	{
		this.hat = PlayerPrefs.GetString("hatCosmetic", "none");
		this.face = PlayerPrefs.GetString("faceCosmetic", "none");
		this.badge = PlayerPrefs.GetString("badgeCosmetic", "none");
		this.leftHandHold = PlayerPrefs.GetString("leftHandHoldCosmetic", "none");
		this.rightHandHold = PlayerPrefs.GetString("rightHandHoldCosmetic", "none");
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x001346B4 File Offset: 0x001328B4
	public void LateUpdate()
	{
		if (!this.initialized && GorillaTagger.Instance.offlineVRRig.InitializedCosmetics)
		{
			this.initialized = true;
			if (GorillaTagger.Instance.offlineVRRig.HasCosmetic("AdministratorBadge"))
			{
				foreach (GameObject gameObject in this.adminObjects)
				{
					Debug.Log("doing this?");
					gameObject.SetActive(true);
				}
			}
			if (GorillaTagger.Instance.offlineVRRig.HasCosmetic("earlyaccess"))
			{
				this.UpdateButtonState();
				this.screen.UpdateText("WELCOME TO THE HAT ROOM!\nTHANK YOU FOR PURCHASING THE EARLY ACCESS SUPPORTER PACK! PLEASE ENJOY THESE VARIOUS HATS AND NOT-HATS!", true);
			}
		}
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x00134750 File Offset: 0x00132950
	public void PressButton(bool isOn, GorillaHatButton.HatButtonType buttonType, string buttonValue)
	{
		if (this.initialized && GorillaTagger.Instance.offlineVRRig.HasCosmetic("earlyaccess"))
		{
			switch (buttonType)
			{
			case GorillaHatButton.HatButtonType.Hat:
				if (this.hat != buttonValue)
				{
					this.hat = buttonValue;
					PlayerPrefs.SetString("hatCosmetic", buttonValue);
				}
				else
				{
					this.hat = "none";
					PlayerPrefs.SetString("hatCosmetic", "none");
				}
				break;
			case GorillaHatButton.HatButtonType.Face:
				if (this.face != buttonValue)
				{
					this.face = buttonValue;
					PlayerPrefs.SetString("faceCosmetic", buttonValue);
				}
				else
				{
					this.face = "none";
					PlayerPrefs.SetString("faceCosmetic", "none");
				}
				break;
			case GorillaHatButton.HatButtonType.Badge:
				if (this.badge != buttonValue)
				{
					this.badge = buttonValue;
					PlayerPrefs.SetString("badgeCosmetic", buttonValue);
				}
				else
				{
					this.badge = "none";
					PlayerPrefs.SetString("badgeCosmetic", "none");
				}
				break;
			}
			PlayerPrefs.Save();
			this.UpdateButtonState();
		}
	}

	// Token: 0x060038A3 RID: 14499 RVA: 0x00134860 File Offset: 0x00132A60
	private void UpdateButtonState()
	{
		foreach (GorillaHatButton gorillaHatButton in this.hatButtons)
		{
			switch (gorillaHatButton.buttonType)
			{
			case GorillaHatButton.HatButtonType.Hat:
				gorillaHatButton.isOn = gorillaHatButton.cosmeticName == this.hat;
				break;
			case GorillaHatButton.HatButtonType.Face:
				gorillaHatButton.isOn = gorillaHatButton.cosmeticName == this.face;
				break;
			case GorillaHatButton.HatButtonType.Badge:
				gorillaHatButton.isOn = gorillaHatButton.cosmeticName == this.badge;
				break;
			}
			gorillaHatButton.UpdateColor();
		}
	}

	// Token: 0x040048A3 RID: 18595
	public GorillaHatButton[] hatButtons;

	// Token: 0x040048A4 RID: 18596
	public GameObject[] adminObjects;

	// Token: 0x040048A5 RID: 18597
	public string hat;

	// Token: 0x040048A6 RID: 18598
	public string face;

	// Token: 0x040048A7 RID: 18599
	public string badge;

	// Token: 0x040048A8 RID: 18600
	public string leftHandHold;

	// Token: 0x040048A9 RID: 18601
	public string rightHandHold;

	// Token: 0x040048AA RID: 18602
	public bool initialized;

	// Token: 0x040048AB RID: 18603
	public GorillaLevelScreen screen;
}
