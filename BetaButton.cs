using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000502 RID: 1282
public class BetaButton : GorillaPressableButton
{
	// Token: 0x06002032 RID: 8242 RVA: 0x000AD650 File Offset: 0x000AB850
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.count++;
		base.StartCoroutine(this.ButtonColorUpdate());
		if (this.count >= 10)
		{
			this.betaParent.SetActive(false);
			PlayerPrefs.SetString("CheckedBox2", "true");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x000AD6A8 File Offset: 0x000AB8A8
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002B03 RID: 11011
	public GameObject betaParent;

	// Token: 0x04002B04 RID: 11012
	public int count;

	// Token: 0x04002B05 RID: 11013
	public float buttonFadeTime = 0.25f;

	// Token: 0x04002B06 RID: 11014
	public Text messageText;
}
