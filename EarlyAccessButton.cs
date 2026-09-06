using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000513 RID: 1299
[Obsolete("Replaced with bundlebutton")]
public class EarlyAccessButton : GorillaPressableButton
{
	// Token: 0x0600206F RID: 8303 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000AE7A4 File Offset: 0x000AC9A4
	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000AE7FE File Offset: 0x000AC9FE
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressEarlyAccessButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x000AE81F File Offset: 0x000ACA1F
	public void AlreadyOwn()
	{
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";
	}

	// Token: 0x06002073 RID: 8307 RVA: 0x000AE855 File Offset: 0x000ACA55
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}
}
