using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200017C RID: 380
public class SIUIProgressBar : MonoBehaviour
{
	// Token: 0x060009F6 RID: 2550 RVA: 0x00035BB4 File Offset: 0x00033DB4
	public void UpdateFillPercent(float percentFull)
	{
		float num = this.backgroundImage.rectTransform.sizeDelta.x * (1f - 2f * this.borderPercent / 100f);
		float num2 = num * Mathf.Min(1f, percentFull);
		float num3 = -(num - num2) / 2f * this.progressImage.rectTransform.localScale.x;
		this.progressImage.rectTransform.sizeDelta = new Vector2(num2, this.progressImage.rectTransform.sizeDelta.y);
		this.progressImage.rectTransform.localPosition = new Vector3(num3, this.progressImage.rectTransform.localPosition.y, this.progressImage.rectTransform.localPosition.z);
	}

	// Token: 0x04000C4B RID: 3147
	public Image backgroundImage;

	// Token: 0x04000C4C RID: 3148
	public Image progressImage;

	// Token: 0x04000C4D RID: 3149
	public float borderPercent;

	// Token: 0x04000C4E RID: 3150
	public TextMeshProUGUI progressText;
}
