using System;
using TMPro;
using UnityEngine;

// Token: 0x02000BBC RID: 3004
public class KIDUI_AgeAppealEmailError : MonoBehaviour
{
	// Token: 0x06004BDA RID: 19418 RVA: 0x001946B6 File Offset: 0x001928B6
	public void ShowAgeAppealEmailErrorScreen(bool hasChallenge, int newAge, string email)
	{
		this.hasChallenge = hasChallenge;
		this.newAge = newAge;
		this._emailText.text = email;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004BDB RID: 19419 RVA: 0x001946DE File Offset: 0x001928DE
	public void onBackPressed()
	{
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAge);
	}

	// Token: 0x04005EB6 RID: 24246
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04005EB7 RID: 24247
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x04005EB8 RID: 24248
	private bool hasChallenge;

	// Token: 0x04005EB9 RID: 24249
	private int newAge;
}
