using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200057C RID: 1404
public class CosmeticButton : GorillaPressableButton
{
	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x060023B1 RID: 9137 RVA: 0x000BFFAB File Offset: 0x000BE1AB
	// (set) Token: 0x060023B2 RID: 9138 RVA: 0x000BFFB3 File Offset: 0x000BE1B3
	public bool Initialized { get; private set; }

	// Token: 0x060023B3 RID: 9139 RVA: 0x000BFFBC File Offset: 0x000BE1BC
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
		this.Initialized = true;
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000BFFD8 File Offset: 0x000BE1D8
	public override void UpdateColor()
	{
		if (this.isSubscriberOnlyButton && !this._localPlayerSubscribed)
		{
			base.SetUnsubscribedMaterial();
			this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
			return;
		}
		if (!base.enabled)
		{
			this.buttonRenderer.material = this.disabledMaterial;
			this.SetOffText(this.myText != null, false, false);
		}
		else if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			this.SetOnText(this.myText.IsNotNull(), false, false);
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			this.SetOffText(this.myText != null, false, false);
		}
		this.UpdatePosition();
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000C00AC File Offset: 0x000BE2AC
	public virtual void UpdatePosition()
	{
		Vector3 vector = this.startingPos;
		if (!base.enabled)
		{
			vector += this.disabledOffset;
		}
		else if (this.isOn)
		{
			vector += this.pressedOffset;
		}
		this.posOffset = base.transform.position;
		base.transform.localPosition = vector;
		this.posOffset = base.transform.position - this.posOffset;
		if (this.myText != null)
		{
			this.myText.transform.position += this.posOffset;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.transform.position += this.posOffset;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.transform.position += this.posOffset;
		}
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000C01B2 File Offset: 0x000BE3B2
	protected override bool AllowNonSubscribedPress()
	{
		return !(CosmeticsController.instance == null) && !string.IsNullOrEmpty(this.SetCosmeticItemID) && CosmeticsController.instance.currentWornSet.HasItem(this.SetCosmeticItemID);
	}

	// Token: 0x04002EE3 RID: 12003
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04002EE4 RID: 12004
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x04002EE5 RID: 12005
	[SerializeField]
	private Vector3 disabledOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04002EE6 RID: 12006
	private Vector3 startingPos;

	// Token: 0x04002EE7 RID: 12007
	protected Vector3 posOffset;

	// Token: 0x04002EE8 RID: 12008
	public string SetCosmeticItemID;
}
