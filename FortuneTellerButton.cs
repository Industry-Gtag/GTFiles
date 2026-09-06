using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020006AB RID: 1707
public class FortuneTellerButton : GorillaPressableButton
{
	// Token: 0x06002A8F RID: 10895 RVA: 0x000E57AD File Offset: 0x000E39AD
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x000E57C0 File Offset: 0x000E39C0
	public override void ButtonActivation()
	{
		this.PressButtonUpdate();
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x000E57C8 File Offset: 0x000E39C8
	public void PressButtonUpdate()
	{
		if (this.pressTime != 0f)
		{
			return;
		}
		base.transform.localPosition = this.startingPos + this.pressedOffset;
		this.buttonRenderer.material = this.pressedMaterial;
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonUpdate>g__ButtonColorUpdate_Local|6_0());
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x000E5855 File Offset: 0x000E3A55
	[CompilerGenerated]
	private IEnumerator <PressButtonUpdate>g__ButtonColorUpdate_Local|6_0()
	{
		yield return new WaitForSeconds(this.durationPressed);
		if (this.pressTime != 0f && Time.time > this.durationPressed + this.pressTime)
		{
			base.transform.localPosition = this.startingPos;
			this.buttonRenderer.material = this.unpressedMaterial;
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x04003765 RID: 14181
	[SerializeField]
	private float durationPressed = 0.25f;

	// Token: 0x04003766 RID: 14182
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04003767 RID: 14183
	private float pressTime;

	// Token: 0x04003768 RID: 14184
	private Vector3 startingPos;
}
