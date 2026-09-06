using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200095E RID: 2398
public class ProgressBar : MonoBehaviour
{
	// Token: 0x06003F01 RID: 16129 RVA: 0x00152EB8 File Offset: 0x001510B8
	public void UpdateProgress(float newFill)
	{
		bool flag = newFill > 1f;
		this._fillAmount = Mathf.Clamp(newFill, 0f, 1f);
		this.fillImage.fillAmount = this._fillAmount;
		if (this.useColors)
		{
			if (flag)
			{
				this.fillImage.color = this.overCapacity;
				return;
			}
			if (Mathf.Approximately(this._fillAmount, 1f))
			{
				this.fillImage.color = this.atCapacity;
				return;
			}
			this.fillImage.color = this.underCapacity;
		}
	}

	// Token: 0x04004F4B RID: 20299
	[SerializeField]
	private Image fillImage;

	// Token: 0x04004F4C RID: 20300
	[SerializeField]
	private bool useColors;

	// Token: 0x04004F4D RID: 20301
	[SerializeField]
	private Color underCapacity = Color.green;

	// Token: 0x04004F4E RID: 20302
	[SerializeField]
	private Color overCapacity = Color.red;

	// Token: 0x04004F4F RID: 20303
	[SerializeField]
	private Color atCapacity = Color.yellow;

	// Token: 0x04004F50 RID: 20304
	private float _fillAmount;
}
