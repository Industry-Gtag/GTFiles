using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200087D RID: 2173
[Obsolete("This class is obsolete and will be removed in a future version. (MattO 2024-02-26) It doesn't appear to be used anywhere.")]
public class GorillaHatButton : MonoBehaviour
{
	// Token: 0x0600389C RID: 14492 RVA: 0x001344C4 File Offset: 0x001326C4
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			if (this.touchTime + this.debounceTime < Time.time)
			{
				this.touchTime = Time.time;
				this.isOn = !this.isOn;
				this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			}
		}
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x0013452C File Offset: 0x0013272C
	private void OnTriggerEnter(Collider collider)
	{
		if (this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			this.isOn = !this.isOn;
			this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x001345CC File Offset: 0x001327CC
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x04004893 RID: 18579
	public GorillaHatButtonParent buttonParent;

	// Token: 0x04004894 RID: 18580
	public GorillaHatButton.HatButtonType buttonType;

	// Token: 0x04004895 RID: 18581
	public bool isOn;

	// Token: 0x04004896 RID: 18582
	public Material offMaterial;

	// Token: 0x04004897 RID: 18583
	public Material onMaterial;

	// Token: 0x04004898 RID: 18584
	public string offText;

	// Token: 0x04004899 RID: 18585
	public string onText;

	// Token: 0x0400489A RID: 18586
	public Text myText;

	// Token: 0x0400489B RID: 18587
	public float debounceTime = 0.25f;

	// Token: 0x0400489C RID: 18588
	public float touchTime;

	// Token: 0x0400489D RID: 18589
	public string cosmeticName;

	// Token: 0x0400489E RID: 18590
	public bool testPress;

	// Token: 0x0200087E RID: 2174
	public enum HatButtonType
	{
		// Token: 0x040048A0 RID: 18592
		Hat,
		// Token: 0x040048A1 RID: 18593
		Face,
		// Token: 0x040048A2 RID: 18594
		Badge
	}
}
